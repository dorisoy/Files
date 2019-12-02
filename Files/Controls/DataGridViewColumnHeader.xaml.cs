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
        public CellResizeWidth cellWidth { get; set; } = new CellResizeWidth();

        public DataGridViewColumnHeader()
        {
            this.InitializeComponent();
            cellWidth.Width = Width;
        }

        
    }
}
