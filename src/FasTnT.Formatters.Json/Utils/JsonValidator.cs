using FasTnT.Model.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NJsonSchema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FasTnT.Formatters.Json.Utils
{
    public class JsonValidator
    {
        public static JsonValidator Instance { get; } = new JsonValidator();
        private readonly JsonSchema _schema;

        private JsonValidator()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var schemaFile = assembly.GetManifestResourceNames()
                                    .FirstOrDefault(x => x.EndsWith(".json", StringComparison.OrdinalIgnoreCase));

            _schema = JsonSchema.FromJsonAsync(new StreamReader(assembly.GetManifestResourceStream(schemaFile)).ReadToEnd()).Result;
        }

        public async Task<IDictionary<string, JToken>> Load(Stream input)
        {
            using (var reader = new StreamReader(input))
            {
                var content = reader.ReadToEnd();
                var result = _schema.Validate(content);

                if (result.Any())
                {
                    throw new EpcisException(ExceptionType.ValidationException, $"{result.First().Kind} at line {result.First().LineNumber} position {result.First().LinePosition}", ExceptionSeverity.Error);
                }
                else
                {
                    return await Task.FromResult(JsonConvert.DeserializeObject<IDictionary<string, JToken>>(content));
                }
            }
        }
    }
}
