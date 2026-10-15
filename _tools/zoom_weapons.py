import sys
from PIL import Image, ImageDraw

src = sys.argv[1]
out = sys.argv[2]
cols = int(sys.argv[3]) if len(sys.argv) > 3 else 10
rows = int(sys.argv[4]) if len(sys.argv) > 4 else 8
cell = 16
scale = 8

img = Image.open(src).convert("RGBA")
region = img.crop((0, 0, cols * cell, rows * cell))
big = region.resize((cols * cell * scale, rows * cell * scale), Image.NEAREST)

# white background so transparent icons are visible
bg = Image.new("RGBA", big.size, (255, 255, 255, 255))
bg.alpha_composite(big)
draw = ImageDraw.Draw(bg)

step = cell * scale
for c in range(cols + 1):
    draw.line([(c * step, 0), (c * step, rows * step)], fill=(255, 0, 0, 180), width=1)
for r in range(rows + 1):
    draw.line([(0, r * step), (cols * step, r * step)], fill=(255, 0, 0, 180), width=1)

# label each cell "col,row"
for r in range(rows):
    for c in range(cols):
        draw.text((c * step + 2, r * step + 2), f"{c},{r}", fill=(0, 0, 255, 255))

bg.convert("RGB").save(out)
print("saved", out, "grid", cols, "x", rows)
