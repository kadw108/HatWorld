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

        public static Type? fancyGraphicsRef = null;
        public void OnEnable()
        {
            On.Creature.ctor += Creature_ctor;

            // Make hat exist
            Fisobs.Core.Content.Register(new HatFisob());

            // Put hats in their respective rooms where they can be found
            HatPlacer.AddHooks();
            HatPlacer.AddSpawns(Assembly.GetExecutingAssembly().GetManifestResourceStream("HatWorld.src.HatPlacer.spawns.txt"));

            // For saving/loading data about which creatures are wearing which hats (used for scavengers)
            HatSaveManager.AddHooks();

            // For adding Moon dialogue for hats
            MoonDialogueManager.AddHooks();

            // For compatability with other mods
            On.RainWorld.Start += RainWorld_Start;
            Debug.Log("Hatworld mod running (version 1.0.0)");
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

            if (self is Player)
            {
                PlayerHatManager n = new PlayerHatManager(self as Player);
                n.AddHooks();
            }
            else if (self is Scavenger)
            {
                ScavHatManager n = new ScavHatManager(self as Scavenger);
                n.AddHooks();
            }
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

        public static void addType(Type type)
        {
            hatTypes.Add(type); 
            SLOracleBehaviorHasMark.MiscItemType test = (SLOracleBehaviorHasMark.MiscItemType) 10;
        }

        public static void addType(Type[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                addType(types[i]);
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
