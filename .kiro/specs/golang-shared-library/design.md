# Design Document

## Overview

Golang shared library sẽ cung cấp các packages tái sử dụng cho tất cả các Golang microservices trong hệ thống TikTok clone. Library được thiết kế theo nguyên tắc modular, cho phép services chỉ import những packages cần thiết. Mỗi package sẽ có interface rõ ràng, dễ test, và tuân theo Go best practices.

## Architecture

### Package Structure

```
shared/golang/
├── pkg/
│   ├── logger/          # Structured logging
│   ├── config/          # Configuration management
│   ├── errors/          # Error handling utilities
│   ├── validator/       # Input validation
│   ├── middleware/      # gRPC & HTTP middleware
│   ├── auth/            # JWT authentication
│   ├── database/        # Database clients
│   ├── tracing/         # Distributed tracing
│   ├── metrics/         # Metrics collection
│   ├── httputil/        # HTTP utilities
│   ├── contextutil/     # Context utilities
│   ├── stringutil/      # String utilities
│   ├── timeutil/        # Time utilities
│   ├── pagination/      # Pagination helpers
│   └── ratelimit/       # Rate limiting
├── go.mod
└── go.sum
```

### Design Principles

1. **Zero Dependencies Between Packages**: Mỗi package độc lập, không phụ thuộc vào packages khác trong shared library
2. **Interface-Based Design**: Expose interfaces để dễ mock và test
3. **Configuration via Options Pattern**: Sử dụng functional options cho flexibility
4. **Context-Aware**: Tất cả operations nhận context.Context để support cancellation và timeouts
5. **Error Wrapping**: Sử dụng error wrapping để preserve error chain
6. **Thread-Safe**: Tất cả components đều thread-safe

## Components and Interfaces

### 1. Logger Package

**Purpose**: Structured logging với support cho multiple output formats

**Key Types**:
```go
type Logger interface {
    Debug(msg string, fields ...Field)
    Info(msg string, fields ...Field)
    Warn(msg string, fields ...Field)
    Error(msg string, fields ...Field)
    Fatal(msg string, fields ...Field)
    With(fields ...Field) Logger
}

type Field struct {
    Key   string
    Value interface{}
}

type Config struct {
    Level      string // debug, info, warn, error
    Format     string // json, console
    Output     string // stdout, stderr, file
    FilePath   string
    ServiceName string
}
```

**Implementation**: Sử dụng `zap` hoặc `zerolog` làm underlying logger

### 2. Config Package

**Purpose**: Load và validate configuration từ environment variables và files

**Key Types**:
```go
type Loader interface {
    Load(cfg interface{}) error
    LoadFromFile(path string, cfg interface{}) error
    Get(key string) (string, bool)
    MustGet(key string) string
}

type Config struct {
    EnvPrefix string
    FilePath  string
    FileType  string // yaml, json, toml
}
```

**Implementation**: Sử dụng `viper` cho config management

### 3. Errors Package

**Purpose**: Consistent error handling và error codes

**Key Types**:
```go
type AppError struct {
    Code       string
    Message    string
    HTTPStatus int
    GRPCStatus codes.Code
    Err        error
    Fields     map[string]interface{}
}

func (e *AppError) Error() string
func (e *AppError) Unwrap() error
func (e *AppError) WithField(key string, value interface{}) *AppError

// Constructors
func New(code, message string) *AppError
func Wrap(err error, message string) *AppError
func NotFound(resource string) *AppError
func BadRequest(message string) *AppError
func Unauthorized(message string) *AppError
func Internal(err error) *AppError
```

**Error Codes**: Định nghĩa constants cho common error codes

### 4. Validator Package

**Purpose**: Validate structs và individual values

**Key Types**:
```go
type Validator interface {
    Validate(v interface{}) error
    ValidateVar(field interface{}, tag string) error
}

type ValidationError struct {
    Field   string
    Tag     string
    Value   interface{}
    Message string
}
```

**Implementation**: Sử dụng `go-playground/validator`

### 5. Middleware Package

**Purpose**: gRPC và HTTP middleware cho cross-cutting concerns

