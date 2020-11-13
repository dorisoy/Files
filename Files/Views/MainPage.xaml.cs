using Files.Controllers;
using Files.Controls;
using Files.Filesystem;
using Files.UserControls.MultitaskingControl;
using Files.View_Models;
using Files.Views.Pages;
using Microsoft.Toolkit.Uwp.Extensions;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources.Core;
using Windows.Storage;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using static Files.Helpers.PathNormalization;

namespace Files.Views
{
    /// <summary>
    /// The root page of Files
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public SettingsViewModel AppSettings => App.AppSettings;
        public static IMultitaskingControl MultitaskingControl { get; set; }

        private TabItem _SelectedTabItem;

        public TabItem SelectedTabItem
        {
            get
            {
                return _SelectedTabItem;
            }
            set
            {
                _SelectedTabItem = value;
                NotifyPropertyChanged(nameof(SelectedTabItem));
            }
        }

        public static ObservableCollection<INavigationControlItem> SideBarItems = new ObservableCollection<INavigationControlItem>();

        public MainPage()
        {
            this.InitializeComponent();

            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;
            var CoreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            CoreTitleBar.ExtendViewIntoTitleBar = true;

            var flowDirectionSetting = ResourceContext.GetForCurrentView().QualifierValues["LayoutDirection"];

            if (flowDirectionSetting == "RTL")
            {
                FlowDirection = FlowDirection.RightToLeft;
            }
            AllowDrop = true;
        }

        public static string initialNavArgs = null;
        protected override void OnNavigatedTo(NavigationEventArgs eventArgs)
        {
            initialNavArgs = eventArgs.Parameter?.ToString();
            if (eventArgs.NavigationMode != NavigationMode.Back)
            {
                App.AppSettings = new SettingsViewModel();
                App.InteractionViewModel = new InteractionViewModel();
                App.SidebarPinnedController = new SidebarPinnedController();

                Helpers.ThemeHelper.Initialize();
            }
        }

        public static async Task<TabItem> AddNewTab()
        {
            return await AddNewTabByPathAsync(typeof(ModernShellPage), "NewTab".GetLocalized());
        }

        public static async Task<TabItem> AddNewTabByPathAsync(Type type, string path, int atIndex = -1)
        {
            string tabLocationHeader = null;
            Microsoft.UI.Xaml.Controls.FontIconSource fontIconSource = new Microsoft.UI.Xaml.Controls.FontIconSource();
            fontIconSource.FontFamily = App.Current.Resources["FluentUIGlyphs"] as FontFamily;

            if (path != null)
            {
                var isRoot = Path.GetPathRoot(path) == path;

                if (Path.IsPathRooted(path) || isRoot) // Or is a directory or a root (drive)
                {
                    var normalizedPath = NormalizePath(path);

                    var dirName = Path.GetDirectoryName(normalizedPath);
                    if (dirName != null)
                    {
                        tabLocationHeader = Path.GetFileName(path);
                        fontIconSource.Glyph = "\xea55";
                    }
                    else
                    {
                        // Pick the best icon for this tab
                        var remDriveNames = (await KnownFolders.RemovableDevices.GetFoldersAsync()).Select(x => x.DisplayName);

                        if (!remDriveNames.Contains(normalizedPath))
                        {
                            if (path != "A:" && path != "B:") // Check if it's using (generally) floppy-reserved letters.
                            {
                                fontIconSource.Glyph = "\xeb4a"; // Floppy Disk icon
                            }
                            else
                            {
                                fontIconSource.Glyph = "\xeb8b"; // Hard Disk icon
                            }

                            tabLocationHeader = normalizedPath;
                        }
                        else
                        {
                            fontIconSource.Glyph = "\xec0a";
                            tabLocationHeader = (await KnownFolders.RemovableDevices.GetFolderAsync(path)).DisplayName;
                        }
                    }
                }
                else
                {
                    // Invalid path, open new tab instead (explorer opens Documents when it fails)
                    Debug.WriteLine($"Invalid path \"{path}\" in InstanceTabsView.xaml.cs\\AddNewTab");

                    path = "NewTab".GetLocalized();
                    tabLocationHeader = "NewTab".GetLocalized();
                    fontIconSource.Glyph = "\xe90c";
                }
            }

            var tabItem = new TabItem()
            {
                Header = tabLocationHeader,
                Path = path,
                Content = new Grid()
                {
                    Children =
                    {
                        new Frame()
                        {
                            CacheSize = 0,
                            Tag = new TabItemContent()
                            {
                                InitialPageType = type,
                                NavigationArg = path
                            }
                        }
                    },
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch
                },
                IconSource = fontIconSource,
                Description = null
            };

            var tabViewItemFrame = (tabItem.Content as Grid).Children[0] as Frame;
            tabViewItemFrame.Loaded += TabViewItemFrame_Loaded;
            return tabItem;
        }

