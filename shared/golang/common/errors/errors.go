package errors

import (
	"fmt"
	"net/http"

	"google.golang.org/grpc/codes"
	"google.golang.org/grpc/status"
)

// AppError là cấu trúc lỗi chuẩn trong ứng dụng
type AppError struct {
	Code       int        // Mã lỗi nội bộ (tùy chỉnh)
	Message    string     // Thông báo lỗi cho Developer
	HTTPStatus int        // HTTP Status Code cho API Gateway
	GRPCCode   codes.Code // gRPC Status Code cho giao tiếp nội bộ
}

// Implement Error interface
func (e *AppError) Error() string {
	return fmt.Sprintf("Code: %d, Message: %s", e.Code, e.Message)
}

// NewAppError tạo một lỗi ứng dụng mới
func NewAppError(code int, msg string, httpStatus int, grpcCode codes.Code) *AppError {
	return &AppError{
		Code:       code,
		Message:    msg,
		HTTPStatus: httpStatus,
		GRPCCode:   grpcCode,
	}
}

// Định nghĩa các lỗi thường gặp
var (
	ErrNotFound     = NewAppError(1001, "Resource not found", http.StatusNotFound, codes.NotFound)
	ErrUnauthorized = NewAppError(1002, "Authentication failed", http.StatusUnauthorized, codes.Unauthenticated)
	ErrForbidden    = NewAppError(1003, "Access denied", http.StatusForbidden, codes.PermissionDenied)
	ErrInternal     = NewAppError(1004, "Internal server error", http.StatusInternalServerError, codes.Internal)
	ErrInvalidParam = NewAppError(1005, "Invalid request parameter", http.StatusBadRequest, codes.InvalidArgument)
)

// ToGRPCCode chuyển AppError sang gRPC Status
func ToGRPCCode(err error) error {
	if appErr, ok := err.(*AppError); ok {
		return status.Error(appErr.GRPCCode, appErr.Message)
	}
	// Mặc định là Internal nếu không phải AppError
	return status.Error(codes.Internal, ErrInternal.Message)
}

// FromGRPCCode chuyển gRPC Status sang AppError
func FromGRPCCode(err error) *AppError {
	s, ok := status.FromError(err)
	if !ok {
		return ErrInternal // Không phải status gRPC
	}

	switch s.Code() {
	case codes.NotFound:
		return ErrNotFound
	case codes.Unauthenticated:
		return ErrUnauthorized
	case codes.PermissionDenied:
		return ErrForbidden
	case codes.InvalidArgument:
		return ErrInvalidParam
	default:
		return ErrInternal
	}
}
