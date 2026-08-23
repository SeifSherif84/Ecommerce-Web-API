using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Store.G02.Domain.Entities.Identity;
using Store.G02.Domain.Exceptions.BadRequest;
using Store.G02.Domain.Exceptions.NotFound;
using Store.G02.Domain.Exceptions.ServerError;
using Store.G02.Domain.Exceptions.UnauthorizedException;
using Store.G02.Services.Abstractions.Auth;
using Store.G02.Services.MailKitFeature;
using Store.G02.Shared;
using Store.G02.Shared.Dtos.Auth;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Store.G02.Services.Auth
{

    public class AuthService(UserManager<AppUser> _userManager,
                             IOptions<JWTOptions> _options,
                             IMapper _mapper,
                             IConfiguration _configuration,
                             IMailService _mailService) : IAuthService
    {

        public async Task<bool> IsEmailExistAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email) is not null;
        }


        public async Task<UserResponse> GetCurrentUserAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null) throw new UserNotFoundException(email);
            return new UserResponse()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await GenerateJwtTokenAsync(user)
            };
        }


        public async Task<UserAddressDto?> GetCurrentUserAddressAsync(string email)
        {
            var user = await _userManager.Users.Include(U => U.Address).Where(U => U.Email == email).FirstOrDefaultAsync();
            if (user is null) throw new UserNotFoundException(email);
            return _mapper.Map<UserAddressDto?>(user.Address);
        }


        public async Task<UserAddressDto?> UpdateUserAddressAsync(string email, UserAddressDto addressDto)
        {
            var user = await _userManager.Users.Include(U => U.Address).Where(U => U.Email == email).FirstOrDefaultAsync();
            if (user is null) throw new UserNotFoundException(email);
            if(user.Address == null)
            {
                // Create New Address
                user.Address = _mapper.Map<Address>(addressDto);
            }
            else
            {
                // Update Old Address
                user.Address.FirstName = addressDto.FirstName;
                user.Address.LastName = addressDto.LastName;
                user.Address.Country = addressDto.Country;
                user.Address.City = addressDto.City;
                user.Address.Street = addressDto.Street;
            }
            await _userManager.UpdateAsync(user);
            return _mapper.Map<UserAddressDto>(user.Address);
        }


        public async Task<UserResponse?> LoginAsync(LoginRequest loginRequest)
        {
            var user = await _userManager.FindByEmailAsync(loginRequest.Email);
            if (user is null) throw new UserNotFoundException(loginRequest.Email);
            var flag = await _userManager.CheckPasswordAsync(user, loginRequest.Password);
            if (!flag) throw new UnauthorizedException();
            if (!await _userManager.IsEmailConfirmedAsync(user))
                throw new EmailConfirmationException("Must Confirm Your Email Before Login !");

            user.RefreshToken = GenerateRefreshToken(); ;
            user.RefreshTokenExpirationDate = DateTime.UtcNow.AddDays(14);
            await _userManager.UpdateAsync(user);

            return new UserResponse()
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Token = await GenerateJwtTokenAsync(user),
                RefreshToken = user.RefreshToken,
            };

        }


        public async Task RegisterAsync(RegisterRequest registerRequest)
        {
            var user = new AppUser()
            {
                UserName = registerRequest.UserName,
                Email = registerRequest.Email,
                PhoneNumber = registerRequest.PhoneNumber,
                DisplayName = registerRequest.DisplayName,
                #region OldCode
                //RefreshToken = GenerateRefreshToken(),
                //RefreshTokenExpirationDate = DateTime.UtcNow.AddDays(7) 
                #endregion
            };
            var Result = await _userManager.CreateAsync(user, registerRequest.Password);
            if ((!Result.Succeeded))
                throw new RegisterationBadRequestException(Result.Errors.Select(E => E.Description).ToList());

            var result = await SendEmailConfirmationURL(user);
            if (!result)
                throw new EmailConfirmationException("Failed to send email confirmation message.");
            #region OldCode
            // Generate JWT Token And Refresh Token Here (Implementation depends on your JWT setup)
            //return new UserResponse()
            //{
            //    Email = user.Email,
            //    DisplayName = user.DisplayName,
            //    Token = await GenerateJwtTokenAsync(user),
            //    RefreshToken = user.RefreshToken,
            //    RefreshTokenExpirationDate = user?.RefreshTokenExpirationDate ?? DateTime.UtcNow.AddDays(7)
            //}; 
            #endregion
        }


        private async Task<string> GenerateJwtTokenAsync(AppUser appUser)
        {
            var authClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.GivenName, appUser.DisplayName),
                new Claim(ClaimTypes.Email, appUser.Email),
                new Claim(ClaimTypes.NameIdentifier, appUser.Id),
                new Claim(ClaimTypes.MobilePhone, appUser.PhoneNumber)
            };

            var roles = await _userManager.GetRolesAsync(appUser);
            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var JWTOptions = _options.Value;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWTOptions.SecurityKey));

            var token = new JwtSecurityToken(
                    issuer: JWTOptions.Issuer,
                    audience: JWTOptions.Audience,
                    claims: authClaims,
                    expires: DateTime.UtcNow.AddMinutes(JWTOptions.ExpiredDurationInMinute),
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha384)
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }


        public async Task<UserResponse?> RefreshTokenAsync(RefreshTokenRequest model)
        {
            if (String.IsNullOrEmpty(model.RefreshToken))
                throw new RefreshTokenRequiredException();

            var user = await _userManager.Users.FirstOrDefaultAsync(user => user.RefreshToken == model.RefreshToken);
            if (user is null || user.RefreshTokenExpirationDate <= DateTime.UtcNow) 
                throw new UnauthorizedException();

            user.RefreshToken = GenerateRefreshToken();
            // user.RefreshTokenExpirationDate = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return new UserResponse()
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Token = await GenerateJwtTokenAsync(user),
                RefreshToken = user.RefreshToken,
            };
        }



        public async Task<bool> SendEmailConfirmationURL(AppUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = System.Web.HttpUtility.UrlEncode(token);
            var callbackUrl = $"{_configuration["BaseURL"]}/{_configuration["EmailConfirmationURL"]}?email={user.Email}&token={encodedToken}";

            var Email = new Email()
            {
                To = user?.Email ?? string.Empty,
                Subject = "EmailConfirmation",
                Body = callbackUrl
            };

            var result = _mailService.SendMail(Email);
            return result;      
        }


        public async Task ConfirmEmailAsync(string? email, string? token)
        {
            if (email is null || token is null)
                throw new EmailConfirmationException("Invalid Email Confirmation");
            AppUser? user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                throw new UserNotFoundException(email);

            var decodedToken = System.Web.HttpUtility.UrlDecode(token);
            var Result = await _userManager.ConfirmEmailAsync(user, decodedToken);
            if (!Result.Succeeded)
                throw new ServerErrorExceptionList(Result.Errors.Select(error => error.Description).ToList());
        }
    }
}


