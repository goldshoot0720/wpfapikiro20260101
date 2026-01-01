-- Supabase 資料庫設定腳本
-- 專案 ID: lobezwpworbfktlkxuyo
-- 在 Supabase Dashboard 的 SQL Editor 中執行此腳本

-- ================================
-- 1. 創建 foods 資料表
-- ================================
CREATE TABLE IF NOT EXISTS foods (
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    food_name VARCHAR(255) NOT NULL,
    price INTEGER DEFAULT 0,
    quantity INTEGER DEFAULT 1,
    photo TEXT,
    photo_hash TEXT,
    shop VARCHAR(255),
    to_date VARCHAR(50),
    description TEXT,
    category VARCHAR(100),
    storage_location VARCHAR(100),
    note TEXT,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW()
);

-- ================================
-- 2. 創建 subscriptions 資料表
-- ================================
CREATE TABLE IF NOT EXISTS subscriptions (
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    subscription_name VARCHAR(255) NOT NULL,
    next_date DATE,
    price INTEGER DEFAULT 0,
    site TEXT,
    account VARCHAR(255),
    note TEXT,
    string_to_date VARCHAR(50),
    date_time TIMESTAMPTZ,
    food_id UUID REFERENCES foods(id) ON DELETE SET NULL,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW()
);

-- ================================
-- 3. 啟用 Row Level Security (RLS)
-- ================================
ALTER TABLE foods ENABLE ROW LEVEL SECURITY;
ALTER TABLE subscriptions ENABLE ROW LEVEL SECURITY;

-- ================================
-- 4. 創建 RLS 政策（開發環境用）
-- ================================
-- 允許所有操作的政策（適用於開發環境）
CREATE POLICY IF NOT EXISTS "Allow all operations on foods" 
ON foods FOR ALL 
USING (true);

CREATE POLICY IF NOT EXISTS "Allow all operations on subscriptions" 
ON subscriptions FOR ALL 
USING (true);

-- ================================
-- 5. 創建索引以提升查詢效能
-- ================================
-- Foods 資料表索引
CREATE INDEX IF NOT EXISTS idx_foods_food_name ON foods(food_name);
CREATE INDEX IF NOT EXISTS idx_foods_category ON foods(category);
CREATE INDEX IF NOT EXISTS idx_foods_shop ON foods(shop);
CREATE INDEX IF NOT EXISTS idx_foods_created_at ON foods(created_at);
CREATE INDEX IF NOT EXISTS idx_foods_to_date ON foods(to_date);

-- Subscriptions 資料表索引
CREATE INDEX IF NOT EXISTS idx_subscriptions_name ON subscriptions(subscription_name);
CREATE INDEX IF NOT EXISTS idx_subscriptions_next_date ON subscriptions(next_date);
CREATE INDEX IF NOT EXISTS idx_subscriptions_site ON subscriptions(site);
CREATE INDEX IF NOT EXISTS idx_subscriptions_created_at ON subscriptions(created_at);
CREATE INDEX IF NOT EXISTS idx_subscriptions_food_id ON subscriptions(food_id);

-- ================================
-- 6. 創建更新時間觸發器
-- ================================
-- 創建更新 updated_at 欄位的函數
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ language 'plpgsql';

-- 為 foods 資料表創建觸發器
DROP TRIGGER IF EXISTS update_foods_updated_at ON foods;
CREATE TRIGGER update_foods_updated_at
    BEFORE UPDATE ON foods
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

-- 為 subscriptions 資料表創建觸發器
DROP TRIGGER IF EXISTS update_subscriptions_updated_at ON subscriptions;
CREATE TRIGGER update_subscriptions_updated_at
    BEFORE UPDATE ON subscriptions
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

-- ================================
-- 7. 插入測試資料（可選）
-- ================================
-- 插入測試食品資料
INSERT INTO foods (food_name, price, shop, to_date, category, note) VALUES
('蘋果', 50, '全聯', '2026-01-15', '水果', '新鮮蘋果'),
('牛奶', 80, '7-11', '2026-01-10', '乳製品', '低脂牛奶'),
('麵包', 35, '麵包店', '2026-01-08', '主食', '全麥麵包')
ON CONFLICT DO NOTHING;

-- 插入測試訂閱資料
INSERT INTO subscriptions (subscription_name, next_date, price, site, note) VALUES
('Netflix', '2026-02-01', 390, 'https://netflix.com', '影音串流服務'),
('Spotify', '2026-01-15', 149, 'https://spotify.com', '音樂串流服務'),
('Adobe Creative Cloud', '2026-01-20', 672, 'https://adobe.com', '創意軟體套件')
ON CONFLICT DO NOTHING;

-- ================================
-- 8. 驗證資料表創建
-- ================================
-- 檢查資料表是否成功創建
SELECT 
    table_name,
    column_name,
    data_type,
    is_nullable
FROM information_schema.columns 
WHERE table_name IN ('foods', 'subscriptions')
ORDER BY table_name, ordinal_position;

-- 檢查 RLS 是否啟用
SELECT 
    schemaname,
    tablename,
    rowsecurity
FROM pg_tables 
WHERE tablename IN ('foods', 'subscriptions');

-- 檢查政策是否創建
SELECT 
    schemaname,
    tablename,
    policyname,
    permissive,
    roles,
    cmd,
    qual
FROM pg_policies 
WHERE tablename IN ('foods', 'subscriptions');

-- ================================
-- 完成訊息
-- ================================
DO $$
BEGIN
    RAISE NOTICE '=================================';
    RAISE NOTICE 'Supabase 資料庫設定完成！';
    RAISE NOTICE '=================================';
    RAISE NOTICE '已創建的資料表:';
    RAISE NOTICE '- foods (食品管理)';
    RAISE NOTICE '- subscriptions (訂閱管理)';
    RAISE NOTICE '';
    RAISE NOTICE '已啟用功能:';
    RAISE NOTICE '- Row Level Security (RLS)';
    RAISE NOTICE '- 自動更新時間戳記';
    RAISE NOTICE '- 查詢效能索引';
    RAISE NOTICE '- 測試資料';
    RAISE NOTICE '';
    RAISE NOTICE '現在可以在應用程式中使用 Supabase 服務了！';
    RAISE NOTICE '=================================';
END $$;