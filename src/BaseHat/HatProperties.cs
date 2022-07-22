// Finished for now

using UnityEngine;
namespace HatWorld
{
    sealed class HatProperties : Fisobs.Properties.ItemProperties
    {
        public override void Throwable(Player player, ref bool throwable)
            => throwable = true;

        // scav rep increase when given this item
        public override void ScavCollectScore(Scavenger scavenger, ref int score)
            => score = 10;

        public override void Grabability(Player player, ref Player.ObjectGrabability grabability)
        {
            // can hold 2 at once
            grabability = Player.ObjectGrabability.OneHand;
        }
    }
}
