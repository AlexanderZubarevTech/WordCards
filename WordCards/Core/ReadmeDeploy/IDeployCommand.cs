using WordCards.Core.Commands;

namespace WordCards.Core.ReadmeDeploy
{
    internal interface IDeployCommand : IEntityCommand
    {
        public void Execute();
    }
}
