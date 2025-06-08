using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;

namespace Flow.Launcher.Plugin.Mock.Settings;

public class Settings : BaseModel {
    
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    // set; is needed for correct JSON serialization/deserialization
    public ObservableCollection<CustomMemeFolder> CustomMemeFolders { get; set; } = new();
    [JsonIgnore]
    public CustomMemeFolder SelectedCustomMemeFolder { get; set; }
    
    public Settings()
    {
        if (CustomMemeFolders.Count > 0)
        {
            SelectedCustomMemeFolder = CustomMemeFolders[0];
        }
    }
    
    public bool CustomMemeFolderKeywordExists(string keyword) {
        return CustomMemeFolders.Any(folder => folder.Keyword.Equals(keyword, StringComparison.OrdinalIgnoreCase));
    }
    
    public class CustomMemeFolder : BaseModel {
        
        [Required]
        public string Keyword { get; set; }
        [Required]
        public string FolderPath { get; set; }

        public CustomMemeFolder DeepCopy()
        {
            var copy = new CustomMemeFolder
            {
                Keyword = Keyword,
                FolderPath = FolderPath,
            };
            return copy;
        }
    }

}