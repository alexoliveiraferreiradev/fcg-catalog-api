namespace Fcg.Catalog.Application.Features.Response
{
    public class PedidoItemResponse
    {
        public Guid JogoId { get; set; }
        public string NomeJogo { get; set; }
        public decimal PrecoOriginal { get; set; }
        public decimal Desconto => PrecoOriginal - PrecoPago;
        public decimal PrecoPago { get; set; }

        public PedidoItemResponse()
        {

        }
    }
}
