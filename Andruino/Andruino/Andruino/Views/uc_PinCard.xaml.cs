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
        public event EventHandler Tapped;
        public event EventHandler UnPinTapped;


        public uc_PinCard()
        {
            InitializeComponent();
        }

        public uc_PinCard(View content)
        {
            InitializeComponent();

            SetContent(content);
        }

        public void SetContent(View content)
        {
            stk_Content.Children.Clear();
            stk_Content.Children.Add(content);
        }

        private void frm_Tapped(object sender, EventArgs e)
        {
            Tapped?.Invoke(this, null);
        }

        private void UnPin_Tapped(object sender, EventArgs e)
        {
            UnPinTapped?.Invoke(this, null);
        }


        public View ViewContent { get { return stk_Content.Children.FirstOrDefault(); } }

        bool _IsMinimize = false;
        public bool IsMinimize
        {
            get { return _IsMinimize; }
            set
            {
                _IsMinimize = value;
                stk_Title.IsVisible = !value;
            }
        }
    }
}