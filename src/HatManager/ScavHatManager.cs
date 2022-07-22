using System;
using UnityEngine;

namespace HatWorld.src.HatManager
{
    public class ScavHatManager : CreatureHatManager
    {
        // make public static hashmap of scav id : wearing hat type to keep track of hats

        public ScavHatManager(Scavenger wearer) : base(wearer) { }

        public override void AddHooks()
        {
            base.AddHooks();

            On.Scavenger.Stun += Scavenger_Stun;
            On.Scavenger.PickUpAndPlaceInInventory += Scavenger_PickUpAndPlaceInInventory;
            On.ScavengerAI.RecognizeCreatureAcceptingGift += ScavengerAI_RecognizeCreatureAcceptingGift;
            // On.ScavengerAI.Update += ScavengerAI_Update; // doesn't work
            // On.Scavenger.Update += Scavenger_Update; // doesn't work
        }


        public void ScavengerAI_RecognizeCreatureAcceptingGift(On.ScavengerAI.orig_RecognizeCreatureAcceptingGift orig, ScavengerAI self, Tracker.CreatureRepresentation subRep, Tracker.CreatureRepresentation objRep, bool objIsMe, PhysicalObject item)
        {
            orig(self, subRep, objRep, objIsMe, item);

            if (self == (wearer as Scavenger).AI && item is HatPhysical && physicalWornHat == null)
            {
                PutOnHat(wearer);
            }
        }

        public void Scavenger_Stun(On.Scavenger.orig_Stun orig, Scavenger self, int st)
        {
            orig(self, st);

            if (self == wearer)
            {
                TakeOffHat(self);
            }
        }

        public void Scavenger_PickUpAndPlaceInInventory(On.Scavenger.orig_PickUpAndPlaceInInventory orig, Scavenger self, PhysicalObject obj)
        {
            orig(self, obj);

            if (self == wearer && obj is HatPhysical && physicalWornHat == null && self.AI.outpostModule.outpost == null)
            {
                PutOnHat(self);
            }
        }
    }
}
