package entity

import (
	"time"

	"github.com/google/uuid"
)

// Video entity - Enterprise Business Rules
type Video struct {
	VideoID         uuid.UUID `gorm:"type:uuid;primary_key;default:gen_random_uuid()"`
	UserID          uuid.UUID `gorm:"type:uuid;not null;index"`
	Title           string    `gorm:"type:varchar(255)"`
	Description     string    `gorm:"type:text"`
	VideoURL        string    `gorm:"type:varchar(500);not null"`
	ThumbnailURL    string    `gorm:"type:varchar(500)"`
	DurationSeconds int       `gorm:"not null"`
	Width           int
	Height          int
	FileSize        int64
	EncodingStatus  string     `gorm:"type:varchar(20);default:'processing';index"`
	ViewCount       int64      `gorm:"default:0;index:idx_view_count"`
	LikeCount       int64      `gorm:"default:0"`
	CommentCount    int64      `gorm:"default:0"`
	ShareCount      int64      `gorm:"default:0"`
	IsPublic        bool       `gorm:"default:true"`
	AllowComments   bool       `gorm:"default:true"`
	AllowDuet       bool       `gorm:"default:true"`
	AllowStitch     bool       `gorm:"default:true"`
	OriginalVideoID *uuid.UUID `gorm:"type:uuid"`
	CreatedAt       time.Time  `gorm:"index:idx_created_at"`
	UpdatedAt       time.Time
}

// TableName specifies the table name
func (Video) TableName() string {
	return "videos"
}

// IsProcessing checks if video is still being processed
func (v *Video) IsProcessing() bool {
	return v.EncodingStatus == "processing"
}

// IsCompleted checks if video processing is completed
func (v *Video) IsCompleted() bool {
	return v.EncodingStatus == "completed"
}

// MarkAsCompleted marks video as completed
func (v *Video) MarkAsCompleted() {
	v.EncodingStatus = "completed"
	v.UpdatedAt = time.Now()
}

// MarkAsFailed marks video as failed
func (v *Video) MarkAsFailed() {
	v.EncodingStatus = "failed"
	v.UpdatedAt = time.Now()
}

// IncrementViewCount increments view count
func (v *Video) IncrementViewCount() {
	v.ViewCount++
}
