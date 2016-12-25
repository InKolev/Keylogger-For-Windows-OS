namespace HIT.Desktop.Spy
{
    class Startup
    {
        static void Main(string[] args)
        {
            // Load configs in a static class/dictionary

            new Inspector().Start();
        }
    }
}
