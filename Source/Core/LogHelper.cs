using System;
using System.Diagnostics;
using BepInEx.Logging;
using JetBrains.Annotations;

namespace ClosingBattle.Core;

/// <summary>
///    Helper class for logging with automatic namespace prefixes.
/// </summary>
[PublicAPI]
public static class LogHelper
{
    public static void LogInfo(object message) => Plugin.Logger.LogInfo(TryAddPrefix(message));

    public static void LogWarning(object message) => Plugin.Logger.LogWarning(TryAddPrefix(message));
    public static void LogError(object message) => Plugin.Logger.LogError(TryAddPrefix(message));
    public static void LogDebug(object message) => Plugin.Logger.LogDebug(TryAddPrefix(message));
    public static void LogFatal(object message) => Plugin.Logger.LogFatal(TryAddPrefix(message));
    public static void LogMessage(object message) => Plugin.Logger.LogMessage(TryAddPrefix(message));
    public static void Log(LogLevel logLevel, object data) => Plugin.Logger.Log(logLevel, TryAddPrefix(data));

    private static object TryAddPrefix(object message)
    {
        if (message is not string str) 
            return message;
       
        var stackTrace = new StackTrace();
        var frame = stackTrace.GetFrame(2); // 0 = this method, 1 = caller (Log), 2 = original caller
        if (frame == null)
            return message;

        var method = frame.GetMethod();
        if (method == null)
            return message;

        var declaringType = method.DeclaringType;
        if (declaringType == null)
            return message;

        var ns = declaringType.Namespace;
        if (string.IsNullOrEmpty(ns))
            return message;

        var parts = ns.Split('.');
        if (parts.Length <= 1 || string.IsNullOrEmpty(parts[1]))
            return message;

        var name = parts[1];
        return $"[{name}] {str}";
    }
}