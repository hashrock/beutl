﻿using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace BeUtl;

public interface ICoreObject : INotifyPropertyChanged, INotifyPropertyChanging, IJsonSerializable
{
    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    string Name { get; set; }

    bool BeginBatchUpdate();

    bool EndBatchUpdate();

    void ClearValue<TValue>(CoreProperty<TValue> property);

    void ClearValue(CoreProperty property);

    TValue GetValue<TValue>(CoreProperty<TValue> property);

    object? GetValue(CoreProperty property);

    void SetValue<TValue>(CoreProperty<TValue> property, TValue? value);

    void SetValue(CoreProperty property, object? value);
}

public abstract class CoreObject : ICoreObject
{
    /// <summary>
    /// Defines the <see cref="Id"/> property.
    /// </summary>
    public static readonly CoreProperty<Guid> IdProperty;

    /// <summary>
    /// Defines the <see cref="Name"/> property.
    /// </summary>
    public static readonly CoreProperty<string> NameProperty;

    /// <summary>
    /// The last JsonNode that was used.
    /// </summary>
    protected JsonNode? JsonNode;

    private Dictionary<int, object?>? _values;
    private Dictionary<int, object?>? _batchChanges;
    private bool _batchUpdate;

    static CoreObject()
    {
        IdProperty = ConfigureProperty<Guid, CoreObject>(nameof(Id))
            .Observability(PropertyObservability.ChangingAndChanged)
            .DefaultValue(Guid.Empty)
            .Register();

        NameProperty = ConfigureProperty<string, CoreObject>(nameof(Name))
            .Observability(PropertyObservability.ChangingAndChanged)
            .DefaultValue(string.Empty)
            .Register();
    }

    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    public Guid Id
    {
        get => GetValue(IdProperty);
        set => SetValue(IdProperty, value);
    }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name
    {
        get => GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    private Dictionary<int, object?> Values => _values ??= new();

    private Dictionary<int, object?> BatchChanges => _batchChanges ??= new();

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Occurs when a property value is changing.
    /// </summary>
    public event PropertyChangingEventHandler? PropertyChanging;

    public static CorePropertyInitializationHelper<T, TOwner> ConfigureProperty<T, TOwner>(string name)
    {
        return new CorePropertyInitializationHelper<T, TOwner>(name);
    }

    public bool BeginBatchUpdate()
    {
        if (_batchUpdate)
        {
            return false;
        }
        else
        {
            _batchUpdate = true;
            return true;
        }
    }

    public bool EndBatchUpdate()
    {
        if (!_batchUpdate)
        {
            return false;
        }
        else
        {
            _batchUpdate = false;
            if (_batchChanges != null)
            {
                foreach (KeyValuePair<int, object?> item in BatchChanges)
                {
                    if (PropertyRegistry.FindRegistered(item.Key) is CoreProperty property)
                    {
                        SetValue(property, item.Value);
                    }
                }
                BatchChanges.Clear();
            }

            return true;
        }
    }

    public TValue GetValue<TValue>(CoreProperty<TValue> property)
    {
        Type ownerType = GetType();
        if (ValidateOwnerType(property, ownerType))
        {
            throw new ElementException("Owner does not match.");
        }

        if (property is StaticProperty<TValue> staticProperty)
        {
            return staticProperty.RouteGetTypedValue(this)!;
        }

        if (_values == null || !_values.ContainsKey(property.Id))
        {
            return (TValue)property.GetMetadata(ownerType).DefaultValue!;
        }

        return (TValue)Values[property.Id]!;
    }

    public object? GetValue(CoreProperty property)
    {
        ArgumentNullException.ThrowIfNull(property);

        return property.RouteGetValue(this);
    }

    public void SetValue<TValue>(CoreProperty<TValue> property, TValue? value)
    {
        if (value != null && !value.GetType().IsAssignableTo(property.PropertyType))
            throw new ElementException($"{nameof(value)} of type {value.GetType().Name} cannot be assigned to type {property.PropertyType}.");

        Type ownerType = GetType();
        if (ValidateOwnerType(property, ownerType)) throw new ElementException("Owner does not match.");

        if (property is StaticProperty<TValue> staticProperty)
        {
            staticProperty.RouteSetTypedValue(this, value);
        }
        else if (_batchUpdate)
        {
            BatchChanges[property.Id] = value;
        }
        else
        {
            if (_values == null || !Values.TryGetValue(property.Id, out object? oldValue))
            {
                oldValue = null;
            }

            object? newValue = value;

            if (!RuntimeHelpers.Equals(oldValue, newValue))
            {
                CorePropertyMetadata metadata = property.GetMetadata(ownerType);
                RaisePropertyChanging(property, metadata);
                Values[property.Id] = newValue;
                RaisePropertyChanged(property, metadata, value, (TValue?)(oldValue ?? default(TValue)));
            }
        }
    }

    public void SetValue(CoreProperty property, object? value)
    {
        ArgumentNullException.ThrowIfNull(property);

        property.RouteSetValue(this, value!);
    }

    public void ClearValue<TValue>(CoreProperty<TValue> property)
    {
        SetValue(property, property.GetMetadata(GetType()).DefaultValue);
    }

    public void ClearValue(CoreProperty property)
    {
        SetValue(property, property.GetMetadata(GetType()).DefaultValue);
    }

    [MemberNotNull("JsonNode")]
    public virtual void FromJson(JsonNode json)
    {
        JsonNode = json;
        Type ownerType = GetType();

        // Todo: 例外処理
        if (json is JsonObject obj)
        {
            IReadOnlyList<CoreProperty> list = PropertyRegistry.GetRegistered(GetType());
            for (int i = 0; i < list.Count; i++)
            {
                CoreProperty item = list[i];
                CorePropertyMetadata metadata = item.GetMetadata(ownerType);
                string? jsonName = metadata.GetValueOrDefault<string>(PropertyMetaTableKeys.JsonName);
                Type type = item.PropertyType;

                if (jsonName != null &&
                    obj.TryGetPropertyValue(jsonName, out JsonNode? jsonNode) &&
                    jsonNode != null)
                {
                    if (type.IsAssignableTo(typeof(IJsonSerializable)))
                    {
                        var sobj = (IJsonSerializable?)Activator.CreateInstance(type);
                        if (sobj != null)
                        {
                            sobj.FromJson(jsonNode!);
                            SetValue(item, sobj);
                        }
                    }
                    else
                    {
                        object? value = JsonSerializer.Deserialize(jsonNode, type, JsonHelper.SerializerOptions);
                        if (value != null)
                        {
                            SetValue(item, value);
                        }
                    }
                }
            }
        }
    }

    [MemberNotNull("JsonNode")]
    public virtual JsonNode ToJson()
    {
        JsonObject? json;
        Type ownerType = GetType();

        if (JsonNode is JsonObject jsonNodeObj)
        {
            json = jsonNodeObj;
        }
        else
        {
            json = new JsonObject();
            JsonNode = json;
        }

        IReadOnlyList<CoreProperty> list = PropertyRegistry.GetRegistered(GetType());
        for (int i = 0; i < list.Count; i++)
        {
            CoreProperty item = list[i];
            CorePropertyMetadata metadata = item.GetMetadata(ownerType);
            string? jsonName = metadata.GetValueOrDefault<string>(PropertyMetaTableKeys.JsonName);
            if (jsonName != null)
            {
                object? obj = GetValue(item);
                object? def = metadata.DefaultValue;

                // デフォルトの値と取得した値が同じ場合、保存しない
                if (RuntimeHelpers.Equals(def, obj))
                {
                    json.Remove(jsonName);
                    continue;
                }

                if (obj is IJsonSerializable child)
                {
                    json[jsonName] = child.ToJson();
                }
                else
                {
                    json[jsonName] = JsonSerializer.SerializeToNode(obj, item.PropertyType, JsonHelper.SerializerOptions);
                }
            }
        }

        return json;
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }

    protected void OnPropertyChanging([CallerMemberName] string? propertyName = null)
    {
        OnPropertyChanging(new PropertyChangingEventArgs(propertyName));
    }

    protected void OnPropertyChanged(PropertyChangedEventArgs args)
    {
        PropertyChanged?.Invoke(this, args);
    }

    protected void OnPropertyChanging(PropertyChangingEventArgs args)
    {
        PropertyChanging?.Invoke(this, args);
    }

    protected bool SetAndRaise<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            OnPropertyChanging(propertyName);
            field = value;
            OnPropertyChanged(propertyName);

            return true;
        }
        else
        {
            return false;
        }
    }

    protected bool SetAndRaise<T>(CoreProperty<T> property, ref T field, T value)
    {
        if (_batchUpdate)
        {
            BatchChanges[property.Id] = value;
            return true;
        }
        else if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            CorePropertyMetadata metadata = property.GetMetadata(GetType());
            RaisePropertyChanging(property, metadata);

            T old = field;
            field = value;

            RaisePropertyChanged(property, metadata, value, old);

            return true;
        }
        else
        {
            return false;
        }
    }

    // オーナーの型が一致しない場合はtrue
    private static bool ValidateOwnerType(CoreProperty property, Type ownerType)
    {
        return !ownerType.IsAssignableTo(property.OwnerType);
    }

    private void RaisePropertyChanged<T>(CoreProperty<T> property, CorePropertyMetadata metadata, T? newValue, T? oldValue)
    {
        if (this is ILogicalElement logicalElement)
        {
            if (oldValue is ILogicalElement oldLogical)
            {
                oldLogical.NotifyDetachedFromLogicalTree(new LogicalTreeAttachmentEventArgs(null));
            }

            if (newValue is ILogicalElement newLogical)
            {
                newLogical.NotifyAttachedToLogicalTree(new LogicalTreeAttachmentEventArgs(logicalElement));
            }
        }

        bool hasChangedFlag = metadata.Observability.HasFlag(PropertyObservability.Changed);
        CorePropertyChangedEventArgs<T>? eventArgs = property.HasObservers || hasChangedFlag
            ? new CorePropertyChangedEventArgs<T>(this, property, newValue, oldValue)
            : null;

        if (property.HasObservers)
        {
            property.NotifyChanged(eventArgs!);
        }

        if (hasChangedFlag)
        {
            OnPropertyChanged(eventArgs!);
        }
    }

    private void RaisePropertyChanging(CoreProperty property, CorePropertyMetadata metadata)
    {
        PropertyObservability observability = metadata.Observability;
        if (observability.HasFlag(PropertyObservability.Changing))
        {
            OnPropertyChanging(property.Name);
        }
    }
}