**Key Functions**:
```go
// gRPC Interceptors
func UnaryLoggingInterceptor(logger Logger) grpc.UnaryServerInterceptor
func UnaryAuthInterceptor(jwtService JWTService) grpc.UnaryServerInterceptor
func UnaryRecoveryInterceptor(logger Logger) grpc.UnaryServerInterceptor
func UnaryTracingInterceptor(tracer Tracer) grpc.UnaryServerInterceptor
func UnaryMetricsInterceptor(metrics MetricsCollector) grpc.UnaryServerInterceptor

// HTTP Middleware
func LoggingMiddleware(logger Logger) func(http.Handler) http.Handler
func RecoveryMiddleware(logger Logger) func(http.Handler) http.Handler
func CORSMiddleware(config CORSConfig) func(http.Handler) http.Handler
```

### 6. Auth Package

**Purpose**: JWT token validation và claims extraction

**Key Types**:
```go
type JWTService interface {
    ValidateToken(tokenString string) (*Claims, error)
    ExtractClaims(tokenString string) (*Claims, error)
}

type Claims struct {
    UserID    string
    Username  string
    Roles     []string
    ExpiresAt int64
    IssuedAt  int64
}

type Config struct {
    SecretKey     string
    Issuer        string
    Audience      string
    AllowedIssuers []string
}
```

**Implementation**: Sử dụng `golang-jwt/jwt`

### 7. Database Package

**Purpose**: Database connection management

**Subpackages**:
- `database/postgres`: PostgreSQL client
- `database/mongodb`: MongoDB client  
- `database/redis`: Redis client

**Key Types**:
```go
// PostgreSQL
type PostgresClient interface {
    DB() *sql.DB
    Close() error
    Ping(ctx context.Context) error
}

type PostgresConfig struct {
    Host            string
    Port            int
    User            string
    Password        string
    Database        string
    MaxOpenConns    int
    MaxIdleConns    int
    ConnMaxLifetime time.Duration
}

// MongoDB
type MongoClient interface {
    Database(name string) *mongo.Database
    Close(ctx context.Context) error
    Ping(ctx context.Context) error
}

type MongoConfig struct {
    URI             string
    Database        string
    Timeout         time.Duration
    MaxPoolSize     uint64
    MinPoolSize     uint64
}

// Redis
type RedisClient interface {
    Get(ctx context.Context, key string) (string, error)
    Set(ctx context.Context, key string, value interface{}, expiration time.Duration) error
    Del(ctx context.Context, keys ...string) error
    Close() error
}

type RedisConfig struct {
    Addr         string
    Password     string
    DB           int
    PoolSize     int
    MinIdleConns int
}
```

### 8. Tracing Package

**Purpose**: Distributed tracing integration

**Key Types**:
```go
type Tracer interface {
    StartSpan(ctx context.Context, operationName string) (context.Context, Span)
    Extract(ctx context.Context, carrier interface{}) (context.Context, error)
    Inject(ctx context.Context, carrier interface{}) error
    Close() error
}

type Span interface {
    SetTag(key string, value interface{})
    SetError(err error)
    Finish()
}

type Config struct {
    ServiceName string
    AgentHost   string
    AgentPort   int
    SamplingRate float64
    Enabled     bool
}
```

**Implementation**: Sử dụng OpenTelemetry hoặc Jaeger client

### 9. Metrics Package

**Purpose**: Metrics collection và exposure

**Key Types**:
```go
type MetricsCollector interface {
    IncrementCounter(name string, labels map[string]string)
    SetGauge(name string, value float64, labels map[string]string)
    ObserveHistogram(name string, value float64, labels map[string]string)
    RegisterCounter(name, help string, labels []string) error
    RegisterGauge(name, help string, labels []string) error
    RegisterHistogram(name, help string, labels []string, buckets []float64) error
}

type Config struct {
    Enabled     bool
    ServiceName string
    Namespace   string
    Port        int
}
```

**Implementation**: Sử dụng Prometheus client

### 10. HTTPUtil Package

**Purpose**: HTTP server utilities

**Key Types**:
```go
type HealthChecker interface {
    AddCheck(name string, check HealthCheckFunc)
    Handler() http.HandlerFunc
}

type HealthCheckFunc func(ctx context.Context) error

type HealthStatus struct {
    Status      string                 // healthy, degraded, unhealthy
    Timestamp   time.Time
    Checks      map[string]CheckResult
}

type CheckResult struct {
    Status  string
    Message string
    Error   string
}

type Server struct {
    httpServer *http.Server
    logger     Logger
}

func (s *Server) Start(addr string) error
func (s *Server) Shutdown(ctx context.Context) error
```

