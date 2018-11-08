using FFImageLoading.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Andruino.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class V_Config : ContentView
	{
        string _currentCommandWaiting = string.Empty;

        public V_Config ()
		{
			InitializeComponent ();

            try
            {
                this.BindingContext = ((App)App.Current).CurrentConfig;
                ((App)App.Current).CurrentConfig.PropertyChanged +=(s,e)=> 
                {
                    if (e.PropertyName == "ServerStateImageSource")
                    {
                        var x = ((App)App.Current).CurrentConfig.ServerStateImageSource;
                        img_ServerSate.Source = x;
                        img_ServerSate.ReloadImage();
                    }
                };

                ViewModels.VM_UDPReceiver.Instance.GetResponse += Instance_GetResponse;
            }
            catch (Exception ex)
            {
                ((Views.MainPage)App.Current.MainPage).Manage_Error(ex);
            }
        }


        uc_PinCard _PinParent = null;
        protected override void OnParentSet()
        {
            base.OnParentSet();
            try
            {
                if (_PinParent != null) return;
                _PinParent = Tools.GetParent<uc_PinCard>(this);
                if (_PinParent != null)
                {
                    grd_Main.Children.Remove(frm_ImgTitle);
                    frm_ImgTitle.HorizontalOptions = LayoutOptions.StartAndExpand;
                    _PinParent.AddTitleContent(frm_ImgTitle);

                    //_PinParent.AddTitleContent(frm_Ping);

                    _PinParent.ForceLayout();
                }
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
                //Crash
                //var a = Tools.GetAllChildren(this.grd_Main).ToList();
                //var b = a.OfType<Label>();
                //foreach (var c in b.Where(l => nameof(l).StartsWith("lbl_Info")))
                //    c.IsVisible = !c.IsVisible;

                stk_ConfigProps.IsVisible = lbl_Info_Link.IsVisible = lbl_Info_Ping.IsVisible = lbl_Info_ModeDemo.IsVisible = lbl_Info_Reset.IsVisible = lbl_Info_Network.IsVisible = lbl_Info_MessageResult.IsVisible
                    = !lbl_Info_ModeDemo.IsVisible;
                _PinParent.ForceLayout();
                await grd_Main.ScaleTo(0.9, 150).ContinueWith(x => grd_Main.ScaleTo(1, 75));

                Title_Tapped(null, null);
            }
            catch (Exception ex)
            {
                ((Views.MainPage)App.Current.MainPage).Manage_Error(ex);
            }
        }


        private async void Ping_Tapped(object sender, EventArgs e)
        {
            try
            {
                frm_Ping.IsEnabled = false;
                await frm_Ping.ScaleTo(0.5, 250).ContinueWith(a => frm_Ping.ScaleTo(1, 100));
                await ((App)App.Current).CurrentConfig.PingServer();
                frm_Ping.IsEnabled = true;
            }
            catch (Exception ex)
            {
                ((Views.MainPage)App.Current.MainPage).Manage_Error(ex);
            }
        }

        private async void Title_Tapped(object sender, EventArgs e)
        {
            try
            {
                frm_ImgTitle.IsEnabled = false;
                await frm_ImgTitle.ScaleTo(0.5, 250).ContinueWith(async a => await frm_ImgTitle.ScaleTo(1, 100));

                lbl_Info_Title.IsVisible = !lbl_Info_Title.IsVisible;
                lbl_Info_Title.Opacity = 0;
                await lbl_Info_Title.FadeTo(1, 750);

                frm_ImgTitle.IsEnabled = true;
            }
            catch (Exception ex)
            {
                ((Views.MainPage)App.Current.MainPage).Manage_Error(ex);
            }
        }

        private async void Link_Tapped(object sender, EventArgs e)
        {
            try
            {
                frm_Link.IsEnabled = false;
                frm_Link.Opacity = 0.5;
                await frm_Link.ScaleTo(0.5, 250).ContinueWith(a => frm_Link.ScaleTo(1, 100));

                _currentCommandWaiting = "SETDATE_";
                await ViewModels.VM_UDPSender.Instance.SendAsync(((App)App.Current).CurrentConfig, "[CMD]SETDATE[/CMD]");

                frm_Link.IsEnabled = true;
                frm_Link.Opacity = 1;
            }
            catch (Exception ex)
            {
                ((Views.MainPage)App.Current.MainPage).Manage_Error(ex);
            }
        }

        private async void Reset_Tapped(object sender, EventArgs e)
        {
            try
            {
                frm_Reset.IsEnabled = false;
                frm_Reset.Opacity = 0.5;
                await frm_Reset.ScaleTo(0.5, 250).ContinueWith(a => frm_Reset.ScaleTo(1, 100));

                _currentCommandWaiting = "RESET_";
                await ViewModels.VM_UDPSender.Instance.SendAsync(((App)App.Current).CurrentConfig, "[CMD]RESET[/CMD]");

                frm_Reset.IsEnabled = true;
                frm_Reset.Opacity = 1;
            }
            catch (Exception ex)
            {
                ((Views.MainPage)App.Current.MainPage).Manage_Error(ex);
            }
        }

        private async void Network_Tapped(object sender, EventArgs e)
        {
            try
            {
                frm_Network.IsEnabled = false;
                frm_Network.Opacity = 0.5;
                await frm_Network.ScaleTo(0.5, 250).ContinueWith(a => frm_Network.ScaleTo(1, 100));

                _currentCommandWaiting = "WIFI_";
                await ViewModels.VM_UDPSender.Instance.SendAsync(((App)App.Current).CurrentConfig, "[CMD]WIFI[/CMD]");
                frm_Network.IsEnabled = true;
                frm_Network.Opacity = 1;
            }
            catch (Exception ex)
            {
                ((Views.MainPage)App.Current.MainPage).Manage_Error(ex);
            }
        }

        private void Instance_GetResponse(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(_currentCommandWaiting)) return;
            try
            {
                var response = ViewModels.VM_UDPReceiver.Instance.Log.Last();
                if (response != null && response.StartsWith(_currentCommandWaiting))
                {
                    ViewModels.VM_UDPReceiver.Instance.Log.Remove(response);
                    response = response.Replace(_currentCommandWaiting, "");
                    ViewModels.VM_UDPReceiver.Instance.Log.Add(response);

                    _currentCommandWaiting = string.Empty;

                    ShowMessage(response);
                }
            }
            catch (Exception ex)
            {
                ((Views.MainPage)App.Current.MainPage).Manage_Error(ex);
            }
        }

        private async void ShowMessage(string Message)
        {
            try
            {
                lbl_MessageResult.Text = Message;
                stk_MessageResult.Scale = 0.3;
                stk_MessageResult.IsVisible = true;
                await stk_MessageResult.ScaleTo(1, 150);
                await stk_MessageResult.FadeTo(0, 5 * 1000);
                stk_MessageResult.IsVisible = false;
                stk_MessageResult.Opacity = 1;
            }
            catch (Exception ex)
            {
                ((Views.MainPage)App.Current.MainPage).Manage_Error(ex);
            }
        }
    }
}