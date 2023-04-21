using System.Threading.Tasks;
using DamnLibrary.Behaviours;

namespace DamnLibrary.Management.Animations
{
	public abstract class WindowAnimator : DamnBehaviour
	{
		public abstract Task PlayOpenAnimationAsync();
		public abstract Task PlayShowAnimationAsync();
		public abstract Task PlayHideAnimationAsync();
		public abstract Task PlayCloseAnimationAsync();
	}
}