using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MrSpex.AppServices;

namespace MrSpex.WebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SalesChannelController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SalesChannelController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<SetSalesChannel.Response> SetSalesChannel(SetSalesChannel.Command command)
        {
            return await _mediator.Send(command);
        }
    }
}