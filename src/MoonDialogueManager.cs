using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using RWCustom;
using UnityEngine;

namespace HatWorld
{
    static class MoonDialogueManager
    {
        public static void AddHooks()
        {
            On.SLOracleBehaviorHasMark.TypeOfMiscItem += SLOracleBehaviorHasMark_TypeOfMiscItem;
            On.SLOracleBehaviorHasMark.MoonConversation.AddEvents += MoonConversation_AddEvents;
        }

        /*
         * Add all HatPhysical to SLOracleBehaviorHasMark.MiscItemType
         * Instead of adding hats directly to the MiscItemType enum, we hook the TypeOfMiscItem method that converts physical object to enum
         */
        private static SLOracleBehaviorHasMark.MiscItemType SLOracleBehaviorHasMark_TypeOfMiscItem(On.SLOracleBehaviorHasMark.orig_TypeOfMiscItem orig, SLOracleBehaviorHasMark self, PhysicalObject testItem)
        {
            SLOracleBehaviorHasMark.MiscItemType result = orig(self, testItem);

            if (testItem is HatPhysical && result == SLOracleBehaviorHasMark.MiscItemType.NA)
            {
                SLOracleBehaviorHasMark.MiscItemType newEnumValue = (SLOracleBehaviorHasMark.MiscItemType) Enum.Parse(typeof(SLOracleBehaviorHasMark.MiscItemType), (testItem.GetType().Namespace + "_" + testItem.GetType().Name));
                return newEnumValue;
            }
            return result;
        }

        private static void MoonConversation_AddEvents(On.SLOracleBehaviorHasMark.MoonConversation.orig_AddEvents orig,
            SLOracleBehaviorHasMark.MoonConversation self)
        {
            bool foundHat = false;
            if (self.id == Conversation.ID.Moon_Misc_Item)
            {
                foreach (Type t in HatWorldMain.hatTypes)
                {
                    Debug.Log("Hatworld: searching " + self.describeItem + t.ToString() + " " + t.Name + " " + (int)(SLOracleBehaviorHasMark.MiscItemType)Enum.Parse(typeof(SLOracleBehaviorHasMark.MiscItemType), (t.Namespace + "_" + t.Name)));
                    if (self.describeItem == (SLOracleBehaviorHasMark.MiscItemType)Enum.Parse(typeof(SLOracleBehaviorHasMark.MiscItemType), (t.Namespace + "_" + t.Name)))
                    {
                        Debug.Log("Hatworld: moon found " + t.Name + " " + (int)(SLOracleBehaviorHasMark.MiscItemType)Enum.Parse(typeof(SLOracleBehaviorHasMark.MiscItemType), (t.Namespace + "_" + t.Name)));
                        foundHat = true;

                        self.events.Add(new Conversation.TextEvent(self, -1, "", -1)); // Begin every convo with an empty dialogue box that instantly disappears - fixes issue where first dialogue box sometimes has wrong size
                        LoadHatEventsFromFile(t.ToString(), self);
                        break;
                    }
                }
            }
            Debug.Log("Hatworld: moon conv add events " + foundHat + " " + self.describeItem);

            orig(self);
        }

