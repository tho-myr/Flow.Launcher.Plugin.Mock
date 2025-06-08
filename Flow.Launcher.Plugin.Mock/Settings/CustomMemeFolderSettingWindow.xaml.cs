using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Flow.Launcher.Plugin.Mock.Settings;

public partial class CustomMemeFolderSettingWindow {
    
    private readonly Settings.CustomMemeFolder _oldCustomMemeFolder;
    private Settings.CustomMemeFolder _customMemeFolder;
    private Action _action;
    private PluginInitContext _context;
    private Settings _settings;
    private GridView _gridView;
    private readonly CustomMemeFolderViewModel _customMemeFolderViewModel;

    public CustomMemeFolderSettingWindow(Settings settings, PluginInitContext context, Settings.CustomMemeFolder old, GridView gridView) {
        _oldCustomMemeFolder = old;
        _customMemeFolderViewModel = new CustomMemeFolderViewModel { CustomMemeFolder = old.DeepCopy() };
        Initialize(settings, context, Action.Edit, gridView);
    }

    public CustomMemeFolderSettingWindow(Settings settings, PluginInitContext context, GridView gridView) {
        _customMemeFolderViewModel = new CustomMemeFolderViewModel { CustomMemeFolder = new Settings.CustomMemeFolder() };
        Initialize(settings, context, Action.Add, gridView);
    }

    private void Initialize(Settings settings, PluginInitContext context, Action action, GridView gridView) {
        InitializeComponent();
        DataContext = _customMemeFolderViewModel;
        _customMemeFolder = _customMemeFolderViewModel.CustomMemeFolder;
        _settings = settings;
        _context = context;
        _action = action;
        _gridView = gridView;
    }

    private void OnCancelButtonClick(object sender, RoutedEventArgs e) {
        Close();
    }

    private void OnConfirmButtonClick(object sender, RoutedEventArgs e) {
        if (string.IsNullOrEmpty(_customMemeFolder.Keyword)) {
            _context.API.ShowMsgBox("please enter a keyword");
        } else if (string.IsNullOrEmpty(_customMemeFolder.FolderPath)) {
            _context.API.ShowMsgBox("please enter a folder path");
        } else if (_action == Action.Add) {
            AddCustomMemeFolder();
        } else if (_action == Action.Edit) {
            EditCustomMemeFolder();
        }
    }

    private void AddCustomMemeFolder() {
        if (_settings.CustomMemeFolderKeywordExists(_customMemeFolder.Keyword)) {
            _context.API.ShowMsgBox("A custom meme folder with this keyword already exists :( Please choose a different keyword.");
            return;
        }
        _settings.CustomMemeFolders.Add(_customMemeFolder);
        _context.API.SaveSettingJsonStorage<Settings>();
        Close();
        RefreshColumnWidths(_gridView);
    }

    private void EditCustomMemeFolder() {
        if (_settings.CustomMemeFolderKeywordExists(_customMemeFolder.Keyword) &&
            _customMemeFolder.Keyword != _oldCustomMemeFolder.Keyword)
        {
            _context.API.ShowMsgBox("A custom meme folder with this keyword already exists :( Please choose a different keyword.");
            return;
        }
        var index = _settings.CustomMemeFolders.IndexOf(_oldCustomMemeFolder);
        _settings.CustomMemeFolders[index] = _customMemeFolder;
        _context.API.SaveSettingJsonStorage<Settings>();
        Close();
        RefreshColumnWidths(_gridView);
    }
    
    private static void RefreshColumnWidths(GridView gridView)
    {
        if (gridView == null) return;
        foreach (var column in gridView.Columns)
        {
            // Force the column to refresh its width
            if (double.IsNaN(column.Width))
            {
                column.Width = column.ActualWidth;
            }
            column.Width = double.NaN; // Set back to Auto
        }
    }
}

public enum Action
{
    Add,
    Edit
}