package db

import (
	"fmt"
	"log"

	"tiktok-clone/shared/config"

	"gorm.io/driver/postgres"
	"gorm.io/gorm"
)

// InitPostgreSQL kết nối và trả về GORM DB instance
func InitPostgreSQL(cfg config.PostgresConfig) *gorm.DB {
	dsn := fmt.Sprintf("host=%s user=%s password=%s dbname=%s port=%s sslmode=%s TimeZone=Asia/Ho_Chi_Minh",
		cfg.Host, cfg.User, cfg.Password, cfg.DBName, cfg.Port, cfg.SSLMode)

	db, err := gorm.Open(postgres.Open(dsn), &gorm.Config{})
	if err != nil {
		log.Fatalf("Failed to connect to PostgreSQL: %v", err)
	}

	sqlDB, err := db.DB()
	if err != nil {
		log.Fatalf("Failed to get SQL DB: %v", err)
	}

	// Cấu hình Connection Pool (tùy chỉnh theo service load)
	sqlDB.SetMaxIdleConns(10)
	sqlDB.SetMaxOpenConns(100)

	log.Println("Connected successfully to PostgreSQL")
	return db
}
