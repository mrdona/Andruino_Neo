using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Andruino.ViewModels
{
    public class VM_UDPReceiver : BaseViewModel
    {
        public event EventHandler GetResponse;

        public static VM_UDPReceiver Instance
        {
            get => instance;
            set => instance = value;
        }
        static VM_UDPReceiver instance = new VM_UDPReceiver();
        private VM_UDPReceiver() { }

        public async void StartListener(int listenPort)
        {
            UdpClient listener = new UdpClient(listenPort);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);

            try
            {
                byte[] bytes = (await listener.ReceiveAsync()).Buffer;

                AddLog(String.Format("[{0}] :\n {1}\n", groupEP.ToString(), Encoding.ASCII.GetString(bytes)));

                ((Views.MainPage)App.Current.MainPage).RandomBackGround();
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

        public void AddLog(string message)
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
        }


        ObservableCollection<string> log = new ObservableCollection<string>();
        public ObservableCollection<string> Log
        {
            get => log;
            set => log = value;
        }

    }
}
