using GetImagesApi.Data;
using GetImagesApi.Data.Entities;
using GetImagesApi.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GetImagesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : Controller
    {
        private readonly MyAppContext _appContext;

        public CategoriesController(MyAppContext appContext)
        {
            _appContext = appContext;
        }

        // GET: api/categories
        [HttpGet]
        public IActionResult Get()
        {
            var categories = _appContext.Categories.ToList();
            return Ok(categories);
        }

        // GET: api/categories/5
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var category = _appContext.Categories.Find(id);
            if (category == null)
                return NotFound();

            return Ok(category);
        }

        // POST: api/categories
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CategoryDTO categoryDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = new CategoryEntity
            {
                Name = categoryDTO.Name,
                Description = categoryDTO.Description
            };

            if (categoryDTO.Image != null)
            {
                var imageName = $"{Guid.NewGuid()}.jpg";
                var imagePath = Path.Combine(Environment.CurrentDirectory, "images", imageName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await categoryDTO.Image.CopyToAsync(stream);
                }

                category.Image = imageName;
            }

            _appContext.Categories.Add(category);
            _appContext.SaveChanges();

            return Created($"/api/Categories/{category.Id}", category);
        }

        // PUT: api/categories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromForm] CategoryDTO categoryDTO)
        {
            var category = _appContext.Categories.Find(id);
            if (category == null)
                return NotFound();

            category.Name = categoryDTO.Name;
            category.Description = categoryDTO.Description;

            if (categoryDTO.Image != null)
            {
                string imageName = $"{Guid.NewGuid()}.jpg";
                string imagePath = Path.Combine(Environment.CurrentDirectory, "images", imageName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await categoryDTO.Image.CopyToAsync(stream);
                }

                category.Image = imageName;
            }

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
            var category = _appContext.Categories.Find(id);
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
    }
}
