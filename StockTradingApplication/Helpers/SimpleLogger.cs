using System;
using System.IO;

namespace StockTradingApplication.Helpers
{
    public class SimpleLogger
    {
        private readonly string _filePath;

        public SimpleLogger(string filePath)
        {
            _filePath = filePath;
            EnsureDirectoryExists();
        }

        private void EnsureDirectoryExists()
        {
            var directory = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public void Log(string message)
        {
            lock (_filePath) // Ensure only one thread writes to the file at a time
            {
                File.AppendAllText(_filePath, $"{DateTime.Now}: {message}{Environment.NewLine}");
            }
        }

        public void LogError(string message)
        {
            Log($"ERROR: {message}");
        }

        public void LogInfo(string message)
        {
            Log($"INFO: {message}");
        }
    }
}
