using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.G02.Services.Abstractions;
using Store.G02.Shared.Dtos.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Store.G02.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController(IServiceManager _serviceManager) : ControllerBase
    {
        [HttpPost] // POST api/orders
        [Authorize]
        public async Task<IActionResult> CreateOrder(OrderRequest orderRequest)
        {
            var userEmailClaim = User.FindFirst(ClaimTypes.Email);
            var orderResponse = await _serviceManager.orderService.CreateOrderAsync(orderRequest, userEmailClaim.Value);
            return Ok(orderResponse);
        }


        [HttpGet("{id}")] // GET api/orders/id
        [Authorize]
        public async Task<IActionResult> GetOrderByIdForSpecificUser(Guid id)
        {
            var userEmailClaim = User.FindFirst(ClaimTypes.Email);
            var orderResponse = await _serviceManager.orderService.GetOrderByIdForSpecificUserAsync(id, userEmailClaim.Value);
            return Ok(orderResponse);
        }


        [HttpGet] // GET api/orders 
        [Authorize]
        public async Task<IActionResult> GetOrdersForSpecificUser()
        {
            var userEmailClaim = User.FindFirst(ClaimTypes.Email);
            var ordersResponse = await _serviceManager.orderService.GetOrdersForSpecificUserAsync(userEmailClaim.Value);
            return Ok(ordersResponse);
        }


        [HttpGet("deliveryMethods")] // Get api/orders/deliveryMethods
        public async Task<IActionResult> GetAllDeliveryMethod()
        {
            var deliveryMethods = await _serviceManager.orderService.GetAllDeliveryMethodAsync();
            return Ok(deliveryMethods);
        }


    }
}
