﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using BEditor.Data;
using BEditor.Data.Primitive;
using BEditor.Data.Property;
using BEditor.Drawing;
using BEditor.Drawing.Pixel;
using BEditor.Primitive.Resources;

namespace BEditor.Primitive.Effects.OpenCv
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class GaussianBlur : ImageEffect
    {
        /// <summary>
        /// Defines the <see cref="Size"/> property.
        /// </summary>
        public static readonly DirectEditingProperty<GaussianBlur, EaseProperty> SizeProperty = EditingProperty.RegisterSerializeDirect<EaseProperty, GaussianBlur>(
            nameof(Size),
            owner => owner.Size,
            (owner, obj) => owner.Size = obj,
            new EasePropertyMetadata(Strings.Size, 20, Min: 0));

        /// <summary>
        /// Defines the <see cref="Resize"/> property.
        /// </summary>
        public static readonly DirectEditingProperty<GaussianBlur, CheckProperty> ResizeProperty = EditingProperty.RegisterSerializeDirect<CheckProperty, GaussianBlur>(
            nameof(Resize),
            owner => owner.Resize,
            (owner, obj) => owner.Resize = obj,
            new CheckPropertyMetadata(Strings.Resize, true));

        /// <summary>
        /// Initializes a new instance of the <see cref="GaussianBlur"/> class.
        /// </summary>
        public GaussianBlur()
        {
        }

        /// <inheritdoc/>
        public override string Name => Strings.GaussianBlur;

        /// <summary>
        /// Gets the size of the kernel.
        /// </summary>
        [AllowNull]
        public EaseProperty Size { get; private set; }

        /// <summary>
        /// Gets the value if the image should be resized.
        /// </summary>
        [AllowNull]
        public CheckProperty Resize { get; private set; }

        /// <inheritdoc/>
        public override void Apply(EffectApplyArgs<Image<BGRA32>> args)
        {
            var size = (int)Size[args.Frame];
            if (Resize.Value)
            {
                var image = args.Value.MakeBorder(args.Value.Width + size, args.Value.Height + size);
                args.Value.Dispose();
                args.Value = image;
            }

            Cv.GaussianBlur(args.Value, size);
        }

        /// <inheritdoc/>
        public override IEnumerable<PropertyElement> GetProperties()
        {
            yield return Size;
            yield return Resize;
        }
    }
}