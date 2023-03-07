namespace GiRest
{
    public class PropertyNameAttribute : Attribute
    {
        public readonly string FieldName;
        public PropertyNameAttribute(string fieldName)
        {
            FieldName = fieldName;
        }
    }
}