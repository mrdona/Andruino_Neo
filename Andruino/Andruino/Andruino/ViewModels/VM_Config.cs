using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Andruino.ViewModels
{
    public class VM_Config : BaseViewModel
    {
        string localIP = string.Empty;
        string serverIP = string.Empty;
        int serverPort = -1;

        public string LocalIP { get => localIP; set => SetProperty(ref localIP, value); }
        public string ServerIP { get => serverIP; set => SetProperty(ref serverIP, value); }
        public int ServerPort { get => serverPort; set => SetProperty(ref serverPort, value); }


        public VM_Config()
        {
            AutoPingRun();
        }


        private async void AutoPingRun()
        {
            while(true)
            {
                try
                {
                    if (AutoPing)
                        await PingServer();
                    await System.Threading.Tasks.Task.Delay(10 * 1000);
                }
                catch (Exception ex)
                {
                    ((Views.MainPage)App.Current.MainPage).Manage_Error(ex);
                }
            }
        }


        System.Net.IPAddress _ServerIP;
        public async Task<bool> PingServer()
        {
            ServerState = false;
            if (!String.IsNullOrEmpty(ServerIP) && System.Net.IPAddress.TryParse(ServerIP, out _ServerIP))
                ServerState = (await new Ping().SendPingAsync(ServerIP)).Status.ToString().Equals("Success");
            OnPropertyChanged("ServerState");
            OnPropertyChanged("ServerStateImageSource");
            return ServerState;
        }

        public bool ServerState { get; set; }

        public bool NotAutoPing { get { return !AutoPing; } }
        bool _AutoPing = false;
        public bool AutoPing
        {
            get { return _AutoPing; }
            set
            {
                _AutoPing = value;
                OnPropertyChanged("NotAutoPing");
            }
        }

        public ImageSource ServerStateImageSource
        {
            get
            {
                return ServerState ? ImageSource.FromFile("connexionOK.png") : ImageSource.FromFile("connexionKO.png");
            }
        }
    }
}
