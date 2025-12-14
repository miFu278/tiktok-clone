package config

import (
	"log"

	"github.com/spf13/viper"
)

// AppConfig chứa toàn bộ cấu hình ứng dụng
type AppConfig struct {
	ServiceName string `mapstructure:"SERVICE_NAME"`
	Environment string `mapstructure:"ENVIRONMENT"`
	Server      ServerConfig
	Postgres    PostgresConfig
	Redis       RedisConfig
	Kafka       KafkaConfig
	Tracing     TracingConfig
}

// ServerConfig cho HTTP/gRPC
type ServerConfig struct {
	GRPCPort string `mapstructure:"GRPC_PORT"`
}

// PostgresConfig cho PostgreSQL
type PostgresConfig struct {
	Host     string `mapstructure:"PG_HOST"`
	Port     string `mapstructure:"PG_PORT"`
	User     string `mapstructure:"PG_USER"`
	Password string `mapstructure:"PG_PASSWORD"`
	DBName   string `mapstructure:"PG_DBNAME"`
	SSLMode  string `mapstructure:"PG_SSLMODE"`
}

// RedisConfig cho Redis
type RedisConfig struct {
	Addr string `mapstructure:"REDIS_ADDR"`
}

// KafkaConfig cho Message Queue
type KafkaConfig struct {
	Brokers []string `mapstructure:"KAFKA_BROKERS"`
}

// TracingConfig cho OpenTelemetry
type TracingConfig struct {
	JaegerEndpoint string `mapstructure:"JAEGER_ENDPOINT"`
}

// LoadConfig đọc cấu hình từ file .env hoặc biến môi trường
func LoadConfig() AppConfig {
	// Set default values (optional)
	viper.SetDefault("SERVICE_NAME", "unknown-service")
	viper.SetDefault("ENVIRONMENT", "development")

	// Đọc từ biến môi trường
	viper.AutomaticEnv()

	// Nếu dùng file .env:
	// viper.SetConfigFile(".env")
	// err := viper.ReadInConfig()

	var cfg AppConfig
	if err := viper.Unmarshal(&cfg); err != nil {
		log.Fatalf("Failed to unmarshal configuration: %v", err)
	}

	// Dùng os.Getenv cho các biến không cần mapstructure
	cfg.Kafka.Brokers = viper.GetStringSlice("KAFKA_BROKERS")
	if len(cfg.Kafka.Brokers) == 0 {
		// Fallback nếu không đọc được từ slice
		log.Println("KAFKA_BROKERS not set or empty. Using default.")
	}

	log.Printf("Configuration loaded successfully for service: %s", cfg.ServiceName)
	return cfg
}
