using Dapper;
using Faithlife.Utility.Dapper;
using FasTnT.Data.PostgreSql.Capture;
using FasTnT.Domain.Data.Model;
using FasTnT.Model.MasterDatas;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FasTnT.PostgreSql.Capture
{
    public static class EpcisMasterdataStore
    {
        public static async Task StoreEpcisMasterdata(CaptureDocumentRequest request, IDbConnection connection, int headerId)
        {
            if (request.Payload.MasterDataList == null || !request.Payload.MasterDataList.Any()) return;

            foreach (var masterData in request.Payload.MasterDataList)
            {
                await connection.ExecuteAsync(new CommandDefinition(CaptureEpcisMasterdataCommands.Delete, masterData, request.Transaction, cancellationToken: request.CancellationToken));
                await connection.ExecuteAsync(new CommandDefinition(CaptureEpcisMasterdataCommands.Insert, masterData, request.Transaction, cancellationToken: request.CancellationToken));

                foreach (var attribute in masterData.Attributes)
                {
                    var output = new List<MasterDataField>();
                    ParseFields(attribute.Fields, output);

                    await connection.ExecuteAsync(new CommandDefinition(CaptureEpcisMasterdataCommands.AttributeInsert, attribute, request.Transaction, cancellationToken: request.CancellationToken));
                    await connection.BulkInsertAsync(CaptureEpcisMasterdataCommands.AttributeFieldInsert, output, request.Transaction, cancellationToken: request.CancellationToken);
                }
            }

            var hierarchies = request.Payload.MasterDataList.SelectMany(x => x.Children.Select(c => new EpcisMasterDataHierarchy { Type = x.Type, ChildrenId = c.ChildrenId, ParentId = x.Id }));
            await connection.BulkInsertAsync(CaptureEpcisMasterdataCommands.HierarchyInsert, hierarchies, request.Transaction, cancellationToken: request.CancellationToken);
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
