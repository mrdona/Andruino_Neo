using PanCardView;
using PanCardView.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace Andruino.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();

            try
            {
                InitControls();
            }
            catch (Exception ex)
            {
                Manage_Error(ex);
            }
        }


        public void RandomBackGround()
        {
            try
            {
                var rand = new Random().Next(0, 7);
                switch (rand)
                {
                    case 0: img_background.Source = ImageSource.FromFile("badsmile.png"); break;
                    case 1: img_background.Source = ImageSource.FromFile("beaten.png"); break;
                    case 2: img_background.Source = ImageSource.FromFile("bigsmile.png"); break;
                    case 3: img_background.Source = ImageSource.FromFile("shocked.png"); break;
                    case 4: img_background.Source = ImageSource.FromFile("exciting.png"); break;
                    case 5: img_background.Source = ImageSource.FromFile("icon.png"); break;
                    case 6: img_background.Source = ImageSource.FromFile("scorn.png"); break;
                    case 7: img_background.Source = ImageSource.FromFile("secretsmile.png"); break;
                        //case 8: img_background.Source = ImageSource.FromFile("electricshock.png"); break;
                }
            }
            catch (Exception ex)
            {
                Manage_Error(ex);
            }
        }


        private void InitControls()
        {
            try
            {
                //carousel_Main = new CarouselView();
                //{
                //    ItemTemplate = new DataTemplate(() => new ContentView()) //your template
                //};

                //var frm_tap = new TapGestureRecognizer();
                //frm_tap.Tapped += (s, ee) => frm_Tapped(s, ee);
                carousel_Main.ItemsSource = new ObservableCollection<object>();

                var frmConfig = new Frame()
                {
                    HasShadow = true,
                    Padding = new Thickness(5),
                    Scale = 0.5,
                    Content = new Views.V_Config(),
                    HeightRequest = 50
                };
                carousel_Main.ItemsSource.Add(frmConfig);
                //frmConfig.GestureRecognizers.Add(frm_tap);

                var frmCommands = new Frame()
                {
                    HasShadow = true,
                    Padding = new Thickness(5),
                    Scale = 0.5,
                    Content = new Views.V_Command()
                };
                carousel_Main.ItemsSource.Add(frmCommands);
                //frmCommands.GestureRecognizers.Add(frm_tap);

                carousel_Main.Children.Add(new IndicatorsControl());
            }
            catch (Exception ex)
            {
                Manage_Error(ex);
            }
        }

        public void Manage_Error(Exception ex)
        {
            try
            {
                frm_Error.IsVisible = true;
                lbl_Error.Text = ex.Message;
                lbl_Error_Detail.Text = ex.TargetSite.Name + Environment.NewLine + ex.StackTrace;
                grd_Main.IsEnabled = false;
                grd_Main.Opacity = 0.4;
            }
            catch (Exception subex)
            {
                DisplayAlert("Error", subex.Message, "Cancel");
            }
        }


        private void MainScreen_Tapped(object sender, EventArgs e)
        {
            lbl_Error_Detail.IsVisible = false;
            frm_Error.IsVisible = false;
            grd_Main.IsEnabled = true;
            grd_Main.Opacity = 1;

            if(AppLoadingAnimation.Source == null)
                AppLoadingAnimation.Source = ImageSource.FromFile("giro.gif");
            AppLoadingAnimation.IsVisible = !AppLoadingAnimation.IsVisible;
            if(AppLoadingAnimation.IsVisible)
                AppLoadingAnimation.ReloadImage();

            InitControls();
        }

        private void frm_Error_Tapped(object sender, EventArgs e)
        {
            lbl_Error_Detail.IsVisible = !lbl_Error_Detail.IsVisible;
        }

        private void img_Config_Tapped(object sender, EventArgs e)
        {
            uc_Config.IsVisible = !uc_Config.IsVisible;
            img_Config.Source = uc_Config.IsVisible ? ImageSource.FromFile("optionsOff.png") : ImageSource.FromFile("optionsOn.png");
        }

        
        private void frm_Tapped(object sender, EventArgs e)
        {
            //stk_Main.Children.Clear();
            //var o = sender as Frame;
            //o.Scale = 1;
            //stk_Main.Children.Add(o);
        }
    }
}