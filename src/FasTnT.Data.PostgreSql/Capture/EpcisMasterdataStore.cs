using Dapper;
using Faithlife.Utility.Dapper;
using FasTnT.Data.PostgreSql.Capture;
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
            if (masterdataList == null || !masterdataList.Any()) return;

            foreach (var masterData in masterdataList)
            {
                await transaction.Connection.ExecuteAsync(new CommandDefinition(CaptureEpcisMasterdataCommands.Delete, masterData, transaction, cancellationToken: cancellationToken));
                await transaction.Connection.ExecuteAsync(new CommandDefinition(CaptureEpcisMasterdataCommands.Insert, masterData, transaction, cancellationToken: cancellationToken));

                await StoreMasterdataAttributes(transaction, masterData, cancellationToken);
            }

            var hierarchies = masterdataList.SelectMany(x => x.Children.Select(c => new EpcisMasterDataHierarchy { Type = x.Type, ChildrenId = c.ChildrenId, ParentId = x.Id }));
            await transaction.Connection.BulkInsertAsync(CaptureEpcisMasterdataCommands.HierarchyInsert, hierarchies, transaction, cancellationToken: cancellationToken);
        }

        private static async Task StoreMasterdataAttributes(IDbTransaction transaction, EpcisMasterData masterData, CancellationToken cancellationToken)
        {
            foreach (var attribute in masterData.Attributes)
            {
                var output = new List<MasterDataField>();
                ParseFields(attribute.Fields, output);

                await transaction.Connection.ExecuteAsync(new CommandDefinition(CaptureEpcisMasterdataCommands.AttributeInsert, attribute, transaction, cancellationToken: cancellationToken));
                await transaction.Connection.BulkInsertAsync(CaptureEpcisMasterdataCommands.AttributeFieldInsert, output, transaction, cancellationToken: cancellationToken);
            }
        }

        private static void ParseFields(IEnumerable<MasterDataField> fields, List<MasterDataField> output, int? parentId = null)
        {
            foreach (var field in fields ?? new MasterDataField[0])
            {
                field.Id = output.Count;
                field.InternalParentId = parentId;

                output.Add(field);
                ParseFields(field.Children, output, output.Count - 1);
            }
        }
    }
}
