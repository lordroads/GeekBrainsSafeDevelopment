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


        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            return Ok( _bookRepository.GetAll() );
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
