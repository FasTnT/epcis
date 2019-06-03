using FasTnT.Model.Responses;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Formatters
{
    public abstract class BaseResponseFormatter<T>
    {
        public T Format(IEpcisResponse entity)
        {
            switch (entity)
            {
                case PollResponse pollResponse:
                    return FormatInternal(pollResponse);
                case GetStandardVersionResponse getStandardVersionResponse:
                    return FormatInternal(getStandardVersionResponse);
                case GetVendorVersionResponse getVendorVersionResponse:
                    return FormatInternal(getVendorVersionResponse);
                case ExceptionResponse exceptionResponse:
                    return FormatInternal(exceptionResponse);
                case GetSubscriptionIdsResult getSubscriptionIdsResult:
                    return FormatInternal(getSubscriptionIdsResult);
                case GetQueryNamesResponse getQueryNamesResponse:
                    return FormatInternal(getQueryNamesResponse);
                default:
                    throw new NotImplementedException($"Unable to format '{entity.GetType()}'");
            }
        }

        public abstract Task Write(IEpcisResponse entity, Stream output, CancellationToken cancellationToken);

        protected abstract T FormatInternal(PollResponse response);
        protected abstract T FormatInternal(GetStandardVersionResponse response);
        protected abstract T FormatInternal(GetVendorVersionResponse response);
        protected abstract T FormatInternal(ExceptionResponse response);
        protected abstract T FormatInternal(GetSubscriptionIdsResult response);
        protected abstract T FormatInternal(GetQueryNamesResponse response);
    }
}
