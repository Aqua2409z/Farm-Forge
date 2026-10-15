# Stories — Thứ tự build game nông trại

> Mỗi story là 1 miếng nhỏ **làm xong + chạy thử được + nhìn thấy kết quả** rồi mới sang story sau.
> Trạng thái: ⬜ chưa làm · 🟡 đang làm · ✅ xong

| # | Story | Kết quả nhìn thấy | Trạng thái |
|---|-------|-------------------|:----------:|
| 1 | **Nhân vật di chuyển top-down** | Nhân vật (ô vuông) chạy quanh màn hình bằng WASD/mũi tên | ✅ |
| 2 | **Lưới ô đất + ô đang nhắm** | Ô ngay trước mặt nhân vật sáng/viền lên | ✅ |
| 3 | **Cuốc đất** | Nhấn phím → ô cỏ biến thành ô đất đã cuốc | ✅ |
| 4 | **Gieo hạt** | Nhấn phím trên ô đã cuốc → hiện mầm cây | ✅ |
| 5 | **Cây lớn theo thời gian** | Mầm → cây non → cây chín (đổi hình theo timer) | ✅ |
| 6 | **Thu hoạch** | Cây chín → nhấn phím → cây biến mất, +1 nông sản | ✅ |
| 7 | **Tiệm + tiền + mua/bán** | Có tiệm; gần tiệm bán nông sản lấy tiền, mua hạt; tiền góc phải | ✅ |
| 8 | **Thay art thật (Sprout Lands)** | Nhân vật thật + đi bộ 4 hướng; cỏ/đất/cây/tiệm bằng pixel-art đẹp | ✅ |

---

## Story 1 — Nhân vật di chuyển top-down (ĐANG LÀM)

**Mục tiêu:** Có 1 nhân vật trên màn hình, di chuyển 8 hướng bằng WASD hoặc phím mũi tên.

**Việc cần làm:**
- [x] Script `PlayerMovement.cs` đọc input và di chuyển.
- [x] Scene `Farm.unity` có Camera (orthographic, nền xanh cỏ) + Player (sprite ô vuông).
- [x] Gắn script vào Player, đặt tốc độ di chuyển.
- [ ] **Người chơi bấm Play → nhân vật chạy được.** (bạn test trong Unity)

**Cách test:** Mở project trong Unity → mở `Assets/Scenes/Farm.unity` → bấm nút **Play (▶)** → bấm W/A/S/D hoặc mũi tên. Ô vuông cam phải di chuyển.

**Định nghĩa "xong":** Nhân vật di chuyển mượt 8 hướng, không bị nhanh hơn khi đi chéo (đã normalize hướng). ✅ ĐÃ XONG

---

## Story 2 — Lưới ô đất + ô đang nhắm (ĐANG LÀM)

**Mục tiêu:** Có lưới ô cỏ; ô ngay trước mặt nhân vật (theo hướng nhìn) sáng lên bằng khung viền vàng.

**Việc cần làm:**
- [x] `PlayerMovement` theo dõi hướng nhìn (`FacingDirection`).
- [x] `FarmGridSystem` sinh lưới ô cỏ + đổi toạ độ world ↔ cell.
- [x] `TileSelector` hiện khung viền vàng ở ô trước mặt, bám theo hướng nhìn.
- [x] `FarmWorldBootstrap` tự dựng thế giới lúc Play (dùng `RuntimeInitializeOnLoadMethod`).
- [ ] **Test:** lưới cỏ hiện ra, khung vàng chạy theo hướng nhân vật.

**Cách test:** Bấm ▶ Play → thấy lưới ô cỏ (có đường viền) + 1 khung vàng ở ngay trước mặt. Đi 4 hướng → khung vàng nhảy sang ô tương ứng.

**Định nghĩa "xong":** Khung vàng luôn nằm đúng ô phía trước nhân vật và không chạy ra ngoài lưới. ✅ ĐÃ XONG

---

## Story 3 — Cuốc đất (ĐANG LÀM)

**Mục tiêu:** Đứng nhắm vào ô cỏ, bấm **Space** → ô đó thành ô đất nâu (đã cuốc).

