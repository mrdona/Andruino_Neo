using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Andruino.ViewModels
{
    /// <summary>
    /// View Model pour la configuration
    /// </summary>
    public class VM_Config : BaseViewModel
    {

        bool _lock = false;
        bool _AutoPing = false;
        string localIP = string.Empty;
        string serverIP = string.Empty;
        int serverPort = -1;

        /// <summary>
        /// IP Locale
        /// </summary>
        public string LocalIP { get => localIP; set => SetProperty(ref localIP, value); }

        /// <summary>
        /// IP du Serveur
        /// </summary>
        public string ServerIP { get => serverIP; set => SetProperty(ref serverIP, value); }

        /// <summary>
        /// Port UDP d'écoute du serveur
        /// </summary>
        public int ServerPort { get => serverPort; set => SetProperty(ref serverPort, value); }

        /// <summary>
        /// Constructeur, démarre la boucle Ping
        /// </summary>
        public VM_Config()
        {
            AutoPingRun();
        }

        /// <summary>
        /// Boucle continue de Ping
        /// </summary>
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

        /// <summary>
        /// Ping le serveur
        /// </summary>
        /// <returns></returns>
        public async Task<bool> PingServer()
        {
            ServerState = false;
            System.Net.IPAddress _ServerIP;
            if (!String.IsNullOrEmpty(ServerIP) && System.Net.IPAddress.TryParse(ServerIP, out _ServerIP))
                ServerState = (await new Ping().SendPingAsync(ServerIP)).Status.ToString().Equals("Success");
            OnPropertyChanged("ServerState");
            OnPropertyChanged("ServerStateImageSource");
            return ServerState;
        }

        /// <summary>
        /// Serveur accessible
        /// </summary>
        public bool ServerState { get; set; }

        /// <summary>
        /// Active la boucle de Ping
        /// </summary>
        public bool AutoPing
        {
            get { return _AutoPing; }
            set
            {
                _AutoPing = value;
                OnPropertyChanged("NotAutoPing");
            }
        }

        /// <summary>
        /// Inversion de AutoPing
        /// </summary>
        public bool NotAutoPing { get { return !AutoPing; } }

        /// <summary>
        /// Image pour l'état d'accessiblitité du serveur
        /// </summary>
        public ImageSource ServerStateImageSource
        {
            get
            {
                return ServerState ? ImageSource.FromFile("connexionOK.png") : ImageSource.FromFile("connexionKO.png");
            }
        }

        /// <summary>
        /// Envoi de la Commande SETDATE au serveur,
        /// Le retour n'est pas lu -> bug ?
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SetDate()
        {
            try
            {
                VM_UDPReceiver.Instance.GetResponse += (s, e) =>
                {
                    var response = ViewModels.VM_UDPReceiver.Instance.Log.Last();
                    if (response != null && response.Contains("SETDATE_"))
                    {
                        _lock = false;
                    }
                };
                VM_UDPSender.Instance.SendAsync(((App)App.Current).CurrentConfig, "[CMD]SETDATE[/CMD]");

                _lock = true;
                int retry = 0;
                while (_lock && retry < 50)
                {
                    await Task.Delay(100);
                    retry++;
                }

                return true;
            }
            catch (Exception ex)
            {
                ((Views.MainPage)App.Current.MainPage).Manage_Error(ex);
                return false;
            }
        }

        /// <summary>
        /// Mode Démonstration
        /// </summary>
        public bool ModeDemo { get; set; }
    }
}
