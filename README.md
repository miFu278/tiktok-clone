# TikTok Clone - Microservices Architecture

A scalable TikTok clone built with microservices architecture using .NET and Golang.

## Architecture Overview

```
Mobile/Web Clients
      ↓ (HTTP/REST)
.NET API Gateway
      ↓ (gRPC)
┌─────────┴─────────┐
↓                   ↓
Golang Services    .NET Services
```

## Services

### Golang Services (High Performance)
- **Video Service** - Video processing & streaming
- **Feed Service** - Content recommendation
- **Comment Service** - Real-time interactions
- **Notification Service** - Push notifications
- **Live Streaming Service** - Real-time streaming

### .NET Services (Business Logic)
- **User Service** - User management & auth
- **Content Moderation Service** - AI/ML content filtering
- **Analytics Service** - Business intelligence
- **Payment Service** - Transaction processing
- **Search Service** - Full-text search

### API Gateway (.NET)
- REST API for clients
- Authentication & authorization
- Rate limiting
- Request routing to microservices

## Tech Stack

### Backend
- **Golang** 1.21+ - High-performance services
- **.NET 8** - Business logic services
- **gRPC** - Inter-service communication
- **PostgreSQL** - Primary database
- **MongoDB** - Document storage
- **Redis** - Caching & real-time data
- **RabbitMQ/Kafka** - Message queue

### Infrastructure
- **Docker** - Containerization
- **Kubernetes** - Orchestration
- **Prometheus & Grafana** - Monitoring
- **Jaeger** - Distributed tracing

## Project Structure

```
tiktok-clone/
├── api-gateway/              # .NET API Gateway
├── services/
│   ├── dotnet/              # .NET microservices
│   │   ├── user-service/
│   │   └── content-moderation-service/
│   └── golang/              # Golang microservices
│       └── video-service/
├── shared/
│   ├── proto/               # gRPC contracts
│   ├── dotnet/              # Shared .NET libraries
│   └── golang/              # Shared Go packages
└── infrastructure/          # IaC & K8s configs
```

## Getting Started

### Prerequisites
- Go 1.21+
- .NET 8 SDK
- Docker & Docker Compose
- PostgreSQL
- Redis

### Development Setup

1. Clone the repository:
```bash
git clone <repository-url>
cd tiktok-clone
```

2. Start infrastructure services:
```bash
docker-compose up -d postgres redis rabbitmq
```

3. Run Golang services:
```bash
cd services/golang/video-service
cp .env.example .env
go run cmd/server/main.go
```

4. Run .NET services:
```bash
cd services/dotnet/user-service
dotnet run --project src/TikTok.UserService.API
```

5. Run API Gateway:
```bash
cd api-gateway
dotnet run
```

## Go Workspace

This project uses Go workspace for managing multiple Go modules:

```bash
# Sync workspace
go work sync

# Run tests across all modules
go test ./...
```

## Documentation

- [Project Requirements](docs/project-requirements.md)
- [Project Structure](docs/project-structure.md)
- [Database Schema](docs/db-schena.sql)

## Contributing

1. Create a feature branch
2. Make your changes
3. Write tests
4. Submit a pull request

## License

MIT License
