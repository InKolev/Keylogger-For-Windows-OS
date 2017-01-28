using System.Threading.Tasks;

namespace HIT.Services
{
    public interface IHttpService
    {
        Task SendAsBson<T>(T data, string baseAddress, string requestURL);
    }
}
