using AiCustomerSupport.Application.Contracts.Services;
using AiCustomerSupport.Application.DTOs.Message;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiCustomerSupport.Api.Controllers
{
    [Route("api/conversation/{conversationId}/messages")]
    [ApiController]
    [Authorize]
    public class MessageController : ControllerBase
    {

        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }


        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<List<GetMessageDto>>> Get(int conversationId)
        {
            var messages = await _messageService.GetAsync(conversationId);
            return Ok(messages);
        }



        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Post(int conversationId, [FromForm] CreateMessageDto dto)
        {
            var response = await _messageService.SendAsync(conversationId, dto);
            return CreatedAtAction(nameof(Get), new { id = response });
        }
    }
}
