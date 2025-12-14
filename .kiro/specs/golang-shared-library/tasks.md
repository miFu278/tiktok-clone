# Implementation Plan

- [ ] 1. Set up project structure and module initialization




  - Create shared/golang directory structure with pkg subdirectories
  - Initialize Go module with go.mod
  - Set up basic README.md with project overview
  - Configure .gitignore for Go projects
  - _Requirements: All_

- [ ] 2. Implement logger package
  - [-] 2.1 Create logger interface and implementation using zap

    - Define Logger interface with Debug, Info, Warn, Error, Fatal methods
    - Implement Config struct for logger configuration
    - Create NewLogger function with options pattern
    - Support JSON and console output formats
    - _Requirements: 1.1, 1.2_
  
  - [ ]* 2.2 Write property test for logger configuration
    - **Property 1: Logger initialization respects configuration**
    - **Validates: Requirements 1.1**
  
  - [ ]* 2.3 Write property test for log message fields
    - **Property 2: Log messages contain required fields**
    - **Validates: Requirements 1.2**
  
  - [ ]* 2.4 Write property test for log level filtering
    - **Property 3: Log level filtering**
    - **Validates: Requirements 1.3**

- [ ] 3. Implement config package
  - [ ] 3.1 Create configuration loader using viper
    - Define Loader interface
    - Implement Load and LoadFromFile methods
    - Support environment variable loading with prefix
    - Support YAML, JSON, and TOML config files
    - _Requirements: 2.1, 2.2_
  
  - [ ]* 3.2 Write property test for environment variable loading
    - **Property 4: Environment variable configuration loading**
    - **Validates: Requirements 2.1**
  
  - [ ]* 3.3 Write property test for configuration merge precedence
    - **Property 5: Configuration merge precedence**
    - **Validates: Requirements 2.3**
  
  - [ ]* 3.4 Write property test for missing configuration validation
    - **Property 6: Missing required configuration validation**
    - **Validates: Requirements 2.4**
  
  - [ ]* 3.5 Write property test for configuration validation completeness
    - **Property 7: Configuration validation completeness**
    - **Validates: Requirements 2.5**

- [ ] 4. Implement errors package
  - [ ] 4.1 Create error types and constructors
    - Define AppError struct with code, message, HTTP status, gRPC status
    - Implement Error() and Unwrap() methods
    - Create constructor functions: New, Wrap, NotFound, BadRequest, Unauthorized, Internal
    - Implement error code constants
    - _Requirements: 3.1, 3.2_
  
  - [ ] 4.2 Implement error conversion utilities
    - Create ToGRPCStatus function for gRPC error mapping
    - Create ToHTTPStatus function for HTTP error mapping
    - Implement error aggregation function
    - _Requirements: 3.3, 3.5_
  
  - [ ]* 4.3 Write property test for error wrapping
    - **Property 8: Error wrapping preserves original error**
    - **Validates: Requirements 3.1**
  
  - [ ]* 4.4 Write property test for domain error completeness
    - **Property 9: Domain error completeness**
    - **Validates: Requirements 3.2**
  
  - [ ]* 4.5 Write property test for HTTP to gRPC status mapping
    - **Property 10: HTTP to gRPC status mapping consistency**
    - **Validates: Requirements 3.3**
  
  - [ ]* 4.6 Write property test for error aggregation
    - **Property 11: Error aggregation preserves all errors**
    - **Validates: Requirements 3.5**

- [ ] 5. Implement validator package
  - [ ] 5.1 Create validator wrapper using go-playground/validator
    - Define Validator interface
    - Implement Validate and ValidateVar methods
    - Create ValidationError type
    - Add custom validation functions for email, length, etc.
    - _Requirements: 4.1, 4.2, 4.3, 4.4, 4.5_
  
  - [ ]* 5.2 Write property test for struct validation
    - **Property 12: Struct validation checks all tags**
    - **Validates: Requirements 4.1**
  
  - [ ]* 5.3 Write property test for required field validation
    - **Property 13: Required field validation**
    - **Validates: Requirements 4.2**
  
  - [ ]* 5.4 Write property test for validation error information
    - **Property 14: Validation error includes rule information**
    - **Validates: Requirements 4.3**
  
  - [ ]* 5.5 Write property test for email validation
    - **Property 15: Email validation correctness**
    - **Validates: Requirements 4.4**
  
  - [ ]* 5.6 Write property test for string length validation
    - **Property 16: String length validation**
    - **Validates: Requirements 4.5**

