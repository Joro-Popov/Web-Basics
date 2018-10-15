namespace SIS.Demo
{
    using Framework.Routers;
    using WebServer;
    using Framework;

    public class Launcher
    {
        public static void Main(string[] args)
        {
            var controllerRouter = new ControllerRouter(); 

            var server = new WebServer(80, controllerRouter);

            MvcEngine.Run(server);
        }
    }
}
