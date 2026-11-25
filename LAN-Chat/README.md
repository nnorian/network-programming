# LAN Chat — Laboratory Work 3

A peer-to-peer chat for the local network over UDP, using the `Socket` class
directly (no `UdpClient`). There is no server: every running copy is a peer.

## Requirements

- .NET 10 SDK (with .NET 8 or 9, change `net10.0` in `LanChat.csproj`).

## Build

From the `LAN-Chat` folder:

    dotnet build

## Run

    dotnet run -- <name> <port> <passphrase>

All peers of one chat use the same port and the same passphrase. Example, three
terminals (or three computers in one network):

    dotnet run -- Ana 9000 our-secret
    dotnet run -- Ion 9000 our-secret
    dotnet run -- Maria 9000 our-secret

A name has 1–20 letters, digits, `_` or `-`. If broadcasts do not pass on your
Wi-Fi, use a phone hotspot.

## Commands

| Command          | What it does                        |
|------------------|-------------------------------------|
| `text`           | sends text to everybody             |
| `/msg name text` | sends text to name only (encrypted) |
| `/users`         | shows who is online                 |
| `/quit`          | leaves the chat                     |

## Log

Each peer writes `chat-<name>-<pid>.log`, one line per datagram:

    17:29:20.865 SENT 192.168.1.5:60922 join_reply 58 bytes
    17:29:25.816 DROP 192.168.1.9:37855 not json 50 bytes

## Protocol

Every datagram is one UTF-8 JSON object, at most 4096 bytes. Fields that are
not used are left out.

| Field        | Meaning                                                    |
|--------------|------------------------------------------------------------|
| `type`       | `msg`, `pm`, `join`, `join_reply`, `name_taken`, `heartbeat`, `leave` |
| `from`       | name of the sender (1–20 of `A-Z a-z 0-9 _ -`)             |
| `id`         | random hex id of the running copy (detects own echo and duplicate names) |
| `text`       | `msg` only: the text, at most 500 characters               |
| `nonce`      | `pm` only: base64 of the 12-byte AES-GCM nonce             |
| `ciphertext` | `pm` only: base64 of the ciphertext followed by the 16-byte tag |

Sent to everybody (broadcast to the chat port): `msg`, `join`, `heartbeat`, `leave`.
Sent to one peer (to the source endpoint of its datagrams): `pm`, `join_reply`, `name_taken`.

Examples:

    {"type":"msg","from":"Ana","id":"9F2C61D04A7B3E85","text":"hello everyone"}
    {"type":"join","from":"Maria","id":"03B7E1A9C2F45D68"}

Presence: a new peer broadcasts `join` and waits 1 s. Every peer answers
`join_reply`, or `name_taken` if that name already belongs to another id.
Every peer broadcasts `heartbeat` every 5 s; a peer silent for 15 s is gone;
`/quit` broadcasts `leave`.

Encryption of `pm`: key = PBKDF2-HMAC-SHA256(passphrase, salt `"LAN-Chat salt v1"`,
600 000 iterations, 32 bytes). Cipher: AES-256-GCM, random 12-byte nonce per
message, associated data = UTF-8 of `"<from>><to>"` (e.g. `Ana>Ion`).
