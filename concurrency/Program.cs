using System.Diagnostics;

string task = args.Length > 0 ? args[0] : "";

if (task == "gen"){
    string genFile = args.Length > 1 ? args[1] : "numbers.txt";
    Generator.Generate(genFile);
    return;
}

if (task == "split"){
    string splitFile = args.Length > 1 ? args[1] : "numbers.txt";
    Generator.Split(splitFile);
    return;
}

int threads = args.Length > 1 ? int.Parse(args[1]) : 1;
string file = args.Length > 2 ? args[2] : "numbers.txt";
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