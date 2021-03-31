namespace NuulEngine.Core.Components
{
    public abstract class Component
    {
        protected Component(GameObject owner)
        {
            Owner = owner;
        }

        public GameObject Owner { get; }

        public abstract void CallComponent(double deltaTime);
    }
}