### 11. ContextUtil Package

**Purpose**: Context helpers

**Key Functions**:
```go
func WithUserID(ctx context.Context, userID string) context.Context
func GetUserID(ctx context.Context) string

func WithTraceID(ctx context.Context, traceID string) context.Context
func GetTraceID(ctx context.Context) string

func WithRequestID(ctx context.Context, requestID string) context.Context
func GetRequestID(ctx context.Context) string

func WithTimeout(ctx context.Context, timeout time.Duration) (context.Context, context.CancelFunc)
```

### 12. StringUtil Package

**Purpose**: String manipulation utilities

**Key Functions**:
```go
func Slugify(s string) string
func Truncate(s string, maxLen int) string
func SanitizeHTML(s string) string
func IsEmpty(s string) bool
func RandomString(length int) string
func ToSnakeCase(s string) string
func ToCamelCase(s string) string
```

### 13. TimeUtil Package

**Purpose**: Time handling utilities

**Key Functions**:
```go
func FormatISO8601(t time.Time) string
func ParseISO8601(s string) (time.Time, error)
func ParseFlexible(s string) (time.Time, error)
func ConvertTimezone(t time.Time, tz string) (time.Time, error)
func FormatDuration(d time.Duration) string
func NowUTC() time.Time
func StartOfDay(t time.Time) time.Time
func EndOfDay(t time.Time) time.Time
```

### 14. Pagination Package

**Purpose**: Pagination helpers

**Key Types**:
```go
type Params struct {
    Page     int
    PageSize int
    MaxSize  int
}

type Response struct {
    Data       interface{}
    TotalCount int64
    Page       int
    PageSize   int
    TotalPages int
}

func ParseParams(page, pageSize, maxSize int) Params
func (p Params) Offset() int
func (p Params) Limit() int
func NewResponse(data interface{}, totalCount int64, params Params) Response
```

### 15. RateLimit Package

**Purpose**: Rate limiting

**Key Types**:
```go
type Limiter interface {
    Allow(ctx context.Context, key string) (bool, error)
    AllowN(ctx context.Context, key string, n int) (bool, error)
    Reset(ctx context.Context, key string) error
}

type Config struct {
    Rate     int           // requests per window
    Window   time.Duration
    RedisClient RedisClient
}

type Result struct {
    Allowed    bool
    Remaining  int
    RetryAfter time.Duration
}
```

**Implementation**: Sử dụng token bucket hoặc sliding window algorithm với Redis

## Data Models

### Common Types

```go
// Shared across packages
type ServiceInfo struct {
    Name    string
    Version string
    Env     string
}

// Standard response wrapper
type Response struct {
    Success bool        `json:"success"`
    Data    interface{} `json:"data,omitempty"`
    Error   *ErrorInfo  `json:"error,omitempty"`
}

type ErrorInfo struct {
    Code    string                 `json:"code"`
    Message string                 `json:"message"`
    Fields  map[string]interface{} `json:"fields,omitempty"`
}
```

## Cor
rectness Properties

*A property is a characteristic or behavior that should hold true across all valid executions of a system-essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.*


Property 1: Logger initialization respects configuration
*For any* valid logger configuration, initializing a logger should return a non-nil logger instance that respects the configured level, format, and output settings
**Validates: Requirements 1.1**

Property 2: Log messages contain required fields
*For any* log message and contextual fields, the output should include timestamp, level, service name, and all provided fields
**Validates: Requirements 1.2**

Property 3: Log level filtering
*For any* configured log level, messages with lower severity should not appear in the output
**Validates: Requirements 1.3**

Property 4: Environment variable configuration loading
*For any* set of environment variables with the configured prefix, loading configuration should populate the corresponding struct fields
**Validates: Requirements 2.1**

Property 5: Configuration merge precedence
*For any* configuration key present in both file and environment variables, the environment variable value should take precedence
**Validates: Requirements 2.3**

Property 6: Missing required configuration validation
*For any* required configuration field that is not provided, validation should fail with an error indicating the missing field name
**Validates: Requirements 2.4**

Property 7: Configuration validation completeness
*For any* configuration struct with validation tags, loading should validate all tagged fields and return errors for invalid values
**Validates: Requirements 2.5**

