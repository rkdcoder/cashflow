using Asp.Versioning;
using CashFlow.Consolidations.Application.DailyEntries.Dto;
using CashFlow.Consolidations.Application.DailyEntries.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CashFlow.Consolidations.Api.Controllers.V1
{
    /// <summary>
    /// Provides endpoints to retrieve daily entry records and related data for cash flow tracking.
    /// </summary>
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class DailyEntriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DailyEntriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves all entries for the specified date.
        /// </summary>
        /// <param name="date">Date in format yyyy-MM-dd</param>
        /// <returns>List of entries for the given date</returns>
        [HttpGet("entries/{date}")]
        public async Task<ActionResult<IEnumerable<ConsolidationEntryDto>>> GetDailyEntries(string date)
        {
            if (!DateTime.TryParseExact(date, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var dt))
                return BadRequest("Date must be in format yyyy-MM-dd");

            var result = await _mediator.Send(new GetDailyEntriesQuery(dt));
            return Ok(result);
        }

        /// <summary>
        /// Retrieves all entries in the system.
        /// </summary>
        /// <returns>List of all entries</returns>
        [HttpGet("entries/all")]
        public async Task<ActionResult<IEnumerable<ConsolidationEntryDto>>> GetAllEntries()
        {
            var result = await _mediator.Send(new GetAllEntriesQuery());
            return Ok(result);
        }
    }
}