**Việc cần làm:**
- [x] `TileState` enum (Grass / Tilled).
- [x] `FarmGridSystem` theo dõi trạng thái từng ô + hàm `TillTile` (đổi hình sang đất nâu).
- [x] `FarmInteraction` đọc phím Space → cuốc ô đang nhắm.
- [x] `FarmWorldBootstrap` tạo thêm `FarmInteraction`.
- [ ] **Test:** nhắm ô cỏ, bấm Space → thành ô đất.

**Cách test:** Play → đi tới chỗ trống, quay mặt vào ô cỏ (khung vàng), bấm **Space**. Ô cỏ xanh → ô đất nâu. Bấm Space vào ô đã cuốc rồi thì không đổi nữa (đúng).

**Định nghĩa "xong":** Chỉ cuốc được ô cỏ trong lưới; ô đã cuốc giữ nguyên; không lỗi. ✅ ĐÃ XONG

---

## Story 4 — Gieo hạt (ĐANG LÀM)

**Mục tiêu:** Nhắm ô đất đã cuốc, bấm **Space** → hiện mầm cây nhỏ.

**Việc cần làm:**
- [x] `Crop` (cây có nhiều giai đoạn) + `CropSystem` (quản lý cây theo ô).
- [x] `PlaceholderSprites.CreateCenteredSquare` (vẽ cây to dần theo giai đoạn).
- [x] Phím Space "thông minh": cỏ → cuốc; đất trống → gieo hạt.
- [x] `FarmWorldBootstrap` tạo thêm `CropSystem`.
- [ ] **Test:** nhắm ô đất → Space → mầm xanh hiện ra.

**Cách test:** Play → cuốc 1 ô (Space lần 1) → vẫn nhắm ô đó, bấm **Space lần 2** → mầm cây xanh nhỏ hiện ở giữa ô. Ô chưa cuốc hoặc đã có cây thì không gieo được.

**Định nghĩa "xong":** Chỉ gieo được trên ô đã cuốc & chưa có cây; mầm hiện đúng ô. ✅ ĐÃ XONG

---

## Story 5 — Cây lớn theo thời gian (ĐANG LÀM)

**Mục tiêu:** Sau khi gieo, cây tự lớn theo thời gian: mầm nhỏ (xanh) → cây non (xanh to) → cây chín (vàng, to nhất).

**Việc cần làm:**
- [x] `Crop` tự đếm giờ trong `Update`, lớn thêm 1 giai đoạn mỗi `secondsPerStage` giây.
- [x] `CropSystem` truyền tốc độ lớn (mặc định 3 giây/giai đoạn) cho cây.
- [x] Cây chín thì dừng (không lớn quá giai đoạn cuối).
- [ ] **Test:** gieo xong đứng chờ → cây đổi hình dần tới vàng.

**Cách test:** Play → cuốc + gieo 1 ô → **đứng chờ ~3 giây** cây to lên lần 1, ~3 giây nữa thành **vàng chín**. (Muốn nhanh/chậm thì chỉnh `secondsPerStage` trong `CropSystem`.)

**Định nghĩa "xong":** Cây lớn qua đủ các giai đoạn rồi dừng ở cây chín (vàng), không lỗi. ✅ ĐÃ XONG

---

## Story 6 — Thu hoạch (ĐANG LÀM)

**Mục tiêu:** Nhắm cây đã chín (vàng), bấm **Space** → cây biến mất, số nông sản +1 (hiện trên màn hình).

**Việc cần làm:**
- [x] `Inventory` giữ số nông sản + `AddCrops`.
- [x] `CropSystem.Harvest` xoá cây chín (ô đất giữ nguyên để trồng lại).
- [x] Phím Space "thông minh": cây chín → thu hoạch trước tiên.
- [x] `GameHUD` (OnGUI) hiện "Nông sản: X".
- [x] `FarmWorldBootstrap` tạo thêm `Inventory` + `GameHUD`.
- [ ] **Test:** cây chín → Space → biến mất, số nông sản tăng.

