using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebSlon.Data;
using WebSlon.Data.Entities;
using WebSlon.Helpers;
using WebSlon.Models.Category;

namespace WebSlon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppEFContext _appEFContext;
        private readonly IMapper _mapper;

        public CategoriesController(AppEFContext context, IMapper mapper)
        {
            _appEFContext = context;
            _mapper = mapper;
        }

        [HttpGet("list")]
        public async Task<IActionResult> Index()
        {
            var model = await _appEFContext.Categories
                .Where(x=>x.IsDeleted==false)
                .Select(x=> _mapper.Map<CategoryItemViewModel>(x))
                .ToListAsync();

            return Ok(model);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cat = await _appEFContext.Categories
                 .Where(x => x.IsDeleted == false)
                .SingleOrDefaultAsync(x => x.Id == id);
            if (cat is null)
                return NotFound();
            var model = _mapper.Map<CategoryItemViewModel>(cat);
            return Ok(model);
        }
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] CategoryCreateViewModel model)
        {
            var cat = _mapper.Map<CategoryEntity>(model);
            if (model.Image != null)
            {
                cat.Image = await ImageWorker.SaveImageAsync(model.Image);
            }

            await _appEFContext.Categories.AddAsync(cat);
            await _appEFContext.SaveChangesAsync();
            return Ok(_mapper.Map<CategoryItemViewModel>(cat));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var cat = await _appEFContext.Categories
                 .Where(x => x.IsDeleted == false)
                .SingleOrDefaultAsync(x=>x.Id == id);
            if (cat is null)
                return NotFound();

            cat.IsDeleted = true;
            _appEFContext.SaveChanges();
            return Ok();
        }

        [HttpPut("edit/{id}")]
        public async Task<IActionResult> Edit(int id, [FromForm] CategoryCreateViewModel model)
        {
            var cat = await _appEFContext.Categories
                .Where(x => x.IsDeleted == false)
                .SingleOrDefaultAsync(x=>x.Id == id);

            if (cat is null)
                return NotFound();

            if (model.Image != null)
            {
                ImageWorker.DeleteImage(cat.Image);
                cat.Image = await ImageWorker.SaveImageAsync(model.Image);
            }
            cat.Name = model.Name;
            cat.Description = model.Description;

            await _appEFContext.SaveChangesAsync();
            return Ok(_mapper.Map<CategoryItemViewModel>(cat));
        }
    }
}
