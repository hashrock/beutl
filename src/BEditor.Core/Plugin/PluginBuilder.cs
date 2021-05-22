﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using BEditor.Data;
using BEditor.Data.Property.Easing;
using BEditor.Media;

using Microsoft.Extensions.DependencyInjection;

namespace BEditor.Plugin
{
    /// <summary>
    /// Represents a class that initializes the services provided by the <see cref="PluginObject"/>.
    /// </summary>
    public sealed class PluginBuilder
    {
        /// <summary>
        /// The plugin config.
        /// </summary>
        internal static PluginConfig? Config = null;
        private readonly Func<PluginObject> _plugin;
        private readonly List<EffectMetadata> _effects = new();
        private readonly List<ObjectMetadata> _objects = new();
        private readonly List<EasingMetadata> _eases = new();
        private readonly List<Func<IProgress<int>, ValueTask>> _task = new();
        private (string?, IEnumerable<ICustomMenu>?) _menus;

        private PluginBuilder(Func<PluginObject> create)
        {
            _plugin = create;
        }

        /// <summary>
        ///  Begin configuring an <see cref="PluginObject"/>.
        /// </summary>
        /// <typeparam name="T">Class that implements the <see cref="PluginObject"/> to be configure.</typeparam>
        /// <returns>The same instance of the <see cref="PluginBuilder"/> for chaining.</returns>
        public static PluginBuilder Configure<T>()
            where T : PluginObject
        {
            return new PluginBuilder(() => (T)Activator.CreateInstance(typeof(T), Config)!);
        }

        /// <summary>
        /// Configure the options for the services to be provided.
        /// </summary>
        /// <param name="metadata">Metadata of the effects to be provided.</param>
        /// <returns>The same instance of the <see cref="PluginBuilder"/> for chaining.</returns>
        public PluginBuilder With(EffectMetadata metadata)
        {
            _effects.Add(metadata);

            return this;
        }

        /// <summary>
        /// Configure the options for the services to be provided.
        /// </summary>
        /// <param name="metadata">Metadata of the objects to be provided.</param>
        /// <returns>The same instance of the <see cref="PluginBuilder"/> for chaining.</returns>
        public PluginBuilder With(ObjectMetadata metadata)
        {
            _objects.Add(metadata);

            return this;
        }

        /// <summary>
        /// Configure the options for the services to be provided.
        /// </summary>
        /// <param name="metadata">Metadata of the easings to be provided.</param>
        /// <returns>The same instance of the <see cref="PluginBuilder"/> for chaining.</returns>
        public PluginBuilder With(EasingMetadata metadata)
        {
            _eases.Add(metadata);

            return this;
        }

        /// <summary>
        /// Configure the options for the services to be provided.
        /// </summary>
        /// <param name="decoderBuilder"></param>
        /// <returns>The same instance of the <see cref="PluginBuilder"/> for chaining.</returns>
        public PluginBuilder With(IDecoderBuilder decoderBuilder)
        {
            DecoderFactory.Register(decoderBuilder);

            return this;
        }

        /// <summary>
        /// Configure the options for the services to be provided.
        /// </summary>
        /// <param name="encoderBuilder"></param>
        /// <returns>The same instance of the <see cref="PluginBuilder"/> for chaining.</returns>
        public PluginBuilder With(IEncoderBuilder encoderBuilder)
        {
            EncoderFactory.Register(encoderBuilder);

            return this;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="func"></param>
        public PluginBuilder Task(Func<IProgress<int>, ValueTask> func)
        {
            if (!_task.Contains(func))
            {
                _task.Add(func);
            }

            return this;
        }

        /// <summary>
        /// Set the menu.
        /// </summary>
        /// <param name="header">The string to display in the menu header.</param>
        /// <param name="menus">Menu to be set.</param>
        /// <returns>The same instance of the <see cref="PluginBuilder"/> for chaining.</returns>
        public PluginBuilder SetCustomMenu(string header, IEnumerable<ICustomMenu> menus)
        {
            _menus = (header, menus);

            return this;
        }

        /// <summary>
        /// Register services into the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="configureServices">A delegate for configuring the <see cref="IServiceCollection"/>.</param>
        /// <returns>The same instance of the <see cref="PluginBuilder"/> for chaining.</returns>
        public PluginBuilder ConfigureServices(Action<IServiceCollection> configureServices)
        {
            configureServices.Invoke(Config!.Application.Services);

            return this;
        }

        /// <summary>
        /// Register this setting to the specified <see cref="PluginManager"/>.
        /// </summary>
        /// <param name="manager"><see cref="PluginManager"/> to register.</param>
        public void Register(PluginManager manager)
        {
            // Effects
            foreach (var meta in _effects)
            {
                EffectMetadata.LoadedEffects.Add(meta);
            }

            // Objects
            foreach (var meta in _objects)
            {
                ObjectMetadata.LoadedObjects.Add(meta);
            }

            // Easing
            foreach (var meta in _eases)
            {
                EasingMetadata.LoadedEasingFunc.Add(meta);
            }

            if (_menus.Item1 is not null && _menus.Item2 is not null)
            {
                manager._menus.Add(_menus!);
            }

            var instance = _plugin();
            if (_task.Count is not 0)
            {
                manager._tasks.Add((instance, _task));
            }
            manager._loaded.Add(instance);
        }

        /// <summary>
        /// Register this setting to the <see cref="PluginManager.Default"/>.
        /// </summary>
        public void Register()
        {
            Register(PluginManager.Default);
        }
    }
}