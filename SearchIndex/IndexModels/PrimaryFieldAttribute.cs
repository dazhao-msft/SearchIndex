using System;

namespace IndexModels
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class PrimaryFieldAttribute : Attribute
    {
    }
}
