using System;
using NuulEngine.Core.Utils;

namespace NuulEngine.Core
{
    [Obsolete("Scene class is unsupported.", true)]
    public class Scene
    {
        private const string DefaultName = "Default Scene";

        public Scene(GameObjectCollection gameObjects)
            : this(DefaultName, gameObjects)
        {
        }

        public Scene(string name, GameObjectCollection gameObjects)
        {
            Name = name;
            GameObjects = gameObjects;
        }

        public string Name { get; }

        public GameObjectCollection GameObjects { get; }
    }
}
