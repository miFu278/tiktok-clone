# Requirements Document

## Introduction

Tạo một shared library cho các Golang microservices trong hệ thống TikTok clone. Library này sẽ cung cấp các utilities, middleware, và components chung được sử dụng bởi tất cả các Golang services (Video Service, Feed Service, Comment Service, Notification Service, Live Streaming Service).

## Glossary

- **Shared Library**: Thư viện code dùng chung giữa các microservices
- **Middleware**: Các hàm xử lý trung gian trong request pipeline
- **gRPC**: Framework RPC hiệu năng cao cho inter-service communication
- **Logger**: Component ghi log có cấu trúc
- **Config**: Cấu hình ứng dụng từ environment variables hoặc files
- **Error Handler**: Component xử lý và format errors một cách nhất quán
- **Validator**: Component validate dữ liệu đầu vào
- **JWT**: JSON Web Token cho authentication
- **Tracing**: Distributed tracing để theo dõi requests qua nhiều services
- **Metrics**: Thu thập số liệu về performance và health của services

## Requirements

### Requirement 1

**User Story:** Là một developer, tôi muốn có một logging system nhất quán, để tất cả các services có thể ghi log theo cùng một format và level.

#### Acceptance Criteria

1. WHEN a service initializes the logger THEN the system SHALL create a structured logger with configurable output format
2. WHEN a service logs a message THEN the system SHALL include timestamp, level, service name, and contextual fields
3. WHEN the log level is configured THEN the system SHALL filter messages below the configured level
4. WHEN logging in production THEN the system SHALL output JSON format for log aggregation tools
5. WHEN logging in development THEN the system SHALL output human-readable format with colors

### Requirement 2

**User Story:** Là một developer, tôi muốn có configuration management, để services có thể load settings từ environment variables và config files một cách dễ dàng.

#### Acceptance Criteria

1. WHEN a service starts THEN the system SHALL load configuration from environment variables
2. WHEN a config file exists THEN the system SHALL merge file-based config with environment variables
3. WHEN an environment variable is set THEN the system SHALL override the corresponding config file value
4. WHEN a required config is missing THEN the system SHALL return a validation error with the missing field name
5. WHEN config is loaded THEN the system SHALL validate all required fields and data types

### Requirement 3

**User Story:** Là một developer, tôi muốn có error handling utilities, để tất cả services xử lý errors theo cùng một pattern và trả về consistent error responses.

#### Acceptance Criteria

1. WHEN an error occurs THEN the system SHALL wrap the error with context information
2. WHEN a domain error is created THEN the system SHALL include error code, message, and HTTP status code
3. WHEN an error is converted to gRPC status THEN the system SHALL map domain errors to appropriate gRPC status codes
4. WHEN an error is logged THEN the system SHALL include stack trace for debugging
5. WHEN multiple errors occur THEN the system SHALL aggregate them into a single error response

### Requirement 4

**User Story:** Là một developer, tôi muốn có validation utilities, để validate input data theo các rules chung trước khi xử lý business logic.

#### Acceptance Criteria

1. WHEN validating a struct THEN the system SHALL check all validation tags and return errors for invalid fields
2. WHEN a required field is missing THEN the system SHALL return an error indicating the field name
3. WHEN a field value is invalid THEN the system SHALL return an error with the validation rule that failed
4. WHEN validating email format THEN the system SHALL verify the string matches email pattern
5. WHEN validating string length THEN the system SHALL check min and max length constraints

### Requirement 5

**User Story:** Là một developer, tôi muốn có gRPC middleware, để xử lý cross-cutting concerns như authentication, logging, và error handling cho tất cả gRPC endpoints.

#### Acceptance Criteria

1. WHEN a gRPC request is received THEN the system SHALL log the request method and metadata
2. WHEN a gRPC request completes THEN the system SHALL log the response status and duration
3. WHEN a gRPC request fails THEN the system SHALL convert domain errors to gRPC status codes
4. WHEN authentication is required THEN the system SHALL validate JWT token from metadata
5. WHEN a panic occurs in handler THEN the system SHALL recover and return Internal error status

### Requirement 6

**User Story:** Là một developer, tôi muốn có JWT utilities, để authenticate và authorize requests từ API Gateway và giữa các services.

#### Acceptance Criteria

1. WHEN validating a JWT token THEN the system SHALL verify signature using the configured secret key
2. WHEN a token is expired THEN the system SHALL return an authentication error
3. WHEN extracting claims from token THEN the system SHALL parse and return user ID, roles, and custom claims
4. WHEN token signature is invalid THEN the system SHALL return an authentication error
5. WHEN required claims are missing THEN the system SHALL return a validation error

### Requirement 7

**User Story:** Là một developer, tôi muốn có database utilities, để connect và interact với PostgreSQL, MongoDB, và Redis một cách nhất quán.

#### Acceptance Criteria

