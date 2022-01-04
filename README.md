# ProjectManageSystemBackend

## Project Build

使用visual studio開起，組態選擇project-manage-system-backend 開起
API測試
```
GET http:\\localhost:5001\api\user
Response User Model
```

EF Core
```
dotnet tool install --global dotnet-ef --version 5.0.2 //安裝EFCORE TOOL
dotnet ef migrations add InitialCreate //當有變動到DB MODEL時需執行
dotnet ef database update //更新資料庫
```
## 2022.01.04進度

可以把前端輸入的jenkins相關資料存入資料庫了，但目前尚未有任何驗證與報錯機制，盡量本周五前改完。


[參考資料](https://docs.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli)