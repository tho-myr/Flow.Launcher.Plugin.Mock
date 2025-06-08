using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Flow.Launcher.Plugin.Mock.Settings;

public class CustomMemeFolderViewModel : BaseModel {
    
    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    public Settings.CustomMemeFolder CustomMemeFolder { get; set; }

}