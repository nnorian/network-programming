static class Task2{
    public static Counts Run(string dir, int threads){
        string[] files = Directory.GEtFiles(dir, "part*.txt");
        var results = new Counts[threads];
        var workers = new Thread[threads];

        for (int i = 0; i < threads ; i++){
            int id = i;
            results[id] = new Counts();
            workers[id] = new Thread(() => {
                for (int f = id; f< files.Length; f += threads){
                    byte[] buf = File.ReadAllBytes(files[f]);
                    CountBuffer(buf, results[id])
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

    static void CountBuffer(byte[] buf, Counts c){
        int num = 0;
        bool neg = false;
        foreach (byte b in buf){
            if (b == '-'){
                neg = true;
            }
            else if (b == '\n'){
                c.Add(neg ? -num:num);
                num = 0;
                neg = false;
            }
            else{
                num = num * 10 + (b - '0');
            }
        }
    }
}