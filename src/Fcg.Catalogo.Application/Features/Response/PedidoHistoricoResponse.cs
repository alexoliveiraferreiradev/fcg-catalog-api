using Fcg.Catalogo.Domain.Enum;

namespace Fcg.Catalogo.Application.Features.Response
{
    public class PedidoHistoricoResponse
    {
        public Guid PedidoId { get; set; }
        public Guid UsuarioId { get; set; }
        public DateTime DataPedido { get; set; }
        public PedidoStatus Status { get; set; }
        public decimal ValorTotal { get; set; }
        public List<PedidoItemResponse> Items { get; set; } = new();
        public PedidoHistoricoResponse()
        {

        }
    }
}
