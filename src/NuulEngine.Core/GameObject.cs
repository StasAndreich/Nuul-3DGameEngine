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
        }

        public string Tag { get; set; }

        public bool IsActive { get; set; }
    }
}
