# ระบบ Database — Fruity Flurfy

เอกสารอธิบายระบบเก็บข้อมูลของเกม โค้ดทั้งหมดอยู่ที่ `Assets/Script/DataBase/`

เกมใช้ฐานข้อมูล 2 ส่วน:

| ส่วน | เทคโนโลยี | เก็บอะไร | ที่เก็บ |
|---|---|---|---|
| Local DB | SQLite (`sqlite-net`) | ผู้เล่น, เซฟ, ไอเทม, buff, achievement | ไฟล์ `game3.db` ใน `Application.persistentDataPath` |
| Leaderboard | Firebase Realtime Database | wave สูงสุดของผู้เล่นแต่ละคน | Cloud (ไม่ทำงานบน WebGL) |

---

## 1. สถาปัตยกรรม

```
UI / Gameplay                 Facade                    Repository              Table
─────────────                 ──────                    ──────────              ─────
LoginUIManager ─────────────────────────────────────▶ PlayerRepository ───────▶ Players
SaveUIManager  ──▶ GameSaver ───────────────────────▶ SaveRepository ─────────▶ SaveData
WaveManager    ──▶ GameSaver ──▶ InventorySaveUI ───▶ InventoryRepository ────▶ Inventory
BuffManager    ─────────────────────────────────────▶ PlayerBuffRepository ──▶ PlayerBuffs
EnemyControl   ──▶ AchievementTracker ──────────────▶ AchievementRepository ──▶ Achievements
GameSaver      ──▶ LeaderBoard.SubmitWave ──────────▶ Firebase /leaderboard
```

- **`DbProvider`** — static class ให้ `SQLiteConnection` (`DbProvider.Connection`)
- **`GameSession`** — static state ว่าใครล็อกอินอยู่ (`PlayerId`, `IsLoggedIn`, `LogOut()`) และชื่อ scene (`StartScene`, `Forest`)
- **Repository** — class ที่ครอบ SQL ของแต่ละตาราง สร้างตารางเองใน constructor ด้วย `db.CreateTable<T>()` (ไม่มี migration แยก)
- **`GameSaver`** — facade รวมลำดับการเซฟ/โหลด ให้ gameplay เรียกจุดเดียว

ทุกฟังก์ชันที่ผูกกับผู้เล่นจะ `return` เงียบ ๆ ถ้า `!GameSession.IsLoggedIn` (โหมด Guest ไม่เซฟอะไร)

