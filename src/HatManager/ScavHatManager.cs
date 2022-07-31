using UnityEngine;

namespace HatWorld.src.HatManager
{
    public class ScavHatManager : CreatureHatManager
    {
        public ScavHatManager(Scavenger wearer) : base(wearer) { }

        public override void AddHooks()
        {
            base.AddHooks();

            On.ScavengerGraphics.ctor += ScavengerGraphics_ctor;
            On.Scavenger.Stun += Scavenger_Stun;
            On.Scavenger.PickUpAndPlaceInInventory += Scavenger_PickUpAndPlaceInInventory;
            On.ScavengerAI.RecognizeCreatureAcceptingGift += ScavengerAI_RecognizeCreatureAcceptingGift;
            // On.ScavengerAI.Update += ScavengerAI_Update; // doesn't work
            // On.Scavenger.Update += Scavenger_Update; // doesn't work
        }

        private void ScavengerGraphics_ctor(On.ScavengerGraphics.orig_ctor orig, ScavengerGraphics self, PhysicalObject ow)
        {
            orig(self, ow);

            if (physicalWornHat == null)
            {
                string physicalHatType;
                if (HatSaveManager.hats.TryGetValue(self.scavenger.abstractCreature.ID, out physicalHatType))
                {
                    Debug.Log("Hatworld scav hat detected " + self.scavenger.abstractCreature.ID + " " + physicalHatType);
                    physicalWornHat = HatWorldMain.GetType(physicalHatType);
                    wornHat = (HatWearing) physicalWornHat.GetMethod("GetWornHat").Invoke(null, new object[] { self });

                    if (effectsOn)
                    {
                        wornHat.AddHatEffects(wearer);
                    }
                }
            }
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

            if (self == wearer && physicalWornHat != null)
            {
                Debug.Log("Hatworld scav stun " + self.abstractCreature.ID + " " + (self.graphicsModule == null));
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

        public override void PutOnHat(Creature self)
        {
            base.PutOnHat(self);

            Debug.Log("hatworld scav putonhat " + this.physicalWornHat == null);
            if (this.physicalWornHat != null)
            {
                HatSaveManager.hats[self.abstractCreature.ID] = this.physicalWornHat.ToString(); // this.physicalWornHat.Namespace + "." + this.physicalWornHat.Name;
            }
        }

        public override HatPhysical TakeOffHat(Creature self)
        {
            HatSaveManager.hats.Remove(self.abstractCreature.ID);
            return base.TakeOffHat(self);
        }
    }
}
