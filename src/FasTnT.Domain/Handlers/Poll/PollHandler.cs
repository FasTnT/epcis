﻿using FasTnT.Commands.Requests;
using FasTnT.Commands.Responses;
using FasTnT.Domain.Queries;
using FasTnT.Model.Exceptions;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.Handlers.Poll
{
    public class PollHandler : IRequestHandler<PollRequest, IEpcisResponse>
    {
        private readonly IEnumerable<IEpcisQuery> _queries;

        public PollHandler(IEnumerable<IEpcisQuery> queries)
        {
            _queries = queries;
        }

        public async Task<IEpcisResponse> Handle(PollRequest request, CancellationToken cancellationToken)
        {
            var query = _queries.SingleOrDefault(q => q.Name == request.QueryName);

            if (query != null)
            {
                return await query.Handle(request.Parameters, cancellationToken);
            }
            else
            {
                throw new EpcisException(ExceptionType.NoSuchNameException, $"Query with name '{request.QueryName}' is not implemented");
            }
        }
    }
}
