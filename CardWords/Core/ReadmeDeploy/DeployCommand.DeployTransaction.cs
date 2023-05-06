using System.Collections.Generic;
using System.Linq;

namespace CardWords.Core.ReadmeDeploy
{
    public sealed partial class DeployCommand
    {
        private sealed class DeployTransaction
        {
            public DeployTransaction(string sql) : this(sql, new List<DeployTransactionCheck>())
            {
            }

            public DeployTransaction(string sql, List<DeployTransactionCheck> checks)
            {
                Sql = sql;
                Checks = checks;
            }

            public string Sql { get; }

            public List<DeployTransactionCheck> Checks { get; }

            public bool Check()
            {
                if(Checks.Count == 0) 
                {
                    return true;
                }

                return Checks.All(x => x.IsChecked);
            }
        }
    }
}
