# BFF (Backend for Frontend) - Reverse Proxy Configuration

## Overview
The BFF now acts as a reverse proxy, routing API requests to the main API service while providing BFF-specific functionality for the BlazorFrontend.

## Architecture
```
BlazorFrontend → BFF (Reverse Proxy) → Main API
     ↓              ↓                    ↓
  Port 7263    Port 7002           Port 7058
```

## Reverse Proxy Configuration

### Built-in Functionality
- Uses .NET 9's built-in reverse proxy functionality
- No additional packages required
- Simple configuration-based routing

### Routes
The BFF proxies the following API endpoints to the main API service:

- `/api/agencies/**` → Main API agencies controller
- `/api/interpreters/**` → Main API interpreters controller  
- `/api/clients/**` → Main API clients controller
- `/api/appointments/**` → Main API appointments controller
- `/api/billing/**` → Main API billing controller
- `/api/admin/**` → Main API admin controller
- `/api/notifications/**` → Main API notifications controller
- `/api/staff/**` → Main API staff controller
- `/api/interpreter-requests/**` → Main API interpreter requests controller
- `/swagger/**` → Main API Swagger documentation

### BFF-Specific Endpoints
The BFF provides its own endpoints for aggregated data:

- `/api/bff/agencies` → Aggregated agency data
- `/api/bff/dashboard` → Dashboard metrics
- `/api/proxy-test/status` → Proxy status check
- `/api/proxy-test/proxy-info` → Proxy configuration info

## Configuration Files

### appsettings.json
Contains the reverse proxy routing configuration with:
- Route definitions for each API endpoint
- Cluster configuration pointing to the main API

### Program.cs
Configures the reverse proxy with:
- Built-in reverse proxy services
- Simple configuration loading

## Benefits

1. **Single Entry Point**: Frontend only needs to communicate with the BFF
2. **API Aggregation**: BFF can combine data from multiple sources
3. **Request Proxying**: All API requests automatically routed to main API
4. **Load Balancing**: Can be extended to route to multiple API instances
5. **Security**: Centralized authentication and authorization
6. **Monitoring**: Centralized logging and metrics

## Running the BFF

1. Ensure the main API is running on `https://localhost:7058`
2. Start the BFF: `dotnet run --project src/Bff`
3. The BFF will be available on `https://localhost:7002`
4. All API requests will be proxied to the main API

## Testing

### Test Endpoints
- BFF Status: `https://localhost:7002/api/proxy-test/status`
- Proxy Info: `https://localhost:7002/api/proxy-test/proxy-info`
- Proxied API: `https://localhost:7002/api/agencies` (forwards to main API)

## Troubleshooting

- Check that the main API is running on the correct port
- Verify the reverse proxy configuration in `appsettings.json`
- Ensure CORS is properly configured for your frontend ports
- Check BFF logs for any routing errors
