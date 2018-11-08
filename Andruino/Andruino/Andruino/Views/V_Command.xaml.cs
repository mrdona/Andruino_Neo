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
        string _currentCommandWaiting = string.Empty;


        public V_Command()
        {
            InitializeComponent();

            try
            {
                ViewModels.VM_UDPReceiver.Instance.GetResponse += Instance_GetResponse;
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
                _PinParent = Tools.GetParent<uc_PinCard>(this);
                if (_PinParent != null)
                {

                    grd_Main.Children.Remove(frm_ImgTitle);
                    frm_ImgTitle.HorizontalOptions = LayoutOptions.Start;
                    _PinParent.AddTitleContent(frm_ImgTitle);

                    _PinParent.ForceLayout();
                }
            }
            catch (Exception ex)
            {
                ((Views.MainPage)App.Current.MainPage).Manage_Error(ex);
            }
        }

        private async void Send_Tapped(object sender, EventArgs e)
        {
            stk_Send.IsEnabled = false;
            await frm_Send.ScaleTo(0.5, 100);
            if (!String.IsNullOrEmpty(tb_Command.Text))
                Send(tb_Command.Text);

            await frm_Send.ScaleTo(1, 100);
            stk_Send.IsEnabled = true;
        }
        private async void TEMP_Tapped(object sender, EventArgs e)
        {
            stkCommands.IsEnabled = false;
            await frm_TEMP.ScaleTo(0.5, 100);
            Send("TEMP");
            await frm_TEMP.ScaleTo(1, 100);
            stkCommands.IsEnabled = true;
        }
        private async void DATE_Tapped(object sender, EventArgs e)
        {
            stkCommands.IsEnabled = false;
            await frm_DATE.ScaleTo(0.5, 100);
            Send("DATE");
            await frm_DATE.ScaleTo(1, 100);
            stkCommands.IsEnabled = true;
        }
        private async void HR_Tapped(object sender, EventArgs e)
        {
            stkCommands.IsEnabled = false;
            await frm_HR.ScaleTo(0.5, 100);
            Send("HR");
            await frm_HR.ScaleTo(1, 100);
            stkCommands.IsEnabled = true;
        }
        private async void LIGHT_Tapped(object sender, EventArgs e)
        {
            stkCommands.IsEnabled = false;
            await frm_LIGHT.ScaleTo(0.5, 100);
            Send("LIGHT");
            await frm_LIGHT.ScaleTo(1, 100);
            stkCommands.IsEnabled = true;
        }
        private async void TEMPC_Tapped(object sender, EventArgs e)
        {
            stkCommands.IsEnabled = false;
            await frm_TEMPC.ScaleTo(0.5, 100);
            Send("TEMPC");
            await frm_TEMPC.ScaleTo(1, 100);
            stkCommands.IsEnabled = true;
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

        private async void grd_Main_Tapped(object sender, EventArgs e)
        {
            try
            {
                stk_Send.IsVisible = !stk_Send.IsVisible;
                lbl_DATE.IsVisible = lbl_LIGHT.IsVisible = lbl_HR.IsVisible = lbl_TEMPC.IsVisible = lbl_Send.IsVisible = lbl_TEMP.IsVisible = lbl_Info_MessageResult.IsEnabled = !lbl_TEMP.IsVisible;
                await stkCommands.ScaleTo(0.9, 250).ContinueWith(a => stkCommands.ScaleTo(1, 100));
                Title_Tapped(null, null);
            }
            catch (Exception ex)
            {
                ((Views.MainPage)App.Current.MainPage).Manage_Error(ex);
            }
        }


        private async void Send(string command)
        {
            try
            {
                _currentCommandWaiting = command.ToUpper() + "_";
                await ViewModels.VM_UDPSender.Instance.SendAsync(((App)App.Current).CurrentConfig, "[CMD]" + command.ToUpper() + "[/CMD]");
            }
            catch (Exception ex)
            {
                ((MainPage)App.Current.MainPage).Manage_Error(ex);
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

        bool _MessageFading = false;
        bool _CancelFading = false;
        private void ShowMessage(string Message)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    if (_MessageFading)
                    {
                        _CancelFading = true;
                        ViewExtensions.CancelAnimations(stk_MessageResult);
                        stk_MessageResult.ForceLayout();
                        //await Task.Delay(100);
                        //await stk_MessageResult.FadeTo(1, 10);
                        _MessageFading = false;
                        stk_MessageResult.IsVisible = true;
                        stk_MessageResult.ForceLayout();
                        _CancelFading = false;
                    }

                    lbl_MessageResult.Text = Message;

                    stk_MessageResult.IsVisible = true;
                    _MessageFading = true;
                    stk_MessageResult.Opacity = 1;
                    await stk_MessageResult.FadeTo(0.1, 5 * 1000).ContinueWith(e =>
                    {
                        if ((!e.IsCanceled || !e.IsFaulted) && _MessageFading && !_CancelFading)
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                _MessageFading = false;
                                stk_MessageResult.IsVisible = false;
                            });
                        }
                    });         
                }
                catch (Exception ex)
                {
                    ((Views.MainPage)App.Current.MainPage).Manage_Error(ex);
                }
            });
        }
    }
}