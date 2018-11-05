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
    public partial class uc_PinCard : ContentView
    {      
        public uc_PinCard()
        {
            InitializeComponent();
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