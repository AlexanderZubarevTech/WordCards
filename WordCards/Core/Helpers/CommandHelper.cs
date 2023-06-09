﻿using System;
using System.Linq;
using WordCards.Core.Commands;

namespace WordCards.Core.Helpers
{
    public sealed class CommandHelper
    {
        private CommandHelper() { }

        public static TInterfaceCommand GetCommand<TInterfaceCommand>()
            where TInterfaceCommand : IEntityCommand
        {
            var type = typeof(TInterfaceCommand);

            var implType = type.Assembly.DefinedTypes.First(x => x.ImplementedInterfaces.Contains(type));

            var constructor = implType.GetConstructors().First();

            return (TInterfaceCommand)constructor.Invoke(null);
        }
    }
}
