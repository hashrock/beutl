﻿using System.Diagnostics.CodeAnalysis;

using BeUtl.Animation;

namespace BeUtl.Styling;

#pragma warning disable CA1816

public class StyleInstance : IStyleInstance
{
    private IStyleable? _target;
    private IStyle? _source;
    private ISetterInstance[] _setters;
    private ISetterInstance[][]? _cache;

    public StyleInstance(IStyleable target, IStyle source, ISetterInstance[] setters, IStyleInstance? baseStyle)
    {
        _target = target;
        _source = source;
        _setters = setters;
        BaseStyle = baseStyle;
    }

    public bool IsEnabled { get; set; }

    public IStyleInstance? BaseStyle { get; private set; }

    public IStyleable Target => _target ?? throw new InvalidOperationException();

    public IStyle Source => _source ?? throw new InvalidOperationException();

    public ReadOnlySpan<ISetterInstance> Setters => _setters;

    public void Apply(IClock clock)
    {
        if (_cache == null)
        {
            Build();
        }

        Target.BeginBatchUpdate();

        foreach (ISetterInstance[] entry in _cache.AsSpan())
        {
            foreach (ISetterInstance item in entry.AsSpan())
            {
                item.Apply(clock);
            }
        }

        Target.EndBatchUpdate();
    }

    public void Dispose()
    {
        foreach (ISetterInstance item in Setters)
        {
            item.Dispose();
        }

        _target = null;
        _source = null;
        _setters = Array.Empty<ISetterInstance>();
        _cache = null;
        BaseStyle = null;
    }

    [MemberNotNull(nameof(_cache))]
    private void Build()
    {
        var dict = new Dictionary<int, ISetterInstance[]>();
        IStyleInstance? next = this;
        while (next != null)
        {
            foreach (ISetterInstance item in next.Setters)
            {
                if (!dict.ContainsKey(item.Property.Id))
                {
                    dict[item.Property.Id] = GetSettersFromProperty(item.Property);
                }
            }

            next = next.BaseStyle;
        }

        _cache = new ISetterInstance[dict.Count][];
        dict.Values.CopyTo(_cache, 0);
    }

    private ISetterInstance[] GetSettersFromProperty(CoreProperty property)
    {
        static void Func(IStyleInstance style, ISetterInstance[] setters, ref int index, CoreProperty property)
        {
            if (style.BaseStyle != null)
            {
                Func(style.BaseStyle, setters, ref index, property);
            }

            if (FindSetter(style.Setters, property) is ISetterInstance setter)
            {
                setters[index++] = setter;
            }
        }

        IStyleInstance? next = this;
        int count = 0;

        while (next != null)
        {
            if (ExistSetter(next.Setters, property))
            {
                count++;
            }
            next = next.BaseStyle;
        }

        var array = new ISetterInstance[count];
        count = 0;
        Func(this, array, ref count, property);

        return array;
    }

    private static bool ExistSetter(ReadOnlySpan<ISetterInstance> setters, CoreProperty property)
    {
        foreach (ISetterInstance item in setters)
        {
            if (item.Property.Id == property.Id)
            {
                return true;
            }
        }

        return false;
    }

    private static ISetterInstance? FindSetter(ReadOnlySpan<ISetterInstance> setters, CoreProperty property)
    {
        foreach (ISetterInstance item in setters)
        {
            if (item.Property.Id == property.Id)
            {
                return item;
            }
        }

        return null;
    }
}
