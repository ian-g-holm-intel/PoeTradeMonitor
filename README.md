# PoeTradeMonitor
This is my primary hobby programming project. It is a collection of tools and services for the game Path of Exile. 
It enables setting up live searches for ingame items, automatically sending ingame whispers to buy the item, and scheduling an automated trade to purchase the item. This project has been iterative over 13 years and has facilitated a lot of my programming learning and integration of new technologies.

## Skills and technologies utilized
- C# .Net 9
- WPF Framework (UI)
- gRPC.Net (RPC communication between frontend and backend service)
- AutoIt (Keyboard/Mouse input simulation)
- SpeedTest.Net Wrapper (Deprecated, formerly used to identify fastest proxies when webproxies were supported)
- Stateless (State machine library for trade automation state management)
- Serilog.Net (High performance logging library)
- Nito.AsyncEx (Task Parallel Library extension library)
- PushoverNet (Push notification client library)
- RateLimiter (Rate limit library)
- AlphaVSS (Wrapper library for windows Volume Shadow Copy Service to enable copying of in-use locked files, necessary for reading browser cookie file while browser is open)

## Excluded Dependencies
This repository depends on a private git submodule that is not included. This module includes the library necessary for reading and accessing Path of Exile game memory.
