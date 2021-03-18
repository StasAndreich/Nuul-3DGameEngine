using NuulEngine.Components.ComponentContracts;

namespace NuulEngine.Core.Components
{
    public abstract class Component : IComponent
    {
        protected Component(GameObject owner)
        {
            Owner = owner;
        }

        public GameObject Owner { get; }

        public abstract void CallComponent(double deltaTime);
    }
}
