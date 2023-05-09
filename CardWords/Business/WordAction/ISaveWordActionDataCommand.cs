using CardWords.Core.Commands;

namespace CardWords.Business.WordAction
{
    interface ISaveWordActionDataCommand : IEntityCommand
    {
        public bool Execute(WordActionData[] data, WordActionInfo info);
    }
}
