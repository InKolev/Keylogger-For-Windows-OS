using System.Threading.Tasks;

namespace HIT.Desktop.Spy
{
    public interface IRunnable
    {
        Task Start();

        void Stop();
    }
}
