﻿using System;
using System.Collections.Generic;
using System.Reflection;
using NuulEngine.Core.Utils;
using NuulEngine.Core.Components;

namespace NuulEngine.Core
{
    public class GameObject
    {
        private const string DefaultName = "Default Name";

        private readonly List<Component> _components = new List<Component>(); 

        public GameObject()
            : this(DefaultName)
        {
        }

        public GameObject(string name)
        {
            IsActive = true;
            Name = name;
            ChildObjects = new GameObjectCollection(this);
        }

        public bool IsActive { get; set; }

        public string Name { get; set; }

        public GameObjectCollection ChildObjects { get; }

        public GameObject Parent { get; internal set; }

        public Transform Transform { get; }

        /// <summary>
        /// Adds a component to a game object.
        /// </summary>
        /// <remarks>Uses reflection for adding an instance to its internal storage.</remarks>
        /// <typeparam name="TComponent">Inheritor of Component class.</typeparam>
        /// <returns>TComponent instance.</returns>
        public TComponent AddComponent<TComponent>()
            where TComponent : Component
        {
            if (typeof(Component).IsAssignableFrom(typeof(TComponent))
                && GetComponent<TComponent>() != null)
            {
                throw new InvalidOperationException("Components must be unique.");
            }

            var ctor = typeof(TComponent).GetConstructor(
                bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic,
                binder: null,
                types: new[] { typeof(GameObject) },
                modifiers: null);

            var component = (TComponent)ctor.Invoke(new[] { this });

            _components.Add(component);

            return component;
        }

        /// <summary>
        /// Returns a component of a specified type from a game object.
        /// </summary>
        /// <typeparam name="TComponent">Inheritor of Component class.</typeparam>
        /// <returns>TComponent instance.</returns>
        public TComponent GetComponent<TComponent>()
            where TComponent : Component
        {
            foreach (var component in _components)
            {
                if(component is TComponent result)
                {
                    return result;
                }
            }

            return null;
        }

        internal void Update(float deltaTime)
        {
            foreach (var component in _components)
            {
                component.CallComponent(deltaTime);
            }
        }
    }
}