**Cách test:** Play → cuốc + gieo + chờ cây **vàng chín** → nhắm vào nó, bấm **Space** → cây biến mất, góc trên trái "Nông sản" +1. Ô đất vẫn còn → gieo lại được (Space, Space).

**Định nghĩa "xong":** Chỉ thu hoạch được cây chín; mỗi lần +1; ô đất trồng lại được; không lỗi. ✅ ĐÃ XONG

---

## Story 7 — Tiệm + tiền + mua/bán (ĐANG LÀM — story cuối MVP)

**Mục tiêu:** Có cái tiệm trong nông trại. Đứng gần tiệm: bán nông sản lấy tiền, mua hạt giống. Gieo hạt tốn 1 hạt. Tiền hiện góc phải.

**Việc cần làm:**
- [x] `Inventory` thêm: tiền (Coins), hạt giống (Seeds, bắt đầu 5), nông sản (Crops).
- [x] `Shop` (ô xanh dương góc trên-phải): gần tiệm bấm **E** bán hết nông sản (+10/cái), **R** mua 1 hạt (-5).
- [x] Gieo hạt (`FarmInteraction`) nay tốn 1 hạt giống.
- [x] `GameHUD`: nông sản + hạt (trái), **tiền (phải)**, gợi ý phím khi gần tiệm.
- [x] `FarmWorldBootstrap` tạo thêm `Shop`.
- [ ] **Test:** cả vòng kinh tế chạy trơn.

**Cách test (vòng chơi FULL):**
1. Gieo hạt (có sẵn 5 hạt) → chờ chín → thu hoạch vài nông sản.
2. Đi tới **ô xanh dương (tiệm)** góc trên-phải → thấy gợi ý phím dưới màn hình.
3. Bấm **E** → bán hết nông sản, **Tiền** (góc phải) tăng.
4. Bấm **R** → mua hạt (tốn tiền, Hạt giống tăng).
5. Về trồng tiếp bằng hạt vừa mua → lặp lại.

**Giá:** bán 1 nông sản = **10 tiền**; mua 1 hạt = **5 tiền** → lời 5/vụ.

**Định nghĩa "xong":** Mua/bán chỉ hoạt động khi gần tiệm; không gieo được khi hết hạt; tiền/hạt/nông sản cập nhật đúng; tiền ở góc phải.

---

## 🎉 Xong Story 7 = HOÀN THÀNH MVP game nông trại!
Vòng chơi đầy đủ: **gieo → lớn → thu hoạch → bán → mua hạt → lặp lại.**

---

## Story 8 — Thay art thật (Sprout Lands) ✅

**Mục tiêu:** Bỏ các ô vuông màu, dùng pixel-art thật của bộ **Sprout Lands (Cup Nooble)**.

**Đã làm:**
- [x] Copy ảnh cần dùng vào `Assets/StreamingAssets/SproutLands/` (character, grass, tilled, plants, chest).
- [x] `SproutLandsArt.cs`: nạp PNG + cắt sprite **lúc chạy** (không cần đóng Unity, không cần Sprite Editor).
- [x] `PlayerVisual.cs`: nhân vật thật + **animation đi bộ 4 hướng** bằng code (không cần Animator).
- [x] Cỏ / đất đã cuốc / 4 giai đoạn cây / cái tiệm (rương vàng) = art thật.
- [x] Credit "Cup Nooble" ở góc dưới-phải HUD (license yêu cầu) + file `CREDITS.txt`.
- [x] Mọi chỗ đều **fallback** về ô vuông cũ nếu thiếu file ảnh → game không bao giờ vỡ.

**Cách test:** Bấm vào cửa sổ Unity (đợi biên dịch ~vài giây) → ▶ Play. Thấy nhân vật người thật đi 4 hướng có hoạt hình, cỏ/đất/cây/tiệm đẹp.

> License Sprout Lands (bản free): **phi thương mại**, không bán lại / phát tán, không NFT/AI, **bắt buộc ghi credit Cup Nooble**.

---

## Nâng cấp sau MVP — làm TỪNG BƯỚC 1

