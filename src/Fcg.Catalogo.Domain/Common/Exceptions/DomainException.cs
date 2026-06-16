namespace Fcg.Catalogo.Domain.Common.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException()
        {

        }
        /// <summary>
        /// Cria instância de DomainException com mensagem personalizada
        /// </summary>
        /// <param name="message"></param>
        public DomainException(string message) : base(message)
        {
        }

        /// <summary>
        /// Cria instância de DomainException com mensagem personalizada e nome do parâmetro relacionado
        /// </summary>
        /// <param name="message"></param>
        /// <param name="paramName"></param>
        public DomainException(string message, string paramName) : base($"{message} (Parâmetro: {paramName})")
        {
        }
    }
}
