# geoai-api（ASP.NET Core の API）

全体ルールはワークスペースの `../CLAUDE.md`、仕様は `../spec.md` にある。このファイルは API 固有のルール。

## 担当範囲

- 業務 API（解析範囲、教師データの受付、ジョブ登録、結果の返却）
- DB のテーブル構造（**スキーマの持ち主は API のみ**。EF Core のマイグレーションで管理）
- Redis Streams へのジョブ投入、停止ジョブの回収
- Phase 3：LLM による実行計画の作成と検証

## 主に読む spec.md の章

| 章 | 内容 |
|---|---|
| 5, 6 | 環境変数、定数 |
| 7 | ドメイン定義（分類クラス、ジョブ状態、ジョブ種別） |
| 8 | データベース（テーブル定義、ワーカー用ロールの権限） |
| 9 | API 仕様 |
| 10, 11 | 実行計画の形式、キュー仕様 |
| 16 | 自然言語入力（Phase 3） |
| 17, 18 | セキュリティ、エラー処理 |

## コマンド

```bash
dotnet restore
dotnet build
dotnet test                                        # テスト（Testcontainers を使うため Docker が必要）
dotnet run --project src/Api                       # ローカル起動（http://localhost:8080）
dotnet ef migrations add <名前> --project src/Api.Infrastructure --startup-project src/Api
dotnet ef database update --project src/Api.Infrastructure --startup-project src/Api
```

## 規約

- プロジェクト構成は spec.md 4.3 のとおり（Api / Api.Domain / Api.Infrastructure / Api.Application）。
- DB の名前はすべて snake_case（EFCore.NamingConventions）。主キーは時刻順 UUID。
- **テナントの取得は必ず `ITenantContext` 経由**。EF Core のグローバルクエリフィルターで `tenant_id` を自動で絞り込む。`IgnoreQueryFilters()` は停止ジョブ回収以外で使わない。
- 他テナントのリソースを指定されたら 404 を返す。
- エラー応答は Problem Details（RFC 9457）形式。
- **C# で空間演算（座標変換・バッファ・面積計算）をしない**。必要なら PostGIS の SQL 関数（例：`ST_Area(geom::geography)`）を使う。
- 分類クラス・ジョブ状態の列挙型は `contracts/domain.json` と一致させ、一致を確認するテストを置く。
- 実行計画の検証には `contracts/plan.v1.json`（JSON Schema）を使う。
- `contracts/` は直接編集しない（ワークスペースの `sync-contracts.sh` で更新される）。

## テスト

- xUnit + Testcontainers（本物の PostgreSQL/PostGIS と Redis をテスト用コンテナで起動）。DB を使うテストでモックは使わない。
- テナント分離のテスト（テナント A のデータをテナント B の文脈で取得できないこと）は必須。
