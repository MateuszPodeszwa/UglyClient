namespace BeautifulClient.Utilities.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class MenuRouteAttribute(string title) : Attribute
{
    public string Title { get; } = title;
}