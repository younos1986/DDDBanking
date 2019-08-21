using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Banking.Application.Commands;
using Banking.Application.Commands.Accounts;
using Banking.Application.Queries.Accounts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Banking.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        readonly IMediator _mediator;
        public AccountController(IMediator mediator)
        {
            _mediator=mediator;
        }


        [HttpPost, Route("[action]")]
        public async Task<ActionResult> AddAccount([FromBody] AddAccountCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost, Route("[action]")]
        public async Task<ActionResult> Deposit([FromBody] DepositCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost, Route("[action]")]
        public async Task<ActionResult> Withdraw([FromBody] WithdrawCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost , Route("[action]")]
        public async Task<ActionResult> CloseAccount([FromBody] CloseAccountCommand command)
        {
            var result =  await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost, Route("[action]")]
        public async Task<ActionResult> GetAccountStatus([FromBody] AccountStatusQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
