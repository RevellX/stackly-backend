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

    [ViewContext] [HtmlAttributeNotBound] public ViewContext ViewContext { get; set; } = null!;

    // Name, value, required
    [HtmlAttributeName("for")] public ModelExpression For { get; set; } = null!;

    public string Type { get; set; } = "";

    public string? Klasa { get; set; }

    public string? Placeholder { get; set; }



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
        // pobieramy ze środka taga o ile istnieje, lub z modelu [Display]
        var child = await output.GetChildContentAsync();
        var labelContent = child.GetContent().Trim();

        if (string.IsNullOrWhiteSpace(labelContent))
        {
            // Bierzemy nazwę z DisplayName ("Nazwa Produktu") lub nazwy właściwości ("Name")
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
        //Generator sam ustawia value, name, id itp.
        var inputBuilder = _generator.GenerateTextBox(
            ViewContext,
            For.ModelExplorer,
            For.Name,
            value: null, // null bo wartość jest brana z modelu
            format: null,
            htmlAttributes: new
            {
                @class = "stackly-form__input",
                placeholder = Placeholder,
                // jeśli podaliśmy type to go uywamy jak nie to domyślny
                type = string.IsNullOrEmpty(Type) ? null : Type
            }
        );

        // Validation message
        var validationBuilder = _generator.GenerateValidationMessage(
            ViewContext,
            For.ModelExplorer,
            For.Name,
            message: null,
            tag: "span",
            htmlAttributes: new { @class = "text-danger" }
        );

        // Łączenie wszystkiego
        output.Content.AppendHtml(labelBuilder);
        output.Content.AppendHtml(inputBuilder);
        output.Content.AppendHtml(validationBuilder);
    }
}