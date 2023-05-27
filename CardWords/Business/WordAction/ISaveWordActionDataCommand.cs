using WordCards.Core.Commands;

namespace WordCards.Business.WordAction
{
    interface ISaveWordActionDataCommand : IEntityCommand
    {
        public bool Execute(WordActionData[] data, WordActionInfo info);
    }
}
