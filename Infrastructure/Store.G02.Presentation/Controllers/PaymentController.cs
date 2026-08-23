using Microsoft.AspNetCore.Mvc;
using Store.G02.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.G02.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController(IServiceManager _serviceManager) : ControllerBase
    {
        [HttpPost("{basketid}")] // api/payment/{basketid}
        public async Task<IActionResult> CreatePaymentIntent(string basketId)
        {
            var basket = await _serviceManager.paymentService.CreatePaymentIntentAsync(basketId);
            return Ok(basket);
        }
    }
}
