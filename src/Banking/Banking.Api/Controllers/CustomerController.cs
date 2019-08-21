using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Banking.Application.Commands;
using Banking.Application.Commands.Customers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Banking.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {

        readonly IMediator _mediator;
        public CustomerController(IMediator mediator)
        {
            _mediator=mediator;
        }


        [HttpPost, Route("[action]")]
        public async Task<ActionResult> AddCustomer([FromBody] AddCustomerCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

       





    }
}
