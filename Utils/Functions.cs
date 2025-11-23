namespace StacklyBackend.Utils;

public class Functions
{
    public static bool IsNullable(Type type) => Nullable.GetUnderlyingType(type) != null;
}