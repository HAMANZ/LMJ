using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.BLO.LogicObjects
{
    public static class ObjectExtensions
    {
        public static bool CanBeConverted<T>(this object value) where T : class
        {
            var jsonData = JsonConvert.SerializeObject(value);
            var generator = new JSchemaGenerator();
            var parsedSchema = generator.Generate(typeof(T));
            var jObject = JObject.Parse(jsonData);

            return jObject.IsValid(parsedSchema);
        }

        public static T ConvertToType<T>(this object value) where T : class
        {
            var jsonData = JsonConvert.SerializeObject(value);
            return JsonConvert.DeserializeObject<T>(jsonData);
        }
    }
}
