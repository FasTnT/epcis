using Dapper;
using Faithlife.Utility.Dapper;
using FasTnT.Data.PostgreSql.Capture;
using FasTnT.Data.PostgreSql.DTOs;
using FasTnT.Model.MasterDatas;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.PostgreSql.Capture
{
    public static class EpcisMasterdataStore
    {
        public static async Task StoreEpcisMasterdata(IEnumerable<EpcisMasterData> masterdataList, IDbTransaction transaction, CancellationToken cancellationToken)
        {
            foreach (var masterData in masterdataList)
            {
                await transaction.Connection.ExecuteAsync(new CommandDefinition(CaptureEpcisMasterdataCommands.UpsertMasterData, masterData, transaction, cancellationToken: cancellationToken));
                await StoreMasterdataAttributes(transaction, masterData, cancellationToken);
            }

            var hierarchies = masterdataList.SelectMany(x => x.Children.Select(c => new EpcisMasterDataHierarchy { Type = x.Type, ChildrenId = c.ChildrenId, ParentId = x.Id }));
            await transaction.Connection.BulkInsertAsync(CaptureEpcisMasterdataCommands.HierarchyInsert, hierarchies, transaction, cancellationToken: cancellationToken);
        }

        private static async Task StoreMasterdataAttributes(IDbTransaction transaction, EpcisMasterData masterData, CancellationToken cancellationToken)
        {
            foreach (var attribute in masterData.Attributes)
            {
                var output = new List<MasterDataFieldDto>();
                ParseFields(attribute.Fields, output);

                await transaction.Connection.ExecuteAsync(new CommandDefinition(CaptureEpcisMasterdataCommands.AttributeInsert, attribute, transaction, cancellationToken: cancellationToken));
                await transaction.Connection.BulkInsertAsync(CaptureEpcisMasterdataCommands.AttributeFieldInsert, output, transaction, cancellationToken: cancellationToken);
            }
        }

        private static void ParseFields(IEnumerable<MasterDataField> fields, List<MasterDataFieldDto> output, int? parentId = null)
        {
            foreach (var field in fields ?? new MasterDataField[0])
            {
                output.Add(MasterDataFieldDto.Create(field, output.Count, parentId));
                ParseFields(field.Children, output, output.Count - 1);
            }
        }
    }
}