| # | Nâng cấp | Trạng thái |
|---|----------|:----------:|
| 9  | **Camera bám theo nhân vật** (mượt) | ✅ |
| 10 | **Âm thanh** (cuốc / gieo / thu hoạch / mua / bán) | ✅ |
| 11 | **Làm đẹp map** (rải hoa/cỏ/nấm) | ✅ |
| 12 | **HUD icon** (nông sản / hạt / tiền bằng hình) | ✅ |
| 13 | **Thêm loại cây** (chọn hạt, cây thứ 2) | ✅ |
| 14 | **Lưu game** (thoát mở lại vẫn còn) | ✅ |
| 15 | **Chu kỳ ngày → đêm** (màn tối dần ban đêm) | ✅ |
| 16 | **Máu người chơi** (thanh tim, nhá đỏ, hồi sinh) | ✅ |
| 17 | **Quái tấn công ban đêm** (orc/skeleton đuổi theo) | ✅ |
| 18 | **Người chơi chém quái** (chuột trái, vung kiếm) | ✅ |

### Story 9 — Camera bám theo nhân vật ✅
- `CameraFollow.cs`: `LateUpdate` dùng `Vector3.SmoothDamp` bám mượt, giữ z camera.
- `FarmWorldBootstrap` gắn vào `Camera.main`.

### Story 10 — Âm thanh ✅
- `GameAudio.cs`: tạo tiếng "bíp" BẰNG CODE (không cần file nhạc) cho cuốc/gieo/thu hoạch/mua/bán; tự thêm `AudioListener` nếu thiếu.
- Gọi từ `FarmInteraction` (cuốc/gieo/thu hoạch) và `Shop` (mua/bán).

### Story 11 — Làm đẹp map ✅
- `MapDecorations.cs`: rải ngẫu nhiên (seed cố định) ~26 hình nhỏ (cỏ/hoa vàng/hoa hồng/chùm đỏ/nấm) từ `decor.png` lên ô cỏ; cuốc ô nào thì xoá trang trí ô đó.

### Story 12 — HUD icon ✅
- `GameHUD` vẽ **icon** nông sản (củ vàng) + hạt giống (túi hạt) góc trái, **đồng tiền vàng** (vẽ bằng code) góc phải — thay cho chữ. Thiếu art thì tự quay về chữ.

**Test (9–12):** Bấm Unity cho biên dịch → ▶ Play. Camera bám theo; map có hoa/cỏ rải rác; góc trái icon nông sản/hạt, góc phải icon tiền; cuốc/gieo/thu hoạch/mua/bán **có tiếng**.

### Story 13 — Thêm loại cây ✅
- `CropType.cs` + `CropDatabase.cs`: mỗi loại cây = 1 hàng trong `plants.png`, có tên + giá bán + giá mua hạt riêng. Hiện có **Củ vàng** (bán 10, hạt 5) và **Củ cải tím** (bán 18, hạt 8). Thêm loại mới chỉ cần thêm 1 dòng `Make(...)`.
- `SeedSelection.cs`: đổi loại hạt đang chọn bằng phím **1 / 2** (chọn thẳng) hoặc **Q** (xoay vòng).
- `Inventory` đếm hạt & nông sản **theo từng loại** (mảng); `Shop` bán mỗi loại theo đúng giá, mua hạt của loại đang chọn.
- `GameHUD` liệt kê nông sản & hạt từng loại, loại đang chọn có dấu **►** tô vàng.

### Story 14 — Lưu game ✅
- `SaveData.cs` + `SaveSystem.cs`: lưu ra file JSON trong `Application.persistentDataPath` (thư mục dữ liệu riêng của game): tiền, hạt & nông sản từng loại, loại hạt đang chọn, các ô đã cuốc, và từng cây (loại + giai đoạn + đồng hồ lớn dở).
- **Tự lưu** khi: thoát game / bấm Stop trong Editor, mỗi 20 giây, hoặc bấm **F5**. **Tự nạp** 1 lần ngay khi mở game (trước khi rải trang trí nên đất đã cuốc không mọc hoa).

**Test (13–14):** ▶ Play → bấm **2** đổi sang Củ cải tím (phải mua hạt ở tiệm trước: lại gần rương, bấm R). Trồng vài cây mỗi loại, cuốc thêm đất, để dành ít tiền → **Stop** (▶ tắt) rồi **Play lại**: đất, cây (đúng độ lớn), tiền, hạt vẫn còn nguyên.

