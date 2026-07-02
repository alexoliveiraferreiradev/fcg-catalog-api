using Fcg.Core.Abstractions.Common;
using Fcg.Core.Abstractions.Resources;

namespace Fcg.Catalog.Domain.Entities
{
    public class Biblioteca : AggregateRoot
    {       
        public Guid UsuarioId { get; private set; }
        public virtual Jogo Jogo { get; private set; }
        public Guid JogoId { get; private set; }
        public DateTime DataCadastro { get; private set; }
        public DateTime DataAlteracao { get; private set; }
        public bool Ativo { get; private set; }

        public Biblioteca(Guid usuarioId, Guid jogoId)
        {
            UsuarioId = usuarioId;
            JogoId = jogoId;
            DataCadastro = DateTime.UtcNow;
            DataAlteracao = DataCadastro;
            Ativo = true;
            ValidateEntity();
        }

        protected override void ValidateEntity()
        {
            AssertionConcern.AssertArgumentNotEquals(UsuarioId, Guid.Empty, MensagensDominio.UsuarioNaoEncontrado);
            AssertionConcern.AssertArgumentNotEquals(JogoId, Guid.Empty, MensagensDominio.JogoNaoEncontrado);
        }

    }
}
