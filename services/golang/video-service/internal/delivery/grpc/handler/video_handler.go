package handler

import (
	"context"

	"tiktok-clone/shared/common/errors"
	"tiktok-clone/shared/common/logger"
	"tiktok-clone/shared/middleware"
	pb "tiktok-clone/shared/proto"
	"tiktok-clone/video-service/internal/usecase"
	"tiktok-clone/video-service/internal/usecase/dto"

	"github.com/google/uuid"
	"go.uber.org/zap"
	"google.golang.org/grpc/codes"
	"google.golang.org/grpc/status"
)

// VideoServiceHandler implements gRPC video service
type VideoServiceHandler struct {
	pb.UnimplementedVideoServiceServer
	videoUseCase *usecase.VideoUseCase
}

// NewVideoServiceHandler creates a new video service handler
func NewVideoServiceHandler(videoUseCase *usecase.VideoUseCase) *VideoServiceHandler {
	return &VideoServiceHandler{
		videoUseCase: videoUseCase,
	}
}

// UploadVideo handles video upload
func (h *VideoServiceHandler) UploadVideo(ctx context.Context, req *pb.UploadVideoRequest) (*pb.VideoResponse, error) {
	log := logger.ForContext(ctx)

	// Get user ID from context (set by auth middleware)
	userIDStr, err := middleware.GetUserIDFromContext(ctx)
	if err != nil {
		return nil, status.Error(codes.Unauthenticated, "unauthorized")
	}

	userID, err := uuid.Parse(userIDStr)
	if err != nil {
		return nil, status.Error(codes.InvalidArgument, "invalid user ID")
	}

	// Create use case request
	uploadReq := &dto.UploadVideoRequest{
		UserID:          userID,
		Title:           req.Title,
		Description:     req.Description,
		VideoData:       req.VideoData,
		ThumbnailData:   req.ThumbnailData,
		DurationSeconds: int(req.DurationSeconds),
		Width:           int(req.Width),
		Height:          int(req.Height),
	}

	// Execute use case
	video, err := h.videoUseCase.UploadVideo(ctx, uploadReq)
	if err != nil {
		log.Error("Failed to upload video", zap.Error(err))
		return nil, errors.ToGRPCCode(err)
	}

	return h.toProtoVideoResponse(video), nil
}

// GetVideo retrieves a video by ID
func (h *VideoServiceHandler) GetVideo(ctx context.Context, req *pb.GetVideoRequest) (*pb.VideoResponse, error) {
	videoID, err := uuid.Parse(req.VideoId)
	if err != nil {
		return nil, status.Error(codes.InvalidArgument, "invalid video ID")
	}

	video, err := h.videoUseCase.GetVideo(ctx, videoID)
	if err != nil {
		return nil, errors.ToGRPCCode(err)
	}

	return h.toProtoVideoResponse(video), nil
}

// GetUserVideos retrieves videos by user ID
func (h *VideoServiceHandler) GetUserVideos(ctx context.Context, req *pb.GetUserVideosRequest) (*pb.GetUserVideosResponse, error) {
	userID, err := uuid.Parse(req.UserId)
	if err != nil {
		return nil, status.Error(codes.InvalidArgument, "invalid user ID")
	}

	videos, err := h.videoUseCase.GetUserVideos(ctx, userID, int(req.Limit), int(req.Offset))
	if err != nil {
		return nil, errors.ToGRPCCode(err)
	}

	protoVideos := make([]*pb.VideoResponse, len(videos))
	for i, video := range videos {
		protoVideos[i] = h.toProtoVideoResponse(video)
	}

	return &pb.GetUserVideosResponse{
		Videos: protoVideos,
		Total:  int32(len(videos)),
	}, nil
}

