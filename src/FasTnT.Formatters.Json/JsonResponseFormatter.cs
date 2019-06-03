using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using fastJSON;
using FasTnT.Formatters.Json.JsonFormatter;
using FasTnT.Model;
using FasTnT.Model.Responses;

namespace FasTnT.Formatters.Json
{
    public class JsonResponseFormatter : BaseResponseFormatter<string>
    {
        public override async Task Write(IEpcisResponse entity, Stream output, CancellationToken cancellationToken)
        {
            if (entity == default(IEpcisResponse)) return;

            using(var writer = new StreamWriter(output))
            {
                await writer.WriteAsync(new ReadOnlyMemory<char>(Format(entity).ToArray()), cancellationToken);
            }
        }

        protected override string FormatInternal(PollResponse response)
        {
            var dict = new Dictionary<string, object>
            {
                { "@context", "https://id.gs1.org/epcis-context.jsonld" },
                { "isA", "EPCISDocument" },
                { "creationDate", DateTime.UtcNow },
                { "schemaVersion", "1.2" },
                { "format", "application/ld+json" },
                { "epcisBody", new Dictionary<string, object>
                    {
                        { "eventList", response.Entities.Select(x => new JsonEventFormatter().FormatEvent((EpcisEvent)x)) }
                    }
                }
            };

            return JSON.ToJSON(dict);
        }

        protected override string FormatInternal(GetStandardVersionResponse response) => JSON.ToJSON(response.Version);
        protected override string FormatInternal(GetVendorVersionResponse response) => JSON.ToJSON(response.Version);

        protected override string FormatInternal(ExceptionResponse response)
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

        protected override string FormatInternal(GetSubscriptionIdsResult response) => throw new NotImplementedException();
        protected override string FormatInternal(GetQueryNamesResponse response) => throw new NotImplementedException();
    }
}
