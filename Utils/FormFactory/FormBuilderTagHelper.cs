using Microsoft.AspNetCore.Razor.TagHelpers;

namespace StacklyBackend.Utils.FormFactory;

[HtmlTargetElement("form-builder")]
public class FormBuilderTagHelper : TagHelper
{
    public string Klasa { get; set; }
    public string Id { get; set; }
    public required string Method { get; set; }
    public required string Action { get; set; }
    
    
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        
        output.TagName = "form";
        var wrapperClass = string.IsNullOrWhiteSpace(Klasa)
            ? "stackly-form"
            : $"stackly-form {Klasa}";
        output.Attributes.SetAttribute("class", wrapperClass);
        
        
        var method = Method;
        var action = Action;
        
        output.Attributes.SetAttribute("method", method);
        output.Attributes.SetAttribute("action", action);
    }

}