using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
namespace StacklyBackend.Utils.FormFactory;


[HtmlTargetElement("form-input-builder")]
public class FormInputBuilderTagHelper : TagHelper
{
    public string klasa { get; set; }
    public string id { get; set; }
    public string type { get; set; }
    public string name { get; set; }
    public string placeholder { get; set; }
    public string value { get; set; }
    
    
    
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        
        output.TagName = "div";
        
        //     output.TagName = "div";                // w co ma się zamienić <stackly-form>
        //     output.Content.SetContent("Działa"); // co ma się pojawić w środku
    }

}


