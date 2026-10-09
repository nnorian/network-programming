# Lab 1 — Concurrency measurements Asus Zenbook

All commands run from the `concurrency` folder 

## 0. Setup

```bash

dotnet build -c Release

# generate 50M numbers file 
dotnet run -c Release -- gen numbers.txt

# split into 8 parts for Task 2
dotnet run -c Release -- split numbers.txt
```

 `wc -l numbers.txt`
 `ls part*.txt`

## 1. Task 1a: thread partition, file streaming

```bash
dotnet run -c Release -- 1a 1 numbers.txt
dotnet run -c Release -- 1a 1 numbers.txt

dotnet run -c Release -- 1a 2 numbers.txt
dotnet run -c Release -- 1a 2 numbers.txt

dotnet run -c Release -- 1a 4 numbers.txt
dotnet run -c Release -- 1a 4 numbers.txt

dotnet run -c Release -- 1a 8 numbers.txt
dotnet run -c Release -- 1a 8 numbers.txt

dotnet run -c Release -- 1a 12 numbers.txt
dotnet run -c Release -- 1a 12 numbers.txt

dotnet run -c Release -- 1a 16 numbers.txt
dotnet run -c Release -- 1a 16 numbers.txt

dotnet run -c Release -- 1a 32 numbers.txt
dotnet run -c Release -- 1a 32 numbers.txt

dotnet run -c Release -- 1a 64 numbers.txt
dotnet run -c Release -- 1a 64 numbers.txt
```

## 2. Task 1b: three sync versions, 12 threads

Uses all logical processors (12 on this machine).  
Version 1 runs 3 times to show race condition non-determinism.

```bash
# run all at once 
dotnet run -c Release -- 1b

# or run separately:
# version 1: no sync, 3 times
dotnet run -c Release -- 1b 16 numbers.txt 1

# version 2: lock
dotnet run -c Release -- 1b 16 numbers.txt 2

# version 3: per-thread counters
dotnet run -c Release -- 1b 16 numbers.txt 3
```


## 3. Task 1c — producer-consumer

worker counts:

```bash
dotnet run -c Release -- 1c 1 numbers.txt
dotnet run -c Release -- 1c 1 numbers.txt

dotnet run -c Release -- 1c 2 numbers.txt
dotnet run -c Release -- 1c 2 numbers.txt

dotnet run -c Release -- 1c 4 numbers.txt
dotnet run -c Release -- 1c 4 numbers.txt

dotnet run -c Release -- 1c 8 numbers.txt
dotnet run -c Release -- 1c 8 numbers.txt

dotnet run -c Release -- 1c 16 numbers.txt
dotnet run -c Release -- 1c 16 numbers.txt

dotnet run -c Release -- 1c 32 numbers.txt
dotnet run -c Release -- 1c 32 numbers.txt

dotnet run -c Release -- 1c 64 numbers.txt
dotnet run -c Release -- 1c 64 numbers.txt
```

## 4. Task 2: multi-file, NVMe first


```bash
dotnet run -c Release -- 2 1 .
dotnet run -c Release -- 2 2 .
dotnet run -c Release -- 2 4 .
```

## 5. Task 2 — USB drive

Before each run we eject and replug the USB to flush OS disk cache. Otherwise Linux caches the files in RAM and we would measure memory speed, not USB speed.

```bash

lsblk

dotnet run -c Release -- 2 1 /run/media/$USER/ESD-USB/concurrency

dotnet run -c Release -- 2 2 /run/media/$USER/ESD-USB/concurrency

dotnet run -c Release -- 2 4 /run/media/$USER/ESD-USB/concurrency
```
most relecent measures:

