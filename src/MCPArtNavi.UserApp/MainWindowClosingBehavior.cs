using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MCPArtNavi.UserApp
{
    public static class MainWindowClosingBehavior
    {
        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClosingProperty =
            DependencyProperty.RegisterAttached("Closing", typeof(ICommand), typeof(MainWindowClosingBehavior), new UIPropertyMetadata(new PropertyChangedCallback(ClosingChanged)));

        public static ICommand GetClosing(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(ClosingProperty);
        }

        public static void SetClosing(DependencyObject obj, ICommand value)
        {
            obj.SetValue(ClosingProperty, value);
        }

        public static void ClosingChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            var window = target as Window;
            if (window == null)
                return;

            if (e.NewValue != null)
                window.Closing += _windowClosing;
            else
                window.Closing -= _windowClosing;
        }

        public static void _windowClosing(Object sender, CancelEventArgs e)
        {
            var window = sender as Window;
            if (window == null)
                return;

            var closing = GetClosing(window);
            if (closing == null)
                return;

            if (closing.CanExecute(e))
                closing.Execute(e);
            else
                e.Cancel = true;
        }
    }
}
