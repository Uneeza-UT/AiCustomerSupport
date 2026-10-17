using AiCustomerSupport.Application.Contracts.Services;
using AiCustomerSupport.Application.DTOs.Common;
using AiCustomerSupport.Application.DTOs.Intent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace AiCustomerSupport.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class IntentController : ControllerBase
    {
        private readonly IIntentService _intentService;

        public IntentController(IIntentService intentService)
        {
            _intentService = intentService;
        }



        [Authorize(Roles = "Admin,Agent")]
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<List<GetIntentDto>>> Get([FromQuery] PaginationDto dto)
        {
            var intents = await _intentService.GetAsync(dto);

            if (intents == null || intents.Count == 0)
            {
                return Ok(new
                {
                    message = "No intents found for the requested page.",
                    data = new List<GetIntentDto>()
                });
            }

            return Ok(intents);
        }



        [Authorize(Roles = "Admin,Agent")]
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<GetIntentDto>> Get(int id)
        {
            var intent = await _intentService.GetByIdAsync(id);
            return Ok(intent);
        }



        [HttpGet("sort")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<List<GetIntentDto>>> Sort([FromQuery] SortDto dto)
        {
            var intents = await _intentService.SortAsync(dto);
            return Ok(intents);
        }




        [HttpGet("search")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<List<GetIntentDto>>> Search([FromQuery] SearchDto dto)
        {
            var intents = await _intentService.SearchAsync(dto);
            return Ok(intents);
        }
    }
}
