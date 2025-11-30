using Microsoft.AspNetCore.Razor.TagHelpers;

namespace StacklyBackend.Utils.FormFactory;

[HtmlTargetElement("form-builder")]
public class FormBuilderTagHelper : TagHelper
{
    public string klasa { get; set; }
    public string id { get; set; }
    public string method { get; set; }
    
    
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        
        output.TagName = "form";
        
        //     output.TagName = "div";                // w co ma się zamienić <stackly-form>
        //     output.Content.SetContent("Działa"); // co ma się pojawić w środku
    }

}