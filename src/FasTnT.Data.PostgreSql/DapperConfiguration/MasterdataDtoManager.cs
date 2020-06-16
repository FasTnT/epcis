using Dapper;
using FasTnT.Data.PostgreSql.DTOs;
using FasTnT.Model.MasterDatas;
using FasTnT.PostgreSql.DapperConfiguration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Data.PostgreSql.DapperConfiguration
{
    public class MasterdataDtoManager
    {
        public List<MasterDataDto> MasterDataDtos { get; set; } = new List<MasterDataDto>();
        public List<MasterDataAttributeDto> AttributeDtos { get; set; } = new List<MasterDataAttributeDto>();
        public List<MasterDataFieldDto> FieldDtos { get; set; } = new List<MasterDataFieldDto>();
        public List<MasterDataHierarchyDto> HierarchyDtos { get; set; } = new List<MasterDataHierarchyDto>();

        public void AddMasterdata(EpcisMasterData masterdata)
        {
            MasterDataDtos.Add(MasterDataDto.Create(masterdata));

            foreach(var attribute in masterdata.Attributes)
            {
                var attributeDto = MasterDataAttributeDto.Create(attribute, masterdata.Id, masterdata.Type);

                AttributeDtos.Add(attributeDto);
                FieldDtos.AddRange(attribute.Fields.ToFlattenedDtos(attributeDto));
            }

            HierarchyDtos.AddRange(masterdata.Children.Select(x => MasterDataHierarchyDto.Create(x, masterdata.Id, masterdata.Type)));
        }

        internal async Task PersistAsync(IDbTransaction transaction, CancellationToken cancellationToken)
        {
            foreach (var masterdata in MasterDataDtos)
            {
                var command = new CommandDefinition(SqlQueries.Delete_MasterData, masterdata, transaction, cancellationToken: cancellationToken);
                await transaction.Connection.ExecuteAsync(command);
            }

            await transaction.BulkInsertAsync(MasterDataDtos, cancellationToken);
            await transaction.BulkInsertAsync(AttributeDtos, cancellationToken);
            await transaction.BulkInsertAsync(FieldDtos, cancellationToken);
            await transaction.BulkInsertAsync(HierarchyDtos, cancellationToken);
        }

        internal IEnumerable<EpcisMasterData> FormatMasterdata()
        {
            throw new NotImplementedException();
        }
    }
}
