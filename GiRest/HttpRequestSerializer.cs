using System.Globalization;
using System.Net;
using System.Reflection;

namespace GiRest
{
    internal static class HttpRequestSerializer
    {
        public static string SerializeObject(object obj, string firstSpliter = "?")
        {
            string request = "";
            var fields = obj.GetType().GetProperties();
            string splitter = firstSpliter;
            foreach (var field in fields)
            {
                var fieldNameAttribute = field.GetCustomAttribute<PropertyNameAttribute>();
                string key = fieldNameAttribute is null ? field.Name : fieldNameAttribute.FieldName;
                string value = "";
                object? realValue = field.GetValue(obj);
                if (realValue is null) value = "";
                else if (realValue is float floatValue) value = floatValue.ToString(CultureInfo.InvariantCulture);
                else if (realValue is double doubleValue) value = doubleValue.ToString(CultureInfo.InvariantCulture);
                else if (realValue is decimal decimalValue) value = decimalValue.ToString(CultureInfo.InvariantCulture);
                else value = realValue.ToString() ?? "";
                value = Uri.EscapeDataString(value);

                request += $"{splitter}{key}" + (value.Length > 0 ? $"={value}" : "");
                splitter = "&";
            }
            return request;
        }

        public static T DeserializeObject<T>(string request)
        {
            T obj = Activator.CreateInstance<T>();
            var fields = request.Split('&');
            if (fields.Length == 0) throw new Exception();
            fields[0] = fields[0].TrimStart('?');
            foreach (var field in fields)
            {
                var key = field.Split('=')[0];
                var value = Uri.UnescapeDataString(field.Split('=')[1]);
                PropertyInfo? property = typeof(T).GetProperty(key);
                property?.SetValue(obj, Convert.ChangeType(value, property.PropertyType, CultureInfo.InvariantCulture));
            }
            return obj;
        }


        public static void ParseHeaderFromObject(WebHeaderCollection header, object obj)
        {
            var fields = obj.GetType().GetProperties();
            foreach (var field in fields)
            {
                var fieldNameAttribute = field.GetCustomAttribute<PropertyNameAttribute>();
                string key = fieldNameAttribute is null ? field.Name : fieldNameAttribute.FieldName;
                string value;
                object? realValue = field.GetValue(obj);
                if (realValue is null) continue;
                if (realValue is float floatValue) value = floatValue.ToString(CultureInfo.InvariantCulture);
                else if (realValue is double doubleValue) value = doubleValue.ToString(CultureInfo.InvariantCulture);
                else if (realValue is decimal decimalValue) value = decimalValue.ToString(CultureInfo.InvariantCulture);
                else value = realValue.ToString() ?? "";
                value = Uri.EscapeDataString(value);
                if (value.Length > 0)
                {
                    header.Add(key, value);
                }
            }
        }
    }
}
