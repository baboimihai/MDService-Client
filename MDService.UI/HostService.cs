using MDService.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace MDService
{
    public class HostService
    {
        private HttpSelfHostServer _server;
        private readonly string[] args;
        public bool IsRunning;
        public bool RestartAPP { get; set; }
        public HostService(string[] args)
        {
            this.OnStart(args);
            this.args = args;
        }
        public void OnStart(string[] args)
        {
            //string pubIp = new System.Net.WebClient().DownloadString("https://api.ipify.org");
            var config = new HttpSelfHostConfiguration("http://"+ /*pubIp*/"localhost" + ":" + SessionIdentity.Port +"/");
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
        public void OnStop()
        {
            _server.CloseAsync().Wait();
            _server.Dispose();
            IsRunning = false;

        }
        public delegate void EventHandler(Object sender, RaiseArgs e);
        public static event EventHandler Event;
        public void RaiseEventRestart()
        {
            RaiseArgs args = new RaiseArgs();
            args.Message = "restart";
            Event(this, args);
        }
        public void Restart()
        {
            this.RestartAPP = true;
            RaiseEventRestart();
        }
    
    }
    public static class CallServer
    {
        public static string POSTJson(string url, string jsonContent)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";

                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                Byte[] byteArray = encoding.GetBytes(jsonContent);

                request.ContentLength = byteArray.Length;
                request.ContentType = @"application/json";

                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }
                try
                {
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        using (var reader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.ASCII))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
                catch (WebException ex)
                {
                    return string.Empty;
                }
            }
            catch (WebException ex)
            {
                return string.Empty;
            }

        }


    }
    public class RaiseArgs : EventArgs
    {
        public string Message { get; set; }
    }
}
