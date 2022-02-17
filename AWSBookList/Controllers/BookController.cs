using AWSBookList.Database;
using AWSBookList.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AWSBookList.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly BookRepository bookRepository;
        public BookController(BookRepository bookRepository)
        {
            this.bookRepository = bookRepository;
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Save(Book book)
        {
            book.IdBook = Guid.NewGuid();
            await this.bookRepository.Save(book);

            return Created($"/{book.IdBook}", book);
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetAll()
        {
            var result = await this.bookRepository.GetAll();

            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await this.bookRepository.GetById(id);
            if(!result.Any()) return NotFound();
            else return Ok(result.FirstOrDefault());
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Edit(Book book)
        {
            var result = await this.bookRepository.GetById(book.IdBook);

            await this.bookRepository.Edit(book);

            return Ok(result.FirstOrDefault());
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await this.bookRepository.GetById(id);

            if (!result.Any()) return NotFound();
            else
            {
                await this.bookRepository.Delete(result.FirstOrDefault());

                return Ok(result.FirstOrDefault());
            }
        }

    }
}
