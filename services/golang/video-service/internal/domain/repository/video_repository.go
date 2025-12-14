package repository

import (
	"context"

	"tiktok-clone/video-service/internal/domain/entity"

	"github.com/google/uuid"
)

// VideoRepository defines the interface for video data access
type VideoRepository interface {
	Create(ctx context.Context, video *entity.Video) error
	GetByID(ctx context.Context, videoID uuid.UUID) (*entity.Video, error)
	GetByUserID(ctx context.Context, userID uuid.UUID, limit, offset int) ([]*entity.Video, error)
	Update(ctx context.Context, video *entity.Video) error
	Delete(ctx context.Context, videoID uuid.UUID) error
	GetTrending(ctx context.Context, limit int) ([]*entity.Video, error)
	IncrementViewCount(ctx context.Context, videoID uuid.UUID) error
	UpdateEncodingStatus(ctx context.Context, videoID uuid.UUID, status string) error
}
