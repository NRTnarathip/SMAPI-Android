adb shell am force-stop com.companyname.stardewvalley

adb push "C:\Users\narat\Desktop\Stardew Valley Android Xamarin\SMAPI Launcher\SMAPI-3.18.6\SMAPI\bin\Debug\StardewModdingAPI.dll" "/storage/emulated/0/Android/data/com.companyname.stardewvalley/files"

adb shell am start -n com.companyname.stardewvalley/crc64e8b22d3833b21ea5.ModActivity
