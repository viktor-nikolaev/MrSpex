using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MrSpex.AppServices;

namespace MrSpex.WebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StockController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StockController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<SetStock.Response>> SetStock(SetStock.Command command)
        {
            var response = await _mediator.Send(command);
            return response;
        }
    }
}