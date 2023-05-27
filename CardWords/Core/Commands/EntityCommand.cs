using System;

namespace WordCards.Core.Commands
{
    public abstract class EntityCommand : IEntityCommand
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
