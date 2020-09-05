using FasTnT.Model;
using System;

namespace FasTnT.Data.PostgreSql.DTOs
{
    public class RequestDto
    {
        public DateTime DocumentTime { get; set; }
        public DateTime RecordTime { get; set; }
        public string SchemaVersion { get; set; }
        public int UserId { get; set; }

        public static RequestDto Create(EpcisRequest request, int userId)
        {
            return new RequestDto
            {
                DocumentTime = request.DocumentTime,
                RecordTime = request.RecordTime,
                SchemaVersion = request.SchemaVersion,
                UserId = userId
            };
        }
    }
}
