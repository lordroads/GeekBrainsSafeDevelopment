using CardStorageService.Models.MongoModels;
using CardStorageService.Services;
using Microsoft.AspNetCore.Mvc;

namespace CardStorageService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : Controller
    {
        private readonly IBookRepository _bookRepository;


        public BookController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        [HttpGet("search/mongo")]
        public IActionResult SearchMongo([FromQuery] string text)
        {
            return Ok(_bookRepository.FullTextSearchMongo(text));
        }

        [HttpGet("search/elastic")]
        public IActionResult SearchElasticPrincipal([FromQuery] string text)
        {
            List<Book> result = new List<Book>();

            foreach (var item in text.Split(' '))
            {
                if (item is not null)
                {
                    var serche = _bookRepository.FullTextSearchElasticPrincipal(item.ToLower());

                    foreach (var s in serche)
                    {
                        if (!result.Contains(s))
                        {
                            result.Add(s);
                        }
                    }
                }
            }

            return Ok(result);
        }

        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            return Ok( _bookRepository.GetAll() );
        }

        [HttpGet("get-all-words")]
        public IActionResult GetAllWords()
        {
            try
            {
                return Ok(_bookRepository.GetAllWords());

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("get-all-words-to-books")]
        public IActionResult GetAllWordsToBooks()
        {
            return Ok(_bookRepository.GetAllWordsToBooks());
        }

        [HttpGet("{id}")]
        public IActionResult Get([FromRoute]int id)
        {
            return Ok( _bookRepository.GetById(id) ?? new Book() );
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] Book data)
        {
            return Ok( _bookRepository.Create(data) );
        }

        [HttpPut("{id}")]
        public IActionResult Update([FromBody] Book data)
        {
            return Ok( _bookRepository.Update(data) );
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Book data)
        {
            return Ok( _bookRepository.Delete(data) );
        }
    }
}
