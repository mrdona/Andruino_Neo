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
	public partial class V_UDPLog : ContentView
	{
		public V_UDPLog ()
		{
			InitializeComponent ();

            ViewModels.VM_UDPReceiver.Instance.GetResponse += (s, e) =>
            {
                ShowLog();
            };
            this.BindingContext = ViewModels.VM_UDPReceiver.Instance;
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

                    //grd_Main.Children.Remove(frm_Reverse);
                    //frm_Reverse.HorizontalOptions = LayoutOptions.Start;
                    //frm_Reverse.WidthRequest = frm_Reverse.HeightRequest = 40;
                    //_PinParent.AddTitleContent(frm_Reverse);

                    _PinParent.ForceLayout();
                }
            }
            catch (Exception ex)
            {
                ((Views.MainPage)App.Current.MainPage).Manage_Error(ex);
            }
        }

        private void ShowLog()
        {
            var current = _revert ?
                                    ViewModels.VM_UDPReceiver.Instance.Log.Reverse().Take(10)
                                    : ViewModels.VM_UDPReceiver.Instance.Log.Take(10);
            stk_Log.Children.Clear();
            foreach (var log in current)
                stk_Log.Children.Add(new Label()
                {
                    Text = log,
                    FontSize = Device.GetNamedSize((NamedSize)currentLogFontSize, typeof(Label)),
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    HorizontalTextAlignment = TextAlignment.Start
                });

        }

        private void Clear_Tapped(object sender, EventArgs e)
        {
            ViewModels.VM_UDPReceiver.Instance.Log.Clear();
            stk_Log.Children.Clear();
            //((MainPage)App.Current.MainPage).RandomBackGround();
        }

        bool _revert = false;
        private void Reverse_Tapped(object sender, EventArgs e)
        {
            _revert = !_revert;
            if(_revert)
                img_Revert.RotateTo(270, 250);
            else
                img_Revert.RotateTo(90, 250);
            ShowLog();
        }

        private void stk_Log_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            //stk_Log.SelectedItem = null;
        }

        int currentLogFontSize = 1;
        private async void FontUp_Tapped(object sender, EventArgs e)
        {
            try
            {
                if (currentLogFontSize == 4) return;
                currentLogFontSize++;
                //var x = Enum.GetName(typeof(NamedSize), currentLogFontSize);
                frm_FontUp.IsEnabled = false;
                foreach (var lbl in stk_Log.Children)
                    ((Label)lbl).FontSize = Device.GetNamedSize((NamedSize)currentLogFontSize, typeof(Label));
                await frm_FontUp.ScaleTo(0.5, 250).ContinueWith(a => frm_FontUp.ScaleTo(1, 100));
                frm_FontUp.IsEnabled = true;
            }
            catch (Exception ex)
            {
                ((Views.MainPage)App.Current.MainPage).Manage_Error(ex);
            }
        }

        private async void FontDown_Tapped(object sender, EventArgs e)
        {
            try
            {
                if (currentLogFontSize == 1) return;
                currentLogFontSize--;
                //var x = Enum.GetName(typeof(NamedSize), currentLogFontSize);
                frm_FontDown.IsEnabled = false;
                foreach (var lbl in stk_Log.Children)
                    ((Label)lbl).FontSize = Device.GetNamedSize((NamedSize)currentLogFontSize, typeof(Label));
                await frm_FontDown.ScaleTo(0.5, 250).ContinueWith(a => frm_FontDown.ScaleTo(1, 100));
                frm_FontDown.IsEnabled = true;
            }
            catch (Exception ex)
            {
                ((Views.MainPage)App.Current.MainPage).Manage_Error(ex);
            }
        }

        private async void stk_Log_Tapped(object sender, EventArgs e)
        {
            try
            {
                await stk_Log.ScaleTo(0.9, 150).ContinueWith(async a => await stk_Log.ScaleTo(1, 75));
                stk_LogOptions.IsVisible = stk_TextOptions.IsVisible = !stk_TextOptions.IsVisible;
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
                await stk_Log.ScaleTo(0.9, 150).ContinueWith(async a => await stk_Log.ScaleTo(1, 75));
                stk_LogOptions.IsVisible = stk_TextOptions.IsVisible = !stk_TextOptions.IsVisible;
                Title_Tapped(null, null);
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
    }

    #region test héritage uc_PinCard
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class V_UDPLog2 : uc_PinCard
    {

        StackLayout stk_Log = new StackLayout() { HorizontalOptions = LayoutOptions.Start };

        public V_UDPLog2()
        {
            var grid = new Grid();
            var label = new Label() { Text = "Log", FontSize = 30, Opacity = 0.3, HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };
            var scrollView = new ScrollView();
            scrollView.Content = stk_Log;
            grid.Children.Add(label);
            grid.Children.Add(scrollView);
            
            var frame = new Frame()
            {
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.End,
                Margin = new Thickness(10),
                Padding = new Thickness(10),
                WidthRequest = 30,
                HeightRequest = 30,
                BackgroundColor = Color.Transparent
            };
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) => {
                Clear_Tapped(null, null);
            };
            frame.GestureRecognizers.Add(tapGestureRecognizer);

            var ffImg = new CachedImage()
            {
                WidthRequest = 40,
                HeightRequest = 40,
                DownsampleToViewSize = true,
                Source = ImageSource.FromFile("reloadOn.png")
            };
            frame.Content = ffImg;
            //grid.Children.Add(frame);

            this.AddContent(grid);
            this.AddTitleContent(frame);

            ViewModels.VM_UDPReceiver.Instance.GetResponse += (s, e) =>
            {
                stk_Log.Children.Add(new Label() { Text = ViewModels.VM_UDPReceiver.Instance.Log.Last() });
            };
        }


        private void Clear_Tapped(View sender, object e)
        {
            stk_Log.Children.Clear();
            //((MainPage)App.Current.MainPage).RandomBackGround();
        }
    }
    #endregion
}