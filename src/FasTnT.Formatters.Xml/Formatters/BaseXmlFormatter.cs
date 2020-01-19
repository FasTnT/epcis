using FasTnT.Commands.Responses;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FasTnT.Parsers.Xml.Formatters
{
    public abstract class BaseXmlFormatter
    {
        private const SaveOptions Options = SaveOptions.DisableFormatting | SaveOptions.OmitDuplicateNamespaces;

        public async Task Write(IEpcisResponse entity, Stream output, CancellationToken cancellationToken)
        {
            if (entity == default(IEpcisResponse)) return;

            await Format(entity).SaveAsync(output, Options, cancellationToken);
        }

        private XDocument Format(IEpcisResponse entity)
        {
            switch (entity)
            {
                case GetStandardVersionResponse getStandardVersionResponse:
                    return FormatInternal(getStandardVersionResponse);
                case GetVendorVersionResponse getVendorVersionResponse:
                    return FormatInternal(getVendorVersionResponse);
                case ExceptionResponse exceptionResponse:
                    return FormatInternal(exceptionResponse);
                case GetQueryNamesResponse getQueryNamesResponse:
                    return FormatInternal(getQueryNamesResponse);
                case GetSubscriptionIdsResponse getSubscriptionIdsResponse:
                    return FormatInternal(getSubscriptionIdsResponse);
                case PollResponse pollResponse:
                    return FormatInternal(pollResponse);
                default:
                    throw new NotImplementedException($"Unable to format '{entity.GetType()}'");
            }
        }

        public abstract XDocument FormatInternal(GetStandardVersionResponse response);
        public abstract XDocument FormatInternal(GetVendorVersionResponse response);
        public abstract XDocument FormatInternal(ExceptionResponse response);
        public abstract XDocument FormatInternal(GetQueryNamesResponse response);
        public abstract XDocument FormatInternal(GetSubscriptionIdsResponse response);
        public abstract XDocument FormatInternal(PollResponse response);
    }
}
