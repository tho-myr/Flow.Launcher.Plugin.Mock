using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;
using Flow.Launcher.Plugin.Mock.SrcFiles;

namespace Flow.Launcher.Plugin.Mock.Settings.Models;

public class Settings : BaseModel {
    
    public const string DefaultMemeFolderKeyword = "default";
    
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    // set; is needed for correct JSON serialization/deserialization
    public ObservableCollection<MemeFolder> CustomMemeFolders { get; set; } = new();
    [JsonIgnore] public MemeFolder SelectedCustomMemeFolder { get; set; }
    [JsonIgnore] private MemeFolder DefaultMemeFolder { get; } = new() {
        Keyword = DefaultMemeFolderKeyword,
        FolderPath = "Images/Memes",
        Description = "default meme folder containing a variety of mocking memes (\u2b2dω\u2b2d)"
    };

    public Settings() {
        if (CustomMemeFolders.Count > 0) {
            SelectedCustomMemeFolder = CustomMemeFolders[0];
        }
    }
    
    public List<MemeFolder> GetAllMemeFolders() {
        var allFolders = new List<MemeFolder> { DefaultMemeFolder };
        allFolders.AddRange(CustomMemeFolders.Select(folder => folder.DeepCopy()));
        return allFolders;
    }

    public bool CustomMemeFolderKeywordExists(string keyword) {
        return GetAllMemeFolders().Any(folder => folder.Keyword.Equals(keyword, StringComparison.OrdinalIgnoreCase));
    }

    public MemeFolder GetCustomMemeFolder(PluginInitContext context, string keyword) {
        var memeFolder = GetAllMemeFolders().FirstOrDefault(folder => folder.Keyword.Equals(keyword, StringComparison.OrdinalIgnoreCase));
        if (memeFolder is { Keyword: DefaultMemeFolderKeyword }) {
            memeFolder.FolderPath = PluginDir.FullPath(PluginDir.MemesDir, context);
        }
        return memeFolder;
    }
}