#if UNITY_5_3_OR_NEWER
using System;
using System.Threading.Tasks;
using DamnLibrary.Behaviours;

namespace DamnLibrary.Managements.Windows.Animations
{
	public abstract class WindowAnimator : DamnBehaviour
	{
		internal Action Internal_CloseWindow { get; set; } 
		
		public abstract Task PlayOpenAnimationAsync();
		public abstract Task PlayShowAnimationAsync();
		public abstract Task PlayHideAnimationAsync();
		public abstract Task PlayCloseAnimationAsync();

		public void CloseWindow() => Internal_CloseWindow?.Invoke();
	}
}
#endif