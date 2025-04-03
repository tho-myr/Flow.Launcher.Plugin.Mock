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
            var setting = new CustomMemeFolderSettingWindow(_settings.CustomMemeFolders, _context);
            setting.ShowDialog();
        }
    
        private void OnDeleteCustomMemeFolderClick(object sender, RoutedEventArgs e)
        {
            if (_settings.SelectedCustomMemeFolder != null)
            {
                var selected = _settings.SelectedCustomMemeFolder;
                var warning = "Are you sure you want to delete this custom meme folder?";
                var result = MessageBox.Show(warning, string.Empty, MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    // TODO: remove keyword from the list of keywords
                    _settings.CustomMemeFolders.Remove(selected);
                }
            }
        }
    
        private void OnEditCustomMemeFolderClick(object sender, RoutedEventArgs e)
        {
            if (_settings.SelectedCustomMemeFolder != null)
            {
                var setting = new CustomMemeFolderSettingWindow(_settings.CustomMemeFolders, _context, _settings.SelectedCustomMemeFolder);
                setting.ShowDialog();
            }
        }
    }