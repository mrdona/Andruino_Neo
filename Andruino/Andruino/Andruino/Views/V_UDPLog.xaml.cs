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
		}


        private void Clear_Tapped(object sender, EventArgs e)
        {
            stk_Log.Children.Clear();
            ((MainPage)App.Current.MainPage).RandomBackGround();
        }
    }
}