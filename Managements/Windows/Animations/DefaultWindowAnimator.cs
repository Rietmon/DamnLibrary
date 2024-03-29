#if ENABLE_DOTWEEN
using System.Threading.Tasks;
using DamnLibrary.Types;
using DamnLibrary.Utilities.Extensions;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace DamnLibrary.Managements.Windows.Animations
{
	public class DefaultWindowAnimator : WindowAnimator
	{
		private const float AnimationDuration = 0.15f;

		private static readonly Vector3Range horizontalContentScaleValues = new(new Vector3(0.6f, 1), new Vector3(1, 1));
		private static readonly Vector3Range verticalContentScaleValues = new(new Vector3(1, 0.3f), new Vector3(1, 1));
		private static readonly FloatRange contentFadeValues = new(0, 1);

		[SerializeField] private bool isHorizontalScale = true;
		[SerializeField] private Image faderImage;
		[SerializeField] private Color faderColor = new(0, 0, 0, 0.75f);
		[SerializeField] private CanvasGroup contentCanvasGroup;

		public override async Task PlayOpenAnimationAsync() =>
			await Internal_PlayOpenCloseAnimation(true);

		public override async Task PlayShowAnimationAsync() =>
			await Internal_PlayShowHideAnimation(true);

		public override async Task PlayHideAnimationAsync() =>
			await Internal_PlayShowHideAnimation(false);

		public override async Task PlayCloseAnimationAsync() =>
			await Internal_PlayOpenCloseAnimation(false);

		private async Task Internal_PlayOpenCloseAnimation(bool isAppear)
		{
			if (faderImage)
			{
				faderImage.color = isAppear ? faderColor.A(0) : faderColor;
				faderImage.DOColor(isAppear ? faderColor : faderColor.A(0), AnimationDuration);
			}

			var firstIndex = isAppear ? 0 : 1;
			var secondIndex = isAppear ? 1 : 0;

			var contentScaleValues = isHorizontalScale ? horizontalContentScaleValues : verticalContentScaleValues;
			contentCanvasGroup.transform.localScale = contentScaleValues[firstIndex];
			contentCanvasGroup.transform.DOScale(contentScaleValues[secondIndex], AnimationDuration);

			contentCanvasGroup.alpha = contentFadeValues[firstIndex];
			await contentCanvasGroup.DOFade(contentFadeValues[secondIndex], AnimationDuration).AsyncWaitForCompletion();
		}

		private async Task Internal_PlayShowHideAnimation(bool isAppear)
		{
			if (faderImage)
			{
				faderImage.color = isAppear ? faderColor.A(0) : faderColor;
				faderImage.DOColor(faderColor, AnimationDuration);
			}

			var firstIndex = isAppear ? 0 : 1;
			var secondIndex = isAppear ? 1 : 0;

			contentCanvasGroup.alpha = contentFadeValues[firstIndex];
			await contentCanvasGroup.DOFade(contentFadeValues[secondIndex], AnimationDuration).AsyncWaitForCompletion();
		}
	}
}
#endif