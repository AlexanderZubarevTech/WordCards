using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace WordCards.Core.Connection
{
    public sealed class MultiplePing
    {
        readonly int _repeat;
        static byte[] defaultPingData;
        const int TIMEOUT_MS = 5000;
        const int PAUSE_MS = 500;

        static MultiplePing()
        {
            defaultPingData = new byte[32];

            for (int i = 0; i < defaultPingData.Length; i++)
            {
                defaultPingData[i] = (byte)(97 + i % 23);
            }
        }

        public MultiplePing(int repeat = 4)
        {
            if (repeat < 1) 
            { 
                throw new ArgumentOutOfRangeException("repeat", "repeat must be greater than zero");
            }

            _repeat = repeat;
        }

        public MultiplePingReply Send(string hostNameOrAddress)
        {
            using (var ping = new Ping())
            {
                int countSuccess = 0, countFailure = 0;
                for (int i = 0; i < _repeat; i++)
                {
                    try
                    {
                        PingReply reply = ping.Send(hostNameOrAddress, TIMEOUT_MS, defaultPingData);
                        if (reply.Status == IPStatus.Success) countSuccess++;
                        else countFailure++;
                    }
                    catch (PingException)
                    {
                        countFailure++;
                    }

                    Thread.Sleep(PAUSE_MS);
                }

                Debug.Assert(countSuccess + countFailure == _repeat);

                return new MultiplePingReply(countSuccess, countFailure);
            }
        }

        public async Task<MultiplePingReply> SendAsync(string hostNameOrAddress)
        {
            using (var ping = new Ping())
            {
                int countSuccess = 0, countFailure = 0;

                for (int i = 0; i < _repeat; i++)
                {
                    try
                    {
                        PingReply reply = await ping.SendPingAsync(hostNameOrAddress, TIMEOUT_MS, defaultPingData);

                        if (reply.Status == IPStatus.Success)
                        {
                            countSuccess++;
                        }
                        else
                        {
                            countFailure++;
                        }
                    }
                    catch (PingException)
                    {
                        countFailure++;
                    }

                    await Task.Delay(PAUSE_MS);
                }

                Debug.Assert(countSuccess + countFailure == _repeat);

                return new MultiplePingReply(countSuccess, countFailure);
            }
        }
    }
}
