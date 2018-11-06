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

        uc_PinCard _PinParent = null;
        protected override void OnParentSet()
        {
            base.OnParentSet();
            try
            {
                if (_PinParent != null) return;
                _PinParent = Tools.ParentPinCard<uc_PinCard>(this);
                if (_PinParent != null)
                {

                    grd_Main.Children.Remove(frm_ImgTitle);
                    frm_ImgTitle.HorizontalOptions = LayoutOptions.Start;
                    frm_ImgTitle.WidthRequest = frm_ImgTitle.HeightRequest = 40;
                    _PinParent.AddTitleContent(frm_ImgTitle);

                    _PinParent.ForceLayout();
                }
            }
            catch (Exception ex)
            {
                ((Views.MainPage)App.Current.MainPage).Manage_Error(ex);
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
                ViewModels.VM_UDPReceiver.Instance.AddLog(Encoding.ASCII.GetString(messageArray));
                await client.SendAsync(messageArray, messageArray.Count());

                var port = ((IPEndPoint)client.Client.LocalEndPoint).Port;
                ViewModels.VM_UDPReceiver.Instance.StartListener(port);
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

        private async void Send_Tapped(object sender, EventArgs e)
        {
            stk_Send.IsEnabled = false;
            await frm_Send.ScaleTo(0.5, 100);
            if (!String.IsNullOrEmpty(tb_Command.Text))
                await Send(tb_Command.Text.ToUpper());
            await frm_Send.ScaleTo(1, 100);
            stk_Send.IsEnabled = true;
        }

        private async void TEMP_Tapped(object sender, EventArgs e)
        {
            stkCommands.IsEnabled = false;
            await frm_TEMP.ScaleTo(0.5, 100);
            await Send("TEMP");
            await frm_TEMP.ScaleTo(1, 100);
            stkCommands.IsEnabled = true;
        }
        private async void DATE_Tapped(object sender, EventArgs e)
        {
            stkCommands.IsEnabled = false;
            await frm_DATE.ScaleTo(0.5, 100);
            await Send("DATE");
            await frm_DATE.ScaleTo(1, 100);
            stkCommands.IsEnabled = true;
        }
        private async void SETDATE_Tapped(object sender, EventArgs e)
        {
            stkCommands.IsEnabled = false;
            await frm_SETDATE.ScaleTo(0.5, 100);
            await Send("SETDATE");
            await frm_SETDATE.ScaleTo(1, 100);
            stkCommands.IsEnabled = true;
        }
        private async void LIGHT_Tapped(object sender, EventArgs e)
        {
            stkCommands.IsEnabled = false;
            await frm_LIGHT.ScaleTo(0.5, 100);
            await Send("LIGHT");
            await frm_LIGHT.ScaleTo(1, 100);
            stkCommands.IsEnabled = true;
        }

        private async void Title_Tapped(object sender, EventArgs e)
        {
            try
            {
                frm_ImgTitle.IsEnabled = false;
                await frm_ImgTitle.ScaleTo(0.5, 250).ContinueWith(a => frm_ImgTitle.ScaleTo(1, 100));
                frm_ImgTitle.IsEnabled = true;
            }
            catch (Exception ex)
            {
                ((Views.MainPage)App.Current.MainPage).Manage_Error(ex);
            }
        }

        private async void grd_Main_Tapped(object sender, EventArgs e)
        {
            try
            {
                lbl_DATE.IsVisible = lbl_LIGHT.IsVisible = lbl_SETDATE.IsVisible = lbl_TEMP.IsVisible = !lbl_TEMP.IsVisible;
                await stkCommands.ScaleTo(0.9, 250).ContinueWith(a => stkCommands.ScaleTo(1, 100));
            }
            catch (Exception ex)
            {
                ((Views.MainPage)App.Current.MainPage).Manage_Error(ex);
            }
        }
    }
}