```
[nnorian@nnorian concurrency]$ dotnet run -c Release -- gen numbers.txt
[nnorian@nnorian concurrency]$ ls
bin               Generator.cs                       part1.txt  part7.txt   task1b.cs
chart_task1a.png  Lab1_Concurrency_Report.docx       part2.txt  part8.txt   task1c.cs
chart_task1b.png  Lab1_Concurrency_Report_v2.0.docx  part3.txt  Program.cs  task2.cs
chart_task1c.png  Lab1.csproj                        part4.txt  README.md
chart_task2.png   numbers.txt                        part5.txt  REPORT.md
counts.cs         obj                                part6.txt  task1a.cs
[nnorian@nnorian concurrency]$ dotnet run -c Release -- split numbers.txt
[nnorian@nnorian concurrency]$ ls
bin               Generator.cs                       part1.txt  part7.txt   task1b.cs
chart_task1a.png  Lab1_Concurrency_Report.docx       part2.txt  part8.txt   task1c.cs
chart_task1b.png  Lab1_Concurrency_Report_v2.0.docx  part3.txt  Program.cs  task2.cs
chart_task1c.png  Lab1.csproj                        part4.txt  README.md
chart_task2.png   numbers.txt                        part5.txt  REPORT.md
counts.cs         obj                                part6.txt  task1a.cs
[nnorian@nnorian concurrency]$ wc -l numbers.txt
50000000 numbers.txt
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 1 numbers.txt
task=1a threads=1 time=1077ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 1 numbers.txt
task=1a threads=1 time=1120ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 2 numbers.txt
task=1a threads=2 time=628ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 2 numbers.txt
task=1a threads=2 time=627ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 4 numbers.txt
task=1a threads=4 time=358ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 4 numbers.txt
task=1a threads=4 time=356ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 8 numbers.txt
task=1a threads=8 time=285ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 8 numbers.txt
task=1a threads=8 time=223ms peak=51MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 12 numbers.txt
task=1a threads=12 time=219ms peak=51MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 12 numbers.txt
task=1a threads=12 time=212ms peak=51MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 16 numbers.txt
task=1a threads=16 time=203ms peak=51MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 16 numbers.txt
task=1a threads=16 time=201ms peak=51MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 32 numbers.txt
task=1a threads=32 time=220ms peak=52MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 32 numbers.txt
task=1a threads=32 time=225ms peak=52MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 64 numbers.txt
task=1a threads=64 time=260ms peak=56MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 64 numbers.txt
task=1a threads=64 time=509ms peak=58MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ 
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1b
Loading numbers from 'numbers.txt' into memory array (not measured)...
Loaded 50000000 numbers. Running with 16 threads.
--- Version 1: Shared counters without synchronisation (3 runs) ---
Version 1 (Run 1): time=529ms | neg=5720133 pos=5945751 zero=24 total=11665908
Version 1 (Run 2): time=530ms | neg=5552542 pos=5773813 zero=24 total=11326379
Version 1 (Run 3): time=615ms | neg=4343962 pos=4910610 zero=24 total=9254596
--- Version 2: Shared counters protected by lock ---
Version 2: time=5027ms | neg=25005087 pos=24994889 zero=24 total=50000000
--- Version 3: Separate counter per thread ---
Version 3: time=26ms | neg=25005087 pos=24994889 zero=24 total=50000000
task=1b threads=16 time=7900ms peak=245MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 1 numbers.txt
task=1c threads=1 time=1140ms peak=89MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 1 numbers.txt
task=1c threads=1 time=1149ms peak=101MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 2 numbers.txt
task=1c threads=2 time=956ms peak=78MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 2 numbers.txt
task=1c threads=2 time=951ms peak=60MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 4 numbers.txt
task=1c threads=4 time=1075ms peak=60MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 4 numbers.txt
task=1c threads=4 time=1247ms peak=62MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 8 numbers.txt
task=1c threads=8 time=1419ms peak=66MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 8 numbers.txt
task=1c threads=8 time=1405ms peak=66MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 16 numbers.txt
task=1c threads=16 time=1871ms peak=81MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 16 numbers.txt
task=1c threads=16 time=1953ms peak=77MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 32 numbers.txt
task=1c threads=32 time=2401ms peak=98MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 32 numbers.txt
task=1c threads=32 time=2503ms peak=101MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 64 numbers.txt
task=1c threads=64 time=2640ms peak=134MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 64 numbers.txt
task=1c threads=64 time=2428ms peak=133MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ 
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 2 1 .
task=2 threads=1 time=398ms peak=394MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 2 2 .
task=2 threads=2 time=214ms peak=394MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 2 4 .
task=2 threads=4 time=121ms peak=394MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ 
[nnorian@nnorian concurrency]$ lsblk
NAME        MAJ:MIN RM   SIZE RO TYPE MOUNTPOINTS
sda           8:0    1  14.9G  0 disk 
└─sda1        8:1    1  14.9G  0 part /run/media/nnorian/ESD-USB
zram0       253:0    0     4G  0 disk [SWAP]
nvme0n1     259:0    0 953.9G  0 disk 
├─nvme0n1p1 259:1    0     1G  0 part /boot
└─nvme0n1p2 259:2    0 952.9G  0 part /
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 2 1 /run/media/$USER/ESD-USB/concurrency
task=2 threads=1 time=19562ms peak=394MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 2 2 /run/media/$USER/ESD-USB/concurrency
task=2 threads=2 time=20169ms peak=394MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 2 4 /run/media/$USER/ESD-USB/concurrency
task=2 threads=4 time=18969ms peak=394MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ 
[nnorian@nnorian concurrency]$ [nnorian@nnorian concurrency]$ dotnet run -c Release -- gen numbers.txt
[nnorian@nnorian concurrency]$ ls
bin               Generator.cs                       part1.txt  part7.txt   task1b.cs
chart_task1a.png  Lab1_Concurrency_Report.docx       part2.txt  part8.txt   task1c.cs
chart_task1b.png  Lab1_Concurrency_Report_v2.0.docx  part3.txt  Program.cs  task2.cs
chart_task1c.png  Lab1.csproj                        part4.txt  README.md
chart_task2.png   numbers.txt                        part5.txt  REPORT.md
counts.cs         obj                                part6.txt  task1a.cs
[nnorian@nnorian concurrency]$ dotnet run -c Release -- split numbers.txt
[nnorian@nnorian concurrency]$ ls
bin               Generator.cs                       part1.txt  part7.txt   task1b.cs
chart_task1a.png  Lab1_Concurrency_Report.docx       part2.txt  part8.txt   task1c.cs
chart_task1b.png  Lab1_Concurrency_Report_v2.0.docx  part3.txt  Program.cs  task2.cs
chart_task1c.png  Lab1.csproj                        part4.txt  README.md
chart_task2.png   numbers.txt                        part5.txt  REPORT.md
counts.cs         obj                                part6.txt  task1a.cs
[nnorian@nnorian concurrency]$ wc -l numbers.txt
50000000 numbers.txt
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 1 numbers.txt
task=1a threads=1 time=1077ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 1 numbers.txt
task=1a threads=1 time=1120ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 2 numbers.txt
task=1a threads=2 time=628ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 2 numbers.txt
task=1a threads=2 time=627ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 4 numbers.txt
task=1a threads=4 time=358ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 4 numbers.txt
task=1a threads=4 time=356ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 8 numbers.txt
task=1a threads=8 time=285ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 8 numbers.txt
task=1a threads=8 time=223ms peak=51MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 12 numbers.txt
task=1a threads=12 time=219ms peak=51MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 12 numbers.txt
task=1a threads=12 time=212ms peak=51MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 16 numbers.txt
task=1a threads=16 time=203ms peak=51MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 16 numbers.txt
task=1a threads=16 time=201ms peak=51MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 32 numbers.txt
task=1a threads=32 time=220ms peak=52MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 32 numbers.txt
task=1a threads=32 time=225ms peak=52MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 64 numbers.txt
task=1a threads=64 time=260ms peak=56MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 64 numbers.txt
task=1a threads=64 time=509ms peak=58MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ 
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1b
Loading numbers from 'numbers.txt' into memory array (not measured)...
Loaded 50000000 numbers. Running with 16 threads.
--- Version 1: Shared counters without synchronisation (3 runs) ---
Version 1 (Run 1): time=529ms | neg=5720133 pos=5945751 zero=24 total=11665908
Version 1 (Run 2): time=530ms | neg=5552542 pos=5773813 zero=24 total=11326379
Version 1 (Run 3): time=615ms | neg=4343962 pos=4910610 zero=24 total=9254596
--- Version 2: Shared counters protected by lock ---
Version 2: time=5027ms | neg=25005087 pos=24994889 zero=24 total=50000000
--- Version 3: Separate counter per thread ---
Version 3: time=26ms | neg=25005087 pos=24994889 zero=24 total=50000000
task=1b threads=16 time=7900ms peak=245MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 1 numbers.txt
task=1c threads=1 time=1140ms peak=89MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 1 numbers.txt
task=1c threads=1 time=1149ms peak=101MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 2 numbers.txt
task=1c threads=2 time=956ms peak=78MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 2 numbers.txt
task=1c threads=2 time=951ms peak=60MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 4 numbers.txt
task=1c threads=4 time=1075ms peak=60MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 4 numbers.txt
task=1c threads=4 time=1247ms peak=62MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 8 numbers.txt
task=1c threads=8 time=1419ms peak=66MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 8 numbers.txt
task=1c threads=8 time=1405ms peak=66MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 16 numbers.txt
task=1c threads=16 time=1871ms peak=81MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 16 numbers.txt
task=1c threads=16 time=1953ms peak=77MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 32 numbers.txt
task=1c threads=32 time=2401ms peak=98MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 32 numbers.txt
task=1c threads=32 time=2503ms peak=101MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 64 numbers.txt
task=1c threads=64 time=2640ms peak=134MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 64 numbers.txt
task=1c threads=64 time=2428ms peak=133MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ 
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 2 1 .
task=2 threads=1 time=398ms peak=394MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 2 2 .
task=2 threads=2 time=214ms peak=394MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 2 4 .
task=2 threads=4 time=121ms peak=394MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ 
[nnorian@nnorian concurrency]$ lsblk
NAME        MAJ:MIN RM   SIZE RO TYPE MOUNTPOINTS
sda           8:0    1  14.9G  0 disk 
└─sda1        8:1    1  14.9G  0 part /run/media/nnorian/ESD-USB
[nnorian@nnorian concurrency]$ [nnorian@nnorian concurrency]$ dotnet run -c Release -- gen numbers.txt
[nnorian@nnorian concurrency]$ ls
bin               Generator.cs                       part1.txt  part7.txt   task1b.cs
chart_task1a.png  Lab1_Concurrency_Report.docx       part2.txt  part8.txt   task1c.cs
chart_task1b.png  Lab1_Concurrency_Report_v2.0.docx  part3.txt  Program.cs  task2.cs
chart_task1c.png  Lab1.csproj                        part4.txt  README.md
chart_task2.png   numbers.txt                        part5.txt  REPORT.md
counts.cs         obj                                part6.txt  task1a.cs
[nnorian@nnorian concurrency]$ dotnet run -c Release -- split numbers.txt
[nnorian@nnorian concurrency]$ ls
bin               Generator.cs                       part1.txt  part7.txt   task1b.cs
chart_task1a.png  Lab1_Concurrency_Report.docx       part2.txt  part8.txt   task1c.cs
chart_task1b.png  Lab1_Concurrency_Report_v2.0.docx  part3.txt  Program.cs  task2.cs
chart_task1c.png  Lab1.csproj                        part4.txt  README.md
chart_task2.png   numbers.txt                        part5.txt  REPORT.md
counts.cs         obj                                part6.txt  task1a.cs
[nnorian@nnorian concurrency]$ wc -l numbers.txt
50000000 numbers.txt
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 1 numbers.txt
task=1a threads=1 time=1077ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 1 numbers.txt
task=1a threads=1 time=1120ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 2 numbers.txt
task=1a threads=2 time=628ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 2 numbers.txt
task=1a threads=2 time=627ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 4 numbers.txt
task=1a threads=4 time=358ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 4 numbers.txt
task=1a threads=4 time=356ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 8 numbers.txt
task=1a threads=8 time=285ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 8 numbers.txt
task=1a threads=8 time=223ms peak=51MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 12 numbers.txt
task=1a threads=12 time=219ms peak=51MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 12 numbers.txt
task=1a threads=12 time=212ms peak=51MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 16 numbers.txt
task=1a threads=16 time=203ms peak=51MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 16 numbers.txt
task=1a threads=16 time=201ms peak=51MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 32 numbers.txt
task=1a threads=32 time=220ms peak=52MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 32 numbers.txt
task=1a threads=32 time=225ms peak=52MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 64 numbers.txt
task=1a threads=64 time=260ms peak=56MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 64 numbers.txt
task=1a threads=64 time=509ms peak=58MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ 
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1b
Loading numbers from 'numbers.txt' into memory array (not measured)...
Loaded 50000000 numbers. Running with 16 threads.
--- Version 1: Shared counters without synchronisation (3 runs) ---
Version 1 (Run 1): time=529ms | neg=5720133 pos=5945751 zero=24 total=11665908
Version 1 (Run 2): time=530ms | neg=5552542 pos=5773813 zero=24 total=11326379
Version 1 (Run 3): time=615ms | neg=4343962 pos=4910610 zero=24 total=9254596
--- Version 2: Shared counters protected by lock ---
Version 2: time=5027ms | neg=25005087 pos=24994889 zero=24 total=50000000
--- Version 3: Separate counter per thread ---
Version 3: time=26ms | neg=25005087 pos=24994889 zero=24 total=50000000
task=1b threads=16 time=7900ms peak=245MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 1 numbers.txt
task=1c threads=1 time=1140ms peak=89MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 1 numbers.txt
task=1c threads=1 time=1149ms peak=101MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 2 numbers.txt
task=1c threads=2 time=956ms peak=78MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 2 numbers.txt
task=1c threads=2 time=951ms peak=60MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 4 numbers.txt
task=1c threads=4 time=1075ms peak=60MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 4 numbers.txt
task=1c threads=4 time=1247ms peak=62MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 8 numbers.txt
task=1c threads=8 time=1419ms peak=66MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 8 numbers.txt
task=1c threads=8 time=1405ms peak=66MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 16 numbers.txt
task=1c threads=16 time=1871ms peak=81MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 16 numbers.txt
task=1c threads=16 time=1953ms peak=77MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 32 numbers.txt
task=1c threads=32 time=2401ms peak=98MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 32 numbers.txt
task=1c threads=32 time=2503ms peak=101MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 64 numbers.txt
task=1c threads=64 time=2640ms peak=134MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 64 numbers.txt
task=1c threads=64 time=2428ms peak=133MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ 
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 2 1 .
task=2 threads=1 time=398ms peak=394MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 2 2 .
task=2 threads=2 time=214ms peak=394MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 2 4 .
task=2 threads=4 time=121ms peak=394MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ 
[nnorian@nnorian concurrency]$ lsblk
NAME        MAJ:MIN RM   SIZE RO TYPE MOUNTPOINTS
sda           8:0    1  14.9G  0 disk 
└─sda1        8:1    1  14.9G  0 part /run/media/nnorian/ESD-USB
[nnorian@nnorian concurrency]$ [nnorian@nnorian concurrency]$ dotnet run -c Release -- gen numbers.txt
[nnorian@nnorian concurrency]$ ls
bin               Generator.cs                       part1.txt  part7.txt   task1b.cs
chart_task1a.png  Lab1_Concurrency_Report.docx       part2.txt  part8.txt   task1c.cs
chart_task1b.png  Lab1_Concurrency_Report_v2.0.docx  part3.txt  Program.cs  task2.cs
chart_task1c.png  Lab1.csproj                        part4.txt  README.md
chart_task2.png   numbers.txt                        part5.txt  REPORT.md
counts.cs         obj                                part6.txt  task1a.cs
[nnorian@nnorian concurrency]$ dotnet run -c Release -- split numbers.txt
[nnorian@nnorian concurrency]$ ls
bin               Generator.cs                       part1.txt  part7.txt   task1b.cs
chart_task1a.png  Lab1_Concurrency_Report.docx       part2.txt  part8.txt   task1c.cs
chart_task1b.png  Lab1_Concurrency_Report_v2.0.docx  part3.txt  Program.cs  task2.cs
chart_task1c.png  Lab1.csproj                        part4.txt  README.md
chart_task2.png   numbers.txt                        part5.txt  REPORT.md
counts.cs         obj                                part6.txt  task1a.cs
[nnorian@nnorian concurrency]$ wc -l numbers.txt
50000000 numbers.txt
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 1 numbers.txt
task=1a threads=1 time=1077ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 1 numbers.txt
task=1a threads=1 time=1120ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 2 numbers.txt
task=1a threads=2 time=628ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 2 numbers.txt
task=1a threads=2 time=627ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 4 numbers.txt
task=1a threads=4 time=358ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 4 numbers.txt
task=1a threads=4 time=356ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 8 numbers.txt
task=1a threads=8 time=285ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 8 numbers.txt
task=1a threads=8 time=223ms peak=51MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 12 numbers.txt
task=1a threads=12 time=219ms peak=51MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 12 numbers.txt
task=1a threads=12 time=212ms peak=51MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 16 numbers.txt
task=1a threads=16 time=203ms peak=51MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 16 numbers.txt
task=1a threads=16 time=201ms peak=51MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 32 numbers.txt
task=1a threads=32 time=220ms peak=52MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 32 numbers.txt
task=1a threads=32 time=225ms peak=52MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 64 numbers.txt
task=1a threads=64 time=260ms peak=56MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 64 numbers.txt
task=1a threads=64 time=509ms peak=58MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ 
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1b
Loading numbers from 'numbers.txt' into memory array (not measured)...
Loaded 50000000 numbers. Running with 16 threads.
--- Version 1: Shared counters without synchronisation (3 runs) ---
Version 1 (Run 1): time=529ms | neg=5720133 pos=5945751 zero=24 total=11665908
Version 1 (Run 2): time=530ms | neg=5552542 pos=5773813 zero=24 total=11326379
Version 1 (Run 3): time=615ms | neg=4343962 pos=4910610 zero=24 total=9254596
--- Version 2: Shared counters protected by lock ---
Version 2: time=5027ms | neg=25005087 pos=24994889 zero=24 total=50000000
--- Version 3: Separate counter per thread ---
Version 3: time=26ms | neg=25005087 pos=24994889 zero=24 total=50000000
task=1b threads=16 time=7900ms peak=245MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 1 numbers.txt
task=1c threads=1 time=1140ms peak=89MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 1 numbers.txt
task=1c threads=1 time=1149ms peak=101MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 2 numbers.txt
task=1c threads=2 time=956ms peak=78MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 2 numbers.txt
task=1c threads=2 time=951ms peak=60MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 4 numbers.txt
task=1c threads=4 time=1075ms peak=60MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 4 numbers.txt
task=1c threads=4 time=1247ms peak=62MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 8 numbers.txt
task=1c threads=8 time=1419ms peak=66MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 8 numbers.txt
task=1c threads=8 time=1405ms peak=66MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 16 numbers.txt
task=1c threads=16 time=1871ms peak=81MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 16 numbers.txt
task=1c threads=16 time=1953ms peak=77MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 32 numbers.txt
task=1c threads=32 time=2401ms peak=98MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 32 numbers.txt
task=1c threads=32 time=2503ms peak=101MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 64 numbers.txt
task=1c threads=64 time=2640ms peak=134MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 64 numbers.txt
task=1c threads=64 time=2428ms peak=133MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ 
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 2 1 .
task=2 threads=1 time=398ms peak=394MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 2 2 .
task=2 threads=2 time=214ms peak=394MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 2 4 .
task=2 threads=4 time=121ms peak=394MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ 
[nnorian@nnorian concurrency]$ lsblk
NAME        MAJ:MIN RM   SIZE RO TYPE MOUNTPOINTS
sda           8:0    1  14.9G  0 disk 
└─sda1        8:1    1  14.9G  0 part /run/media/nnorian/ESD-USB
[nnorian@nnorian concurrency]$ [nnorian@nnorian concurrency]$ dotnet run -c Release -- gen numbers.txt
[nnorian@nnorian concurrency]$ ls
bin               Generator.cs                       part1.txt  part7.txt   task1b.cs
chart_task1a.png  Lab1_Concurrency_Report.docx       part2.txt  part8.txt   task1c.cs
chart_task1b.png  Lab1_Concurrency_Report_v2.0.docx  part3.txt  Program.cs  task2.cs
chart_task1c.png  Lab1.csproj                        part4.txt  README.md
chart_task2.png   numbers.txt                        part5.txt  REPORT.md
counts.cs         obj                                part6.txt  task1a.cs
[nnorian@nnorian concurrency]$ dotnet run -c Release -- split numbers.txt
[nnorian@nnorian concurrency]$ ls
bin               Generator.cs                       part1.txt  part7.txt   task1b.cs
chart_task1a.png  Lab1_Concurrency_Report.docx       part2.txt  part8.txt   task1c.cs
chart_task1b.png  Lab1_Concurrency_Report_v2.0.docx  part3.txt  Program.cs  task2.cs
chart_task1c.png  Lab1.csproj                        part4.txt  README.md
chart_task2.png   numbers.txt                        part5.txt  REPORT.md
counts.cs         obj                                part6.txt  task1a.cs
[nnorian@nnorian concurrency]$ wc -l numbers.txt
50000000 numbers.txt
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 1 numbers.txt
task=1a threads=1 time=1077ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 1 numbers.txt
task=1a threads=1 time=1120ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 2 numbers.txt
task=1a threads=2 time=628ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 2 numbers.txt
task=1a threads=2 time=627ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 4 numbers.txt
task=1a threads=4 time=358ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 4 numbers.txt
task=1a threads=4 time=356ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 8 numbers.txt
task=1a threads=8 time=285ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 8 numbers.txt
task=1a threads=8 time=223ms peak=51MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 12 numbers.txt
task=1a threads=12 time=219ms peak=51MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 12 numbers.txt
task=1a threads=12 time=212ms peak=51MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 16 numbers.txt
task=1a threads=16 time=203ms peak=51MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 16 numbers.txt
task=1a threads=16 time=201ms peak=51MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 32 numbers.txt
task=1a threads=32 time=220ms peak=52MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 32 numbers.txt
task=1a threads=32 time=225ms peak=52MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 64 numbers.txt
task=1a threads=64 time=260ms peak=56MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 64 numbers.txt
task=1a threads=64 time=509ms peak=58MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ 
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1b
Loading numbers from 'numbers.txt' into memory array (not measured)...
Loaded 50000000 numbers. Running with 16 threads.
--- Version 1: Shared counters without synchronisation (3 runs) ---
Version 1 (Run 1): time=529ms | neg=5720133 pos=5945751 zero=24 total=11665908
Version 1 (Run 2): time=530ms | neg=5552542 pos=5773813 zero=24 total=11326379
Version 1 (Run 3): time=615ms | neg=4343962 pos=4910610 zero=24 total=9254596
--- Version 2: Shared counters protected by lock ---
Version 2: time=5027ms | neg=25005087 pos=24994889 zero=24 total=50000000
--- Version 3: Separate counter per thread ---
Version 3: time=26ms | neg=25005087 pos=24994889 zero=24 total=50000000
task=1b threads=16 time=7900ms peak=245MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 1 numbers.txt
task=1c threads=1 time=1140ms peak=89MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 1 numbers.txt
task=1c threads=1 time=1149ms peak=101MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 2 numbers.txt
task=1c threads=2 time=956ms peak=78MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 2 numbers.txt
task=1c threads=2 time=951ms peak=60MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 4 numbers.txt
task=1c threads=4 time=1075ms peak=60MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 4 numbers.txt
task=1c threads=4 time=1247ms peak=62MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 8 numbers.txt
task=1c threads=8 time=1419ms peak=66MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 8 numbers.txt
task=1c threads=8 time=1405ms peak=66MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 16 numbers.txt
task=1c threads=16 time=1871ms peak=81MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 16 numbers.txt
task=1c threads=16 time=1953ms peak=77MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 32 numbers.txt
task=1c threads=32 time=2401ms peak=98MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 32 numbers.txt
task=1c threads=32 time=2503ms peak=101MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 64 numbers.txt
task=1c threads=64 time=2640ms peak=134MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 64 numbers.txt
task=1c threads=64 time=2428ms peak=133MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ 
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 2 1 .
task=2 threads=1 time=398ms peak=394MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 2 2 .
task=2 threads=2 time=214ms peak=394MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 2 4 .
task=2 threads=4 time=121ms peak=394MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ 
[nnorian@nnorian concurrency]$ lsblk
NAME        MAJ:MIN RM   SIZE RO TYPE MOUNTPOINTS
sda           8:0    1  14.9G  0 disk 
└─sda1        8:1    1  14.9G  0 part /run/media/nnorian/ESD-USB
[nnorian@nnorian concurrency]$ [nnorian@nnorian concurrency]$ dotnet run -c Release -- gen numbers.txt
[nnorian@nnorian concurrency]$ ls
bin               Generator.cs                       part1.txt  part7.txt   task1b.cs
chart_task1a.png  Lab1_Concurrency_Report.docx       part2.txt  part8.txt   task1c.cs
chart_task1b.png  Lab1_Concurrency_Report_v2.0.docx  part3.txt  Program.cs  task2.cs
chart_task1c.png  Lab1.csproj                        part4.txt  README.md
chart_task2.png   numbers.txt                        part5.txt  REPORT.md
counts.cs         obj                                part6.txt  task1a.cs
[nnorian@nnorian concurrency]$ dotnet run -c Release -- split numbers.txt
[nnorian@nnorian concurrency]$ ls
bin               Generator.cs                       part1.txt  part7.txt   task1b.cs
chart_task1a.png  Lab1_Concurrency_Report.docx       part2.txt  part8.txt   task1c.cs
chart_task1b.png  Lab1_Concurrency_Report_v2.0.docx  part3.txt  Program.cs  task2.cs
chart_task1c.png  Lab1.csproj                        part4.txt  README.md
chart_task2.png   numbers.txt                        part5.txt  REPORT.md
counts.cs         obj                                part6.txt  task1a.cs
[nnorian@nnorian concurrency]$ wc -l numbers.txt
50000000 numbers.txt
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 1 numbers.txt
task=1a threads=1 time=1077ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 1 numbers.txt
task=1a threads=1 time=1120ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 2 numbers.txt
task=1a threads=2 time=628ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 2 numbers.txt
task=1a threads=2 time=627ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 4 numbers.txt
task=1a threads=4 time=358ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 4 numbers.txt
task=1a threads=4 time=356ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 8 numbers.txt
task=1a threads=8 time=285ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 8 numbers.txt
task=1a threads=8 time=223ms peak=51MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 12 numbers.txt
task=1a threads=12 time=219ms peak=51MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 12 numbers.txt
task=1a threads=12 time=212ms peak=51MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 16 numbers.txt
task=1a threads=16 time=203ms peak=51MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 16 numbers.txt
task=1a threads=16 time=201ms peak=51MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 32 numbers.txt
task=1a threads=32 time=220ms peak=52MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 32 numbers.txt
task=1a threads=32 time=225ms peak=52MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 64 numbers.txt
task=1a threads=64 time=260ms peak=56MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 64 numbers.txt
task=1a threads=64 time=509ms peak=58MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ 
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1b
Loading numbers from 'numbers.txt' into memory array (not measured)...
Loaded 50000000 numbers. Running with 16 threads.
--- Version 1: Shared counters without synchronisation (3 runs) ---
Version 1 (Run 1): time=529ms | neg=5720133 pos=5945751 zero=24 total=11665908
Version 1 (Run 2): time=530ms | neg=5552542 pos=5773813 zero=24 total=11326379
Version 1 (Run 3): time=615ms | neg=4343962 pos=4910610 zero=24 total=9254596
--- Version 2: Shared counters protected by lock ---
Version 2: time=5027ms | neg=25005087 pos=24994889 zero=24 total=50000000
--- Version 3: Separate counter per thread ---
Version 3: time=26ms | neg=25005087 pos=24994889 zero=24 total=50000000
task=1b threads=16 time=7900ms peak=245MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 1 numbers.txt
task=1c threads=1 time=1140ms peak=89MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 1 numbers.txt
task=1c threads=1 time=1149ms peak=101MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 2 numbers.txt
task=1c threads=2 time=956ms peak=78MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 2 numbers.txt
task=1c threads=2 time=951ms peak=60MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 4 numbers.txt
task=1c threads=4 time=1075ms peak=60MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 4 numbers.txt
task=1c threads=4 time=1247ms peak=62MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 8 numbers.txt
task=1c threads=8 time=1419ms peak=66MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 8 numbers.txt
task=1c threads=8 time=1405ms peak=66MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 16 numbers.txt
task=1c threads=16 time=1871ms peak=81MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 16 numbers.txt
task=1c threads=16 time=1953ms peak=77MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 32 numbers.txt
task=1c threads=32 time=2401ms peak=98MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 32 numbers.txt
task=1c threads=32 time=2503ms peak=101MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 64 numbers.txt
task=1c threads=64 time=2640ms peak=134MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 64 numbers.txt
task=1c threads=64 time=2428ms peak=133MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ 
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 2 1 .
task=2 threads=1 time=398ms peak=394MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 2 2 .
task=2 threads=2 time=214ms peak=394MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 2 4 .
task=2 threads=4 time=121ms peak=394MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ 
[nnorian@nnorian concurrency]$ lsblk
NAME        MAJ:MIN RM   SIZE RO TYPE MOUNTPOINTS
sda           8:0    1  14.9G  0 disk 
└─sda1        8:1    1  14.9G  0 part /run/media/nnorian/ESD-USB
[nnorian@nnorian concurrency]$ [nnorian@nnorian concurrency]$ dotnet run -c Release -- gen numbers.txt
[nnorian@nnorian concurrency]$ ls
bin               Generator.cs                       part1.txt  part7.txt   task1b.cs
chart_task1a.png  Lab1_Concurrency_Report.docx       part2.txt  part8.txt   task1c.cs
chart_task1b.png  Lab1_Concurrency_Report_v2.0.docx  part3.txt  Program.cs  task2.cs
chart_task1c.png  Lab1.csproj                        part4.txt  README.md
chart_task2.png   numbers.txt                        part5.txt  REPORT.md
counts.cs         obj                                part6.txt  task1a.cs
[nnorian@nnorian concurrency]$ dotnet run -c Release -- split numbers.txt
[nnorian@nnorian concurrency]$ ls
bin               Generator.cs                       part1.txt  part7.txt   task1b.cs
chart_task1a.png  Lab1_Concurrency_Report.docx       part2.txt  part8.txt   task1c.cs
chart_task1b.png  Lab1_Concurrency_Report_v2.0.docx  part3.txt  Program.cs  task2.cs
chart_task1c.png  Lab1.csproj                        part4.txt  README.md
chart_task2.png   numbers.txt                        part5.txt  REPORT.md
counts.cs         obj                                part6.txt  task1a.cs
[nnorian@nnorian concurrency]$ wc -l numbers.txt
50000000 numbers.txt
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 1 numbers.txt
task=1a threads=1 time=1077ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 1 numbers.txt
task=1a threads=1 time=1120ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 2 numbers.txt
task=1a threads=2 time=628ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 2 numbers.txt
task=1a threads=2 time=627ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 4 numbers.txt
task=1a threads=4 time=358ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 4 numbers.txt
task=1a threads=4 time=356ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 8 numbers.txt
task=1a threads=8 time=285ms peak=53MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 8 numbers.txt
task=1a threads=8 time=223ms peak=51MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 12 numbers.txt
task=1a threads=12 time=219ms peak=51MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 12 numbers.txt
task=1a threads=12 time=212ms peak=51MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 16 numbers.txt
task=1a threads=16 time=203ms peak=51MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 16 numbers.txt
task=1a threads=16 time=201ms peak=51MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 32 numbers.txt
task=1a threads=32 time=220ms peak=52MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 32 numbers.txt
task=1a threads=32 time=225ms peak=52MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 64 numbers.txt
task=1a threads=64 time=260ms peak=56MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1a 64 numbers.txt
task=1a threads=64 time=509ms peak=58MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ 
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1b
Loading numbers from 'numbers.txt' into memory array (not measured)...
Loaded 50000000 numbers. Running with 16 threads.
--- Version 1: Shared counters without synchronisation (3 runs) ---
Version 1 (Run 1): time=529ms | neg=5720133 pos=5945751 zero=24 total=11665908
Version 1 (Run 2): time=530ms | neg=5552542 pos=5773813 zero=24 total=11326379
Version 1 (Run 3): time=615ms | neg=4343962 pos=4910610 zero=24 total=9254596
--- Version 2: Shared counters protected by lock ---
Version 2: time=5027ms | neg=25005087 pos=24994889 zero=24 total=50000000
--- Version 3: Separate counter per thread ---
Version 3: time=26ms | neg=25005087 pos=24994889 zero=24 total=50000000
task=1b threads=16 time=7900ms peak=245MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 1 numbers.txt
task=1c threads=1 time=1140ms peak=89MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 1 numbers.txt
task=1c threads=1 time=1149ms peak=101MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 2 numbers.txt
task=1c threads=2 time=956ms peak=78MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 2 numbers.txt
task=1c threads=2 time=951ms peak=60MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 4 numbers.txt
task=1c threads=4 time=1075ms peak=60MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 4 numbers.txt
task=1c threads=4 time=1247ms peak=62MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 8 numbers.txt
task=1c threads=8 time=1419ms peak=66MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 8 numbers.txt
task=1c threads=8 time=1405ms peak=66MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 16 numbers.txt
task=1c threads=16 time=1871ms peak=81MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 16 numbers.txt
task=1c threads=16 time=1953ms peak=77MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 32 numbers.txt
task=1c threads=32 time=2401ms peak=98MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 32 numbers.txt
task=1c threads=32 time=2503ms peak=101MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 64 numbers.txt
task=1c threads=64 time=2640ms peak=134MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 1c 64 numbers.txt
task=1c threads=64 time=2428ms peak=133MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ 
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 2 1 .
task=2 threads=1 time=398ms peak=394MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 2 2 .
task=2 threads=2 time=214ms peak=394MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 2 4 .
task=2 threads=4 time=121ms peak=394MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ 
[nnorian@nnorian concurrency]$ lsblk
NAME        MAJ:MIN RM   SIZE RO TYPE MOUNTPOINTS
sda           8:0    1  14.9G  0 disk 
└─sda1        8:1    1  14.9G  0 part /run/media/nnorian/ESD-USB
zram0       253:0    0     4G  0 disk [SWAP]
nvme0n1     259:0    0 953.9G  0 disk 
├─nvme0n1p1 259:1    0     1G  0 part /boot
└─nvme0n1p2 259:2    0 952.9G  0 part /
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 2 1 /run/media/$USER/ESD-USB/concurrency
task=2 threads=1 time=19562ms peak=394MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 2 2 /run/media/$USER/ESD-USB/concurrency
task=2 threads=2 time=20169ms peak=394MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ dotnet run -c Release -- 2 4 /run/media/$USER/ESD-USB/concurrency
task=2 threads=4 time=18969ms peak=394MB
neg=25005087 pos=24994889 zero=24 total=50000000
[nnorian@nnorian concurrency]$ 
```