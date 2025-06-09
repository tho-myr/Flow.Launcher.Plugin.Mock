using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;

namespace Flow.Launcher.Plugin.Mock.Settings.Models;

public class Settings : BaseModel {
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    // set; is needed for correct JSON serialization/deserialization
    public ObservableCollection<CustomMemeFolder> CustomMemeFolders { get; set; } = new();
    [JsonIgnore] public CustomMemeFolder SelectedCustomMemeFolder { get; set; }

    public Settings() {
        if (CustomMemeFolders.Count > 0) {
            SelectedCustomMemeFolder = CustomMemeFolders[0];
        }
    }

    public bool CustomMemeFolderKeywordExists(string keyword) {
        return CustomMemeFolders.Any(folder => folder.Keyword.Equals(keyword, StringComparison.OrdinalIgnoreCase));
    }
}