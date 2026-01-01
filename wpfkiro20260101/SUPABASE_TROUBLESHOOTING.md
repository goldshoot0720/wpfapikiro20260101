# Supabase 故障排除指南

## 🚨 常見錯誤：NotFound (404)

### 可能原因和解決方案

#### 1. 資料表名稱問題
**症狀**: API 回應 404 NotFound
**原因**: 資料表名稱可能與程式碼中的不一致

**檢查步驟**:
1. 在 Supabase Dashboard 中確認實際的資料表名稱
2. 檢查是否為單數或複數形式
3. 檢查大小寫是否正確

**可能的資料表名稱**:
- `food` (單數)
- `foods` (複數)
- `Food` (大寫開頭)
- `Foods` (大寫複數)

#### 2. Row Level Security (RLS) 問題
**症狀**: API 回應 404 或 403
**原因**: RLS 政策阻止了資料存取

**解決方案**:
```sql
-- 在 Supabase SQL Editor 中執行
ALTER TABLE food ENABLE ROW LEVEL SECURITY;
ALTER TABLE subscriptions ENABLE ROW LEVEL SECURITY;

-- 創建允許所有操作的政策（開發環境用）
CREATE POLICY "Allow all operations on food" 
ON food FOR ALL 
USING (true);

CREATE POLICY "Allow all operations on subscriptions" 
ON subscriptions FOR ALL 
USING (true);
```

#### 3. API 金鑰權限問題
**症狀**: 連接失敗或權限錯誤
**原因**: 使用了錯誤的 API 金鑰

**檢查項目**:
- ✅ 使用 `service_role` 金鑰（以 `sb_secret_` 開頭）
- ❌ 不要使用 `anon` 金鑰（以 `sb_publishable_` 開頭）

**正確配置**:
```
API Key: sb_secret_B2gtQik_DZEKevBc82viAw_mbvPA8F1
```

#### 4. API URL 問題
**症狀**: 連接完全失敗
**原因**: API URL 不正確

**正確格式**:
```
API URL: https://lobezwpworbfktlkxuyo.supabase.co
```

**注意**: 不要包含 `/rest/v1/` 後綴

## 🔧 診斷步驟

### 步驟 1: 基本連接測試
```bash
curl -H "apikey: sb_secret_B2gtQik_DZEKevBc82viAw_mbvPA8F1" \
     -H "Authorization: Bearer sb_secret_B2gtQik_DZEKevBc82viAw_mbvPA8F1" \
     https://lobezwpworbfktlkxuyo.supabase.co/rest/v1/
```

### 步驟 2: 測試資料表存取
```bash
# 測試 food 資料表
curl -H "apikey: sb_secret_B2gtQik_DZEKevBc82viAw_mbvPA8F1" \
     -H "Authorization: Bearer sb_secret_B2gtQik_DZEKevBc82viAw_mbvPA8F1" \
     https://lobezwpworbfktlkxuyo.supabase.co/rest/v1/food

# 測試 subscription 資料表
curl -H "apikey: sb_secret_B2gtQik_DZEKevBc82viAw_mbvPA8F1" \
     -H "Authorization: Bearer sb_secret_B2gtQik_DZEKevBc82viAw_mbvPA8F1" \
     https://lobezwpworbfktlkxuyo.supabase.co/rest/v1/subscription
```

### 步驟 3: 檢查資料表結構
在 Supabase Dashboard 中：
1. 進入 Table Editor
2. 確認資料表名稱
3. 檢查欄位名稱和類型
4. 確認 RLS 設定

## 🛠️ 修正方案

### 方案 1: 更新資料表名稱
如果實際資料表名稱不同，更新 SupabaseService.cs：

```csharp
// 如果資料表名稱是 foods（複數）
var response = await _httpClient.GetAsync($"{_settings.ApiUrl}/rest/v1/foods");

// 如果資料表名稱是 Food（大寫）
var response = await _httpClient.GetAsync($"{_settings.ApiUrl}/rest/v1/Food");
```

### 方案 2: 重新創建資料表
如果資料表不存在，在 Supabase SQL Editor 中執行：

```sql
-- 創建 food 資料表
CREATE TABLE food (
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    name TEXT,
    todate TEXT,
    account TEXT,
    photo TEXT,
    price BIGINT,
    shop TEXT
);

-- 創建 subscriptions 資料表
CREATE TABLE subscriptions (
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    name TEXT,
    nextdate DATE,
    price BIGINT,
    site TEXT,
    note TEXT,
    account TEXT
);
```

### 方案 3: 設定 RLS 政策
```sql
-- 啟用 RLS
ALTER TABLE food ENABLE ROW LEVEL SECURITY;
ALTER TABLE subscriptions ENABLE ROW LEVEL SECURITY;

-- 創建允許所有操作的政策
CREATE POLICY "Allow all operations on food" ON food FOR ALL USING (true);
CREATE POLICY "Allow all operations on subscriptions" ON subscriptions FOR ALL USING (true);
```

## 📋 檢查清單

在聯繫支援前，請確認以下項目：

- [ ] API URL 正確：`https://lobezwpworbfktlkxuyo.supabase.co`
- [ ] API Key 正確：`sb_secret_B2gtQik_DZEKevBc82viAw_mbvPA8F1`
- [ ] 資料表存在且名稱正確
- [ ] RLS 政策已設定
- [ ] 網路連接正常
- [ ] Supabase 專案狀態正常

## 🔍 使用診斷工具

執行內建的診斷測試：

```csharp
// 在應用程式中執行
var debugTest = new SupabaseDebugTest();
await debugTest.RunDiagnosticTests();
```

這將測試：
- 基本連接
- API 根路徑
- 各種資料表名稱變體
- 詳細的錯誤訊息

## 📞 獲取幫助

如果問題仍然存在：

1. **檢查 Supabase 狀態**: https://status.supabase.com/
2. **查看 Supabase 文件**: https://supabase.com/docs
3. **檢查專案設定**: https://supabase.com/dashboard/project/lobezwpworbfktlkxuyo

## 🎯 快速修正

最常見的修正方法：

1. **確認資料表名稱**: 在 Dashboard 中檢查實際名稱
2. **重設 RLS**: 停用後重新啟用 RLS 政策
3. **重新生成 API 金鑰**: 在專案設定中重新生成
4. **清除快取**: 重新啟動應用程式