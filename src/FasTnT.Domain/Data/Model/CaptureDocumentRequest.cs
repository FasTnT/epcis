using FasTnT.Commands.Requests;
using FasTnT.Model.Users;
using System.Data;
using System.Threading;

namespace FasTnT.Domain.Data.Model
{
    public class CaptureDocumentRequest
    {
        public CaptureEpcisDocumentRequest Payload { get; set; }
        public User User { get; set; }
        public IDbTransaction Transaction { get; set; }
        public CancellationToken CancellationToken { get; set; }
    }
}
