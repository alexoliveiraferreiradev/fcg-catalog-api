using Fcg.Catalogo.Domain.Common.Exceptions;

namespace Fcg.Catalogo.Domain.Common
{
    public class AssertionConcern
    {
        public static void AssertArgumentRealValues(string value, string message)
        {            
            if (value.Trim().Equals("Nome do Jogo", StringComparison.OrdinalIgnoreCase))
            {
                throw new DomainException(message);
            }
            if (value.Trim().Equals("Descrição do Jogo", StringComparison.OrdinalIgnoreCase))
            {
                throw new DomainException(message);
            }
        }

        /// <summary>
        /// Valida se uma string é nula, vazia ou composta apenas por espaços em branco, e lança uma exceção de domínio com a mensagem fornecida caso seja.
        /// </summary>
        /// <param name="stringValue"></param>
        /// <param name="message"></param>
        /// <exception cref="DomainException"></exception>
        public static void AssertArgumentEmpty(string stringValue, string message)
        {
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                throw new DomainException(message);
            }
        }

        /// <summary>
        /// Valida se o comprimento de uma string está dentro do intervalo especificado (inclusivo),
        /// e lança uma exceção de domínio com a mensagem fornecida caso esteja fora do intervalo.
        /// </summary>
        /// <param name="stringValue"></param>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <param name="message"></param>
        /// <exception cref="DomainException"></exception>
        public static void AssertArgumentLength(string stringValue, int minimum, int maximum, string message)
        {
            if (minimum > maximum)
            {
                throw new DomainException("valor mínimo deve ter menos ou igual ao valor máximo", nameof(minimum));
            }

            if (stringValue == null)
            {
                throw new DomainException(message);
            }

            int length = stringValue.Trim().Length;
            if (length < minimum || length > maximum)
            {
                throw new DomainException(message);
            }
        }

        /// <summary>
        /// Valida se o valor decimal é negativo, e lança uma exceção de domínio com a mensagem fornecida caso seja negativo.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="message"></param>
        /// <exception cref="DomainException"></exception>
        public static void AssertArgumentValueFormat(decimal value, string message)
        {
            if (value < 0) throw new DomainException(message);
        }

        /// <summary>
        /// Valida se um objeto é nulo e lança uma exceção de domínio com a mensagem fornecida caso seja nulo.
        /// </summary>
        /// <param name="object1"></param>
        /// <param name="message"></param>
        /// <exception cref="DomainException"></exception>
        public static void AssertArgumentNotNull(object object1, string message)
        {
            if (object1 == null)
            {
                throw new DomainException(message);
            }
        }

        /// <summary>
        /// Valida se um valor inteiro está dentro de um intervalo especificado (inclusivo), e lança uma exceção de domínio com a mensagem fornecida caso esteja fora do intervalo.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <param name="message"></param>
        /// <exception cref="DomainException"></exception>
        public static void AssertArgumentRange(int value, int minimum, int maximum, string message)
        {
            if (value < minimum || value > maximum)
            {
                throw new DomainException(message);
            }
        }

        /// <summary>
        /// Valida se dois objetos são diferentes, utilizando o método Equals, e lança uma exceção de domínio com a mensagem fornecida caso sejam iguais.
        /// </summary>
        /// <param name="object1"></param>
        /// <param name="object2"></param>
        /// <param name="message"></param>
        /// <exception cref="DomainException"></exception>
        public static void AssertArgumentNotEquals(object object1, object object2, string message)
        {
            if (object1.Equals(object2))
            {
                throw new DomainException(message);
            }
        }
    }
}
