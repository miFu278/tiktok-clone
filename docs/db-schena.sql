-- ============================================
-- POSTGRESQL SCHEMA - User & Core Data
-- ============================================

-- Users Table
CREATE TABLE users (
    user_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    username VARCHAR(50) UNIQUE NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    phone_number VARCHAR(20) UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    full_name VARCHAR(100),
    bio TEXT,
    avatar_url VARCHAR(500),
    date_of_birth DATE,
    gender VARCHAR(20),
    is_verified BOOLEAN DEFAULT FALSE,
    is_private BOOLEAN DEFAULT FALSE,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    last_login_at TIMESTAMP
);

CREATE INDEX idx_users_username ON users(username);
CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_created_at ON users(created_at);

-- User Settings
CREATE TABLE user_settings (
    setting_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
    language VARCHAR(10) DEFAULT 'vi',
    push_notifications BOOLEAN DEFAULT TRUE,
    email_notifications BOOLEAN DEFAULT TRUE,
    privacy_level VARCHAR(20) DEFAULT 'public',
    allow_comments BOOLEAN DEFAULT TRUE,
    allow_duet BOOLEAN DEFAULT TRUE,
    allow_stitch BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(user_id)
);

-- Videos Table
CREATE TABLE videos (
    video_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
    title VARCHAR(255),
    description TEXT,
    video_url VARCHAR(500) NOT NULL,
    thumbnail_url VARCHAR(500),
    duration_seconds INTEGER NOT NULL,
    width INTEGER,
    height INTEGER,
    file_size BIGINT,
    encoding_status VARCHAR(20) DEFAULT 'processing', -- processing, completed, failed
    view_count BIGINT DEFAULT 0,
    like_count BIGINT DEFAULT 0,
    comment_count BIGINT DEFAULT 0,
    share_count BIGINT DEFAULT 0,
    is_public BOOLEAN DEFAULT TRUE,
    allow_comments BOOLEAN DEFAULT TRUE,
    allow_duet BOOLEAN DEFAULT TRUE,
    allow_stitch BOOLEAN DEFAULT TRUE,
    original_video_id UUID REFERENCES videos(video_id),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_videos_user_id ON videos(user_id);
CREATE INDEX idx_videos_created_at ON videos(created_at DESC);
CREATE INDEX idx_videos_view_count ON videos(view_count DESC);
CREATE INDEX idx_videos_encoding_status ON videos(encoding_status);

-- Video Hashtags
CREATE TABLE hashtags (
    hashtag_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(100) UNIQUE NOT NULL,
    usage_count BIGINT DEFAULT 0,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_hashtags_name ON hashtags(name);
CREATE INDEX idx_hashtags_usage_count ON hashtags(usage_count DESC);

-- Video-Hashtag Relationship
CREATE TABLE video_hashtags (
    video_id UUID NOT NULL REFERENCES videos(video_id) ON DELETE CASCADE,
    hashtag_id UUID NOT NULL REFERENCES hashtags(hashtag_id) ON DELETE CASCADE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (video_id, hashtag_id)
);

CREATE INDEX idx_video_hashtags_hashtag ON video_hashtags(hashtag_id);

-- Follows
CREATE TABLE follows (
    follower_id UUID NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
    following_id UUID NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (follower_id, following_id),
    CHECK (follower_id != following_id)
);

CREATE INDEX idx_follows_follower ON follows(follower_id);
CREATE INDEX idx_follows_following ON follows(following_id);
CREATE INDEX idx_follows_created_at ON follows(created_at DESC);

-- Likes
CREATE TABLE video_likes (
    like_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
    video_id UUID NOT NULL REFERENCES videos(video_id) ON DELETE CASCADE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(user_id, video_id)
);

CREATE INDEX idx_video_likes_user ON video_likes(user_id);
CREATE INDEX idx_video_likes_video ON video_likes(video_id);
CREATE INDEX idx_video_likes_created_at ON video_likes(created_at DESC);

-- Video Views (for analytics)
CREATE TABLE video_views (
    view_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    video_id UUID NOT NULL REFERENCES videos(video_id) ON DELETE CASCADE,
    user_id UUID REFERENCES users(user_id) ON DELETE SET NULL,
    watch_duration_seconds INTEGER,
    completion_rate DECIMAL(5,2),
    ip_address INET,
    user_agent TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_video_views_video ON video_views(video_id);
CREATE INDEX idx_video_views_user ON video_views(user_id);
CREATE INDEX idx_video_views_created_at ON video_views(created_at DESC);

-- User Statistics (Materialized View or Table)
CREATE TABLE user_stats (
    user_id UUID PRIMARY KEY REFERENCES users(user_id) ON DELETE CASCADE,
    follower_count BIGINT DEFAULT 0,
    following_count BIGINT DEFAULT 0,
    total_likes BIGINT DEFAULT 0,
    total_videos BIGINT DEFAULT 0,
    total_views BIGINT DEFAULT 0,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- ============================================
-- MONGODB SCHEMA - Comments & Real-time Data
-- ============================================

/*
// Comments Collection
{
  "_id": ObjectId("..."),
  "comment_id": "uuid-v4",
  "video_id": "uuid-v4",
  "user_id": "uuid-v4",
  "parent_comment_id": "uuid-v4", // null for top-level comments
  "content": "Great video!",
  "like_count": 150,
  "reply_count": 5,
  "is_deleted": false,
  "created_at": ISODate("2024-12-09T10:00:00Z"),
  "updated_at": ISODate("2024-12-09T10:00:00Z"),
  
  // Denormalized user info for performance
  "user": {
    "username": "johndoe",
    "avatar_url": "https://...",
    "is_verified": true
  }
}

// Indexes
db.comments.createIndex({ "video_id": 1, "created_at": -1 })
db.comments.createIndex({ "user_id": 1 })
db.comments.createIndex({ "parent_comment_id": 1 })
db.comments.createIndex({ "comment_id": 1 }, { unique: true })

// Comment Likes Collection
{
  "_id": ObjectId("..."),
  "comment_id": "uuid-v4",
  "user_id": "uuid-v4",
  "created_at": ISODate("2024-12-09T10:00:00Z")
}

db.comment_likes.createIndex({ "comment_id": 1, "user_id": 1 }, { unique: true })
db.comment_likes.createIndex({ "user_id": 1 })

// Notifications Collection
{
  "_id": ObjectId("..."),
  "notification_id": "uuid-v4",
  "user_id": "uuid-v4", // recipient
  "type": "like", // like, comment, follow, mention
  "actor_id": "uuid-v4", // who triggered the notification
  "entity_type": "video", // video, comment
  "entity_id": "uuid-v4",
  "message": "johndoe liked your video",
  "is_read": false,
  "created_at": ISODate("2024-12-09T10:00:00Z"),
  
  // Denormalized data
  "actor": {
    "username": "johndoe",
    "avatar_url": "https://..."
  },
  "thumbnail_url": "https://..." // if applicable
}

db.notifications.createIndex({ "user_id": 1, "created_at": -1 })
db.notifications.createIndex({ "user_id": 1, "is_read": 1 })

// User Activity Logs
{
  "_id": ObjectId("..."),
  "user_id": "uuid-v4",
  "action": "video_upload", // video_upload, video_view, login, etc.
  "metadata": {
    "video_id": "uuid-v4",
    "ip_address": "192.168.1.1",
    "device": "iOS",
    "location": "Vietnam"
  },
  "created_at": ISODate("2024-12-09T10:00:00Z")
}

db.user_activity_logs.createIndex({ "user_id": 1, "created_at": -1 })
db.user_activity_logs.createIndex({ "action": 1, "created_at": -1 })
*/

-- ============================================
-- REDIS SCHEMA - Caching & Real-time
-- ============================================

/*
// User Session
Key: "session:{session_id}"
Value: {
  "user_id": "uuid",
  "username": "johndoe",
  "expires_at": 1234567890
}
TTL: 7 days

// User Feed Cache (personalized)
Key: "feed:{user_id}:{page}"
Value: ["video_id_1", "video_id_2", "video_id_3", ...]
TTL: 10 minutes

// Trending Videos
Key: "trending:videos:{region}"
Value: [
  {"video_id": "uuid", "score": 9500},
  {"video_id": "uuid", "score": 8900}
]
Type: Sorted Set
TTL: 5 minutes

// Video View Count (Real-time)
Key: "video:views:{video_id}"
Value: 123456
Type: Counter

// Like Count Cache
Key: "video:likes:{video_id}"
Value: 5432
Type: Counter

// Comment Count Cache
Key: "video:comments:{video_id}"
Value: 890
Type: Counter

// Online Users
Key: "online:users"
Value: Set of user_ids
Type: Set

// Rate Limiting
Key: "rate_limit:{user_id}:{action}"
Value: request_count
TTL: Based on rate limit window (e.g., 1 minute)

// Live Stream Viewers
Key: "livestream:{stream_id}:viewers"
Value: Set of user_ids
Type: Set

// Recently Watched Videos (for recommendation)
Key: "user:{user_id}:watched"
Value: ["video_id_1", "video_id_2", ...]
Type: List
Max Length: 100
*/

-- ============================================
-- CASSANDRA/SCYLLADB - Analytics & Time-series
-- ============================================

/*
// Video Analytics Events
CREATE TABLE video_analytics (
    video_id UUID,
    event_date DATE,
    event_hour INT,
    event_type TEXT, // view, like, share, comment
    user_id UUID,
    device_type TEXT,
    country TEXT,
    event_timestamp TIMESTAMP,
    metadata MAP<TEXT, TEXT>,
    PRIMARY KEY ((video_id, event_date), event_hour, event_timestamp)
) WITH CLUSTERING ORDER BY (event_hour DESC, event_timestamp DESC);

// User Engagement Metrics
CREATE TABLE user_engagement (
    user_id UUID,
    date DATE,
    total_watch_time INT,
    videos_watched INT,
    videos_liked INT,
    comments_posted INT,
    shares INT,
    PRIMARY KEY (user_id, date)
) WITH CLUSTERING ORDER BY (date DESC);

// Trending Scores (Time-series)
CREATE TABLE trending_scores (
    video_id UUID,
    timestamp TIMESTAMP,
    view_velocity DOUBLE,
    engagement_rate DOUBLE,
    trending_score DOUBLE,
    PRIMARY KEY (video_id, timestamp)
) WITH CLUSTERING ORDER BY (timestamp DESC);
*/

-- ============================================
-- ELASTICSEARCH - Search Index
-- ============================================

/*
// Videos Index
{
  "mappings": {
    "properties": {
      "video_id": { "type": "keyword" },
      "user_id": { "type": "keyword" },
      "username": { "type": "keyword" },
      "title": { 
        "type": "text",
        "analyzer": "standard",
        "fields": {
          "keyword": { "type": "keyword" }
        }
      },
      "description": { "type": "text" },
      "hashtags": { "type": "keyword" },
      "view_count": { "type": "long" },
      "like_count": { "type": "long" },
      "created_at": { "type": "date" },
      "duration_seconds": { "type": "integer" },
      "thumbnail_url": { "type": "keyword" }
    }
  }
}

// Users Index
{
  "mappings": {
    "properties": {
      "user_id": { "type": "keyword" },
      "username": {
        "type": "text",
        "analyzer": "autocomplete",
        "fields": {
          "keyword": { "type": "keyword" }
        }
      },
      "full_name": { "type": "text" },
      "bio": { "type": "text" },
      "follower_count": { "type": "long" },
      "is_verified": { "type": "boolean" }
    }
  }
}

// Hashtags Index
{
  "mappings": {
    "properties": {
      "hashtag_id": { "type": "keyword" },
      "name": {
        "type": "text",
        "analyzer": "hashtag_analyzer",
        "fields": {
          "keyword": { "type": "keyword" }
        }
      },
      "usage_count": { "type": "long" }
    }
  }
}
*/