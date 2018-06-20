using FluentScheduler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace MDService.UI
{
    public class KeepAliveSession : IJob
    {
        private string websiteLink = /*"http://mihaidm.ddns.net";//*/"http://www.dm.com/";
        public void Execute()
        {
            if (string.IsNullOrEmpty(SessionIdentity.Token))
            {
                InstallNewService();
            }
            else
            {

                var resutl = CallServer.POSTJson(websiteLink + "/Api/DynamicMicros/KeepAlive", SessionIdentity.Token);
                if (resutl.Replace("\\", "").Replace("\"", "") != "Ok")
                {
                    MDService.UI.Program.serviceTask.Restart();
                }
            }
        }
        public void InstallNewService()
        {
            //string pubIp = new System.Net.WebClient().DownloadString("https://api.ipify.org");
            string pubIp = "localhost";
            var url = websiteLink + "/Api/DynamicMicros/StartSession";
            string requestInfo = SessionIdentity.ConnectionSecurity.GenerateRequest();
            var clientToken = Guid.NewGuid().ToString().Substring(0, 10);
            var jsonContent = requestInfo + " " + SessionIdentity.Port + " " + clientToken + " " + pubIp;
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
                            var result = reader.ReadToEnd().Replace("\\", "").Replace("\"", "").Replace("\\", "").Replace("\"", "");
                            SessionIdentity.Token = clientToken;
                            SessionIdentity.ConnectionSecurity.HandleResponse(result);
                            SessionIdentity.Key = SessionIdentity.ConnectionSecurity.GetKey();
                            request = (HttpWebRequest)WebRequest.Create(websiteLink + "/Api/DynamicMicros/Ready");
                            request.Method = "POST";
                            request.ContentLength = 0;
                            byteArray = encoding.GetBytes(clientToken);

                            request.ContentLength = byteArray.Length;
                            request.ContentType = @"application/json";

                            using (Stream dataStream = request.GetRequestStream())
                            {
                                dataStream.Write(byteArray, 0, byteArray.Length);
                            }
                            var responseReady = (HttpWebResponse)request.GetResponse();
                        }
                    }
                }
                catch (WebException ex)
                {

                }
            }
            catch (WebException ex)
            {

            }
        }
    }
}
