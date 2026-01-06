using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace StacklyBackend.Utils.FormFactory;

[HtmlTargetElement("form-builder")]
public class  FormBuilderTagHelper : TagHelper
{
    
    private readonly IHtmlGenerator _htmlGenerator;
    
    public FormBuilderTagHelper (IHtmlGenerator htmlGenerator)
    {
        _htmlGenerator = htmlGenerator;
    }
    
    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; } = null!;
    
    public string? Action { get; set; }
    public string? Controller { get; set; }
    public string Method { get; set; } = "post";
    public string Klasa { get; set; } = "";
    public string? Enctype {get; set;}
    
    
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        
        output.TagName = "form";
        var wrapperClass = string.IsNullOrWhiteSpace(Klasa)
            ? "stackly-form"
            : $"stackly-form {Klasa}";
        output.Attributes.SetAttribute("class", wrapperClass);

        var htmlAttributes = new Dictionary<string, object>()
        {
            { "method", Method }
        };

        if (!string.IsNullOrWhiteSpace(Enctype))
        {
            htmlAttributes.Add("enctype", Enctype);
        }
        
        var formTag = _htmlGenerator.GenerateForm(
            ViewContext,
            Action,
            Controller,
            fragment: null,
            routeValues: null,
            htmlAttributes: htmlAttributes,
            method: Method
        );

        if (formTag != null)
        {
            output.MergeAttributes(formTag);
        }
        
        // Antifroggery token
        if (string.Equals(Method, "post", StringComparison.OrdinalIgnoreCase))
        {
            var antiForgeryTag = _htmlGenerator.GenerateAntiforgery(ViewContext);
            if (antiForgeryTag != null)
            {
                output.Content.AppendHtml(antiForgeryTag);
            }
        }
        
        var error = ViewContext.ViewData["error"] as string; 
        if (!string.IsNullOrEmpty(error))
        {
            string errorHtml =
                $"<div role=\"alert\" class=\"stackly-form__error\">{error}</div>";
            output.PreElement.AppendHtml(errorHtml);
        }
        
        var child = await output.GetChildContentAsync();
        output.Content.AppendHtml(child);
    }
}