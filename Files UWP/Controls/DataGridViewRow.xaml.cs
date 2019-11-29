using Files.Filesystem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Files.Controls
{
    public sealed partial class DataGridViewRow : UserControl
    {
        private ObservableCollection<PropertyInfoValueItem> selectedProperties { get; set; } = new ObservableCollection<PropertyInfoValueItem>();
        List<string> allowedPropertyNames { get; set; } = new List<string>();
        public DataGridViewRow()
        {
            this.InitializeComponent();
            this.DataContextChanged += DataGridViewRow_DataContextChanged;
            this.Loaded += DataGridViewRow_Loaded;
        }

        private void DataGridViewRow_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (DataContext == null || Tag == null) { return; }
            selectedProperties.Clear();
            List<PropertyInfo> properties = (DataContext as ListedItem).GetType().GetProperties().ToList();
            allowedPropertyNames.Clear();
            allowedPropertyNames.Add("FileImg");
            allowedPropertyNames.Add("FileName");
            allowedPropertyNames.Add("FileDate");
            allowedPropertyNames.Add("FileType");
            allowedPropertyNames.Add("FileSize");
            int index = 0;
            foreach (PropertyInfo property in properties)
            {
                if (allowedPropertyNames.Contains(property.Name))
                {
                    bool EditAllowed = false;
                    if (property.Name == "FileName")
                    {
                        EditAllowed = true;
                    }
                    var width = (this.Tag as ObservableCollection<DataGridViewHeader>)[index].InitialWidth;
                    selectedProperties.Add(new PropertyInfoValueItem() { PropertyName = property.Name, Value = property.GetValue(DataContext, null), isValueEditable = EditAllowed, cellWidth = width });
                    index++;
                }
            }
        }

        public void DisableRowCutDisplayState()
        {
            var cellDataItem = CellsList.Items[0] as PropertyInfoValueItem;
            var cellContainer = CellsList.ContainerFromItem(cellDataItem) as ListViewItem;
            if (cellContainer != null)
                cellContainer.Opacity = 1.0;
        }

        public void EnableRowCutDisplayState()
        {
            var cellDataItem = CellsList.Items[0] as PropertyInfoValueItem;
            var cellContainer = CellsList.ContainerFromItem(cellDataItem) as DataGridViewCell;
            if (cellContainer != null)
                cellContainer.Opacity = 0.4;
        }



        private void DataGridViewRow_Loaded(object sender, RoutedEventArgs e)
        {     
            
            DataGridViewRow_DataContextChanged(null, null);


            //this.Loaded -= DataGridViewRow_Loaded;
        }
    }

    public class PropertyInfoValueItem
    {
        public string PropertyName { get; set; }
        public object Value { get; set; }
        public bool isValueEditable { get; set; }
        public int cellWidth { get; set; }
    }

    public class CellDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ImageCellTemplate { get; set; }
        public DataTemplate TextCellTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var valueItem = item as PropertyInfoValueItem;
            if ((valueItem.Value as BitmapImage) != null)
            {
                return ImageCellTemplate;
            }
            else
            {
                return TextCellTemplate;
            }

        }
    }

}
