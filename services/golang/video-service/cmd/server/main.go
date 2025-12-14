package main

import (
	"fmt"
	"log"
	"net"

	"tiktok-clone/shared/common/logger"
	"tiktok-clone/shared/config"
	"tiktok-clone/shared/db"
	"tiktok-clone/shared/middleware"
	pb "tiktok-clone/shared/proto"
	"tiktok-clone/video-service/internal/delivery/grpc/handler"
	"tiktok-clone/video-service/internal/infrastructure/persistence/postgres"
	"tiktok-clone/video-service/internal/infrastructure/storage"
	"tiktok-clone/video-service/internal/usecase"

	"google.golang.org/grpc"
)

func main() {
	// Load configuration
	cfg := config.LoadConfig()

	// Initialize logger
	logger.InitLogger(cfg.ServiceName, cfg.Environment)
	log.Println("Starting Video Service...")

	// Initialize database
	database := db.InitPostgreSQL(cfg.Postgres)

	// Initialize storage service
	storageService, err := storage.NewS3Storage("tiktok-videos", "us-east-1")
	if err != nil {
		log.Fatalf("Failed to initialize storage: %v", err)
	}

	// Initialize repositories
	videoRepo := postgres.NewVideoRepository(database)

	// Initialize use cases
	videoUseCase := usecase.NewVideoUseCase(videoRepo, storageService, nil)

	// Initialize gRPC handlers
	videoHandler := handler.NewVideoServiceHandler(videoUseCase)

	// Create gRPC server
	grpcServer := grpc.NewServer(
		grpc.UnaryInterceptor(middleware.GRPCExtractUserInterceptor),
	)

	// Register services
	pb.RegisterVideoServiceServer(grpcServer, videoHandler)

	// Start listening
	port := cfg.Server.GRPCPort
	if port == "" {
		port = "50051"
	}

	listener, err := net.Listen("tcp", fmt.Sprintf(":%s", port))
	if err != nil {
		log.Fatalf("Failed to listen: %v", err)
	}

	log.Printf("Video Service listening on port %s", port)
	if err := grpcServer.Serve(listener); err != nil {
		log.Fatalf("Failed to serve: %v", err)
	}
}
