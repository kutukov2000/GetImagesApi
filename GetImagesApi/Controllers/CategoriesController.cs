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

            var categoryDTO = new CategoryDTO
            {
                Name = category.Name,
                Description = category.Description
            };
            return Ok(categoryDTO);
        }

        // POST: api/categories
        [HttpPost]
        public IActionResult Create([FromBody] CategoryDTO categoryDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = new CategoryEntity
            {
                Name = categoryDTO.Name,
                Description = categoryDTO.Description
            };

            _appContext.Categories.Add(category);
            _appContext.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = category.Id }, categoryDTO);
        }

        // PUT: api/categories/5
        [HttpPut("{id}")]
        public IActionResult Edit(int id, [FromBody] CategoryDTO categoryDTO)
        {
            var category = _appContext.Categories.Find(id);
            if (category == null)
                return NotFound();

            category.Name = categoryDTO.Name;
            category.Description = categoryDTO.Description;

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

            _appContext.Categories.Remove(category);
            _appContext.SaveChanges();

            var categoryDTO = new CategoryDTO
            {
                Name = category.Name,
                Description = category.Description
            };

            return Ok(categoryDTO);
        }
    }
}
