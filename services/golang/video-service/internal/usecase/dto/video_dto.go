package dto

import (
	"time"

	"github.com/google/uuid"
)

// UploadVideoRequest represents video upload request
type UploadVideoRequest struct {
	UserID          uuid.UUID
	Title           string
	Description     string
	VideoData       []byte
	ThumbnailData   []byte
	DurationSeconds int
	Width           int
	Height          int
}

// VideoResponse represents video response
type VideoResponse struct {
	VideoID         string    `json:"video_id"`
	UserID          string    `json:"user_id"`
	Title           string    `json:"title"`
	Description     string    `json:"description"`
	VideoURL        string    `json:"video_url"`
	ThumbnailURL    string    `json:"thumbnail_url"`
	DurationSeconds int       `json:"duration_seconds"`
	Width           int       `json:"width"`
	Height          int       `json:"height"`
	EncodingStatus  string    `json:"encoding_status"`
	ViewCount       int64     `json:"view_count"`
	LikeCount       int64     `json:"like_count"`
	CommentCount    int64     `json:"comment_count"`
	ShareCount      int64     `json:"share_count"`
	CreatedAt       time.Time `json:"created_at"`
}

// UpdateVideoRequest represents video update request
type UpdateVideoRequest struct {
	VideoID       uuid.UUID
	Title         *string
	Description   *string
	IsPublic      *bool
	AllowComments *bool
	AllowDuet     *bool
	AllowStitch   *bool
}
