using Microsoft.AspNetCore.Mvc.Rendering;
namespace StacklyBackend.Utils.FormFactory;

public static class SelectListExtensions
{
    public static IEnumerable<SelectListItem> ToSelectList<T, TValue>(
        this IEnumerable<T> source,
        Func<T, TValue> valueSelector,
        Func<T, string> textSelector)
    {
        return source.Select(x => new SelectListItem
        {
            Value = valueSelector(x)?.ToString() ?? string.Empty,
            Text  = textSelector(x)
        });
    }
    
}