using FasTnT.Model.MasterDatas;
using System;

namespace FasTnT.Data.PostgreSql.DTOs
{
    public class MasterDataDto
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdate { get; set; }

        internal static MasterDataDto Create(EpcisMasterData masterdata)
        {
            return new MasterDataDto
            {
                Type = masterdata.Type,
                Id = masterdata.Id
            };
        }

        internal EpcisMasterData ToMasterData()
        {
            return new EpcisMasterData
            {
                Id = Id,
                Type = Type
            };
        }
    }
}
