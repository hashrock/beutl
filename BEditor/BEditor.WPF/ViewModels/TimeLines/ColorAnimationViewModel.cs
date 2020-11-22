﻿using System;

using BEditor.Models.Settings;
using BEditor.ViewModels.Helper;

using BEditor.Core.Data;
using BEditor.Core.Data.Property;
using BEditor.Core.Data.Primitive.Properties;
using BEditor.Core.Command;

namespace BEditor.ViewModels.TimeLines
{
    public class ColorAnimationViewModel
    {
        public double TrackHeight => Setting.ClipHeight;
        public ColorAnimationProperty ColorAnimationProperty { get; }

        public ColorAnimationViewModel(ColorAnimationProperty colorProperty)
        {
            ColorAnimationProperty = colorProperty;
            AddKeyFrameCommand.Subscribe(x => CommandManager.Do(new ColorAnimationProperty.AddCommand(colorProperty, x)));
            RemoveKeyFrameCommand.Subscribe(x => CommandManager.Do(new ColorAnimationProperty.RemoveCommand(colorProperty, x)));
            MoveKeyFrameCommand.Subscribe(x => CommandManager.Do(new ColorAnimationProperty.MoveCommand(colorProperty, x.Item1, x.Item2)));

            colorProperty.AddKeyFrameEvent += (_, value) => AddKeyFrameIcon?.Invoke(value.frame, value.index);
            colorProperty.DeleteKeyFrameEvent += (_, value) => DeleteKeyFrameIcon?.Invoke(value);
            colorProperty.MoveKeyFrameEvent += (_, value) => MoveKeyFrameIcon?.Invoke(value.fromindex, value.toindex);
        }

        #region View操作のAction

        public Action<int, int> AddKeyFrameIcon { get; set; }
        public Action<int> DeleteKeyFrameIcon { get; set; }
        public Action<int, int> MoveKeyFrameIcon { get; set; }

        #endregion

        public DelegateCommand<int> AddKeyFrameCommand { get; } = new DelegateCommand<int>();
        public DelegateCommand<int> RemoveKeyFrameCommand { get; } = new DelegateCommand<int>();
        public DelegateCommand<(int, int)> MoveKeyFrameCommand { get; } = new DelegateCommand<(int, int)>();
    }
}
