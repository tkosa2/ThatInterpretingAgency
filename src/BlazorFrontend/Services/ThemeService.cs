using Microsoft.JSInterop;

namespace BlazorFrontend.Services;

public class ThemeService
{
    private readonly IJSRuntime _jsRuntime;
    public event Action? OnThemeChanged;

    public ThemeService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<string> GetCurrentThemeAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string>("themeManager.getCurrentTheme") ?? "light";
        }
        catch
        {
            // Fallback to localStorage if JS interop fails
            try
            {
                return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "theme") ?? "light";
            }
            catch
            {
                return "light";
            }
        }
    }

    public async Task SetThemeAsync(string theme)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("themeManager.setTheme", theme);
            OnThemeChanged?.Invoke();
        }
        catch
        {
            // Fallback if JS interop fails
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "theme", theme);
                await _jsRuntime.InvokeVoidAsync("document.documentElement.setAttribute", "data-theme", theme);
                OnThemeChanged?.Invoke();
            }
            catch
            {
                // Last resort fallback
            }
        }
    }

    public async Task ToggleThemeAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("themeManager.toggleTheme");
            OnThemeChanged?.Invoke();
        }
        catch
        {
            // Fallback if JS interop fails
            var currentTheme = await GetCurrentThemeAsync();
            var newTheme = currentTheme == "light" ? "dark" : "light";
            await SetThemeAsync(newTheme);
        }
    }

    public async Task InitializeThemeAsync()
    {
        try
        {
            // The JavaScript will handle initialization automatically
            // Just ensure the current theme is reflected in the service
            var theme = await GetCurrentThemeAsync();
            // Trigger the event to update any components
            OnThemeChanged?.Invoke();
        }
        catch
        {
            // Fallback initialization
            try
            {
                var theme = await GetCurrentThemeAsync();
                await _jsRuntime.InvokeVoidAsync("document.documentElement.setAttribute", "data-theme", theme);
            }
            catch
            {
                // Set default theme
                await _jsRuntime.InvokeVoidAsync("document.documentElement.setAttribute", "data-theme", "light");
            }
        }
    }
}
