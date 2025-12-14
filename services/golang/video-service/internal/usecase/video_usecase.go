package usecase

import (
	"context"

	"tiktok-clone/shared/common/errors"
	"tiktok-clone/shared/common/logger"
	"tiktok-clone/video-service/internal/domain/entity"
	"tiktok-clone/video-service/internal/domain/repository"
	"tiktok-clone/video-service/internal/usecase/dto"

	"github.com/google/uuid"
	"go.uber.org/zap"
)

// VideoUseCase handles video business logic
type VideoUseCase struct {
	videoRepo          repository.VideoRepository
	storageService     StorageService
	transcodingService TranscodingService
}

// StorageService interface for file storage
type StorageService interface {
	UploadVideo(ctx context.Context, videoID uuid.UUID, data []byte) (string, error)
	UploadThumbnail(ctx context.Context, videoID uuid.UUID, data []byte) (string, error)
	DeleteVideo(ctx context.Context, videoURL string) error
}

// TranscodingService interface for video transcoding
type TranscodingService interface {
	StartTranscoding(ctx context.Context, videoID uuid.UUID, videoURL string) error
}

// NewVideoUseCase creates a new video use case
func NewVideoUseCase(
	videoRepo repository.VideoRepository,
	storageService StorageService,
	transcodingService TranscodingService,
) *VideoUseCase {
	return &VideoUseCase{
		videoRepo:          videoRepo,
		storageService:     storageService,
		transcodingService: transcodingService,
	}
}

// UploadVideo handles video upload
func (uc *VideoUseCase) UploadVideo(ctx context.Context, req *dto.UploadVideoRequest) (*dto.VideoResponse, error) {
	log := logger.ForContext(ctx)
	log.Info("Starting video upload", zap.String("userID", req.UserID.String()))

	// Create video entity
	video := &entity.Video{
		VideoID:         uuid.New(),
		UserID:          req.UserID,
		Title:           req.Title,
		Description:     req.Description,
		DurationSeconds: req.DurationSeconds,
		Width:           req.Width,
		Height:          req.Height,
		FileSize:        int64(len(req.VideoData)),
		EncodingStatus:  "processing",
		IsPublic:        true,
		AllowComments:   true,
		AllowDuet:       true,
		AllowStitch:     true,
	}

	// Upload video to storage
	videoURL, err := uc.storageService.UploadVideo(ctx, video.VideoID, req.VideoData)
	if err != nil {
		log.Error("Failed to upload video", zap.Error(err))
		return nil, errors.ErrInternal
	}
	video.VideoURL = videoURL

	// Upload thumbnail if provided
	if len(req.ThumbnailData) > 0 {
		thumbnailURL, err := uc.storageService.UploadThumbnail(ctx, video.VideoID, req.ThumbnailData)
		if err != nil {
			log.Warn("Failed to upload thumbnail", zap.Error(err))
		} else {
			video.ThumbnailURL = thumbnailURL
		}
	}

	// Save to database
	if err := uc.videoRepo.Create(ctx, video); err != nil {
		log.Error("Failed to save video", zap.Error(err))
		return nil, errors.ErrInternal
	}

	// Start transcoding asynchronously
	go func() {
		if err := uc.transcodingService.StartTranscoding(context.Background(), video.VideoID, videoURL); err != nil {
			log.Error("Failed to start transcoding", zap.Error(err))
		}
	}()

	log.Info("Video uploaded successfully", zap.String("videoID", video.VideoID.String()))
	return uc.toVideoResponse(video), nil
}

// GetVideo retrieves a video by ID
func (uc *VideoUseCase) GetVideo(ctx context.Context, videoID uuid.UUID) (*dto.VideoResponse, error) {
	video, err := uc.videoRepo.GetByID(ctx, videoID)
	if err != nil {
		return nil, errors.ErrNotFound
	}

	return uc.toVideoResponse(video), nil
}

