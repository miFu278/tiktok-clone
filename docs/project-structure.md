tiktok-clone/
├── README.md
├── docker-compose.yml
├── .env.example
├── docs/
│   ├── architecture.md
│   ├── api-specs/
│   └── grpc-contracts/
│
├── api-gateway/                          # .NET API Gateway (Main Entry Point)
│   ├── TikTok.Gateway.sln
│   ├── src/
│   │   └── TikTok.Gateway/
│   │       ├── Controllers/              # HTTP/REST endpoints
│   │       │   ├── VideoController.cs
│   │       │   ├── UserController.cs
│   │       │   ├── FeedController.cs
│   │       │   └── AuthController.cs
│   │       ├── Middleware/
│   │       │   ├── AuthenticationMiddleware.cs
│   │       │   ├── RateLimitingMiddleware.cs
│   │       │   └── ExceptionHandlingMiddleware.cs
│   │       ├── GrpcClients/              # gRPC client connections
│   │       │   ├── VideoServiceClient.cs
│   │       │   ├── FeedServiceClient.cs
│   │       │   ├── UserServiceClient.cs
│   │       │   └── CommentServiceClient.cs
│   │       ├── Models/
│   │       │   ├── Requests/
│   │       │   └── Responses/
│   │       ├── Configurations/
│   │       │   └── ServiceConfiguration.cs
│   │       ├── Program.cs
│   │       ├── appsettings.json
│   │       └── appsettings.Development.json
│   └── tests/
│       ├── TikTok.Gateway.UnitTests/
│       └── TikTok.Gateway.IntegrationTests/
│
├── services/
│   │
│   ├── dotnet/                           # .NET Microservices
│   │   │
│   │   ├── user-service/                 # User Management Service
│   │   │   ├── TikTok.UserService.sln
│   │   │   ├── src/
│   │   │   │   ├── TikTok.UserService.Domain/      # Enterprise Business Rules
│   │   │   │   │   ├── Entities/
│   │   │   │   │   │   ├── User.cs
│   │   │   │   │   │   ├── UserSettings.cs
│   │   │   │   │   │   └── UserStats.cs
│   │   │   │   │   ├── ValueObjects/
│   │   │   │   │   │   ├── Email.cs
│   │   │   │   │   │   └── Username.cs
│   │   │   │   │   ├── Enums/
│   │   │   │   │   │   └── UserStatus.cs
│   │   │   │   │   ├── Exceptions/
│   │   │   │   │   │   └── DomainException.cs
│   │   │   │   │   └── Interfaces/
│   │   │   │   │       └── IUserRepository.cs
│   │   │   │   │
│   │   │   │   ├── TikTok.UserService.Application/ # Application Business Rules
│   │   │   │   │   ├── DTOs/
│   │   │   │   │   │   ├── UserDto.cs
│   │   │   │   │   │   └── UserProfileDto.cs
│   │   │   │   │   ├── Interfaces/
│   │   │   │   │   │   ├── IUserService.cs
│   │   │   │   │   │   └── IAuthService.cs
│   │   │   │   │   ├── Services/
│   │   │   │   │   │   ├── UserService.cs
│   │   │   │   │   │   └── AuthService.cs
│   │   │   │   │   ├── UseCases/
│   │   │   │   │   │   ├── RegisterUser/
│   │   │   │   │   │   │   ├── RegisterUserCommand.cs
│   │   │   │   │   │   │   └── RegisterUserHandler.cs
│   │   │   │   │   │   ├── LoginUser/
│   │   │   │   │   │   ├── UpdateProfile/
│   │   │   │   │   │   └── FollowUser/
│   │   │   │   │   ├── Validators/
│   │   │   │   │   │   └── RegisterUserValidator.cs
│   │   │   │   │   └── Mappings/
│   │   │   │   │       └── UserMappingProfile.cs
│   │   │   │   │
│   │   │   │   ├── TikTok.UserService.Infrastructure/ # Frameworks & Drivers
│   │   │   │   │   ├── Persistence/
│   │   │   │   │   │   ├── ApplicationDbContext.cs
│   │   │   │   │   │   ├── Repositories/
│   │   │   │   │   │   │   ├── UserRepository.cs
│   │   │   │   │   │   │   └── FollowRepository.cs
│   │   │   │   │   │   ├── Configurations/
│   │   │   │   │   │   │   └── UserConfiguration.cs
│   │   │   │   │   │   └── Migrations/
│   │   │   │   │   ├── ExternalServices/
│   │   │   │   │   │   ├── EmailService.cs
│   │   │   │   │   │   └── SmsService.cs
│   │   │   │   │   ├── Caching/
│   │   │   │   │   │   └── RedisCacheService.cs
│   │   │   │   │   └── Identity/
│   │   │   │   │       ├── JwtTokenService.cs
│   │   │   │   │       └── PasswordHasher.cs
│   │   │   │   │
│   │   │   │   └── TikTok.UserService.API/          # Interface Adapters
│   │   │   │       ├── Protos/                      # gRPC definitions
│   │   │   │       │   └── user_service.proto
│   │   │   │       ├── Services/                    # gRPC service implementations
│   │   │   │       │   └── UserGrpcService.cs
│   │   │   │       ├── Program.cs
│   │   │   │       ├── appsettings.json
│   │   │   │       └── Dockerfile
│   │   │   │
│   │   │   └── tests/
│   │   │       ├── TikTok.UserService.UnitTests/
│   │   │       ├── TikTok.UserService.IntegrationTests/
│   │   │       └── TikTok.UserService.ArchitectureTests/
│   │   │
│   │   ├── content-moderation-service/   # Content Moderation Service
│   │   │   ├── TikTok.ModerationService.sln
│   │   │   ├── src/
│   │   │   │   ├── TikTok.ModerationService.Domain/
│   │   │   │   │   ├── Entities/
│   │   │   │   │   │   ├── ModerationCase.cs
│   │   │   │   │   │   └── ModerationRule.cs
│   │   │   │   │   └── Interfaces/
│   │   │   │   ├── TikTok.ModerationService.Application/
│   │   │   │   │   ├── Services/
│   │   │   │   │   │   ├── ContentAnalysisService.cs
│   │   │   │   │   │   └── ModerationService.cs
│   │   │   │   │   └── UseCases/
│   │   │   │   │       ├── AnalyzeVideo/
│   │   │   │   │       └── ReviewContent/
│   │   │   │   ├── TikTok.ModerationService.Infrastructure/
│   │   │   │   │   ├── Persistence/
│   │   │   │   │   ├── ML/
│   │   │   │   │   │   └── AzureComputerVisionClient.cs
│   │   │   │   │   └── ExternalServices/
│   │   │   │   └── TikTok.ModerationService.API/
│   │   │   │       ├── Protos/
│   │   │   │       │   └── moderation_service.proto
│   │   │   │       └── Services/
│   │   │   └── tests/
│   │   │
│   │   ├── analytics-service/            # Analytics & Reporting
│   │   │   └── [Similar structure]
│   │   │
│   │   ├── payment-service/              # Payment Processing
│   │   │   └── [Similar structure]
│   │   │
│   │   └── search-service/               # Search & Discovery
│   │       └── [Similar structure]
│   │
│   └── golang/                           # Golang Microservices
│       │
│       ├── video-service/                # Video Processing Service
│       │   ├── cmd/
│       │   │   └── server/
│       │   │       └── main.go
│       │   ├── internal/
│       │   │   ├── domain/               # Enterprise Business Rules
│       │   │   │   ├── entity/
│       │   │   │   │   ├── video.go
│       │   │   │   │   └── video_metadata.go
│       │   │   │   ├── valueobject/
│       │   │   │   │   └── video_quality.go
│       │   │   │   └── repository/
│       │   │   │       └── video_repository.go
│       │   │   │
│       │   │   ├── usecase/              # Application Business Rules
│       │   │   │   ├── video_usecase.go
│       │   │   │   ├── dto/
│       │   │   │   │   ├── video_dto.go
│       │   │   │   │   └── upload_request.go
│       │   │   │   └── interface.go
│       │   │   │
│       │   │   ├── infrastructure/       # Frameworks & Drivers
│       │   │   │   ├── persistence/
│       │   │   │   │   ├── postgres/
│       │   │   │   │   │   └── video_repository_impl.go
│       │   │   │   │   └── redis/
│       │   │   │   │       └── cache_repository.go
│       │   │   │   ├── storage/
│       │   │   │   │   ├── s3_client.go
│       │   │   │   │   └── minio_client.go
│       │   │   │   ├── transcoding/
│       │   │   │   │   └── ffmpeg_service.go
│       │   │   │   └── messaging/
│       │   │   │       └── rabbitmq_producer.go
│       │   │   │
│       │   │   └── delivery/             # Interface Adapters
│       │   │       ├── grpc/
│       │   │       │   ├── handler/
│       │   │       │   │   └── video_handler.go
│       │   │       │   └── proto/
│       │   │       │       └── video_service.proto
│       │   │       └── http/             # Optional REST endpoints
│       │   │           └── handler/
│       │   │
│       │   ├── pkg/                      # Shared utilities
│       │   │   ├── logger/
│       │   │   ├── config/
│       │   │   └── validator/
│       │   │
│       │   ├── config/
│       │   │   └── config.yaml
│       │   ├── Dockerfile
│       │   ├── go.mod
│       │   └── go.sum
│       │
│       ├── feed-service/                 # Feed & Recommendation
│       │   ├── cmd/
│       │   │   └── server/
│       │   │       └── main.go
│       │   ├── internal/
│       │   │   ├── domain/
│       │   │   │   ├── entity/
│       │   │   │   │   ├── feed_item.go
│       │   │   │   │   └── user_preference.go
│       │   │   │   └── repository/
│       │   │   ├── usecase/
│       │   │   │   ├── feed_usecase.go
│       │   │   │   ├── recommendation_usecase.go
│       │   │   │   └── dto/
│       │   │   ├── infrastructure/
│       │   │   │   ├── persistence/
│       │   │   │   │   ├── redis/
│       │   │   │   │   │   └── feed_cache.go
│       │   │   │   │   └── cassandra/
│       │   │   │   │       └── analytics_repository.go
│       │   │   │   └── algorithm/
│       │   │   │       ├── collaborative_filtering.go
│       │   │   │       └── content_based_filtering.go
│       │   │   └── delivery/
│       │   │       └── grpc/
│       │   │           ├── handler/
│       │   │           │   └── feed_handler.go
│       │   │           └── proto/
│       │   │               └── feed_service.proto
│       │   ├── pkg/
│       │   ├── config/
│       │   ├── Dockerfile
│       │   ├── go.mod
│       │   └── go.sum
│       │
│       ├── comment-service/              # Comments & Interactions
│       │   ├── cmd/
│       │   ├── internal/
│       │   │   ├── domain/
│       │   │   │   ├── entity/
│       │   │   │   │   └── comment.go
│       │   │   │   └── repository/
│       │   │   ├── usecase/
│       │   │   │   ├── comment_usecase.go
│       │   │   │   └── dto/
│       │   │   ├── infrastructure/
│       │   │   │   ├── persistence/
│       │   │   │   │   └── mongodb/
│       │   │   │   │       └── comment_repository_impl.go
│       │   │   │   └── cache/
│       │   │   └── delivery/
│       │   │       ├── grpc/
│       │   │       └── websocket/        # Real-time comments
│       │   │           └── handler.go
│       │   ├── pkg/
│       │   └── config/
│       │
│       ├── notification-service/         # Push Notifications
│       │   ├── cmd/
│       │   ├── internal/
│       │   │   ├── domain/
│       │   │   ├── usecase/
│       │   │   ├── infrastructure/
│       │   │   │   ├── persistence/
│       │   │   │   │   └── mongodb/
│       │   │   │   ├── push/
│       │   │   │   │   ├── fcm_client.go
│       │   │   │   │   └── apns_client.go
│       │   │   │   └── websocket/
│       │   │   └── delivery/
│       │   │       └── grpc/
│       │   └── pkg/
│       │
│       └── livestream-service/           # Live Streaming
│           ├── cmd/
│           ├── internal/
│           │   ├── domain/
│           │   ├── usecase/
│           │   ├── infrastructure/
│           │   │   ├── webrtc/
│           │   │   │   └── signaling_server.go
│           │   │   └── streaming/
│           │   │       └── media_server.go
│           │   └── delivery/
│           │       └── grpc/
│           └── pkg/
│
├── shared/                               # Shared Code
│   ├── proto/                            # Shared gRPC contracts
│   │   ├── common.proto
│   │   ├── user_service.proto
│   │   ├── video_service.proto
│   │   ├── feed_service.proto
│   │   └── comment_service.proto
│   │
│   ├── dotnet/                           # Shared .NET libraries
│   │   └── TikTok.Shared/
│   │       ├── TikTok.Shared.Common/
│   │       │   ├── Extensions/
│   │       │   ├── Helpers/
│   │       │   └── Constants/
│   │       ├── TikTok.Shared.Logging/
│   │       └── TikTok.Shared.Monitoring/
│   │
│   └── golang/                           # Shared Go packages
│       ├── common/
│       │   ├── errors/
│       │   ├── logger/
│       │   └── config/
│       ├── middleware/
│       │   ├── auth.go
│       │   ├── logging.go
│       │   └── tracing.go
│       └── utils/
│
├── infrastructure/                       # Infrastructure as Code
│   ├── kubernetes/
│   │   ├── api-gateway/
│   │   │   ├── deployment.yaml
│   │   │   ├── service.yaml
│   │   │   └── ingress.yaml
│   │   ├── services/
│   │   │   ├── user-service/
│   │   │   ├── video-service/
│   │   │   └── feed-service/
│   │   ├── databases/
│   │   │   ├── postgres/
│   │   │   ├── mongodb/
│   │   │   ├── redis/
│   │   │   └── cassandra/
│   │   └── monitoring/
│   │       ├── prometheus/
│   │       └── grafana/
│   │
│   ├── terraform/
│   │   ├── aws/
│   │   │   ├── s3.tf
│   │   │   ├── rds.tf
│   │   │   └── eks.tf
│   │   └── modules/
│   │
│   └── helm/
│       └── tiktok-clone/
│           ├── Chart.yaml
│           ├── values.yaml
│           └── templates/
│
├── scripts/
│   ├── build-all.sh
│   ├── deploy.sh
│   ├── seed-data.sh
│   └── migration/
│       ├── postgres/
│       └── mongodb/
│
└── monitoring/
    ├── prometheus/
    │   └── prometheus.yml
    ├── grafana/
    │   └── dashboards/
    └── jaeger/
        └── jaeger-config.yaml