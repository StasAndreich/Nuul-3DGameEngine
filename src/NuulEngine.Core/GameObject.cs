using NuulEngine.Core.Utils;

namespace NuulEngine.Core
{
    public class GameObject
    {
        private const string _defaultTag = "Default Tag";

        public GameObject()
            : this(_defaultTag)
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
    }
}
