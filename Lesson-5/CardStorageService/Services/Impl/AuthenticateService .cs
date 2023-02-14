using AutoMapper;
using CardStorageService.Data;
using CardStorageService.Data.Models;
using CardStorageService.Models;
using CardStorageService.Models.Dto;
using CardStorageService.Models.Requests;
using CardStorageService.Models.Responses;
using CardStorageService.Utils;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CardStorageService.Services.Impl;

public class AuthenticateService : IAuthenticateService
{
    private readonly Dictionary<string, SessionDto> _sessions = new Dictionary<string, SessionDto>();
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public AuthenticateService(IServiceScopeFactory serviceScopeFactory, IConfiguration configuration, IMapper mapper)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _configuration = configuration;
        _mapper = mapper;
    }

    public AuthenticationResponse Login(AuthenticationRequest authenticationRequest)
    {
        using IServiceScope serviceScope = _serviceScopeFactory.CreateScope();
        CardStorageServiceDbContext context = serviceScope.ServiceProvider.GetRequiredService<CardStorageServiceDbContext>();

        Account account = FindAccountByLogin(context, authenticationRequest.Login);

        if (account == null)
        {
            return new AuthenticationResponse
            {
                Status = AuthenticationStatus.UserNotFound
            };
        }

        if (!PasswordUtil.VerifyPassword(authenticationRequest.Password, account.PasswordSalt, account.PasswordHash, _configuration["SECRET_KEY"]))
        {
            return new AuthenticationResponse { Status = AuthenticationStatus.InvalidPassword };
        }

        AccountSession session = new AccountSession
        {
            AccountId = account.AccountId,
            SessionToken = CreateSessionToken(account),
            TimeCreated = DateTime.Now,
            TimeLastRequest = DateTime.Now,
            IsClosed = false
        };

        context.AccountSessions.Add(session);
        context.SaveChanges();

        SessionDto sessionDto = GetSessionDto(account, session);

        lock (_sessions)
        {
            _sessions[session.SessionToken] = sessionDto;
        }

        return new AuthenticationResponse
        {
            Status = AuthenticationStatus.Success,
            Session = sessionDto
        };
    }

    public AuthenticationResponse Logout(string sessionToken)
    {
        if (string.IsNullOrEmpty(sessionToken))
        {
            return new AuthenticationResponse 
            { 
                Status = AuthenticationStatus.Failure,
                Message = "Token is null or empty"
            };

        }

        lock (_sessions)
        {
            _sessions.Remove(sessionToken);
        }

        using IServiceScope scope = _serviceScopeFactory.CreateScope();
        CardStorageServiceDbContext context = scope.ServiceProvider.GetRequiredService<CardStorageServiceDbContext>();

        AccountSession accountSession = context.AccountSessions.FirstOrDefault(item => item.SessionToken == sessionToken);

        if (accountSession is null)
        {
            return new AuthenticationResponse
            {
                Status = AuthenticationStatus.Success,
                Message = "Token is not exist in database."
            };
        }

        accountSession.IsClosed = true;

        context.AccountSessions.Update(accountSession);
        context.SaveChanges();

        return new AuthenticationResponse
        {
            Status = AuthenticationStatus.Success,
            Message = "Session is closed."
        };
    }

    public AuthenticationResponse GetSession(string sessionToken)
    {
        SessionDto sessionDto;

        lock (_sessions)
        {
            _sessions.TryGetValue(sessionToken, out sessionDto);
        }

        if (sessionDto == null)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            CardStorageServiceDbContext context = scope.ServiceProvider.GetRequiredService<CardStorageServiceDbContext>();

            AccountSession session = context.AccountSessions.FirstOrDefault(item => item.SessionToken == sessionToken);

            if (session == null || session.IsClosed)
            {
                return new AuthenticationResponse 
                {
                    Status = AuthenticationStatus.Failure,
                    Message = "Session is null or closed."
                };

            }

            Account account = context.Accounts.FirstOrDefault(account => account.AccountId == session.AccountId);

            sessionDto = GetSessionDto(account, session);

            lock (_sessions)
            {
                _sessions[sessionToken] = sessionDto;
            }
        }

        return new AuthenticationResponse 
        {
            Status = AuthenticationStatus.Success,
            Session = sessionDto
        };
    }

    public AuthenticationResponse Registration(AuthenticationRequest authenticationRequest)
    {
        using IServiceScope serviceScope = _serviceScopeFactory.CreateScope();
        CardStorageServiceDbContext context = serviceScope.ServiceProvider.GetRequiredService<CardStorageServiceDbContext>();

        Account account = FindAccountByLogin(context, authenticationRequest.Login);

        if (account is null)
        {
            var dataPassword = PasswordUtil.CreatePasswordHash(authenticationRequest.Password, _configuration["SECRET_KEY"]);

            account = new Account
            {
                EMail = authenticationRequest.Login,
                Locked= false,
                FirstName = "None",
                LastName = "None",
                SecondName = "None",
                PasswordSalt = dataPassword.passwordSalt,
                PasswordHash = dataPassword.passwordHash
            };

            context.Accounts.Add(account);
            context.SaveChanges();

            account = FindAccountByLogin(context, authenticationRequest.Login);

            AccountSession session = new AccountSession
            {
                AccountId = account.AccountId,
                SessionToken = CreateSessionToken(account),
                TimeCreated = DateTime.Now,
                TimeLastRequest = DateTime.Now,
                IsClosed = false
            };

            context.AccountSessions.Add(session);
            context.SaveChanges();

            SessionDto sessionDto = GetSessionDto(account, session);

            lock (_sessions)
            {
                _sessions[session.SessionToken] = sessionDto;
            }

            return new AuthenticationResponse
            {
                Status = AuthenticationStatus.Success,
                Session = sessionDto
            };
        }

        return new AuthenticationResponse
        {
            Status = AuthenticationStatus.AccountAlreadyExists
        };
    }



    #region Private Methods

    private Account FindAccountByLogin(CardStorageServiceDbContext context, string login)
    {
        return context.Accounts.FirstOrDefault(account => account.EMail == login);
    }

    private string CreateSessionToken(Account account)
    {
        JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        byte[] key = Encoding.ASCII.GetBytes(_configuration["SECRET_KEY"]);

        SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor();
        securityTokenDescriptor.Subject = new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, account.EMail),
            new Claim(ClaimTypes.NameIdentifier, account.AccountId.ToString())
        });
        securityTokenDescriptor.Expires = DateTime.UtcNow.AddMinutes(15);
        securityTokenDescriptor.SigningCredentials =
            new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);

        SecurityToken securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
        return jwtSecurityTokenHandler.WriteToken(securityToken);
    }

    private SessionDto GetSessionDto(Account account, AccountSession accountSession)
    {
        accountSession.Account = account;

        return _mapper.Map<SessionDto>(accountSession);
    }

    #endregion
}