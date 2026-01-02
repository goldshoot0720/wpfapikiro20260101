-- 創建 Supabase 表結構 (最終版本 - 基於實際表結構)
-- 在 Supabase SQL Editor 中執行此腳本

-- ================================
-- 1. 創建 food 資料表
-- ================================
CREATE TABLE IF NOT EXISTS food (
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    name TEXT,
    todate TIMESTAMPTZ,
    amount INTEGER DEFAULT 1,
    photo TEXT,
    price BIGINT DEFAULT 0,
    shop TEXT,
    photohash TEXT
);

-- ================================
-- 2. 創建 subscription 資料表
-- ================================
CREATE TABLE IF NOT EXISTS subscription (
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    name TEXT,
    nextdate DATE,
    price BIGINT DEFAULT 0,
    site TEXT,
    note TEXT,
    account TEXT
);

-- ================================
-- 3. 啟用 Row Level Security (RLS)
-- ================================
ALTER TABLE food ENABLE ROW LEVEL SECURITY;
ALTER TABLE subscription ENABLE ROW LEVEL SECURITY;

-- ================================
-- 4. 創建 RLS 政策（開發環境用）
-- ================================
-- 允許所有操作的政策（適用於開發環境）
CREATE POLICY IF NOT EXISTS "Allow all operations on food" 
ON food FOR ALL 
USING (true);

CREATE POLICY IF NOT EXISTS "Allow all operations on subscription" 
ON subscription FOR ALL 
USING (true);

-- ================================
-- 5. 插入測試資料
-- ================================
-- 插入測試食品資料
INSERT INTO food (name, todate, amount, photo, price, shop, photohash) VALUES
('測試蘋果', '2026-02-01 00:00:00+00', 1, 'https://example.com/apple.jpg', 50, '測試商店', ''),
('測試香蕉', '2026-01-25 00:00:00+00', 2, 'https://example.com/banana.jpg', 30, '水果店', '')
ON CONFLICT DO NOTHING;

-- 插入測試訂閱資料
INSERT INTO subscription (name, nextdate, price, site, note, account) VALUES
('Netflix', '2026-02-01', 390, 'https://netflix.com', '影音串流服務', 'test@example.com'),
('Spotify', '2026-01-15', 149, 'https://spotify.com', '音樂串流服務', 'test@example.com')
ON CONFLICT DO NOTHING;

-- ================================
-- 6. 驗證資料表創建
-- ================================
-- 檢查資料表是否成功創建
SELECT 
    table_name,
    column_name,
    data_type,
    is_nullable
FROM information_schema.columns 
WHERE table_name IN ('food', 'subscription')
ORDER BY table_name, ordinal_position;

-- 檢查資料筆數
SELECT 'food' as table_name, COUNT(*) as record_count FROM food
UNION ALL
SELECT 'subscription' as table_name, COUNT(*) as record_count FROM subscription;

-- ================================
-- 完成訊息
-- ================================
DO $
BEGIN
    RAISE NOTICE '=================================';
    RAISE NOTICE 'Supabase 表結構創建完成！';
    RAISE NOTICE '=================================';
    RAISE NOTICE 'Food 表結構: id,created_at,name,todate,amount,photo,price,shop,photohash';
    RAISE NOTICE 'Subscription 表結構: id,created_at,name,nextdate,price,site,note,account';
    RAISE NOTICE '';
    RAISE NOTICE '現在可以使用正確的 CSV 格式導入資料了！';
    RAISE NOTICE '=================================';
END $;