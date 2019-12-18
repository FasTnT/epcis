using FasTnT.Commands.Requests;
using FasTnT.Commands.Responses;
using FasTnT.Domain.Commands;
using MediatR;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FasTnT.Parsers.Xml
{
    public class XmlCommandParser : ICommandParser
    {
        public string ContentType { get { return "application/xml"; } }

        public async Task<IRequest<IEpcisResponse>> ParseCommand(Stream input, CancellationToken cancellationToken)
        {
            var document = await XDocument.LoadAsync(input, LoadOptions.None, cancellationToken);
            var localName = document.Root.Element("EPCISBody").Elements().First().Name.LocalName;

            switch (localName)
            {
                case "GetStandardVersion":
                    return new GetStandardVersionRequest();
                case "GetVendorVersion":
                    return new GetVendorVersionRequest();
                case "Poll":
                    return new PollRequest();
                case "GetQueryNames":
                    return new GetQueryNamesRequest();
                case "GetSubscriptionIDs":
                    return new GetSubscriptionIdsRequest();
                case "Subscribe":
                    return new SubscribeRequest();
                case "Unsubscribe":
                    return new UnsubscribeRequest();
                default:
                    throw new Exception();
            }
        }

        public async Task WriteResponse(IEpcisResponse epcisResponse, Stream body, CancellationToken requestAborted)
        {
            await Task.CompletedTask;
        }
    }
}
