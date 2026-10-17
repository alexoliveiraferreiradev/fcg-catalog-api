namespace Fcg.Catalog.Application.Common
{
    /// <summary>
    /// Caracteres aceitos no nome e na descrição do jogo, iguais no cadastro e na edição.
    /// </summary>
    public static class GameTextPatterns
    {
        /// <summary>Letras (com acentos), dígitos, espaços, dois-pontos e hífen.</summary>
        public const string Name = @"^[a-zA-ZáéíóúÁÉÍÓÚâêîôûÂÊÎÔÛãõÃÕçÇ0-9\s:\-]+$";

        /// <summary>Letras (com acentos), dígitos, espaços e . , ! ? ' " -</summary>
        public const string Description = @"^[a-zA-ZáéíóúÁÉÍÓÚâêîôûÂÊÎÔÛãõÃÕçÇ0-9\s.,!?'""\-]+$";
    }
}
