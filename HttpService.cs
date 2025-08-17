using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace AutoInteractiveTool
{
    /// <summary>
    /// HTTP service implementation for sending POST requests
    /// </summary>
    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;

        public HttpService()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        /// <summary>
        /// Sends HTTP POST request with text data
        /// </summary>
        /// <param name="url">Target URL</param>
        /// <param name="bearerToken">Bearer token for authorization</param>
        /// <param name="textData">Text data to send</param>
        /// <returns>HTTP response result</returns>
        public async Task<HttpRequestResult> SendPostRequestAsync(string url, string bearerToken, string textData)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(url))
                {
                    return new HttpRequestResult
                    {
                        IsSuccess = false,
                        Message = "URL cannot be empty",
                        StatusCode = 0
                    };
                }

                if (string.IsNullOrWhiteSpace(bearerToken))
                {
                    return new HttpRequestResult
                    {
                        IsSuccess = false,
                        Message = "Bearer token cannot be empty",
                        StatusCode = 0
                    };
                }

                // Create request
                using var request = new HttpRequestMessage(HttpMethod.Post, url);
                
                // Add authorization header
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);
                
                // Create JSON content with the text data
                var jsonContent = JsonSerializer.Serialize(new { data = textData });
                request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Send request
                using var response = await _httpClient.SendAsync(request);
                
                var responseContent = await response.Content.ReadAsStringAsync();
                
                return new HttpRequestResult
                {
                    IsSuccess = response.IsSuccessStatusCode,
                    Message = response.IsSuccessStatusCode ? "Success" : $"Error: {response.ReasonPhrase} - {responseContent}",
                    StatusCode = (int)response.StatusCode
                };
            }
            catch (HttpRequestException ex)
            {
                return new HttpRequestResult
                {
                    IsSuccess = false,
                    Message = $"HTTP Error: {ex.Message}",
                    StatusCode = 0
                };
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                return new HttpRequestResult
                {
                    IsSuccess = false,
                    Message = "Request timeout",
                    StatusCode = 0
                };
            }
            catch (Exception ex)
            {
                return new HttpRequestResult
                {
                    IsSuccess = false,
                    Message = $"Unexpected error: {ex.Message}",
                    StatusCode = 0
                };
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}