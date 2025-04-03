using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Flow.Launcher.Plugin.Mock.Settings;

public class Settings : BaseModel {
    
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
    
    
    public class CustomMemeFolder : BaseModel {
        
        [Required]
        public string Keyword { get; set; }
        [Required]
        public string FolderPath { get; set; }
        [Required]
        public string Icon { get; set; }
        
        [JsonIgnore]
        public string IconPath => System.IO.Path.Combine(Main.CustomIconsDirectory, Icon);

        public CustomMemeFolder DeepCopy()
        {
            var copy = new CustomMemeFolder
            {
                Keyword = Keyword,
                FolderPath = FolderPath,
                Icon = Icon
            };
            return copy;
        }
    }

}