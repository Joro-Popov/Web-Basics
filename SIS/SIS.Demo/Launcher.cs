namespace SIS.Demo
{
    using Controllers;
    using HTTP.Enums;
    using Framework.Routers;
    using WebServer;
    using Framework;

    public class Launcher
    {
        public static void Main(string[] args)
        {
            //var routingTable = new ServerRoutingTable();

            //routingTable.Routes[HttpRequestMethod.Get]["/Home/Index"] = request => new HomeController().Index();
            //var server = new WebServer(80, routingTable);

            var controllerRouter = new ControllerRouter(); 

            var server = new WebServer(80, controllerRouter);

            MvcEngine.Run(server);
        }
    }
}
