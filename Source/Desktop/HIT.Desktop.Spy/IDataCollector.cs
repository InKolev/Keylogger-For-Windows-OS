using System.Threading.Tasks;

namespace HIT.Desktop.Spy
{
    public interface IDataCollector
    {
        /// <summary>
        /// Start collecting data
        /// </summary>
        /// <returns></returns>
        Task Start();

        /// <summary>
        /// Stop collecting data
        /// </summary>
        void Stop();
    }
}
