using UnityEngine;
using RWCustom;

namespace HatWorld
{
    sealed class BubbleHatPhysical : HatPhysical
    {
        // Constants for sLeaser sprite index (higher index appears over lower)
        public const int neckIndex = 0;
        public const int edgeIndex = 1;

		public override HatType hatType => HatType.Bubble;

        public BubbleHatPhysical(HatAbstract abstr, World world) : base(abstr, world) {}

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
			sLeaser.sprites = new FSprite[2];
            sLeaser.sprites[neckIndex] = new FSprite("SpearRag", true);
            sLeaser.sprites[edgeIndex] = new FSprite("LizardBubble7", true);

            this.AddToContainer(sLeaser, rCam, null);
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            // Setup
            base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
            for (int j = 0; j < sLeaser.sprites.Length; j++)
            {
                sLeaser.sprites[j].rotation = hatRotation;
            }

            /* Set positions */
            sLeaser.sprites[neckIndex].SetPosition(drawPos);
            sLeaser.sprites[edgeIndex].SetPosition(drawPos + upDir * 5f);
        }

        public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
        }
    }
}

