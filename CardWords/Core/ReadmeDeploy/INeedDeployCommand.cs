using CardWords.Core.Commands;

namespace CardWords.Core.ReadmeDeploy
{
    internal interface INeedDeployCommand : IEntityCommand
    {
        public bool Execute();
    }
}
