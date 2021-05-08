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
    public sealed class Negaposi : ImageEffect
    {
        /// <summary>
        /// Defines the <see cref="Red"/> property.
        /// </summary>
        public static readonly DirectEditingProperty<Negaposi, EaseProperty> RedProperty = EditingProperty.RegisterSerializeDirect<EaseProperty, Negaposi>(
            nameof(Red),
            owner => owner.Red,
            (owner, obj) => owner.Red = obj,
            new EasePropertyMetadata(Strings.Red, 255, 255, 0));

        /// <summary>
        /// Defines the <see cref="Green"/> property.
        /// </summary>
        public static readonly DirectEditingProperty<Negaposi, EaseProperty> GreenProperty = EditingProperty.RegisterSerializeDirect<EaseProperty, Negaposi>(
            nameof(Green),
            owner => owner.Green,
            (owner, obj) => owner.Green = obj,
            new EasePropertyMetadata(Strings.Green, 255, 255, 0));

        /// <summary>
        /// Defines the <see cref="Blue"/> property.
        /// </summary>
        public static readonly DirectEditingProperty<Negaposi, EaseProperty> BlueProperty = EditingProperty.RegisterSerializeDirect<EaseProperty, Negaposi>(
            nameof(Blue),
            owner => owner.Blue,
            (owner, obj) => owner.Blue = obj,
            new EasePropertyMetadata(Strings.Blue, 255, 255, 0));

        /// <summary>
        /// Initializes a new instance of the <see cref="Negaposi"/> class.
        /// </summary>
        public Negaposi()
        {
        }

        /// <inheritdoc/>
        public override string Name => Strings.Negaposi;

        /// <summary>
        /// Gets the red.
        /// </summary>
        [AllowNull]
        public EaseProperty Red { get; private set; }

        /// <summary>
        /// Gets the green.
        /// </summary>
        [AllowNull]
        public EaseProperty Green { get; private set; }

        /// <summary>
        /// Gets the blue.
        /// </summary>
        [AllowNull]
        public EaseProperty Blue { get; private set; }

        /// <inheritdoc/>
        public override void Apply(EffectApplyArgs<Image<BGRA32>> args)
        {
            args.Value.Negaposi(
                (byte)Red[args.Frame],
                (byte)Green[args.Frame],
                (byte)Blue[args.Frame],
                Parent.Parent.DrawingContext);
        }

        /// <inheritdoc/>
        public override IEnumerable<PropertyElement> GetProperties()
        {
            yield return Red;
            yield return Green;
            yield return Blue;
        }
    }
}