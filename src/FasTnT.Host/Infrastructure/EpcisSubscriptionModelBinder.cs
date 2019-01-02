using FasTnT.Formatters.Xml;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

namespace FasTnT.Host.Binders
{
    public class EpcisSubscriptionModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var parser = new XmlSubscriptionFormatter(); // TODO: get from Content-Type.
            var inputStream = bindingContext.ActionContext?.HttpContext?.Request?.Body;

            if (inputStream == null || !inputStream.CanRead)
            {
                throw new Exception("HTTP context is null or body can't be read");
            }

            bindingContext.Result = ModelBindingResult.Success(parser.Read(inputStream));

            return Task.CompletedTask;
        }
    }
}
