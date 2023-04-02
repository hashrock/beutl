﻿using Beutl.Animation;
using Beutl.Rendering;
using Beutl.Styling;

namespace Beutl.Operation;

public abstract class StyledSourcePublisher : StylingOperator, ISourcePublisher
{
    public IStyleInstance? Instance { get; private set; }

    public virtual IRenderable? Publish(IClock clock)
    {
        OnPrePublish();
        IRenderable? renderable = Instance?.Target as IRenderable;
        
        if (!ReferenceEquals(Style, Instance?.Source) || Instance?.Target == null)
        {
            renderable = Activator.CreateInstance(Style.TargetType) as IRenderable;
            if (renderable is ICoreObject coreObj)
            {
                Instance?.Dispose();
                Instance = Style.Instance(coreObj);
            }
            else
            {
                renderable = null;
            }
        }

        if (Instance != null && IsEnabled)
        {
            Instance.Begin();
            Instance.Apply(clock);
            Instance.End();
        }

        OnPostPublish();

        return IsEnabled ? renderable : null;
    }

    protected override void OnDetachedFromHierarchy(in HierarchyAttachmentEventArgs args)
    {
        base.OnDetachedFromHierarchy(args);
        IStyleInstance? tmp = Instance;
        Instance = null;
        tmp?.Dispose();
    }

    protected virtual void OnPrePublish()
    {
    }

    protected virtual void OnPostPublish()
    {
    }
}
