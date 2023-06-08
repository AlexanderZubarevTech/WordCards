using System;

namespace UpdaterLibrary.Connection
{
    internal sealed class MultiplePingReply
    {
        int _countSuccess, _countFailure;
        public MultiplePingReply(int success, int failure)
        {
            if (success < 0)
            {
                throw new ArgumentOutOfRangeException("success", "success must be positive number or zero");
            }

            if (failure < 0)
            {
                throw new ArgumentOutOfRangeException("failure", "failure must be positive number or zero");
            }

            if (success == 0 && failure == 0)
            {
                throw new ArgumentException("success and failure cannot be both zero");
            }

            _countSuccess = success;
            _countFailure = failure;
        }

        public bool AllSuccess => _countFailure == 0;

        public bool AllFailed => _countSuccess == 0;
    }
}
