using System;
using UnityEngine;

namespace Helpers
{
    public static class DebugHelper
    {
        public static void LogException(string methodName, Exception exception)
        {
            var message = $"An exception occurred while executing {methodName}. Exception: {exception}";
            Debug.LogError(message);
        }
    }
}
