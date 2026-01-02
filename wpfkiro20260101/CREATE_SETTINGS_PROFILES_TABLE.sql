-- Appwrite 設定檔表創建腳本
-- 此腳本用於在 Appwrite 控制台中創建 settings_profiles 集合

-- 集合名稱: settings_profiles
-- 描述: 儲存應用程式的設定檔，支援多個後端服務配置

-- 欄位定義:

-- 1. profile_name (字串, 必填, 最大長度: 100)
--    設定檔名稱，用於識別不同的設定檔

-- 2. backend_service (字串, 必填, 最大長度: 50)
--    後端服務類型 (Appwrite, Supabase, NHost, Contentful, Back4App, MySQL, Strapi, Sanity)

-- 3. api_url (字串, 必填, 最大長度: 500)
--    API 端點 URL

-- 4. project_id (字串, 必填, 最大長度: 200)
--    專案 ID 或相關識別碼

-- 5. api_key (字串, 選填, 最大長度: 1000)
--    API 金鑰或存取權杖

-- 6. database_id (字串, 選填, 最大長度: 200)
--    資料庫 ID (主要用於 Appwrite)

-- 7. bucket_id (字串, 選填, 最大長度: 200)
--    儲存桶 ID (主要用於 Appwrite)

-- 8. food_collection_id (字串, 選填, 最大長度: 200)
--    食品集合 ID (主要用於 Appwrite)

-- 9. subscription_collection_id (字串, 選填, 最大長度: 200)
--    訂閱集合 ID (主要用於 Appwrite)

-- 10. description (字串, 選填, 最大長度: 500)
--     設定檔描述

-- 索引建議:
-- - profile_name (唯一索引)
-- - backend_service (一般索引)
-- - $createdAt (一般索引，用於排序)

-- 權限設定建議:
-- - 讀取: 任何已驗證用戶
-- - 創建: 任何已驗證用戶
-- - 更新: 文檔擁有者
-- - 刪除: 文檔擁有者

-- 在 Appwrite 控制台中的操作步驟:
-- 1. 進入 Databases 頁面
-- 2. 選擇您的資料庫 (通常是 default 或您設定的資料庫)
-- 3. 點擊 "Create Collection"
-- 4. 輸入集合名稱: settings_profiles
-- 5. 添加上述欄位，設定適當的類型和限制
-- 6. 設定索引和權限
-- 7. 儲存集合

-- 注意事項:
-- - 確保 profile_name 設定為唯一索引以避免重複名稱
-- - api_key 欄位包含敏感資訊，請確保適當的權限設定
-- - 建議定期備份設定檔資料
-- - 最多支援 100 筆設定檔記錄