using Files.View_Models;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Files.UserControls
{
    public sealed partial class StatusBarControl : UserControl
    {
        public SettingsViewModel AppSettings => App.AppSettings;
        public DirectoryPropertiesViewModel DirectoryPropertiesViewModel => App.CurrentInstance.ContentPage.DirectoryPropertiesViewModel;
        public SelectedItemsPropertiesViewModel SelectedItemsPropertiesViewModel => App.CurrentInstance.ContentPage.SelectedItemsPropertiesViewModel;

        public StatusBarControl()
        {
            this.InitializeComponent();
        }

        private void LayoutModeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var senderItem = (sender as RadioButtons).SelectedItem as TextBlock;
            if (senderItem != null)
            {
                switch (int.Parse(senderItem.DataContext.ToString()))
                {
                    case 0:
                        if (AppSettings.LayoutMode != 0)
                            AppSettings.ToggleLayoutModeToListView();
                        break;
                    case 1:
                        if (AppSettings.LayoutMode != 1)
                            AppSettings.ToggleLayoutModeToTilesView();
                        break;
                    case 2:
                        if (AppSettings.LayoutMode != 2)
                            AppSettings.ToggleLayoutModeToGridView();
                        break;
                }
            }
            
        }
    }
}