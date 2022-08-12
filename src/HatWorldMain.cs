using System;
using BepInEx;
using UnityEngine;

using System.Collections.Generic;
using System.Reflection;
using OptionalUI;
using HatWorld.src.HatManager;

namespace HatWorld
{
    [BepInPlugin("kadw.hatworld", "HatWorld", "1.0.0")]
    public class HatWorldMain : BaseUnityPlugin
    {
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
            hatFisob.AddIconAndSandbox("HatWorld.BubblePhysical", EnumExt_HatWorld.BubbleUnlockID, new Color(0.57f, 0.79f, 0.94f));
            hatFisob.AddIconAndSandbox("HatWorld.FlowerPhysical", EnumExt_HatWorld.FlowerUnlockID, new Color(0.5f, 0.7f, 0.4f));
            hatFisob.AddIconAndSandbox("HatWorld.FountainPhysical", EnumExt_HatWorld.FountainUnlockID, new Color(0.26f, 0.26f, 0.65f));
            hatFisob.AddIconAndSandbox("HatWorld.SantaPhysical", EnumExt_HatWorld.SantaUnlockID, new Color(0.9f, 0.3f, 0.3f));
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
            Debug.Log("hatworld new creature constructed " + self.abstractCreature.ID + " " + CreatureHatManager.HatManagers.ContainsKey(self.abstractCreature.ID));
        }

        public static Type GetType(string typeName)
        {
            foreach(Type t in hatTypes)
            {
                if (t.ToString().Equals(typeName))
                {
                    return t;
                }
            }
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
