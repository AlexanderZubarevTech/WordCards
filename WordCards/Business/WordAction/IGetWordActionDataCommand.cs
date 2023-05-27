using WordCards.Core.Commands;

namespace WordCards.Business.WordAction
{
    interface IGetWordActionDataCommand : IEntityCommand
    {
        public WordActionData[] Execute(int count);
    }
}
