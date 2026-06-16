namespace Fcg.Catalogo.Domain.Resources
{
    public class MensagensDominio
    {
        public static string JogoNomeObrigatorio = "O nome do jogo é obrigatório.";
        public static string JogoTamanhoNomeInvalido = "O nome do jogo deve conter entre 3 e 20 caracteres.";
        public static string JogoDescricaoTamanhoInvalido = "A descrição do jogo deve conter entre 5 e 100 caracteres.";
        public static string JogoDescricaoObrigatoria = "A descrição do jogo é obrigatória.";
        public static string JogoGeneroObrigatorio = "O gênero do jogo é inválido.";
        public static string JogoNaoEncontrado = "O jogo não foi encontrado.";
        public static string JogoInvalido = "O jogo está desativado.";
        public static string JogoAtivo = "O jogo já está ativo.";
        public static string JogoMesmoNomeExistente = "Já existe um jogo com esse nome.";
        public static string JogoSemPromocoes = "Não há promoções para este jogo.";
        public static string JogoPromocoes = "Já há promoções para este jogo.";
        public static string NomeJogoNaoReal = "Por favor, informe um nome de jogo real.";
        public static string DescricaoJogoNaoReal = "Por favor, informe um descrição de jogo real.";
        public static string ValorInvalido = "O valor deve ser positivo.";
        public static string DataFimInvalida = "A data final deve ser no futuro.";
        public static string UsuarioNaoEncontrado = "O usuário não foi encontrado.";

        #region Promoção
        public static string PeriodoObrigatorio = "O período da promoção é obrigatório.";
        public static string PromocaoNaoEncontrada = "Promoção não encontrada.";
        public static string PromocaoValorMaior = "O valor promocional deve ser menor que o preço base.";
        public static string PromocaoInativa = "A promoção já está desativada.";
        #endregion
    }
}
