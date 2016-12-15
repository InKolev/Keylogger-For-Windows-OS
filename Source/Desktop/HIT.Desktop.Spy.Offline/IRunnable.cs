using System.Threading.Tasks;

namespace HIT.Desktop.Spy.Offline
{
    public interface IRunnable
    {
        Task Start();

        void Stop();
    }
}
