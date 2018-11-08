using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Andruino.ViewModels
{
    /// <summary>
    /// ViewModel de reception des messages UDP
    /// </summary>
    public class VM_UDPReceiver : BaseViewModel
    {
        /// <summary>
        /// Log
        /// </summary>
        public ObservableCollection<string> Log { get; set; } = new ObservableCollection<string>();

        /// <summary>
        /// Event de Réponse
        /// </summary>
        public event EventHandler GetResponse;

        /// <summary>
        /// Constructeur privé
        /// </summary>
        private VM_UDPReceiver() { }

        /// <summary>
        /// Singleton
        /// </summary>
        public static VM_UDPReceiver Instance { get; set; } = new VM_UDPReceiver();

        /// <summary>
        /// Démarre l'écoute sur toutes les IP pour ce port
        /// </summary>
        /// <param name="listenPort"></param>
        public async void StartListener(int listenPort)
        {
            UdpClient listener = new UdpClient(listenPort);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);

            try
            {
                byte[] bytes = (await listener.ReceiveAsync()).Buffer;

                //AddLog(String.Format("[{0}] :\n {1}\n", groupEP.ToString(), Encoding.ASCII.GetString(bytes)));
                AddLog(String.Format(Encoding.ASCII.GetString(bytes)));
                //((Views.MainPage)App.Current.MainPage).RandomBackGround();
            }
            catch (Exception ex)
            {
                ((Views.MainPage)App.Current.MainPage).Manage_Error(ex);
            }
            finally
            {
                listener.Close();
            }
        }

        /// <summary>
        /// Ajoute un message dans le Log
        /// </summary>
        /// <param name="message"></param>
        public void AddLog(string message)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    Log.Add(message);
                    GetResponse?.Invoke(this, null);
                }
                catch (Exception ex)
                {
                    ((Views.MainPage)App.Current.MainPage).Manage_Error(ex);
                }
            });
        }


    }
}
