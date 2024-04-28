using AutoMapper;
using GetImagesApi.Data;
using GetImagesApi.Data.Entities;
using GetImagesApi.Data.Entities.Identity;
using GetImagesApi.dtos;
using GetImagesApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GetImagesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly MyAppContext _appContext;
        private readonly UserManager<UserEntity> _userManager;
        private readonly IMapper _mapper;

        public CategoriesController(MyAppContext appContext,
            UserManager<UserEntity> userManager,
            IMapper mapper)
        {
            _appContext = appContext;
            _userManager = userManager;
            _mapper = mapper;
        }

        private async Task<UserEntity> GetUserAuthAsync()
        {
            var email = User.Claims.FirstOrDefault().Value;
            var user = await _userManager.FindByEmailAsync(email);
            return user;
        }

        // GET: api/categories
        [HttpGet]
        public IActionResult Get()
        {
            var user = GetUserAuthAsync();
            var categories = _appContext.Categories
                .Where(u => u.UserId == user.Id)
                .Select(c => _mapper.Map<CategoryItemModel>(c))
                .ToList();
            return Ok(categories);
        }

        // GET: api/categories/5
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var category = _appContext.Categories.Find(id);
            if (category == null)
                return NotFound();

            return Ok(_mapper.Map<CategoryItemModel>(category));
        }

        // POST: api/categories
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CategoryDTO categoryDTO)
        {
            var user = GetUserAuthAsync();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = new CategoryEntity
            {
                Name = categoryDTO.Name,
                Description = categoryDTO.Description,
                UserId = user.Id,
            };

            if (categoryDTO.Image != null)
                await SaveImage(category, categoryDTO);

            _appContext.Categories.Add(category);
            _appContext.SaveChanges();

            return Created($"/api/Categories/{category.Id}", _mapper.Map<CategoryItemModel>(category));
        }

        // PUT: api/categories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromForm] CategoryDTO categoryDTO)
        {
            var user = GetUserAuthAsync();

            var category = _appContext.Categories.Find(id);
            if (category == null)
                return NotFound();

            category.Name = categoryDTO.Name;
            category.Description = categoryDTO.Description;
            category.UserId = user.Id;

            if (categoryDTO.Image != null)
                await SaveImage(category, categoryDTO);

            _appContext.Entry(category).State = EntityState.Modified;

            try
            {
                _appContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_appContext.Categories.Any(c => c.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/categories/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var user = GetUserAuthAsync();

            var category = _appContext.Categories
                .Where(x => x.UserId == id)
                .SingleOrDefault(x => x.Id == id);

            if (category == null)
                return NotFound();

            if (!string.IsNullOrEmpty(category.Image))
            {
                var imagePath = Path.Combine(Environment.CurrentDirectory, "images", category.Image);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _appContext.Categories.Remove(category);
            _appContext.SaveChanges();

            return NoContent();
        }

        private async Task SaveImage(CategoryEntity category, CategoryDTO categoryDTO)
        {
            string imageName = $"{Guid.NewGuid()}.jpg";
            string imagePath = Path.Combine(Environment.CurrentDirectory, "images", imageName);

            using (var stream = new FileStream(imagePath, FileMode.Create))
                await categoryDTO.Image.CopyToAsync(stream);

            category.Image = imageName;
        }
    }
}