- [ ] 6. Checkpoint - Ensure all tests pass
  - Ensure all tests pass, ask the user if questions arise.

- [ ] 7. Implement auth package
  - [ ] 7.1 Create JWT service implementation
    - Define JWTService interface
    - Implement ValidateToken method using golang-jwt/jwt
    - Implement ExtractClaims method
    - Create Claims struct with user ID, roles, timestamps
    - Add Config struct for JWT settings
    - _Requirements: 6.1, 6.2, 6.3, 6.4, 6.5_
  
  - [ ]* 7.2 Write property test for JWT signature validation
    - **Property 22: JWT signature validation**
    - **Validates: Requirements 6.1**
  
  - [ ]* 7.3 Write property test for expired token rejection
    - **Property 23: Expired token rejection**
    - **Validates: Requirements 6.2**
  
  - [ ]* 7.4 Write property test for JWT claims extraction
    - **Property 24: JWT claims extraction completeness**
    - **Validates: Requirements 6.3**
  
  - [ ]* 7.5 Write property test for invalid signature rejection
    - **Property 25: Invalid signature rejection**
    - **Validates: Requirements 6.4**
  
  - [ ]* 7.6 Write property test for missing claims validation
    - **Property 26: Missing claims validation**
    - **Validates: Requirements 6.5**

- [ ] 8. Implement middleware package
  - [ ] 8.1 Create gRPC interceptors
    - Implement UnaryLoggingInterceptor
    - Implement UnaryAuthInterceptor using JWT service
    - Implement UnaryRecoveryInterceptor
    - Implement UnaryTracingInterceptor
    - Implement UnaryMetricsInterceptor
    - _Requirements: 5.1, 5.2, 5.3, 5.4, 5.5_
  
  - [ ] 8.2 Create HTTP middleware
    - Implement LoggingMiddleware
    - Implement RecoveryMiddleware
    - Implement CORSMiddleware
    - _Requirements: 5.1, 5.2, 5.5_
  
  - [ ]* 8.3 Write property test for gRPC request logging
    - **Property 17: gRPC request logging**
    - **Validates: Requirements 5.1**
  
  - [ ]* 8.4 Write property test for gRPC response logging
    - **Property 18: gRPC response logging**
    - **Validates: Requirements 5.2**
  
  - [ ]* 8.5 Write property test for gRPC error conversion
    - **Property 19: gRPC error conversion**
    - **Validates: Requirements 5.3**
  
  - [ ]* 8.6 Write property test for JWT authentication in gRPC
    - **Property 20: JWT authentication in gRPC**
    - **Validates: Requirements 5.4**
  
  - [ ]* 8.7 Write property test for gRPC panic recovery
    - **Property 21: gRPC panic recovery**
    - **Validates: Requirements 5.5**

- [ ] 9. Implement database package
  - [ ] 9.1 Create PostgreSQL client
    - Define PostgresClient interface
    - Implement NewPostgresClient with connection pooling
    - Add Ping and Close methods
    - Implement retry logic with exponential backoff
    - _Requirements: 7.1, 7.4, 7.5_
  
  - [ ] 9.2 Create MongoDB client
    - Define MongoClient interface
    - Implement NewMongoClient with timeout and retry settings
    - Add Ping and Close methods
    - _Requirements: 7.2, 7.4, 7.5_
  
  - [ ] 9.3 Create Redis client
    - Define RedisClient interface
    - Implement NewRedisClient with connection pooling
    - Add Get, Set, Del methods
    - Add Close method
    - _Requirements: 7.3, 7.5_
  
  - [ ]* 9.4 Write property test for PostgreSQL connection pool
    - **Property 27: PostgreSQL connection pool configuration**
    - **Validates: Requirements 7.1**
  
  - [ ]* 9.5 Write property test for MongoDB client configuration
    - **Property 28: MongoDB client configuration**
    - **Validates: Requirements 7.2**
  
  - [ ]* 9.6 Write property test for Redis client pooling
    - **Property 29: Redis client pooling**
    - **Validates: Requirements 7.3**
  
  - [ ]* 9.7 Write property test for database retry logic
    - **Property 30: Database retry with exponential backoff**
    - **Validates: Requirements 7.4**
  
  - [ ]* 9.8 Write property test for graceful connection closure
    - **Property 31: Graceful database connection closure**
    - **Validates: Requirements 7.5**

