﻿using Beutl.Graphics.Rendering;
using Beutl.Graphics.Transformation;
using Beutl.Media;
using Beutl.Utilities;
using SkiaSharp;

namespace Beutl.Graphics.Effects;

public class DisplacementMapEffect : FilterEffect
{
    public static readonly CoreProperty<IBrush?> DisplacementMapProperty;
    public static readonly CoreProperty<DisplacementMapTransform?> TransformProperty;
    public static readonly CoreProperty<bool> ShowDisplacementMapProperty;
    private static readonly SKRuntimeEffect? s_runtimeEffect;
    private IBrush? _displacementMap;
    private DisplacementMapTransform? _transform;
    private bool _showDisplacementMap;

    static DisplacementMapEffect()
    {
        DisplacementMapProperty = ConfigureProperty<IBrush?, DisplacementMapEffect>(nameof(DisplacementMap))
            .Accessor(o => o.DisplacementMap, (o, v) => o.DisplacementMap = v)
            .Register();

        TransformProperty = ConfigureProperty<DisplacementMapTransform?, DisplacementMapEffect>(nameof(Transform))
            .Accessor(o => o.Transform, (o, v) => o.Transform = v)
            .Register();

        ShowDisplacementMapProperty =
            ConfigureProperty<bool, DisplacementMapEffect>(nameof(ShowDisplacementMap))
                .Accessor(o => o.ShowDisplacementMap, (o, v) => o.ShowDisplacementMap = v)
                .Register();

        AffectsRender<DisplacementMapEffect>(
            DisplacementMapProperty,
            TransformProperty,
            ShowDisplacementMapProperty);
    }

    public IBrush? DisplacementMap
    {
        get => _displacementMap;
        set => SetAndRaise(DisplacementMapProperty, ref _displacementMap, value);
    }

    public DisplacementMapTransform? Transform
    {
        get => _transform;
        set => SetAndRaise(TransformProperty, ref _transform, value);
    }

    public bool ShowDisplacementMap
    {
        get => _showDisplacementMap;
        set => SetAndRaise(ShowDisplacementMapProperty, ref _showDisplacementMap, value);
    }

    public override void ApplyTo(FilterEffectContext context)
    {
        if (DisplacementMap is null) return;
        var displacementMap_ = (DisplacementMap as IMutableBrush)?.ToImmutable() ?? DisplacementMap;

        if (ShowDisplacementMap)
        {
            context.CustomEffect(displacementMap_,
                (d, c) =>
                {
                    for (int i = 0; i < c.Targets.Count; i++)
                    {
                        EffectTarget effectTarget = c.Targets[i];
                        using var displacementMapShader =
                            new BrushConstructor(new Rect(effectTarget.Bounds.Size), d, BlendMode.SrcOver)
                                .CreateShader();

                        using (var paint = new SKPaint())
                        {
                            var newTarget = c.CreateTarget(effectTarget.Bounds);
                            var canvas = newTarget.RenderTarget!.Value.Canvas;
                            paint.Shader = displacementMapShader;
                            canvas.DrawRect(new SKRect(0, 0, effectTarget.Bounds.Width, effectTarget.Bounds.Height),
                                paint);

                            c.Targets[i] = newTarget;
                        }

                        effectTarget.Dispose();
                    }
                });
        }
        else if (Transform != null)
        {
            Transform.ApplyTo(displacementMap_, context);
        }
    }
}
