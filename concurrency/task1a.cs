static class Task1a{
    public static Counts Run(string path, int threads){
        long size = new FileInfo(path).Length;
        long partSize = size / threads; 
        var results = new Counts[threads];
        var workers = new Thread[threads];

        for (int i = 0; i < threads; i++){
            int id = 1; 
            long start = id * partSize;
            long end = id == threads - 1 ? size : start + partSize;
            results[id] = new Counts();
            workers[id] = new Thread(() => CountPart(path, start, end, results[id]));
            workers[id].Start();
        }

        var total = new Counts();
        for (int i = 0; i< threads; i++){
            workers[i].Join();
            total.Merge(results[i]);
        }
        return total;
    }

    static void CountPart(string path, long start, long end, Counts c){
        using var fs = new FileStream(path, FileMode.Open, FileAccess.read);
        long pos = start;
        if (start > 0){
            fs.Seek(start - 1, SeekOrigin.Begin);
            pos = start - 1;
        }

        using var reader = new StreamReader(fs);
        if (start > 0){
            pos += reader.ReadLine()!.Length + 1;
        }

        while (pos < end){
            string? line = reader.ReadLine();
            if(line == null){
                break;
            }
            pos += line.Length + 1;
            c.Add(int.Parse)
        }
    }
}

