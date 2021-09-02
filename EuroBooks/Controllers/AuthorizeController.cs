using System;
using System.Text;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using EuroBooks.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using EuroBooks.Application.Common.Interfaces;
using EuroBooks.API.Models.Account;
using System.Collections.Generic;

namespace EuroBooks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizeController : ApiController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        //private readonly RoleManager<ApplicationRole> roleManager;
        private readonly IJwtConfiguration jwtConfiguration;
        private readonly IDateTime dateTime;

        public AuthorizeController(UserManager<ApplicationUser> userManager
            //, RoleManager<ApplicationRole> roleManager
            , SignInManager<ApplicationUser> signInManager
            , IJwtConfiguration jwtConfiguration
            , IDateTime dateTime
            )
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            //this.roleManager = roleManager;
            this.jwtConfiguration = jwtConfiguration;
            this.dateTime = dateTime;
        }

        /// <summary>
        /// Login user
        /// </summary>
        /// <param name="model"></param>
        /// <returns>JWT Token</returns>
        [HttpPost("login", Order = 1)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await userManager.FindByNameAsync(model.Username);
            if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim("FirstName", user.FirstName),
                    new Claim("LastName", user.LastName),
                    new Claim("Email", user.Email)
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.Secret));

                var token = new JwtSecurityToken(
                    issuer: jwtConfiguration.Issuer,
                    audience: jwtConfiguration.Audience,
                    expires: DateTime.UtcNow.AddHours(5),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                //set value indicating whether session is persisted and the time at which the authentication was issued
                var authenticationProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    IssuedUtc = DateTime.UtcNow,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(15) //expire time
                };


                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    userName = user.UserName,
                    name = user.FirstName,
                    lastName = user.LastName,
                    roles = string.Join(',', userRoles),
                    userId = user.Id
                });
            }
            return Unauthorized();
        }

        /// <summary>
        /// Send email to reset password
        /// </summary>
        /// <param name="model"></param>
        /// <returns>No content</returns>
        [HttpPost("password/forgot", Order = 2)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest("Could not found user");

            // Send email
            //var command = new ResetUserPasswordEmailCommand { Email = model.Email };
            //await Mediator.Send(command);

            return Ok("Email has been sent to reset password");
        }

        /// <summary>
        /// Update password
        /// </summary>
        /// <param name="model"></param>
        /// <returns>No content</returns>
        [HttpPut("password/update", Order = 3)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePassword([FromBody] ResetPasswordModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return NotFound("Could not found user");

            if (model.Password != model.ConfirmPassword)
                return BadRequest("Password and confirm password do not match");

            var hashPwd = userManager.PasswordHasher.HashPassword(user, model.Password);
            user.PasswordHash = hashPwd;
            user.LastModified = dateTime.Now;

            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded) return NoContent();
            else return BadRequest("Unable to update password");
        }

        /// <summary>
        /// Log user out
        /// </summary>
        /// <returns></returns>
        [HttpPost("logout", Order = 4)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            return NoContent();
        }
    }
}