using SIS.Framework;

namespace IRunes.App
{
    using Controllers;
    using SIS.HTTP.Enums;
    using SIS.WebServer;
    using SIS.Framework.Routers;

    public class Startup
    {
        public static void Main()
        {
            var serverRoutingTable = new ServerRoutingTable();

            ConfigureRouting(serverRoutingTable);

            var server = new WebServer(80, new ControllerRouter());

            MvcEngine.Run(server);
        }

        private static void ConfigureRouting(ServerRoutingTable serverRoutingTable)
        {
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/"] = request => new HomeController().Index();
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Home/Index"] = request => new HomeController().Index();


            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Users/Login"] = request => new UsersController().Login();
            serverRoutingTable.Routes[HttpRequestMethod.Post]["/Users/Login"] = request => new UsersController().LoginPost();
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Users/Register"] = request => new UsersController().Register();
            serverRoutingTable.Routes[HttpRequestMethod.Post]["/Users/Register"] = request => new UsersController().RegisterPost();
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Users/Logout"] = request => new UsersController().Logout();

            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Albums/All"] = request => new AlbumController().All();
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Albums/Create"] = request => new AlbumController().CreateAlbum();
            serverRoutingTable.Routes[HttpRequestMethod.Post]["/Albums/Create"] = request => new AlbumController().CreateAlbumPost();
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Albums/Details"] = request => new AlbumController().AlbumDetails();
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Tracks/Create"] = request => new AlbumController().CreateTrack();
            serverRoutingTable.Routes[HttpRequestMethod.Post]["/Tracks/Create"] = request => new AlbumController().CreateTrackPost();
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Tracks/Details"] = request => new AlbumController().TrackDetails();
        }
    }
}
