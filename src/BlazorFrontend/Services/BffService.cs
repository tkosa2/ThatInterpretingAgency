using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BlazorFrontend.Data;

namespace BlazorFrontend.Services;

public class BffService : IBffService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BffService> _logger;

    public BffService(HttpClient httpClient, ILogger<BffService> logger)
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
            _logger.LogError(ex, "Error calling BFF GET {Endpoint}", endpoint);
            return default;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing BFF response from {Endpoint}", endpoint);
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
                
                _logger.LogWarning("BFF POST request failed: {Endpoint}, Status: {StatusCode}, Error: {Error}", 
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
            throw;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error calling BFF POST {Endpoint}", endpoint);
            throw new ApiException(HttpStatusCode.ServiceUnavailable, "Network error. Please check your connection and try again.");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing BFF response from {Endpoint}", endpoint);
            throw new ApiException(HttpStatusCode.InternalServerError, "Invalid response from BFF server. Please try again.");
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Request timeout for BFF POST {Endpoint}", endpoint);
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
            throw;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error calling BFF PUT {Endpoint}", endpoint);
            throw new ApiException(HttpStatusCode.ServiceUnavailable, "Network error. Please check your connection and try again.");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing BFF response from {Endpoint}", endpoint);
            throw new ApiException(HttpStatusCode.InternalServerError, "Invalid response from BFF server. Please try again.");
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Request timeout for BFF PUT {Endpoint}", endpoint);
            throw new ApiException(HttpStatusCode.RequestTimeout, "Request timeout. Please try again.");
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
            _logger.LogError(ex, "Network error calling BFF DELETE {Endpoint}", endpoint);
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
            _logger.LogError(ex, "Error calling BFF GET {Endpoint}", endpoint);
            return new List<T>();
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing BFF response from {Endpoint}", endpoint);
            return new List<T>();
        }
    }

    private string ExtractErrorMessage(string errorContent)
    {
        try
        {
            var errorResponse = JsonSerializer.Deserialize<JsonElement>(errorContent);
            if (errorResponse.TryGetProperty("error", out var errorProp))
            {
                return errorProp.GetString() ?? "Unknown error";
            }
            if (errorResponse.TryGetProperty("message", out var messageProp))
            {
                return messageProp.GetString() ?? "Unknown error";
            }
            return errorContent;
        }
        catch
        {
            return errorContent;
        }
    }
}

