using System;

[AttributeUsage(AttributeTargets.Field)] // This attribute is for fields only
public class ConditionalHideAttribute : Attribute
{
    public string ConditionalSourceField { get; private set; }
    public object ConditionalValue { get; private set; }

    public ConditionalHideAttribute(string conditionalSourceField, object conditionalValue)
    {
        ConditionalSourceField = conditionalSourceField;
        ConditionalValue = conditionalValue;
    }
}
