using System;
using System.Linq;
using System.Net.NetworkInformation;
using UpdaterLibrary.Commands;

namespace UpdaterLibrary.Connection
{
    internal sealed class CheckConnectionCommand : EntityCommand, ICheckConnectionCommand
    {
        public bool Execute(params string[] hostNames)
        {
            if (hostNames == null || hostNames.Length == 0)
            {
                return AreAllAvailable("google.com", "microsoft.com");
            }

            return AreAllAvailable(hostNames);
        }

        private static bool AreAllAvailable(params string[] hostNameOrAddresses)
        {
            if (hostNameOrAddresses == null)
            {
                throw new ArgumentNullException("hostNameOrAddresses");
            }

            if (hostNameOrAddresses.Length == 0)
            {
                throw new ArgumentException("hostNameOrAddresses must have at least one element", "hostNameOrAddresses");
            }

            var result = from hostNameOrAddress in hostNameOrAddresses.AsParallel().WithDegreeOfParallelism(hostNameOrAddresses.Length)
                         let p = new MultiplePing().Send(hostNameOrAddress)
                         select new
                         {
                             HostNameOrAddress = hostNameOrAddress,
                             p.AllFailed
                         };
            try
            {
                return !result.Any(r => r.AllFailed);
            }
            catch (AggregateException ex)
            {
                // Игнорируем PingException. Остальным даем выйти наружу.
                ex.Handle(_ => _.GetType() == typeof(PingException));
            }

            return false;
        }
    }
}
