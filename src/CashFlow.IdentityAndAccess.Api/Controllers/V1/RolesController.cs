using Asp.Versioning;
using CashFlow.IdentityAndAccess.Application.Roles.Dto;
using CashFlow.IdentityAndAccess.Application.Roles.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CashFlow.IdentityAndAccess.Api.Controllers.V1
{
    /// <summary>
    /// Controller responsible for managing user roles within the identity and access context.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RolesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Returns all available roles.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<SimpleRoleDto>), 200)]
        public async Task<ActionResult<List<SimpleRoleDto>>> GetAll()
        {
            var result = await _mediator.Send(new GetRolesQuery());
            return Ok(result);
        }
    }
}