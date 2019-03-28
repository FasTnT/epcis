using FasTnT.Formatters;
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

        public IFormatter<T> GetFormatter<T>(string contentType)
        {
            var factory = _formatters.FirstOrDefault(x => x.AllowedContentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase));

            return factory != null 
                ? factory.GetFormatter<T>()
                : throw new ArgumentException($"No formatter found for content-type '{contentType}'");
        }
    }
}
