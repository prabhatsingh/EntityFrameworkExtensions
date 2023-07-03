using Newtonsoft.Json;

namespace EFExtensions.Library
{
    public static class Extensions
    {
        public static T DeepCopy<T>(this T obj) where T : class
        {
            return obj.Clone();
        }

        public static T Clone<T>(this T obj) where T : class
        {
            if (obj == null)
                return default;

            var desrializesettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj), desrializesettings);
        }

        public static T GetPropertyValue<T>(this object obj, string propname)
        {
            if (obj == null || obj.GetType().GetProperty(propname) == null)
                return default;

            return (T)obj.GetType().GetProperty(propname).GetValue(obj);
        }

        public static void SetPropertyValue(this object obj, string propertyname, object value)
        {
            obj.GetType().GetProperty(propertyname).SetValue(obj, value);
        }
    }
}
