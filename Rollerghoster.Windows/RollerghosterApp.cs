using Stride.Engine;

namespace Rollerghoaster.Windows
{
    class RollerghosterApp
    {
        static void Main(string[] args)
        {
            using (var game = new Game())
            {
                game.Run();
            }
        }
    }
}
