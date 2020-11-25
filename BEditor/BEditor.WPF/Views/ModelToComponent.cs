﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using BEditor.Models.Settings;
using BEditor.ViewModels.TimeLines;
using BEditor.Views.CustomControl;
using BEditor.Views.PropertyControls;
using BEditor.Views.TimeLines;

using BEditor.Core.Data.Property;
using BEditor.Core.Data.Property.EasingProperty;
using BEditor.Core.Extensions;
using BEditor.Core.Data.Primitive.Properties;
using BEditor.Core.Data.Control;
using BEditor.Core.Data;

namespace BEditor.Views
{
    public static class ModelToComponent
    {
        public static List<PropertyViewBuilder> PropertyViewCreaters { get; } = new List<PropertyViewBuilder>();
        public static List<KeyFrameViewBuilder> KeyFrameViewCreaters { get; } = new List<KeyFrameViewBuilder>();

        static ModelToComponent()
        {

            #region CreatePropertyView
            PropertyViewCreaters.Add(new()
            {
                PropertyType = typeof(CheckProperty),
                CreateFunc = (elm) => new PropertyControls.CheckBox(elm as CheckProperty)
            });
            PropertyViewCreaters.Add(new()
            {
                PropertyType = typeof(ColorAnimationProperty),
                CreateFunc = (elm) => new PropertyControl.ColorAnimation(elm as ColorAnimationProperty)
            });
            PropertyViewCreaters.Add(new()
            {
                PropertyType = typeof(ColorProperty),
                CreateFunc = (elm) => new PropertyControls.ColorPicker(elm as ColorProperty)
            });
            PropertyViewCreaters.Add(new()
            {
                PropertyType = typeof(DocumentProperty),
                CreateFunc = (elm) => new Document(elm as DocumentProperty)
            });
            PropertyViewCreaters.Add(new()
            {
                PropertyType = typeof(EaseProperty),
                CreateFunc = (elm) => new EaseControl(elm as EaseProperty)
            });
            PropertyViewCreaters.Add(new()
            {
                PropertyType = typeof(ExpandGroup),
                CreateFunc = (elm) =>
                {
                    var group = elm as ExpandGroup;
                    var _settingcontrol = new CustomTreeView()
                    {
                        Header = group.PropertyMetadata.Name,
                        HeaderHeight = 35F
                    };

                    var stack = new VirtualizingStackPanel();
                    VirtualizingPanel.SetIsVirtualizing(stack, true);
                    VirtualizingPanel.SetVirtualizationMode(stack, VirtualizationMode.Recycling);

                    var margin = new Thickness(32.5, 0, 0, 0);

                    foreach (var item in group.Children)
                    {
                        var content = item.GetCreatePropertyView();

                        if (content is FrameworkElement fe)
                        {
                            fe.Margin = margin;
                        }

                        stack.Children.Add(content);
                    }

                    _settingcontrol.Content = stack;

                    _settingcontrol.SetResourceReference(CustomTreeView.HeaderColorProperty, "MaterialDesignBody");
                    _settingcontrol.SetBinding(CustomTreeView.IsExpandedProperty, new Binding("IsExpanded") { Mode = BindingMode.TwoWay, Source = group });

                    _settingcontrol.ExpanderUpdate();

                    return _settingcontrol;
                }
            });
            PropertyViewCreaters.Add(new()
            {
                PropertyType = typeof(FileProperty),
                CreateFunc = (elm) => new FileControl(elm as FileProperty)
            });
            PropertyViewCreaters.Add(new()
            {
                PropertyType = typeof(FontProperty),
                CreateFunc = (elm) => new SelectorControl(elm as FontProperty)
            });
            PropertyViewCreaters.Add(new()
            {
                PropertyType = typeof(Group),
                CreateFunc = (elm) =>
                {
                    VirtualizingStackPanel stack = new VirtualizingStackPanel();
                    VirtualizingPanel.SetIsVirtualizing(stack, true);
                    VirtualizingPanel.SetVirtualizationMode(stack, VirtualizationMode.Recycling);

                    var group = elm as Group;

                    foreach (var item in group.Children)
                    {
                        var content = item.GetCreatePropertyView();

                        stack.Children.Add(content);
                    }

                    return stack;
                }
            });
            PropertyViewCreaters.Add(new()
            {
                PropertyType = typeof(SelectorProperty),
                CreateFunc = (elm) => new SelectorControl(elm as SelectorProperty)
            });
            PropertyViewCreaters.Add(new()
            {
                PropertyType = typeof(ValueProperty),
                CreateFunc = (elm) => new ValueControl(elm as ValueProperty)
            });
            #endregion

            #region CreateKeyFrameView
            KeyFrameViewCreaters.Add(new()
            {
                PropertyType = typeof(EaseProperty),
                CreateFunc = (elm) => new KeyFrame(elm.GetParent3(), elm as EaseProperty)
            });
            KeyFrameViewCreaters.Add(new()
            {
                PropertyType = typeof(ColorAnimationProperty),
                CreateFunc = (elm) => new ColorAnimation(elm as ColorAnimationProperty)
            });
            KeyFrameViewCreaters.Add(new()
            {
                PropertyType = typeof(ExpandGroup),
                CreateFunc = (elm) =>
                {
                    var group = elm as ExpandGroup;

                    var expander = new CustomTreeView()
                    {
                        Header = group.PropertyMetadata.Name,
                        HeaderHeight = (float)(Setting.ClipHeight + 1),
                        TreeStair = 1
                    };

                    var stack = new VirtualizingStackPanel();
                    VirtualizingPanel.SetIsVirtualizing(stack, true);
                    VirtualizingPanel.SetVirtualizationMode(stack, VirtualizationMode.Recycling);

                    expander.Content = stack;

                    var binding = new Binding("ActualWidth") { Mode = BindingMode.OneWay, Source = stack };

                    foreach (var item in group.Children)
                    {
                        if (item is IKeyFrameProperty easing)
                        {
                            var tmp = easing.GetCreateKeyFrameView();

                            (tmp as FrameworkElement)?.SetBinding(FrameworkElement.WidthProperty, binding);

                            stack.Children.Add(tmp);
                        }
                    }

                    expander.SetBinding(CustomTreeView.IsExpandedProperty, new Binding("IsExpanded") { Mode = BindingMode.TwoWay, Source = group });

                    expander.ExpanderUpdate();

                    return expander;
                }
            });
            KeyFrameViewCreaters.Add(new()
            {
                PropertyType = typeof(Group),
                CreateFunc = (elm) =>
                {
                    var group = elm as Group;

                    var stack = new VirtualizingStackPanel();
                    VirtualizingPanel.SetIsVirtualizing(stack, true);
                    VirtualizingPanel.SetVirtualizationMode(stack, VirtualizationMode.Recycling);

                    var binding = new Binding("ActualWidth") { Mode = BindingMode.OneWay, Source = stack };

                    foreach (var item in group.Children)
                    {
                        if (item is IKeyFrameProperty easing)
                        {
                            var tmp = easing.GetCreateKeyFrameView();
                            (tmp as FrameworkElement)?.SetBinding(FrameworkElement.WidthProperty, binding);
                            stack.Children.Add(tmp);
                        }
                    }

                    return stack;
                }
            });
            #endregion
        }

