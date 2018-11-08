using PanCardView;
using PanCardView.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace Andruino.Views
{
    /// <summary>
    /// Page Principale de l'application
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        bool _gifRunning = false;


        /// <summary>
        /// Constructeur
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            try
            {
                //MessagingCenter.Subscribe<MainPage, string>(this, "Refresh", Refresh);

                frm_Error.BackgroundColor = Color.LightGray.MultiplyAlpha(0.7);
                InitControls();
            }
            catch (Exception ex)
            {
                Manage_Error(ex);
            }
            finally
            {
                RunRefresh();
            }
        }

        private async void Refresh(MainPage arg1, string arg2)
        {
            await Task.Delay(1);
        }


        /// <summary>
        /// Rafraîchissemnt automatique
        /// </summary>
        public async void RunRefresh()
        {
            try
            {
                while (true)
                {
                    await RandomBackGroundAsync();
                    await Task.Delay(10 * 1000);
                }
            }
            catch (Exception ex)
            {
                Manage_Error(ex);
            }
        }

        /// <summary>
        /// Change la tête de l'Arduino
        /// </summary>
        /// <returns></returns>
        public async Task RandomBackGroundAsync()
        {
            try
            {
                var rand = new Random().Next(0, 18);
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

                    case 8: img_background.Source = ImageSource.FromFile("anger.png"); break;
                    case 9: img_background.Source = ImageSource.FromFile("cry.png"); break;
                    case 10: img_background.Source = ImageSource.FromFile("eyesdroped.png"); break;
                    case 11: img_background.Source = ImageSource.FromFile("girl.png"); break;
                    case 12: img_background.Source = ImageSource.FromFile("haha.png"); break;

                    case 13: img_background.Source = ImageSource.FromFile("nothingtosay.png"); break;
                    case 14: img_background.Source = ImageSource.FromFile("shame.png"); break;
                    case 15: img_background.Source = ImageSource.FromFile("superman.png"); break;
                    case 16: img_background.Source = ImageSource.FromFile("theironman.png"); break;
                    case 17: img_background.Source = ImageSource.FromFile("what.png"); break;

                    case 18: img_background.Source = ImageSource.FromFile("test.gif"); break;
                }
                await img_background.ScaleTo(0.5, 150).ContinueWith(a => img_background.ScaleTo(1, 50));

            }
            catch (Exception ex)
            {
                Manage_Error(ex);
            }
        }


        /// <summary>
        /// Tous les controles
        /// </summary>
        ObservableCollection<View> controls = new ObservableCollection<View>();

        /// <summary>
        /// Les controles à afficher dans le carousel
        /// </summary>
        IEnumerable<View> controls_UnPin
        {
            get { return controls.Where(c => ((uc_PinCard)c).IsMinimize); }
        }

        /// <summary>
        /// Les controles à afficher dans le stack principal
        /// </summary>
        IEnumerable<View> controls_Pin
        {
            get { return controls.Where(c => !((uc_PinCard)c).IsMinimize); }
        }

        /// <summary>
        /// Création des controles de la page
        /// </summary>
        private void InitControls()
        {
            try
            {
                controls = new ObservableCollection<View>();

                var commands = new uc_PinCard();
                commands.AddContent(new Views.V_Command() { IsEnabled = false });
                commands.Scale = 0.5;
                //commands.AddTitleContent(new Label()
                //{
                //    Text = "Commandes",
                //    VerticalOptions = LayoutOptions.Center,
                //    VerticalTextAlignment = TextAlignment.Center,
                //    FontAttributes = FontAttributes.Bold,
                //    FontSize = 18
                //});
                commands.Tapped += (o, e) => { frm_Tapped(o, e); };
                commands.UnPinTapped += (o, e) => { frm_UnPin(o, e); };
                controls.Add(commands);

                var log = new uc_PinCard();
                log.AddContent(new Views.V_UDPLog() { IsEnabled = false });
                log.Scale = 0.5;
                //log.AddTitleContent(new Label()
                //{
                //    Text = "Log",
                //    VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center,
                //    FontAttributes = FontAttributes.Bold,
                //    FontSize = 18
                //});
                log.Tapped += (o, e) => { frm_Tapped(o, e); };
                log.UnPinTapped += (o, e) => { frm_UnPin(o, e); };
                controls.Add(log);

                var config = new uc_PinCard();
                config.AddContent(new Views.V_Config() { IsEnabled = false });
                config.Scale = 0.5;
                //config.AddTitleContent(new Label()
                //{
                //    Text = "Configuration",
                //    VerticalOptions = LayoutOptions.Center,
                //    VerticalTextAlignment = TextAlignment.Center,
                //    FontAttributes = FontAttributes.Bold,
                //    FontSize = 18
                //});
                config.Tapped += (o, e) => { frm_Tapped(o, e); };
                config.UnPinTapped += (o, e) => { frm_UnPin(o, e); };
                controls.Add(config);

                var lightChart = new uc_PinCard();
                lightChart.AddContent(new Views.uc_LightChart() { IsEnabled = false });
                lightChart.Scale = 0.5;
                lightChart.AddTitleContent(new Label()
                {
                    Text = "Luminosité",
                    VerticalOptions = LayoutOptions.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 18
                });
                lightChart.Tapped += (o, e) => { frm_Tapped(o, e); };
                lightChart.UnPinTapped += (o, e) => { frm_UnPin(o, e); };
                controls.Add(lightChart);


                //var log2 = new V_UDPLog2();
                //log2.Scale = 0.5;
                //log2.SetTitleContent(new Label()
                //{
                //    Text = "Log2",
                //    VerticalOptions = LayoutOptions.Center,
                //    VerticalTextAlignment = TextAlignment.Center,
                //    FontAttributes = FontAttributes.Bold,
                //    FontSize = 18
                //});
                //log2.Tapped += (o, e) => { frm_Tapped(o, e); };
                //log2.UnPinTapped += (o, e) => { frm_UnPin(o, e); };
                //controls.Add(log2);

                //var log2 = new uc_PinCard();
                //log2.SetContent(new Views.V_UDPLog() { IsEnabled = false });
                //log2.Scale = 0.5;
                //log2.SetTitleContent(new Label()
                //{
                //    Text = "Log 2",
                //    VerticalOptions = LayoutOptions.Center,
                //    VerticalTextAlignment = TextAlignment.Center,
                //    FontAttributes = FontAttributes.Bold,
                //    FontSize = 18
                //});
                //log2.Tapped += (o, e) => { frm_Tapped(o, e); };
                //log2.UnPinTapped += (o, e) => { frm_UnPin(o, e); };
                //controls.Add(log2);




                foreach (var c in controls)
                    ((uc_PinCard)c).IsMinimize = true;

                carousel_Main.IsCyclical = true;
                carousel_Main.ItemsSource = controls;
                carousel_Main.Children.Add(new LeftArrowControl() { IsCyclical = true });
                carousel_Main.Children.Add(new RightArrowControl() { IsCyclical = true });
            }
            catch (Exception ex)
            {
                Manage_Error(ex);
            }
        }


        /// <summary>
        /// Gestion des erreurs
        /// </summary>
        /// <param name="ex"></param>
        public void Manage_Error(Exception ex)
        {
            try
            {
                frm_Error.IsVisible = true;
                lbl_Error.Text = ex.Message;
                lbl_Error_Detail.Text = ex.TargetSite.Name + Environment.NewLine + ex.StackTrace;
                stk_Grid.IsEnabled = false;
                stk_Grid.Opacity = 0.4;
            }
            catch (Exception subex)
            {
                DisplayAlert("Error", subex.Message, "Cancel");
            }
        }

        /// <summary>
        /// Touche sur l'écran : masque l'erreur affichée si il y a lieu ;
        /// affiche l'image animée en fond d'écran
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainScreen_Tapped(object sender, EventArgs e)
        {
            if(!frm_Error.IsVisible)
            {
                try
                {
                    if (img_gif.Source == null)
                        img_gif.Source = ImageSource.FromFile("giro.gif");
                    img_gif.IsVisible = !img_gif.IsVisible;
                    if (_gifRunning) return;

                    if (img_gif.IsVisible)
                    {
                        _gifRunning = true;
                        img_gif.ReloadImage();
                        img_gif.ScaleTo(0.1, 10 * 1000, Easing.CubicIn)
                            .ContinueWith((a) =>
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    img_gif.IsVisible = false;
                                    img_gif.Scale = 1;
                                    _gifRunning = false;
                                }));
                    }
                }
                catch (Exception ex)
                {
                    Manage_Error(ex);
                }
            }

            if (frm_Error.IsVisible)
            {
                sv_Error_Detail.IsVisible = false;
                frm_Error.IsVisible = false;
                stk_Grid.IsEnabled = true;
                stk_Grid.Opacity = 1;
            }
        }

        /// <summary>
        /// Touche sur l'erreur, affiche le stack trace
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frm_Error_Tapped(object sender, EventArgs e)
        {
            sv_Error_Detail.IsVisible = !sv_Error_Detail.IsVisible;
        }
     
        /// <summary>
        /// Ferme le stack trace de l'erreur
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frm_Error_Detail_Close_Tapped(object sender, EventArgs e)
        {
            sv_Error_Detail.IsVisible = !sv_Error_Detail.IsVisible;
        }


        /// <summary>
        /// Touche sur un control; l'affiche dans le stack principal
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frm_Tapped(object sender, EventArgs e)
        {
            try
            {
                if (controls_Pin != null && controls_Pin.Any(c => c == sender)) return;

                bool carouselWasOneOrNone = controls_UnPin != null ? controls_UnPin.Count() == 1 : true;

                if (sender.GetType() != typeof(V_UDPLog2))
                {
                    var o = controls.First(c => c == sender) as uc_PinCard;
                    var newO = new uc_PinCard(o.ViewContent, o.ViewTitle) { IsMinimize = false };
                    controls.Add(newO);
                    newO.ViewContent.ForEach(c => c.IsEnabled = true);
                    newO.Tapped += (oo, ee) => { frm_Tapped(oo, ee); };
                    newO.UnPinTapped += (oo, ee) => { frm_UnPin(oo, ee); };
                    controls.Remove(o);
                }
                else
                    (controls.First(c => c == sender) as V_UDPLog2).IsMinimize = false;

                Refresh_Stacks(carouselWasOneOrNone);

                if (carousel_Main.ItemsSource == null || carousel_Main.ItemsSource.Count == 0)
                    CarrouselHide_Tapped(null, null);
            }
            catch (Exception ex)
            {
                Manage_Error(ex);
            }
        }

        /// <summary>
        /// Fermeture d'un control, l'affiche dans le carousel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frm_UnPin(object sender, EventArgs e)
        {
            try
            {
                bool carouselWasOneOrNone = controls_UnPin != null ? controls_UnPin.Count() == 1 : true;

                if (sender.GetType() != typeof(V_UDPLog2))
                {
                    var o = controls.First(c => c == sender) as uc_PinCard;
                    var newO = new uc_PinCard(o.ViewContent, o.ViewTitle) { IsMinimize = true, Scale = 0.6 };
                    controls.Add(newO);
                    newO.ViewContent.ForEach(c => c.IsEnabled = false);
                    newO.Tapped += (oo, ee) => { frm_Tapped(oo, ee); };
                    newO.UnPinTapped += (oo, ee) => { frm_UnPin(oo, ee); };
                    controls.Remove(o);
                }
                else
                    (controls.First(c => c == sender) as uc_PinCard).IsMinimize = true;

                Refresh_Stacks(carouselWasOneOrNone);
            }
            catch (Exception ex)
            {
                Manage_Error(ex);
            }
        }

        /// <summary>
        /// Rafraîchissement du carousel et du stack principal
        /// </summary>
        /// <param name="wasOneOrNone"></param>
        private void Refresh_Stacks(bool wasOneOrNone)
        {
            try
            {
                carousel_Main.ItemsSource = controls_UnPin != null ? controls_UnPin.ToList() : null;
                //Bug affichage flèches
                if (wasOneOrNone && carousel_Main.ItemsSource != null && carousel_Main.ItemsSource.Count > 1)
                {
                    var arrows = carousel_Main.Children.Where(c => c.GetType() == typeof(ArrowControl));
                    foreach (var a in arrows)
                        carousel_Main.Children.Remove(a);

                    carousel_Main.Children.Add(new LeftArrowControl() { IsCyclical = true });
                    carousel_Main.Children.Add(new RightArrowControl() { IsCyclical = true });
                }

                stk_Main.Children.Clear();
                if (controls_Pin != null)
                    foreach (var c in controls_Pin)
                        try { stk_Main.Children.Add(c); } catch (Exception) { } //Bug Xamarin
            }
            catch (Exception ex)
            {
                Manage_Error(ex);
            }
        }


        /// <summary>
        /// Lors de l'apparition d'un item dans le carousel
        /// </summary>
        /// <param name="view"></param>
        /// <param name="args"></param>
        private void carousel_Main_ItemAppearing(CardsView view, PanCardView.EventArgs.ItemAppearingEventArgs args)
        {
            try
            {
                if(args.Item != null)
                    ((View)args.Item).Scale = 0.6;
            }
            catch (Exception ex)
            {
                Manage_Error(ex);
            }
        }

        /// <summary>
        /// Réduit le carousel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CarrouselHide_Tapped(object sender, EventArgs e)
        {
            try
            {
                await carousel_Main.ScaleTo(0.5, 250);
                carousel_Main.IsVisible = !carousel_Main.IsVisible;
                if (!carousel_Main.IsVisible)
                    await img_Carrousel_Tapped.RotateTo(90, 150);
                else
                    await img_Carrousel_Tapped.RotateTo(270, 150);
                await carousel_Main.ScaleTo(1);
            }
            catch (Exception ex)
            {
                ((Views.MainPage)App.Current.MainPage).Manage_Error(ex);
            }
        }

    }
}