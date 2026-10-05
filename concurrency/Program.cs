using System.Diagnostics;

string task = args.Length > 0 ? args[0] : "";
int threads = args.Length > 1 ? int.Parse(args[1]) : 1;
string file = args.Length > 2 ? args[2] : "numbers.txt";

if (task == "gen"){
    Generator.Generate(file);
    return;
}

if (task == "split"){
    Generator.Split(file);
    return;
}
var sw = Stopwatch.StartNew();
Counts c;
switch (task){
    case "1a":
        c= Task1a.Run(file, threads);
        break;
    case "2":
        c = Task2.Run(file, threads);
        break;
    default:
        Console.WriteLine("usage: Lab1 <gen|split|1a|2> [threads] [file|folder]");
        return;
}

sw.Stop();

long peakMb = Process.GetCurrentProcess().PeakWorkingSet64 / 1024 / 1024;
Console.WriteLine($"task={task} threads={threads} time={sw.ElapsedMilliseconds}ms peak={peakMb}MB");
Console.WriteLine(c);