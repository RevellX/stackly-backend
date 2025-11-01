using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StacklyBackend.Models;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : Controller
{
    private static AppDbContext _context = new AppDbContext();

    public CategoryController()
    {
        _context = new AppDbContext();
    }

    [HttpGet]
    public ActionResult<IEnumerable<Category>> GetAll(
        string? byName,
        string? byDisplayName
        )
    {
        var query = _context.Categories
        .AsEnumerable();

        if (!string.IsNullOrWhiteSpace(byName))
            query = query.Where(i => i.Name != null && i.Name.Contains(byName, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(byDisplayName))
            query = query.Where(i => i.DisplayName != null && i.Name.Contains(byDisplayName, StringComparison.OrdinalIgnoreCase));

        var items = query.ToList();
        return Ok(items);
    }

    [HttpGet]
    [Route("{categoryName}")]
    public ActionResult<Category> Get([FromRoute] string categoryName)
    {
        var dbCategory = _context.Categories.FirstOrDefault(p => p.Name.Equals(categoryName));
        if (dbCategory is null)
            return NotFound();
        return Ok(dbCategory);
    }

    [HttpPost]
    public async Task<ActionResult<Category>> Create([FromBody] Category category)
    {
        var dbCategory = _context.Items.FirstOrDefault(p => p.Name.Equals(category.Name));
        if (dbCategory is not null)
            return Conflict(new { message = $"Category '{category.Name}' already exists." });

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { categoryName = category.Name }, category);
    }

    [HttpPut]
    [Route("{categoryName}")]
    public async Task<ActionResult<Category>> Edit([FromBody] CategoryEdit category, [FromRoute] string categoryName)
    {
        categoryName = categoryName.ToLower();
        var dbCategory = _context.Categories.FirstOrDefault(p => p.Name.Equals(categoryName));
        if (dbCategory is null)
            return NotFound();
        dbCategory.DisplayName = category.DisplayName;
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Edit), new { id = dbCategory.Name }, dbCategory);
    }

    [HttpDelete]
    [Route("{categoryName}")]
    public ActionResult<Item> Delete([FromRoute] string categoryName)
    {
        var dbCategory = _context.Categories.Where(p => p.Name.Equals(categoryName)).ExecuteDelete();
        return Ok(dbCategory);
    }
}