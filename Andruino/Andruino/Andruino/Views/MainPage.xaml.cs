using PanCardView;
using PanCardView.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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


        ObservableCollection<View> controls = new ObservableCollection<View>();
        IEnumerable<View> controls_UnPin
        {
            get { return controls.Where(c => ((uc_PinCard)c).IsMinimize); }
        }
        IEnumerable<View> controls_Pin
        {
            get { return controls.Where(c => !((uc_PinCard)c).IsMinimize); }
        }
        private void InitControls()
        {
            try
            {
                controls = new ObservableCollection<View>();

                var config = new uc_PinCard();
                config.SetContent(new Views.V_Config() { IsEnabled = false });
                config.Scale = 0.5;
                config.Tapped += (o, e) => { frm_Tapped(o, e); };
                config.UnPinTapped += (o, e) => { frm_UnPin(o, e); };
                controls.Add(config);


                var commands = new uc_PinCard();
                commands.SetContent(new Views.V_Command() { IsEnabled = false });
                commands.Scale = 0.5;
                commands.Tapped += (o, e) => { frm_Tapped(o, e); };
                commands.UnPinTapped += (o, e) => { frm_UnPin(o, e); };
                controls.Add(commands);

                foreach (var c in controls)
                    ((uc_PinCard)c).IsMinimize = true;


                carousel_Main.ItemsSource = controls;
                //carousel_Main.Children.Add(new IndicatorsControl() { BackgroundColor = Color.Aquamarine , VerticalOptions = LayoutOptions.Start });
                carousel_Main.Children.Add(new LeftArrowControl());
                carousel_Main.Children.Add(new RightArrowControl());
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

        }

        private void frm_Error_Tapped(object sender, EventArgs e)
        {
            lbl_Error_Detail.IsVisible = !lbl_Error_Detail.IsVisible;
        }

        private void img_Config_Tapped(object sender, EventArgs e)
        {
            try
            {
                uc_Config.IsVisible = !uc_Config.IsVisible;
                img_Config.Source = uc_Config.IsVisible ? ImageSource.FromFile("optionsOff.png") : ImageSource.FromFile("optionsOn.png");
            }
            catch (Exception ex)
            {
                Manage_Error(ex);
            }
        }

        
        private void frm_Tapped(object sender, EventArgs e)
        {
            try
            {
                var o = controls.First(c => c == sender) as uc_PinCard;
                var newO = new uc_PinCard(o.ViewContent) { IsMinimize = false };
                controls.Add(newO);
                newO.Tapped += (oo, ee) => { frm_Tapped(oo, ee); };
                newO.UnPinTapped += (oo, ee) => { frm_UnPin(oo, ee); };
                controls.Remove(o);

                carousel_Main.ItemsSource = controls_UnPin != null ? controls_UnPin.ToList() : null;

                stk_Main.Children.Clear();
                foreach (var c in controls_Pin)
                    stk_Main.Children.Add(c);
            }
            catch (Exception ex)
            {
                Manage_Error(ex);
            }
        }

        private void frm_UnPin(object sender, EventArgs e)
        {
            try
            {
                var o = controls.First(c => c == sender) as uc_PinCard;
                var newO = new uc_PinCard(o.ViewContent) { IsMinimize = true, Scale = 0.6 };
                controls.Add(newO);
                newO.Tapped += (oo, ee) => { frm_Tapped(oo, ee); };
                newO.UnPinTapped += (oo, ee) => { frm_UnPin(oo, ee); };
                controls.Remove(o);

                carousel_Main.ItemsSource = controls_UnPin != null ? controls_UnPin.ToList() : null;

                stk_Main.Children.Clear();
                if(controls_Pin != null)
                    foreach (var c in controls_Pin)
                        stk_Main.Children.Add(c);
            }
            catch (Exception ex)
            {
                Manage_Error(ex);
            }
        }

        private void carousel_Main_ItemAppearing(CardsView view, PanCardView.EventArgs.ItemAppearingEventArgs args)
        {
            try
            {
                ((View)args.Item).Scale = 0.6;
            }
            catch (Exception ex)
            {
                Manage_Error(ex);
            }
        }
    }
}