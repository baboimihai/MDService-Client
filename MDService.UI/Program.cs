using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaskTrayApplication;
using FluentScheduler;

namespace MDService.UI
{
    static class Program
    {
        static string[] arr1 = new string[] { "one", "two", "three" };
        public static HostService serviceTask = new HostService(arr1);

        //public static HostService serviceTask = new HostService(arr1);
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Process.GetCurrentProcess().Kill();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var uiContext = new UIContext();
            uiContext.Subscribe(serviceTask);
            var registry = new Registry();
            registry.Schedule<KeepAliveSession>().ToRunNow().AndEvery(1).Minutes();
            JobManager.Initialize(registry);
            Application.Run(uiContext);

            //System.Diagnostics.Process.Start(Application.ExecutablePath);
            //Server.serviceTask.RestartAPP = false;
            //System.Diagnostics.Process.Start(Application.ExecutablePath);
            //Thread.Sleep(2000);
            //Application.Exit();
            //return;


        }

    }
}
