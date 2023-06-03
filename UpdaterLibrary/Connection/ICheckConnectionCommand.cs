using UpdaterLibrary.Commands;

namespace UpdaterLibrary.Connection
{
    internal interface ICheckConnectionCommand : IEntityCommand
    {
        bool Execute(params string[] hostNames);
    }
}
