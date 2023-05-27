using WordCards.Core.Commands;

namespace WordCards.Core.ReadmeDeploy
{
    internal interface INeedDeployCommand : IEntityCommand
    {
        public bool Execute();
    }
}
