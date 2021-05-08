﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using BEditor.Data;
using BEditor.Data.Primitive;
using BEditor.Data.Property;
using BEditor.Drawing;
using BEditor.Drawing.Pixel;
using BEditor.Primitive.Resources;

namespace BEditor.Primitive.Objects
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Framebuffer : ImageObject
    {
        /// <summary>
        /// Defines the <see cref="BufferClear"/> property.
        /// </summary>
        public static readonly DirectEditingProperty<Framebuffer, CheckProperty> BufferClearProperty = EditingProperty.RegisterSerializeDirect<CheckProperty, Framebuffer>(
            nameof(BufferClear),
            owner => owner.BufferClear,
            (owner, obj) => owner.BufferClear = obj,
            new CheckPropertyMetadata(Strings.ClearFramebuffer));

        /// <summary>
        /// Initializes a new instance of the <see cref="Framebuffer"/> class.
        /// </summary>
        public Framebuffer()
        {
        }

        /// <inheritdoc/>
        public override string Name => Strings.Framebuffer;

        /// <summary>
        /// Gets the <see cref="CheckProperty"/> representing the value whether to clear the frame buffer.
        /// </summary>
        [AllowNull]
        public CheckProperty BufferClear { get; private set; }

        /// <inheritdoc/>
        public override IEnumerable<PropertyElement> GetProperties()
        {
            yield return Coordinate;
            yield return Scale;
            yield return Blend;
            yield return Rotate;
            yield return Material;
            yield return BufferClear;
        }

        /// <inheritdoc/>
        protected override Image<BGRA32>? OnRender(EffectApplyArgs args)
        {
            var scene = Parent.Parent;
            var image = new Image<BGRA32>(scene.Width, scene.Height);

            scene.GraphicsContext?.ReadImage(image);

            if (BufferClear.Value)
            {
                scene.GraphicsContext?.Clear();
            }

            return image;
        }
    }
}