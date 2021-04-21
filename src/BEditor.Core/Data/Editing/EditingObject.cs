﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Threading;

using BEditor.Data.Property;
using BEditor.Resources;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace BEditor.Data
{
    /// <summary>
    /// Represents the base class of the edit data.
    /// </summary>
    public class EditingObject : BasePropertyChanged, IEditingObject, IJsonObject
    {
        private Dictionary<EditingPropertyRegistryKey, object?>? _values = new();
        private Type? _ownerType;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditingObject"/> class.
        /// </summary>
        protected EditingObject()
        {
            // static コンストラクターを呼び出す
            InvokeStaticInititlizer();

            // DirectEditingPropertyかつInitializerがnullじゃない
            foreach (var prop in EditingPropertyRegistry.GetInitializableProperties(OwnerType))
            {
                if (prop is IDirectProperty direct && direct.Get(this) is null)
                {
                    direct.Set(this, direct.Initializer!.Create());
                }
            }
        }

        /// <inheritdoc/>
        public IServiceProvider? ServiceProvider { get; internal set; }

        /// <inheritdoc/>
        public bool IsLoaded { get; private set; }

        /// <inheritdoc/>
        public Guid ID { get; protected set; } = Guid.NewGuid();

        private Dictionary<EditingPropertyRegistryKey, object?> Values => _values ??= new();

        private Type OwnerType => _ownerType ??= GetType();

        /// <inheritdoc/>
        public object? this[EditingProperty property]
        {
            get => GetValue(property);
            set => SetValue(property, value);
        }

        /// <inheritdoc/>
        public TValue GetValue<TValue>(EditingProperty<TValue> property)
        {
            if (CheckOwnerType(this, property))
            {
                throw new DataException(Strings.TheOwnerTypeDoesNotMatch);
            }

            if (property is IDirectProperty<TValue> directProp)
            {
                var value = directProp.Get(this);
                if (value is null)
                {
                    value = directProp.Initializer is null ? default! : directProp.Initializer.Create();

                    if (value is not null)
                    {
                        directProp.Set(this, value);
                    }
                }

                return value;
            }

            if (!Values.ContainsKey(property.Key))
            {
                var value = property.Initializer is null ? default! : property.Initializer.Create();

                Values.Add(property.Key, value);

                return value;
            }

            return (TValue)Values[property.Key]!;
        }

        /// <inheritdoc/>
        public object? GetValue(EditingProperty property)
        {
            if (CheckOwnerType(this, property))
            {
                throw new DataException(Strings.TheOwnerTypeDoesNotMatch);
            }

            if (property is IDirectProperty directProp)
            {
                var value = directProp.Get(this);
                if (value is null)
                {
                    value = directProp.Initializer?.Create();

                    if (value is not null)
                    {
                        directProp.Set(this, value);
                    }
                }

                return value;
            }

            if (!Values.ContainsKey(property.Key))
            {
                var value = property.Initializer?.Create();
                Values.Add(property.Key, value);

                return value;
            }

            return Values[property.Key];
        }

        /// <inheritdoc/>
        public void SetValue<TValue>(EditingProperty<TValue> property, TValue value)
        {
            if (CheckOwnerType(this, property))
            {
                throw new DataException(Strings.TheOwnerTypeDoesNotMatch);
            }

            if (property is IDirectProperty<TValue> directProp)
            {
                directProp.Set(this, value);

                return;
            }

            if (AddIfNotExist(property, value))
            {
                return;
            }

            Values[property.Key] = value;
        }

        /// <inheritdoc/>
        public void SetValue(EditingProperty property, object? value)
        {
            if (value is not null && CheckValueType(property, value))
            {
                throw new DataException(string.Format(Strings.TheValueWasNotTypeButType, property.ValueType, value.GetType()));
            }
            if (CheckOwnerType(this, property))
            {
                throw new DataException(Strings.TheOwnerTypeDoesNotMatch);
            }

            if (property is IDirectProperty directProp)
            {
                directProp.Set(this, value!);

                return;
            }

            if (AddIfNotExist(property, value))
            {
                return;
            }

            Values[property.Key] = value;
        }

        /// <inheritdoc/>
        public void ClearDisposable()
        {
            static void ClearChildren(EditingObject @object)
            {
                if (@object is IParent<IEditingObject> parent)
                {
                    foreach (var child in parent.Children)
                    {
                        child.ClearDisposable();
                    }
                }

                if (@object is IKeyframeProperty property)
                {
                    property.EasingType?.ClearDisposable();
                }
            }

            foreach (var value in Values.Where(i => i.Key.IsDisposable).ToArray())
            {
                (value.Value as IDisposable)?.Dispose();
                Values.Remove(value.Key);
            }

            ClearChildren(this);
        }

        /// <inheritdoc/>
        public void Load()
        {
            if (IsLoaded) return;

            if (this is IChild<EditingObject> obj1)
            {
                ServiceProvider = obj1.Parent?.ServiceProvider;
            }

            if (this is IChild<IApplication> child_app)
            {
                ServiceProvider = child_app.Parent?.Services.BuildServiceProvider();
            }

            OnLoad();

            if (this is IParent<EditingObject> obj2)
            {
                foreach (var prop in EditingPropertyRegistry.GetProperties(OwnerType))
                {
                    var value = this[prop];
                    if (value is PropertyElement p && prop.Initializer is PropertyElementMetadata pmeta)
                    {
                        p.PropertyMetadata = pmeta;
                    }
                }

                foreach (var item in obj2.Children)
                {
                    item.Load();
                }
            }

            IsLoaded = true;
        }

        /// <inheritdoc/>
        public void Unload()
        {
            if (!IsLoaded) return;

            OnUnload();

            if (this is IParent<EditingObject> obj)
            {
                foreach (var item in obj.Children)
                {
                    item.Unload();
                }
            }

            IsLoaded = false;
        }

        /// <inheritdoc/>
        public virtual void GetObjectData(Utf8JsonWriter writer)
        {
            writer.WriteString(nameof(ID), ID);

            foreach (var prop in EditingPropertyRegistry.GetSerializableProperties(OwnerType))
            {
                var value = GetValue(prop);

                if (value is not null)
                {
                    writer.WriteStartObject(prop.Name);

                    prop.Serializer!.Write(writer, value);

                    writer.WriteEndObject();
                }
            }
        }

        /// <inheritdoc/>
        public virtual void SetObjectData(JsonElement element)
        {
            // static コンストラクターを呼び出す
            InvokeStaticInititlizer();

            ID = (element.TryGetProperty(nameof(ID), out var id) && id.TryGetGuid(out var guid)) ? guid : Guid.NewGuid();

            foreach (var prop in EditingPropertyRegistry.GetSerializableProperties(OwnerType))
            {
                if (element.TryGetProperty(prop.Name, out var propElement))
                {
                    SetValue(prop, prop.Serializer!.Read(propElement));
                }
                else if (prop.Initializer is not null)
                {
                    SetValue(prop, prop.Initializer.Create());
                }
            }
        }

        /// <inheritdoc cref="IElementObject.Load"/>
        protected virtual void OnLoad()
        {
        }

        /// <inheritdoc cref="IElementObject.Unload"/>
        protected virtual void OnUnload()
        {
        }

        // 値の型が一致しない場合はtrue
        private static bool CheckValueType(EditingProperty property, object value)
        {
            var valueType = value.GetType();

            return !valueType.IsAssignableTo(property.ValueType);
        }

        // オーナーの型が一致しない場合はtrue
        private static bool CheckOwnerType(EditingObject obj, EditingProperty property)
        {
            var ownerType = obj.OwnerType;

            return !ownerType.IsAssignableTo(property.OwnerType);
        }

        // 追加した場合はtrue
        private bool AddIfNotExist(EditingProperty property, object? value)
        {
            if (!Values.ContainsKey(property.Key))
            {
                Values.Add(property.Key, value);

                return true;
            }

            return false;
        }

        private bool AddIfNotExist<TValue>(EditingProperty property, TValue value)
        {
            if (!Values.ContainsKey(property.Key))
            {
                Values.Add(property.Key, value);

                return true;
            }

            return false;
        }

        private void InvokeStaticInititlizer()
        {
            OwnerType.TypeInitializer?.Invoke(null, null);
            OwnerType.BaseType?.TypeInitializer?.Invoke(null, null);
        }
    }
}