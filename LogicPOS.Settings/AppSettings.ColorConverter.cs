using Newtonsoft.Json;
using System;
using System.Drawing;

namespace LogicPOS.Settings
{
    public partial class AppSettings
    {
        public class ColorConverter : JsonConverter<Color>
        {
            public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                var value = reader.Value.ToString().Split(',');
                return Color.FromArgb(int.Parse(value[0]), int.Parse(value[1]), int.Parse(value[2]));
            }

            public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
            {
               writer.WriteValue($"{value.R},{value.G},{value.B}");
            }
        }
    }
}

