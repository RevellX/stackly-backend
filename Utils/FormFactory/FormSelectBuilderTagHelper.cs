using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace StacklyBackend.Utils.FormFactory;

[HtmlTargetElement("form-select-builder")]
public class FormSelectBuilderTagHelper : TagHelper
{
    private readonly IHtmlGenerator _htmlGenerator;
    
    public FormSelectBuilderTagHelper(IHtmlGenerator htmlGenerator)
    {
        _htmlGenerator = htmlGenerator;
    }
    
    
    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; } = null!;
    
    [HtmlAttributeName("for")]
    public ModelExpression For { get; set; } = null!;
    
    [HtmlAttributeName("items")]
    public IEnumerable<SelectListItem>? Items { get; set; }
    
    public string? Klasa { get; set; }
    
    // opcja 1, wybierz kategorie etc.
    public string? OptionLabel { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "div";
        var wrapperClass = string.IsNullOrEmpty(Klasa)
            ? "stackly-form__field"
            : $"stackly-form__field {Klasa}";
        output.Attributes.SetAttribute("class", wrapperClass);
        
        //label
        var child = await output.GetChildContentAsync();
        var labelContent = child.GetContent().Trim();
        if (string.IsNullOrWhiteSpace(labelContent))
        {
            labelContent = For.Metadata.DisplayName ?? For.Metadata.PropertyName;
        }
        
        bool isRequired = For.Metadata.IsRequired;
        string requiredSpan = isRequired ? "<span class=\"text-danger\">*</span>" : "";
        
        var labelBuilder = new TagBuilder("label");
        labelBuilder.Attributes.Add("for", TagBuilder.CreateSanitizedId(For.Name, "-"));
        labelBuilder.AddCssClass("stackly-form__label");
        labelBuilder.InnerHtml.AppendHtml(labelContent + requiredSpan);
        
        // generate select
        var selectBuilder = _htmlGenerator.GenerateSelect(
            ViewContext,
            For.ModelExplorer,
            OptionLabel,
            For.Name,
            Items,
            allowMultiple: false,
            htmlAttributes: new { @class = "stackly-form__select" }
        );
        
        // walidacja
        var validationBuilder = _htmlGenerator.GenerateValidationMessage(
            ViewContext,
            For.ModelExplorer,
            For.Name,
            message: null,
            tag: "span",
            htmlAttributes: new { @class = "text-danger" }
        );
        
        // łączenie
        output.Content.AppendHtml(labelBuilder);
        output.Content.AppendHtml(selectBuilder);
        output.Content.AppendHtml(validationBuilder);

    }
}
