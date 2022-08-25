# Creating Custom Hats

If you know how to mod, you can use HatWorld to add your own hats into the game. 

I didn't code the mod with custom hats as the primary purpose, so it may be a messy process. Perhaps this doc is mainly a guide for myself?

This folder contains files for an example custom hat called RedBubble. It's identical to the Bubble hat from HatWorld, but red. It can be used as a reference for creating your own custom hat.

Enabling the custom hat:
* HatWorld and its dependencies must be enabled.
* Copy `icon_HatWorldCustomExample.RedBubblePhysical.png` in the `icon` folder to Rain World\ModConfigs\CustomSpritesLoader\Load
* Copy the text files in `moon_dialogue_files\encrypted` OR `moon_dialogue_files\unencrypted` to Rain World\ModConfigs\HatWorld\HatWorldText. If you use the unencrypted files, giving a RedBubblePhysical hat to Moon will create encrypted versions of those text files.
* Copy `HatWorldCustomExample.dll` in the `FINAL_DLL` folder to Rain World\Mods and then enable it. Or copy it directly to Rain World\BepInEx\plugins (it won't show up in BOIModManager, but will be enabled).

Descriptions of the files in this folder:
* `FINAL_DLL\HatWorldCustomExample.dll` is a .dll file for the custom hat example. It requires HatWorld and its dependencies to be enabled. It also requires the custom hat's icon and text files to be in the correct folders (see above section on enabling the custom hat).
* `icon` contains the sandbox/shelter icon for the custom hat. See [Icons](#icons).
* `moon_dialogue_files` contains encrypted and unencrypted versions of Moon's dialogue for the RedBubble hat, in different languages. Only the encrypted files are required for the mod to function properly. The unencrypted files are there to show what unencrypted text files look like. These dialogue files don't have translations for different languages, so she will always speak in English no matter what the language setting is. See [Adding Moon Dialogue](#adding-moon-dialogue).
* `src`, along with `HatWorldCustomExample.csproj` and `HatWorldCustomExample.sln` contain the actual code for the RedBubble hat. See [Code](#code), below.

---

### Code

The .csproj and .sln can be used as a reference. If starting from scratch, you will need to add HatWorld, Fisobs, and EnumExtender as project references, along with the usual mandatory references (see the .csproj file. Note that paths are specific to me so you will have to change them).

Every hat needs a few things:

* A [hatname]Physical.cs class that derives from HatPhysical, and determines behavior of the hat when it's not being worn
* A [hatname]Wearing.cs class that derives from HatWearing, and determines behavior of the hat when it's being worn

* Two enums. All enums for your hats can be declared in a single EnumExt class (see EnumExt_HatWorldCustom.cs)
	* A MultiplayerUnlocks.SandboxUnlockID enum, so the shelter/sandbox icon gets registered (required even if the hat won't be in sandbox)
	* A SLOracleBehaviorHasMark.MiscItemType [namespace]_[name]Physical enum, so the game doesn't crash when Moon encounters the hat

* Several lines of code must be included, e.g. with RedBubble hat:
	* `HatWorldMain.AddType(typeof(RedBubblePhysical));` to make the new hat exist
		- use typeof(the physical type for your hat, ie. [hatname]Physical)
	* `HatWorldMain.hatFisob.AddIconAndSandbox("HatWorldCustomExample_RedBubblePhysical", EnumExt_HatWorldCustom.RedBubbleUnlockID, Color.red);` to create the shelter icon/sandbox icon
		- first argument is [namespace]_[name]Physical
		- second argument is MultiplayerUnlocks.SandboxUnlockID for your hat
		- third argument is color of icon

* An icon. See Icons section.

Optional:

* To make the hat appear somewhere in a room, much like a colored pearl:
	* Create a text file somewhere and make it an embedded resource. Then put `HatPlacer.AddSpawns(Assembly.GetExecutingAssembly().GetManifestResourceStream("[text file name]"));` into the code. Names for embedded resources should be [namespace].[enclosing folder(s)].[text file name], see https://stackoverflow.com/questions/44577716/net-get-embedded-resource-file
* Adding Moon dialogue is optional but still possible. See [Adding Moon Dialogue](#adding-moon-dialogue) section.



#### More Detail on HatWearing and HatPhysical

* The derived classes of HatWearing and HatPhysical both need InitiateSprites and ApplyPalette to be defined.
* HatPhysical classes also requires DrawSprites to be defined (don't forget to put a base.DrawSprites call at the beginning). In HatWearing classes, you should override ChildDrawSprites instead, and don't override DrawSprites -- no base.DrawSprites call is necessary.

* HatPhysical has an Update method which can be overriden for extra graphics calculations, and HatWearing has a ChildUpdate method which can be overriden for the same purpose.

* HatWearing has AddHatEffects and RemoveHatEffects, two methods which are called to trigger special effects of wearing a hat if they are turned on in the settings. You can look at existing hats to see how these methods are used.

---

### Icons

Every custom hat needs an icon image. Recommended size 20x20 pixels or smaller. The name of the icon image must be "icon_[namespace].[name of physical hat].png". Put all the icon images in Rain World\ModConfigs\CustomSpritesLoader\Load (requires CustomSpritesLoader mod, but that's a dependency for HatWorld, so you should have it if you have HatWorld installed)

---

### Adding Moon Dialogue

Code for Moon's hat dialogue was based on https://rain-world-modding.fandom.com/wiki/Adding_a_Custom_DataPearl.

HatWorld expects moon dialogue files in the format: Rain World\Mods\HatWorldText\[namespace of hat].[name of hat]_[language].txt

The languages are: eng, fre, ger, ita, por, spa
(English, French, German, Italian, Portuguese, Spanish)

The file name is NOT case sensitive.

EXAMPLE: You have a RedBubblePhysical hat in a HatWorldCustomExample namespace. Your moon dialogue files should look like: hatworldcustomexample.redbubblephysical_eng, hatworldcustomexample.redbubblephysical_fre, hatworldcustomexample.redbubblephysical_ger, hatworldcustomexample.redbubblephysical_ita, hatworldcustomexample.redbubblephysical_por, hatworldcustomexample.redbubblephysical_spa.

All these text files should be located in your Rain World\Mods\HatWorldText folder, corresponding to wherever your Rain World files are.

--

The mod can read both encrypted and unencrypted text files. (Rain World provides a way to encrypt text files so people can't just look at the game files and see what Moon is supposed to say.)

IMPORTANT: In an unencrypted text file, the first line must be "0-1". Otherwise the file won't be read properly.

For best results, use encrypted text files in your final release, so people can't just look at the text files and instantly know what the dialogue is.

To encrypt an unencrypted text file: If HatWorld mod reads an unencrypted moon dialogue file, it will automatically create encrypted versions. The encrypted versions will appear in the HatWorldText folder, inside folder Text_Eng, Text_Fre, Text_Ger, etc. Important: you must use the encrypted text file located inside the correct language folder. Drag it out into the HatWorldText folder.

#### Moon Dialogue Example

Let's say you have unencrypted versions of AntennaPhysical dialogue in two languages: English and Portuguese. The file names are hatworld.antennaphysical_eng.txt and hatworld.antennaphysical_por.txt. 

Run the game, give yourself the mark (DevLog mod is good for this), teleport yourself to Moon's room (Warp mod is good for this) and then give her an AntennaPhysical hat to talk about. This triggers the encryption/decryption code: HatWorld mod will detect that the files are unencrypted. It will create Text_Eng, Text_Fre, Text_Ger, etc. folders, each folder containing encrypted versions of hatworld.antennaphysical_eng.txt and hatworld.antennaphysical_por.txt.

(If you have multiple hats/multiple text files with unencrypted dialogue, just giving her one of those hats is enough for HatWorld to encrypt all the text files.)

You want the encrypted version of hatworld.antennaphysical_eng.txt located in the Text_Eng folder, since it's encrypted for English. And you want the encrypted version of hatworld.antennaphysical_por.txt in the Text_Por folder, since it's encrypted for Portuguese. All the other folders/files can be discarded.

Move the correct encrypted versions of hatworld.antennaphysical_eng.txt and hatworld.antennaphysical_por.txt into the HatWorldText folder, and the mod will be able to read them.
