namespace WordCards.Core.ReadmeDeploy
{
    public sealed partial class DeployCommand
    {
        private sealed class DeployTransactionCheck
        {
            public DeployTransactionCheck(string sql)
            {
                Sql = sql;
                IsChecked = false;
            }

            public string Sql { get; }

            public bool IsChecked { get; set; }
        }
    }
}