        public class PropertyViewBuilder
        {
            public Type PropertyType { get; set; }
            public Func<PropertyElement, UIElement> CreateFunc { get; set; }
        }

        public class KeyFrameViewBuilder
        {
            public Type PropertyType { get; set; }
            public Func<IKeyFrameProperty, UIElement> CreateFunc { get; set; }
        }

        public static UIElement GetCreatePropertyView(this PropertyElement property)
        {
            if (!property.ComponentData.ContainsKey("GetPropertyView"))
            {
                var type = property.GetType();
                var func = PropertyViewCreaters.Find(x => type == x.PropertyType || type.IsSubclassOf(x.PropertyType));

                property.ComponentData.Add("GetPropertyView", func.CreateFunc?.Invoke(property));
            }
            return property.ComponentData["GetPropertyView"];
        }
        public static UIElement GetCreateKeyFrameView(this IKeyFrameProperty property)
        {
            if (!property.ComponentData.ContainsKey("GetKeyFrameView"))
            {
                var type = property.GetType();
                var func = KeyFrameViewCreaters.Find(x => type == x.PropertyType || type.IsSubclassOf(x.PropertyType));

                property.ComponentData.Add("GetKeyFrameView", func.CreateFunc?.Invoke(property));
            }
            return property.ComponentData["GetKeyFrameView"];
        }
        public static ClipUI GetCreateClipView(this ClipData clip)
        {
            if (!clip.ComponentData.ContainsKey("GetClipView"))
            {
                clip.ComponentData.Add("GetClipView", new ClipUI(clip)
                {
                    Name = clip.Name,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top
                });
            }
            return clip.ComponentData["GetClipView"];
        }
        public static ClipUIViewModel GetCreateClipViewModel(this ClipData clip)
        {
            if (!clip.ComponentData.ContainsKey("GetClipViewModel"))
            {
                clip.ComponentData.Add("GetClipViewModel", new ClipUIViewModel(clip));
            }
            return clip.ComponentData["GetClipViewModel"];
        }
        public static UIElement GetCreatePropertyView(this EffectElement effect)
        {
            if (!effect.ComponentData.ContainsKey("GetControl"))
            {
                CustomTreeView expander;
                VirtualizingStackPanel stack;

                if (effect is ObjectElement @object)
                {
                    (expander, stack) = App.CreateTreeObject(@object);
                }
                else
                {
                    (expander, stack) = App.CreateTreeEffect(effect);
                }

                //Get毎にnewされると非効率なのでローカルに置く

                var margin = new Thickness(0, 0, 32.5, 0);
                foreach (var item in effect.Children)
                {
                    var tmp = item.GetCreatePropertyView();

                    if (tmp is FrameworkElement element)
                    {
                        element.Margin = margin;
                    }

                    stack.Children.Add(tmp);
                }

                //エクスパンダーをアップデート
                expander.ExpanderUpdate();

                effect.ComponentData.Add("GetControl", expander);
            }
            return effect.ComponentData["GetControl"];
        }
        public static UIElement GetCreateKeyFrameView(this EffectElement effect)
        {
            if (!effect.ComponentData.ContainsKey("GetKeyFrame"))
            {
                var keyFrame = new CustomTreeView() { HeaderHeight = (float)(Setting.ClipHeight + 1) };

                VirtualizingStackPanel stack = new VirtualizingStackPanel();
                VirtualizingPanel.SetIsVirtualizing(stack, true);
                VirtualizingPanel.SetVirtualizationMode(stack, VirtualizationMode.Recycling);

                keyFrame.Content = stack;

                //Get毎にnewされると非効率なのでローカルに置く
                var binding = new Binding("ActualWidth") { Mode = BindingMode.OneWay, Source = keyFrame };

                foreach (var item in effect.Children)
                {
                    if (item is IKeyFrameProperty e)
                    {

                        var tmp = e.GetCreateKeyFrameView();
                        (tmp as FrameworkElement)?.SetBinding(FrameworkElement.WidthProperty, binding);
                        stack.Children.Add(tmp);
                    }
                }

                keyFrame.SetBinding(CustomTreeView.HeaderProperty, new Binding("Name") { Mode = BindingMode.OneTime, Source = effect });
                keyFrame.SetBinding(CustomTreeView.IsExpandedProperty, new Binding("IsExpanded") { Mode = BindingMode.TwoWay, Source = effect });

                //エクスパンダーをアップデート
                keyFrame.ExpanderUpdate();

                effect.ComponentData.Add("GetKeyFrame", keyFrame);
            }
            return effect.ComponentData["GetKeyFrame"];
        }
        public static UIElement GetCreatePropertyView(this ClipData clip)
        {
            if (!clip.ComponentData.ContainsKey("GetPropertyView"))
            {
                clip.ComponentData.Add("GetPropertyView", new Object_Setting(clip));
            }
            return clip.ComponentData["GetPropertyView"];
        }
        public static TimeLine GetCreateTimeLineView(this Scene scene)
        {
            if (!scene.ComponentData.ContainsKey("GetTimeLine"))
            {
                scene.ComponentData.Add("GetTimeLine", new TimeLine(scene));
            }
            return scene.ComponentData["GetTimeLine"];
        }
        public static TimeLineViewModel GetCreateTimeLineViewModel(this Scene scene)
        {
            if (!scene.ComponentData.ContainsKey("GetTimeLineViewModel"))
            {
                scene.ComponentData.Add("GetTimeLineViewModel", new TimeLineViewModel(scene));
            }
            return scene.ComponentData["GetTimeLineViewModel"];
        }
        public static PropertyTab GetCreatePropertyTab(this Scene scene)
        {
            if (!scene.ComponentData.ContainsKey("GetPropertyTab"))
            {
                scene.ComponentData.Add("GetPropertyTab", new PropertyTab() { DataContext = scene });
            }
            return scene.ComponentData["GetPropertyTab"];
        }
        public static UIElement GetCreatePropertyView(this EasingFunc easing)
        {
            if (!easing.ComponentData.ContainsKey("GetPropertyView"))
            {
                var _createdControl = new VirtualizingStackPanel()
                {
                    Orientation = Orientation.Vertical,
                    Width = float.NaN,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };

                foreach (var setting in easing.Children)
                {
                    _createdControl.Children.Add(((PropertyElement)setting).GetCreatePropertyView());
                }

                easing.ComponentData.Add("GetPropertyView", _createdControl);
            }
            return easing.ComponentData["GetPropertyView"];
        }
    }
}