- [ ] 10. Checkpoint - Ensure all tests pass
  - Ensure all tests pass, ask the user if questions arise.

- [ ] 11. Implement tracing package
  - [ ] 11.1 Create tracing implementation using OpenTelemetry
    - Define Tracer and Span interfaces
    - Implement NewTracer with Jaeger exporter
    - Implement StartSpan, Extract, Inject methods
    - Support no-op tracer when disabled
    - _Requirements: 8.1, 8.2, 8.3, 8.4_
  
  - [ ]* 11.2 Write property test for trace ID presence
    - **Property 32: Trace ID presence**
    - **Validates: Requirements 8.1**
  
  - [ ]* 11.3 Write property test for trace context propagation
    - **Property 33: Trace context propagation**
    - **Validates: Requirements 8.2**
  
  - [ ]* 11.4 Write property test for span metadata
    - **Property 34: Span metadata completeness**
    - **Validates: Requirements 8.3**
  
  - [ ]* 11.5 Write property test for span error marking
    - **Property 35: Span error marking**
    - **Validates: Requirements 8.4**

- [ ] 12. Implement metrics package
  - [ ] 12.1 Create metrics collector using Prometheus
    - Define MetricsCollector interface
    - Implement counter, gauge, and histogram registration
    - Implement metric recording methods
    - Add default system metrics (CPU, memory, goroutines)
    - Create HTTP handler for metrics endpoint
    - _Requirements: 9.1, 9.2, 9.3, 9.4_
  
  - [ ]* 12.2 Write property test for gRPC metrics recording
    - **Property 36: gRPC metrics recording**
    - **Validates: Requirements 9.2**
  
  - [ ]* 12.3 Write property test for custom metrics support
    - **Property 37: Custom metrics support**
    - **Validates: Requirements 9.3**

- [ ] 13. Implement httputil package
  - [ ] 13.1 Create health checker
    - Define HealthChecker interface
    - Implement AddCheck and Handler methods
    - Create HealthStatus and CheckResult types
    - Support healthy, degraded, and unhealthy statuses
    - _Requirements: 10.1, 10.2_
  
  - [ ] 13.2 Create HTTP server wrapper
    - Implement Server struct with graceful shutdown
    - Add Start and Shutdown methods
    - Support shutdown timeout and forced closure
    - _Requirements: 10.4, 10.5_
  
  - [ ]* 13.3 Write property test for health check aggregation
    - **Property 38: Health check aggregation**
    - **Validates: Requirements 10.1**
  
  - [ ]* 13.4 Write property test for degraded status
    - **Property 39: Degraded status on check failure**
    - **Validates: Requirements 10.2**
  
  - [ ]* 13.5 Write property test for graceful shutdown
    - **Property 40: Graceful server shutdown**
    - **Validates: Requirements 10.4**
  
  - [ ]* 13.6 Write property test for forced shutdown
    - **Property 41: Forced shutdown on timeout**
    - **Validates: Requirements 10.5**

- [ ] 14. Implement contextutil package
  - [ ] 14.1 Create context helper functions
    - Implement WithUserID and GetUserID
    - Implement WithTraceID and GetTraceID
    - Implement WithRequestID and GetRequestID
    - Implement WithTimeout wrapper
    - _Requirements: 11.1, 11.2, 11.3, 11.4, 11.5_
  
  - [ ]* 14.2 Write property test for context value round-trip
    - **Property 42: Context value round-trip**
    - **Validates: Requirements 11.1, 11.2, 11.3**
  
  - [ ]* 14.3 Write property test for context deadline cancellation
    - **Property 43: Context deadline cancellation**
    - **Validates: Requirements 11.4**
  
  - [ ]* 14.4 Write property test for context cancellation propagation
    - **Property 44: Context cancellation propagation**
    - **Validates: Requirements 11.5**

