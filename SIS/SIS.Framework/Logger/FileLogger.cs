namespace SIS.Framework.Logger
{
    using System;
    using System.IO;
    using Contracts;

    public class FileLogger : IFileLogger
    {
        private const string DEFAULT_PATH = "Logs/log.txt";

        private readonly string path;
        
        public FileLogger(string path = DEFAULT_PATH)
        {
            this.path = path;
        }
        
        public void Log(string content)
        {
            File.AppendAllText(this.path, content);
            File.AppendAllText(this.path, Environment.NewLine);
        }
    }
}
