using Reminder.Core.Commands;

namespace Reminder.Core.ReadmeDeploy
{
    internal interface IDeployCommand : IEntityCommand
    {
        public void Execute();
    }
}
