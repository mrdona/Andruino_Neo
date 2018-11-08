using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Andruino.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class uc_LightChart : ContentView
	{
		public uc_LightChart ()
		{
			InitializeComponent ();


            try
            {
                var _colors = new List<string>();
                foreach (var field in typeof(Xamarin.Forms.Color).GetFields(BindingFlags.Static | BindingFlags.Public))
                {
                    if (field != null && !String.IsNullOrEmpty(field.Name))
                        _colors.Add(field.Name);
                }
                pck_Color.ItemsSource = _colors;
                //pck_Color.SelectedIndexChanged += (sender, e) =>
                //{
                //    currentLightColor = (Color)new Tools.StringToColorConverter().Convert(pck_Color.SelectedItem, typeof(Color), null, null);
                //};

                lbl_SyncDelay.Text = "Délai Synchro.: " + Math.Round(sl_delayLIGHT.Value).ToString("0");
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
                    frm_ImgTitle.HorizontalOptions = LayoutOptions.Start;
                    frm_ImgTitle.WidthRequest = frm_ImgTitle.HeightRequest = 40;
                    _PinParent.AddTitleContent(frm_ImgTitle);

                    grd_Main.Children.Remove(frm_CurrentLight);
                    frm_CurrentLight.HorizontalOptions = LayoutOptions.End;
                    frm_CurrentLight.WidthRequest = frm_CurrentLight.HeightRequest = 40;
                    _PinParent.AddTitleContent(frm_CurrentLight);

                    _PinParent.ForceLayout();
                }
            }
            catch (Exception ex)
            {
                ((Views.MainPage)App.Current.MainPage).Manage_Error(ex);
            }
        }


        private int maxPoints = 30;
        private Color currentLightColor = Color.Red;

        List<Microcharts.Entry> listPointsLight = new List<Microcharts.Entry>();
        public List<Microcharts.Entry> ListPointsLight
        {
            get
            {
                if (listPointsLight.Count > maxPoints)
                {
                    var u = new List<Microcharts.Entry>(listPointsLight);
                    u.Reverse();
                    return u.Take(maxPoints).Reverse().ToList();
                }
                return listPointsLight;
            }
            set { listPointsLight = value; }
        }


        private async void Instance_GetResponse(object sender, EventArgs e)
        {
            try
            {
                var response = ViewModels.VM_UDPReceiver.Instance.Log.Last();
                if (response != null && response.Contains("LIGHT_"))
                {
                    ViewModels.VM_UDPReceiver.Instance.Log.Remove(response);
                    response = response.Replace("LIGHT_", "");
                    ViewModels.VM_UDPReceiver.Instance.Log.Add(response);

                    var dummy = response.Replace("Light Analog reading = ", "");
                    var l = int.Parse(dummy);
                    var scaleL = Tools.Scalevalue(l, 0, 1024, 25, 100);
                    var color = currentLightColor.MultiplyAlpha(scaleL / 100);

                    listPointsLight.Add(new Microcharts.Entry(l)
                    {
                        Label = DateTime.Now.ToString("HH:mm:ss"),
                        ValueLabel = l.ToString(),
                        Color = SkiaSharp.SKColor.Parse(Tools.GetHexString(color)),
                        TextColor = SkiaSharp.SKColor.Parse(Tools.GetHexString(Color.Black))
                    });
                    var bgColor = Color.Transparent.MultiplyAlpha(0.1);
                    chartViewLIGHT.Chart = new Microcharts.BarChart() { Entries = ListPointsLight, BackgroundColor = SkiaSharp.SKColor.Parse(Tools.GetHexString(bgColor)), Margin = 0.1f };

                    await frm_ImgTitle.ScaleTo(0.5, 150).ContinueWith(a => frm_ImgTitle.ScaleTo(1, 75));

                    lbl_CurrentLight.Text = l.ToString();
                }
            }
            catch (Exception ex)
            {
                ((Views.MainPage)App.Current.MainPage).Manage_Error(ex);
            }
        }

        bool _OnSync = false;
        bool _SyncEnabled = false;
        private async Task ToggleSync()
        {
            try
            {
                _SyncEnabled = !_SyncEnabled;
                prg_Sync.IsVisible = _SyncEnabled;
                listPointsLight = new List<Microcharts.Entry>();
                stk_LightChart.IsVisible = _SyncEnabled;
                grd_Main_Tapped(null, null);
                frm_ImgTitle.Opacity = _SyncEnabled ? 1 : 0.5;
                frm_ImgRun.IsVisible = !stk_LightChart.IsVisible;
                lbl_CurrentLight.Text = string.Empty;

                while (_OnSync)
                    await Task.Delay(1);

                if (_SyncEnabled)
                {
                    ViewModels.VM_UDPReceiver.Instance.GetResponse += Instance_GetResponse;
                    while (_SyncEnabled)
                    {
                        ViewModels.VM_UDPSender.Instance.SendAsync(((App)App.Current).CurrentConfig, "[CMD]LIGHT[/CMD]");
                        prg_Sync.Progress = 0;
                        _OnSync = true;
                        await prg_Sync.ProgressTo(1, Convert.ToUInt32(Math.Round(sl_delayLIGHT.Value) * 1000), Easing.Linear);
                        _OnSync = false;
                        //await Task.Delay(Convert.ToInt32(Math.Round(sl_delayLIGHT.Value) * 1000));
                    }
                    ViewModels.VM_UDPReceiver.Instance.GetResponse -= Instance_GetResponse;
                }
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

                await ToggleSync();
            }
            catch (Exception ex)
            {
                ((Views.MainPage)App.Current.MainPage).Manage_Error(ex);
            }
        }

        private void sl_delayLIGHT_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            try
            {
                lbl_SyncDelay.Text = "Délai Synchro.: " + Math.Round(sl_delayLIGHT.Value).ToString("0");
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
                if(stk_Options.IsVisible)
                    await stk_Options.ScaleTo(0.9, 250).ContinueWith(a => stk_Options.ScaleTo(1, 50));
                stk_Options.IsVisible = !stk_Options.IsVisible;
            }
            catch (Exception ex)
            {
                ((Views.MainPage)App.Current.MainPage).Manage_Error(ex);
            }
        }

        private void pck_Color_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                currentLightColor = (Color)new Tools.StringToColorConverter().Convert(pck_Color.SelectedItem, typeof(Color), null, null);

                var newList = new List<Microcharts.Entry>();
                foreach (var l in listPointsLight)
                {
                    var scaleL = Tools.Scalevalue(l.Value, 0, 1024, 25, 100);
                    var color = currentLightColor.MultiplyAlpha(scaleL / 100);
                    newList.Add(new Microcharts.Entry(l.Value)
                    {
                        Label = l.Label,
                        ValueLabel = l.Value.ToString(),
                        Color = SkiaSharp.SKColor.Parse(Tools.GetHexString(color)),
                        TextColor = SkiaSharp.SKColor.Parse(Tools.GetHexString(Color.Black))
                    });
                }

                ListPointsLight = newList;

                var bgColor = Color.Transparent.MultiplyAlpha(0.1);
                chartViewLIGHT.Chart = new Microcharts.BarChart() { Entries = ListPointsLight, BackgroundColor = SkiaSharp.SKColor.Parse(Tools.GetHexString(bgColor)), Margin = 0.1f };
            }
            catch (Exception ex)
            {
                ((Views.MainPage)App.Current.MainPage).Manage_Error(ex);
            }
        }

        private async void Run_Tapped(object sender, EventArgs e)
        {
            try
            {
                frm_ImgRun.IsEnabled = false;
                await frm_ImgRun.ScaleTo(0.5, 250).ContinueWith(a => frm_ImgRun.ScaleTo(1, 100));
                frm_ImgRun.IsEnabled = true;

                await ToggleSync();
            }
            catch (Exception ex)
            {
                ((Views.MainPage)App.Current.MainPage).Manage_Error(ex);
            }
        }

        private async void CurrentLight_Tapped(object sender, EventArgs e)
        {
            try
            {
                frm_CurrentLight.IsEnabled = false;
                await frm_CurrentLight.ScaleTo(0.5, 250).ContinueWith(a => frm_CurrentLight.ScaleTo(1, 100));
                frm_CurrentLight.IsEnabled = true;

                await ToggleSync();
            }
            catch (Exception ex)
            {
                ((Views.MainPage)App.Current.MainPage).Manage_Error(ex);
            }
        }

        private void tb_MaxPoints_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                maxPoints = int.Parse(tb_MaxPoints.Text);
            }
            catch (Exception)
            {
                //OSEF
                //((Views.MainPage)App.Current.MainPage).Manage_Error(ex);
            }
        }
    }
}