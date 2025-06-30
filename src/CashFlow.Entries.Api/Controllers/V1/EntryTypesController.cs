using Asp.Versioning;
using CashFlow.Entries.Application.Entries.Dto;
using CashFlow.Entries.Application.Entries.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CashFlow.Entries.Api.Controllers.V1
{
    /// <summary>
    /// Controller responsible for managing entry types.
    /// </summary>
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class EntryTypesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EntryTypesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Returns all entry types.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EntryTypeDto>>> GetAll()
        {
            var types = await _mediator.Send(new GetAllEntryTypesQuery());
            return Ok(types);
        }
    }
}