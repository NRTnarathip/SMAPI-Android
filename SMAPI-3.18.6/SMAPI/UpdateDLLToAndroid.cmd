echo Current directory: %cd%

adb shell am force-stop com.chucklefish.stardewvalley

adb push "StardewModdingAPI.dll" "/storage/emulated/0/Android/data/com.chucklefish.stardewvalley/files/Saves/SMAPI-Game"

adb shell am start -n com.chucklefish.stardewvalley/crc64e8b22d3833b21ea5.MainActivity