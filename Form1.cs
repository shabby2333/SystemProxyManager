using IWshRuntimeLibrary;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SystemProxyManager
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            InitializeComponent();
        }

        [DllImport("wininet.dll")]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        private const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        private const int INTERNET_OPTION_REFRESH = 37;
        private bool settingsReturn, refreshReturn;
        RegistryKey registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
        private AutoStart autoStart = new AutoStart()
        {
            QuickName = "ProxyManager",
            QuickDescribe = "ProxyManager",
            WindowStyle = WshWindowStyle.WshMinimizedFocus
        };

        public void CheckProxyOpened()
        {
            btnStatus.Checked = Convert.ToInt32(registry.GetValue("ProxyEnable")) == 1;
            notifyIcon1.Icon = btnStatus.Checked? Properties.Resources.on: Properties.Resources.off;
            tbProxyAddress.Text = Convert.ToString(registry.GetValue("ProxyServer"));
            btnAutoStart.Checked = autoStart.GetAutoStart();
        }

        private void btnStatus_Click(object sender, EventArgs e)
        {
            btnStatus.Checked = !btnStatus.Checked;
            ChangeProxyOpen(btnStatus.Checked);
            notifyIcon1.Icon = btnStatus.Checked ? Properties.Resources.on : Properties.Resources.off;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CheckProxyOpened();
        }

        private void btnAutoStart_Click(object sender, EventArgs e)
        {
            btnAutoStart.Checked = !btnAutoStart.Checked;
            autoStart.SetAutoStart(btnAutoStart.Checked);
        }

        public void ChangeProxyOpen(bool open)
        {
            registry.SetValue("ProxyEnable", open? 1: 0);
            if(open && !String.IsNullOrEmpty(tbProxyAddress.Text))
                registry.SetValue("ProxyServer", tbProxyAddress.Text);

            // These lines implement the Interface in the beginning of program 
            // They cause the OS to refresh the settings, causing IP to realy update
            settingsReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            refreshReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);

        }
    }
}
