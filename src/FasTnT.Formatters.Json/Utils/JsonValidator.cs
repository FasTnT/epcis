using FasTnT.Model.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Formatters.Json.Utils
{
    public class JsonValidator
    {
        public static JsonValidator Instance { get; } = new JsonValidator();
        private readonly JSchema _schema;

        private JsonValidator()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var schemaFile = assembly.GetManifestResourceNames()
                                    .FirstOrDefault(x => x.EndsWith(".json", StringComparison.OrdinalIgnoreCase));

            _schema = JSchema.Load(new JsonTextReader(new StreamReader(assembly.GetManifestResourceStream(schemaFile))));
        }

        public async Task<IDictionary<string, JToken>> Load(Stream input)
        {
            using (var reader = new StreamReader(input))
            {
                var validatingReader = new JSchemaValidatingReader(new JsonTextReader(reader))
                {
                    Schema = _schema
                };
                var serializer = new JsonSerializer();

                try
                {
                    return await Task.FromResult(serializer.Deserialize<IDictionary<string, JToken>>(validatingReader));
                }
                catch(JSchemaValidationException ex)
                {
                    throw new EpcisException(ExceptionType.ValidationException, ex.Message, ExceptionSeverity.Error);
                }
            }
        }
    }
}
