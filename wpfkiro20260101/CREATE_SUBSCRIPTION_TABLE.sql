-- 創建 subscription 資料表 (單數，與應用程式一致)
-- 在 Supabase SQL Editor 中執行此腳本

-- 創建 subscription 資料表
CREATE TABLE IF NOT EXISTS subscription (
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    name TEXT,
    nextdate TEXT,
    price BIGINT DEFAULT 0,
    site TEXT,
    account TEXT,
    note TEXT
);

-- 啟用 Row Level Security (RLS)
ALTER TABLE subscription ENABLE ROW LEVEL SECURITY;

-- 創建允許所有操作的政策（開發環境用）
CREATE POLICY "Allow all operations on subscription" 
ON subscription FOR ALL 
USING (true);

-- 插入測試資料
INSERT INTO subscription (name, nextdate, price, site, account, note) VALUES
('Netflix', '2026-02-01', 390, 'https://netflix.com', 'test@example.com', '影音串流服務'),
('Spotify', '2026-01-15', 149, 'https://spotify.com', 'test@example.com', '音樂串流服務');

-- 確認資料表創建成功
SELECT 'subscription 資料表創建成功' as status;
SELECT COUNT(*) as record_count FROM subscription;