using Fcg.Catalog.Application.Features.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Catalog.Application.Features.Orders.Query.GetOrderHistory
{
    public class GetOrderHistoryQueryHandler : IRequestHandler<GetOrderHistoryQuery, IEnumerable<OrderHistoryResponse>>
    {
        public Task<IEnumerable<OrderHistoryResponse>> Handle(GetOrderHistoryQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
