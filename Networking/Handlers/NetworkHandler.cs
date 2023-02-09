#if ENABLE_SERIALIZATION && ENABLE_NETWORKING
using System.Threading.Tasks;
using DamnLibrary.Debugging;
using DamnLibrary.Extensions;
using DamnLibrary.Networking.Client;

namespace DamnLibrary.Networking.Handlers
{
    public abstract class BaseNetworkHandler
    {
        public abstract void Handle();

        protected abstract Task OnHandleAsync();
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

                await TaskUtilities.Delay(DamnNetworking.ClientHandleDelay);
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

                await TaskUtilities.Delay(DamnNetworking.ServerHandleDelay);
            }
        }
    }
}
#endif