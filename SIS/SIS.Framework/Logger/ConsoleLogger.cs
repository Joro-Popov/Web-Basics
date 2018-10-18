using SIS.Framework.Logger.Contracts;

namespace SIS.Framework.Logger
{
    using System;

    public class ConsoleLogger : IConsoleLogger
    {
        public void Log(string content)
        {
            Console.WriteLine(content);
        }
    }
}
