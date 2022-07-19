// default imports
using System;
using BepInEx;
using UnityEngine;

// physical objects library
using Fisobs;
using System.Collections.Generic;
using System.Reflection;
using OptionalUI;

namespace HatWorld
{
    [BepInPlugin("kadw.hatworld", "HatWorld", "1.0.0")]
    public class HatWorldMain : BaseUnityPlugin
    {
        public static List<Type> hatTypes = new List<Type>() {
            typeof(SantaPhysical), typeof(WizardPhysical), typeof(BubblePhysical), typeof(FlowerPhysical),
            typeof(TorchPhysical), typeof(WingPhysical), typeof(FountainPhysical), typeof(AntennaPhysical)
        };

        public static List<PlayerHatManager> hatManagers = new List<PlayerHatManager>() { };


        public void OnEnable()
        {
            On.Creature.ctor += Creature_ctor;

            // Make hat exist
            Fisobs.Core.Content.Register(new HatFisob());

            // Put hats in their respective rooms where they can be found
            HatPlacer.AddHooks();
            HatPlacer.AddSpawns(Assembly.GetExecutingAssembly().GetManifestResourceStream("HatWorld.src.HatPlacer.spawns.txt"));
        }

        private void Creature_ctor(On.Creature.orig_ctor orig, Creature self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);

            if (self is Player)
            {
                PlayerHatManager n = new PlayerHatManager(self as Player);
                n.AddHooks();
                hatManagers.Add(n);
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
        }

        public OptionInterface LoadOI()
        {
            return new HatWorldConfig(this);
        }
    }
}
