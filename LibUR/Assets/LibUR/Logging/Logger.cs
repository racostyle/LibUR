using UnityEngine;

namespace LibUR.Logging
{
    internal class Logger : ILogger
    {
        public void Log(string message)
        {
            Debug.Log(message);
        }
    }
}
