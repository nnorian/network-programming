static class Generator
{
    public static void Generate(string path)
    {
        var rnd = new Random(42); // fixed seed -> same file every run
        using var w = new StreamWriter(path);
        w.NewLine = "\n";         // 1 byte line ending, also on Windows
        for (int i = 0; i < 50_000_000; i++)
            w.WriteLine(rnd.Next(-1_000_000, 1_000_001)); // upper bound is exclusive
    }
}
