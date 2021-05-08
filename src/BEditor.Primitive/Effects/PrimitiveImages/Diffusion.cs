﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using BEditor.Data;
using BEditor.Data.Primitive;
using BEditor.Data.Property;
using BEditor.Drawing;
using BEditor.Drawing.Pixel;
using BEditor.Primitive.Resources;

namespace BEditor.Primitive.Effects
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Diffusion : ImageEffect
    {
        /// <summary>
        /// Defines the <see cref="Value"/> property.
        /// </summary>
        public static readonly DirectEditingProperty<Diffusion, EaseProperty> ValueProperty = EditingProperty.RegisterSerializeDirect<EaseProperty, Diffusion>(
            nameof(Value),
            owner => owner.Value,
            (owner, obj) => owner.Value = obj,
            new EasePropertyMetadata(Strings.ThresholdValue, 7, 30, 0));

        /// <summary>
        /// Initializes a new instance of the <see cref="Diffusion"/> class.
        /// </summary>
        public Diffusion()
        {
        }

        /// <inheritdoc/>
        public override string Name => Strings.Diffusion;

        /// <summary>
        /// Gets the threshold value.
        /// </summary>
        [AllowNull]
        public EaseProperty Value { get; set; }

        /// <inheritdoc/>
        public override void Apply(EffectApplyArgs<Image<BGRA32>> args)
        {
            args.Value.Diffusion((byte)Value[args.Frame]);
        }

        /// <inheritdoc/>
        public override IEnumerable<PropertyElement> GetProperties()
        {
            yield return Value;
        }
    }
}