using BepInEx.Logging;
using System;
using System.Security;
using System.Security.Permissions;

namespace DeadlockDash
{
    internal static class Log
    {
        private const string Prefix = "[DeadlockDash]: ";

        internal static ManualLogSource _logSource;

        internal static void Init(ManualLogSource logSource)
        {
            _logSource = logSource;
        }

        internal static void Debug(object data) => Write(LogLevel.Debug, data);
        internal static void Error(object data) => Write(LogLevel.Error, data);
        internal static void ErrorAssetBundle(string assetName, string bundleName) =>
            Error($"failed to load asset '{assetName}' because it does not exist in asset bundle '{bundleName}'.");
        internal static void Error(Exception exception, string context = null)
        {
            if (exception == null)
            {
                Error(context ?? "unknown exception");
                return;
            }

            string message = string.IsNullOrEmpty(context) ? exception.ToString() : $"{context}: {exception}";
            Error(message);
        }

        internal static void Fatal(object data) => Write(LogLevel.Fatal, data);
        internal static void Info(object data) => Write(LogLevel.Info, data);
        internal static void Message(object data) => Write(LogLevel.Message, data);
        internal static void Warning(object data) => Write(LogLevel.Warning, data);

        private static void Write(LogLevel level, object data)
        {
            string message = Prefix + (data?.ToString() ?? "<null>");
            if (_logSource != null)
            {
                _logSource.Log(level, message);
            }
        }
    }
}
