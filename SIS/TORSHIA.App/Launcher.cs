namespace TORSHIA.App
{
    using SIS.Framework;

    public class Launcher
    {
        public static void Main(string[] args)
        {
            WebHost.Start(new Startup());
        }
    }
}