        // based on CustomRegions.CustomPearls.SLOracleBehaviorHasMarkHook.LoadCustomEventsFromFile
        private static void LoadHatEventsFromFile(string fileName, Conversation self)
        {
            // Dialogue files are normally named with an arbitrary number for encryption.
            // But to allow dialogue files to be titled after the hat they're for, we just use 1 as default encryption number for all files
            int number = 1;

            Debug.Log("Hatworld: ~~~LOAD CONVO " + fileName);

            // Expects format ...\Mods\HatWorldText\[namespace of hat].[name of hat]_[language].txt
            string file = fileName + "_" + LocalizationTranslator.LangShort(self.interfaceOwner.rainWorld.inGameTranslator.currentLanguage) + ".txt";
            string convoFolder = Custom.RootFolderDirectory() + "ModConfigs" +
                Path.DirectorySeparatorChar + "HatWorld" + Path.DirectorySeparatorChar + "HatWorldText";
            string convoPath = convoFolder + Path.DirectorySeparatorChar + file;

            Debug.Log("Hatworld: search convoPath " + convoPath + " root dir " + Custom.RootFolderDirectory());

            if (!File.Exists(convoPath))
            {
                Debug.Log("Hatworld: convoPath NOT FOUND " + convoPath);
                return;
            }

            string text2 = File.ReadAllText(convoPath, Encoding.UTF8);

            // if convoFolder is encrypted, decrypt convoFolder
            if (text2[0] != '0')
            {
                Debug.Log("Hatworld: encrypted file detected, decrypting");
                text2 = Custom.xorEncrypt(text2, (int)(54 + number + (int)self.interfaceOwner.rainWorld.inGameTranslator.currentLanguage * 7));
            }
            else // if convoFolder not encrypted, encrypt convoFolder file (use number = 1)
            {
                Debug.Log("Hatworld: unencrypted file detected");
                EncryptHatDialogue(convoFolder);
            }

            string[] array = Regex.Split(text2, Environment.NewLine);
            if (array.Length < 2)
            {
                Debug.Log($"Hatworld: Corrupted conversation");
                foreach (String x in array) {
                    Debug.Log("Hatworld array contents: " + x);
                }
            }
            try
            {
                if (Regex.Split(array[0], "-")[1] == number.ToString())
                {
                    Debug.Log($"Hatworld: Moon conversation... [{array[1].Substring(0, Math.Min(array[1].Length, 15))}]");
                    for (int j = 1; j < array.Length; j++)
                    {
                        string[] array3 = Regex.Split(array[j], " : ");

                        if (array3.Length == 1 && array3[0].Length > 0)
                        {
                            self.events.Add(new Conversation.TextEvent(self, 0, array3[0], 0));
                        }
                    }
                }
                else
                {
                    Debug.Log($"Hatworld: Corrupted dialogue file...[{Regex.Split(array[0], "-")[1]}]");
                }
            }
            catch
            {
                Debug.Log("Hatworld: TEXT ERROR " + array);
                self.events.Add(new Conversation.TextEvent(self, 0, "TEXT ERROR", 100));
            }
        }

        // from EncryptUtility by AndrewFM and Garrakx
        private static void EncryptHatDialogue(string convoFolder)
        {
            if (Directory.Exists(convoFolder))
            {
                foreach (object obj in Enum.GetValues(typeof(InGameTranslator.LanguageID)))
                {
                    InGameTranslator.LanguageID languageID = (InGameTranslator.LanguageID)obj;
                    string text2 = "Text_" + LocalizationTranslator.LangShort(languageID);
                    if (!Directory.Exists(convoFolder + "/" + text2))
                    {
                        Directory.CreateDirectory(convoFolder + "/" + text2);
                    }
                    FileInfo[] files = new DirectoryInfo(convoFolder).GetFiles();
                    for (int i = 0; i < files.Length; i++)
                    {
                        if (files[i].Name.Substring(files[i].Name.Length - 4, 4) == ".txt")
                        {

                            int num = 1; // hardcode 1 into encryption instead of using filename
                            /*
                            int num;
                            if (files[i].Name.Contains("_"))
                            {
                                num = int.Parse(files[i].Name.Substring(0, files[i].Name.IndexOf("_")));
                            }
                            else
                            {
                                num = int.Parse(files[i].Name.Substring(0, files[i].Name.Length - 4));
                            }
                            */

                            string text3 = File.ReadAllText(convoFolder + "/" + files[i].Name, Encoding.Default);
                            if (text3[0] == '0')
                            {
                                string text4 = Custom.xorEncrypt(text3, (int)(54 + num + (int)languageID * 7));
                                text4 = "1" + text4.Remove(0, 1);
                                File.WriteAllText(string.Concat(new string[]
                                {
                            convoFolder,
                            "/",
                            text2,
                            "/",
                            files[i].Name
                                }), text4);
                            }
                        }
                    }
                }
            }
        }
    }
}
