using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.OData;
using EuroBooks.Application.Book.Commands;
using EuroBooks.Application.Book.Queries;
using EuroBooks.Application.Common.Interfaces;
using EuroBooks.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EuroBooks.API.Controllers
{
    /// <summary>
    /// Endpoints to manage Book entities
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ApiController
    {
        private readonly IDateTime dateTime;

        public BookController(IDateTime dateTime)
        {
            this.dateTime = dateTime;
        }


        /// <summary>
        /// Get list of all Books
        /// </summary>
        /// <returns></returns>
        [EnableQuery]
        [HttpPost("list", Order = 1)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetList([FromForm] DataTableParams param)
        {
            try
            {
                PagingInfo paging = new PagingInfo(param);

                var response = await Mediator.Send(new GetBookListQuery { paging = paging });

                return Ok(new
                {
                    param.draw,
                    recordsTotal = paging.resultCount,
                    recordsFiltered = paging.resultCount,
                    data = response
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        /// <summary>
        /// Add a new Book
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost(Order = 2)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] CreateBookCommand command)
        {
            var response = await Mediator.Send(command);

            return Ok(response);
        }

        /// <summary>
        /// Update a Book
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPut(Order = 3)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put([FromBody] UpdateBookCommand command)
        {
            var response = await Mediator.Send(command);
            if (!response)
                return BadRequest("Unable to update Book");

            return Ok(response);
        }

        /// <summary>
        /// Get a Book
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Order = 4)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(long id)
        {
            try
            {
                var response = await Mediator.Send(new GetBookQuery {Id = id });
                if (response == null)
                    return NotFound();

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

        /// <summary>
        /// Delete a Book
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}", Order = 5)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(long id)
        {
            var response = await Mediator.Send(new DeleteBookCommand { Id = id });
            if (!response)
                return BadRequest("Unable to delete Book");

            return NoContent();
        }

    }
}