        private static void TabViewItemFrame_Loaded(object sender, RoutedEventArgs e)
        {
            var frame = sender as Frame;
            if (frame.CurrentSourcePageType != typeof(ModernShellPage))
            {
                frame.Navigate((frame.Tag as TabItemContent).InitialPageType, (frame.Tag as TabItemContent).NavigationArg);
                frame.Loaded -= TabViewItemFrame_Loaded;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void NavigateToNumberedTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            int indexToSelect = 0;

            switch (sender.Key)
            {
                case VirtualKey.Number1:
                    indexToSelect = 0;
                    break;

                case VirtualKey.Number2:
                    indexToSelect = 1;
                    break;

                case VirtualKey.Number3:
                    indexToSelect = 2;
                    break;

                case VirtualKey.Number4:
                    indexToSelect = 3;
                    break;

                case VirtualKey.Number5:
                    indexToSelect = 4;
                    break;

                case VirtualKey.Number6:
                    indexToSelect = 5;
                    break;

                case VirtualKey.Number7:
                    indexToSelect = 6;
                    break;

                case VirtualKey.Number8:
                    indexToSelect = 7;
                    break;

                case VirtualKey.Number9:
                    // Select the last tab
                    indexToSelect = MultitaskingControl.Items.Count - 1;
                    break;
            }

            // Only select the tab if it is in the list
            if (indexToSelect < MultitaskingControl.Items.Count)
            {
                App.InteractionViewModel.TabStripSelectedIndex = indexToSelect;
            }
            args.Handled = true;
        }

        private async void CloseSelectedTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            if (MultitaskingControl.Items.Count == 1)
            {
                await ApplicationView.GetForCurrentView().TryConsolidateAsync();
            }
            else
            {
                if (App.InteractionViewModel.TabStripSelectedIndex >= MultitaskingControl.Items.Count)
                {
                    var item = MultitaskingControl.Items.Last() as TabItem;
                    // Cleanup resources for the closed tab
                    (((item.Content as Grid).Children[0] as Frame).Content as IShellPage)?.Dispose();
                    MultitaskingControl.Items.RemoveAt(MultitaskingControl.Items.Count - 1);
                }
                else
                {
                    var item = MultitaskingControl.Items[App.InteractionViewModel.TabStripSelectedIndex] as TabItem;
                    // Cleanup resources for the closed tab
                    (((item.Content as Grid).Children[0] as Frame).Content as IShellPage)?.Dispose();
                    MultitaskingControl.Items.RemoveAt(App.InteractionViewModel.TabStripSelectedIndex);
                }
            }
            args.Handled = true;
        }

        private async void AddNewInstanceAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            await AddNewTabByPathAsync(typeof(ModernShellPage), "NewTab".GetLocalized());
            args.Handled = true;
        }

        private async void HorizontalMultitaskingControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (MultitaskingControl == null)
            {
                MultitaskingControl = HorizontalMultitaskingControl;
             
                if (string.IsNullOrEmpty(initialNavArgs))
                {
                    try
                    {
                        if (App.AppSettings.ResumeAfterRestart)
                        {
                            App.AppSettings.ResumeAfterRestart = false;

                            foreach (string path in App.AppSettings.LastSessionPages)
                            {
                                await AddNewTabByPathAsync(typeof(ModernShellPage), path);
                            }

                            if (!App.AppSettings.ContinueLastSessionOnStartUp)
                            {
                                App.AppSettings.LastSessionPages = null;
                            }
                        }
                        else if (App.AppSettings.OpenASpecificPageOnStartup)
                        {
                            if (App.AppSettings.PagesOnStartupList != null)
                            {
                                foreach (string path in App.AppSettings.PagesOnStartupList)
                                {
                                    await AddNewTabByPathAsync(typeof(ModernShellPage), path);
                                }
                            }
                            else
                            {
                                MultitaskingControl.Items.Add(await AddNewTab());
                            }
                        }
                        else if (App.AppSettings.ContinueLastSessionOnStartUp)
                        {
                            if (App.AppSettings.LastSessionPages != null)
                            {
                                foreach (string path in App.AppSettings.LastSessionPages)
                                {
                                    await AddNewTabByPathAsync(typeof(ModernShellPage), path);
                                }
                                App.AppSettings.LastSessionPages = new string[] { "NewTab".GetLocalized() };
                            }
                            else
                            {
                                MultitaskingControl.Items.Add(await AddNewTab());
                            }
                        }
                        else
                        {
                            MultitaskingControl.Items.Add(await AddNewTab());
                        }
                    }
                    catch (Exception)
                    {
                        MultitaskingControl.Items.Add(await AddNewTab());
                    }
                }
                else if (string.IsNullOrEmpty(initialNavArgs))
                {
                    MultitaskingControl.Items.Add(await AddNewTab());
                }
                else
                {
                    await AddNewTabByPathAsync(typeof(ModernShellPage), initialNavArgs);
                }

                // Initial setting of SelectedTabItem
                Frame rootFrame = Window.Current.Content as Frame;
                var mainView = rootFrame.Content as MainPage;
                mainView.SelectedTabItem = MultitaskingControl.Items[App.InteractionViewModel.TabStripSelectedIndex] as TabItem;

            }
            else
            {
                if (MultitaskingControl.Items.Count > 0)
                {
                    HorizontalMultitaskingControl.Items.Clear();
                    foreach (TabItem ti in MultitaskingControl.Items)
                    {
                        HorizontalMultitaskingControl.Items.Add(ti);
                    }
                }

                MultitaskingControl = HorizontalMultitaskingControl;
            }
        }
    }
}