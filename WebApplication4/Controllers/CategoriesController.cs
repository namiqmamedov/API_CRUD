using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication4.DAL;
using WebApplication4.DTOs.CategoryDTOs;
using WebApplication4.Entities;

namespace WebApplication4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _context.Categories.Where(c => c.IsDeleted == false)
                .Select(x => new CategoryForListDto
                {
                    Id = x.ID,
                    Name = x.Name
                })
                .ToListAsync());
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.IsDeleted == false && c.ID == id);

            if (category == null)
            {
                return BadRequest();
            }

            return Ok(category);
        }


        [HttpPut]
        [Route("id")]

        public async Task<IActionResult> Put(int? id, Category category)
        {
            if (id == null)
            {
                return BadRequest();
            }

            if (category.ID != id)
            {
                return BadRequest();
            }

            Category existedCategory = await _context.Categories.FirstOrDefaultAsync(c => c.IsDeleted == false && c.ID == id);

            if (existedCategory == null)
            {
                return NotFound();
            }

            if (await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.Name.ToLower() == category.Name.Trim().ToLower() && c.ID != id))
            {
                return Conflict();
            }

            existedCategory.Name = category.Name.Trim();
            existedCategory.UpdatedAt = DateTime.UtcNow;
            existedCategory.UpdatedBy = "System";

            await _context.SaveChangesAsync();

            return NotFound(); 
        }

        [HttpPost]
        public async Task<IActionResult> Post(Category category)
        {
            if (await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.Name.ToLower() == category.Name.Trim().ToLower()))
            {
                return Conflict($"Name: {category.Name} is already exists");
            }

            category.CreatedAt = DateTime.Now;
            category.CreatedBy = "System";
            category.Name = category.Name.Trim();

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpDelete]
        [Route("{id}")]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.IsDeleted == false && c.ID == id);

            if (category == null)
            {
                return NotFound();
            }

            category.DeletedAt = DateTime.UtcNow;
            category.DeletedBy = "System";
            category.IsDeleted = true;

            await _context.SaveChangesAsync();

            return NotFound();
        }
    }
}
