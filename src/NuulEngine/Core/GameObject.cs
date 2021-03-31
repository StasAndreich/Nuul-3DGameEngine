using System.Collections.Generic;
using NuulEngine.Core.Utils;
using NuulEngine.Core.Components;

namespace NuulEngine.Core
{
    public class GameObject
    {
        private const string DefaultTag = "Default Tag";

        private List<Component> components; 

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

        public void AddComponent(Component component)
        {
            components.Add(component);
        }

        public TComponent GetComponent<TComponent>()
            where TComponent : Component
        {
            foreach (var component in components)
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
