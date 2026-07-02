using Fcg.Catalog.Domain.Enum;
using Fcg.Catalog.Domain.ValueObject;
using Fcg.Core.Abstractions.Common;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Resources;

namespace Fcg.Catalog.Domain.Entities
{
    public class Jogo : AggregateRoot
    {
        public Nome Nome { get; private set; }
        public Descricao Descricao { get; private set; }
        public Preco PrecoBase { get; private set; }
        public bool Ativo { get; private set; }
        public DateTime DataCadastro { get; private set; }
        public DateTime DataAlteracao { get; private set; }
        public GeneroJogo Genero { get; private set; }
        private List<Promocao> _promocoes = new List<Promocao>();
        public IReadOnlyCollection<Promocao> Promocoes => _promocoes;

        protected Jogo()
        {
        }


        public Jogo(Nome nomeJogo, Descricao descricaoJogo, Preco precoJogo, GeneroJogo generoJogo)
        {
            Nome = nomeJogo;
            Descricao = descricaoJogo;
            PrecoBase = precoJogo;
            Genero = generoJogo;
            Ativo = true;
            DataCadastro = DateTime.UtcNow;
            DataAlteracao = DataCadastro;
            ValidateEntity();
        }

        protected override void ValidateEntity()
        {
            AssertionConcern.AssertArgumentRange((int)Genero, 1, 20, MensagensDominio.JogoGeneroObrigatorio);
        }

        public void Desativar()
        {
            if (!Ativo) throw new DomainException(MensagensDominio.JogoInvalido);
            Ativo = false;
            DataAlteracao = DateTime.UtcNow;
        }

        public void Reativar()
        {
            if (Ativo) throw new DomainException(MensagensDominio.JogoAtivo);
            Ativo = true;
            DataAlteracao = DateTime.UtcNow;
        }

        public void Atualizar(Nome novoNome, Descricao novaDescricao, Preco novoPreco, GeneroJogo novoGenero)
        {
            if (!Ativo) throw new DomainException(MensagensDominio.JogoInvalido);

            AtualizarNome(novoNome);
            AtualizarDescricao(novaDescricao);
            AtualizarPreco(novoPreco);
            AtualizarGenero(novoGenero);
            DataAlteracao = DateTime.UtcNow;
        }

        private void AtualizarGenero(GeneroJogo novoGenero)
        {
            AssertionConcern.AssertArgumentRange((int)novoGenero, 1, 20, MensagensDominio.JogoGeneroObrigatorio);
            if (Genero == novoGenero) return;
            Genero = novoGenero;
        }

        private void AtualizarPreco(Preco novoPreco)
        {
            if (PrecoBase == novoPreco) return;
            PrecoBase = novoPreco;
        }

        private void AtualizarDescricao(Descricao novaDescricao)
        {
            AssertionConcern.AssertArgumentNotNull(novaDescricao, MensagensDominio.JogoDescricaoObrigatoria);
            if (Descricao == novaDescricao) return;
            Descricao = novaDescricao;
        }

        private void AtualizarNome(Nome novoNome)
        {
            AssertionConcern.AssertArgumentNotNull(novoNome, MensagensDominio.JogoNomeObrigatorio);
            if (Nome == novoNome) return;
            Nome = novoNome;
        }

        public void AdicionarPromocao(Preco valorPromocao, Periodo dataFim)
        {
            if (valorPromocao.Valor >= PrecoBase.Valor) throw new DomainException(MensagensDominio.PromocaoValorMaior);
            foreach (var p in _promocoes.Where(x => x.Ativo)) p.Desativar();
            _promocoes.Add(new Promocao(Id, valorPromocao, dataFim));
            DataAlteracao = DateTime.UtcNow;
        }
        public void AlteraPromocao(Guid promocaoId, Preco novoPreco, DateTime novaDataFim)
        {
            if (novoPreco.Valor >= PrecoBase.Valor) throw new DomainException(MensagensDominio.PromocaoValorMaior);
            foreach (var p in _promocoes.Where(x => x.Id == promocaoId)) p.AtualizarPromocao(novoPreco, novaDataFim);
        }
        public Preco ObterPrecoAtual()
        {
            var promoAtiva = _promocoes.FirstOrDefault(p => p.EstaValida());

            var precoReferencia = promoAtiva != null ? promoAtiva.ValorPromocao : PrecoBase;

            return new Preco(precoReferencia.Valor);
        }
        public void DesativarPromocao(Guid promocaoId)
        {
            var promocao = _promocoes.FirstOrDefault(x => x.Id == promocaoId);
            if (promocao == null) throw new DomainException(MensagensDominio.PromocaoNaoEncontrada);
            promocao.Desativar();
            DataAlteracao = DateTime.UtcNow;
        }
    }
}
