using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("Layout/Extended Content Size Fitter", 142)]
[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class ExtendedContentSizeFilter : UIBehaviour, ILayoutSelfController
{
    public FitMode HorizontalFit => horizontalFit;
    public FitMode VerticalFit => verticalFit;

    private RectTransform RectTransform
    {
        get
        {
            if (rect == null)
                rect = GetComponent<RectTransform>();
            return rect;
        }
    }

    [SerializeField] protected FitMode horizontalFit = FitMode.Unconstrained;

    [SerializeField] protected FitMode verticalFit = FitMode.Unconstrained;

    [SerializeField] private float minimalHeight;

    [SerializeField] private float minimalWidth;

    [NonSerialized] private RectTransform rect;

#pragma warning disable 649
    private DrivenRectTransformTracker tracker;
#pragma warning restore 649

    protected override void OnEnable()
    {
        base.OnEnable();
        SetDirty();
    }

    protected override void OnDisable()
    {
        tracker.Clear();
        LayoutRebuilder.MarkLayoutForRebuild(RectTransform);
        base.OnDisable();
    }

    protected override void OnRectTransformDimensionsChange()
    {
        SetDirty();
    }

    private void HandleSelfFittingAlongAxis(int axis)
    {
        FitMode fitting = (axis == 0 ? HorizontalFit : VerticalFit);
        if (fitting == FitMode.Unconstrained)
        {
            tracker.Add(this, RectTransform, DrivenTransformProperties.None);
            return;
        }

        tracker.Add(this, RectTransform,
            (axis == 0 ? DrivenTransformProperties.SizeDeltaX : DrivenTransformProperties.SizeDeltaY));

        if (fitting == FitMode.MinSize)
        {
            var size = LayoutUtility.GetMinSize(rect, axis);
            var minimalSize = axis == 0 ? minimalWidth : minimalHeight;
            RectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis,
                size < minimalSize && minimalSize != -1 ? minimalSize : size);
        }
        else
        {
            var size = LayoutUtility.GetPreferredSize(rect, axis);
            var minimalSize = axis == 0 ? minimalWidth : minimalHeight;
            
            RectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis,
                size < minimalSize && minimalSize != -1 ? minimalSize : size);
        }
    }

    public virtual void SetLayoutHorizontal()
    {
        tracker.Clear();
        HandleSelfFittingAlongAxis(0);
    }

    public virtual void SetLayoutVertical()
    {
        HandleSelfFittingAlongAxis(1);
    }

    protected void SetDirty()
    {
        if (!IsActive())
            return;

        LayoutRebuilder.MarkLayoutForRebuild(RectTransform);
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        SetDirty();
    }
#endif

    protected override void Reset()
    {
        base.Reset();

        minimalWidth = -1;
        minimalHeight = -1;
    }

    public enum FitMode
    {
        Unconstrained,
        MinSize,
        PreferredSize
    }
}
