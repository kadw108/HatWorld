using RWCustom;
using UnityEngine;

namespace HatWorld
{
	sealed class WearingFlowerHat : WearingHat
	{
        // Constants for sLeaser sprite index (higher index appears over lower)
        public int coneIndex = 0;

		public override HatType hatType => HatType.Flower;

		public WearingFlowerHat(GraphicsModule parent, int anchorSprite, float rotation, float headRadius)
			: base(parent, anchorSprite, rotation, headRadius) {}

		public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
		{
			sLeaser.sprites = new FSprite[0];
			this.AddToContainer(sLeaser, rCam, null);
		}

		public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
		{
			base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
		}

		public override void ChildUpdate(bool eu)
		{
		}

		public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
		{
		}
	}
}

