#if ENABLE_SERIALIZATION && ENABLE_NETWORKING
using System.Threading.Tasks;
using DamnLibrary.Debugging;
using DamnLibrary.Extensions;
using DamnLibrary.Networking.Client;

namespace DamnLibrary.Networking.Handlers
{
    public abstract class BaseNetworkHandler
    {
        public bool IsAvailable { get; protected set; }
        
        public bool IsReconnecting { get; protected set; }

        public abstract void Handle();

        protected abstract Task OnHandleAsync();

        protected bool ValidateReadPacket(int bytesRead)
        {
            if (bytesRead != 0) 
                return true;

            if (IsAvailable && !IsReconnecting)
            {
                IsAvailable = false;
                IsReconnecting = true;
                UniversalDebugger.LogError($"[{nameof(ClientNetworkHandler)}] ({nameof(ValidateReadPacket)}) Read empty response. Trying reconnecting...");
            }
            return false;
        }
    } 
    
    public abstract class ClientNetworkHandler : BaseNetworkHandler
    {
        public abstract bool IsConnected { get; }
        
        public abstract bool IsPaused { get; set; }
        
        public override async void Handle()
        {
            while (IsConnected)
            {
                if (!IsPaused)
                {
                    await OnHandleAsync();
                }

                await TaskUtilities.Delay(DamnNetworking.HandleDelay);
            }
        }
    }
    
    public abstract class ServerNetworkHandler : BaseNetworkHandler
    {
        public abstract bool IsWorking { get; set; }
        
        public abstract bool IsPaused { get; set; }
        
        public override async void Handle()
        {
            while (IsWorking)
            {
                if (!IsPaused)
                {
                    await OnHandleAsync();
                }

                await TaskUtilities.Delay(DamnNetworking.HandleDelay);
            }
        }
    }
}
#endif