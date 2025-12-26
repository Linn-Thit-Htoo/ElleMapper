namespace ElleMapper;

[AttributeUsage(AttributeTargets.Property)]
public class ColumnAttribute : Attribute
{
    public string Name { get; }

    public ColumnAttribute(string name)
    {
        Name = name;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class TableAttribute : Attribute
{
    public string Name { get; }

    public TableAttribute(string name)
    {
        Name = name;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class ViewAttribute : Attribute
{
    public string Name { get; }

    public ViewAttribute(string name)
    {
        Name = name;
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class IdentityAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Property)]
public class KeyAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Property)]
public class IgnoreAttribute : Attribute { }
