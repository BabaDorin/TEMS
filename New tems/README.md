# TEMS - Total Equipment Management System

## Quick Start

```bash
# Start all services
./start-tems.sh

# Stop all services
./stop-tems.sh
```

Visit http://localhost:4200 to access the application.

## Documentation

- **[QUICKSTART.md](./QUICKSTART.md)** - Getting started guide
- **[AUTH_SETUP.md](./AUTH_SETUP.md)** - Complete authentication architecture documentation
- **[IMPLEMENTATION_SUMMARY.md](./IMPLEMENTATION_SUMMARY.md)** - Implementation details and checklist

## Architecture

TEMS uses a modern microservices architecture with:

- **Frontend**: Angular 20 with Tailwind CSS
- **API Gateway**: Keycloak (OIDC/OAuth 2.0)
- **Identity Provider**: Duende IdentityServer
- **Backend API**: ASP.NET Core 9 with FastEndpoints
- **Database**: MongoDB 7.0

## Services

| Service | URL | Purpose |
|---------|-----|---------|
| Frontend | http://localhost:4200 | Angular SPA |
| Keycloak | http://localhost:8080 | Authentication gateway |
| IdentityServer | http://localhost:5001 | User management |
| Backend API | http://localhost:14721 | Business logic |
| MongoDB | mongodb://localhost:27017 | Data storage |

## Features

### ✅ Completed
- User registration and authentication
- Role-based authorization
- Claim-based permissions
- JWT token management
- Token refresh with sliding expiration
- OIDC authentication flow with PKCE
- Beautiful iOS-style UI with Tailwind CSS
- Comprehensive documentation
- Automated startup/shutdown scripts

### 🚀 Coming Soon
- Email verification
- Password reset
- Two-factor authentication
- User profile management
- Admin dashboard

## Development

### Prerequisites
- Docker & Docker Compose
- .NET 9 SDK
- Node.js v20.19.5
- MongoDB client (optional)

### First Time Setup

1. Clone the repository
2. Run `./start-tems.sh`
3. Wait for services to start (2-3 minutes)
4. Open http://localhost:4200
5. Register a new user
6. Start developing!

### Project Structure

```
TEMS/
├── Backend/
│   └── Tems/
│       ├── Tems.Host/              # Backend API
│       ├── Tems.IdentityServer/    # User authentication
│       ├── Tems.Common/            # Shared libraries
│       ├── Modules/                # Feature modules
│       └── compose.yaml            # Docker services
├── Frontend/
│   └── Tems/                       # Angular application
├── Infrastructure/
│   └── Keycloak/                   # Keycloak setup (Pulumi)
├── start-tems.sh                   # Start all services
├── stop-tems.sh                    # Stop all services
└── Documentation files
```

## Contributing

1. Create a feature branch
2. Make your changes
3. Test locally with `./start-tems.sh`
4. Submit a pull request

## License

Proprietary - All Rights Reserved

## Support

For issues or questions, see [AUTH_SETUP.md](./AUTH_SETUP.md) troubleshooting section.

---

Built with ❤️ using Angular, .NET, and Keycloak
