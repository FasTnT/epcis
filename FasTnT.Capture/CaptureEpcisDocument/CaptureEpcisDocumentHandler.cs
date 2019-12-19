using Dapper;
using FasTnT.Commands.Requests;
using FasTnT.Model;
using FasTnT.Model.MasterDatas;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Faithlife.Utility.Dapper;
using FasTnT.Commands.Responses;

namespace FasTnT.Handlers.CaptureEpcisDocument
{
    public class CaptureEpcisDocumentHandler : IRequestHandler<CaptureEpcisDocumentRequest, IEpcisResponse>
    {
        private readonly IDbConnection _connection;

        public CaptureEpcisDocumentHandler(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEpcisResponse> Handle(CaptureEpcisDocumentRequest request, CancellationToken cancellationToken)
        {
            using (var tx = _connection.BeginTransaction())
            {
                var headerId = await PersistHeader(request.Header, tx, cancellationToken);

                await PersistMasterData(headerId, request.MasterDataList, tx, cancellationToken);
                await PersistEvents(headerId, request.EventList, tx, cancellationToken);

                tx.Commit();
            }

            return EmptyResponse.Value;
        }

        private async Task<int> PersistHeader(EpcisRequestHeader header, IDbTransaction transaction, CancellationToken cancellationToken)
        {
            var commandDefinition = new CommandDefinition(
                commandText: CaptureEpcisDocumentCommands.PersistHeader, 
                parameters: header, 
                transaction: transaction, 
                cancellationToken: cancellationToken
            );

            var headerId = await _connection.QuerySingleAsync<int>(commandDefinition);

            await StoreStandardBusinessHeader(header, headerId, transaction, cancellationToken);
            await StoreCustomFields(header, headerId, transaction, cancellationToken);

            return headerId;
        }

        private async Task StoreCustomFields(EpcisRequestHeader header, int headerId, IDbTransaction transaction, CancellationToken cancellationToken)
        {
            await _connection.BulkInsertAsync(CaptureEpcisDocumentCommands.PersistHeader, header.CustomFields, transaction, cancellationToken: cancellationToken);
        }

        private async Task StoreStandardBusinessHeader(EpcisRequestHeader header, int headerId, IDbTransaction transaction, CancellationToken cancellationToken)
        {
            var contactInformations = header.StandardBusinessHeader.ContactInformations; // TODO: mappings.

            await _connection.ExecuteAsync("", header, transaction);
            await _connection.BulkInsertAsync("", contactInformations, transaction, cancellationToken: cancellationToken);
        }

        private Task PersistMasterData(int headerId, IList<EpcisMasterData> masterDataList, IDbTransaction transaction, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private Task PersistEvents(int headerId, IList<EpcisEvent> eventList, IDbTransaction transaction, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
