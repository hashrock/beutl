﻿using System;
using System.Collections.Generic;
using System.Linq;

using BEditor.Data;
using BEditor.Data.Primitive;
using BEditor.Data.Property;
using BEditor.Drawing;
using BEditor.Drawing.Pixel;
using BEditor.Primitive.Resources;

namespace BEditor.Primitive.Effects
{
#pragma warning disable CS1591
    public sealed class Sepia : ImageEffect
    {
        public Sepia()
        {
        }

        public override string Name => Strings.Sepia;

        public override void Apply(EffectApplyArgs<Image<BGRA32>> args)
        {
            args.Value.Sepia(Parent.Parent.DrawingContext);
        }

        public override IEnumerable<PropertyElement> GetProperties()
        {
            return Enumerable.Empty<PropertyElement>();
        }
    }
#pragma warning restore CS1591
}