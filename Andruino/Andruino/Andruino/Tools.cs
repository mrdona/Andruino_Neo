using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Andruino
{
    public static class Tools
    {
        public static T GetParent<T>(Xamarin.Forms.View child) where T : new()
        {
            try
            {
                var parent = child.Parent;
                while (parent.GetType() != typeof(T) && parent != null)
                    parent = parent.Parent;
                return (T)(object)parent;
            }
            catch (Exception ex)
            {
                ((Views.MainPage)App.Current.MainPage).Manage_Error(ex);
            }
            return new T();
        }

        public static string GetHexString(Xamarin.Forms.Color color)
        {
            var red = (int)(color.R * 255);
            var green = (int)(color.G * 255);
            var blue = (int)(color.B * 255);
            var alpha = (int)(color.A * 255);
            var hex = $"#{alpha:X2}{red:X2}{green:X2}{blue:X2}";

            return hex;
        }

        public static double Scalevalue(double value, int min, int max, int minScale, int maxScale)
        {
            double scaled = minScale + (double)(value - min) / (max - min) * (maxScale - minScale);
            return scaled;
        }

        public static List<VisualElement> GetAllChildren(VisualElement source)
        {
            List<VisualElement> ret = new List<VisualElement>();

            try
            {
                if (source != null)
                {
                    ContentView contentView = source as ContentView;
                    if (contentView != null)
                    {
                        ret.Add(contentView.Content);
                        foreach (View view in GetAllChildren(contentView))
                            ret.Add(view);

                    }
                    else
                    {
                        Layout<View> viewLayout = source as Layout<View>;
                        if (viewLayout != null)
                        {
                            foreach (View child in viewLayout.Children)
                            {
                                ret.Add(child);
                                foreach (View view in GetAllChildren(child))
                                    ret.Add(view);
                            }
                        }
                        else
                        {
                            ContentPage contentPage = source as ContentPage;
                            if (contentPage != null)
                            {
                                ret.Add(contentPage.Content);
                                foreach (View view in GetAllChildren(contentPage.Content))
                                    ret.Add(view);
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return ret;
        }


        public class StringToColorConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value == null)
                    return Color.Default;

                string valueAsString = value.ToString();
                switch (valueAsString)
                {
                    case (""):
                        {
                            return Color.Default;
                        }
                    case ("Accent"):
                        {
                            return Color.Accent;
                        }
                    default:
                        {
                            var converter = new ColorTypeConverter();
                            var result = converter.ConvertFromInvariantString(valueAsString);
                            return result;
                        }
                }
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return null;
            }

        }
    }
}

namespace Xamarin.Forms
{
    using System;
    using System.Linq;
    public class AutoSizeBehavior : Behavior<ListView>
    {
        ListView _ListView;
        ITemplatedItemsView<Cell> Cells => _ListView;

        protected override void OnAttachedTo(ListView bindable)
        {
            bindable.ItemAppearing += AppearanceChanged;
            bindable.ItemDisappearing += AppearanceChanged;
            _ListView = bindable;
        }

        protected override void OnDetachingFrom(ListView bindable)
        {
            bindable.ItemAppearing -= AppearanceChanged;
            bindable.ItemDisappearing -= AppearanceChanged;
            _ListView = null;
        }

        void AppearanceChanged(object sender, ItemVisibilityEventArgs e) =>
          UpdateHeight(e.Item);

        void UpdateHeight(object item)
        {
            if (_ListView.HasUnevenRows)
            {
                double height;
                if ((height = _ListView.HeightRequest) ==
                    (double)VisualElement.HeightRequestProperty.DefaultValue)
                    height = 0;

                height += MeasureRowHeight(item);
                SetHeight(height);
            }
            else if (_ListView.RowHeight == (int)ListView.RowHeightProperty.DefaultValue)
            {
                var height = MeasureRowHeight(item);
                _ListView.RowHeight = height;
                SetHeight(height);
            }
        }

        int MeasureRowHeight(object item)
        {
            var template = _ListView.ItemTemplate;
            var cell = (Cell)template.CreateContent();
            cell.BindingContext = item;
            var height = cell.RenderHeight;
            var mod = height % 1;
            if (mod > 0)
                height = height - mod + 1;
            return (int)height;
        }

        void SetHeight(double height)
        {
            //TODO if header or footer is string etc.
            if (_ListView.Header is VisualElement header)
                height += header.Height;
            if (_ListView.Footer is VisualElement footer)
                height += footer.Height;
            _ListView.HeightRequest = height;
        }
    }
}
