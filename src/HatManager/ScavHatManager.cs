using System;
using UnityEngine;

namespace HatWorld.src.HatManager
{
    public class ScavHatManager : CreatureHatManager
    {
        public ScavHatManager(Scavenger wearer) : base(wearer) { }

        public override void AddHooks()
        {
            base.AddHooks();

            On.Scavenger.PickUpAndPlaceInInventory += Scavenger_PickUpAndPlaceInInventory;
        }

        public void Scavenger_PickUpAndPlaceInInventory(On.Scavenger.orig_PickUpAndPlaceInInventory orig, Scavenger self, PhysicalObject obj)
        {
            orig(self, obj);

            if (self == wearer && obj is HatPhysical)
            {
                Debug.Log("Hatworld scav wear hat " + obj.GetType());
                PutOnHat(self);
            }
        }

    }
}
