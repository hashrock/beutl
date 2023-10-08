﻿using Avalonia;

using Beutl.Controls.PropertyEditors;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Beutl.ViewModels.Editors
{
    // Vector2
    public sealed class PixelPointEditorViewModel : ValueEditorViewModel<Media.PixelPoint>
    {
        public PixelPointEditorViewModel(IAbstractProperty<Media.PixelPoint> property)
            : base(property)
        {
            FirstValue = Value
                .Select(x => x.X)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);

            SecondValue = Value
                .Select(x => x.Y)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);
        }

        public ReadOnlyReactivePropertySlim<int> FirstValue { get; }

        public ReadOnlyReactivePropertySlim<int> SecondValue { get; }

        public override void Accept(IPropertyEditorContextVisitor visitor)
        {
            base.Accept(visitor);
            if (visitor is Vector2Editor<int> editor)
            {
                editor.FirstHeader = "X";
                editor.SecondHeader = "Y";
                editor[!Vector2Editor<int>.FirstValueProperty] = FirstValue.ToBinding();
                editor[!Vector2Editor<int>.SecondValueProperty] = SecondValue.ToBinding();
                editor.ValueConfirmed += OnValueConfirmed;
                editor.ValueChanging += OnValueChanging;
            }
        }

        private void OnValueConfirmed(object? sender, PropertyEditorValueChangedEventArgs e)
        {
            if (e is PropertyEditorValueChangedEventArgs<(int X, int Y)> args)
            {
                SetValue(new Media.PixelPoint(args.OldValue.X, args.OldValue.Y),
                         new Media.PixelPoint(args.NewValue.X, args.NewValue.Y));
            }
        }

        private void OnValueChanging(object? sender, PropertyEditorValueChangedEventArgs e)
        {
            if (sender is Vector2Editor<int> editor)
            {
                Media.PixelPoint coerced = SetCurrentValueAndGetCoerced(
                    new Media.PixelPoint(editor.FirstValue, editor.SecondValue));
                editor.FirstValue = coerced.X;
                editor.SecondValue = coerced.Y;
            }
        }
    }
    public sealed class PixelSizeEditorViewModel : ValueEditorViewModel<Media.PixelSize>
    {
        public PixelSizeEditorViewModel(IAbstractProperty<Media.PixelSize> property)
            : base(property)
        {
            FirstValue = Value
                .Select(x => x.Width)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);

            SecondValue = Value
                .Select(x => x.Height)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);
        }

        public ReadOnlyReactivePropertySlim<int> FirstValue { get; }

        public ReadOnlyReactivePropertySlim<int> SecondValue { get; }

        public override void Accept(IPropertyEditorContextVisitor visitor)
        {
            base.Accept(visitor);
            if (visitor is Vector2Editor<int> editor)
            {
                editor.FirstHeader = Strings.Width;
                editor.SecondHeader = Strings.Height;
                editor[!Vector2Editor<int>.FirstValueProperty] = FirstValue.ToBinding();
                editor[!Vector2Editor<int>.SecondValueProperty] = SecondValue.ToBinding();
                editor.ValueConfirmed += OnValueChanged;
                editor.ValueChanging += OnValueChanging;
            }
        }

        private void OnValueChanged(object? sender, PropertyEditorValueChangedEventArgs e)
        {
            if (e is PropertyEditorValueChangedEventArgs<(int Width, int Height)> args)
            {
                SetValue(new Media.PixelSize(args.OldValue.Width, args.OldValue.Height),
                         new Media.PixelSize(args.NewValue.Width, args.NewValue.Height));
            }
        }

        private void OnValueChanging(object? sender, PropertyEditorValueChangedEventArgs e)
        {
            if (sender is Vector2Editor<int> editor)
            {
                Media.PixelSize coerced = SetCurrentValueAndGetCoerced(
                    new Media.PixelSize(editor.FirstValue, editor.SecondValue));
                editor.FirstValue = coerced.Width;
                editor.SecondValue = coerced.Height;
            }
        }
    }
    public sealed class PointEditorViewModel : ValueEditorViewModel<Graphics.Point>
    {
        public PointEditorViewModel(IAbstractProperty<Graphics.Point> property)
            : base(property)
        {
            FirstValue = Value
                .Select(x => x.X)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);

            SecondValue = Value
                .Select(x => x.Y)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);
        }

        public ReadOnlyReactivePropertySlim<float> FirstValue { get; }

        public ReadOnlyReactivePropertySlim<float> SecondValue { get; }

        public override void Accept(IPropertyEditorContextVisitor visitor)
        {
            base.Accept(visitor);
            if (visitor is Vector2Editor<float> editor)
            {
                editor.FirstHeader = "X";
                editor.SecondHeader = "Y";
                editor[!Vector2Editor<float>.FirstValueProperty] = FirstValue.ToBinding();
                editor[!Vector2Editor<float>.SecondValueProperty] = SecondValue.ToBinding();
                editor.ValueConfirmed += OnValueChanged;
                editor.ValueChanging += OnValueChanging;
            }
        }

        private void OnValueChanged(object? sender, PropertyEditorValueChangedEventArgs e)
        {
            if (e is PropertyEditorValueChangedEventArgs<(float X, float Y)> args)
            {
                SetValue(new Graphics.Point(args.OldValue.X, args.OldValue.Y),
                         new Graphics.Point(args.NewValue.X, args.NewValue.Y));
            }
        }

        private void OnValueChanging(object? sender, PropertyEditorValueChangedEventArgs e)
        {
            if (sender is Vector2Editor<float> editor)
            {
                Graphics.Point coerced = SetCurrentValueAndGetCoerced(
                    new Graphics.Point(editor.FirstValue, editor.SecondValue));
                editor.FirstValue = coerced.X;
                editor.SecondValue = coerced.Y;
            }
        }
    }
    public sealed class SizeEditorViewModel : ValueEditorViewModel<Graphics.Size>
    {
        public SizeEditorViewModel(IAbstractProperty<Graphics.Size> property)
            : base(property)
        {
            FirstValue = Value
                .Select(x => x.Width)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);

            SecondValue = Value
                .Select(x => x.Height)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);
        }

        public ReadOnlyReactivePropertySlim<float> FirstValue { get; }

        public ReadOnlyReactivePropertySlim<float> SecondValue { get; }

        public override void Accept(IPropertyEditorContextVisitor visitor)
        {
            base.Accept(visitor);
            if (visitor is Vector2Editor<float> editor)
            {
                editor.FirstHeader = Strings.Width;
                editor.SecondHeader = Strings.Height;
                editor[!Vector2Editor<float>.FirstValueProperty] = FirstValue.ToBinding();
                editor[!Vector2Editor<float>.SecondValueProperty] = SecondValue.ToBinding();
                editor.ValueConfirmed += OnValueChanged;
                editor.ValueChanging += OnValueChanging;
            }
        }

        private void OnValueChanged(object? sender, PropertyEditorValueChangedEventArgs e)
        {
            if (e is PropertyEditorValueChangedEventArgs<(float Width, float Height)> args)
            {
                SetValue(new Graphics.Size(args.OldValue.Width, args.OldValue.Height),
                         new Graphics.Size(args.NewValue.Width, args.NewValue.Height));
            }
        }

        private void OnValueChanging(object? sender, PropertyEditorValueChangedEventArgs e)
        {
            if (sender is Vector2Editor<float> editor)
            {
                Graphics.Size coerced = SetCurrentValueAndGetCoerced(
                    new Graphics.Size(editor.FirstValue, editor.SecondValue));
                editor.FirstValue = coerced.Width;
                editor.SecondValue = coerced.Height;
            }
        }
    }
    public sealed class VectorEditorViewModel : ValueEditorViewModel<Graphics.Vector>
    {
        public VectorEditorViewModel(IAbstractProperty<Graphics.Vector> property)
            : base(property)
        {
            FirstValue = Value
                .Select(x => x.X)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);

            SecondValue = Value
                .Select(x => x.Y)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);
        }

        public ReadOnlyReactivePropertySlim<float> FirstValue { get; }

        public ReadOnlyReactivePropertySlim<float> SecondValue { get; }

        public override void Accept(IPropertyEditorContextVisitor visitor)
        {
            base.Accept(visitor);
            if (visitor is Vector2Editor<float> editor)
            {
                editor.FirstHeader = "X";
                editor.SecondHeader = "Y";
                editor[!Vector2Editor<float>.FirstValueProperty] = FirstValue.ToBinding();
                editor[!Vector2Editor<float>.SecondValueProperty] = SecondValue.ToBinding();
                editor.ValueConfirmed += OnValueChanged;
                editor.ValueChanging += OnValueChanging;
            }
        }

        private void OnValueChanged(object? sender, PropertyEditorValueChangedEventArgs e)
        {
            if (e is PropertyEditorValueChangedEventArgs<(float X, float Y)> args)
            {
                SetValue(new Graphics.Vector(args.OldValue.X, args.OldValue.Y),
                         new Graphics.Vector(args.NewValue.X, args.NewValue.Y));
            }
        }

        private void OnValueChanging(object? sender, PropertyEditorValueChangedEventArgs e)
        {
            if (sender is Vector2Editor<float> editor)
            {
                Graphics.Vector coerced = SetCurrentValueAndGetCoerced(
                    new Graphics.Vector(editor.FirstValue, editor.SecondValue));
                editor.FirstValue = coerced.X;
                editor.SecondValue = coerced.Y;
            }
        }
    }
    public sealed class Vector2EditorViewModel : ValueEditorViewModel<System.Numerics.Vector2>
    {
        public Vector2EditorViewModel(IAbstractProperty<System.Numerics.Vector2> property)
            : base(property)
        {
            FirstValue = Value
                .Select(x => x.X)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);

            SecondValue = Value
                .Select(x => x.Y)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);
        }

        public ReadOnlyReactivePropertySlim<float> FirstValue { get; }

        public ReadOnlyReactivePropertySlim<float> SecondValue { get; }

        public override void Accept(IPropertyEditorContextVisitor visitor)
        {
            base.Accept(visitor);
            if (visitor is Vector2Editor<float> editor)
            {
                editor.FirstHeader = "X";
                editor.SecondHeader = "Y";
                editor[!Vector2Editor<float>.FirstValueProperty] = FirstValue.ToBinding();
                editor[!Vector2Editor<float>.SecondValueProperty] = SecondValue.ToBinding();
                editor.ValueConfirmed += OnValueChanged;
                editor.ValueChanging += OnValueChanging;
            }
        }

        private void OnValueChanged(object? sender, PropertyEditorValueChangedEventArgs e)
        {
            if (e is PropertyEditorValueChangedEventArgs<(float X, float Y)> args)
            {
                SetValue(new System.Numerics.Vector2(args.OldValue.X, args.OldValue.Y),
                         new System.Numerics.Vector2(args.NewValue.X, args.NewValue.Y));
            }
        }

        private void OnValueChanging(object? sender, PropertyEditorValueChangedEventArgs e)
        {
            if (sender is Vector2Editor<float> editor)
            {
                System.Numerics.Vector2 coerced = SetCurrentValueAndGetCoerced(
                    new System.Numerics.Vector2(editor.FirstValue, editor.SecondValue));
                editor.FirstValue = coerced.X;
                editor.SecondValue = coerced.Y;
            }
        }
    }

    // Vector3
    public sealed class Vector3EditorViewModel : ValueEditorViewModel<System.Numerics.Vector3>
    {
        public Vector3EditorViewModel(IAbstractProperty<System.Numerics.Vector3> property)
            : base(property)
        {
            FirstValue = Value
                .Select(x => x.X)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);

            SecondValue = Value
                .Select(x => x.Y)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);

            ThirdValue = Value
                .Select(x => x.Z)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);
        }

        public ReadOnlyReactivePropertySlim<float> FirstValue { get; }

        public ReadOnlyReactivePropertySlim<float> SecondValue { get; }

        public ReadOnlyReactivePropertySlim<float> ThirdValue { get; }

        public override void Accept(IPropertyEditorContextVisitor visitor)
        {
            base.Accept(visitor);
            if (visitor is Vector3Editor<float> editor)
            {
                editor.FirstHeader = "X";
                editor.SecondHeader = "Y";
                editor.ThirdHeader = "Z";

                editor[!Vector3Editor<float>.FirstValueProperty] = FirstValue.ToBinding();
                editor[!Vector3Editor<float>.SecondValueProperty] = SecondValue.ToBinding();
                editor[!Vector3Editor<float>.ThirdValueProperty] = ThirdValue.ToBinding();
                editor.ValueConfirmed += OnValueChanged;
                editor.ValueChanging += OnValueChanging;
            }
        }

        private void OnValueChanged(object? sender, PropertyEditorValueChangedEventArgs e)
        {
            if (e is PropertyEditorValueChangedEventArgs<(float X, float Y, float Z)> args)
            {
                SetValue(new System.Numerics.Vector3(args.OldValue.X, args.OldValue.Y, args.OldValue.Z),
                         new System.Numerics.Vector3(args.NewValue.X, args.NewValue.Y, args.NewValue.Z));
            }
        }

        private void OnValueChanging(object? sender, PropertyEditorValueChangedEventArgs e)
        {
            if (sender is Vector3Editor<float> editor)
            {
                System.Numerics.Vector3 coerced = SetCurrentValueAndGetCoerced(
                    new System.Numerics.Vector3(editor.FirstValue, editor.SecondValue, editor.ThirdValue));
                editor.FirstValue = coerced.X;
                editor.SecondValue = coerced.Y;
                editor.ThirdValue = coerced.Z;
            }
        }
    }

    // Vector4
    public sealed class PixelRectEditorViewModel : ValueEditorViewModel<Media.PixelRect>
    {
        public PixelRectEditorViewModel(IAbstractProperty<Media.PixelRect> property)
            : base(property)
        {
            FirstValue = Value
                .Select(x => x.X)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);

            SecondValue = Value
                .Select(x => x.Y)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);

            ThirdValue = Value
                .Select(x => x.Width)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);

            FourthValue = Value
                .Select(x => x.Height)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);
        }

        public ReadOnlyReactivePropertySlim<int> FirstValue { get; }

        public ReadOnlyReactivePropertySlim<int> SecondValue { get; }

        public ReadOnlyReactivePropertySlim<int> ThirdValue { get; }

        public ReadOnlyReactivePropertySlim<int> FourthValue { get; }

        public override void Accept(IPropertyEditorContextVisitor visitor)
        {
            base.Accept(visitor);
            if (visitor is Vector4Editor<int> editor)
            {
                editor.FirstHeader = "X";
                editor.SecondHeader = "Y";
                editor.ThirdHeader = Strings.Width;
                editor.FourthHeader = Strings.Height;

                editor[!Vector4Editor<int>.FirstValueProperty] = FirstValue.ToBinding();
                editor[!Vector4Editor<int>.SecondValueProperty] = SecondValue.ToBinding();
                editor[!Vector4Editor<int>.ThirdValueProperty] = ThirdValue.ToBinding();
                editor[!Vector4Editor<int>.FourthValueProperty] = FourthValue.ToBinding();
                editor.ValueConfirmed += OnValueChanged;
                editor.ValueChanging += OnValueChanging;
            }
        }

        private void OnValueChanged(object? sender, PropertyEditorValueChangedEventArgs e)
        {
            if (e is PropertyEditorValueChangedEventArgs<(int X, int Y, int Width, int Height)> args)
            {
                SetValue(new Media.PixelRect(args.OldValue.X, args.OldValue.Y, args.OldValue.Width, args.OldValue.Height),
                         new Media.PixelRect(args.NewValue.X, args.NewValue.Y, args.NewValue.Width, args.NewValue.Height));
            }
        }

        private void OnValueChanging(object? sender, PropertyEditorValueChangedEventArgs e)
        {
            if (sender is Vector4Editor<int> editor)
            {
                Media.PixelRect coerced = SetCurrentValueAndGetCoerced(
                    new Media.PixelRect(editor.FirstValue, editor.SecondValue, editor.ThirdValue, editor.FourthValue));
                editor.FirstValue = coerced.X;
                editor.SecondValue = coerced.Y;
                editor.ThirdValue = coerced.Width;
                editor.FourthValue = coerced.Height;
            }
        }
    }
    public sealed class RectEditorViewModel : ValueEditorViewModel<Graphics.Rect>
    {
        public RectEditorViewModel(IAbstractProperty<Graphics.Rect> property)
            : base(property)
        {
            FirstValue = Value
                .Select(x => x.X)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);

            SecondValue = Value
                .Select(x => x.Y)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);

            ThirdValue = Value
                .Select(x => x.Width)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);

            FourthValue = Value
                .Select(x => x.Height)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);
        }

        public ReadOnlyReactivePropertySlim<float> FirstValue { get; }

        public ReadOnlyReactivePropertySlim<float> SecondValue { get; }

        public ReadOnlyReactivePropertySlim<float> ThirdValue { get; }

        public ReadOnlyReactivePropertySlim<float> FourthValue { get; }

        public override void Accept(IPropertyEditorContextVisitor visitor)
        {
            base.Accept(visitor);
            if (visitor is Vector4Editor<float> editor)
            {
                editor.FirstHeader = "X";
                editor.SecondHeader = "Y";
                editor.ThirdHeader = Strings.Width;
                editor.FourthHeader = Strings.Height;

                editor[!Vector4Editor<float>.FirstValueProperty] = FirstValue.ToBinding();
                editor[!Vector4Editor<float>.SecondValueProperty] = SecondValue.ToBinding();
                editor[!Vector4Editor<float>.ThirdValueProperty] = ThirdValue.ToBinding();
                editor[!Vector4Editor<float>.FourthValueProperty] = FourthValue.ToBinding();
                editor.ValueConfirmed += OnValueChanged;
                editor.ValueChanging += OnValueChanging;
            }
        }

        private void OnValueChanged(object? sender, PropertyEditorValueChangedEventArgs e)
        {
            if (e is PropertyEditorValueChangedEventArgs<(float X, float Y, float Width, float Height)> args)
            {
                SetValue(new Graphics.Rect(args.OldValue.X, args.OldValue.Y, args.OldValue.Width, args.OldValue.Height),
                         new Graphics.Rect(args.NewValue.X, args.NewValue.Y, args.NewValue.Width, args.NewValue.Height));
            }
        }

        private void OnValueChanging(object? sender, PropertyEditorValueChangedEventArgs e)
        {
            if (sender is Vector4Editor<float> editor)
            {
                Graphics.Rect coerced = SetCurrentValueAndGetCoerced(
                    new Graphics.Rect(editor.FirstValue, editor.SecondValue, editor.ThirdValue, editor.FourthValue));
                editor.FirstValue = coerced.X;
                editor.SecondValue = coerced.Y;
                editor.ThirdValue = coerced.Width;
                editor.FourthValue = coerced.Height;
            }
        }
    }
    public sealed class Vector4EditorViewModel : ValueEditorViewModel<System.Numerics.Vector4>
    {
        public Vector4EditorViewModel(IAbstractProperty<System.Numerics.Vector4> property)
            : base(property)
        {
            FirstValue = Value
                .Select(x => x.X)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);

            SecondValue = Value
                .Select(x => x.Y)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);

            ThirdValue = Value
                .Select(x => x.Z)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);

            FourthValue = Value
                .Select(x => x.W)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(Disposables);
        }

        public ReadOnlyReactivePropertySlim<float> FirstValue { get; }

        public ReadOnlyReactivePropertySlim<float> SecondValue { get; }

        public ReadOnlyReactivePropertySlim<float> ThirdValue { get; }

        public ReadOnlyReactivePropertySlim<float> FourthValue { get; }

        public override void Accept(IPropertyEditorContextVisitor visitor)
        {
            base.Accept(visitor);
            if (visitor is Vector4Editor<float> editor)
            {
                editor.FirstHeader = "X";
                editor.SecondHeader = "Y";
                editor.ThirdHeader = "Z";
                editor.FourthHeader = "W";

                editor[!Vector4Editor<float>.FirstValueProperty] = FirstValue.ToBinding();
                editor[!Vector4Editor<float>.SecondValueProperty] = SecondValue.ToBinding();
                editor[!Vector4Editor<float>.ThirdValueProperty] = ThirdValue.ToBinding();
                editor[!Vector4Editor<float>.FourthValueProperty] = FourthValue.ToBinding();
                editor.ValueConfirmed += OnValueChanged;
                editor.ValueChanging += OnValueChanging;
            }
        }

        private void OnValueChanged(object? sender, PropertyEditorValueChangedEventArgs e)
        {
            if (e is PropertyEditorValueChangedEventArgs<(float X, float Y, float Z, float W)> args)
            {
                SetValue(new System.Numerics.Vector4(args.OldValue.X, args.OldValue.Y, args.OldValue.Z, args.OldValue.W),
                         new System.Numerics.Vector4(args.NewValue.X, args.NewValue.Y, args.NewValue.Z, args.NewValue.W));
            }
        }

        private void OnValueChanging(object? sender, PropertyEditorValueChangedEventArgs e)
        {
            if (sender is Vector4Editor<float> editor)
            {
                System.Numerics.Vector4 coerced = SetCurrentValueAndGetCoerced(
                    new System.Numerics.Vector4(editor.FirstValue, editor.SecondValue, editor.ThirdValue, editor.FourthValue));
                editor.FirstValue = coerced.X;
                editor.SecondValue = coerced.Y;
                editor.ThirdValue = coerced.Z;
                editor.FourthValue = coerced.W;
            }
        }
    }
}
