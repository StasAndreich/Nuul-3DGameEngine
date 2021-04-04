using NuulEngine.Core;
using NuulEngine.Core.Utils;

namespace SampleGame
{
    internal class SceneInitializer : ISceneInitializer
    {
        public void Initialize(GameObjectCollection scene)
        {
            scene.Add(new GameObject("firstObject"));
        }
    }
}
