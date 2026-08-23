using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.G02.Services.Abstractions;
using Store.G02.Shared.Dtos.Auth;
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
    public class AuthController(IServiceManager _serviceManager) : ControllerBase
    {

        [HttpPost("login")] // POST api/auth/login
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var Response = await _serviceManager.authService.LoginAsync(loginRequest);
            return Ok(Response);
        }


        [HttpPost("register")] // POST api/auth/register
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            await _serviceManager.authService.RegisterAsync(registerRequest);
            return Ok("Check Your Inbox");
        }


        [HttpGet("isEmailExists")] // GET api/auth/isEmailExist?email 
        public async Task<IActionResult> IsEmailExist(string email)
        {
            var flag = await _serviceManager.authService.IsEmailExistAsync(email);
            return Ok(flag);
        }


        [HttpGet("currentUser")] // GET api/auth/currentUser
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email);
            var user = await _serviceManager.authService.GetCurrentUserAsync(userEmail.Value);
            return Ok(user);
        }


        [HttpGet("getAddress")] // GET api/auth/getAddress
        [Authorize]
        public async Task<IActionResult> GetCurrentUserAddress()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email);
            var address = await _serviceManager.authService.GetCurrentUserAddressAsync(userEmail.Value);
            return Ok(address);
        }


        [HttpPut("updateAddress")] // PUT api/auth/updateAddress
        [Authorize]
        public async Task<IActionResult> UpdateUserAddress(UserAddressDto addressDto)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email);
            var address = await _serviceManager.authService.UpdateUserAddressAsync(userEmail.Value, addressDto);
            return Ok(address);

        }


        [HttpPost("refresh-Token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            var response = await _serviceManager.authService.RefreshTokenAsync(refreshTokenRequest);
            return Ok(response);
        }


        [HttpGet("Confirm-Email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery]string email, [FromQuery] string token)
        {
            await _serviceManager.authService.ConfirmEmailAsync(email, token);
            return Ok();
        }

    }
}
