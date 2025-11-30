using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace StacklyBackend.Utils.FormFactory;

[HtmlTargetElement("form-select-builder")]
public class FormSelectBuilderTagHelper : TagHelper
{
    public string? Klasa { get; set; }
    public string? Id { get; set; }
    public required string Name { get; set; }

    public IEnumerable<SelectListItem>? Items { get; set; }
    public string? SelectedValue { get; set; }

    public string? EmptyOptionText { get; set; }
    public string? EmptyOptionValue { get; set; } = "";

    public bool Required { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "div";
        var wrapperClass = string.IsNullOrWhiteSpace(Klasa)
            ? "stackly-field"
            : $"stackly-field {Klasa}";
        output.Attributes.SetAttribute("class", wrapperClass);

        var child = await output.GetChildContentAsync();
        var labelText = child.GetContent().Trim();

        var id = Id ?? Name;

        string labelHtml = Required
            ? $"<label for=\"{id}\" class=\"stackly-label\">{labelText} <span class=\"text-danger\">*</span></label>"
            : $"<label for=\"{id}\" class=\"stackly-label\">{labelText}</label>";

        var items = Items ?? Enumerable.Empty<SelectListItem>();
        var sb = new System.Text.StringBuilder();
        var requiredAttr = Required ? " required" : "";

        sb.Append($"<select id=\"{id}\" name=\"{Name}\" class=\"stackly-select\"{requiredAttr}>");

        if (string.IsNullOrEmpty(EmptyOptionText))
        {
            sb.Append($"<option value=\"{EmptyOptionValue}\">{EmptyOptionText}</option>");
        }

        foreach (var item in items)
        {
            var isSelected = !string.IsNullOrEmpty(SelectedValue)
                ? string.Equals(item.Value, SelectedValue, StringComparison.Ordinal)
                : item.Selected;

            var selectedAttr = isSelected ? " selected" : "";
            sb.Append($"<option value=\"{item.Value}\"{selectedAttr}>{item.Text}</option>");
        }

        sb.Append("</select>");

        output.Content.SetHtmlContent(labelHtml + sb.ToString());
    }
}
