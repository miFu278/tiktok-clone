# Video Service

Video processing and management microservice built with Go.

## Features

- Video upload and storage (S3/MinIO)
- Video transcoding (FFmpeg)
- Video metadata management
- View counting and analytics
- Trending videos algorithm
- gRPC API for internal communication

## Architecture

This service follows Clean Architecture principles:

```
internal/
├── domain/          # Enterprise Business Rules
│   ├── entity/      # Domain entities
│   └── repository/  # Repository interfaces
├── usecase/         # Application Business Rules
│   └── dto/         # Data Transfer Objects
├── infrastructure/  # Frameworks & Drivers
│   ├── persistence/ # Database implementations
│   ├── storage/     # File storage (S3)
│   └── transcoding/ # Video processing
└── delivery/        # Interface Adapters
    └── grpc/        # gRPC handlers
```

## Setup

1. Copy environment file:
```bash
cp .env.example .env
```

2. Update environment variables in `.env`

3. Run the service:
```bash
go run cmd/server/main.go
```

## Dependencies

- PostgreSQL - Video metadata storage
- Redis - Caching and real-time counters
- S3/MinIO - Video file storage
- RabbitMQ/Kafka - Async job processing

## gRPC Endpoints

- `UploadVideo` - Upload new video
- `GetVideo` - Get video by ID
- `GetUserVideos` - Get videos by user
- `UpdateVideo` - Update video metadata
- `DeleteVideo` - Delete video
- `GetTrendingVideos` - Get trending videos
- `RecordView` - Record video view

## Environment Variables

See `.env.example` for all available configuration options.
