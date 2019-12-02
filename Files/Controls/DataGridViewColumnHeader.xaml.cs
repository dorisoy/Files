using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Files.Controls
{
    public sealed partial class DataGridViewColumnHeader : UserControl
    {
        public string HeaderTextProp { get; set; }
        public int InitialWidthProp { get; set; }
        public object SortDirection { get; set; } = null;
        public bool isIconHeader { get; set; } = false;

        public DataGridViewColumnHeader()
        {
            this.InitializeComponent();
        }

        private void Header_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var header = sender as Border;
            if (Math.Abs(e.Delta.Scale) < (header.Width - 10))
            {
                header.Width += e.Delta.Scale;
            }
        }

        PointerPoint StartingPoint;
        private void HeaderBorder_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            StartingPoint = e.GetCurrentPoint(HeaderBorder);
            e.Handled = true;
        }

        private void HeaderBorder_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (e.GetCurrentPoint(HeaderBorder).Properties.IsLeftButtonPressed) 
            {
                if (StartingPoint != null)
                {
                    double movement = e.GetCurrentPoint(HeaderBorder).Position.X - StartingPoint.Position.X;
                    //double absoluteDistance = Math.Abs(movement);
                    if ((HeaderBorder.Width + movement) >= 10)
                    {
                        HeaderBorder.Width += movement;
                    }
                    else
                    {
                        HeaderBorder.Width = 10;
                    }
                    e.Handled = true;
                }
            }
        }

        private void HeaderBorder_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            (sender as Border).BorderThickness = new Thickness(0, 0, 1, 0);
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.SizeWestEast, 2);
        }

        private void HeaderBorder_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            (sender as Border).BorderThickness = new Thickness(0, 0, 0.5, 0);
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);
        }
    }
}
