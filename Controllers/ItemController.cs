using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StacklyBackend.Models;
using StacklyBackend.Utils.DataGenerator;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ItemController : ControllerBase
{
    private static AppDbContext _context = new AppDbContext();

    public ItemController()
    {
        _context = new AppDbContext();
    }

    [HttpGet]
    public ActionResult<IEnumerable<Item>> GetAll(
        string? byId,
        string? byCategory,
        string? byName,
        string? byDescription,
        string? byQuantity,
        string? byMinQuantity,
        string? byMaxQuantity
        )
    {
        // var query = _context.Items.AsQueryable(); // <- This is preffered, but can't use OrdinalIgnoreCase while filtering
        var query = _context.Items
        .Include(i => i.Category)
        .AsEnumerable(); // <- This is slower, because it fetches all data to RAM and then filters it

        if (!string.IsNullOrWhiteSpace(byId))
            query = query.Where(i => i.Id.Contains(byId, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(byCategory))
        {
            if (byCategory.Equals("null", StringComparison.OrdinalIgnoreCase))
                query = query.Where(i => i.CategoryName == null);
            else
                query = query.Where(i => i.CategoryName != null && i.CategoryName.Contains(byCategory, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(byName))
            query = query.Where(i => i.Name != null && i.Name.Contains(byName, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(byDescription))
            query = query.Where(i => i.Description != null && i.Description.Contains(byDescription, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(byQuantity) && int.TryParse(byQuantity, out var qty))
            query = query.Where(i => i.Quantity == qty);

        if (!string.IsNullOrWhiteSpace(byMinQuantity) && int.TryParse(byMinQuantity, out var minQty))
            query = query.Where(i => i.Quantity >= minQty);

        if (!string.IsNullOrWhiteSpace(byMaxQuantity) && int.TryParse(byMaxQuantity, out var maxQty))
            query = query.Where(i => i.Quantity <= maxQty);

        var items = query.ToList();
        return Ok(new
        {
            itemsCount = 10,
            items
        });
    }

    [HttpGet]
    [Route("{itemId}")]
    public ActionResult<Item> Get([FromRoute] string itemId)
    {
        var dbItem = _context.Items.FirstOrDefault(p => p.Id.Equals(itemId));
        if (dbItem is null)
            return NotFound();
        return Ok(dbItem);
    }

    [HttpPost]
    public async Task<ActionResult<Item>> Create([FromBody] ItemCreate item)
    {
        string id;
        do
        {
            id = Generator.GetRandomString(StringType.Alphanumeric, StringCase.Lowercase, 10);
        } while (_context.Items.FirstOrDefault(p => p.Id.Equals(id)) is not null);

        if (item.CategoryName is not null)
        {
            var dbCategory = _context.Categories.FirstOrDefault(p => p.Name.Equals(item.CategoryName));
            if (dbCategory is null)
                return NotFound();
        }

        var newItem = new Item
        {
            Id = id,
            Name = item.Name,
            Description = item.Description,
            Quantity = item.Quantity,
            CategoryName = item.CategoryName
        };

        _context.Items.Add(newItem);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Create), new { id = newItem.Id }, newItem);
    }

    [HttpPut]
    [Route("{itemId}")]
    public async Task<ActionResult<Item>> Edit([FromBody] ItemEdit item, [FromRoute] string itemId)
    {
        itemId = itemId.ToLower();
        var dbItem = _context.Items.FirstOrDefault(p => p.Id.Equals(itemId));
        if (dbItem is null)
            return NotFound();
        if (item.Name is not null) dbItem.Name = item.Name;
        if (item.Description is not null) dbItem.Description = item.Description;
        if (item.Quantity is not null) dbItem.Quantity = (int)item.Quantity;
        if (item.CategoryName is not null)
        {
            if (item.CategoryName == "") // User want to unasign category
            {
                dbItem.Category = null;
                dbItem.CategoryName = null;
            }
            else // User wants to change category
            {
                var dbCategory = _context.Categories.FirstOrDefault(p => p.Name.Equals(item.CategoryName));
                if (dbCategory is null)
                    return NotFound();
                dbItem.Category = dbCategory;
            }
        }

        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Edit), new { id = dbItem.Id }, dbItem);

    }

    [HttpDelete]
    [Route("{itemId}")]
    public ActionResult<Item> Delete([FromRoute] string itemId)
    {
        var dbItem = _context.Items.Where(p => p.Id.Equals(itemId)).ExecuteDelete();
        return Ok(dbItem);
    }
}