using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Flow.Launcher.Plugin.Mock.SrcFiles;

namespace Flow.Launcher.Plugin.Mock.Settings.Models;

public class MemeFolder : BaseModel {
    [Required] public string Keyword { get; set; }
    [Required] public string FolderPath { get; set; }
    
    public string Description { get; set; }

    public string GetIconPath (PluginInitContext context) {
        if (Settings.DefaultMemeFolderKeyword.Equals(Keyword)) {
            FolderPath = PluginDir.FullPath(PluginDir.MemesDir, context);
        }
        
        if (string.IsNullOrEmpty(FolderPath) || !Directory.Exists(FolderPath)) {
            return "Images/missing-icon.png";
        }

        var iconFile = Directory.GetFiles(FolderPath, "_icon.png", SearchOption.TopDirectoryOnly).FirstOrDefault() 
                       ?? Directory.GetFiles(FolderPath, "_icon.jpg", SearchOption.TopDirectoryOnly).FirstOrDefault();

        return iconFile ?? "Images/missing-icon.png";
    }

    public MemeFolder DeepCopy() {
        var copy = new MemeFolder {
            Keyword = Keyword,
            FolderPath = FolderPath,
            Description = Description
        };
        return copy;
    }
}