using System.Windows;
    using System.Windows.Controls;
    
    namespace Flow.Launcher.Plugin.Mock.Settings;
    
    public partial class PluginSettings : UserControl {
    
        private readonly PluginInitContext _context;
        private readonly Settings _settings;
    
        public PluginSettings(PluginInitContext context, SettingsViewModel viewModel)
        {
            InitializeComponent();
            _context = context;
            _settings = viewModel.Settings;
            DataContext = viewModel;
        }
    
        private void OnAddCustomMemeFolderClick(object sender, RoutedEventArgs e)
        {
            var gridView = CustomMemeFoldersListView.View as GridView;
            var setting = new CustomMemeFolderSettingWindow(_settings, _context, gridView);
            setting.ShowDialog();
        }
    
        private void OnDeleteCustomMemeFolderClick(object sender, RoutedEventArgs e) {
            if (_settings.SelectedCustomMemeFolder == null) return;
            
            var selected = _settings.SelectedCustomMemeFolder;
            const string warning = "Are you sure you want to delete this custom meme folder?";
            var result = _context.API.ShowMsgBox(warning, "delete entry :(", MessageBoxButton.YesNo);
                
            if (result != MessageBoxResult.Yes) return;
            _settings.CustomMemeFolders.Remove(selected);
            _context.API.SaveSettingJsonStorage<Settings>();
        }
    
        private void OnEditCustomMemeFolderClick(object sender, RoutedEventArgs e)
        {
            if (_settings.SelectedCustomMemeFolder != null)
            {
                var gridView = CustomMemeFoldersListView.View as GridView;
                var setting = new CustomMemeFolderSettingWindow(_settings, _context, _settings.SelectedCustomMemeFolder, gridView);
                setting.ShowDialog();
            }
        }
    }