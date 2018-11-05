using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Andruino.Views;
using System.Net;
using System.Linq;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Andruino
{
    public partial class App : Application
    {
        public ViewModels.VM_Config CurrentConfig = new ViewModels.VM_Config()
        {
            LocalIP = Dns.GetHostAddresses(Dns.GetHostName()).First().ToString(),
            ServerPort = 2390,
            ServerIP = "192.168.1.44"
        };


        public App()
        {
            InitializeComponent();


            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
