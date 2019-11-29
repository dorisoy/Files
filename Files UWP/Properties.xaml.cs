using Files.Filesystem;
using Files.Interacts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Files
{

    public sealed partial class Properties : Page
    {
        ObservableCollection<ExtendedPropertyItem> extendedProperties = new ObservableCollection<ExtendedPropertyItem>();
        ListedItem Item { get; set; }
        ContentDialog PropertiesDialog { get; set; }
        public Properties()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var result = e.Parameter as IEnumerable;
            IList<ListedItem> listedItems = result.OfType<ListedItem>().ToList();
            Item = listedItems[0];
            if (Item.FileType.Equals("Folder"))
            {
                DisplayFolderMetadata(Item.FilePath);
            }
            else
            {
                DisplayFileMetadata(Item.FilePath);
            }

            PropertiesDialog = Frame.Tag as ContentDialog;
            base.OnNavigatedTo(e);
        }

        List<string> knownImageExtensions = new List<string>()
        {
            ".jpg", ".png", ".raw", ".arw", ".cr2", ".nrw", ".k25", ".bmp", ".tiff", ".webp", ".psd", ".heif", ".heic"
        };

        private async void DisplayFileMetadata(string filePath)
        {
            var file = await StorageFile.GetFileFromPathAsync(filePath);
            if (knownImageExtensions.Contains(file.FileType.ToLower()))
            {
                extendedProperties.Clear();
                var imgProps = await file.Properties.GetImagePropertiesAsync();
                extendedProperties.Add(new ExtendedPropertyItem() { Property = "Title", Value = imgProps.Title });
                extendedProperties.Add(new ExtendedPropertyItem() { Property = "Camera Manufacturer", Value = imgProps.CameraManufacturer });
                extendedProperties.Add(new ExtendedPropertyItem() { Property = "Camera Model", Value = imgProps.CameraModel});
                extendedProperties.Add(new ExtendedPropertyItem() { Property = "Date Taken", Value = imgProps.DateTaken.DateTime.ToString()});
                extendedProperties.Add(new ExtendedPropertyItem() { Property = "Width", Value = imgProps.Width.ToString()});
                extendedProperties.Add(new ExtendedPropertyItem() { Property = "Height", Value = imgProps.Height.ToString()});
                extendedProperties.Add(new ExtendedPropertyItem() { Property = "Latitude", Value = imgProps.Latitude.ToString()});
                extendedProperties.Add(new ExtendedPropertyItem() { Property = "Longitude", Value = imgProps.Longitude.ToString()});
                extendedProperties.Add(new ExtendedPropertyItem() { Property = "Orientation", Value = imgProps.Orientation.ToString()});
                extendedProperties.Add(new ExtendedPropertyItem() { Property = "Rating", Value = imgProps.Rating.ToString() + " Stars"});
            }
        }

        private void DisplayFolderMetadata(string filePath)
        {
            
        }
    }

    public class ExtendedPropertyItem
    {
        public string Property { get; set; }
        public string Value { get; set; }
    }
}
