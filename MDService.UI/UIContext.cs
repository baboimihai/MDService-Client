using MDService;
using MDService.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace TaskTrayApplication
{
    public class UIContext : ApplicationContext
    {
        NotifyIcon notifyIcon = new NotifyIcon();
        Form1 configWindow = new Form1();

        public UIContext()
        {
            MenuItem configMenuItem = new MenuItem("Settings", new EventHandler(ShowConfig));
            MenuItem exitMenuItem = new MenuItem("Exit", new EventHandler(Exit));

            notifyIcon.Icon = new System.Drawing.Icon("AppIcon.ico") ;
            notifyIcon.DoubleClick += new EventHandler(ShowMessage);
            notifyIcon.ContextMenu = new ContextMenu(new MenuItem[] { configMenuItem, exitMenuItem });
            notifyIcon.Visible = true;
            notifyIcon.Text = "Your magic microservice";
            
    
        }
        public void Subscribe(HostService publisher)
        {
            HostService.Event += HeardEvent;
        }

        static void HeardEvent(Object sender, RaiseArgs e)
        {
            RestartApp();
        }
        public static void RestartApp()
        {
  
            Program.serviceTask.RestartAPP = false;
            ProcessStartInfo Info = new ProcessStartInfo();
            Info.Arguments = "/C choice /C Y /N /D Y /T 1 & START \"\" \"" + Assembly.GetExecutingAssembly().Location + "\"";
            Info.WindowStyle = ProcessWindowStyle.Hidden;
            Info.CreateNoWindow = true;
            Info.FileName = "cmd.exe";
            Process.Start(Info);
            Application.Exit();
            Program.serviceTask.OnStop();
            Program.serviceTask = null;
            Process.GetCurrentProcess().Kill();
        }

        void ShowMessage(object sender, EventArgs e)
        {
            if (configWindow.Visible)
                configWindow.Focus();
            else
                configWindow.ShowDialog();
        }

        void ShowConfig(object sender, EventArgs e)
        {
            if (configWindow.Visible)
                configWindow.Focus();
            else
                configWindow.ShowDialog();
        }

        void Exit(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            Program.serviceTask.OnStop();
            Program.serviceTask = null;
            Application.Exit();
            Process.GetCurrentProcess().Kill();

        }
    }
}
