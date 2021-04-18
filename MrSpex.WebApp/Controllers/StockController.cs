using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MrSpex.AppServices;
using MrSpex.AppServices.ReadModel;

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
            return await _mediator.Send(command);
        }

        [HttpGet]
        public async Task<GetStockPerSalesChannel.Response> GetStockPerSalesChannel(GetStockPerSalesChannel.Query query)
        {
            return await _mediator.Send(query);
        }
    }
}