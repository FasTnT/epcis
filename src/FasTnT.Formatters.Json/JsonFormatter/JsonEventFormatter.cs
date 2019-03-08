using FasTnT.Model;
using System;
using System.Collections.Generic;

namespace FasTnT.Formatters.Json.JsonFormatter
{
    public class JsonEventFormatter
    {
        const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ssZ";

        public IDictionary<string, object> FormatEvent(EpcisEvent evt)
        {
            var dictionary = new Dictionary<string, object>
            {
                { "isA", evt.Type.DisplayName },
                { "recordTime", evt.CaptureTime.ToString(DateTimeFormat) },
                { "eventTime", evt.EventTime.ToString(DateTimeFormat) },
                { "eventTimeZoneOffset", evt.EventTimeZoneOffset }
            };

            AddIfNotNull(evt, dictionary, "action", x => x.Action, x => x.Action.DisplayName);
            AddIfNotNull(evt, dictionary, "bizStep", x => x.BusinessStep);

            return dictionary;
        }

        private void AddIfNotNull(EpcisEvent evt, IDictionary<string, object> formatted, string key, Func<EpcisEvent, object> selector, Func<EpcisEvent, string> value = null)
        {
            if(selector(evt) != null)
            {
                formatted.Add(key, value == null ? selector(evt) : value(evt));
            }
        }
    }
}
