using FasTnT.Model.Responses;
using FasTnT.Domain.Persistence;
using System.Threading.Tasks;
using FasTnT.Model;

namespace FasTnT.Domain.Services.Handlers
{
    public class CaptureEventsHandler : IHandler<EpcisEventDocument>
    {
        private readonly IEventStore _eventStore;

        public CaptureEventsHandler(IEventStore eventStore) => _eventStore = eventStore;

        public async Task<IEpcisResponse> Handle(EpcisEventDocument request)
        {
            await _eventStore.Store(request);

            return new CaptureSucceedResponse();
        }
    }
}
