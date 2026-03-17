/* Newtube */

CREATE TABLE categories (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL
);

CREATE TABLE videos (
    id INT AUTO_INCREMENT PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    description TEXT NULL,
    file_path VARCHAR(500) NOT NULL,
    file_name VARCHAR(255) NOT NULL,
    hash_sha256 VARCHAR(64) NULL,
    cover_path VARCHAR(500) NULL,
    size_bytes BIGINT NOT NULL,
    duration_sec INT NOT NULL,
    resolution VARCHAR(50) NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NULL,
    is_published TINYINT(1) NOT NULL DEFAULT 1,
    is_deleted TINYINT(1) NOT NULL DEFAULT 0,
    views BIGINT NOT NULL DEFAULT 0,
    category_id INT NOT NULL,
    uploaded_by VARCHAR(100) NULL,
    CONSTRAINT fk_videos_category
        FOREIGN KEY (category_id) REFERENCES categories(id)
);

CREATE TABLE watch_progress (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    video_id INT NOT NULL,
    current_time DOUBLE NOT NULL DEFAULT 0,
    duration DOUBLE NULL,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
        ON UPDATE CURRENT_TIMESTAMP,
    UNIQUE KEY uq_user_video (user_id, video_id)
);


CREATE TABLE users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(100) NOT NULL UNIQUE,
    email VARCHAR(255) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE video_likes (
    user_id INT NOT NULL,
    video_id INT NOT NULL,
    is_like TINYINT(1) NOT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (user_id, video_id),
    FOREIGN KEY (user_id) REFERENCES users(id),
    FOREIGN KEY (video_id) REFERENCES videos(id)
);


CREATE TABLE comments (
    id INT AUTO_INCREMENT PRIMARY KEY,
    video_id INT NOT NULL,
    user_id INT NOT NULL,
    content TEXT NOT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (video_id) REFERENCES videos(id),
    FOREIGN KEY (user_id) REFERENCES users(id)
);

CREATE TABLE tags (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE video_tags (
    video_id INT NOT NULL,
    tag_id INT NOT NULL,
    PRIMARY KEY (video_id, tag_id),
    FOREIGN KEY (video_id) REFERENCES videos(id),
    FOREIGN KEY (tag_id) REFERENCES tags(id)
);

#1. Vidéos les plus publié
SELECT *
FROM videos
WHERE is_published = 1 AND is_deleted = 0
ORDER BY created_at DESC;

#2. Lister les vidéos d’une catégorie
SELECT v.*
FROM videos v
JOIN categories c ON v.category_id = c.id
WHERE c.name = 'Musique'
  AND v.is_published = 1
  AND v.is_deleted = 0
ORDER BY v.created_at DESC;

#4. Top vidéos (par vues)
SELECT id, title, views
FROM videos
WHERE is_published = 1 AND is_deleted = 0
ORDER BY views DESC
LIMIT 20;


#5.
    SELECT id, title, created_at
FROM videos
WHERE is_published = 1 AND is_deleted = 0
ORDER BY created_at DESC
LIMIT 20;


#Incrémenter le nombre de vues d’une vidéo
DELIMITER $$
CREATE PROCEDURE increment_video_views(IN p_video_id INT)
BEGIN
    UPDATE videos
    SET views = views + 1
    WHERE id = p_video_id AND is_deleted = 0;
END $$
DELIMITER ;


# 2.Mettre à jour la progression d’un utilisateur
DELIMITER $$
CREATE PROCEDURE update_watch_progress(
    IN p_user_id INT,
    IN p_video_id INT,
    IN p_current_time DOUBLE,
    IN p_duration DOUBLE
)
BEGIN
    INSERT INTO watch_progress (user_id, video_id, current_time, duration)
    VALUES (p_user_id, p_video_id, p_current_time, p_duration)
    ON DUPLICATE KEY UPDATE
        current_time = p_current_time,
        duration = p_duration,
        updated_at = CURRENT_TIMESTAMP;
END $$
DELIMITER ;


# Lister les vidéos d’une catégorie
DELIMITER $$
CREATE PROCEDURE get_videos_by_category(IN p_category_id INT)
BEGIN
    SELECT *
    FROM videos
    WHERE category_id = p_category_id
      AND is_published = 1
      AND is_deleted = 0
    ORDER BY created_at DESC;
END $$
DELIMITER ;


#Publier / dépublier une vidéo
DELIMITER $$
CREATE PROCEDURE set_video_publish_status(
    IN p_video_id INT,
    IN p_status TINYINT
)
BEGIN
    UPDATE videos
    SET is_published = p_status,
        updated_at = CURRENT_TIMESTAMP
    WHERE id = p_video_id AND is_deleted = 0;
END $$
DELIMITER ;



#audit #audit #audit #audit #audit #audit #audit #audit #audit #audit #audit #audit #audit 

    
CREATE TABLE video_audit (
    id INT AUTO_INCREMENT PRIMARY KEY,
    video_id INT NOT NULL,
    action VARCHAR(50) NOT NULL,          -- INSERT, UPDATE, DELETE, SOFT_DELETE, PUBLISH...
    old_data JSON NULL,
    new_data JSON NULL,
    changed_by VARCHAR(100) NULL,
    changed_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE system_logs (
    id INT AUTO_INCREMENT PRIMARY KEY,
    event_type VARCHAR(50) NOT NULL,   -- LOGIN, ERROR, VIDEO_VIEW, API_CALL...
    message TEXT NOT NULL,
    user_id INT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);


#audit #audit #audit #audit #audit #audit #audit #audit #audit #audit #audit #audit #audit 

