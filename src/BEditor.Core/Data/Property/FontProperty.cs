﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Disposables;
using System.Runtime.Serialization;
using System.Threading.Tasks;

using BEditor.Core.Command;
using BEditor.Core.Data.Bindings;
using BEditor.Core.Data.Property;
using BEditor.Core.Properties;
using BEditor.Drawing;

namespace BEditor.Core.Data.Property
{
    /// <summary>
    /// フォントを選択するプロパティ表します
    /// </summary>
    [DataContract]
    public class FontProperty : PropertyElement<FontPropertyMetadata>, IEasingProperty, IBindable<Font>
    {
        #region Fields

        /// <summary>
        /// 読み込まれているフォントのリスト
        /// </summary>
        //Todo: FontManagerを作る
        public static readonly List<Font> FontList = new();

        private static readonly PropertyChangedEventArgs _SelectArgs = new(nameof(Select));
        private Font _SelectItem;
        private List<IObserver<Font>>? _List;

        private IDisposable? _BindDispose;
        private IBindable<Font>? _Bindable;
        private string? _BindHint;

        #endregion


        /// <summary>
        /// <see cref="FontProperty"/> クラスの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="metadata">このプロパティの <see cref="FontPropertyMetadata"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="metadata"/> が <see langword="null"/> です</exception>
        public FontProperty(FontPropertyMetadata metadata)
        {
            PropertyMetadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            _SelectItem = metadata.SelectItem;
        }


        private List<IObserver<Font>> Collection => _List ??= new();
        /// <summary>
        /// 選択されているフォントを取得または設定します
        /// </summary>
        [DataMember]
        public Font Select
        {
            get => _SelectItem;
            set => SetValue(value, ref _SelectItem, _SelectArgs, this, state =>
            {
                foreach (var observer in state.Collection)
                {
                    try
                    {
                        observer.OnNext(state._SelectItem);
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                    }
                }
            });
        }
        /// <inheritdoc/>
        public Font Value => Select;
        /// <inheritdoc/>
        [DataMember]
        public string? BindHint
        {
            get => _Bindable?.GetString();
            private set => _BindHint = value;
        }


        #region Methods

        /// <inheritdoc/>
        protected override void OnLoad()
        {
            if (_BindHint is not null && this.GetBindable(_BindHint, out var b))
            {
                Bind(b);
            }
            _BindHint = null;
        }
        /// <inheritdoc/>
        public override string ToString() => $"(Select:{Select} Name:{PropertyMetadata?.Name})";

        /// <summary>
        /// フォントを変更するコマンドを作成します
        /// </summary>
        [Pure]
        public IRecordCommand ChangeFont(Font font) => new ChangeSelectCommand(this, font);

        #region IBindable

        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<Font> observer)
        {
            if (observer is null) throw new ArgumentNullException(nameof(observer));

            Collection.Add(observer);
            return Disposable.Create((observer, this), state =>
            {
                state.observer.OnCompleted();
                state.Item2.Collection.Remove(state.observer);
            });
        }

        /// <inheritdoc/>
        public void OnCompleted() { }
        /// <inheritdoc/>
        public void OnError(Exception error) { }
        /// <inheritdoc/>
        public void OnNext(Font value)
        {
            Select = value;
        }

        public void Bind(IBindable<Font>? bindable)
        {
            _BindDispose?.Dispose();
            _Bindable = bindable;

            if (bindable is not null)
            {
                Select = bindable.Value;

                // bindableが変更時にthisが変更
                _BindDispose = bindable.Subscribe(this);
            }
        }

        #endregion

        #endregion


        #region Commands

        /// <summary>
        /// フォントを変更するコマンド
        /// </summary>
        /// <remarks>このクラスは <see cref="CommandManager.Do(IRecordCommand)"/> と併用することでコマンドを記録できます</remarks>
        private sealed class ChangeSelectCommand : IRecordCommand
        {
            private readonly FontProperty _Property;
            private readonly Font _New;
            private readonly Font _Old;

            /// <summary>
            /// <see cref="ChangeSelectCommand"/> クラスの新しいインスタンスを初期化します
            /// </summary>
            /// <param name="property">対象の <see cref="FontProperty"/></param>
            /// <param name="select">新しい値</param>
            /// <exception cref="ArgumentNullException"><paramref name="property"/> が <see langword="null"/> です</exception>
            public ChangeSelectCommand(FontProperty property, Font select)
            {
                _Property = property ?? throw new ArgumentNullException(nameof(property));
                _New = select;
                _Old = property.Select;
            }

            public string Name => CommandName.ChangeFont;

            /// <inheritdoc/>
            public void Do() => _Property.Select = _New;

            /// <inheritdoc/>
            public void Redo() => Do();

            /// <inheritdoc/>
            public void Undo() => _Property.Select = _Old;
        }

        #endregion
    }

    /// <summary>
    /// <see cref="FontProperty"/> のメタデータを表します
    /// </summary>
    public record FontPropertyMetadata : PropertyElementMetadata
    {
        /// <summary>
        /// <see cref="FontPropertyMetadata"/> クラスの新しいインスタンスを初期化します
        /// </summary>
        public FontPropertyMetadata() : base(Resources.Font)
        {
            SelectItem = FontProperty.FontList.FirstOrDefault()!;
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<Font> ItemSource => FontProperty.FontList;
        /// <summary>
        /// 
        /// </summary>
        public Font SelectItem { get; init; }
        /// <summary>
        /// 
        /// </summary>
        public string MemberPath => "Name";
    }
}
