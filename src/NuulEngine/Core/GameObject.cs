using System;
using System.Collections.Generic;
using System.Reflection;
using NuulEngine.Core.Utils;
using NuulEngine.Core.Components;

namespace NuulEngine.Core
{
    public class GameObject
    {
        private const string DefaultTag = "Default Tag";

        private readonly List<Component> _components = new List<Component>(); 

        public GameObject()
            : this(DefaultTag)
        {
        }

        public GameObject(string tag)
        {
            IsActive = true;
            Tag = tag;
            ChildObjects = new GameObjectCollection(this);
        }

        public bool IsActive { get; set; }

        public string Tag { get; set; }

        public GameObject Parent { get; internal set; }

        public GameObjectCollection ChildObjects { get; }

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
    }
}
