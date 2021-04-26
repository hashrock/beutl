﻿using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Avalonia;
using Avalonia.Controls;

using BEditor.Data;
using BEditor.Data.Property;
using BEditor.Extensions;
using BEditor.Models;
using BEditor.Views.Properties;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace BEditor.ViewModels.Properties
{
    public sealed class DocumentPropertyViewModel : IDisposable
    {
        private string _oldvalue;
        private readonly CompositeDisposable _disposables = new();

        public DocumentPropertyViewModel(DocumentProperty property)
        {
            Property = property;

            _oldvalue = Property.Value;

            Reset.Where(_ => Property.Value != (Property.PropertyMetadata?.DefaultText ?? string.Empty))
                .Subscribe(_ => Property.ChangeText(Property.PropertyMetadata?.DefaultText ?? string.Empty).Execute())
                .AddTo(_disposables);

            Bind.Subscribe(async () =>
            {
                var window = new SetBinding
                {
                    DataContext = new SetBindingViewModel<string>(Property)
                };
                await window.ShowDialog(App.GetMainWindow());
            }).AddTo(_disposables);

            CopyID.Subscribe(async () => await Application.Current.Clipboard.SetTextAsync(Property.ID.ToString())).AddTo(_disposables);

            GotFocus.Subscribe(_ => _oldvalue = Property.Value).AddTo(_disposables);

            LostFocus
                .Where(i => i != Property.Value)
                .Subscribe(text =>
            {
                Property.Value = _oldvalue;

                Property.ChangeText(text).Execute();
            }).AddTo(_disposables);

            TextChanged.Subscribe(text =>
            {
                Property.Value = text;

                (AppModel.Current.Project!).PreviewUpdate(Property.GetParent<ClipElement>()!);
            }).AddTo(_disposables);

            Cut.Subscribe(t => t.Cut()).AddTo(_disposables);
            Copy.Subscribe(t => t.Copy()).AddTo(_disposables);
            Paste.Subscribe(t => t.Paste()).AddTo(_disposables);
        }

        ~DocumentPropertyViewModel()
        {
            Dispose();
        }

        public DocumentProperty Property { get; }

        public ReactiveCommand Reset { get; } = new();

        public ReactiveCommand Bind { get; } = new();

        public ReactiveCommand CopyID { get; } = new();

        public ReactiveCommand<string> GotFocus { get; } = new();

        public ReactiveCommand<string> LostFocus { get; } = new();

        public ReactiveCommand<string> TextChanged { get; } = new();

        public ReactiveCommand<TextBox> Cut { get; } = new();

        public ReactiveCommand<TextBox> Copy { get; } = new();

        public ReactiveCommand<TextBox> Paste { get; } = new();

        public void Dispose()
        {
            Reset.Dispose();
            Bind.Dispose();
            CopyID.Dispose();
            GotFocus.Dispose();
            LostFocus.Dispose();
            TextChanged.Dispose();
            Cut.Dispose();
            Copy.Dispose();
            Paste.Dispose();
            _disposables.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}