using Microsoft.AspNetCore.Razor.TagHelpers;

namespace StacklyBackend.Utils.FormFactory;


[HtmlTargetElement("form-submit-builder")]
public class FormSubmitBuilerTagHelper : TagHelper
{
    public string? Klasa { get; set; }
    
    public string Type { get; set; } = "submit";
    
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "button";
        
        // Something inside tag is value of the button
        var child = await output.GetChildContentAsync();
        var buttonText = child.GetContent().Trim();
        
        var classes = string.IsNullOrWhiteSpace(Klasa)
            ? "stackly-btn"
            : $"stackly-btn {Klasa}";

        output.Attributes.SetAttribute("class", classes);
        output.Attributes.SetAttribute("type", Type);

        output.Content.SetHtmlContent(buttonText);
    }
}