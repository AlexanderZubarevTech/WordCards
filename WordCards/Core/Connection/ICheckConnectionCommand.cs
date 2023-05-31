using System.Collections.Generic;
using System.Linq;
using System.Text;
using WordCards.Core.Commands;

namespace WordCards.Core.Connection
{
    public interface ICheckConnectionCommand : IEntityCommand
    {
        public bool Execute();
    }
}