> ⚠️ `DbProvider.Connection` สร้าง `SQLiteConnection` **ใหม่ทุกครั้งที่เรียก** (และ log path ทุกครั้ง) ไม่ได้ cache ไว้ (เดิมเคย cache แต่ถูกเอาออกในเวอร์ชันปัจจุบัน) ดู [ข้อควรระวัง](#7-ข้อควรระวัง--ข้อสังเกต)

---

## 2. Schema (SQLite)

### `Players` — [Players.cs](../Assets/Script/DataBase/Player/Players.cs)
| คอลัมน์ | ชนิด | หมายเหตุ |
|---|---|---|
| `Id` | int | PK, AutoIncrement |
| `Name` | string(50) | Unique — ใช้เป็น username (ไม่มีรหัสผ่าน) |
| `Wave` | int | default 1 — wave ปัจจุบันของรอบ |
| `Gold` | int | default 0 |
| `Kill` | int | default 0 — kill สะสมตลอดกาล |

### `SaveData` — [SaveData.cs](../Assets/Script/DataBase/Save/SaveData.cs)
เซฟรอบปัจจุบัน 1 แถวต่อผู้เล่น
| คอลัมน์ | ชนิด | หมายเหตุ |
|---|---|---|
| `Id` | int | PK, AutoIncrement |
| `PlayerId` | int | Unique |
| `SceneName` | string(50) | scene ที่จะโหลดตอนเข้าเกม |
| `Wave` | int | default 1 |
| `Gold` | int | default 0 |
| `Kill` | int | default 0 — kill ของรอบปัจจุบัน (`GameSaver.RunKills`) |

### `Inventory` — [InventoryRow.cs](../Assets/Script/DataBase/Inventory/InventoryRow.cs)
ไอเทมที่ซื้อ 1 แถวต่อ (ผู้เล่น, ไอเทม) — **composite PK `(PlayerId, ItemId)`** (`ItemId` = `ShopData.id` เป็น int ที่กำหนดใน Inspector) สร้างด้วย `CREATE TABLE IF NOT EXISTS` ก่อน `CreateTable<T>()`
| คอลัมน์ | ชนิด | หมายเหตุ |
|---|---|---|
| `PlayerId` | int | PK (ส่วนที่ 1) |
| `ItemId` | int | PK (ส่วนที่ 2) = `ShopData.id` |
| `PlayerName` | string(50) | เก็บซ้ำเพื่ออ่านง่าย |
| `ItemName` | string(50) | เก็บซ้ำเพื่ออ่านง่าย |
| `CurrentRun` | int | จำนวนในรอบนี้ (รีเซ็ตเมื่อตาย) |
| `AllRun` | int | จำนวนที่ซื้อสะสมทุกรอบ |

### `PlayerBuffs` — [Buffs.cs](../Assets/Script/DataBase/Buff/Buffs.cs) (`PlayerBuffRow`)
**composite PK `(PlayerId, BuffId)`** (`BuffId` = `BuffData.id` เป็น int ที่กำหนดใน Inspector) สร้างด้วย `CREATE TABLE IF NOT EXISTS` ก่อน `CreateTable<T>()`
| คอลัมน์ | ชนิด | หมายเหตุ |
|---|---|---|
| `PlayerId` | int | PK (ส่วนที่ 1) → Players |
| `BuffId` | int | PK (ส่วนที่ 2) = `BuffData.id` |
| `PlayerName` | string(50) | เก็บซ้ำเพื่ออ่านง่าย |
| `BuffName` | string(50) | เก็บซ้ำเพื่ออ่านง่าย |
| `CurrentRun` | int | จำนวนครั้งที่เลือกในรอบนี้ |
| `AllRun` | int | สะสมทุกรอบ |

### `Achievements` — [AchievementRow.cs](../Assets/Script/DataBase/Achievement/AchievementRow.cs)
| คอลัมน์ | ชนิด | หมายเหตุ |
|---|---|---|
| `Id` | int | PK, AutoIncrement |
| `PlayerId` | int | Unique composite `PlayerAchievement` (0) |
| `AchievementId` | string(50) | = `AchievementData.id`, Unique composite (1) |
| `Progress` | int | ค่าสูงสุดที่เคยทำได้ (ไม่ลดลง) |
| `UnlockedAt` | string(30) | เวลา UTC รูปแบบ ISO-8601 (`"o"`) — ว่าง = ยังไม่ปลดล็อก |

### ER Diagram
```
Players 1───1 SaveData          (PlayerId)
Players 1───* Inventory         (PlayerId, ItemId = ShopData.id)
Players 1───* PlayerBuffs       (PlayerId, BuffId = BuffData.id)
Players 1───* Achievements      (PlayerId)
```
ไม่มีการประกาศ Foreign Key constraint จริงใน SQLite — เป็นความสัมพันธ์ตามชื่อคอลัมน์เท่านั้น

---

## 3. Repository API

### `PlayerRepository`
| เมธอด | ทำอะไร |
|---|---|
| `AddPlayer(name)` | Insert ผู้เล่นใหม่ คืน `Players` (พร้อม `Id`) |
| `GetByName(name)` / `GetById(id)` | ค้นหา คืน `null` ถ้าไม่เจอ |
| `UpdateProgress(id, wave, gold)` | อัปเดต `Wave`, `Gold` |
| `AddKills(id, kills)` | `Kill = Kill + ?` |

### `SaveRepository`
| เมธอด | ทำอะไร |
|---|---|
| `Save(playerId, scene, wave, gold, kill = 0)` | ลบแถวเก่าแล้ว insert ใหม่ ใน transaction เดียว |
| `Load(playerId)` | คืน `SaveData` หรือ `null` |
| `Clear(playerId)` | ลบเซฟ |

### `InventoryRepository`
| เมธอด | ทำอะไร |
|---|---|
| `SetupRow(playerId, shopItems)` | รับ `ShopData` สร้างแถว (0/0) ให้ไอเทมที่ผู้เล่นยังไม่มี |
| `SaveQuantities(playerId, entries)` | `entries` คือ `(ShopData, Quantity)` — `UPDATE` `CurrentRun` ตามจำนวนปัจจุบัน และเพิ่ม `AllRun` ด้วยส่วนต่างที่เพิ่มขึ้น (ไม่ติดลบ) แล้ว insert ไอเทมที่ยังไม่มีแถว |
| `GetActive(playerId)` | แถวที่ `CurrentRun > 0` |
| `GetAll(playerId)` | ทุกแถว |
| `ResetRun(playerId)` | `CurrentRun = 0` |

### `PlayerBuffRepository`
| เมธอด | ทำอะไร |
|---|---|
| `SetupRow(playerId, buffs)` | รับ `BuffData` สร้างแถว `PlayerBuffs` ที่ยังไม่มี และอัปเดต `PlayerName` ของแถวเดิม |
| `AddPick(playerId, buff)` | (`BuffData`) `UPDATE ... CurrentRun+1, AllRun+1` ถ้าไม่มีแถว (0 rows) จึง insert (1/1) |
| `GetActive(playerId)` | แถวที่ `CurrentRun > 0` |
| `GetAllCount(playerId)` | ผลรวม `AllRun` |
| `ResetRun(playerId)` | `CurrentRun = 0` |

### `AchievementRepository`
| เมธอด | ทำอะไร |
|---|---|
| `GetAll(playerId)` | progress ทุก achievement |
| `SaveProgress(playerId, id, progress, target)` | อัปเดต progress (เก็บค่าสูงสุด) และตั้ง `UnlockedAt` เมื่อ `progress >= target` คืน `true` **เฉพาะครั้งที่เพิ่งปลดล็อก** |

---

## 4. Flow การทำงาน

### 4.1 Login / Register — `LoginUIManager`
1. **Register**: ตรวจชื่อว่าง/ซ้ำ → `AddPlayer` → ตั้ง `GameSession.PlayerId` → `EnterGame`
2. **Login**: `GetByName` → ตั้ง `GameSession.PlayerId` → แสดง StartPage (ผู้เล่นกด Play ต่อ → `OnPlayGame`)
3. **EnterGame**: อ่าน `SaveData.SceneName` ถ้าไม่มี/โหลดไม่ได้ ใช้ `GameSession.GameScene` (`Forest`)

### 4.2 เซฟกลางเกม — `GameSaver.SaveAtWave(wave)`
เรียกจาก `WaveManager` (ทุกจบ wave) และปุ่ม Save ใน `SaveUIManager`
1. `PlayerRepository.UpdateProgress(wave, gold)`
2. `SaveRepository.Save(scene, wave, gold, RunKills)`
3. `InventorySaveUI.Save()` → `InventoryRepository.SaveQuantities` 
4. `AchievementTracker.Evaluate(wave)`

(Buff ถูกเขียนลง DB ทันทีที่เลือกผ่าน `BuffManager.Apply` → `AddPick` จึงไม่ต้องเซฟซ้ำ)

### 4.3 โหลดรอบ — `GameSaver.LoadRun()`
เรียกจาก `GameLoader` ตอนเริ่ม scene เกม
1. รีเซ็ต `GameSaver.RunKills = 0` แล้วตั้ง `RunKills` และ gold จาก `SaveData`
2. `InventorySaveUI.Load()` — ใส่ของเข้า `InventoryManager` และ apply modifier ซ้ำ (ข้าม `Heal`)
3. `BuffManager.Load()` — `Restore` buff ที่ `CurrentRun > 0` (ข้าม `Heal`)

`GameSaver.GetSavedWave()` ใช้โดย `WaveManager` เพื่อเริ่มที่ wave ที่เซฟไว้ (ขั้นต่ำ 1)

### 4.4 ตาย — `GameSaver.SaveOnDeath(wave)`
0. เก็บ `RunKills` ไว้ใช้ต่อ (run history / หน้าสรุป) แล้วตั้งเป็น 0
1. รีเซ็ต `Players.Wave=1, Gold=0` และ `SaveData` เป็น wave 1 / gold 0 ที่ scene `Forest`
2. `InventorySaveUI.ResetRun()` และ `PlayerBuffRepository.ResetRun()` (`CurrentRun = 0`, **`AllRun` คงไว้**)
3. `AchievementTracker.Evaluate(wave)`
4. ส่งขึ้น Leaderboard `LeaderBoard.SubmitWave(name, wave)` (ยกเว้น WebGL)

### 4.5 ฆ่าศัตรู — `EnemyControl`
`GameSaver.RunKills++` (kill ของรอบ) และ `AddKills(playerId, 1)` (kill สะสม) แล้ว `AchievementTracker.EvaluateKills()` (ตรวจเฉพาะ achievement ชนิด `Kills` เพื่อลดงาน)

---

## 5. Achievement System

- `AchievementTracker` (static) ถือ **catalog** ของ `AchievementData` (ScriptableObject) — `AchievementUI` เรียก `SetCatalog` ตอนเริ่ม
- Metric ที่รองรับ และแหล่งข้อมูล:

| `AchievementMetric` | ค่าที่ใช้ |
|---|---|
| `Kills` | `Players.Kill` |
| `BestWave` | wave ที่ส่งเข้า `Evaluate(wave)` |
| `ItemsBought` | ผลรวม `Inventory.AllRun` |
| `BuffsPicked` | ผลรวม `PlayerBuffs.AllRun` |

- เมื่อปลดล็อกใหม่: เพิ่มเข้า `JustUnlocked` และยิง event `OnUnlocked(AchievementData)` ให้ UI แสดง popup
- `Progress` เก็บค่าสูงสุดเท่านั้น (ไม่ลดลง) ปลดล็อกแล้วไม่ปลดซ้ำ

---

## 6. Leaderboard (Firebase)

ไฟล์: [LeaderBoard.cs](../Assets/Script/DataBase/LeaderBoard/LeaderBoard.cs) — ครอบด้วย `#if !UNITY_WEBGL`

โครงสร้างข้อมูล:
```
/leaderboard
   └─ {playerName}
        ├─ name: "..."
        └─ wave: 12
```

- **`SubmitWave(name, wave)`** — ใช้ `RunTransaction` กับ `wave`: เขียนทับเฉพาะเมื่อ wave ใหม่ **สูงกว่า**สถิติเดิม (ไม่งั้น `Abort`) แล้วเขียน `name`
- **`LoadScores()`** — `OrderByChild("wave").LimitToLast(3)` (แสดง Top 3) → เรียงมาก→น้อยบน client → แสดงใน `nameText` / `waveText` ข้ามแถวที่ไม่มี `name` หรือ `wave`
- `OnOpen()` / `OnClose()` เปิด-ปิดหน้า leaderboard (และโหลดคะแนนใหม่ตอนเปิด)
- Key คือชื่อผู้เล่น ดังนั้นชื่อเดียวกันใช้ node เดียวกัน

---

## 7. ข้อควรระวัง / ข้อสังเกต

1. **Connection ไม่ถูก cache และไม่ถูก dispose** — `DbProvider.Connection` getter สร้าง connection ใหม่ทุกครั้ง และ `GameSaver`/`AchievementTracker` เรียกหลายครั้งต่อการเซฟ (โดยเฉพาะ `EvaluateKills` ที่เปิดไฟล์ DB ใหม่ทุกครั้งที่ฆ่าศัตรู) ควรพิจารณา cache instance เดียว (`db ??= new ...`) — โค้ดเวอร์ชันก่อนหน้าเคย cache เพื่อกัน "database is locked" แต่ถูกเอาออก ฟิลด์ `db` ที่เป็น static จึงเหลือไว้เฉยๆ
2. **ไม่มีรหัสผ่าน** — login ด้วยชื่ออย่างเดียว ใครรู้ชื่อก็เข้าได้
3. **ไม่มี schema migration** — `CreateTable<T>()` จะเพิ่มคอลัมน์ใหม่ให้อัตโนมัติ แต่ไม่ลบ/เปลี่ยนชนิดคอลัมน์เดิม ถ้าแก้ schema หนัก ๆ ให้ลบ `game.db` ตอนพัฒนา
4. **ไม่มี FK constraint** — ลบ `Players` แล้วแถวลูกจะค้าง
5. **ข้อมูลซ้ำซ้อน** — `Players.Wave/Gold` กับ `SaveData.Wave/Gold` เก็บค่าเดียวกัน (`SaveUIManager` อ่านจาก `SaveData` ก่อน แล้ว fallback เป็น `Players`)
6. **Leaderboard key = ชื่อผู้เล่น** — ชื่อที่มีอักขระต้องห้ามของ Firebase (`. $ # [ ] /`) จะทำให้เขียนไม่สำเร็จ และ client ใครก็เขียนได้ถ้า Security Rules ไม่ได้จำกัด (ตรวจ Rules ใน Firebase Console)
7. **ที่เก็บไฟล์ DB** — ดู path ได้จาก log `db path: ...` ใน Console (บน Windows อยู่ใต้ `%USERPROFILE%\AppData\LocalLow\<Company>\<Product>\game3.db`)
8. **ใช้ไฟล์ `game3.db`** — ข้อมูลเดิมใน `game.db`/`game2.db` (schema เก่า) จะไม่ถูกย้ายมา ผู้เล่นต้องสมัครใหม่
9. **ตารางที่เปลี่ยนเป็น composite PK** — `Inventory`/`PlayerBuffs` ถ้ามีไฟล์ DB เก่าอยู่ `CREATE TABLE IF NOT EXISTS` จะไม่แก้ตารางเดิม จึงต้องใช้ DB ใหม่ (เหตุผลที่เปลี่ยนเป็น `game3.db`)
10. **`ShopData.id` / `BuffData.id` ต้องไม่ซ้ำและห้ามเปลี่ยนหลังเริ่มใช้** — เป็น key ใน DB (มี `OnValidate` เตือนเมื่อซ้ำ) เปลี่ยนชื่อไอเทม/buff ได้โดยข้อมูลผู้เล่นไม่หาย แต่ stat/value อ่านจาก ScriptableObject ตรง ๆ ไม่ได้เก็บใน DB
