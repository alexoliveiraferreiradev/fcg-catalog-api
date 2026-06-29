using Fcg.Catalogo.Application.Features.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Catalogo.Application.Features.Pedidos.Commands.RealizarPedido
{
    public record RealizarPedidoCommand(Guid UsuarioId, List<Guid> JogosIds) : IRequest<PedidoResponse>;
}
