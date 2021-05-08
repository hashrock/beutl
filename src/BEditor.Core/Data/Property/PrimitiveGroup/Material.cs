﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using BEditor.Drawing;
using BEditor.Resources;

namespace BEditor.Data.Property.PrimitiveGroup
{
    /// <summary>
    /// Represents a property that sets the Material.
    /// </summary>
    public sealed class Material : ExpandGroup
    {
        /// <summary>
        /// Defines the <see cref="Ambient"/> property.
        /// </summary>
        public static readonly DirectEditingProperty<Material, ColorAnimationProperty> AmbientProperty = EditingProperty.RegisterSerializeDirect<ColorAnimationProperty, Material>(
            nameof(Ambient),
            owner => owner.Ambient,
            (owner, obj) => owner.Ambient = obj,
            new ColorAnimationPropertyMetadata(Strings.Ambient, Color.Light, true));

        /// <summary>
        /// Defines the <see cref="Diffuse"/> property.
        /// </summary>
        public static readonly DirectEditingProperty<Material, ColorAnimationProperty> DiffuseProperty = EditingProperty.RegisterSerializeDirect<ColorAnimationProperty, Material>(
            nameof(Diffuse),
            owner => owner.Diffuse,
            (owner, obj) => owner.Diffuse = obj,
            new ColorAnimationPropertyMetadata(Strings.Diffuse, Color.Light, true));

        /// <summary>
        /// Defines the <see cref="Specular"/> property.
        /// </summary>
        public static readonly DirectEditingProperty<Material, ColorAnimationProperty> SpecularProperty = EditingProperty.RegisterSerializeDirect<ColorAnimationProperty, Material>(
            nameof(Specular),
            owner => owner.Specular,
            (owner, obj) => owner.Specular = obj,
            new ColorAnimationPropertyMetadata(Strings.Specular, Color.Light, true));

        /// <summary>
        /// Defines the <see cref="Shininess"/> property.
        /// </summary>
        public static readonly DirectEditingProperty<Material, EaseProperty> ShininessProperty = EditingProperty.RegisterSerializeDirect<EaseProperty, Material>(
            nameof(Shininess),
            owner => owner.Shininess,
            (owner, obj) => owner.Shininess = obj,
            new EasePropertyMetadata(Strings.Shininess, 10, float.NaN, 1));

        /// <summary>
        /// Initializes a new instance of the <see cref="Material"/> class.
        /// </summary>
        /// <param name="metadata">Metadata of this property.</param>
        /// <exception cref="ArgumentNullException"><paramref name="metadata"/> is <see langword="null"/>.</exception>
        public Material(MaterialMetadata metadata) : base(metadata)
        {
        }

        /// <summary>
        /// Gets the ambient.
        /// </summary>
        [AllowNull]
        public ColorAnimationProperty Ambient { get; private set; }

        /// <summary>
        /// Gets the diffuse.
        /// </summary>
        [AllowNull]
        public ColorAnimationProperty Diffuse { get; private set; }

        /// <summary>
        /// Gets the shininess.
        /// </summary>
        [AllowNull]
        public ColorAnimationProperty Specular { get; private set; }

        /// <summary>
        /// Gets the shininess.
        /// </summary>
        [AllowNull]
        public EaseProperty Shininess { get; private set; }

        /// <inheritdoc/>
        public override IEnumerable<PropertyElement> GetProperties()
        {
            yield return Ambient;
            yield return Diffuse;
            yield return Specular;
            yield return Shininess;
        }
    }

    /// <summary>
    /// The metadata of <see cref="Material"/>.
    /// </summary>
    /// <param name="Name">The string displayed in the property header.</param>
    public record MaterialMetadata(string Name) : PropertyElementMetadata(Name), IEditingPropertyInitializer<Material>
    {
        /// <inheritdoc/>
        public Material Create()
        {
            return new(this);
        }
    }
}