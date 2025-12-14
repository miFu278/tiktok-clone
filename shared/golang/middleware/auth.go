package middleware

import (
	"context"
	"strings"

	"go.uber.org/zap"
	"google.golang.org/grpc"
	"google.golang.org/grpc/metadata"

	"tiktok-clone/shared/common/errors"
	"tiktok-clone/shared/common/logger"
)

// AuthKey là key để lưu User ID trong context
const AuthKey = "user-id"
const MetadataAuthHeader = "x-user-id" // Header/Metadata chuẩn từ API Gateway

// GetUserIDFromContext lấy User ID từ context
func GetUserIDFromContext(ctx context.Context) (string, error) {
	id, ok := ctx.Value(AuthKey).(string)
	if !ok || id == "" {
		// Log lỗi nếu User ID không tồn tại
		logger.ForContext(ctx).Error("User ID not found in context")
		return "", errors.ErrUnauthorized
	}
	return id, nil
}

// GRPCExtractUserInterceptor là gRPC Interceptor để trích xuất User ID từ Metadata
func GRPCExtractUserInterceptor(ctx context.Context, req interface{}, info *grpc.UnaryServerInfo, handler grpc.UnaryHandler) (interface{}, error) {
	log := logger.ForContext(ctx)

	// Lấy metadata từ context (do API Gateway .NET truyền qua gRPC)
	md, ok := metadata.FromIncomingContext(ctx)
	if !ok {
		log.Warn("Missing gRPC metadata in request")
		// Vẫn cho phép đi tiếp nếu service không yêu cầu AUTH
		return handler(ctx, req)
	}

	// Lấy User ID từ header nội bộ (đã được xác thực từ Gateway)
	userIDs := md.Get(MetadataAuthHeader)
	if len(userIDs) > 0 && userIDs[0] != "" {
		userID := strings.TrimSpace(userIDs[0])

		// Đặt User ID vào context để các handler sau dễ dàng sử dụng
		newCtx := context.WithValue(ctx, AuthKey, userID)
		log.Debug("Authenticated user found", zap.String("userID", userID))

		return handler(newCtx, req)
	}

	// Nếu không có header, tiếp tục xử lý (dành cho các endpoint Public)
	// Các handler cụ thể sẽ check auth nếu cần
	return handler(ctx, req)
}
