using Microsoft.AspNetCore.Mvc;
using Store.G02.Services.Abstractions;
using Store.G02.Shared.Dtos.Baskets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.G02.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BasketsController(IServiceManager _serviceManager) : ControllerBase
    {

        [HttpGet] // baseUrl/api/baskets/id
        public async Task<IActionResult> GetBasketById(string id)
        {
            var basketDto = await _serviceManager.basketService.GetBasketAsync(id);
            return Ok(basketDto);
        }


        [HttpPost] // baseUrl/api/baskets
        public async Task<IActionResult> CreateOrUpdateBasket(BasketDto basketDto)
        {
            var basketdto = await _serviceManager.basketService.CreateBasketAsync(basketDto, TimeSpan.FromDays(1));
            return Ok(basketdto);
        }


        [HttpDelete] // baseUrl/api/baskets/id
        public async Task<IActionResult> DeleteBasket(string id)
        {
            var flag = await _serviceManager.basketService.DeleteBasketAsync(id);
            return NoContent();
        }


    }
}
