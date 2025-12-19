using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Rendering;

public class GroupDropdownModel
{
    public string SelectedId { get; set; } = string.Empty;
    public IEnumerable<SelectListItem> Groups { get; set; } = null!;
}