namespace SampleGame
{
    class Program
    {
        static void Main(string[] args)
        {
            var sceneInitializer = new SceneInitializer();

            using (var game = new GameCore(sceneInitializer))
            {
                game.Run();
            }
        }
    }
}