1. WHEN connecting to PostgreSQL THEN the system SHALL establish connection pool with configurable size
2. WHEN connecting to MongoDB THEN the system SHALL create client with timeout and retry settings
3. WHEN connecting to Redis THEN the system SHALL create client with connection pooling
4. WHEN a database operation fails THEN the system SHALL retry with exponential backoff
5. WHEN closing connections THEN the system SHALL gracefully shutdown all database clients

### Requirement 8

**User Story:** Là một developer, tôi muốn có distributed tracing support, để track requests qua nhiều services và debug performance issues.

#### Acceptance Criteria

1. WHEN a request enters a service THEN the system SHALL extract or create a trace ID
2. WHEN calling another service THEN the system SHALL propagate trace context in metadata
3. WHEN a span is created THEN the system SHALL include service name, operation name, and timestamps
4. WHEN an error occurs THEN the system SHALL mark the span as error with details
5. WHEN tracing is disabled THEN the system SHALL use no-op tracer without performance impact

### Requirement 9

**User Story:** Là một developer, tôi muốn có metrics collection, để monitor service health, performance, và business metrics.

#### Acceptance Criteria

1. WHEN a service starts THEN the system SHALL register default metrics for CPU, memory, and goroutines
2. WHEN a gRPC request is processed THEN the system SHALL record request count, duration, and status
3. WHEN recording a custom metric THEN the system SHALL support counters, gauges, and histograms
4. WHEN metrics endpoint is called THEN the system SHALL expose metrics in Prometheus format
5. WHEN a metric has labels THEN the system SHALL allow filtering and aggregation by label values

### Requirement 10

**User Story:** Là một developer, tôi muốn có HTTP utilities, để các services có thể expose health check và metrics endpoints.

#### Acceptance Criteria

1. WHEN health check endpoint is called THEN the system SHALL return service status and dependency health
2. WHEN a dependency is unhealthy THEN the system SHALL return degraded status with details
3. WHEN metrics endpoint is called THEN the system SHALL return Prometheus-formatted metrics
4. WHEN graceful shutdown is triggered THEN the system SHALL stop accepting new requests and drain existing ones
5. WHEN shutdown timeout is reached THEN the system SHALL force close remaining connections

### Requirement 11

**User Story:** Là một developer, tôi muốn có context utilities, để pass request-scoped data như user ID, trace ID, và timeout qua các layers.

#### Acceptance Criteria

1. WHEN extracting user ID from context THEN the system SHALL return the authenticated user ID or empty string
2. WHEN extracting trace ID from context THEN the system SHALL return the distributed trace ID
3. WHEN setting a value in context THEN the system SHALL create a new context with the value
4. WHEN context deadline is exceeded THEN the system SHALL cancel ongoing operations
5. WHEN context is cancelled THEN the system SHALL propagate cancellation to child contexts

### Requirement 12

**User Story:** Là một developer, tôi muốn có string utilities, để xử lý các operations phổ biến như slugify, truncate, và sanitize.

#### Acceptance Criteria

1. WHEN generating a slug from text THEN the system SHALL convert to lowercase, replace spaces with hyphens, and remove special characters
2. WHEN truncating a string THEN the system SHALL limit length and append ellipsis if truncated
3. WHEN sanitizing HTML THEN the system SHALL remove or escape dangerous tags and attributes
4. WHEN checking if string is empty THEN the system SHALL trim whitespace before checking
5. WHEN generating random string THEN the system SHALL use cryptographically secure random source

### Requirement 13

**User Story:** Là một developer, tôi muốn có time utilities, để xử lý timezone conversions và formatting một cách nhất quán.

#### Acceptance Criteria

1. WHEN formatting a timestamp THEN the system SHALL use ISO 8601 format by default
2. WHEN parsing a timestamp string THEN the system SHALL handle multiple common formats
3. WHEN converting timezone THEN the system SHALL preserve the absolute time point
4. WHEN calculating duration THEN the system SHALL return human-readable format
5. WHEN getting current time THEN the system SHALL use UTC by default

### Requirement 14

**User Story:** Là một developer, tôi muốn có pagination utilities, để implement consistent pagination across all list endpoints.

#### Acceptance Criteria

1. WHEN parsing pagination params THEN the system SHALL extract page number and page size from request
2. WHEN page size exceeds maximum THEN the system SHALL cap it to the configured maximum
3. WHEN calculating offset THEN the system SHALL compute correct offset from page number and size
4. WHEN building pagination response THEN the system SHALL include total count, current page, and total pages
5. WHEN no results exist THEN the system SHALL return empty list with zero total count

### Requirement 15

**User Story:** Là một developer, tôi muốn có rate limiting utilities, để protect services from abuse và ensure fair usage.

#### Acceptance Criteria

1. WHEN checking rate limit THEN the system SHALL use Redis to track request counts per user or IP
2. WHEN rate limit is exceeded THEN the system SHALL return error with retry-after header
3. WHEN rate limit window resets THEN the system SHALL allow new requests
4. WHEN configuring rate limits THEN the system SHALL support different limits per endpoint or user tier
5. WHEN rate limiter is unavailable THEN the system SHALL fail open and allow requests
