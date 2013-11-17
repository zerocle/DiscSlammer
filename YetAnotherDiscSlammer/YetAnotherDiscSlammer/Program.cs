using System;

namespace YetAnotherDiscSlammer
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (DiscSlammerGame game = new DiscSlammerGame())
            {
                game.Run();
            }
        }
    }
#endif
}

