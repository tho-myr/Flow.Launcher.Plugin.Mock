namespace Flow.Launcher.Plugin.Mock.SrcFiles;

public static class MockingCaseConverter {
    
    public static string Convert(string input) {
        var result = new char[input.Length];
        for (int i = 0; i < input.Length; i++) {
            result[i] = i % 2 == 0 ? char.ToUpper(input[i]) : char.ToLower(input[i]);
        }
        return new string(result);
    }
    
}