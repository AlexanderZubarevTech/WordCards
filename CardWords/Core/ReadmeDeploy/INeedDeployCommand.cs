using Reminder.Core.Commands;

namespace Reminder.Core.ReadmeDeploy
{
    internal interface INeedDeployCommand : IEntityCommand
    {
        public bool Execute();
    }
}
