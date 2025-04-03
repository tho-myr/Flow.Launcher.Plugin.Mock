using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Flow.Launcher.Plugin.Mock.Settings;

public class CustomMemeFolderViewModel : BaseModel {
    public Settings.CustomMemeFolder CustomMemeFolder { get; set; }

    public void UpdateIconAttributes(Settings.CustomMemeFolder customMemeFolder, string fullPathToSelectedImage) {
        var parentDirectorySelectedImg = Directory.GetParent(fullPathToSelectedImage).ToString();
        
        customMemeFolder.Icon = parentDirectorySelectedImg != Main.CustomIconsDirectory
            ? Path.GetFileName(fullPathToSelectedImage)
            : string.Empty;
    }

    public bool CopyNewImageToUserDataDirectoryIfRequired(PluginInitContext context, Settings.CustomMemeFolder customMemeFolder, string fullPathToSelectedImage, string fullPathToOriginalImage) {
        var destinationFileNameFullPath =
            Path.Combine(Main.CustomIconsDirectory, Path.GetFileName(fullPathToSelectedImage));

        var parentDirectorySelectedImg = Directory.GetParent(fullPathToSelectedImage).ToString();

        if (parentDirectorySelectedImg != Main.CustomIconsDirectory) {
            try {
                File.Copy(fullPathToSelectedImage, destinationFileNameFullPath);
                return true;
            }
            catch (Exception) {
                context.API.ShowMsg(string.Format("Copying the selected image file to {0} has failed, changes will now be reverted", destinationFileNameFullPath));
                UpdateIconAttributes(customMemeFolder, fullPathToOriginalImage);
                return false;
            }
        }
        return true;
    }

    internal void SetupCustomIconsDirectory() {
        if (!Directory.Exists(Main.CustomIconsDirectory))
            Directory.CreateDirectory(Main.CustomIconsDirectory);
    }

    internal async ValueTask<ImageSource> LoadPreviewIconAsync(string pathToPreviewIconImage) {
        if (string.IsNullOrEmpty(pathToPreviewIconImage) || !File.Exists(pathToPreviewIconImage))
            return null;

        var bitmap = new BitmapImage();
        await using (var stream = new FileStream(pathToPreviewIconImage, FileMode.Open, FileAccess.Read, FileShare.Read)) {
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = stream;
            bitmap.EndInit();
            bitmap.Freeze();
        }

        return bitmap;
    }
}