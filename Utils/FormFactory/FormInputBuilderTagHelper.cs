using Microsoft.AspNetCore.Razor.TagHelpers;
namespace StacklyBackend.Utils.FormFactory;


[HtmlTargetElement("form-input-builder")]
public class FormInputBuilderTagHelper : TagHelper
{
    public string? Klasa { get; set; }
    public string? Id { get; set; }
    public string Type { get; set; } = "text";
    public string Name { get; set; } = "";
    public string? Placeholder { get; set; }
    public string? Value { get; set; }
    public bool Required { get; set; }
    
    
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "div";
        var wrapperClass = string.IsNullOrWhiteSpace(Klasa)
            ? "stackly-form__field"
            : $"stackly-form__field {Klasa}";
        output.Attributes.SetAttribute("class", wrapperClass);

        // Get the inner content (label text)
        var child = await output.GetChildContentAsync();
        var labelText = child.GetContent().Trim();
        
        var name = Name;
        var type = Type;
        var id = Id ?? Name;
        var value = Value;
        var required = Required;
        
        string labelHtml = required
            ? $"<label for=\"{id}\" class=\"stackly-form__label\">{labelText}<span class=\"text-danger\">*</span></label>"
            : $"<label for=\"{id}\" class=\"stackly-form__label\">{labelText}</label>";
        
        
        string requiredAttr = required ? "required" : "";
        string inputHtml =
            $"<input type=\"{type}\" id=\"{id}\" name=\"{name}\" " +
            $"class=\"stackly-form__input\" {requiredAttr} value=\"{value}\" />";
        
        output.Content.SetHtmlContent(labelHtml + inputHtml);
    }
}


