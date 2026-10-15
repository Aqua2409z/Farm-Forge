import sys
from PIL import Image, ImageDraw

src = sys.argv[1]
out = sys.argv[2]
# candidate cells (col,row) that looked like clean swords
cands = [(0, 4), (3, 4), (3, 2), (7, 2), (0, 1), (6, 4)]
cell = 16
scale = 12
pad = 8

w = len(cands) * (cell * scale + pad) + pad
h = cell * scale + 30
canvas = Image.new("RGBA", (w, h), (255, 255, 255, 255))
draw = ImageDraw.Draw(canvas)

img = Image.open(src).convert("RGBA")
x = pad
for (c, r) in cands:
    icon = img.crop((c * cell, r * cell, c * cell + cell, r * cell + cell))
    big = icon.resize((cell * scale, cell * scale), Image.NEAREST)
    canvas.alpha_composite(big, (x, 20))
    draw.text((x + 2, 4), f"{c},{r}", fill=(255, 0, 0, 255))
    x += cell * scale + pad

canvas.convert("RGB").save(out)
print("saved", out)
