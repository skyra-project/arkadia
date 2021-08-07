<div align="center">

![Arkadia Logo](https://cdn.skyra.pw/gh-assets/arkadia-logo.png)

# Arkadia

**C# Microservices for [Skyra]**

[![GitHub](https://img.shields.io/github/license/skyra-project/arkadia)](https://github.com/skyra-project/arkadia/blob/main/LICENSE.md)
[![codecov](https://codecov.io/gh/skyra-project/arkadia/branch/main/graph/badge.svg?token=DYhRX6ailX)](https://codecov.io/gh/skyra-project/arkadia)

[![Support Server](https://discord.com/api/guilds/254360814063058944/embed.png?style=banner2)][support server]

</div>

---

**Table of Contents**

-   [Arkadia](#arkadia)
    -   [Description](#description)
    -   [Published Docker Images](#published-docker-images)
        -   [arkadia.notifications](#arkadianotifications)
            -   [Quick reference](#quick-reference)
            -   [Supported tags and respective `Dockerfile` links](#supported-tags-and-respective-dockerfile-links)
            -   [What is arkadia.notifications](#what-is-arkadianotifications)
            -   [Environment Variables](#environment-variables)
        -   [arkadia.cdn](#arkadiacdn)
            -   [Quick reference](#quick-reference-1)
            -   [Supported tags and respective `Dockerfile` links](#supported-tags-and-respective-dockerfile-links-1)
            -   [What is arkadia.notifications](#what-is-arkadiacdn)
            -   [Environment Variables](#environment-variables-1)
    -   [Buy us some doughnuts](#buy-us-some-doughnuts)

## Description

Arkadia is the aptly named arcade of micro services that are being used for [Skyra] to instrument certain needs - such as notification streams, critical services (like moderation) and other features (such as social services or CDN's) with zero downtime and maximum reliability. Built atop the [dotnet] stack, using the latest features and runtime, and cross communicating with the main bot process via [gRPC], they are critical and useful components of Skyra's infrastructure.

## Published Docker Images

### [`arkadia.notifications`]

#### Quick reference

-   **Maintained by**:  
    [Skyra Project](https://github.com/skyra-project)

-   **Where to get help**:  
    [the Skyra Lounge server](https://join.skyra.pw)

-   **Where to file issues**:  
    [https://github.com/skyra-project/arkadia/issues](https://github.com/skyra-project/arkadia/issues)

#### Supported tags and respective `Dockerfile` links

-   [`latest`](https://github.com/skyra-project/arkadia/blob/main/Notifications.Dockerfile)

#### What is [`arkadia.notifications`]

Arkadia.Notifications is a microservice based around Google's PubSubHubBub API for Youtube. Interacted with via gRPC, it sends requests to the hub asking for notifications to be recieved over a HTTP connection.
The hub then sends an authentication request, to ensure we wanted to subscribe or unsubscribe, and it pumps upload notifications in. See the README in the root folder for more information.

#### Environment Variables

**Required Environment Variables**

-   `HTTP_PORT`  
     The port for the http server to run on.  
     _For Example:_ `9009`
-   `GRPC_PORT`  
     The port for the grpc server to run on.  
     _For example:_ `9010`
-   `CALLBACK_URL`  
     The url passed to google's pubsubhubbub to send notifications to.  
     _For Example:_ `https://notifications.skyra.pw`
-   `YOUTUBE_API_KEY`  
     The api key for googles data api.  
     _For example:_ `4MRhPzudqh4n9UP68y9c6xsh_Nk3TeG6Kf_3*oFAn2jpF@nffN`

**Optional Environment Variables**

-   `RESUB_TIMER_INTERVAL`  
    The amount of seconds we should wait between checking for resubscriptions  
    _default:_ `60`
-   `PUBSUB_URL`  
    The url for the PubSubHubBub API  
    _default:_ `https://pubsubhubbub.appspot.com/`
-   `POSTGRES_USER`  
    The user for the Postgres database  
    _default:_ `postgres`
-   `POSTGRES_PASSWORD`  
    The password for the database  
    _default:_ `postgres`
-   `POSTGRES_HOST`  
    The host url 
    _default:_ `localhost`
-   `POSTGRES_PORT`  
    The port used to connect to Postgres  
    _default:_ `5432`
-   `POSTGRES_DB`  
    The database to use in Postgres  
    _default:_ `arkadia`
-   `SENTRY_URL`  
    The DSN url for Sentry (null means no Sentry reporting)  
    _default:_ `null`
	

### [`arkadia.cdn`]

#### Quick reference

-   **Maintained by**:  
    [Skyra Project](https://github.com/skyra-project)

-   **Where to get help**:  
    [the Skyra Lounge server](https://join.skyra.pw)

-   **Where to file issues**:  
    [https://github.com/skyra-project/arkadia/issues](https://github.com/skyra-project/arkadia/issues)

#### Supported tags and respective `Dockerfile` links

-   [`latest`](https://github.com/skyra-project/arkadia/blob/main/Cdn.Dockerfile)

#### What is [`arkadia.cdn`]

Arkadia.Cdn is a CDN that conforms to the RFC 7232 spec, internally interacted with via gRPC.

#### Environment Variables

**Required Environment Variables**

-   `HTTP_PORT`  
     The port for the http server to run on.  
     _For Example:_ `9009`
-   `GRPC_PORT`  
     The port for the grpc server to run on.  
     _For example:_ `9010`
-   `BASE_ASSET_LOCATION`  
     The location for assets to be stored at. Must exist before app start.  
     _For Example:_ `/assets`

**Optional Environment Variables**

-   `POSTGRES_USER`  
    The user for the Postgres database  
    _default:_ `postgres`
-   `POSTGRES_PASSWORD`  
    The password for the database  
    _default:_ `postgres`
-   `POSTGRES_HOST`  
    The host url 
    _default:_ `localhost`
-   `POSTGRES_PORT`  
    The port used to connect to Postgres  
    _default:_ `5432`
-   `POSTGRES_DB`  
    The database to use in Postgres  
    _default:_ `arkadia`
-   `SENTRY_URL`  
    The DSN url for Sentry (null means no Sentry reporting)  
    _default:_ `null`

---

## Buy us some doughnuts

Skyra Project is open source and always will be, even if we don't get donations. That said, we know there are amazing people who
may still want to donate just to show their appreciation. Thanks you very much in advance!

We accept donations through Patreon, BitCoin, Ethereum, and Litecoin. You can use the buttons below to donate through your method of choice.

| Donate With |         QR         |                        Address                         |
| :---------: | :----------------: | :----------------------------------------------------: |
|   Patreon   | ![PatreonImage][]  |                 [Click Here][patreon]                  |
|   PayPal    |  ![PayPalImage][]  |                  [Click Here][paypal]                  |
|   BitCoin   | ![BitcoinImage][]  |     [3JNzCHMTFtxYFWBnVtDM9Tt34zFbKvdwco][bitcoin]      |
|  Ethereum   | ![EthereumImage][] | [0xcB5EDB76Bc9E389514F905D9680589004C00190c][ethereum] |
|  Litecoin   | ![LitecoinImage][] |     [MNVT1keYGMfGp7vWmcYjCS8ntU8LNvjnqM][litecoin]     |

[bitcoin]: bitcoin:3JNzCHMTFtxYFWBnVtDM9Tt34zFbKvdwco?amount=0.01&label=Skyra%20Discord%20Bot
[bitcoinimage]: https://cdn.skyra.pw/gh-assets/bitcoin.png
[ethereum]: ethereum:0xcB5EDB76Bc9E389514F905D9680589004C00190c?amount=0.01&label=Skyra%20Discord%20Bot
[ethereumimage]: https://cdn.skyra.pw/gh-assets/ethereum.png
[litecoin]: litecoin:MNVT1keYGMfGp7vWmcYjCS8ntU8LNvjnqM?amount=0.01&label=Skyra%20Discord%20Bot
[litecoinimage]: https://cdn.skyra.pw/gh-assets/litecoin.png
[patreon]: https://donate.skyra.pw/patreon
[patreonimage]: https://cdn.skyra.pw/gh-assets/patreon.png
[paypal]: https://donate.skyra.pw/paypal
[paypalimage]: https://cdn.skyra.pw/gh-assets/paypal.png
[skyra]: https://github.com/skyra-project/skyra
[support server]: https://join.skyra.pw
[dotnet]: https://dotnet.microsoft.com
[grpc]: https://grpc.io
[`arkadia.notifications`]: https://github.com/skyra-project/docker-images/pkgs/container/arkadia.notifications
[`arkadia.cdn`]: https://github.com/skyra-project/docker-images/pkgs/container/arkadia.cdn
