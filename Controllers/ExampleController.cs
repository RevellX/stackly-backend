using Microsoft.AspNetCore.Mvc;
using StacklyBackend.Models;
using StacklyBackend.Utils.DataGenerator;

[ApiController]
[Route("api/[controller]")]
public class ExampleController : ControllerBase
{
    private static AppDbContext _context = new AppDbContext();

    public ExampleController()
    {
        _context = new AppDbContext();
    }

    [HttpGet]
    public ActionResult<IEnumerable<Example>> GetAll()
    {

        return Ok(_context.Examples);
    }

    [HttpGet("{id}")]
    public ActionResult<Example> GetById(string id)
    {
        var example = _context.Examples.FirstOrDefault(p => p.Id.Equals(id));
        return example is null ? NotFound() : Ok(example);
    }


    [HttpPost]
    public async Task<ActionResult<Example>> Create(ExampleCreate example)
    {
        string id;
        do
        {
            id = Generator.GetRandomString(StringType.Alphanumeric, StringCase.Lowercase, 10);
        } while (_context.Examples.FirstOrDefault(p => p.Id.Equals(id)) is not null);

        var newExample = new Example
        {
            Id = id,
            Name = example.Name,
            Price = example.Price
        };

        _context.Examples.Add(newExample);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Create), new { id = newExample.Id }, newExample);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, Example updated)
    {
        var example = _context.Examples.FirstOrDefault(p => p.Id.Equals(id));
        if (example is null) return NotFound();

        example.Name = updated.Name;
        example.Price = updated.Price;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var example = _context.Examples.FirstOrDefault(p => p.Id.Equals(id));
        if (example is null) return NotFound();

        _context.Examples.Remove(example);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}