namespace ImmutaMap.Test
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FirstNameAttribute : Attribute
    {
        public string RealName { get; set; } = string.Empty; 
    }
}