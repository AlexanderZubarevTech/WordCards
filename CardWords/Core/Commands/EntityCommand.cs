using System;

namespace Reminder.Core.Commands
{
    public class EntityCommand : IEntityCommand
    {
        public event EventHandler? CanExecuteChanged;

        public EntityCommand()
        {
        }

        public bool CanExecute(object? parameter)
        {
            return true;            
        }

        public void Execute(object? parameter)
        {
            return;
        }       
    }
}
