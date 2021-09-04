using System;
using System.Threading.Tasks;
using EuroBooks.Application.Subscribtion.Commands;
using EuroBooks.Application.Subscribtion.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EuroBooks.API.Controllers
{
    /// <summary>
    /// Manages User subscriptions 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ApiController 
    {
        /// <summary>
        /// Gets all existng and active subscriptions for a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("list/{userId}", Order = 1)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Subscriptions(long userId)
        {
            try
            {
                var response = await Mediator.Send(new GetUserSubscriptionsQuery());

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        /// <summary>
        /// Creates a subscription for a user
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost(Order = 2)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Subscribe([FromBody] CreateSubscriptionCommand command)
        {
            var response = await Mediator.Send(command);

            return Ok(response);
        }

        /// <summary>
        /// Unsubscribes a user from a book
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPut("{id}/unsubscribe/{userId}", Order = 3)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Unsubscribe(long Id, long userId)
        {
            var response = await Mediator.Send(new UnsubscribeCommand { UserId = userId , Id = Id});
            if (!response)
                return BadRequest("Unable to update Book");

            return Ok(response);
        }
    }
}