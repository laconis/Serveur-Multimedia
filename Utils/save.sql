CREATE TABLE videos (
    id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,

    -- Infos fichier
    file_path VARCHAR(1024) NOT NULL,
    file_name VARCHAR(255) NOT NULL,
    extension VARCHAR(20) DEFAULT NULL,
    mime_type VARCHAR(100) DEFAULT NULL,
    hash_sha256 CHAR(64) DEFAULT NULL,
    size_bytes BIGINT UNSIGNED DEFAULT NULL,

    -- Métadonnées vidéo
    title VARCHAR(255) DEFAULT NULL,
    description TEXT DEFAULT NULL,
    category TINYINT UNSIGNED DEFAULT NULL,
    duration_sec INT UNSIGNED DEFAULT NULL,
    resolution VARCHAR(50) DEFAULT NULL,      -- ex: 1920x1080
    width INT UNSIGNED DEFAULT NULL,
    height INT UNSIGNED DEFAULT NULL,
    fps DECIMAL(6,2) DEFAULT NULL,

    -- Audio / encodage
    video_codec VARCHAR(50) DEFAULT NULL,
    audio_codec VARCHAR(50) DEFAULT NULL,
    bitrate_kbps INT UNSIGNED DEFAULT NULL,

    -- Images / aperçu
    cover_path VARCHAR(1024) DEFAULT NULL,
    thumbnail_path VARCHAR(1024) DEFAULT NULL,

    -- Lecture / suivi
    last_position_sec INT UNSIGNED DEFAULT 0,
    is_watched TINYINT(1) NOT NULL DEFAULT 0,
    watch_count INT UNSIGNED NOT NULL DEFAULT 0,

    -- Séries / épisodes
    series_name VARCHAR(255) DEFAULT NULL,
    season_number INT UNSIGNED DEFAULT NULL,
    episode_number INT UNSIGNED DEFAULT NULL,

    -- Gestion interne
    is_favorite TINYINT(1) NOT NULL DEFAULT 0,
    is_active TINYINT(1) NOT NULL DEFAULT 1,
    source_type VARCHAR(50) DEFAULT NULL,     -- ex: HDD, NAS, Raspberry, Upload

    -- Dates
    file_created_at DATETIME DEFAULT NULL,
    scanned_at DATETIME DEFAULT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

    -- Contraintes
    UNIQUE KEY uq_videos_file_path (file_path),
    UNIQUE KEY uq_videos_hash_sha256 (hash_sha256),

    -- Index utiles
    INDEX idx_videos_title (title),
    INDEX idx_videos_category (category),
    INDEX idx_videos_series (series_name),
    INDEX idx_videos_season_episode (season_number, episode_number),
    INDEX idx_videos_created_at (created_at),
    INDEX idx_videos_is_active (is_active),
    INDEX idx_videos_is_favorite (is_favorite)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
