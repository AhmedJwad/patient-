using HealthCare.API.Data;
using HealthCare.API.Data.Entities;
using HealthCare.API.Helpers;
using HealthCare.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HealthCare.API.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IuserHelper _userhelper;
        private readonly IConfiguration _configuration;
        private readonly DataContext _context;
        private readonly IMailHelper _mailHelper;
        private readonly IBlobHelper _blobHelper;

        public AccountController(IuserHelper userhelper , IConfiguration configuration , DataContext context,
            IMailHelper mailHelper, IBlobHelper blobHelper)
        {
            _userhelper = userhelper;
           _configuration = configuration;
            _context = context;
            _mailHelper = mailHelper;
            _blobHelper = blobHelper;
        }

        [HttpPost]
        [Route("CreateToken")]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userhelper.GetUserAsync(model.Username);
                if (user != null)
                {
                    Microsoft.AspNetCore.Identity.SignInResult result = await _userhelper.ValidatePasswordAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        return CreateToken(user);
                    }
                }
            }

            return BadRequest();
        }

        private IActionResult CreateToken(User user)
        {
            Claim[] claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken token = new JwtSecurityToken(
                _configuration["Tokens:Issuer"],
                _configuration["Tokens:Audience"],
                claims,
                expires: DateTime.UtcNow.AddDays(99),
                signingCredentials: credentials);
            var results = new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
                user
            };

            return Created(string.Empty, results);
        }

    }
}
