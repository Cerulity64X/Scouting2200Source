import os

class DPI:
    def __init__(self, iconw: int, iconh: int, launcherw: int, launcherh: int) -> None:
        self.iconw = iconw;
        self.iconh = iconh;
        self.launcherw = launcherw;
        self.launcherh = launcherh;
    def __init__(self, icons: int, launchers: int) -> None:
        self.iconw = icons;
        self.iconh = icons;
        self.launcherw = launchers;
        self.launcherh = launchers;

sizes = {
    "mipmap-mdpi": DPI(48, 108),
    "mipmap-hdpi": DPI(72, 162),
    "mipmap-xhdpi": DPI(96, 216),
    "mipmap-xxhdpi": DPI(144, 324),
    "mipmap-xxxhdpi": DPI(192, 432),
}

icon = "icon.png"
launcher = "launcher.png"

for name in sizes.keys():
    dpi = sizes[name]
    os.system(f'ffmpeg -y -i {icon} -vf scale={dpi.iconw}:{dpi.iconh} {name}\icon.png')
    os.system(f'ffmpeg -y -i {launcher} -vf scale={dpi.launcherw}:{dpi.launcherh} {name}\launcher_foreground.png')
