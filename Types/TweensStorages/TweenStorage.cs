using System.Collections.Generic;
using DG.Tweening;

namespace DamnLibrary.Types
{
    public class TweenStorage : List<Tween>
    {
        public void KillAndClear()
        {
            foreach (var tween in this)
                tween.Kill();
            Clear();
        }
    }
}