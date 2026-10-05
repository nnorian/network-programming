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

    // class for taks 2
    //splitting the data
    public static void Split(string path);
    for (int p = 1; p<= 8; p++){
        using var w = new StreamWriter($"part{p}.txt");
        w.NewLine = "\n";
        for (int i = 0; i < 6250000; i++){
            w.WriteLine(r.ReadLine());
        }
    }
}
