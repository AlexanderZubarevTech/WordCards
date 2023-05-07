using CardWords.Core.Commands;
using System.Collections.Generic;

namespace CardWords.Business.WordAction
{
    interface IGetWordActionDataCommand : IEntityCommand
    {
        public List<WordActionData> Execute(int count);
    }
}
