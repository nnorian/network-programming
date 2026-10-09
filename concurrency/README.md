# Lab 1 — Concurrency measurements (Machine 2 / laptop)

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
