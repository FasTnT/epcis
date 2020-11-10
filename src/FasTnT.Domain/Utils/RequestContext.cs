using FasTnT.Domain.Commands;
using FasTnT.Model.Users;

namespace FasTnT.Domain
{
    public class RequestContext
    {
        public ICommandFormatter Formatter { get; set; }
        public User User { get; set; }
    }
}
