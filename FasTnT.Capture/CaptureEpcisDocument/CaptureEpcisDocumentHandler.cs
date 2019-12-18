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

                await PersistMasterData(headerId, request.MasterDataList, tx);
                await PersistEvents(headerId, request.EventList, tx);

                tx.Commit();
            }

            return EmptyResponse.Default;
        }

        private async Task<int> PersistHeader(EpcisRequestHeader header, IDbTransaction tx, CancellationToken cancellationToken)
        {
            var commandDefinition = new CommandDefinition(
                commandText: CaptureEpcisDocumentCommands.PersistHeader, 
                parameters: header, 
                transaction: tx, 
                cancellationToken: cancellationToken
            );

            var headerId = await _connection.QuerySingleAsync<int>(commandDefinition);

            await StoreStandardBusinessHeader(header, headerId, tx, cancellationToken);
            await StoreCustomFields(header, headerId, tx, cancellationToken);

            return headerId;
        }

        private async Task StoreCustomFields(EpcisRequestHeader header, int headerId, IDbTransaction tx, CancellationToken cancellationToken)
        {
            await _connection.BulkInsertAsync(CaptureEpcisDocumentCommands.PersistHeader, header.CustomFields, tx, cancellationToken: cancellationToken);
        }

        private async Task StoreStandardBusinessHeader(EpcisRequestHeader header, int headerId, IDbTransaction tx, CancellationToken cancellationToken)
        {
            var contactInformations = header.StandardBusinessHeader.ContactInformations; // TODO: mappings.

            await _connection.ExecuteAsync("", header, tx);
            await _connection.BulkInsertAsync("", contactInformations, tx, cancellationToken: cancellationToken);
        }

        private Task PersistMasterData(int headerId, IList<EpcisMasterData> masterDataList, IDbTransaction tx)
        {
            throw new NotImplementedException();
        }

        private Task PersistEvents(int headerId, IList<EpcisEvent> eventList, IDbTransaction tx)
        {
            throw new NotImplementedException();
        }
    }
}
