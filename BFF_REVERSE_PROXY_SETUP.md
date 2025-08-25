# BFF Reverse Proxy Setup Guide

## Overview
Your BFF (Backend for Frontend) has been configured as a reverse proxy that routes API requests to the main API service while providing BFF-specific functionality.

## What Has Been Configured

### 1. Built-in Reverse Proxy
- Uses .NET 9's built-in reverse proxy functionality
- No additional packages required
- Simple configuration-based routing

### 2. Configuration Files
- **appsettings.json**: Contains reverse proxy routing rules
- **Program.cs**: Configures built-in reverse proxy services
- **ProxyTestController.cs**: Test endpoints to verify proxy functionality

### 3. Proxy Routes
The BFF now proxies these endpoints to the main API (port 7058):
- `/api/agencies/**` → Main API agencies
- `/api/interpreters/**` → Main API interpreters
- `/api/clients/**` → Main API clients
- `/api/appointments/**` → Main API appointments
- `/api/billing/**` → Main API billing
- `/api/admin/**` → Main API admin
- `/api/notifications/**` → Main API notifications
- `/api/staff/**` → Main API staff
- `/api/interpreter-requests/**` → Main API interpreter requests
- `/swagger/**` → Main API Swagger

### 4. BFF-Specific Endpoints
- `/api/bff/agencies` → Aggregated agency data
- `/api/bff/dashboard` → Dashboard metrics
- `/api/proxy-test/status` → Proxy status check
- `/api/proxy-test/proxy-info` → Proxy configuration info

## How to Test

### Prerequisites
1. Ensure the main API is running on `https://localhost:7058`
2. Make sure the database is accessible

### Starting the BFF
1. **Option 1**: Use the PowerShell script
   ```powershell
   .\start-bff-proxy.ps1
   ```

2. **Option 2**: Manual start
   ```bash
   cd src/Bff
   dotnet run
   ```

### Testing the Reverse Proxy

#### 1. Test BFF Status
```bash
curl https://localhost:7002/api/proxy-test/status
```
Expected response: BFF status information

#### 2. Test Proxy Info
```bash
curl https://localhost:7002/api/proxy-test/proxy-info
```
Expected response: Detailed proxy configuration

#### 3. Test Proxied Endpoints
```bash
# This will be proxied to the main API
curl https://localhost:7002/api/agencies
```

#### 4. Test BFF-Specific Endpoints
```bash
curl https://localhost:7002/api/bff/agencies
curl https://localhost:7002/api/bff/dashboard
```

## Architecture Flow

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│ BlazorFrontend  │───▶│   BFF (7002)    │───▶│  Main API      │
│   (Port 7263)   │    │ (Reverse Proxy) │    │  (Port 7058)   │
└─────────────────┘    └─────────────────┘    └─────────────────┘
                              │
                              ▼
                       ┌─────────────────┐
                       │ BFF Aggregated  │
                       │   Endpoints     │
                       └─────────────────┘
```

## Benefits of This Setup

1. **Single Entry Point**: Frontend only communicates with BFF
2. **API Proxying**: All API requests automatically routed to main API
3. **Data Aggregation**: BFF can combine data from multiple sources
4. **Simple Configuration**: Easy to maintain and extend
5. **Load Balancing**: Can be extended to route to multiple API instances
6. **Security**: Centralized authentication and authorization

## Configuration Details

### Built-in Features
- Configuration-based routing
- Automatic request forwarding
- Error handling
- CORS support

### CORS Configuration
- Configured for BlazorFrontend ports: 7263, 5096, 7001, 5001
- Allows all headers and methods

## Troubleshooting

### Common Issues

1. **Port Conflicts**
   - Ensure main API is running on port 7058
   - Ensure BFF is running on port 7002

2. **Proxy Not Working**
   - Check that main API is accessible
   - Verify reverse proxy configuration in appsettings.json
   - Check BFF logs for errors

3. **CORS Issues**
   - Verify frontend ports are in CORS configuration
   - Check browser console for CORS errors

### Debug Steps

1. Check BFF logs for any routing errors
2. Verify main API is responding on port 7058
3. Test direct API calls vs. proxied calls
4. Check network tab in browser developer tools

## Next Steps

1. **Test the setup** using the provided endpoints
2. **Update BlazorFrontend** to use BFF endpoints
3. **Monitor logs** for any routing issues
4. **Extend functionality** with additional BFF-specific endpoints

## Files Modified

- `src/Bff/Bff.csproj` - Removed unnecessary package references
- `src/Bff/appsettings.json` - Added reverse proxy configuration
- `src/Bff/Program.cs` - Added built-in reverse proxy services
- `src/Bff/Controllers/ProxyTestController.cs` - New test controller
- `src/Bff/README.md` - Updated documentation
- `start-bff-proxy.ps1` - Startup script
- `BFF_REVERSE_PROXY_SETUP.md` - This setup guide

Your BFF is now successfully configured as a reverse proxy using .NET 9's built-in functionality! 🎉
