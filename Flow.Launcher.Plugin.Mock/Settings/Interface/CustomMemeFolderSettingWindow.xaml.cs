using System.IO;
using System.Windows;
using System.Windows.Controls;
using Flow.Launcher.Plugin.Mock.Settings.Models;

namespace Flow.Launcher.Plugin.Mock.Settings.Interface;

public partial class CustomMemeFolderSettingWindow {
    
    private const string KeywordInvalidTitle = "invalid keyword :(";
    private const string KeywordEmptyMessage = "keyword is emtpy. please enter a keyword :3";
    private const string KeywordCannotContainSpacesMessage = "keyword cannot contain spaces. please enter a valid keyword :3";
    private const string KeywordAlreadyExistsMessage = "a custom meme folder with this keyword already exists. please choose another one :3";
    
    private const string FolderPathInvalidTitle = "invalid folder path :(";
    private const string FolderPathEmptyMessage = "folder path is empty. please enter a folder path :3";
    private const string FolderPathNotExistingMessage = "path could not be resolved or doesn't exist. please enter a valid path or check if the given path exists :3";

    private Models.Settings _settings;
    private PluginInitContext _context;
    private Action _action;
    private GridView _gridView;
    private CustomMemeFolder _customMemeFolder;
    private readonly CustomMemeFolder _oldCustomMemeFolder;
    private readonly CustomMemeFolderViewModel _customMemeFolderViewModel;

    public CustomMemeFolderSettingWindow(Models.Settings settings, PluginInitContext context, CustomMemeFolder old, GridView gridView) {
        _oldCustomMemeFolder = old;
        _customMemeFolderViewModel = new CustomMemeFolderViewModel { CustomMemeFolder = old.DeepCopy() };
        Initialize(settings, context, Action.Edit, gridView);
    }

    public CustomMemeFolderSettingWindow(Models.Settings settings, PluginInitContext context, GridView gridView) {
        _customMemeFolderViewModel = new CustomMemeFolderViewModel { CustomMemeFolder = new CustomMemeFolder() };
        Initialize(settings, context, Action.Add, gridView);
    }

    private void Initialize(Models.Settings settings, PluginInitContext context, Action action, GridView gridView) {
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
        if (IsCurrentCustomMemeFolderInvalid()) return;
        _settings.CustomMemeFolders.Add(_customMemeFolder);
        _context.API.SaveSettingJsonStorage<Models.Settings>();
        Close();
        RefreshColumnWidths(_gridView);
    }

    private void EditCustomMemeFolder() {
        if (IsCurrentCustomMemeFolderInvalid()) return;
        var index = _settings.CustomMemeFolders.IndexOf(_oldCustomMemeFolder);
        _settings.CustomMemeFolders[index] = _customMemeFolder;
        _context.API.SaveSettingJsonStorage<Models.Settings>();
        Close();
        RefreshColumnWidths(_gridView);
    }

    private bool IsCurrentCustomMemeFolderInvalid() {
        if (string.IsNullOrEmpty(_customMemeFolder.Keyword)) {
            _context.API.ShowMsgBox(KeywordEmptyMessage, KeywordInvalidTitle);
            return true;
        }
        if (_customMemeFolder.Keyword.Contains(' ')) {
            _context.API.ShowMsgBox(KeywordCannotContainSpacesMessage, KeywordInvalidTitle);
            return true;
        }
        if (_settings.CustomMemeFolderKeywordExists(_customMemeFolder.Keyword) && 
            (_oldCustomMemeFolder == null || _customMemeFolder.Keyword != _oldCustomMemeFolder.Keyword)) {
            _context.API.ShowMsgBox(KeywordAlreadyExistsMessage, KeywordInvalidTitle);
            return true;
        }
        if (string.IsNullOrEmpty(_customMemeFolder.FolderPath)) {
            _context.API.ShowMsgBox(FolderPathEmptyMessage, FolderPathInvalidTitle);
            return true;
        }
        if (!Directory.Exists(_customMemeFolder.FolderPath)) {
            _context.API.ShowMsgBox(FolderPathNotExistingMessage, FolderPathInvalidTitle);
            return true;
        }

        return false;
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