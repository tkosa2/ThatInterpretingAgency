// Theme management JavaScript
(function () {
    'use strict';

    // Function to get system preference
    function getSystemThemePreference() {
        if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
            return 'dark';
        }
        return 'light';
    }

    // Function to initialize theme
    function initializeTheme() {
        const savedTheme = localStorage.getItem('theme');
        const systemTheme = getSystemThemePreference();
        const theme = savedTheme || systemTheme;
        
        document.documentElement.setAttribute('data-theme', theme);
        
        // Add transition class after initial load
        setTimeout(() => {
            document.documentElement.classList.add('theme-transition');
        }, 100);
    }

    // Function to set theme
    function setTheme(theme) {
        document.documentElement.setAttribute('data-theme', theme);
        localStorage.setItem('theme', theme);
    }

    // Function to toggle theme
    function toggleTheme() {
        const currentTheme = document.documentElement.getAttribute('data-theme');
        const newTheme = currentTheme === 'light' ? 'dark' : 'light';
        setTheme(newTheme);
    }

    // Listen for system theme changes
    if (window.matchMedia) {
        const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
        mediaQuery.addListener(function (e) {
            const savedTheme = localStorage.getItem('theme');
            if (!savedTheme) {
                // Only auto-switch if user hasn't manually set a preference
                setTheme(e.matches ? 'dark' : 'light');
            }
        });
    }

    // Initialize theme on page load
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initializeTheme);
    } else {
        initializeTheme();
    }

    // Expose functions globally for Blazor interop
    window.themeManager = {
        setTheme: setTheme,
        toggleTheme: toggleTheme,
        getCurrentTheme: function() {
            return document.documentElement.getAttribute('data-theme');
        }
    };
})();
