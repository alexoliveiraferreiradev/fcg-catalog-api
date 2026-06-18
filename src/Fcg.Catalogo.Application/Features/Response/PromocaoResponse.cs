namespace Fcg.Catalogo.Application.Features.Response
{
    public class PromocaoResponse
    {
        public Guid PromocaoId { get; set; }
        public Guid JogoId { get; set; }
        public decimal ValorPromocao { get; set; }
        public string NomeJogo { get; set; }
        public string DescricaoJogo { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
    }
}
