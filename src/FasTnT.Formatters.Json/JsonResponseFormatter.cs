using System;
using System.Collections.Generic;
using System.IO;
using fastJSON;
using FasTnT.Model.Responses;

namespace FasTnT.Formatters.Json
{
    public class JsonResponseFormatter : IResponseFormatter
    {
        public string ToContentTypeString() => "application/json";

        public IEpcisResponse Read(Stream input) => throw new NotImplementedException();

        public void Write(IEpcisResponse entity, Stream output)
        {
            if (entity != default(IEpcisResponse))
            {
                using(var writer = new StreamWriter(output))
                {
                    writer.Write(Format((dynamic)entity));
                }
            }
        }

        public string Format(ExceptionResponse response)
        {
            return JSON.ToJSON(new Dictionary<string, object>
            {
                { "@Context", "test" },
                { "isA", response.Exception },
                { "creationDate", DateTime.UtcNow },
                { "schemaVersion", "1"},
                { "format", "application/ld+json" },
                { "Body", new Dictionary<string, object>{
                        { "@Type", response.Exception },
                        { "Severity", response.Severity.DisplayName },
                        { "Reason", response.Reason }
                    }
                }
            });
        }
    }
}
