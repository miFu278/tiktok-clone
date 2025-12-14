# TikTok Clone - Project Requirements

## Overview
A scalable TikTok clone built with microservices architecture, leveraging both .NET and Golang for optimal performance and maintainability.

## Architecture Approach

### Microservices Strategy
- **Polyglot Architecture**: Use the right tool for the right job
- **API Gateway Pattern**: Centralized entry point for all client requests
- **gRPC for Internal Communication**: High-performance inter-service communication
- **REST API for Clients**: Standard HTTP/REST for mobile and web clients

---

## Service Distribution by Technology

### Golang Services
**Purpose**: Real-time processing, high throughput, I/O intensive operations

| Service | Responsibility | Key Features |
|---------|---------------|--------------|
| **Video Service** | Video processing & streaming | Transcoding, adaptive bitrate, HLS/DASH |
| **Feed Service** | Content recommendation | ML-based recommendations, millions req/sec |
| **Live Streaming Service** | Real-time streaming | WebRTC signaling, live chat, low latency |
| **Comment Service** | Real-time interactions | High concurrency, WebSocket support |
| **Notification Service** | Push notifications | FCM/APNS integration, WebSocket connections |

**Why Golang?**
- Excellent concurrency with goroutines
- High performance for I/O operations
- Low memory footprint
- Fast compilation and deployment

### .NET Services
**Purpose**: Complex business logic, enterprise features, security-critical operations

| Service | Responsibility | Key Features |
|---------|---------------|--------------|
| **User Service** | User management & auth | JWT, OAuth2, role-based access control |
| **Content Moderation Service** | AI/ML content filtering | Azure Cognitive Services, custom ML models |
| **Analytics Service** | Business intelligence | Data aggregation, reporting, dashboards |
| **Payment Service** | Transaction processing | Stripe/PayPal integration, billing |
| **Search Service** | Full-text search | Elasticsearch integration, fuzzy search |

**Why .NET?**
- Rich ecosystem (Entity Framework, LINQ)
- Strong typing and compile-time safety
- Excellent Azure integration
- Enterprise-grade security features
- Mature dependency injection

---

## API Gateway

### .NET API Gateway (Main Entry Point)

**Responsibilities:**
- Expose REST APIs to mobile/web clients
- Centralized authentication & authorization
- Rate limiting & throttling
- Request routing to microservices
- API versioning
- Response aggregation

**Technology Stack:**
- ASP.NET Core
- YARP (Yet Another Reverse Proxy) or Ocelot
- JWT Bearer Authentication
- Swagger/OpenAPI documentation

**Why .NET for Gateway?**
- Unified authentication with Identity Server
- Better REST API tooling
- Easy middleware implementation
- Strong validation frameworks
- Excellent monitoring integration

---

## Communication Flow

### Architecture Diagram
```
Mobile/Web Clients
      ↓ (HTTP/REST)
.NET API Gateway
      ↓ (gRPC)
┌─────────┴─────────┐
↓                   ↓
Golang Services    .NET Services
(gRPC Servers)     (gRPC Servers)
```

### Example Flows

#### 1. Video Upload Flow
```
1. Client → REST POST /api/videos → API Gateway
2. Gateway → gRPC → User Service (validate auth)
3. Gateway → gRPC → Video Service (process upload)
4. Video Service → Transcoding (async)
5. Video Service → gRPC → Content Moderation Service
6. Store metadata → PostgreSQL
7. Upload video → S3/MinIO
8. Response → Client
```

#### 2. Feed Generation Flow
```
1. Client → REST GET /api/feed → API Gateway
2. Gateway → gRPC → User Service (get preferences)
3. Gateway → gRPC → Feed Service (get recommendations)
4. Feed Service → Redis Cache (check)
5. Feed Service → PostgreSQL (fetch videos)
6. Response → Client
```

#### 3. Real-time Comment Flow
```
1. Client → REST POST /api/comments → API Gateway
2. Gateway → gRPC → User Service (validate)
3. Gateway → gRPC → Comment Service (store)
4. Comment Service → WebSocket → Broadcast to viewers
5. Comment Service → MongoDB (persist)
```

