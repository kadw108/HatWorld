﻿using System.Collections.Generic;
using UnityEngine;

namespace HatWorld.src.HatManager
{
    public class ScavHatManager : CreatureHatManager
    {

        public ScavHatManager(EntityID wearer) : base(wearer) { }

        public override void AddHooks()
        {
            base.AddHooks();

            On.ScavengerGraphics.ctor += ScavengerGraphics_ctor;
            On.Scavenger.Stun += Scavenger_Stun; // remove hat if stunned

            /* Hook to make scav wear hat upon picking it up. For some reason only the third hook lets player get rep boost from giving hat to scav
               (also they don't pick up hats by default like they would for pearls) - fix later? */
            // On.Scavenger.PickUpAndPlaceInInventory += Scavenger_PickUpAndPlaceInInventory;
            // On.Scavenger.PlacedGrabbedObjectInCorrectContainer += Scavenger_PlacedGrabbedObjectInCorrectContainer;
            On.ScavengerAI.RecognizeCreatureAcceptingGift += ScavengerAI_RecognizeCreatureAcceptingGift;
            // On.ScavengerAI.Update += ScavengerAI_Update; // doesn't work
            // On.Scavenger.Update += Scavenger_Update; // doesn't work
        }


        // Add hat to scav if HatSaveManager indicates scav should be wearing hat
        // (applicable after quitting and re-entering game/entering new cycle)
        private void ScavengerGraphics_ctor(On.ScavengerGraphics.orig_ctor orig, ScavengerGraphics self, PhysicalObject ow)
        {
            orig(self, ow);

            if (self.scavenger.abstractCreature.ID == wearer && physicalWornHat == null && wornHat == null)
            {
                Debug.Log("hatworld scav graphics load " + self.scavenger.abstractCreature.ID + ((physicalWornHat == null) ? "no hat" : physicalWornHat.ToString()));

                string physicalHatType;
                if (HatSaveManager.hats.TryGetValue(self.scavenger.abstractCreature.ID, out physicalHatType))
                {
                    Debug.Log("Hatworld scav hat detected " + self.scavenger.abstractCreature.ID + " " + physicalHatType);
                    physicalWornHat = HatWorldMain.GetType(physicalHatType);

                    if (physicalWornHat != null)
                    {
                        wornHat = (HatWearing) physicalWornHat.GetMethod("GetWornHat").Invoke(null, new object[] { self });

                        if (effectsOn)
                        {
                            wornHat.AddHatEffects(self.scavenger);
                        }
                    }
                }
            }
        }

        public void ScavengerAI_RecognizeCreatureAcceptingGift(On.ScavengerAI.orig_RecognizeCreatureAcceptingGift orig, ScavengerAI self, Tracker.CreatureRepresentation subRep, Tracker.CreatureRepresentation objRep, bool objIsMe, PhysicalObject item)
        {
            orig(self, subRep, objRep, objIsMe, item);

            if (self.scavenger.abstractCreature.ID == wearer && item is HatPhysical && physicalWornHat == null)
            {
                PutOnHat(self.scavenger);
            }
        }

        public void Scavenger_Stun(On.Scavenger.orig_Stun orig, Scavenger self, int st)
        {
            orig(self, st);

            if (self.abstractCreature.ID == wearer && physicalWornHat != null && self.graphicsModule != null) // only remove hat if scav is on screen
            {
                Debug.Log("Hatworld scav stun " + self.abstractCreature.ID + " " + (self.graphicsModule == null));
                TakeOffHat(self);
            }
        }

        /*
        public void Scavenger_PickUpAndPlaceInInventory(On.Scavenger.orig_PickUpAndPlaceInInventory orig, Scavenger self, PhysicalObject obj)
        {
            orig(self, obj);

            if (self.abstractCreature.ID == wearer && obj is HatPhysical && physicalWornHat == null && self.AI.outpostModule.outpost == null)
            {
                // Debug.Log("Hatworld scav wear " + wearer.abstractCreature.ID + " " + obj.GetType().ToString());
                PutOnHat(self);
            }
        }

        public void Scavenger_PlacedGrabbedObjectInCorrectContainer(On.Scavenger.orig_PlacedGrabbedObjectInCorrectContainer orig, Scavenger self, PhysicalObject obj, int grasp)
        {
            orig(self, obj, grasp);

            if (self.abstractCreature.ID == wearer && obj is HatPhysical && physicalWornHat == null && self.AI.outpostModule.outpost == null)
            {
                PutOnHat(self);
            }
        }
        */

        public override void PutOnHat(Creature self)
        {
            base.PutOnHat(self);

            // Debug.Log("hatworld scav putonhat " + (self.abstractCreature.ID) + ((this.physicalWornHat == null) ? "null physicalwornhat???" : this.physicalWornHat.ToString()));
            if (this.physicalWornHat != null)
            {
                HatSaveManager.hats[self.abstractCreature.ID] = this.physicalWornHat.ToString();
            }
        }

        public override HatPhysical TakeOffHat(Creature self)
        {
            HatSaveManager.hats.Remove(self.abstractCreature.ID);
            return base.TakeOffHat(self);
        }
    }
}
