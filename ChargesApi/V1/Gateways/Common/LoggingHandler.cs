using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;

namespace ChargesApi.V1.Gateways.Common
{
    public static class LoggingHandler
    {
        public static void LogError(string message)
        {
            LambdaLogger.Log($"[ERROR]: {message}");
        }

        public static void LogWarning(string message)
        {
            LambdaLogger.Log($"[WARNING]: {message}");
        }

        public static void LogInfo(string message)
        {
            LambdaLogger.Log($"[INFO]: {message}");
        }
    }
}
