using System;
using FasTnT.Model;

namespace FasTnT.Persistence.Dapper
{
    public class RequestHeaderEntity : EpcisRequestHeader
    {
        public Guid Id { get; set; }
    }
}
