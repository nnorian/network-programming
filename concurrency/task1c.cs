using System.Collections.Concurrent;

static class Task1c{
    public static Counts Run(string path, int workersCount){
        var queue = new BlockingCollection<string[]>(100);
        var results = new Counts[workersCount];
        var workers = new Thread[workersCount];

        for (int i = 0; i < workersCount; i++){
            int id = i;
            results[id] = new Counts();
            workers[id] = new Thread(() => {
                var c = results[id];
                foreach (var batch in queue.GetConsumingEnumerable()){
                    for (int j = 0; j < batch.Length; j++){
                        c.Add(int.Parse(batch[j]));
                    }
                }
            });
            workers[id].Start();
        }

        var reader = new Thread(() => {
            using var r = new StreamReader(path);
            const int batchSize = 10_000;
            var batch = new string[batchSize];
            int idx = 0;
            string? line;
            while ((line = r.ReadLine()) != null){
                batch[idx++] = line;
                if (idx == batchSize){
                    queue.Add(batch);
                    batch = new string[batchSize];
                    idx = 0;
                }
            }
            if (idx > 0){
                Array.Resize(ref batch, idx);
                queue.Add(batch);
            }
            queue.CompleteAdding();
        });
        reader.Start();

        reader.Join();

        var total = new Counts();
        for (int i = 0; i < workersCount; i++){
            workers[i].Join();
            total.Merge(results[i]);
        }
        return total;
    }
}
