using FasTnT.Model;
using FasTnT.Model.Events.Enums;
using FasTnT.Model.Exceptions;
using MoreLinq;
using System;
using System.Linq;

namespace FasTnT.Domain.Services
{
    internal static class EpcisEventValidator
    {
        internal static void Validate(EpcisEvent evt)
        {
            evt.Epcs.ForEach(e => UriValidator.Validate(e.Id));

            // TCR-7 parentID is Populated for ADD or DELETE Actions in Aggregation Events
            if (IsAddOrDeleteAggregation(evt) && !evt.Epcs.Any(x => x.Type == EpcType.ParentId))
                throw new EpcisException(ExceptionType.ValidationException, "TCR-7: parentID must be populated for ADD or DELETE aggregation event.");
        }

        private static bool IsAddOrDeleteAggregation(EpcisEvent evt) => evt.Type == EventType.Aggregation && new[] { EventAction.Add, EventAction.Delete }.Contains(evt.Action);
    }
}
