package storage

import (
	"bytes"
	"context"
	"fmt"

	"github.com/aws/aws-sdk-go/aws"
	"github.com/aws/aws-sdk-go/aws/session"
	"github.com/aws/aws-sdk-go/service/s3"
	"github.com/google/uuid"
)

// S3Storage implements StorageService using AWS S3
type S3Storage struct {
	client     *s3.S3
	bucketName string
	region     string
}

// NewS3Storage creates a new S3 storage service
func NewS3Storage(bucketName, region string) (*S3Storage, error) {
	sess, err := session.NewSession(&aws.Config{
		Region: aws.String(region),
	})
	if err != nil {
		return nil, err
	}

	return &S3Storage{
		client:     s3.New(sess),
		bucketName: bucketName,
		region:     region,
	}, nil
}

// UploadVideo uploads video to S3
func (s *S3Storage) UploadVideo(ctx context.Context, videoID uuid.UUID, data []byte) (string, error) {
	key := fmt.Sprintf("videos/%s/original.mp4", videoID.String())

	_, err := s.client.PutObjectWithContext(ctx, &s3.PutObjectInput{
		Bucket:      aws.String(s.bucketName),
		Key:         aws.String(key),
		Body:        bytes.NewReader(data),
		ContentType: aws.String("video/mp4"),
	})
	if err != nil {
		return "", err
	}

	url := fmt.Sprintf("https://%s.s3.%s.amazonaws.com/%s", s.bucketName, s.region, key)
	return url, nil
}

// UploadThumbnail uploads thumbnail to S3
func (s *S3Storage) UploadThumbnail(ctx context.Context, videoID uuid.UUID, data []byte) (string, error) {
	key := fmt.Sprintf("videos/%s/thumbnail.jpg", videoID.String())

	_, err := s.client.PutObjectWithContext(ctx, &s3.PutObjectInput{
		Bucket:      aws.String(s.bucketName),
		Key:         aws.String(key),
		Body:        bytes.NewReader(data),
		ContentType: aws.String("image/jpeg"),
	})
	if err != nil {
		return "", err
	}

	url := fmt.Sprintf("https://%s.s3.%s.amazonaws.com/%s", s.bucketName, s.region, key)
	return url, nil
}

// DeleteVideo deletes video from S3
func (s *S3Storage) DeleteVideo(ctx context.Context, videoURL string) error {
	// Extract key from URL
	// This is a simplified implementation
	key := videoURL // You should parse the URL properly

	_, err := s.client.DeleteObjectWithContext(ctx, &s3.DeleteObjectInput{
		Bucket: aws.String(s.bucketName),
		Key:    aws.String(key),
	})
	return err
}
