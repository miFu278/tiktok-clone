.PHONY: build run test clean proto docker

# Build the application
build:
	go build -o bin/video-service ./cmd/server

# Run the application
run:
	go run ./cmd/server/main.go

# Run tests
test:
	go test -v -race -coverprofile=coverage.out ./...

# View test coverage
coverage:
	go tool cover -html=coverage.out

# Clean build artifacts
clean:
	rm -rf bin/
	rm -f coverage.out

# Generate protobuf files
proto:
	protoc --go_out=. --go_opt=paths=source_relative \
		--go-grpc_out=. --go-grpc_opt=paths=source_relative \
		internal/delivery/grpc/proto/*.proto

# Build Docker image
docker:
	docker build -t video-service:latest .

# Run with Docker Compose
docker-up:
	docker-compose up -d

docker-down:
	docker-compose down

# Format code
fmt:
	go fmt ./...

# Lint code
lint:
	golangci-lint run

# Install dependencies
deps:
	go mod download
	go mod tidy

# Run migrations
migrate-up:
	migrate -path migrations -database "postgresql://postgres:postgres@localhost:5432/tiktok_videos?sslmode=disable" up

migrate-down:
	migrate -path migrations -database "postgresql://postgres:postgres@localhost:5432/tiktok_videos?sslmode=disable" down