// UpdateVideo updates video metadata
func (h *VideoServiceHandler) UpdateVideo(ctx context.Context, req *pb.UpdateVideoRequest) (*pb.VideoResponse, error) {
	videoID, err := uuid.Parse(req.VideoId)
	if err != nil {
		return nil, status.Error(codes.InvalidArgument, "invalid video ID")
	}

	updateReq := &dto.UpdateVideoRequest{
		VideoID: videoID,
	}

	if req.Title != nil {
		updateReq.Title = &req.Title.Value
	}
	if req.Description != nil {
		updateReq.Description = &req.Description.Value
	}
	if req.IsPublic != nil {
		updateReq.IsPublic = &req.IsPublic.Value
	}

	if err := h.videoUseCase.UpdateVideo(ctx, updateReq); err != nil {
		return nil, errors.ToGRPCCode(err)
	}

	// Get updated video
	video, err := h.videoUseCase.GetVideo(ctx, videoID)
	if err != nil {
		return nil, errors.ToGRPCCode(err)
	}

	return h.toProtoVideoResponse(video), nil
}

// DeleteVideo deletes a video
func (h *VideoServiceHandler) DeleteVideo(ctx context.Context, req *pb.DeleteVideoRequest) (*pb.DeleteVideoResponse, error) {
	videoID, err := uuid.Parse(req.VideoId)
	if err != nil {
		return nil, status.Error(codes.InvalidArgument, "invalid video ID")
	}

	userIDStr, err := middleware.GetUserIDFromContext(ctx)
	if err != nil {
		return nil, status.Error(codes.Unauthenticated, "unauthorized")
	}

	userID, err := uuid.Parse(userIDStr)
	if err != nil {
		return nil, status.Error(codes.InvalidArgument, "invalid user ID")
	}

	if err := h.videoUseCase.DeleteVideo(ctx, videoID, userID); err != nil {
		return nil, errors.ToGRPCCode(err)
	}

	return &pb.DeleteVideoResponse{Success: true}, nil
}

// GetTrendingVideos retrieves trending videos
func (h *VideoServiceHandler) GetTrendingVideos(ctx context.Context, req *pb.GetTrendingVideosRequest) (*pb.GetTrendingVideosResponse, error) {
	videos, err := h.videoUseCase.GetTrendingVideos(ctx, int(req.Limit))
	if err != nil {
		return nil, errors.ToGRPCCode(err)
	}

	protoVideos := make([]*pb.VideoResponse, len(videos))
	for i, video := range videos {
		protoVideos[i] = h.toProtoVideoResponse(video)
	}

	return &pb.GetTrendingVideosResponse{
		Videos: protoVideos,
	}, nil
}

// RecordView records a video view
func (h *VideoServiceHandler) RecordView(ctx context.Context, req *pb.RecordViewRequest) (*pb.RecordViewResponse, error) {
	videoID, err := uuid.Parse(req.VideoId)
	if err != nil {
		return nil, status.Error(codes.InvalidArgument, "invalid video ID")
	}

	if err := h.videoUseCase.RecordView(ctx, videoID); err != nil {
		return nil, errors.ToGRPCCode(err)
	}

	return &pb.RecordViewResponse{Success: true}, nil
}

// toProtoVideoResponse converts DTO to protobuf response
func (h *VideoServiceHandler) toProtoVideoResponse(video *dto.VideoResponse) *pb.VideoResponse {
	return &pb.VideoResponse{
		VideoId:         video.VideoID,
		UserId:          video.UserID,
		Title:           video.Title,
		Description:     video.Description,
		VideoUrl:        video.VideoURL,
		ThumbnailUrl:    video.ThumbnailURL,
		DurationSeconds: int32(video.DurationSeconds),
		Width:           int32(video.Width),
		Height:          int32(video.Height),
		EncodingStatus:  video.EncodingStatus,
		ViewCount:       video.ViewCount,
		LikeCount:       video.LikeCount,
		CommentCount:    video.CommentCount,
		ShareCount:      video.ShareCount,
		CreatedAt:       video.CreatedAt.Unix(),
	}
}
