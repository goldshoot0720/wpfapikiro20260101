-- NHost 資料表創建腳本
-- Region: eu-central-1
-- Subdomain: uxgwdiuehabbzenwtcqo
-- Admin Secret: cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr

-- =====================================================
-- 創建 Foods 表
-- =====================================================
CREATE TABLE IF NOT EXISTS foods (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name TEXT NOT NULL,
    price DECIMAL(10,2),
    photo TEXT,
    shop TEXT,
    todate TIMESTAMP WITH TIME ZONE,
    photohash TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- 創建 Foods 表的更新觸發器
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ language 'plpgsql';

CREATE TRIGGER update_foods_updated_at 
    BEFORE UPDATE ON foods 
    FOR EACH ROW 
    EXECUTE FUNCTION update_updated_at_column();

-- =====================================================
-- 創建 Subscriptions 表
-- =====================================================
CREATE TABLE IF NOT EXISTS subscriptions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name TEXT NOT NULL,
    nextdate TIMESTAMP WITH TIME ZONE,
    price DECIMAL(10,2),
    site TEXT,
    note TEXT,
    account TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- 創建 Subscriptions 表的更新觸發器
CREATE TRIGGER update_subscriptions_updated_at 
    BEFORE UPDATE ON subscriptions 
    FOR EACH ROW 
    EXECUTE FUNCTION update_updated_at_column();

-- =====================================================
-- 插入測試資料
-- =====================================================

-- 插入測試食品資料
INSERT INTO foods (name, price, photo, shop, todate, photohash) VALUES
('蘋果', 50.00, 'apple.jpg', '水果店', NOW() + INTERVAL '7 days', 'hash_apple_001'),
('香蕉', 30.00, 'banana.jpg', '水果店', NOW() + INTERVAL '5 days', 'hash_banana_002'),
('牛奶', 80.00, 'milk.jpg', '超市', NOW() + INTERVAL '10 days', 'hash_milk_003'),
('麵包', 45.00, 'bread.jpg', '麵包店', NOW() + INTERVAL '3 days', 'hash_bread_004'),
('雞蛋', 120.00, 'eggs.jpg', '超市', NOW() + INTERVAL '14 days', 'hash_eggs_005');

-- 插入測試訂閱資料
INSERT INTO subscriptions (name, nextdate, price, site, note, account) VALUES
('Netflix', NOW() + INTERVAL '30 days', 390.00, 'netflix.com', '影音串流服務', 'user@example.com'),
('Spotify', NOW() + INTERVAL '30 days', 149.00, 'spotify.com', '音樂串流服務', 'user@example.com'),
('Adobe Creative Cloud', NOW() + INTERVAL '30 days', 1680.00, 'adobe.com', '創意軟體套件', 'user@example.com'),
('Microsoft 365', NOW() + INTERVAL '30 days', 219.00, 'microsoft.com', '辦公軟體套件', 'user@example.com'),
('GitHub Pro', NOW() + INTERVAL '30 days', 4.00, 'github.com', '程式碼託管服務', 'user@example.com');

-- =====================================================
-- 創建索引以提升查詢效能
-- =====================================================

-- Foods 表索引
CREATE INDEX IF NOT EXISTS idx_foods_name ON foods(name);
CREATE INDEX IF NOT EXISTS idx_foods_shop ON foods(shop);
CREATE INDEX IF NOT EXISTS idx_foods_todate ON foods(todate);
CREATE INDEX IF NOT EXISTS idx_foods_created_at ON foods(created_at);

-- Subscriptions 表索引
CREATE INDEX IF NOT EXISTS idx_subscriptions_name ON subscriptions(name);
CREATE INDEX IF NOT EXISTS idx_subscriptions_site ON subscriptions(site);
CREATE INDEX IF NOT EXISTS idx_subscriptions_nextdate ON subscriptions(nextdate);
CREATE INDEX IF NOT EXISTS idx_subscriptions_created_at ON subscriptions(created_at);

-- =====================================================
-- 查詢測試
-- =====================================================

-- 測試查詢所有食品
-- SELECT * FROM foods ORDER BY created_at DESC;

-- 測試查詢所有訂閱
-- SELECT * FROM subscriptions ORDER BY created_at DESC;

-- 測試查詢即將到期的食品 (7天內)
-- SELECT * FROM foods WHERE todate <= NOW() + INTERVAL '7 days' ORDER BY todate ASC;

-- 測試查詢即將續費的訂閱 (30天內)
-- SELECT * FROM subscriptions WHERE nextdate <= NOW() + INTERVAL '30 days' ORDER BY nextdate ASC;