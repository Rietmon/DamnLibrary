#if UNITASK_DOTWEEN_SUPPORT
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace DamnLibrary.Utilities.Extensions
{
    public static class UniTaskExtensions
    {
        public static void Forget(this Tween tween) => tween.GetAwaiter();
    }
}
#endif