﻿using System.Threading.Tasks;
using Updater.Core.Commands;

namespace Updater.Business
{
    public interface ILoadUpdatersCommand : IEntityCommand
    {
        Task<string> Execute();
    }
}
