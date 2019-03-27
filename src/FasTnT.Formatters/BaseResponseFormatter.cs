using FasTnT.Model.Responses;
using System;
using System.IO;

namespace FasTnT.Formatters
{
    public abstract class BaseResponseFormatter<T> : IResponseFormatter
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

        public abstract string ToContentTypeString();
        public virtual IEpcisResponse Read(Stream input) => throw new NotImplementedException();
        public abstract void Write(IEpcisResponse entity, Stream output);

        protected abstract T FormatInternal(PollResponse response);
        protected abstract T FormatInternal(GetStandardVersionResponse response);
        protected abstract T FormatInternal(GetVendorVersionResponse response);
        protected abstract T FormatInternal(ExceptionResponse response);
        protected abstract T FormatInternal(GetSubscriptionIdsResult response);
        protected abstract T FormatInternal(GetQueryNamesResponse response);
    }
}
