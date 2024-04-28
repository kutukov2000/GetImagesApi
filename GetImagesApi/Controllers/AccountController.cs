using GetImagesApi.Data.Entities.Identity;
using GetImagesApi.Interfaces;
using GetImagesApi.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GetImagesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IJwtTokenService _jwtTokenService;

        public AccountController(UserManager<UserEntity> userManager, IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest();
            }
            var isAuth = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!isAuth)
            {
                return BadRequest();
            }
            var token = await _jwtTokenService.CreateToken(user);
            return Ok(new { token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new UserEntity { Email = model.Email, UserName = model.Username };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (model.Image != null)
                user.Image = await SaveImage(model.Image);

            if (result.Succeeded)
            {
                var token = await _jwtTokenService.CreateToken(user);
                return Ok(new { token });
            }

            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
        }

        private async Task<string> SaveImage(IFormFile image)
        {
            string imageName = $"{Guid.NewGuid()}.jpg";
            string imagePath = Path.Combine(Environment.CurrentDirectory, "images", imageName);

            using (var stream = new FileStream(imagePath, FileMode.Create))
                await image.CopyToAsync(stream);

            return imageName;
        }
    }
}
