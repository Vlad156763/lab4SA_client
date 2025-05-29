using System.Runtime.CompilerServices;
using System;
using lab4.Interface;


namespace lab4.Models {
    public class LoggerComponent : ILogger {
        private void log(string message, string TypeLog, ConsoleColor color, bool is_fullPath = false, [CallerFilePath] string fullPathFile = "",[CallerLineNumber] int line = 0) {
            string fileLog = is_fullPath ? fullPathFile : System.IO.Path.GetFileName(fullPathFile);
            Console.Write($"[{fileLog} : {line}] ");
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(TypeLog);
            Console.ForegroundColor = previousColor;
            Console.WriteLine(message);
        }
        public void error(string message, bool is_fullPath = false, [CallerFilePath] string fullPathFile = "", [CallerLineNumber] int line = 0) {
            log(message,"ERROR: ", ConsoleColor.Red, is_fullPath, fullPathFile, line);
        }
        public void warn(string message, bool is_fullPath = false, [CallerFilePath] string fullPathFile = "", [CallerLineNumber] int line = 0) {
            log(message,"WARNING: ", ConsoleColor.Yellow, is_fullPath, fullPathFile, line);
        }
        public void debug(string message, bool is_fullPath = false, [CallerFilePath] string fullPathFile = "", [CallerLineNumber] int line = 0) {
            log(message,"DEBUG: ", ConsoleColor.Cyan, is_fullPath, fullPathFile, line);
        }
    }
}