// GetUserVideos retrieves videos by user ID
func (uc *VideoUseCase) GetUserVideos(ctx context.Context, userID uuid.UUID, limit, offset int) ([]*dto.VideoResponse, error) {
	videos, err := uc.videoRepo.GetByUserID(ctx, userID, limit, offset)
	if err != nil {
		return nil, errors.ErrInternal
	}

	responses := make([]*dto.VideoResponse, len(videos))
	for i, video := range videos {
		responses[i] = uc.toVideoResponse(video)
	}

	return responses, nil
}

// UpdateVideo updates video metadata
func (uc *VideoUseCase) UpdateVideo(ctx context.Context, req *dto.UpdateVideoRequest) error {
	video, err := uc.videoRepo.GetByID(ctx, req.VideoID)
	if err != nil {
		return errors.ErrNotFound
	}

	// Update fields if provided
	if req.Title != nil {
		video.Title = *req.Title
	}
	if req.Description != nil {
		video.Description = *req.Description
	}
	if req.IsPublic != nil {
		video.IsPublic = *req.IsPublic
	}
	if req.AllowComments != nil {
		video.AllowComments = *req.AllowComments
	}
	if req.AllowDuet != nil {
		video.AllowDuet = *req.AllowDuet
	}
	if req.AllowStitch != nil {
		video.AllowStitch = *req.AllowStitch
	}

	return uc.videoRepo.Update(ctx, video)
}

// DeleteVideo deletes a video
func (uc *VideoUseCase) DeleteVideo(ctx context.Context, videoID, userID uuid.UUID) error {
	video, err := uc.videoRepo.GetByID(ctx, videoID)
	if err != nil {
		return errors.ErrNotFound
	}

	// Check ownership
	if video.UserID != userID {
		return errors.ErrForbidden
	}

	// Delete from storage
	if err := uc.storageService.DeleteVideo(ctx, video.VideoURL); err != nil {
		logger.ForContext(ctx).Warn("Failed to delete video from storage", zap.Error(err))
	}

	// Delete from database
	return uc.videoRepo.Delete(ctx, videoID)
}

// GetTrendingVideos retrieves trending videos
func (uc *VideoUseCase) GetTrendingVideos(ctx context.Context, limit int) ([]*dto.VideoResponse, error) {
	videos, err := uc.videoRepo.GetTrending(ctx, limit)
	if err != nil {
		return nil, errors.ErrInternal
	}

	responses := make([]*dto.VideoResponse, len(videos))
	for i, video := range videos {
		responses[i] = uc.toVideoResponse(video)
	}

	return responses, nil
}

// RecordView records a video view
func (uc *VideoUseCase) RecordView(ctx context.Context, videoID uuid.UUID) error {
	return uc.videoRepo.IncrementViewCount(ctx, videoID)
}

// UpdateEncodingStatus updates video encoding status
func (uc *VideoUseCase) UpdateEncodingStatus(ctx context.Context, videoID uuid.UUID, status string) error {
	return uc.videoRepo.UpdateEncodingStatus(ctx, videoID, status)
}

// toVideoResponse converts entity to DTO
func (uc *VideoUseCase) toVideoResponse(video *entity.Video) *dto.VideoResponse {
	return &dto.VideoResponse{
		VideoID:         video.VideoID.String(),
		UserID:          video.UserID.String(),
		Title:           video.Title,
		Description:     video.Description,
		VideoURL:        video.VideoURL,
		ThumbnailURL:    video.ThumbnailURL,
		DurationSeconds: video.DurationSeconds,
		Width:           video.Width,
		Height:          video.Height,
		EncodingStatus:  video.EncodingStatus,
		ViewCount:       video.ViewCount,
		LikeCount:       video.LikeCount,
		CommentCount:    video.CommentCount,
		ShareCount:      video.ShareCount,
		CreatedAt:       video.CreatedAt,
	}
}
