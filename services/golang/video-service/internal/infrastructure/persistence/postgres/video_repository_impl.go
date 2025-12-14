package postgres

import (
	"context"

	"tiktok-clone/video-service/internal/domain/entity"

	"github.com/google/uuid"
	"gorm.io/gorm"
)

// VideoRepositoryImpl implements VideoRepository
type VideoRepositoryImpl struct {
	db *gorm.DB
}

// NewVideoRepository creates a new video repository
func NewVideoRepository(db *gorm.DB) *VideoRepositoryImpl {
	return &VideoRepositoryImpl{db: db}
}

// Create creates a new video
func (r *VideoRepositoryImpl) Create(ctx context.Context, video *entity.Video) error {
	return r.db.WithContext(ctx).Create(video).Error
}

// GetByID retrieves a video by ID
func (r *VideoRepositoryImpl) GetByID(ctx context.Context, videoID uuid.UUID) (*entity.Video, error) {
	var video entity.Video
	err := r.db.WithContext(ctx).Where("video_id = ?", videoID).First(&video).Error
	if err != nil {
		return nil, err
	}
	return &video, nil
}

// GetByUserID retrieves videos by user ID
func (r *VideoRepositoryImpl) GetByUserID(ctx context.Context, userID uuid.UUID, limit, offset int) ([]*entity.Video, error) {
	var videos []*entity.Video
	err := r.db.WithContext(ctx).
		Where("user_id = ? AND is_public = ?", userID, true).
		Order("created_at DESC").
		Limit(limit).
		Offset(offset).
		Find(&videos).Error
	return videos, err
}

// Update updates a video
func (r *VideoRepositoryImpl) Update(ctx context.Context, video *entity.Video) error {
	return r.db.WithContext(ctx).Save(video).Error
}

// Delete deletes a video
func (r *VideoRepositoryImpl) Delete(ctx context.Context, videoID uuid.UUID) error {
	return r.db.WithContext(ctx).Where("video_id = ?", videoID).Delete(&entity.Video{}).Error
}

// GetTrending retrieves trending videos
func (r *VideoRepositoryImpl) GetTrending(ctx context.Context, limit int) ([]*entity.Video, error) {
	var videos []*entity.Video
	err := r.db.WithContext(ctx).
		Where("is_public = ? AND encoding_status = ?", true, "completed").
		Order("view_count DESC, created_at DESC").
		Limit(limit).
		Find(&videos).Error
	return videos, err
}

// IncrementViewCount increments video view count
func (r *VideoRepositoryImpl) IncrementViewCount(ctx context.Context, videoID uuid.UUID) error {
	return r.db.WithContext(ctx).
		Model(&entity.Video{}).
		Where("video_id = ?", videoID).
		UpdateColumn("view_count", gorm.Expr("view_count + ?", 1)).
		Error
}

// UpdateEncodingStatus updates video encoding status
func (r *VideoRepositoryImpl) UpdateEncodingStatus(ctx context.Context, videoID uuid.UUID, status string) error {
	return r.db.WithContext(ctx).
		Model(&entity.Video{}).
		Where("video_id = ?", videoID).
		Update("encoding_status", status).
		Error
}
