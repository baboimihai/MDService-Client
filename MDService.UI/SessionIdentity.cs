using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDService.UI
{
    public static class SessionIdentity
    {
        static SessionIdentity()
        {
            var rnd = new Random(DateTime.Now.Millisecond);
            int ticks = rnd.Next(8000, 9000);
            while (ticks == 8080)
            {
                ticks = rnd.Next(8000, 9000);
            }
            Port = ticks.ToString();//
        }
        public static string ClassName { get; set; }
        public static string Namespace { get; set; }
        public static byte[] Key { get; set; }
        public static string Token { get; set; }
        public static string Port { get; set; }
        public static IKEServer ConnectionSecurity = new IKEServer();
    }
}