### Story 15 — Chu kỳ ngày → đêm ✅
- `DayNightCycle.cs`: thời gian lặp **ban ngày 45s → ban đêm 30s**. Một lớp phủ tối (`NightOverlay`) bám theo camera, mờ dần rồi tối nhất giữa đêm (`maxDarkness 0.62`) rồi sáng lại trước khi sang ngày mới. Đếm **số ngày** đã qua.
- HUD hiện nhãn **"Ngày N — Ban ngày / BAN ĐÊM"** (vàng khi ngày, xanh khi đêm).

### Story 16 — Máu người chơi ✅
- `Health.cs`: máu chung cho người chơi lẫn quái; phát sự kiện `Damaged` / `Died`. `HitFlash.cs`: nhá **đỏ** 0.12s khi trúng đòn (không đụng sprite nên hoạt hình vẫn chạy).
- `PlayerStatus.cs`: người chơi **6 tim**. Bị quái chạm → mất 1 tim + **bất tử ngắn 0.8s** (tránh mất máu dồn). Hết máu → **hồi sinh** tại chỗ xuất phát, đầy máu, bất tử 2s.
- HUD vẽ **dãy tim** (đỏ = còn, xám = đã mất) ở giữa-trên màn hình.

### Story 17 — Quái tấn công ban đêm ✅
- `PixelCrawlerArt.cs`: nạp art quái (orc / skeleton) lúc chạy từ `StreamingAssets/PixelCrawler/`.
- `Enemy.cs`: quái **đuổi theo** người chơi (quay mặt đúng hướng), **chạm vào trừ 1 tim** (có nghỉ giữa đòn). Có máu riêng; chết thì **rơi vàng** + biến mất. Hoạt hình "đứng" 4 khung chạy bằng code.
- `EnemySpawner.cs`: **ban đêm** sinh quái ở **rìa bản đồ** (tối đa 6 con), quái **mạnh dần theo số ngày**. **Trời sáng** thì quái còn lại tự biến mất (ban ngày an toàn làm nông).

### Story 18 — Người chơi chém quái ✅
- `PlayerCombat.cs`: bấm **chuột trái** (phím **J** vẫn chém được cho ai thích bàn phím) → **vung cây kiếm thật** theo vòng cung trước mặt, gây sát thương mọi quái trong tầm & ~phía trước (theo hướng đang nhìn).
- `WeaponArt.cs` + `SwordSwing.cs`: nạp art **kiếm sắt** từ `StreamingAssets/Weapons/sword_iron.png` (cắt từ bộ "16x16 Weapons RPG Icons"), vung quanh chuôi (pivot góc dưới-trái) một nhát ~120° rồi mờ dần. Thiếu art kiếm thì tự quay về vệt chém lưỡi liềm (`SlashEffect.cs`).
- ⚠ Bộ icon vũ khí KHÔNG kèm license (xem `StreamingAssets/Weapons/CREDITS.txt`) → dùng tạm cho game học tập/cá nhân.

**Test (15–18):** ▶ Play → chơi nông trại bình thường một lúc, màn hình **tối dần** báo ban đêm; **orc/skeleton** xuất hiện từ mép map và đuổi theo. Quay mặt về phía quái, **bấm chuột trái** để **vung kiếm chém** (quái nhá đỏ, mấy nhát thì chết + được vàng). Bị quái chạm thì **mất tim** (nhá đỏ); hết tim thì hồi sinh. Trời sáng, quái biến mất.

---

## Ghi chú kỹ thuật
- **Input:** dùng Input Manager cũ (`Input.GetAxisRaw`) cho đơn giản ở game đầu tay. Có thể nâng cấp lên New Input System sau.
- **Di chuyển:** hiện dùng `transform.Translate`. Khi cần va chạm (tường, vật cản) sẽ chuyển sang `Rigidbody2D`.
- **Kiến trúc:** code tách gọn, dùng `[SerializeField]`, dễ đọc dễ sửa.