---

## Database Architecture

### Primary Databases

| Database | Use Case | Data Types |
|----------|----------|------------|
| **PostgreSQL** | Primary relational data | Users, videos, relationships, transactions |
| **MongoDB** | Document storage | Comments, notifications, logs, user activities |
| **Redis** | Caching & real-time | Sessions, feed cache, leaderboards, rate limiting |
| **Cassandra/ScyllaDB** | Time-series data | Analytics events, view counts, engagement metrics |

### Specialized Storage

| Storage | Purpose |
|---------|---------|
| **S3/MinIO** | Video files, thumbnails, user avatars |
| **Elasticsearch** | Full-text search indexing |
| **Neo4j** (Optional) | Social graph (followers/following) |

### Database Selection Rationale

**PostgreSQL:**
- ACID compliance for critical data
- Strong relational integrity
- JSON support for flexible schemas
- Excellent performance with proper indexing

**MongoDB:**
- Flexible schema for evolving data
- High write throughput
- Good for nested documents (comments with replies)

**Redis:**
- Sub-millisecond latency
- Perfect for caching and sessions
- Pub/Sub for real-time features

**Cassandra:**
- Linear scalability
- High write throughput
- Perfect for analytics and time-series

---

## Infrastructure Requirements

### Message Queue
- **RabbitMQ** or **Kafka**
- Use cases: Video transcoding jobs, email notifications, analytics events

### Service Mesh (Optional)
- **Istio** or **Linkerd**
- Features: Service discovery, load balancing, circuit breaking, observability

### Monitoring & Observability
- **Prometheus** - Metrics collection
- **Grafana** - Visualization dashboards
- **Jaeger** - Distributed tracing
- **ELK Stack** - Centralized logging

### Container Orchestration
- **Docker** - Containerization
- **Kubernetes** - Orchestration
- **Helm** - Package management

---

## Non-Functional Requirements

### Performance
- API response time < 200ms (p95)
- Video feed load time < 1s
- Support 10,000+ concurrent users per service
- Video transcoding < 5 minutes for 1-minute video

### Scalability
- Horizontal scaling for all services
- Auto-scaling based on CPU/memory metrics
- Database read replicas for high traffic

### Security
- JWT-based authentication
- OAuth2 for third-party integrations
- HTTPS/TLS for all communications
- Rate limiting per user/IP
- Input validation and sanitization
- SQL injection prevention
- XSS protection

### Reliability
- 99.9% uptime SLA
- Circuit breakers for fault tolerance
- Graceful degradation
- Database backups (daily)
- Disaster recovery plan

### Observability
- Distributed tracing across services
- Centralized logging
- Real-time metrics and alerts
- Health check endpoints

---

## Development Guidelines

### Code Quality
- Clean Architecture principles
- SOLID principles
- Domain-Driven Design (DDD)
- Unit test coverage > 80%
- Integration tests for critical flows

### API Standards
- RESTful conventions
- Consistent error responses
- API versioning (v1, v2)
- Comprehensive OpenAPI documentation
- Pagination for list endpoints

### Git Workflow
- Feature branch workflow
- Pull request reviews required
- CI/CD pipeline for automated testing
- Semantic versioning

---

## Future Considerations

### Phase 2 Features
- AI-powered content recommendations
- Live streaming with multiple hosts
- E-commerce integration
- Advanced analytics dashboard
- Multi-language support

### Scalability Enhancements
- CDN integration (CloudFlare, Akamai)
- Edge computing for video processing
- Multi-region deployment
- Database sharding strategies

---

## Success Metrics

### Technical KPIs
- API latency (p50, p95, p99)
- Error rate < 0.1%
- Service uptime > 99.9%
- Database query performance

### Business KPIs
- Daily Active Users (DAU)
- Video upload rate
- User engagement (likes, comments, shares)
- Average session duration
- Video completion rate
