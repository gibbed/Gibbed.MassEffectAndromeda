/* Copyright (c) 2017 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

namespace Gibbed.MassEffectAndromeda.Dumping
{
    internal static class LogHelper
    {
        public static void SetConfiguration(NLog.LogLevel logLevel)
        {
            var config = new NLog.Config.LoggingConfiguration();
            var consoleTarget = new NLog.Targets.ColoredConsoleTarget();
            consoleTarget.UseDefaultRowHighlightingRules = false;
            consoleTarget.RowHighlightingRules.Add(
                new NLog.Targets.ConsoleRowHighlightingRule(
                    "level == LogLevel.Fatal",
                    NLog.Targets.ConsoleOutputColor.Magenta,
                    NLog.Targets.ConsoleOutputColor.NoChange));
            consoleTarget.RowHighlightingRules.Add(
                new NLog.Targets.ConsoleRowHighlightingRule(
                    "level == LogLevel.Error",
                    NLog.Targets.ConsoleOutputColor.Red,
                    NLog.Targets.ConsoleOutputColor.NoChange));
            consoleTarget.RowHighlightingRules.Add(
                new NLog.Targets.ConsoleRowHighlightingRule(
                    "level == LogLevel.Warn",
                    NLog.Targets.ConsoleOutputColor.Yellow,
                    NLog.Targets.ConsoleOutputColor.NoChange));
            consoleTarget.RowHighlightingRules.Add(
                new NLog.Targets.ConsoleRowHighlightingRule(
                    "level == LogLevel.Info",
                    NLog.Targets.ConsoleOutputColor.Gray,
                    NLog.Targets.ConsoleOutputColor.NoChange));
            consoleTarget.RowHighlightingRules.Add(
                new NLog.Targets.ConsoleRowHighlightingRule(
                    "level == LogLevel.Debug",
                    NLog.Targets.ConsoleOutputColor.Gray,
                    NLog.Targets.ConsoleOutputColor.DarkGray));
            consoleTarget.RowHighlightingRules.Add(
                new NLog.Targets.ConsoleRowHighlightingRule(
                    "level == LogLevel.Trace",
                    NLog.Targets.ConsoleOutputColor.DarkGray,
                    NLog.Targets.ConsoleOutputColor.Gray));
            consoleTarget.Layout = @"${message}${onexception:${newline}${exception:format=tostring}}";
            config.AddTarget("console", consoleTarget);
            config.LoggingRules.Add(new NLog.Config.LoggingRule("*", logLevel, consoleTarget));
            NLog.LogManager.Configuration = config;
        }
    }
}
