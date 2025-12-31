using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StacklyBackend.Models;

namespace StacklyBackend.Controllers;

public class HomeController(AppDbContext _context) : Controller
{
    public IActionResult Index()
    {
        ViewData["items_stored_total"] = _context.Items.Count();
        ViewData["categories_stored_total"] = _context.Categories.Count();
        ViewData["users_stored_total"] = _context.Users.Count();
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
