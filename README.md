# DamnLibrary
Game core library for .NET/Unity. There are different features for Unity and .NET. It will not throw an error, but some API will be not available. For example, in .NET will not compiling any components and extensions for Unity. Be careful!
## API
After installing will be available only basic methods. For some special features you would to define Preprocessor defines in your project.

`ENABLE_DAMN_SCRIPT` -- Enable DamnScript API
`ENABLE_DOTWEEN` -- Enable DOTween extensions  
`ENABLE_SERIALIZATIONS` -- Enable bytes serialization  
`ENABLE_ADDRESSABLE` -- Enable features for Unity Addressable  
`ENABLE_LOCALIZATION` -- Enable features for Unity Localization (work only with `ENABLE_ADDRESSABLE`)  
`ENABLE_MEMORY_UTILITIES` -- Enable features for unsafe code  
`ENABLE_WINDOWS_UTILITIES` -- Enable features from native Windows libraries  
`ENABLE_NETWORKING` -- Enable features for networking. For now only for TCP (work only with `ENABLE_SERIALIZATIONS`). Documentation will be soon... :)  

`DEBUG_ENABLE_WATCHING_MEMORY_ALLOCATING` -- Enable collecting debug information of `MemoryUtilities.Allocate(...)`  
`DEBUG_ENABLE_LOGS_NETWORKING` -- Enable logs of networking, include size, type of sent and received packets, connection status and more  

## Special thanks
- TinyJSON
