using UnityEngine;
using RWCustom;

namespace HatWorld
{
    sealed class FlowerHatPhysical : HatPhysical
    {
        // Constants for sLeaser sprite index (higher index appears over lower)
        public int coneIndex = 0;

		public override HatType hatType => HatType.Flower;

        public FlowerHatPhysical(HatAbstract abstr, World world) : base(abstr, world) { }

        public override void Update(bool eu)
        {
            base.Update(eu);

            if (this.room != null)
            {
			}
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            // Taken from FestiveWorld SantaHat
            sLeaser.sprites = new FSprite[0];

            this.AddToContainer(sLeaser, rCam, null);
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
        }

        public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
        }
    }
}

