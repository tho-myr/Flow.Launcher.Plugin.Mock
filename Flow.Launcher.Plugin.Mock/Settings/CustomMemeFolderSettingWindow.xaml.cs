using System.Collections.Generic;
using System.Windows;
using Microsoft.Win32;

namespace Flow.Launcher.Plugin.Mock.Settings;

public partial class CustomMemeFolderSettingWindow {
    
    private readonly Settings.CustomMemeFolder _oldCustomMemeFolder;
    private Settings.CustomMemeFolder _customMemeFolder;
    private IList<Settings.CustomMemeFolder> _customMemeFolders;
    private Action _action;
    private PluginInitContext _context;
    private readonly CustomMemeFolderViewModel _customMemeFolderViewModel;
    private string selectedNewIconImageFullPath;

    public CustomMemeFolderSettingWindow(IList<Settings.CustomMemeFolder> folders, PluginInitContext context, Settings.CustomMemeFolder old) {
        _oldCustomMemeFolder = old;
        _customMemeFolderViewModel = new CustomMemeFolderViewModel { CustomMemeFolder = old.DeepCopy() };
        Initialize(folders, context, Action.Edit);
    }

    public CustomMemeFolderSettingWindow(IList<Settings.CustomMemeFolder> folders, PluginInitContext context) {
        _customMemeFolderViewModel = new CustomMemeFolderViewModel { CustomMemeFolder = new Settings.CustomMemeFolder() };
        Initialize(folders, context, Action.Add);
    }

    private async void Initialize(IList<Settings.CustomMemeFolder> folders, PluginInitContext context, Action action) {
        InitializeComponent();
        DataContext = _customMemeFolderViewModel;
        _customMemeFolder = _customMemeFolderViewModel.CustomMemeFolder;
        _customMemeFolders = folders;
        _action = action;
        _context = context;

        _customMemeFolderViewModel.SetupCustomIconsDirectory();

        ImgPreviewIcon.Source = await _customMemeFolderViewModel.LoadPreviewIconAsync(_customMemeFolder.Icon);
    }

    private void OnCancelButtonClick(object sender, RoutedEventArgs e) {
        Close();
    }

    private void OnConfirmButtonClick(object sender, RoutedEventArgs e) {
        if (string.IsNullOrEmpty(_customMemeFolder.Keyword)) {
            MessageBox.Show("please enter a keyword");
        } else if (string.IsNullOrEmpty(_customMemeFolder.FolderPath)) {
            MessageBox.Show("please enter a folder path");
        } else if (string.IsNullOrEmpty(selectedNewIconImageFullPath)) {
            MessageBox.Show("please select a icon");
        } else if (_action == Action.Add) {
            AddCustomMemeFolder();
        } else if (_action == Action.Edit) {
            EditCustomMemeFolder();
        }
    }

    private void AddCustomMemeFolder() {
        // TODO: Check if the keyword already exists
        var success = _customMemeFolderViewModel.CopyNewImageToUserDataDirectoryIfRequired(
            _context, 
            _customMemeFolder, 
            selectedNewIconImageFullPath,
            string.Empty
        );
        if (!success) {
            MessageBox.Show("failed to copy the selected image file to custom icons folder.");
            return;
        }
        _customMemeFolderViewModel.UpdateIconAttributes(_customMemeFolder, selectedNewIconImageFullPath);
        _customMemeFolders.Add(_customMemeFolder);
        Close();
    }

    private void EditCustomMemeFolder() {
        // TODO: keyword handling
        var index = _customMemeFolders.IndexOf(_oldCustomMemeFolder);
        _customMemeFolders[index] = _customMemeFolder;

        if (!string.IsNullOrEmpty(selectedNewIconImageFullPath)) {
            _customMemeFolderViewModel.UpdateIconAttributes(_customMemeFolder, selectedNewIconImageFullPath);
            _customMemeFolderViewModel.CopyNewImageToUserDataDirectoryIfRequired(
                _context, 
                _customMemeFolder, 
                selectedNewIconImageFullPath,
                _oldCustomMemeFolder.Icon
            );
        }

        Close();
    }

    private async void OnSelectIconClick(object sender, RoutedEventArgs e) {
        const string filter = "image files (*.jpg, *.jpeg, *.gif, *.png, *.bmp) |*.jpg; *.jpeg; *.gif; *.png; *.bmp";
        var dialog = new OpenFileDialog { InitialDirectory = Main.CustomIconsDirectory, Filter = filter };
        
        var result = dialog.ShowDialog();
        if (result != true) return;
        selectedNewIconImageFullPath = dialog.FileName;
        
        if (!string.IsNullOrEmpty(selectedNewIconImageFullPath)) {
            ImgPreviewIcon.Source = await _customMemeFolderViewModel.LoadPreviewIconAsync(selectedNewIconImageFullPath);
        }
    }
}

public enum Action
{
    Add,
    Edit
}