Property 8: Error wrapping preserves original error
*For any* error that is wrapped with context, the wrapped error should contain the original error accessible via Unwrap()
**Validates: Requirements 3.1**

Property 9: Domain error completeness
*For any* domain error created through constructors, the error should have non-empty code, message, and valid HTTP status code
**Validates: Requirements 3.2**

Property 10: HTTP to gRPC status mapping consistency
*For any* domain error with an HTTP status code, converting to gRPC status should produce a consistent and appropriate gRPC code
**Validates: Requirements 3.3**

Property 11: Error aggregation preserves all errors
*For any* collection of errors, aggregating them should produce a single error that contains information from all individual errors
**Validates: Requirements 3.5**

Property 12: Struct validation checks all tags
*For any* struct with validation tags, validation should check all tagged fields and return errors for each invalid field
**Validates: Requirements 4.1**

Property 13: Required field validation
*For any* struct with required field tags, validation should fail when required fields are missing or empty
**Validates: Requirements 4.2**

Property 14: Validation error includes rule information
*For any* field that fails validation, the error should indicate which validation rule failed
**Validates: Requirements 4.3**

Property 15: Email validation correctness
*For any* string, email validation should correctly identify valid email addresses and reject invalid ones
**Validates: Requirements 4.4**

Property 16: String length validation
*For any* string with min/max length constraints, validation should enforce both bounds correctly
**Validates: Requirements 4.5**

Property 17: gRPC request logging
*For any* gRPC request processed through the logging interceptor, a log entry should be created containing the method name and metadata
**Validates: Requirements 5.1**

Property 18: gRPC response logging
*For any* gRPC request that completes, a log entry should include the response status and request duration
**Validates: Requirements 5.2**

Property 19: gRPC error conversion
*For any* gRPC handler that returns a domain error, the error interceptor should convert it to an appropriate gRPC status code
**Validates: Requirements 5.3**

Property 20: JWT authentication in gRPC
*For any* gRPC request with authentication interceptor, the JWT token in metadata should be validated before handler execution
**Validates: Requirements 5.4**

Property 21: gRPC panic recovery
*For any* gRPC handler that panics, the recovery interceptor should catch the panic and return Internal error status
**Validates: Requirements 5.5**

Property 22: JWT signature validation
*For any* JWT token, validation should verify the signature matches the configured secret key
**Validates: Requirements 6.1**

Property 23: Expired token rejection
*For any* JWT token with expiration time in the past, validation should fail with an authentication error
**Validates: Requirements 6.2**

Property 24: JWT claims extraction completeness
*For any* valid JWT token, extracting claims should return all expected fields including user ID, roles, and custom claims
**Validates: Requirements 6.3**

Property 25: Invalid signature rejection
*For any* JWT token with invalid signature, validation should fail with an authentication error
**Validates: Requirements 6.4**

Property 26: Missing claims validation
*For any* JWT token missing required claims, validation should fail with a validation error
**Validates: Requirements 6.5**

Property 27: PostgreSQL connection pool configuration
*For any* valid PostgreSQL configuration, connecting should establish a connection pool with the specified max connections and idle connections
**Validates: Requirements 7.1**

Property 28: MongoDB client configuration
*For any* valid MongoDB configuration, the created client should respect the configured timeout and connection pool settings
**Validates: Requirements 7.2**

Property 29: Redis client pooling
*For any* valid Redis configuration, the created client should use connection pooling with the specified pool size
**Validates: Requirements 7.3**

Property 30: Database retry with exponential backoff
*For any* transient database error, the retry mechanism should use exponential backoff between attempts
**Validates: Requirements 7.4**

Property 31: Graceful database connection closure
*For any* database client, calling Close should cleanly release all resources without errors
**Validates: Requirements 7.5**

Property 32: Trace ID presence
*For any* request processed through tracing middleware, a trace ID should be present in the context
**Validates: Requirements 8.1**

Property 33: Trace context propagation
*For any* outgoing service call, the trace context should be injected into the request metadata
**Validates: Requirements 8.2**

Property 34: Span metadata completeness
*For any* created span, it should include service name, operation name, start time, and end time
**Validates: Requirements 8.3**

Property 35: Span error marking
*For any* span where an error occurs, the span should be marked with error status and include error details
**Validates: Requirements 8.4**

