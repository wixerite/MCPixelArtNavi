using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MCPArtNavi.UserApp.PixelCanvasRulers
{
    /// <summary>
    /// RulerScrollViewer.xaml の相互作用ロジック
    /// </summary>
    [ContentProperty("InnerContent")]
    public partial class RulerScrollViewer : UserControl
    {
        public static readonly DependencyProperty InnerContentProperty =
        DependencyProperty.Register("InnerContent", typeof(object), typeof(RulerScrollViewer));

        public object InnerContent
        {
            get { return (object)GetValue(InnerContentProperty); }
            set { SetValue(InnerContentProperty, value); }
        }



        public double ContentScale
        {
            get { return (double)GetValue(ContentScaleProperty); }
            set { SetValue(ContentScaleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContentScale.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentScaleProperty =
            DependencyProperty.Register("ContentScale", typeof(double), typeof(RulerScrollViewer), new PropertyMetadata(1.0d));



        public RulerScrollViewer()
        {
            InitializeComponent();

            this.mainScrollViewer.ScrollChanged += _mainScrollViewer_ScrollChanged;
        }

        private void _mainScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            this.horizontalRulerScrollViewer.ScrollToHorizontalOffset(e.HorizontalOffset);
            this.verticalRulerScrollViewer.ScrollToVerticalOffset(e.VerticalOffset);
        }
    }
}
