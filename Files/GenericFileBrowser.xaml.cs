using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.ComponentModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Files.Enums;
using Files.Filesystem;
using Windows.System;
using Windows.UI.Xaml.Input;
using Files.Controls;
using System.Linq;

namespace Files
{
    public sealed partial class GenericFileBrowser : BaseLayout
    {
        
        public string previousFileName;
        

        public GenericFileBrowser()
        {
            this.InitializeComponent();

            
        }

        
        private void AllView_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;

        }

        private async void AllView_DropAsync(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                App.OccupiedInstance.instanceInteraction.itemsPasted = 0;
                App.OccupiedInstance.instanceInteraction.ItemsToPaste = await e.DataView.GetStorageItemsAsync();
                foreach (IStorageItem item in await e.DataView.GetStorageItemsAsync())
                {
                    if (item.IsOfType(StorageItemTypes.Folder))
                    {
                        App.OccupiedInstance.instanceInteraction.CloneDirectoryAsync((item as StorageFolder).Path, App.OccupiedInstance.instanceViewModel.Universal.path, (item as StorageFolder).DisplayName, false);
                    }
                    else
                    {
                        App.OccupiedInstance.UpdateProgressFlyout(InteractionOperationType.PasteItems, ++App.OccupiedInstance.instanceInteraction.itemsPasted, App.OccupiedInstance.instanceInteraction.ItemsToPaste.Count);
                        await (item as StorageFile).CopyAsync(await StorageFolder.GetFolderFromPathAsync(App.OccupiedInstance.instanceViewModel.Universal.path));
                    }
                }
            }
        }

        private void AllView_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            var textBox = e.EditingElement as TextBox;
            var selectedItem = allView.rootList.SelectedItem as ListedItem;
            int extensionLength = selectedItem.DotFileExtension?.Length ?? 0;

            previousFileName = selectedItem.FileName;
            textBox.Focus(FocusState.Programmatic); // Without this, cannot edit text box when renaming via right-click
            textBox.Select(0, selectedItem.FileName.Length - extensionLength);
        }

        private async void AllView_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Cancel)
                return;

            var selectedItem = allView.rootList.SelectedItem as ListedItem;
            string currentName = previousFileName;
            string newName = (e.EditingElement as TextBox).Text;

            bool successful = await App.OccupiedInstance.instanceInteraction.RenameFileItem(selectedItem, currentName, newName);
            if (!successful)
            {
                selectedItem.FileName = currentName;
                ((sender as DataGrid).Columns[1].GetCellContent(e.Row) as TextBlock).Text = currentName;
            }
        }

        private void GenericItemView_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            allView.rootList.SelectedItem = null;
            App.OccupiedInstance.HomeItems.isEnabled = false;
            App.OccupiedInstance.ShareItems.isEnabled = false;
        }

        private void AllView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            allView.CommitEdit(1);
            if (e.AddedItems.Count > 0)
            {
                App.OccupiedInstance.HomeItems.isEnabled = true;
                App.OccupiedInstance.ShareItems.isEnabled = true;
            }
            else if (allView.rootList.SelectedItems.Count == 0)
            {
                App.OccupiedInstance.HomeItems.isEnabled = false;
                App.OccupiedInstance.ShareItems.isEnabled = false;
            }
        }

        private void AllView_DragStarting(UIElement sender, DragStartingEventArgs args)
        {
            args.DragUI.SetContentFromDataPackage();
        }
        
        private void AllView_Sorting(object sender, DataGridColumnEventArgs e)
        {
            
        }

        private void AllView_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                App.OccupiedInstance.instanceInteraction.List_ItemClick(null, null);
                e.Handled = true;
            }
        }
    }
}
