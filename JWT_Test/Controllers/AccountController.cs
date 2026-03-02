using JWT_Test.Data;
using JWT_Test.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JWT_Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(Dtonew dt)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new()
                {
                    UserName = dt.UserName,
                    Email = dt.Email
                };
                IdentityResult result = await _userManager.CreateAsync(user, dt.Password);

                if (result.Succeeded)
                {
                    return Ok("User Created");
                }
                else
                {
                    return BadRequest(result.Errors);
                }
                
            }
            return Ok();
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(Dtologin dt)
        {
            AppUser user = await _userManager.FindByEmailAsync(dt.email);


            if (user != null && await _userManager.CheckPasswordAsync(user, dt.password))
            {
                var claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, user.UserName));
                claims.Add(new Claim(ClaimTypes.Email, user.Email));
                // claims.Add(new Claim(JWTRegisteredClaimn))


                var roles = await _userManager.GetRolesAsync(user);
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
                }

                var key = new SymmetricSecurityKey(
     Encoding.UTF8.GetBytes(_configuration["JWT:Key"])
 );


                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:Issuer"],
                    audience: _configuration["JWT:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(
                        Convert.ToDouble(_configuration["JWT:ExpireMinutes"])
                    ),
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );
                var _token = new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                };
                return Ok(_token);
            }
            return Ok();
        }
    }
}
