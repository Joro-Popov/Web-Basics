using SIS.Framework;

namespace TORSHIA.App
{
    public class Launcher
    {
        public static void Main(string[] args)
        {
            WebHost.Start(new Startup());
        }
    }
}
