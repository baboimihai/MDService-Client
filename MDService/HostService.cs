using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace MDService
{
    public class Server {
        static string[] arr1 = new string[] { "one", "two", "three" };
        public static HostService serviceTask = new HostService(arr1);
    }


    public class HostService:ServiceBase
    {
        private HttpSelfHostServer _server;
        private readonly string[] args;
        private bool IsRunning;
        public bool RestartAPP { get; set; }
        public HostService(string[] args)
        {
            this.OnStart(args);
            this.args = args;
         //   this.OnStop();
        }
        protected override void OnStart(string[] args)
        {
            var config = new HttpSelfHostConfiguration("http://localhost:8180/");
            config.Routes.MapHttpRoute(name: "API", 
                routeTemplate: "{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional });
             _server = new HttpSelfHostServer(config);
            IsRunning = true;
            config.MaxConcurrentRequests = 1000;
            config.MaxBufferSize = 1147483648;
            config.MaxReceivedMessageSize = 1147483648;
            _server.OpenAsync().Wait();

        }
        protected override void OnStop()
        {
            _server.CloseAsync().Wait();
            _server.Dispose();
            IsRunning = false;

        }
        public void Restart()
        {
            RestartAPP = true;
        }

    }
}
