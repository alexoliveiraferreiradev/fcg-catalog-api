using Fcg.Catalog.Domain.Enum;

namespace Fcg.Catalog.Application.Features.Response
{
    public class PedidoResponse
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public DateTime DataPedido { get; set; }
        public PedidoStatus Status { get; set; }
        public decimal ValorTotal { get; set; }
        public List<string> MensagensInformativas { get; set; } = new();
        public IEnumerable<PedidoItemResponse> Items { get; set; }
        public PedidoResponse()
        {

        }
    }
}