- [ ] 15. Implement stringutil package
  - [ ] 15.1 Create string utility functions
    - Implement Slugify function
    - Implement Truncate function
    - Implement SanitizeHTML using bluemonday
    - Implement IsEmpty function
    - Implement RandomString using crypto/rand
    - Implement ToSnakeCase and ToCamelCase
    - _Requirements: 12.1, 12.2, 12.3, 12.4, 12.5_
  
  - [ ]* 15.2 Write property test for slug generation
    - **Property 45: Slug generation rules**
    - **Validates: Requirements 12.1**
  
  - [ ]* 15.3 Write property test for string truncation
    - **Property 46: String truncation with ellipsis**
    - **Validates: Requirements 12.2**
  
  - [ ]* 15.4 Write property test for HTML sanitization
    - **Property 47: HTML sanitization safety**
    - **Validates: Requirements 12.3**
  
  - [ ]* 15.5 Write property test for empty string detection
    - **Property 48: Empty string detection with trimming**
    - **Validates: Requirements 12.4**
  
  - [ ]* 15.6 Write property test for random string generation
    - **Property 49: Cryptographically secure random strings**
    - **Validates: Requirements 12.5**

- [ ] 16. Checkpoint - Ensure all tests pass
  - Ensure all tests pass, ask the user if questions arise.

- [ ] 17. Implement timeutil package
  - [ ] 17.1 Create time utility functions
    - Implement FormatISO8601 function
    - Implement ParseISO8601 and ParseFlexible functions
    - Implement ConvertTimezone function
    - Implement FormatDuration function
    - Implement NowUTC, StartOfDay, EndOfDay functions
    - _Requirements: 13.1, 13.2, 13.3, 13.4, 13.5_
  
  - [ ]* 17.2 Write property test for ISO 8601 formatting
    - **Property 50: ISO 8601 timestamp formatting**
    - **Validates: Requirements 13.1**
  
  - [ ]* 17.3 Write property test for flexible parsing
    - **Property 51: Flexible timestamp parsing**
    - **Validates: Requirements 13.2**
  
  - [ ]* 17.4 Write property test for timezone conversion
    - **Property 52: Timezone conversion preserves absolute time**
    - **Validates: Requirements 13.3**
  
  - [ ]* 17.5 Write property test for duration formatting
    - **Property 53: Human-readable duration formatting**
    - **Validates: Requirements 13.4**

- [ ] 18. Implement pagination package
  - [ ] 18.1 Create pagination types and functions
    - Define Params and Response structs
    - Implement ParseParams function
    - Implement Offset and Limit methods
    - Implement NewResponse function
    - _Requirements: 14.1, 14.2, 14.3, 14.4, 14.5_
  
  - [ ]* 18.2 Write property test for offset calculation
    - **Property 54: Pagination offset calculation**
    - **Validates: Requirements 14.3**
  
  - [ ]* 18.3 Write property test for page size capping
    - **Property 55: Page size capping**
    - **Validates: Requirements 14.2**
  
  - [ ]* 18.4 Write property test for pagination response
    - **Property 56: Pagination response completeness**
    - **Validates: Requirements 14.4**

- [ ] 19. Implement ratelimit package
  - [ ] 19.1 Create rate limiter using Redis
    - Define Limiter interface
    - Implement token bucket or sliding window algorithm
    - Implement Allow and AllowN methods
    - Implement Reset method
    - Add fail-open behavior when Redis is unavailable
    - _Requirements: 15.1, 15.2, 15.3, 15.4, 15.5_
  
  - [ ]* 19.2 Write property test for rate limit counter
    - **Property 57: Rate limit counter increment**
    - **Validates: Requirements 15.1**
  
  - [ ]* 19.3 Write property test for rate limit exceeded
    - **Property 58: Rate limit exceeded response**
    - **Validates: Requirements 15.2**
  
  - [ ]* 19.4 Write property test for window reset
    - **Property 59: Rate limit window reset**
    - **Validates: Requirements 15.3**
  
  - [ ]* 19.5 Write property test for per-key configuration
    - **Property 60: Per-key rate limit configuration**
    - **Validates: Requirements 15.4**
  
  - [ ]* 19.6 Write property test for fail-open behavior
    - **Property 61: Rate limiter fail-open behavior**
    - **Validates: Requirements 15.5**

- [ ] 20. Create comprehensive documentation
  - [ ] 20.1 Write package documentation
    - Add GoDoc comments to all exported types and functions
    - Create README.md with installation and usage instructions
    - Add examples for each package
    - Document configuration options
    - _Requirements: All_
  
  - [ ] 20.2 Create integration guides
    - Write guide for integrating logger package
    - Write guide for setting up database clients
    - Write guide for implementing gRPC interceptors
    - Write guide for adding metrics and tracing
    - _Requirements: All_

- [ ] 21. Final checkpoint - Ensure all tests pass
  - Ensure all tests pass, ask the user if questions arise.
