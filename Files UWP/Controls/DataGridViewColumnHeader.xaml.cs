using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
        public DataGridViewColumnHeader()
        {
            this.InitializeComponent();
        }

        private void Header_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var header = sender as Grid;
            if (Math.Abs(e.Delta.Scale) < (header.Width - 10))
            {
                scaleTransform.ScaleX += e.Delta.Scale;
            }
        }
    }
}
