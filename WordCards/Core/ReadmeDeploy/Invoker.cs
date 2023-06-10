using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace WordCards.Core.ReadmeDeploy
{
    internal static class Invoker
    {
        private static readonly IReadOnlyDictionary<string, Action> actions = new Dictionary<string, Action>()
        {
            {"temp", Temp}
        };

        public static bool InvokeAction(string name)
        {
            var success = false;

            if(actions.ContainsKey(name))
            {
                success = true;

                try
                {
                    actions[name].Invoke();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());

                    success = false;
                }
            }

            return success;
        }

        private static void Temp()
        {
        }
    }
}
