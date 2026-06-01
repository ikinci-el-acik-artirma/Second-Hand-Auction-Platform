# Second-Hand Auction Platform

Second-Hand Auction Platform is an ASP.NET Core Web API MVP for second-hand
electronic device auctions. Sellers can create listings and upload product
photos, while buyers can browse listings, filter them by category, and place
valid bids. The project also includes an OpenAI-powered device type detection
flow and a simple browser interface for demonstration.

## Features

- Buyer and seller registration
- Login with hashed password verification
- Password reset token generation and password update
- Seller-only auction listing creation
- Auction listing search by category
- Buyer-only bidding with active-auction and minimum-amount checks
- Bid history ordered from highest to lowest amount
- Product photo upload
- AI device type detection: `Phone`, `Laptop`, `Tablet`, `DesktopComputer`, or
  `Unknown`
- AI device type report endpoint
- Static MVP web interface for registration, login, listing search, and product
  details
- xUnit test suite with 36 passing test cases

## Technology Stack

| Area | Technology |
| --- | --- |
| Backend | ASP.NET Core Web API, .NET 10 |
| Data access | Entity Framework Core |
| MVP data store | EF Core InMemory provider |
| Frontend | HTML, CSS, vanilla JavaScript |
| Unit testing | xUnit |
| AI integration | OpenAI API |
| Planning | Jira Kanban |
| Version control | Git and GitHub pull requests |

## Project Structure

```text
AuctionSystem.API/
  AuctionSystem.API/
    Controllers/       REST API controllers
    Data/              Entity Framework Core DbContext
    DTOs/              Request and response contracts
    Models/            User, AuctionItem, Bid, and DevicePhoto entities
    Services/          Business rules and AI integration
    wwwroot/           Static MVP web interface
  AuctionSystem.Tests/ xUnit tests
```

## Requirements

- .NET 10 SDK
- An OpenAI API key for the device photo detection endpoint

The registration, login, listing, search, and bidding flows do not require an
OpenAI API key. The key is only needed for `POST /api/devicephotos/detect`.

## Configuration

Store the OpenAI key outside the repository. For local development, use .NET
user secrets from the API project directory:

```powershell
dotnet user-secrets set "OpenAI:ApiKey" "YOUR_OPENAI_API_KEY"
```

Do not commit API keys to GitHub.

## Run the API

From the repository root:

```powershell
dotnet run --project AuctionSystem.API/AuctionSystem.API/AuctionSystem.API.csproj
```

The launch settings expose the development API at:

- `https://localhost:7262`
- `http://localhost:5204`

Open the root URL in a browser to use the static MVP interface.

## Run the Tests

```powershell
dotnet test AuctionSystem.API/AuctionSystem.Tests/AuctionSystem.Tests.csproj
```

Current verified result:

```text
Passed: 36
Failed: 0
Skipped: 0
```

## API Summary

| Method | Endpoint | Description |
| --- | --- | --- |
| `POST` | `/api/auth/register` | Register a buyer or seller |
| `POST` | `/api/auth/login` | Log in with email and password |
| `POST` | `/api/auth/forgot-password` | Create a password reset token |
| `POST` | `/api/auth/reset-password` | Update the password with a valid token |
| `GET` | `/api/auctionitems?category=Laptop` | List or filter auction items |
| `POST` | `/api/auctionitems` | Create an auction listing as a seller |
| `GET` | `/api/auctionitems/{id}/bids` | List bids from highest to lowest |
| `POST` | `/api/auctionitems/{id}/bids` | Place a valid bid as a buyer |
| `POST` | `/api/devicephotos/detect` | Upload a photo and detect device type |
| `GET` | `/api/devicephotos/{id}/report` | Read the AI device type report |

Example requests are available in
`AuctionSystem.API/AuctionSystem.API/AuctionSystem.API.http`.

## Demo Flow

1. Register a seller account.
2. Create an auction listing with a future end date.
3. Register a buyer account.
4. Browse or filter the auction listing.
5. Place a bid higher than the starting price.
6. Read the bid history.
7. Configure the OpenAI API key and upload a product photo when testing AI
   device type detection.

## Current MVP Limitations

- The InMemory database is reset when the API restarts.
- JWT authentication and endpoint authorization policies are not implemented
  yet. The services enforce buyer and seller role rules.
- Password reset tokens are returned by the API for the MVP. A production
  system should send reset links through an email provider.
- The AI scope is limited to device type classification. It does not include
  cosmetic condition analysis, damage detection, valuation, or price
  prediction.
- The static browser interface covers registration, login, listing search, and
  product details. Auction creation, bidding, and AI upload can be demonstrated
  through the REST API request file.

## Team Contributions

| Team member | Main contributions |
| --- | --- |
| Muhammed Yilmaz | Auction listing, bidding engine, category search, basic web interface |
| Mehmetali Murt | Authentication, password reset, AI report view |
| Ahmet Furkan Tufan | Photo upload, device type detection, pull request reviews |

## Workflow

The project was developed with Jira Kanban cards, feature branches, pull
requests, and team review approvals before merge. The Jira project key is
`SHAP`.
