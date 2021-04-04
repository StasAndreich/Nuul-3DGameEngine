namespace SampleGame
{
    class Program
    {
        static void Main(string[] args)
        {
            var sceneInitializer = new SceneInitializer();
            var game = new GameCore(sceneInitializer);
            game.Run();
        }
    }
}
