# Game Brief — Game Nông Trại (bản MVP)

> Tài liệu 1 trang mô tả game. Đây là "kim chỉ nam" — làm tính năng nào cũng ngắm vào đây.

## Tên tạm
**Tiny Farm** (sẽ đặt tên chính thức sau)

## Pitch (1 câu)
Game 2D nhìn từ trên xuống, thư giãn: người chơi **cuốc đất → gieo hạt → chờ cây lớn → thu hoạch → bán lấy tiền → mua hạt** và lặp lại để phát triển nông trại.

## Góc nhìn & phong cách
- **2D top-down** (nhìn từ trên xuống).
- Pixel art (art dự kiến lấy từ ansimuz.itch.io — gói **Legend of Faune** miễn phí cho nhân vật top-down).
- Nhịp chơi chậm rãi, thư giãn (cozy game).

## Vòng chơi cốt lõi (core loop)
```
Di chuyển → Cuốc đất → Gieo hạt → (chờ cây lớn theo thời gian) → Thu hoạch → Bán → Mua hạt → lặp lại
```

## Tính năng MVP (xem thứ tự build trong stories.md)
1. Nhân vật di chuyển top-down (WASD / mũi tên).
2. Lưới ô đất + ô đang nhắm phía trước nhân vật.
3. Cuốc đất (ô cỏ → ô đất).
4. Gieo hạt trên ô đã cuốc.
5. Cây lớn dần theo thời gian (hạt → mầm → cây chín).
6. Thu hoạch cây chín → +1 nông sản.
7. Tiền + HUD + mua/bán đơn giản.

## KHÔNG làm ở bản MVP (để dành sau)
- Chu kỳ ngày/đêm, thời tiết.
- Nhiều loại cây, cây ăn quả lâu năm.
- Lưu / tải game.
- NPC, cửa hàng có hội thoại, nhiệm vụ.
- Âm thanh, nhạc nền.
- Tưới nước, phân bón, động vật.

## Ghi chú về art
- Logic game **không phụ thuộc art**: Story 1–6 dùng **ô vuông màu sinh bằng code** để chạy được ngay.
- Khi logic ổn, **thay sprite ansimuz vào** (chỉ đổi hình, không đổi code).
- Riêng tile đất/cây trồng ansimuz ít có sẵn → có thể cần tìm thêm tileset farming khác hoặc tự vẽ/tạo.

## Tiêu chí "xong MVP"
Người chơi mở game, điều khiển nhân vật trồng được ít nhất 1 vụ: cuốc → trồng → chờ → thu hoạch → bán lấy tiền → mua hạt lại. Chạy mượt, không lỗi.
