namespace Flow.Launcher.Plugin.Mock.Settings;

public class SettingsViewModel {
    
    public SettingsViewModel(Settings settings) {
        Settings = settings;
    }

    public Settings Settings { get; }
    
}