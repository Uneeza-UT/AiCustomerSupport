using AiCustomerSupport.Application.Contracts.Services;
using AiCustomerSupport.Application.DTOs.Common;
using AiCustomerSupport.Application.DTOs.Conversation;
using AiCustomerSupport.Application.DTOs.Identity;
using AiCustomerSupport.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace AiCustomerSupport.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ConversationController : ControllerBase
    {
        private readonly IConversationService _conversationService;

        public ConversationController(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("analysis")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<ConversationAnalysisDto>> Get()
        {
            var conversationAnalysis = await _conversationService.GetAnalysisAsync();
            return Ok(conversationAnalysis);
        }



        [HttpGet("{userId}/user")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<List<GetConversationDto>>> Get(string userId, [FromQuery] PaginationDto dto)
        {
            var conversations = await _conversationService.GetByUserAsync(dto, userId);

            if (conversations == null || conversations.Count == 0)
            {
                return Ok(new
                {
                    message = "No conversations found for the requested page.",
                    data = new List<GetConversationDto>()
                });
            }

            return Ok(conversations);
        }



        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<GetConversationDto>> Get(int id)
        {
            var conversation = await _conversationService.GetByIdAsync(id);
            return Ok(conversation);
        }



        [Authorize(Roles = "Admin")]
        [HttpGet("customers")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<List<GetUserInfo>>> GetCustomers()
        {
            var customers = await _conversationService.GetCustomersWithConversationsAsync();
            return Ok(customers);
        }




        [Authorize(Roles = "Customer")]
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(403)]
        public async Task<ActionResult> Post()
        {
            var response = await _conversationService.CreateAsync();
            return CreatedAtAction(nameof(Get), new { id = response });
        }




        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/assign")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Patch(int id, [FromBody] AssignConversationToAgentDto dto)
        {
            await _conversationService.AssignToAgentAsync(id, dto);
            return NoContent();
        }



        [Authorize(Roles = "Admin,Customer")]
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Delete(int id)
        {
            await _conversationService.DeleteAsync(id);
            return NoContent();
        }



        [HttpGet("{userId}/sort")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<List<GetConversationDto>>> Sort(string userId, [FromQuery] SortDto dto)
        {
            var conversations = await _conversationService.SortAsync(dto, userId);
            return Ok(conversations);
        }




        [HttpGet("{userId}/search")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<List<GetConversationDto>>> Search(string userId, [FromQuery] SearchDto dto)
        {
            var conversations = await _conversationService.SearchByIntentAsync(dto, userId);
            return Ok(conversations);
        }



        [HttpGet("{userId}/filter")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<List<GetConversationDto>>> Filter
            (string userId, [FromQuery] ConversationStatus status)
        {
            var conversations = await _conversationService.FilterByStatusAsync(status, userId);
            return Ok(conversations);
        }
    }
}
