using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using HatWorld.src.HatManager;
using OptionalUI;
using UnityEngine;

namespace HatWorld
{
    [BepInPlugin("kadw.hatworld", "HatWorld", "1.0.1")]
    public class HatWorldMain : BaseUnityPlugin
    {
        // Code for AutoUpdate support
        // Should be put in the main PartialityMod class.
        // Comments are optional.

        // Update URL - don't touch!
        // You can go to this in a browser (it's safe), but you might not understand the result.
        // This URL is specific to this mod, and identifies it on AUDB.
        public string updateURL = "http://beestuff.pythonanywhere.com/audb/api/mods/15/0";
        // Version - increase this by 1 when you upload a new version of the mod.
        // The first upload should be with version 0, the next version 1, the next version 2, etc.
        // If you ever lose track of the version you're meant to be using, ask Pastebin.
        public int version = 1;
        // Public key in base64 - don't touch!
        public string keyE = "AQAB";
        public string keyN = "sjXGBzj2fVUlV9GFImX4Njxfpuo0uX+XVD4jKofeiTTu2bGh+xzT9K2ShLiV5mLrKHMwCEmRghcQRDUaVNGoDwsegDohuLmqmdngLO6lrv3LhDKt2FCinqy2CxdC8W4Y5ztdgE7ccjv5bEH7j3wblROiJv0FRab3mM/vsS5HubjtjIKwNL8oIyPNt8uEQl952Ch4V7OtojJamuRBmB1i2BcSuvSq0/wEUWpDC3jvXt/n7KCUt8bJM/D26WPDWwnjPjmjdRiMrgzvYlG1c6TM1WGnu6lXrl97Nkdwi59SfepEuoLgwPGWN/ILkrzuP2IoKLyqmrQTGOjEqxAW6hjtuyprMAL57rK7ighVLpr4svgP2vd2PONecL9cqNEd3GGHvlt9dEGI5loNIcqJ9fwcyTa7PMhPRsqH05LWzPlDWYil/AoQvhu/UikINlR5aPBN6BC4/iPgaT2ZroU+/wh0N2YFy44TCFf3jIaD+RMovmfwOzA3m3MXIqhbrAwMgePSFjI897sRaGX6pZsu7sm1t+jO0+a5Ax4JfGDLGGdX9AEScJpLrVpIXqDs+kiacZ2rH/qCxG8OIUyAXml5Zoz9f6RRKYnccfQRzEa6w0dQlC2CWHWvv0WQRyBGttzcSQ1xZAvMI/zMLv/OHiVQ3mym1gA59d3U905GYMw29Ccq4+0=";
        // ------------------------------------------------

        public static List<Type> hatTypes = new List<Type>() {
            typeof(SantaPhysical), typeof(WizardPhysical), typeof(BubblePhysical), typeof(FlowerPhysical),
            typeof(TorchPhysical), typeof(WingPhysical), typeof(FountainPhysical), typeof(AntennaPhysical)
        };

        public static HatFisob hatFisob = null;

        public static Type? fancyGraphicsRef = null;
        public void OnEnable()
        {
            On.Creature.ctor += Creature_ctor;

            // Make hat exist in main game + create hat icons and sandbox unlocks
            hatFisob = new HatFisob();
            Fisobs.Core.Content.Register(hatFisob);
            hatFisob.AddIconAndSandbox("HatWorld.AntennaPhysical", EnumExt_HatWorld.AntennaUnlockID, new Color(0.3f, 1f, 0.3f));
            hatFisob.AddIconAndSandbox("HatWorld.BubblePhysical", EnumExt_HatWorld.BubbleUnlockID, new Color(0.77f, 0.89f, 0.94f));
            hatFisob.AddIconAndSandbox("HatWorld.FlowerPhysical", EnumExt_HatWorld.FlowerUnlockID, new Color(0.5f, 0.7f, 0.4f));
            hatFisob.AddIconAndSandbox("HatWorld.FountainPhysical", EnumExt_HatWorld.FountainUnlockID, new Color(0.36f, 0.66f, 0.75f));
            hatFisob.AddIconAndSandbox("HatWorld.SantaPhysical", EnumExt_HatWorld.SantaUnlockID, new Color(0.8f, 0.6f, 0.6f));
            hatFisob.AddIconAndSandbox("HatWorld.TorchPhysical", EnumExt_HatWorld.TorchUnlockID, new Color(0.92f, 0.72f, 0f));
            hatFisob.AddIconAndSandbox("HatWorld.WingPhysical", EnumExt_HatWorld.WingUnlockID, WingPhysical.lightRed);
            hatFisob.AddIconAndSandbox("HatWorld.WizardPhysical", EnumExt_HatWorld.WizardUnlockID, new Color(0.6f, 0.6f, 0.8f));

            // Put hats in their respective rooms where they can be found
            HatPlacer.AddHooks();
            HatPlacer.AddSpawns(Assembly.GetExecutingAssembly().GetManifestResourceStream("HatWorld.src.HatPlacer.spawns.txt"));

            // For saving/loading data about which creatures are wearing which hats (used for scavengers)
            HatSaveManager.AddHooks();

            // For adding Moon dialogue for hats
            MoonDialogueManager.AddHooks();

            // For compatability with other mods
            On.RainWorld.Start += RainWorld_Start;
        }

        private void RainWorld_Start(On.RainWorld.orig_Start orig, RainWorld self)
        {
            orig(self);

            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (asm.GetName().Name == "FancySlugcats")
                {
                    fancyGraphicsRef = asm.GetType("FancySlugcats.FancyPlayerGraphics");
                    Debug.Log("Hatworld: FancySlugcats found.");
                    break;
                }
            }

            if (fancyGraphicsRef == null)
                Debug.Log("Hatworld: FancySlugcats not found.");

        }


        private void Creature_ctor(On.Creature.orig_ctor orig, Creature self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);

            CreatureHatManager.CreateHatManager(self, abstractCreature.ID);
            // Debug.Log("Hatworld: new creature constructed " + self.abstractCreature.ID + " " + CreatureHatManager.HatManagers.ContainsKey(self.abstractCreature.ID));
        }

        public static Type GetType(string typeName)
        {
            foreach (Type t in hatTypes)
            {
                if (t.ToString().Equals(typeName))
                {
                    return t;
                }
            }
            Debug.Log("Hatworld ERROR: could not find hat " + typeName);
            return null;
        }

        public static void AddType(Type type)
        {
            hatTypes.Add(type);
        }

        public static void AddType(Type[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                AddType(types[i]);
            }
        }

        /*
         * For ConfigMachine config menu
         */
        public OptionInterface LoadOI()
        {
            return new HatWorldConfig(this);
        }
    }
}
