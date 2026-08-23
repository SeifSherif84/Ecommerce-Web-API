using Store.G02.Domain.Entities.Identity;
using Store.G02.Shared.Dtos.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.G02.Services.Abstractions.Auth
{
    public interface IAuthService
    {
        public Task<UserResponse?> LoginAsync(LoginRequest loginRequest);
        public Task<UserResponse?> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
        public Task RegisterAsync(RegisterRequest registerRequest);
        public Task<bool> IsEmailExistAsync(string email);
        public Task<UserResponse> GetCurrentUserAsync(string email);
        public Task<UserAddressDto?> GetCurrentUserAddressAsync(string email);
        public Task<UserAddressDto?> UpdateUserAddressAsync(string email, UserAddressDto addressDto);
        public Task<bool> SendEmailConfirmationURL(AppUser user);
        public Task ConfirmEmailAsync(string email, string token);
    }


}
