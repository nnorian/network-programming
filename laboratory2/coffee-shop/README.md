# Coffee Shop Server — Laboratory Work 2

A coffee shop server in C# that uses the `Socket` class directly
(no `TcpListener`, `TcpClient` or `NetworkStream`).

There are three roles:

- **Server**: keeps the stock, takes orders, prepares them with one worker per CPU core,
  and logs every connection, order, delivery and error.
- **Client**: asks for the menu and orders products.
- **Provider**: delivers new products and gets an alert when a product runs low.

The client and the provider are one program that starts in either role.

## Requirements

- .NET 10 SDK: https://dotnet.microsoft.com/download

  Check the installation with:

      dotnet --version

  With .NET 8 or 9, change `net10.0` to `net8.0` or `net9.0` in both `.csproj` files.

## Build

From the `coffee-shop` folder (the one with `CoffeeShop.slnx`):

    dotnet build

The build must end with `0 Error(s)`.

## Run

Open one terminal for the server and one terminal for each client or provider.
Run every command from the `coffee-shop` folder.

### 1. Start the server

    dotnet run --project CoffeeShopServer -- <port>

Example:

    dotnet run --project CoffeeShopServer -- 5050

Stop the server with `Ctrl+C`.

### 2. Start a client

    dotnet run --project CoffeeShopClient -- <address> <port>

Example:

    dotnet run --project CoffeeShopClient -- 127.0.0.1 5050

### 3. Start the provider

    dotnet run --project CoffeeShopClient -- <address> <port> provider

Example:

    dotnet run --project CoffeeShopClient -- 127.0.0.1 5050 provider

The role is the last word, `client` or `provider`. Type it without brackets.
Without a role, the program starts as a client. Only one provider can be connected at a time.

The address can be an IP address (`127.0.0.1`) or a host name (`localhost`).

### Two computers in the same network

1. Run the server on computer 1.
2. Find the IP address of computer 1: `ip addr` on Linux, `ipconfig` on Windows.
3. On computer 2, run:

       dotnet run --project CoffeeShopClient -- <IP of computer 1> 5050

If the client cannot connect (`TimedOut` or `ConnectionRefused`), allow the port in the
firewall of computer 1:

- Linux (ufw): `sudo ufw allow 5050/tcp`
- Linux (firewalld): `sudo firewall-cmd --add-port=5050/tcp`
- Windows: Windows Defender Firewall, Advanced settings, Inbound Rules,
  New Rule, Port, TCP 5050

## Commands

Type a command and press Enter. Lines that start with `<<` come from the server.
They are shown as soon as they arrive, even while you are typing. Type `quit` to exit.

| You type                         | The server answers                                          |
|----------------------------------|-------------------------------------------------------------|
| `MENU`                           | `MENU coffee=23 tea=40 croissant=12`                        |
| `ORDER coffee=2 tea=1`           | `OK ORDER 7 TIME 11`, later `READY 7`                       |
|                                  | or `REFUSED coffee LEFT 1` if the stock is too low          |
| `DELIVER coffee=20 croissant=10` | `OK DELIVERED coffee=20 croissant=10` (provider only)       |
| anything invalid                 | `ERROR <reason>`                                            |

The server also sends on its own:

- `READY <order>` to a client, when its order is prepared
- `ALERT <product> <left>` to the provider, when a product falls below 5.
  If no provider is connected, it gets the alert when it connects.

| Product   | Preparation time |
|-----------|------------------|
| coffee    | 4 s              |
| tea       | 3 s              |
| croissant | 2 s              |

The preparation time of an order is the sum of its items:
`ORDER croissant=2 tea=1 coffee=1` takes 2·2 + 3 + 4 = 11 seconds.

## The stock

On the first start the server puts a random amount from 10 to 50 of each product
into its stock. The stock is saved to `stock.txt` in the folder where the server
was started. On later starts it is loaded from that file. Delete `stock.txt` to
start again with a random stock.

## Protocol

- Text messages in UTF-8. Every message ends with `\n`. The maximum is 1024 bytes.
- The client sends `PING` every 3 seconds automatically. If the server hears nothing
  from a client for 10 seconds, it considers the client gone and cancels its orders.
- When a client disconnects, its waiting orders and the one being prepared are cancelled,
  and their products go back to the stock.

## Quick test

1. Start the server, two clients and the provider, as shown above.
2. Client: `MENU`. The stock is shown.
3. Client: `ORDER croissant=1`. You get `OK ORDER 1 TIME 2`, then `READY 1` 2 s later.
4. Client: `ORDER pizza=1`. You get `ERROR unknown product 'pizza'`.
5. Client: `DELIVER coffee=5`. You get `ERROR only the provider can deliver`.
6. Provider: `DELIVER coffee=20`. You get `OK DELIVERED coffee=20`.
7. Order more clients than the server has workers, all at the same time. The extra
   orders wait in the queue until a worker is free.
8. Close a client while its order is being prepared. The server log shows the order
   cancelled and its products returned to the stock.
9. Stop the server. Every client prints `Disconnected: ...` and ends.

To send random bytes on Linux:

    head -c 300 /dev/urandom | nc 127.0.0.1 5050

The server answers with an error and keeps serving the other clients.

## Project structure

```
coffee-shop/
├── CoffeeShop.slnx
├── CoffeeShopServer/
│   ├── Program.cs            reads the port, starts the server
│   ├── Server.cs             listening socket, accept loop, one thread per client
│   ├── ClientConnection.cs   one connection: message framing, commands, sending, cleanup
│   ├── Protocol.cs           parsing and checking "coffee=2 tea=1"
│   ├── IShop.cs              interface between the network part and the shop
│   ├── Shop.cs               stock, order queue, workers, provider alerts, saved stock
│   ├── Products.cs           the products and their preparation times
│   ├── OrderResult.cs        the answer of the shop to an order
│   └── Log.cs                one log line with time and endpoint
└── CoffeeShopClient/
    └── Program.cs            client and provider program
```
