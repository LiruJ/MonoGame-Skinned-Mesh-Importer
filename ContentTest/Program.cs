using System;

namespace ContentTest
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new AnimationState())
                game.Run();
        }
    }
}
