using Files.Enums;
using Files.Filesystem;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Files.Controls
{
    public sealed partial class DataGridView : UserControl
    {
        public MenuFlyout rowContextFlyout { get; set; }
        public MenuFlyout emptySpaceFlyout { get; set; }

        private DataGridViewHeader _sortedColumn;
        public DataGridViewHeader SortedColumn
        {
            get
            {
                return _sortedColumn;
            }
            set
            {
                if (value.HeaderText == "Name")
                    App.OccupiedInstance.instanceViewModel.DirectorySortOption = SortOption.Name;
                else if (value.HeaderText == "Date modified")
                    App.OccupiedInstance.instanceViewModel.DirectorySortOption = SortOption.DateModified;
                else if (value.HeaderText == "Type")
                    App.OccupiedInstance.instanceViewModel.DirectorySortOption = SortOption.FileType;
                else if (value.HeaderText == "Size")
                    App.OccupiedInstance.instanceViewModel.DirectorySortOption = SortOption.Size;
                else
                    App.OccupiedInstance.instanceViewModel.DirectorySortOption = SortOption.Name;

                if (value != _sortedColumn)
                {
                    // Remove arrow on previous sorted column
                    if (_sortedColumn != null)
                        _sortedColumn.SortDirection = null;
                }
                value.SortDirection = App.OccupiedInstance.instanceViewModel.DirectorySortDirection == SortDirection.Ascending ? DataGridSortDirection.Ascending : DataGridSortDirection.Descending;
                _sortedColumn = value;
            }
        }

        public ObservableCollection<ListedItem> itemsSource { get; set; } = new ObservableCollection<ListedItem>();
        public ObservableCollection<DataGridViewCell> TemplatedRowCells { get; set; } = new ObservableCollection<DataGridViewCell>();
        public ObservableCollection<DataGridViewHeader> dataGridViewHeaders { get; set; } = new ObservableCollection<DataGridViewHeader>();
        public DataGridView()
        {
            this.InitializeComponent();

            
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DirectorySortOption")
            {
                switch (App.OccupiedInstance.instanceViewModel.DirectorySortOption)
                {
                    case SortOption.Name:
                        SortedColumn = dataGridViewHeaders.First(x => x.HeaderText == "Name");
                        break;
                    case SortOption.DateModified:
                        SortedColumn = dataGridViewHeaders.First(x => x.HeaderText == "Date modified");
                        break;
                    case SortOption.FileType:
                        SortedColumn = dataGridViewHeaders.First(x => x.HeaderText == "Type");
                        break;
                    case SortOption.Size:
                        SortedColumn = dataGridViewHeaders.First(x => x.HeaderText == "Size");
                        break;
                }
            }
            else if (e.PropertyName == "DirectorySortDirection")
            {
                // Swap arrows
                SortedColumn = _sortedColumn;
            }
        }


        public void BeginEdit(int cellIndex)
        {
            var cellToEdit = ((rootList.SelectedItem as DataGridViewRow).CellsList.Items[cellIndex] as DataGridViewCell);
            if (cellToEdit.IsEditable)
            {
                cellToEdit.IsEditing = true;
                cellToEdit.CellTextField.Focus(FocusState.Programmatic);
                cellToEdit.CellTextField.IsReadOnly = false;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public async void CommitEdit(int cellIndex)
        {
            var cellToEdit = ((rootList.SelectedItem as DataGridViewRow).CellsList.Items[cellIndex] as DataGridViewCell);
            if (cellToEdit.IsEditable)
            {
                cellToEdit.IsEditing = false;
                cellToEdit.CellTextField.Focus(FocusState.Programmatic);
                cellToEdit.CellTextField.IsReadOnly = true;
                // TODO: In the future, control which Data Source proprty is set
                itemsSource[rootList.SelectedIndex].FileName = cellToEdit.CellTextField.Text;
                if(itemsSource[rootList.SelectedIndex].FileType != "Folder")
                {
                    await (await StorageFile.GetFileFromPathAsync(itemsSource[rootList.SelectedIndex].FilePath)).RenameAsync(cellToEdit.CellTextField.Text);
                }
                else
                {
                    await (await StorageFolder.GetFolderFromPathAsync(itemsSource[rootList.SelectedIndex].FilePath)).RenameAsync(cellToEdit.CellTextField.Text);
                }
                UnloadObject(cellToEdit.CellTextField);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        private void ListViewHeader_ClickToSort(object sender, ItemClickEventArgs e)
        {
            if ((e.ClickedItem as DataGridViewHeader).HeaderText == SortedColumn.HeaderText)
                App.OccupiedInstance.instanceViewModel.IsSortedAscending = !App.OccupiedInstance.instanceViewModel.IsSortedAscending;
            else if (!(e.ClickedItem as DataGridViewHeader).isIconHeader)
                SortedColumn = (e.ClickedItem as DataGridViewHeader);
        }

        private void rootList_Loaded(object sender, RoutedEventArgs e)
        {
            switch (App.OccupiedInstance.instanceViewModel.DirectorySortOption)
            {
                case SortOption.Name:
                    SortedColumn = dataGridViewHeaders.First(x => x.HeaderText == "Name");
                    break;
                case SortOption.DateModified:
                    SortedColumn = dataGridViewHeaders.First(x => x.HeaderText == "Date modified");
                    break;
                case SortOption.FileType:
                    SortedColumn = dataGridViewHeaders.First(x => x.HeaderText == "Type");
                    break;
                case SortOption.Size:
                    SortedColumn = dataGridViewHeaders.First(x => x.HeaderText == "Size");
                    break;
            }

            App.OccupiedInstance.instanceViewModel.PropertyChanged += ViewModel_PropertyChanged;
            rootList.Loaded -= rootList_Loaded;
        }

        private void rootList_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var ItemPressed = ((FrameworkElement)e.OriginalSource).DataContext as ListedItem;
            if (ItemPressed != null)
            {
                // Check if RightTapped row isn't currently selected
                if (!rootList.SelectedItems.Contains(ItemPressed))
                {
                    // The following code is only reachable when a user RightTapped an unselected row
                    rootList.SelectedItems.Clear();
                    rootList.SelectedItems.Add(ItemPressed);
                }

                rowContextFlyout.ShowAt(sender as ListView, e.GetPosition(sender as ListView));
            }
            else
            {
                emptySpaceFlyout.ShowAt(sender as ListView, e.GetPosition(sender as ListView));
            }
        }
    }

    public class DataGridViewHeader
    {
        public int InitialWidth { get; set; }
        public string HeaderText { get; set; }
        public object SortDirection { get; set; } = null;
        public bool isIconHeader { get; set; } = false;
    }

}
