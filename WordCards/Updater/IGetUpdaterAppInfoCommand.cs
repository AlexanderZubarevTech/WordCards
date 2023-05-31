﻿using System.Threading.Tasks;
using WordCards.Core.Commands;

namespace WordCards.Updater
{
    public interface IGetUpdaterAppInfoCommand : IEntityCommand
    {
        public Task<UpdaterAppInfo> Execute();
    }
}
