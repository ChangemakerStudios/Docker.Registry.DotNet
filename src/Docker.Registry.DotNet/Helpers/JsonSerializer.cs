using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Docker.Registry.DotNet.Helpers
{
    /// <summary>
    ///     Facade for <see cref="JsonConvert" />.
    /// </summary>
    internal class JsonSerializer
    {
        static JsonSerializer()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
                                                {
                                                    NullValueHandling = NullValueHandling.Ignore
                                                };
        }

        public JsonSerializer()
        {
            this.Converters = new JsonConverter[]
                              {
                                  //new JsonIso8601AndUnixEpochDateConverter(),
                                  //new JsonVersionConverter(),
                                  new StringEnumConverter()
                                  //new TimeSpanSecondsConverter(),
                                  //new TimeSpanNanosecondsConverter()
                              };
        }

        private JsonConverter[] Converters { get; }

        public T DeserializeObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, this.Converters);
        }

        public string SerializeObject<T>(T value)
        {
            return JsonConvert.SerializeObject(value, this.Converters);
        }
    }
}