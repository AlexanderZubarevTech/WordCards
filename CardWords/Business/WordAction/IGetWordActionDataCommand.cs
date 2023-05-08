using CardWords.Core.Commands;

namespace CardWords.Business.WordAction
{
    interface IGetWordActionDataCommand : IEntityCommand
    {
        public WordActionData[] Execute(int count);
    }
}
