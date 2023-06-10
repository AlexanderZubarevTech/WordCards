using System.Collections.Generic;

namespace WordCards.Core.ReadmeDeploy
{
    public sealed partial class DeployCommand
    {
        private sealed class DeployFileInfo
        {
            public DeployFileInfo(string id)
                : this(id, null, null)
            {
            }            

            public DeployFileInfo(string id, List<DeployTransaction>? transactions, List<string>? actions) 
            {
                Id = id;
                Transactions = transactions ?? new List<DeployTransaction>(0);
                Actions = actions ?? new List<string>(0);
            }

            public string Id { get; }

            public IReadOnlyList<DeployTransaction> Transactions { get; }

            public IReadOnlyList<string> Actions { get; }

            public bool IsEmpty => Transactions.Count == 0 && Actions.Count == 0;
        }
    }
}
