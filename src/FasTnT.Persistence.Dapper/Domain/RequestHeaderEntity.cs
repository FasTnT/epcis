using System;
using FasTnT.Model;

namespace FasTnT.Persistence.Dapper
{
    public class RequestHeaderEntity : EpcisRequestHeader
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
    }
}
