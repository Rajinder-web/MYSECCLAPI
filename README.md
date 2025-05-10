# MYSECCLUI

# Investment Technical Test Solution

This solution demonstrates a decoupled system to fetch, process, and display client portfolio data from the SECCL API.

## Architecture

-   **MYSECCLAPI (ASP.NET Core Web API):** Backend service acting as middleware.
    -   Connects to the SECCL Staging API.
    -   Authenticates and fetches portfolio balance and valuation data.
    -   Aggregates data for three predefined client portfolios (total value and value by account type).
    -   Exposes an API endpoint for the frontend.
-   **P1Investment.Web (Blazor WASM):** Frontend UI.
    -   Consumes the API exposed by `MYSECCLAPI`.
    -   Displays the aggregated portfolio information on a dashboard.

## Prerequisites

-   .NET 8 SDK (or the version specified in the project files)
-   Visual Studio 2022 (Community Edition or higher) or Visual Studio Code with C# Dev Kit.
-   Git

## Setup

1.  **Clone the repository:**
    ```bash
    git clone <your-repo-url>
    cd P1InvestmentSolution
    ```

2.  **Configure Backend API (`MYSECCLAPI`):**
    *   **SECCL Credentials:**
        The API requires credentials to access the SECCL API. These should be stored using the MYSECCLAPI.Api/appsettings.json for the `MYSECCLAPI.Api` project.
        Or, using the .NET CLI (navigate to the `MYSECCLAPI` project directory):
        
    *   **Portfolio IDs and SECCL Credentials:**
        Open `MYSECCLAPI/appsettings.json`.
        Update the `PortfolioSettings:ClientPortfolioIds` array with three valid portfolio IDs accessible with the provided credentials.
        ```json
        "PortfolioSettings": {
          "ClientPortfolioIds": [
            "ACTUAL_PORTFOLIO_ID_1",
            "ACTUAL_PORTFOLIO_ID_2",
            "ACTUAL_PORTFOLIO_ID_3"
          ]
        }
        ```
    *   **CORS Origin for Blazor App (Development):**
        Open `MYSECCLAPI/appsettings.Development.json`.
        Ensure `AllowedCorsOrigin` matches the HTTPS URL and port your `MYSECCLUI` (Blazor WASM) app runs on during development. Check `MYSECCLUI/Properties/launchSettings.json` for its `applicationUrl`.
        Example:
        ```json
        {
          "AllowedCorsOrigin": "https://localhost:7123" // Replace 7123 with your Blazor app's port
        }
        ```

3.  **Configure Frontend (`MYSECCLUI`):**
    *   **API Base URL:**
        Open `MYSECCLUI/wwwroot/appsettings.json`.
        Ensure `ApiBaseUrl` points to the HTTPS URL and port where `P1Investment.Api` will be running. Check `MYSECCLAPI/Properties/launchSettings.json` for its `applicationUrl`.
        Example:
        ```json
        {
          "ApiBaseUrl": "https://localhost:7234" // Replace 7234 with your API's port
        }
        ```

## Running the Application

1.  **Start the Backend API (`MYSECCLAPI`):**
    *   In Visual Studio: Set `MYSECCLAPI` as the startup project and run it (e.g., F5 or Ctrl+F5).
    *   Using .NET CLI (from `MYSECCLAPI` directory): `dotnet run`
    *   Note the HTTPS port it's running on.

2.  **Start the Frontend Blazor WASM App (`MYSECCLUI`):**
    *   In Visual Studio: Set `MYSECCLUI` as the startup project and run it.
    *   Using .NET CLI (from `MYSECCLUI` directory): `dotnet run`
    *   Alternatively, to serve it with `dotnet watch` for hot reload (from `MYSECCLUI` directory): `dotnet watch run`
    *   Open your browser and navigate to the Blazor app's URL (e.g., `https://localhost:7123`).

3.  Navigate to the `/dashboard` page in the Blazor application to view the data.

## Project Structure Overview

-   `MYSECCLAPI/`: Contains the ASP.NET Core Web API.
    -   `Controllers/`: API endpoints.
    -   `Services/`: Business logic, SECCL API interaction, data aggregation.
    -   `Models/`: Data Transfer Objects (DTOs) and models for SECCL API responses.
-   `P1Investment.Web/`: Contains the Blazor WASM frontend application.
    -   `Pages/`: Routable Blazor components.
    -   `Services/`: Services for calling the backend API.
    -   `Shared/`: Shared UI components (layouts, navigation).
    -   `wwwroot/`: Static assets, including `appsettings.json` for frontend configuration.

## Key Assumptions & Decisions

-   **Portfolio IDs:** The specific portfolio IDs to fetch are hardcoded in `MYSECCLAPI/appsettings.json`. These need to be valid IDs from the SECCL staging environment.
-   **SECCL API Endpoints:** Placeholder SECCL API endpoints (`/auth/token`, `/portfolios/{portfolioId}/balance-and-valuations`) are used in `SecclApiService.cs`. These **MUST BE VERIFIED** against the official SECCL Postman documentation and updated if different.
-   **SECCL API Response Models:** The `SecclPortfolioValuationResponse` and `Holding` models in `MYSECCLAPI/Models/SecclPortfolioModels.cs` are based on common portfolio data structures. These **MUST BE VERIFIED** and adjusted to match the actual JSON response from SECCL's "Balances and valuations" endpoint.
-   **Error Handling:** Basic error handling and logging are implemented. More sophisticated error handling could be added.
-   **Access Token Caching:** The SECCL access token is cached in memory for a short duration to avoid re-fetching on every API call.
-   **Decoupling:** The frontend and backend are separate projects communicating via HTTP, fulfilling the decoupled architecture requirement.

## Code Comments

Comments have been added to key areas of the code to explain logic and highlight areas needing verification (like API endpoints and response models).

## Git Workflow

This project should be managed under Git version control from the beginning:
1. `git init` in the solution root.
2. Create an initial commit with the project structure.
3. Commit changes incrementally as features are developed.
