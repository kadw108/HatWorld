using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HatWorld
{
    public class HatFisob : Fisobs.Items.Fisob
    {
        private static readonly HatProperties properties = new();

        /* Since we only have one int field to store custom data, we map the int field to the string of an existing hat type and a color for the sandbox icon
        (new types/colors are added with AddIconAndSandbox) 
        There may be a better way to do this than two dictionaries?
        */
        internal static Dictionary<int, string> hatIntMapping = new();
        internal static Dictionary<int, Color> colorIntMapping = new();

        public HatFisob() : base(EnumExt_HatWorld.HatAbstract)
        {
            Icon = new AllHatIcon();
        }

        public override Fisobs.Properties.ItemProperties Properties(PhysicalObject forObject)
        {
            return properties;
        }

        public override AbstractPhysicalObject Parse(World world, Fisobs.Core.EntitySaveData saveData, Fisobs.Sandbox.SandboxUnlock? unlock)
        {
            if (unlock is Fisobs.Sandbox.SandboxUnlock u)
            {
                Debug.Log("HatWorld: unlock sandbox " + u.Data + " " + hatIntMapping[u.Data]);
                return new HatAbstract(world, saveData.Pos, saveData.ID, hatIntMapping[u.Data]);
            }

            Debug.Log("HatWorld: hat fisob parse " + (unlock == null ? "null" : (unlock.ToString() + unlock.Value)) + " custom " + saveData.CustomData);
            HatAbstract results = new HatAbstract(world, saveData.Pos, saveData.ID, saveData.CustomData);

            return results;
        }

        public void AddIconAndSandbox(string hatType, MultiplayerUnlocks.SandboxUnlockID id, Color iconColor,
            MultiplayerUnlocks.SandboxUnlockID? parent = MultiplayerUnlocks.SandboxUnlockID.Slugcat)
        {
            int nextInt = NextAvailableInt();
            hatIntMapping[nextInt] = hatType;
            colorIntMapping[nextInt] = iconColor;
            if (parent != null) // hat will not be available in sandbox if parent is null
            {
                RegisterUnlock(id, parent, data: nextInt);
            }

            /*
            using (FileStream aFile = new FileStream(Custom.RootFolderDirectory() + "hatworld_log.txt", FileMode.Append, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(aFile))
            {
                sw.WriteLine("HatWorld: registered new icon/sandbox - " + hatType + " int: " + nextInt + " color: " + iconColor + " parent: " + parent);
            }
            */
        }

        private static int NextAvailableInt(int startIndex = 0, int maxIndex = Int32.MaxValue)
        {
            for (int i = startIndex; i < maxIndex; i++)
            {
                if (!hatIntMapping.ContainsKey(i))
                {
                    return i;
                }
            }
            Debug.Log("HatWorld ERROR: HatFisob dictionaries out of keys.");
            return -1;
        }
    }

    sealed class AllHatIcon : Fisobs.Core.Icon
    {
        // Vanilla only gives you one int field to store all your custom data.
        // In this case, that int field is used to store an int corresponding to the physical hat type
        public override int Data(AbstractPhysicalObject apo)
        {
            string hatType = (apo as HatAbstract).hatType;

            // Reverse lookup dictionary to get corresponding int for string representing type of hat
            int hatDataInt = HatFisob.hatIntMapping.FirstOrDefault(x => x.Value == hatType).Key;
            return hatDataInt;
        }

        public override Color SpriteColor(int data)
        {
            return HatFisob.colorIntMapping[data];
        }

        public override string SpriteName(int data)
        {
            // Fisobs autoloads the embedded resource named `icon_{Type}` automatically
            // For HatFisob, this is icon_HatAbstract, which only allows using that one icon for every hat
            // Thus to use different icons for different hats we load sprites with CustomSpritesLoader mod instead
            Debug.Log("HatWorld: fetch sprite name " + data + " " + HatFisob.hatIntMapping[data]);
            return "icon_" + HatFisob.hatIntMapping[data]; // expects icon_[namespace].[physical type name].png eg. icon_HatWorld.WizardPhysical.png
        }
    }
}
