using System.Diagnostics;

static class Task1b{
    public static Counts Run(string path, int threads, string version = ""){
        Console.WriteLine($"Loading numbers from '{path}' into memory array (not measured)...");
        int[] numbers = LoadNumbers(path);
        Console.WriteLine($"Loaded {numbers.Length} numbers. Running with {threads} threads.");

        if (version == "1"){
            return RunVersion1(numbers, threads);
        }
        if (version == "2"){
            return RunVersion2(numbers, threads);
        }
        if (version == "3"){
            return RunVersion3(numbers, threads);
        }

        // Run all three versions as required by the lab
        Console.WriteLine("--- Version 1: Shared counters without synchronisation (3 runs) ---");
        for (int r = 1; r <= 3; r++){
            var sw1 = Stopwatch.StartNew();
            var c1 = RunVersion1(numbers, threads);
            sw1.Stop();
            Console.WriteLine($"Version 1 (Run {r}): time={sw1.ElapsedMilliseconds}ms | {c1}");
        }

        Console.WriteLine("--- Version 2: Shared counters protected by lock ---");
        var sw2 = Stopwatch.StartNew();
        var c2 = RunVersion2(numbers, threads);
        sw2.Stop();
        Console.WriteLine($"Version 2: time={sw2.ElapsedMilliseconds}ms | {c2}");

        Console.WriteLine("--- Version 3: Separate counter per thread ---");
        var sw3 = Stopwatch.StartNew();
        var c3 = RunVersion3(numbers, threads);
        sw3.Stop();
        Console.WriteLine($"Version 3: time={sw3.ElapsedMilliseconds}ms | {c3}");

        return c3;
    }

    public static Counts RunVersion1(int[] numbers, int threads){
        var shared = new Counts();
        int partSize = numbers.Length / threads;
        var workers = new Thread[threads];

        for (int i = 0; i < threads; i++){
            int id = i;
            int start = id * partSize;
            int end = id == threads - 1 ? numbers.Length : start + partSize;
            workers[id] = new Thread(() => {
                for (int j = start; j < end; j++){
                    shared.Add(numbers[j]);
                }
            });
            workers[id].Start();
        }

        for (int i = 0; i < threads; i++){
            workers[i].Join();
        }
        return shared;
    }

    public static Counts RunVersion2(int[] numbers, int threads){
        var shared = new Counts();
        object lockObj = new object();
        int partSize = numbers.Length / threads;
        var workers = new Thread[threads];

        for (int i = 0; i < threads; i++){
            int id = i;
            int start = id * partSize;
            int end = id == threads - 1 ? numbers.Length : start + partSize;
            workers[id] = new Thread(() => {
                for (int j = start; j < end; j++){
                    int n = numbers[j];
                    lock (lockObj){
                        shared.Add(n);
                    }
                }
            });
            workers[id].Start();
        }

        for (int i = 0; i < threads; i++){
            workers[i].Join();
        }
        return shared;
    }

    public static Counts RunVersion3(int[] numbers, int threads){
        int partSize = numbers.Length / threads;
        var results = new Counts[threads];
        var workers = new Thread[threads];

        for (int i = 0; i < threads; i++){
            int id = i;
            int start = id * partSize;
            int end = id == threads - 1 ? numbers.Length : start + partSize;
            results[id] = new Counts();
            workers[id] = new Thread(() => {
                var c = results[id];
                for (int j = start; j < end; j++){
                    c.Add(numbers[j]);
                }
            });
            workers[id].Start();
        }

        var total = new Counts();
        for (int i = 0; i < threads; i++){
            workers[i].Join();
            total.Merge(results[i]);
        }
        return total;
    }

    static int[] LoadNumbers(string path){
        var list = new int[50_000_000];
        using var r = new StreamReader(path);
        int count = 0;
        string? line;
        while ((line = r.ReadLine()) != null && count < list.Length){
            list[count++] = int.Parse(line);
        }
        if (count < list.Length){
            Array.Resize(ref list, count);
        }
        return list;
    }
}
