using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebSlon.Constants;
using WebSlon.Data.Entities.Identity;
using WebSlon.Helpers;
using WebSlon.Interfaces;
using WebSlon.Models.Account;

namespace WebSlon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IMapper _mapper;

        public AccountController(UserManager<UserEntity> userManager, IJwtTokenService jwtTokenService,
            IMapper mapper)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return BadRequest("Не вірно вказано дані!");
                }
                if (!await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    return BadRequest("Не вірно вказано дані!");
                }
                var token = await _jwtTokenService.CreateTokenAsync(user);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                string imageName = string.Empty;
                if(!string.IsNullOrEmpty(model.ImageBase64)) 
                {
                    var bytes = model.ImageBase64.Base64ToBytesArray();
                    imageName = await ImageWorker.SaveImageAsync(bytes);
                }
                var user = _mapper.Map<UserEntity>(model);
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    result = await _userManager.AddToRoleAsync(user, Roles.User);
                    var token = await _jwtTokenService.CreateTokenAsync(user);
                    return Ok(new { token });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
