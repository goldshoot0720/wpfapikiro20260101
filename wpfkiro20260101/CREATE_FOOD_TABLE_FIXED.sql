-- 創建 food 資料表 (修正版 - 移除 updated_at)
-- 在 Supabase SQL Editor 中執行此腳本

-- 創建 food 資料表
CREATE TABLE IF NOT EXISTS food (
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    name TEXT,
    price BIGINT DEFAULT 0,
    photo TEXT,
    shop TEXT,
    todate TEXT,
    account TEXT
);

-- 啟用 Row Level Security (RLS)
ALTER TABLE food ENABLE ROW LEVEL SECURITY;

-- 創建允許所有操作的政策（開發環境用）
CREATE POLICY "Allow all operations on food" 
ON food FOR ALL 
USING (true);

-- 插入測試資料
INSERT INTO food (name, price, photo, shop, todate, account) VALUES
('測試蘋果', 50, 'https://example.com/apple.jpg', '測試商店', '2026-02-01', 'test@example.com'),
('測試香蕉', 30, 'https://example.com/banana.jpg', '水果店', '2026-01-25', 'test@example.com');

-- 確認資料表創建成功
SELECT 'food 資料表創建成功' as status;
SELECT COUNT(*) as record_count FROM food;