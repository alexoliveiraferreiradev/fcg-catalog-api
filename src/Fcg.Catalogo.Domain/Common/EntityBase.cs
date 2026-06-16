using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Catalogo.Domain.Common
{
    public abstract class EntityBase
    {
        public Guid Id { get; private set; }
        public EntityBase()
        {
            Id = Guid.NewGuid();
        }

        protected abstract void ValidarEntidade();
    }
}
