// Finished for now

using UnityEngine;
using HatFisobs;

namespace HatWorld
{
    sealed class HatProperties : FisobProperties
    {
        public override void CanThrow(Player player, ref bool throwable)
            => throwable = true;

        // scav rep increase when given this item
        public override void GetScavCollectibleScore(Scavenger scavenger, ref int score)
            => score = 10;

        public override void GetGrabability(Player player, ref Player.ObjectGrabability grabability)
        {
            // can hold 2 at once
            grabability = Player.ObjectGrabability.TwoHands;
        }
    }
}
