using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StacklyBackend.Models;

public class GroupDropdownViewComponent(AppDbContext _context, UserManager<User> _userManager) : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var userId = _userManager.GetUserId(HttpContext.User);

        var groups = _context.Groups.Where(
                g => g.OwnerId == userId ||
                g.Users.Any(
                    u => u.Id == userId
                )
            ).Select(g => new SelectListItem
            {
                Value = g.Id,
                Text = g.Name
            }).ToList();

        var selectedId = HttpContext.Session.GetString("SelectedGroupId") ?? "";

        var model = new GroupDropdownModel
        {
            SelectedId = selectedId,
            Groups = groups,
        };

        return View(model);
    }
}