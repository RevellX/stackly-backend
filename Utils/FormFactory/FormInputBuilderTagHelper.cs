using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace StacklyBackend.Utils.FormFactory;


[HtmlTargetElement("form-input-builder")]
public class FormInputBuilderTagHelper : TagHelper
{

    private readonly IHtmlGenerator _generator;

    public FormInputBuilderTagHelper(IHtmlGenerator generator)
    {
        _generator = generator;
    }

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; } = null!;

    // Name, value, required
    [HtmlAttributeName("for")] 
    public ModelExpression For { get; set; } = null!;
    
    public string Type { get; set; } = "";
    public string? Klasa { get; set; }
    public string? Placeholder { get; set; }
    public bool Multiple { get; set; } = false;



    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        // zaczynamy od kontenera div
        output.TagName = "div";
        output.TagMode = TagMode.StartTagAndEndTag;

        var wrapperClass = string.IsNullOrWhiteSpace(Klasa)
            ? "stackly-form__field"
            : $"stackly-form__field {Klasa}";
        output.Attributes.SetAttribute("class", wrapperClass);

        // Label
        var child = await output.GetChildContentAsync();
        var labelContent = child.GetContent().Trim();

        if (string.IsNullOrWhiteSpace(labelContent))
        {
            labelContent = For.Metadata.DisplayName ?? For.Metadata.PropertyName;
        }

        bool isRequired = For.Metadata.IsRequired;
        string requiredSpan = isRequired ? "<span class=\"text-danger\">*</span>" : "";

        // budowanie labela
        var labelBuilder = new TagBuilder("label");
        labelBuilder.Attributes.Add("for", TagBuilder.CreateSanitizedId(For.Name, "-"));
        labelBuilder.AddCssClass("stackly-form__label");
        labelBuilder.InnerHtml.AppendHtml(labelContent + requiredSpan);
        
        // Input

        var inputAttributes = new Dictionary<string, object>
        {
            { "class", "stackly-form__input" },
            { "placeholder", Placeholder ?? "" }
        };
        
        if (!string.IsNullOrEmpty(Type))
        {
            inputAttributes.Add("type", Type);
        }

        if (Type == "file")
        {
            inputAttributes["class"] = "stackly-form__input file-input";
            if (Multiple)
            {
                inputAttributes.Add("multiple", "multiple");
            }
        }
        
        var inputBuilder = _generator.GenerateTextBox(
            ViewContext,
            For.ModelExplorer,
            For.Name,
            value: For.Model,
            format: null,
            htmlAttributes: inputAttributes
        );
        
        var validationBuilder = _generator.GenerateValidationMessage(
            ViewContext,
            For.ModelExplorer,
            For.Name,
            message: null,
            tag: "span",
            htmlAttributes: new { @class = "text-danger" }
        );
        
        output.Content.AppendHtml(labelBuilder);
        output.Content.AppendHtml(inputBuilder);
        output.Content.AppendHtml(validationBuilder);
    }
}