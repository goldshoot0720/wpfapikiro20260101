using System.Threading.Tasks;

namespace wpfkiro20260101.Services
{
    public interface IBackendService
    {
        Task<bool> TestConnectionAsync();
        Task<bool> InitializeAsync();
        string ServiceName { get; }
        BackendServiceType ServiceType { get; }
    }

    public class BackendServiceResult<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }

        public static BackendServiceResult<T> CreateSuccess(T data)
        {
            return new BackendServiceResult<T>
            {
                Success = true,
                Data = data
            };
        }

        public static BackendServiceResult<T> CreateError(string errorMessage)
        {
            return new BackendServiceResult<T>
            {
                Success = false,
                ErrorMessage = errorMessage
            };
        }
    }
}