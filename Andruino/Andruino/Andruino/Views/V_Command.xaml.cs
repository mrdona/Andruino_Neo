using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Andruino.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class V_Command : ContentView
    {

        public V_Command()
        {
            InitializeComponent();

            try
            {
                MessagingCenter.Subscribe<MainPage, string>(this, "Refresh", Refresh);
            }
            catch (Exception ex)
            {
                ((MainPage)App.Current.MainPage).Manage_Error(ex);
            }
        }

        private async void Refresh(MainPage arg1, string arg2)
        {
            await Task.Delay(1);
        }

        private async Task Send(string message)
        {
            var client = new UdpClient();
            try
            {
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse(((App)App.Current).CurrentConfig.ServerIP), ((App)App.Current).CurrentConfig.ServerPort); // endpoint where server is listening
                client.Connect(ep);

                var messageArray = Encoding.ASCII.GetBytes("[CMD]" + message + "[/CMD]");
                outToLog(Encoding.ASCII.GetString(messageArray));
                client.Send(messageArray, messageArray.Count());

                var port = ((IPEndPoint)client.Client.LocalEndPoint).Port;
                await StartListenerAsync(port);
            }
            catch (Exception ex)
            {
                ((MainPage)App.Current.MainPage).Manage_Error(ex);
            }
            finally
            {
                client.Close();
            }
        }

        private async Task StartListenerAsync(int listenPort)
        {
            UdpClient listener = new UdpClient(listenPort);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);

            try
            {
                byte[] bytes = (await listener.ReceiveAsync()).Buffer;
                outToLog(String.Format("[{0}] :\n {1}\n",
                    groupEP.ToString(),
                    Encoding.ASCII.GetString(bytes)));

                ((MainPage)App.Current.MainPage).RandomBackGround();
            }
            catch (Exception ex)
            {
                ((MainPage)App.Current.MainPage).Manage_Error(ex);
            }
            finally
            {
                listener.Close();
            }
        }

        private void outToLog(string output)
        {
            //stk_Log.Children.Add(new Label() { Text = (DateTime.Now.ToString("HH:mm:ss") + ": " + output) });
        }


        private async void tnTest_Clicked(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(tb_Command.Text)) return;
            await Send(tb_Command.Text.ToUpper());
        }

        private async void btn_TEMP_Clicked(object sender, EventArgs e)
        {
            await Send("TEMP");
        }

        private async void btn_SETDATE_Clicked(object sender, EventArgs e)
        {
            await Send("SETDATE");
        }

        private async void btn_DATE_Clicked(object sender, EventArgs e)
        {
            await Send("DATE");
        }

        private async void btn_LIGHT_Clicked(object sender, EventArgs e)
        {
            await Send("LIGHT");
        }

    }
}