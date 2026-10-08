```
[nnorian@nnorian concurrency]$ dotnet --version
10.0.112
[nnorian@nnorian concurrency]$ lscpu | grep -E "Model name|Core|Thread|Socket"
Model name:                              AMD Ryzen AI 7 350 w/ Radeon 860M
Thread(s) per core:                      2
Core(s) per socket:                      8
Socket(s):                               1
[nnorian@nnorian concurrency]$ free -h
               total        used        free      shared  buff/cache   available
Mem:            30Gi       7.5Gi        19Gi       229Mi       4.5Gi        22Gi
Swap:          4.0Gi          0B       4.0Gi
[nnorian@nnorian concurrency]$ lsblk -d -o NAME,MODEL,TRAN,ROTA,SIZE
NAME    MODEL                   TRAN   ROTA   SIZE
zram0                                     0     4G
nvme0n1 MTFDKBA1T0QGN-1BN1AABGA nvme      0 953.9G
[nnorian@nnorian concurrency]$ uname -r; go version
7.2.7-arch1-1
go version go1.27.1-X:nodwarf5 linux/amd64
[nnorian@nnorian concurrency]$ 

➜  concurrency git:(main) dotnet --version
10.0.112
➜  concurrency git:(main) lscpu | grep -E "Model name|Core|Thread|Socket"
Model name:                              AMD Ryzen 5 7535HS with Radeon Graphics
Thread(s) per core:                      2
Core(s) per socket:                      6
Socket(s):                               1
➜  concurrency git:(main) free -h
               total        used        free      shared  buff/cache   available
Mem:            12Gi       7.5Gi       1.3Gi       572Mi       4.8Gi       5.2Gi
Swap:          4.0Gi       478Mi       3.5Gi
➜  concurrency git:(main) lsblk -d -o NAME,MODEL,TRAN,ROTA,SIZE
NAME    MODEL                   TRAN   ROTA   SIZE
zram0                                     0     4G
nvme0n1 SKHynix_HFS512GEJ4X112N nvme      0 476.9G
nvme1n1 TS1TMTE115S             nvme      0 931.5G
➜  concurrency git:(main) uname -r
7.2.9-1-cachyos
➜  concurrency git:(main) 
```