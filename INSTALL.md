How to Install 0DTK
=====

[The release page](https://github.com/prodzpod/ZeroDayToolKit/releases/latest) should have every file you need.

## How to get it for yourself (easy)
When shipping an extension without including the dll, you must instruct the players to [the release page](https://https://github.com/prodzpod/ZeroDayToolKit/releases/latest) and download `install.exe` as well.
- Download `install.exe` and execute it.
  - If you know how to do python, you can execute `install.py` directly. You may have to install some packages.
- Select the folder with Hacknet.exe in it. (if steam, default: `C:\Program Files (x86)\Steam\steamapps\common\Hacknet`)

## How to include it in an extension
Choosing this way will make it so that existing pathfinder users can just download the extension and it does the setups for them, but **will not** include the korean font patch. You can also use `install.exe` and install the extension in both places for best of both worlds.
- Download [Pathfinder](https://github.com/Arkhist/Hacknet-Pathfinder/releases/latest) if you havent.
- Install `ZeroDayToolKit.dll` and put it in your `ExtensionName/Plugins` folder. make one if you don't have it.
- Install `0dtk-global.xml` and put it in your `ExtensionName/Locales` folder. make one if you don't have it.

## How to get it for yourself (hard)
I don't know why you want to do this @ast3riskinc on discord but here you go
- Download [Pathfinder](https://github.com/Arkhist/Hacknet-Pathfinder/releases/latest) if you havent.
- Go to the folder with Hacknet.exe in it. (if steam, default: `C:\Program Files (x86)\Steam\steamapps\common\Hacknet`)
- Install `ZeroDayToolKit.dll` and put it in your `BepInEx/plugins` folder.
- Install `0dtk-global.xml` and put it in your `locales/Custom` folder (NOT `Content/Locales`), make the folder if it doesn't exists.
- Install `KRPatch.zip` and replace the contents of `Content\Locales\ko-kr\Fonts` with the contents of the zip file.