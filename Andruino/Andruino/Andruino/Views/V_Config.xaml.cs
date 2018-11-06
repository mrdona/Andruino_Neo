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
                _PinParent = Tools.ParentPinCard<uc_PinCard>(this);
                if (_PinParent != null)
                {
                    grd_Main.Children.Remove(frm_ImgTitle);
                    frm_ImgTitle.HorizontalOptions = LayoutOptions.Start;
                    frm_ImgTitle.WidthRequest = frm_ImgTitle.HeightRequest = 40;
                    _PinParent.AddTitleContent(frm_ImgTitle);

                    _PinParent.AddTitleContent(frm_Ping);
                    _PinParent.ForceLayout();
                }
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
                await frm_ImgTitle.ScaleTo(0.5, 250).ContinueWith(a => frm_ImgTitle.ScaleTo(1, 100));
                frm_ImgTitle.IsEnabled = true;
            }
            catch (Exception ex)
            {
                ((Views.MainPage)App.Current.MainPage).Manage_Error(ex);
            }
        }
    }
}