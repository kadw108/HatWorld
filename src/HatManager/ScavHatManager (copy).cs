/*

// Version of ScavHatManager that uses HatSaveManager.hats instead of wearer to track which scav is wearing which hat.
// Might be more efficient in practice. But it doesn't work - hats appear in wrong places.

using UnityEngine;

namespace HatWorld.src.HatManager
{
    public class ScavHatManager : CreatureHatManager
    {
        public ScavHatManager(Scavenger wearer) : base(wearer)
        {

            if (!HatSaveManager.hats.ContainsKey(wearer.abstractCreature.ID))
            {
                HatSaveManager.hats[wearer.abstractCreature.ID] = "";
            }
        }

        public static bool hooksAdded = false;

        public override void AddHooks()
        {
            base.AddHooks();

            if (!hooksAdded)
            {
                On.ScavengerGraphics.ctor += ScavengerGraphics_ctor;
                On.Scavenger.Stun += Scavenger_Stun;
                On.Scavenger.PickUpAndPlaceInInventory += Scavenger_PickUpAndPlaceInInventory;
                On.ScavengerAI.RecognizeCreatureAcceptingGift += ScavengerAI_RecognizeCreatureAcceptingGift;
                // On.ScavengerAI.Update += ScavengerAI_Update; // doesn't work
                // On.Scavenger.Update += Scavenger_Update; // doesn't work

                hooksAdded = true;
            }
        }

        private void ScavengerGraphics_ctor(On.ScavengerGraphics.orig_ctor orig, ScavengerGraphics self, PhysicalObject ow)
        {
            orig(self, ow);

            Debug.Log("scav graphics ctor " + (physicalWornHat == null) + (self.scavenger == null) + (self.scavenger.abstractCreature == null) + (this.wearer == null));
            if (physicalWornHat == null)
            {
                if (HatSaveManager.hats.TryGetValue(self.scavenger.abstractCreature.ID, out string physicalHatType) && physicalHatType != "")
                {
                    Debug.Log("Hatworld scav hat detected " + self.scavenger.abstractCreature.ID + " " + physicalHatType);
                    physicalWornHat = HatWorldMain.GetType(physicalHatType);
                    wornHat = (HatWearing)physicalWornHat.GetMethod("GetWornHat").Invoke(null, new object[] { self });

                    if (effectsOn)
                    {
                        wornHat.AddHatEffects(self.scavenger);
                    }
                }
            }
        }

        public void ScavengerAI_RecognizeCreatureAcceptingGift(On.ScavengerAI.orig_RecognizeCreatureAcceptingGift orig, ScavengerAI self, Tracker.CreatureRepresentation subRep, Tracker.CreatureRepresentation objRep, bool objIsMe, PhysicalObject item)
        {
            orig(self, subRep, objRep, objIsMe, item);

            if (HatSaveManager.hats.TryGetValue(self.scavenger.abstractCreature.ID, out string physicalHatType) && item is HatPhysical && physicalHatType == "")
            {
                PutOnHat(self.scavenger);
            }
        }

        public void Scavenger_Stun(On.Scavenger.orig_Stun orig, Scavenger self, int st)
        {
            orig(self, st);

            if (HatSaveManager.hats.TryGetValue(self.abstractCreature.ID, out string physicalHatType) && physicalHatType != "")
            {
                Debug.Log("Hatworld scav stun " + self.abstractCreature.ID + " " + (self.graphicsModule == null));
                HatSaveManager.hats[self.abstractCreature.ID] = "";
                TakeOffHat(self);
            }
        }

        public void Scavenger_PickUpAndPlaceInInventory(On.Scavenger.orig_PickUpAndPlaceInInventory orig, Scavenger self, PhysicalObject obj)
        {
            orig(self, obj);

            Debug.Log("hatworld scav pick up " + self.abstractCreature.ID + " " + HatSaveManager.hats[self.abstractCreature.ID] + " " + obj.GetType().ToString());
            if (obj is HatPhysical && HatSaveManager.hats.TryGetValue(self.abstractCreature.ID, out string physicalHatType) && physicalHatType == "" && self.AI.outpostModule.outpost == null)
            {
                Debug.Log("hatworld pass " + physicalHatType);
                PutOnHat(self);

                Debug.Log("hat [ut pno");
            }
        }

        public override void PutOnHat(Creature self)
        {
            base.PutOnHat(self);

            Debug.Log("special put on hat AAAAAAAAAAAAAAAAAAAAAAA");
            Debug.Log("hatworld scav putonhat " + (this.physicalWornHat == null));
            if (this.physicalWornHat != null)
            {
                HatSaveManager.hats[self.abstractCreature.ID] = this.physicalWornHat.ToString();
            }
        }

        public override HatPhysical TakeOffHat(Creature self)
        {
            HatSaveManager.hats[self.abstractCreature.ID] = "";
            return base.TakeOffHat(self);
        }
    }
}
*/
