using System;
using System.Runtime.CompilerServices;
namespace lab4.Interface;

public interface ILogger {
        public void error(string message, bool is_fullPath = false, [CallerFilePath] string fullPathFile = "", [CallerLineNumber] int line = 0);
        public void warn(string message, bool is_fullPath = false, [CallerFilePath] string fullPathFile = "", [CallerLineNumber] int line = 0);
        public void debug(string message, bool is_fullPath = false, [CallerFilePath] string fullPathFile = "", [CallerLineNumber] int line = 0);
}