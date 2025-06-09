namespace Flow.Launcher.Plugin.Mock.Settings.Models;

public class SettingsViewModel {
    
    public SettingsViewModel(Settings settings) {
        Settings = settings;
    }

    public Settings Settings { get; }
    
}