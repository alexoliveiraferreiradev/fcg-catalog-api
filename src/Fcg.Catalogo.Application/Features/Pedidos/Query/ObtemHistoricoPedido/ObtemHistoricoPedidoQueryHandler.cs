using Fcg.Catalogo.Application.Features.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Catalogo.Application.Features.Pedidos.Query.ObtemHistoricoPedido
{
    public class ObtemHistoricoPedidoQueryHandler : IRequestHandler<ObtemHistoricoPedidoQuery, IEnumerable<PedidoHistoricoResponse>>
    {
        public Task<IEnumerable<PedidoHistoricoResponse>> Handle(ObtemHistoricoPedidoQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
