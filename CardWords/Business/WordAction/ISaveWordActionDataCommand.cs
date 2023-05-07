using CardWords.Core.Commands;
using System.Collections.Generic;

namespace CardWords.Business.WordAction
{
    interface ISaveWordActionDataCommand : IEntityCommand
    {
        public bool Execute(List<WordActionData> data);
    }
}
