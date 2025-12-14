package logger

import (
	"context"

	"go.uber.org/zap"
	"go.uber.org/zap/zapcore"
)

var (
	globalLogger *zap.Logger
	// LoggerKey là key để lấy Logger từ context
	LoggerKey = "logger"
)

// InitLogger khởi tạo Zap logger.
func InitLogger(serviceName, env string) {
	var config zap.Config
	if env == "production" {
		config = zap.NewProductionConfig()
	} else {
		config = zap.NewDevelopmentConfig()
	}

	config.EncoderConfig.EncodeTime = zapcore.ISO8601TimeEncoder
	config.InitialFields = map[string]any{
		"service": serviceName,
		"env":     env,
	}

	var err error
	globalLogger, err = config.Build()
	if err != nil {
		panic(err)
	}
	zap.ReplaceGlobals(globalLogger)
}

// ForContext lấy logger từ context, nếu không có thì trả về global logger.
// Rất quan trọng để inject trace_id/request_id vào logger.
func ForContext(ctx context.Context) *zap.Logger {
	if ctx == nil {
		return globalLogger
	}
	if ctxLogger, ok := ctx.Value(LoggerKey).(*zap.Logger); ok {
		return ctxLogger
	}
	return globalLogger
}

// WithContext tạo context mới với logger đã thêm field (ví dụ: trace ID)
func WithContext(ctx context.Context, fields ...zapcore.Field) context.Context {
	return context.WithValue(ctx, LoggerKey, globalLogger.With(fields...))
}

// Các hàm wrapper cơ bản
func Info(msg string, fields ...zapcore.Field) {
	globalLogger.Info(msg, fields...)
}

func Error(msg string, fields ...zapcore.Field) {
	globalLogger.Error(msg, fields...)
}

// ... thêm các hàm Debug, Warn, Fatal
