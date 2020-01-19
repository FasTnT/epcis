using FasTnT.Domain.Commands;
using FasTnT.Model.Users;
using System;

namespace FasTnT.Domain
{
    public class RequestContext
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public ICommandFormatter Formatter { get; set; }
        public User User { get; set; }
    }
}
