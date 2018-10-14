namespace SIS.WebServer
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    using API;

    public class WebServer
    {
        private const string LocalhostIpAddress = "127.0.0.1";

        private const string StartServerMessage = "Server started at http://{0}:{1}";

        private readonly int port;
        private readonly TcpListener listener;
        private readonly IHttpHandler handler;

        private bool isRunning;
        
        public WebServer(int port, IHttpHandler handler)
        {
            this.port = port;
            this.listener = new TcpListener(IPAddress.Parse(LocalhostIpAddress), this.port);
            this.handler = handler;
        }

        public void Run()
        {
            this.listener.Start();
            this.isRunning = true;

            Console.WriteLine(StartServerMessage, LocalhostIpAddress, this.port);

            var task = Task.Run(this.ListenLoopAsync);
           
            task.Wait();
        }

        public async Task ListenLoopAsync()
        {
            while (this.isRunning)
            {
                var client = await this.listener.AcceptSocketAsync();
                var connectionHandler = new ConnectionHandler(client, handler);

                connectionHandler.ProcessRequestAsync();
            }
        }
    }
}