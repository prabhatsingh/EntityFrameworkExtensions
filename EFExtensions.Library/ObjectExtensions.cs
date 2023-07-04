using Newtonsoft.Json;

namespace EFExtensions.Library
{
    public static class ObjectExtensions
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
    }
}
