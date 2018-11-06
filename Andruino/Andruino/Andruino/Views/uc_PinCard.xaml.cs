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

        public uc_PinCard(List<View> content)
        {
            InitializeComponent();

            SetContent(content);
        }

        public uc_PinCard(List<View> content, List<View> titleContent)
        {
            InitializeComponent();

            SetContent(content);
            SetTitleContent(titleContent);
        }

        public void SetContent(List<View> contents)
        {
            stk_Content.Children.Clear();
            contents.ForEach(c => stk_Content.Children.Add(c));
        }
        public void AddContent(View contentToAdd)
        {
            stk_Content.Children.Add(contentToAdd);
            stk_Content.ForceLayout();
        }

        public void SetTitleContent(List<View> contents)
        {
            stk_Title_Content.Children.Clear();
            contents.ForEach(c => stk_Title_Content.Children.Add(c));
        }
        public void AddTitleContent(View contentToAdd)
        {
            stk_Title_Content.Children.Add(contentToAdd);
            stk_Title_Content.ForceLayout();
        }


        private void frm_Tapped(object sender, EventArgs e)
        {
            Tapped?.Invoke(this, null);
        }

        private async void UnPin_Tapped(object sender, EventArgs e)
        {
            await frm_Close.ScaleTo(0.5, 100);
            UnPinTapped?.Invoke(this, null);
            await frm_Close.ScaleTo(0.5, 100);
        }


        public List<View> ViewTitle { get { return stk_Title_Content.Children.ToList(); } }

        public List<View> ViewContent { get { return stk_Content.Children.ToList(); } }

        bool _IsMinimize = false;
        public bool IsMinimize
        {
            get { return _IsMinimize; }
            set
            {
                _IsMinimize = value;
                stk_Title_Content.IsVisible = !value;
                frm_Close.IsVisible = !value;
            }
        }
    }
}