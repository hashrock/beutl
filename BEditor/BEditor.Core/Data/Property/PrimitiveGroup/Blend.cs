﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using BEditor.Core.Command;
using BEditor.Core.Data.Property;
using BEditor.Core.Extensions;
using BEditor.Core.Properties;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace BEditor.Core.Data.Property.PrimitiveGroup
{
    [DataContract]
    public sealed class Blend : ExpandGroup
    {
        public static readonly EasePropertyMetadata AlphaMetadata = new(Resources.Alpha, 100, 100, 0);
        public static readonly ColorAnimationPropertyMetadata ColorMetadata = new(Resources.Color, Drawing.Color.Light, false);
        public static readonly SelectorPropertyMetadata BlendTypeMetadata = new(Resources.Blend, new string[4] { "通常", "加算", "減算", "乗算" });
        public static readonly Action[] BlentFunc = new Action[]
        {
            () =>
            {
                GL.BlendEquationSeparate(BlendEquationMode.FuncAdd, BlendEquationMode.FuncAdd);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            },
            () =>
            {
                GL.BlendEquationSeparate(BlendEquationMode.FuncAdd, BlendEquationMode.FuncAdd);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);
            },
            () =>
            {
                GL.BlendEquationSeparate(BlendEquationMode.FuncReverseSubtract, BlendEquationMode.FuncReverseSubtract);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);
            },
            () =>
            {
                GL.BlendEquationSeparate(BlendEquationMode.FuncAdd, BlendEquationMode.FuncAdd);
                GL.BlendFunc(BlendingFactor.Zero, BlendingFactor.SrcColor);
            }
        };

        public override IEnumerable<PropertyElement> Properties => new PropertyElement[]
        {
            Alpha,
            Color,
            BlendType
        };
        [DataMember(Order = 0)]
        public EaseProperty Alpha { get; private set; }
        [DataMember(Order = 1)]
        public ColorAnimationProperty Color { get; private set; }
        [DataMember(Order = 2)]
        public SelectorProperty BlendType { get; private set; }

        public Blend(PropertyElementMetadata metadata) : base(metadata)
        {
            Alpha = new(AlphaMetadata);
            BlendType = new(BlendTypeMetadata);
            Color = new(ColorMetadata);
        }

        protected override void OnLoad()
        {
            Alpha.Load(AlphaMetadata);
            BlendType.Load(BlendTypeMetadata);
            Color.Load(ColorMetadata);
        }
        protected override void OnUnload()
        {
            Alpha.Unload();
            BlendType.Unload();
            Color.Unload();
        }
    }
}
