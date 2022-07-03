﻿using BeUtl.Animation;
using BeUtl.Rendering;
using BeUtl.Styling;

namespace BeUtl.Streaming;

public abstract class StreamStyler : StylingOperator, IStreamSelector
{
    public virtual IRenderable? Select(IRenderable? value, IClock clock)
    {
        OnPreSelect(value);
        if (value == null)
            return null;

        IStyleInstance? prevInstance = Instance;
        IStyleInstance? instance = GetInstance(value);
        if (!ReferenceEquals(prevInstance, instance))
        {
            prevInstance?.Dispose();
            Instance = instance;
        }

        if (Instance != null)
        {
            ApplyStyle(Instance, value, clock);
        }

        OnPostSelect(value);

        return value;
    }

    protected virtual void OnPreSelect(IRenderable? value)
    {
    }

    protected virtual void OnPostSelect(IRenderable? value)
    {
    }

    protected virtual IStyleInstance? GetInstance(IRenderable value)
    {
        Type type = value.GetType();
        if (!ReferenceEquals(Instance?.Target, value))
        {
            if (Style.TargetType.IsAssignableTo(type) && value is IStyleable styleable)
            {
                return Style.Instance(styleable);
            }
            else
            {
                return null;
            }
        }
        else
        {
            return Instance;
        }
    }

    protected virtual void ApplyStyle(IStyleInstance instance, IRenderable value, IClock clock)
    {
        instance.IsEnabled = IsEnabled;
        instance.Begin();
        instance.Apply(clock);
        instance.End();
    }
}