Property 36: gRPC metrics recording
*For any* gRPC request processed through metrics interceptor, metrics should be recorded for request count, duration, and status
**Validates: Requirements 9.2**

Property 37: Custom metrics support
*For any* metric type (counter, gauge, histogram), the collector should support recording values with labels
**Validates: Requirements 9.3**

Property 38: Health check aggregation
*For any* health checker with registered checks, calling the health endpoint should return results for all registered checks
**Validates: Requirements 10.1**

Property 39: Degraded status on check failure
*For any* health checker where at least one check fails, the overall status should be degraded or unhealthy
**Validates: Requirements 10.2**

Property 40: Graceful server shutdown
*For any* HTTP server with in-flight requests, graceful shutdown should wait for requests to complete before closing
**Validates: Requirements 10.4**

Property 41: Forced shutdown on timeout
*For any* server shutdown that exceeds the configured timeout, remaining connections should be forcibly closed
**Validates: Requirements 10.5**

Property 42: Context value round-trip
*For any* value stored in context, retrieving it should return the same value
**Validates: Requirements 11.1, 11.2, 11.3**

Property 43: Context deadline cancellation
*For any* context with a deadline, operations should be cancelled when the deadline is exceeded
**Validates: Requirements 11.4**

Property 44: Context cancellation propagation
*For any* parent context that is cancelled, all child contexts should also be cancelled
**Validates: Requirements 11.5**

Property 45: Slug generation rules
*For any* input string, slugify should produce a lowercase string with spaces replaced by hyphens and special characters removed
**Validates: Requirements 12.1**

Property 46: String truncation with ellipsis
*For any* string longer than the specified max length, truncation should limit the length and append ellipsis
**Validates: Requirements 12.2**

Property 47: HTML sanitization safety
*For any* HTML string containing dangerous tags or attributes, sanitization should remove or escape them
**Validates: Requirements 12.3**

Property 48: Empty string detection with trimming
*For any* string, the empty check should return true only if the string is empty after trimming whitespace
**Validates: Requirements 12.4**

Property 49: Cryptographically secure random strings
*For any* random string generation, the implementation should use crypto/rand as the random source
**Validates: Requirements 12.5**

Property 50: ISO 8601 timestamp formatting
*For any* timestamp, default formatting should produce a valid ISO 8601 string
**Validates: Requirements 13.1**

Property 51: Flexible timestamp parsing
*For any* common timestamp format (ISO 8601, RFC3339, Unix), parsing should successfully convert to time.Time
**Validates: Requirements 13.2**

Property 52: Timezone conversion preserves absolute time
*For any* time value and target timezone, converting timezone should preserve the absolute moment in time
**Validates: Requirements 13.3**

Property 53: Human-readable duration formatting
*For any* duration value, formatting should produce a human-readable string (e.g., "2h 30m")
**Validates: Requirements 13.4**

Property 54: Pagination offset calculation
*For any* page number and page size, the calculated offset should equal (page - 1) * pageSize
**Validates: Requirements 14.3**

Property 55: Page size capping
*For any* requested page size greater than the configured maximum, the parsed params should cap it to the maximum
**Validates: Requirements 14.2**

Property 56: Pagination response completeness
*For any* data set and pagination params, the response should include total count, current page, page size, and total pages
**Validates: Requirements 14.4**

Property 57: Rate limit counter increment
*For any* rate limit check, the request count in Redis should be incremented for the given key
**Validates: Requirements 15.1**

Property 58: Rate limit exceeded response
*For any* request that exceeds the rate limit, the result should indicate the limit was exceeded and include retry-after duration
**Validates: Requirements 15.2**

Property 59: Rate limit window reset
*For any* rate limit key after the window duration has passed, new requests should be allowed
**Validates: Requirements 15.3**

Property 60: Per-key rate limit configuration
*For any* rate limit key, different rate limits can be configured and enforced independently
**Validates: Requirements 15.4**

Property 61: Rate limiter fail-open behavior
*For any* rate limit check when Redis is unavailable, the limiter should allow the request through
**Validates: Requirements 15.5**

## Error Handling

### Error Types

1. **Configuration Errors**: Invalid or missing configuration
2. **Validation Errors**: Input validation failures
3. **Authentication Errors**: JWT validation failures
4. **Database Errors**: Connection or query failures
5. **Network Errors**: Service communication failures
6. **Internal Errors**: Unexpected system errors

