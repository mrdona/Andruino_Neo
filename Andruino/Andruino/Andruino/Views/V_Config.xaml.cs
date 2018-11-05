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
            this.BindingContext = ((App)App.Current).CurrentConfig;
        }

        private void V_Config_BindingContextChanged(object sender, EventArgs e)
        {
            //if(App.Current.MainPage != null)
            //    this.BindingContext = ((MainPage)App.Current.MainPage).CurrentConfig;
        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            return base.OnMeasure(widthConstraint, heightConstraint);
        }

        private void lbl_IP_Focused(object sender, FocusEventArgs e)
        {
            
        }
    }
}