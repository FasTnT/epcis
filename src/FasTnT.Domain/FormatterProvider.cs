using FasTnT.Formatters;
using FasTnT.Model;
using FasTnT.Model.Exceptions;
using System;
using System.Linq;

namespace FasTnT.Domain
{
    public class FormatterProvider
    {
        private readonly IFormatterFactory[] _formatters;
        
        public FormatterProvider(IFormatterFactory[] formatters)
        {
            _formatters = formatters;
        }

        public IFormatter<T> GetFormatter<T>(string contentType) where T : IEpcisPayload
        {
            var factory = _formatters.FirstOrDefault(x => x.AllowedContentTypes.Contains(contentType.Split(';').First(), StringComparer.OrdinalIgnoreCase));

            return factory != null 
                ? factory.GetFormatter<T>()
                : throw new ContentTypeException($"No formatter found for content-type '{contentType}'");
        }
    }
}
