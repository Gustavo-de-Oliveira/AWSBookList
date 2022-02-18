using Amazon.DynamoDBv2.Model;
using AWSBookList.Database;
using AWSBookList.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
            if (!result.Any()) return NotFound();
            else return Ok(result.FirstOrDefault());
        }

        [HttpGet]
        [Route("author/{Author}")]
        public async Task<IActionResult> GetByAuthor(String author)
        {
            var result = await this.bookRepository.GetByAuthor(author);

            return Ok(result);
        }

        [HttpGet]
        [Route("genre/{Genre}")]
        public async Task<IActionResult> GetByGenre(String genre)
        {
            var result = await this.bookRepository.GetByGenre(genre);

            return Ok(result);
        }

        [HttpGet]
        [Route("title/{Title}")]
        public async Task<IActionResult> GetByTitle(String title)
        {
            var result = await this.bookRepository.GetByTitle(title);

            return Ok(result);
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
