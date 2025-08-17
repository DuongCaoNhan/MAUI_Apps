using System.Threading.Tasks;
using System.Net.Http;

namespace AutoInteractiveTool
{
    /// <summary>
    /// Interface for HTTP service operations
    /// </summary>
    public interface IHttpService
    {
        /// <summary>
        /// Sends HTTP POST request with text data
        /// </summary>
        /// <param name="url">Target URL</param>
        /// <param name="bearerToken">Bearer token for authorization</param>
        /// <param name="textData">Text data to send</param>
        /// <returns>HTTP response result</returns>
        Task<HttpRequestResult> SendPostRequestAsync(string url, string bearerToken, string textData);
    }

    /// <summary>
    /// Result of HTTP request operation
    /// </summary>
    public class HttpRequestResult
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public int StatusCode { get; set; }
    }
}