﻿using System.Collections.Generic;
using System.Linq;

using BEditor.Data;
using BEditor.Data.Primitive;
using BEditor.Data.Property;
using BEditor.Data.Property.PrimitiveGroup;
using BEditor.Drawing;
using BEditor.Drawing.Pixel;
using BEditor.Graphics;
using BEditor.Primitive.Resources;

namespace BEditor.Primitive.Effects
{
    /// <summary>
    /// Represents a <see cref="ImageEffect"/> that provides the ability to edit multiple objects by specifying their indices.
    /// </summary>
    public sealed class MultipleControls : ImageEffect
    {
        /// <summary>
        /// Represents <see cref="Coordinate"/> metadata.
        /// </summary>
        public static readonly PropertyElementMetadata CoordinateMetadata = ImageObject.CoordinateMetadata;
        /// <summary>
        /// Represents <see cref="Zoom"/> metadata.
        /// </summary>
        public static readonly PropertyElementMetadata ZoomMetadata = ImageObject.ScaleMetadata;
        /// <summary>
        /// Represents <see cref="Angle"/> metadata.
        /// </summary>
        public static readonly PropertyElementMetadata AngleMetadata = ImageObject.RotateMetadata;
        /// <summary>
        /// Represents <see cref="Index"/> metadata.
        /// </summary>
        public static readonly ValuePropertyMetadata IndexMetadata = new("index", 0, Min: 0);

        /// <summary>
        /// INitializes a new instance of the <see cref="MultipleControls"/> class.
        /// </summary>
        public MultipleControls()
        {
            Coordinate = new(CoordinateMetadata);
            Zoom = new(ZoomMetadata);
            Angle = new(AngleMetadata);
            Index = new(IndexMetadata);
        }

        /// <inheritdoc/>
        public override string Name => Strings.MultipleImageControls;
        /// <inheritdoc/>
        public override IEnumerable<PropertyElement> Properties => new PropertyElement[]
        {
            Coordinate,
            Zoom,
            Angle,
            Index
        };
        /// <summary>
        /// Get the coordinates.
        /// </summary>
        [DataMember]
        public Coordinate Coordinate { get; private set; }
        /// <summary>
        /// Get the scale.
        /// </summary>
        [DataMember]
        public Scale Zoom { get; private set; }
        /// <summary>
        /// Get the angle.
        /// </summary>
        [DataMember]
        public Rotate Angle { get; private set; }
        /// <summary>
        /// Gets the <see cref="ValueProperty"/> representing the index of the image to be controlled.
        /// </summary>
        [DataMember]
        public ValueProperty Index { get; private set; }

        /// <inheritdoc/>
        public override void Render(EffectRenderArgs<IEnumerable<ImageInfo>> args)
        {
            args.Value = args.Value.Select((img, i) =>
            {
                var trans = img.Transform;

                if (i == (int)Index.Value)
                {
                    return new(
                        img.Source,
                        _ =>
                        {
                            var f = args.Frame;
                            var s = Zoom.Scale1[f] / 100;
                            var sx = Zoom.ScaleX[f] / 100 * s - 1;
                            var sy = Zoom.ScaleY[f] / 100 * s - 1;
                            var sz = Zoom.ScaleZ[f] / 100 * s - 1;

                            return img.Transform + Transform.Create(
                                new(Coordinate.X[f], Coordinate.Y[f], Coordinate.Z[f]),
                                new(Coordinate.CenterX[f], Coordinate.CenterY[f], Coordinate.CenterZ[f]),
                                new(Angle.RotateX[f], Angle.RotateY[f], Angle.RotateZ[f]),
                                new(sx, sy, sz));
                        });
                }

                return img;
            });
        }
        /// <inheritdoc/>
        public override void Render(EffectRenderArgs<Image<BGRA32>> args)
        {

        }
        /// <inheritdoc/>
        protected override void OnLoad()
        {
            Coordinate.Load(CoordinateMetadata);
            Zoom.Load(ZoomMetadata);
            Angle.Load(AngleMetadata);
            Index.Load(IndexMetadata);
        }
        /// <inheritdoc/>
        protected override void OnUnload()
        {
            Coordinate.Unload();
            Zoom.Unload();
            Angle.Unload();
            Index.Unload();
        }
    }
}
