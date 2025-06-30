using Asp.Versioning;
using CashFlow.Entries.Application.Entries.Command;
using CashFlow.Entries.Application.Entries.Dto;
using CashFlow.Entries.Application.Entries.Query;
using CashFlow.Shared.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CashFlow.Entries.Api.Controllers.V1
{
    /// <summary>
    /// Controller responsible for managing cash flow entries (debits and credits).
    /// </summary>
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class EntriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EntriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Creates a new cash flow entry.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CommandResultDto<Guid>>> Create([FromBody] CreateEntryCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing cash flow entry.
        /// </summary>
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<CommandResultDto<bool>>> Update(Guid id, [FromBody] UpdateEntryDto dto)
        {
            var command = new UpdateEntryCommand(id, dto.Amount, dto.Description, dto.CategoryId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a cash flow entry.
        /// </summary>
        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult<CommandResultDto<bool>>> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteEntryCommand(id));
            return Ok(result);
        }

        /// <summary>
        /// Gets a list of entry IDs between the given date range (inclusive).
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Guid>>> GetIdsByDateRange([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var startUtc = DateTime.SpecifyKind(start.Date, DateTimeKind.Utc);
            var endUtc = DateTime.SpecifyKind(end.Date.AddDays(1).AddMilliseconds(-1), DateTimeKind.Utc);

            var ids = await _mediator.Send(new GetEntryIdsByDateRangeQuery(startUtc, endUtc));

            return Ok(ids);
        }

        /// <summary>
        /// Gets the details of a specific entry by its ID.
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EntryDto>> GetById(Guid id)
        {
            var entry = await _mediator.Send(new GetEntryByIdQuery(id));
            return Ok(entry);
        }
    }
}