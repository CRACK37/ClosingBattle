# Closing battle - Ultrakill Mod

We best mod, mod ever best we

# Building
How to build

1. Download this repository
2. Create a text file in the same folder as "ClosingBattle.sln"
3. Rename that text file to "ClosingBattle.csproj.user"
4. Paste this code inside the file
```
<Project>
  <PropertyGroup>
    <UltrakillDir><ULTRAKILL FOLDER PATH></UltrakillDir>
  </PropertyGroup>
</Project>
```
5. Replace the <ULTRAKILL FOLDER PATH> with the location of your ULTRAKILL folder
6. Open the mod project, and click build
7. If everything went right, you should be able to open ULTRAKILL and play the mod (since it copies the dll/assets directly into your game build)

## To build the bundles
This step is not necessary unless you want to modify the Assets used by this mod
This repository uses [symbolic links](https://learn.microsoft.com/en-us/windows/win32/fileio/symbolic-links) to keep the assets inside unity in sync with the github repository.

To build the bundles, a RUDE project is needed
You can find one how to create one [here](https://envy-spite-team.github.io/ULTRAMappingDocs/Setup/Editor%20Setup)

1. Open the RUDE Project
2. Open a cmd tab in administrator mode
3. Replace the <RUDE PROJECT PATH> and <MOD PROJECT PATH> with your folders locations, and execute the command below
```
mklink /D "<RUDE PROJECT PATH>\Assets\ClosingBattle" "<MOD PROJECT PATH>\Unity\ClosingBattle"
```
4. Go to "ClosingBattle\Unity" and copy the "Editor" folder, paste it inside the RUDE Project Assets folder.
5. Go to the RUDE Project and right next to Tools (On the upper left-center part of the screen) there should be a Button that says "Setup".
6. Click on "Setup" and then "Create ClosingBattle Addressable Group"
7. To build the bundle, go to "Window" and then "Raw Addressable Bundle Exporter"
8. The Groups and Remote load path will already be loaded (if not, Use "ClosingBattle" for "Groups to build" and "{ClosingBattle.Plugin.AddressableAssetPath}" for "Remote load path")
9. Click on "Open Export Folder" and go to the ClosingBattle repository, and then go to "Source/Assets", and select that as the path to use
10. Click on "Build raw Bundles" to finally build the bundles