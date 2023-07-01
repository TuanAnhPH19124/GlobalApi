using GlobalApi.DataTransfer;
using GlobalApi.Ultilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GlobalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthenticationController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration)
        {
            _userManager=userManager;
            _signInManager=signInManager;
            _configuration=configuration;
        }

        [AllowAnonymous]
        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] LoginRequestDto user)
        {
            if (ModelState.IsValid)
            {
                var user_exits = await _userManager.FindByEmailAsync(user.Email);

                if (user_exits != null)
                {
                    var is_correct = await _userManager.CheckPasswordAsync(user_exits, user.Password);

                    if (is_correct)
                    {
                        var result = await _signInManager.PasswordSignInAsync(user_exits, user.Password, false, false);
                        if (result.Succeeded)
                        {
                            var roles = await _userManager.GetRolesAsync(user_exits);
                            
                            var token = JwtService.GenerateJwtToken(user_exits,roles ,_configuration);
                            return Ok(new { 
                                Message = "dang nhap thang cong",
                                Token = token
                            });
                        }

                        return BadRequest("Dang nhap that bai");
                    }

                    return BadRequest("Mat khau chua chinh xac");
                }

                return NotFound("Email khong ton tai");
            }

            return BadRequest();
        }

        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignInRequestDto new_user)
        {
            if (ModelState.IsValid)
            {
                var userExists = await _userManager.FindByEmailAsync(new_user.Email);
                if (userExists == null)
                {
                    var newUser = new IdentityUser()
                    {
                        Email = new_user.Email,
                        UserName = new_user.UserName
                    };
                    var newUserResponse = await _userManager.CreateAsync(newUser, new_user.Password);
                    if (newUserResponse.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(newUser, "Admin");
                        var roles = await _userManager.GetRolesAsync(newUser);
                        var token = JwtService.GenerateJwtToken(newUser, roles,_configuration);
                        return Ok(new { 
                           Message = "dang ki thanh cong",
                           Token = token
                        });
                    }
                    return BadRequest(newUserResponse.Errors);
                }
                return BadRequest("Tai khoan da ton tai");
            }
            return BadRequest("Khong duoc bo trong");
        }

        
    }
}
