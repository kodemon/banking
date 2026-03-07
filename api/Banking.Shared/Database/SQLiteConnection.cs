namespace Banking.Shared.Database;

public static class SQLiteConnection
{
    public static string Load(string domain)
    {
        LoadEnvFile();

        var basePath =
            Environment.GetEnvironmentVariable("SQLITE_DB_PATH")
            ?? throw new InvalidOperationException(
                "No database path provided. Set SQLITE_DB_PATH in your .env file."
            );

        return $"Data Source={basePath}/{domain}.db";
    }

    private static void LoadEnvFile()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            var envPath = Path.Combine(directory.FullName, ".env");

            if (File.Exists(envPath))
            {
                foreach (var line in File.ReadAllLines(envPath))
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                        continue;

                    var parts = line.Split('=', 2);
                    if (parts.Length != 2)
                        continue;

                    var key = parts[0].Trim();
                    var value = parts[1].Trim().Trim('"').Trim('\'');

                    // Resolve relative paths against the .env file's directory
                    if (key == "SQLITE_DB_PATH")
                    {
                        if (!Path.IsPathRooted(value))
                        {
                            var absolutePath = Path.GetFullPath(
                                Path.Combine(directory.FullName, value)
                            );
                            value = $"{absolutePath}";
                        }
                    }

                    Environment.SetEnvironmentVariable(key, value);
                }
                return;
            }

            directory = directory.Parent;
        }
    }
}
