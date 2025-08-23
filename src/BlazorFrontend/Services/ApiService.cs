using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace BlazorFrontend.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiService> _logger;

    public ApiService(HttpClient httpClient, ILogger<ApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error calling GET {Endpoint}", endpoint);
            return default;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing response from {Endpoint}", endpoint);
            return default;
        }
    }

    public async Task<T?> PostAsync<T>(string endpoint, object data)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(endpoint, data);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var errorMessage = ExtractErrorMessage(errorContent);
                
                // Log the error for debugging
                _logger.LogWarning("API POST request failed: {Endpoint}, Status: {StatusCode}, Error: {Error}", 
                    endpoint, response.StatusCode, errorMessage);
                
                throw new ApiException(response.StatusCode, errorMessage, errorContent);
            }
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (ApiException)
        {
            // Re-throw API exceptions
            throw;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error calling POST {Endpoint}", endpoint);
            throw new ApiException(HttpStatusCode.ServiceUnavailable, "Network error. Please check your connection and try again.");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing response from {Endpoint}", endpoint);
            throw new ApiException(HttpStatusCode.InternalServerError, "Invalid response from server. Please try again.");
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Request timeout for POST {Endpoint}", endpoint);
            throw new ApiException(HttpStatusCode.RequestTimeout, "Request timeout. Please try again.");
        }
    }

    public async Task<T?> PutAsync<T>(string endpoint, object data)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync(endpoint, data);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var errorMessage = ExtractErrorMessage(errorContent);
                throw new ApiException(response.StatusCode, errorMessage, errorContent);
            }
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (ApiException)
        {
            // Re-throw API exceptions
            throw;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error calling PUT {Endpoint}", endpoint);
            return default;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing response from {Endpoint}", endpoint);
            return default;
        }
    }

    public async Task<bool> DeleteAsync(string endpoint)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(endpoint);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error calling DELETE {Endpoint}", endpoint);
            return false;
        }
    }

    public async Task<List<T>> GetListAsync<T>(string endpoint)
    {
        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<T>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            return result ?? new List<T>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error calling GET {Endpoint}", endpoint);
            return new List<T>();
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing response from {Endpoint}", endpoint);
            return new List<T>();
        }
    }

    private string ExtractErrorMessage(string errorContent)
    {
        try
        {
            // Try to parse the error response to extract the actual error message
            var errorResponse = JsonSerializer.Deserialize<JsonElement>(errorContent);
            
            // Check for common error message patterns
            if (errorResponse.TryGetProperty("error", out var errorElement))
            {
                return errorElement.GetString() ?? "An error occurred";
            }
            
            if (errorResponse.TryGetProperty("message", out var messageElement))
            {
                return messageElement.GetString() ?? "An error occurred";
            }
            
            if (errorResponse.TryGetProperty("title", out var titleElement))
            {
                return titleElement.GetString() ?? "An error occurred";
            }
            
            // If we can't extract a specific error message, return the raw content
            return errorContent;
        }
        catch
        {
            // If parsing fails, return the raw content
            return errorContent;
        }
    }
}