### Error Handling Strategy

- All errors should be wrapped with context using `errors.Wrap` or `fmt.Errorf` with `%w`
- Domain errors should include error codes for client handling
- gRPC errors should use appropriate status codes
- Transient errors should be retried with exponential backoff
- Fatal errors should be logged and cause graceful shutdown

### Error Response Format

```go
type ErrorResponse struct {
    Code    string                 `json:"code"`
    Message string                 `json:"message"`
    Details map[string]interface{} `json:"details,omitempty"`
}
```

## Testing Strategy

### Unit Testing

- Test each package independently with mocked dependencies
- Use table-driven tests for multiple input scenarios
- Test error cases and edge conditions
- Aim for >80% code coverage

**Key unit test areas**:
- Logger output formatting and level filtering
- Configuration parsing and validation
- Error wrapping and unwrapping
- JWT token validation logic
- String manipulation functions
- Time formatting and parsing
- Pagination calculations

### Property-Based Testing

Property-based testing will be used to verify universal properties across all inputs using the `gopter` library (Go port of QuickCheck).

**Configuration**:
- Each property test should run a minimum of 100 iterations
- Use appropriate generators for each data type
- Shrink failing inputs to minimal counterexamples

**Property test tagging**:
Each property-based test must include a comment with this format:
```go
// Feature: golang-shared-library, Property X: [property description]
// Validates: Requirements Y.Z
```

**Key property test areas**:
- Logger configuration and output properties
- Configuration loading and merging behavior
- Error wrapping and unwrapping chains
- Validation rules across random inputs
- JWT token validation with various token states
- Database retry logic with simulated failures
- Context value storage and retrieval
- String utilities with random inputs
- Pagination calculations with various page/size combinations
- Rate limiting with concurrent requests

### Integration Testing

- Test interactions between packages where necessary
- Test database clients with real database instances (using testcontainers)
- Test gRPC interceptors with real gRPC servers
- Test rate limiter with real Redis instance

### Performance Testing

- Benchmark critical paths (logging, validation, JWT parsing)
- Ensure no memory leaks in long-running operations
- Test connection pool behavior under load
- Verify rate limiter performance with high concurrency

## Dependencies

### External Libraries

- **zap** or **zerolog**: Structured logging
- **viper**: Configuration management
- **go-playground/validator**: Struct validation
- **golang-jwt/jwt**: JWT token handling
- **lib/pq**: PostgreSQL driver
- **mongo-go-driver**: MongoDB client
- **go-redis/redis**: Redis client
- **opentelemetry-go** or **jaeger-client-go**: Distributed tracing
- **prometheus/client_golang**: Metrics collection
- **bluemonday**: HTML sanitization
- **gopter**: Property-based testing

### Version Management

- Use Go modules for dependency management
- Pin major versions to avoid breaking changes
- Regular security updates for dependencies
- Document any version-specific requirements

## Deployment Considerations

### Versioning

- Use semantic versioning (v1.0.0, v1.1.0, etc.)
- Tag releases in Git
- Maintain CHANGELOG.md for version history

### Backward Compatibility

- Maintain backward compatibility within major versions
- Deprecate features before removal
- Provide migration guides for breaking changes

### Documentation

- GoDoc comments for all exported types and functions
- README.md with usage examples
- Architecture decision records (ADRs) for major decisions
- Integration guides for each package

## Security Considerations

- Use crypto/rand for random string generation
- Validate all inputs before processing
- Sanitize HTML to prevent XSS
- Use parameterized queries to prevent SQL injection
- Rotate JWT secret keys regularly
- Use TLS for all database connections
- Implement rate limiting to prevent abuse
- Log security events for audit trails

## Performance Considerations

- Use connection pooling for all database clients
- Implement caching where appropriate
- Use buffered channels for async operations
- Minimize allocations in hot paths
- Use sync.Pool for frequently allocated objects
- Profile and benchmark critical code paths
- Set appropriate timeouts for all operations

## Monitoring and Observability

- Expose Prometheus metrics for all services
- Implement distributed tracing for request flows
- Log structured data for easy parsing
- Include correlation IDs in all logs
- Monitor error rates and latencies
- Set up alerts for critical metrics
- Provide health check endpoints
