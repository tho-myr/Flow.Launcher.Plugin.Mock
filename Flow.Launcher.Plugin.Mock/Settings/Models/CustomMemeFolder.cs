using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace Flow.Launcher.Plugin.Mock.Settings.Models;

public class CustomMemeFolder : BaseModel {
    [Required] public string Keyword { get; set; }
    [Required] public string FolderPath { get; set; }

    public string GetIconPath () {
        if (string.IsNullOrEmpty(FolderPath) || !Directory.Exists(FolderPath)) {
            return "Images/missing-icon.png";
        }

        var iconFile = Directory.GetFiles(FolderPath, "_icon.png", SearchOption.TopDirectoryOnly).FirstOrDefault() 
                       ?? Directory.GetFiles(FolderPath, "_icon.jpg", SearchOption.TopDirectoryOnly).FirstOrDefault();

        return iconFile ?? "Images/missing-icon.png";
    }

    public CustomMemeFolder DeepCopy() {
        var copy = new CustomMemeFolder {
            Keyword = Keyword,
            FolderPath = FolderPath,
        };
        return copy;
    }
}