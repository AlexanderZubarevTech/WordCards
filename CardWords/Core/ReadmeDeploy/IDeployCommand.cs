using CardWords.Core.Commands;

namespace CardWords.Core.ReadmeDeploy
{
    internal interface IDeployCommand : IEntityCommand
    {
        public void Execute();
    }
}
