using Cysharp.Threading.Tasks;

namespace Rietmon.Extensions
{
    public static class UniTaskExtensions
    {
        public static void Wait(this UniTask task) => task.AsTask().Wait();
    }
}