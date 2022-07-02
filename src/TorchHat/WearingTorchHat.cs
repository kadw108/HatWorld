using RWCustom;
using UnityEngine;

namespace HatWorld
{
	// Based on HolyFire
	public class WearingTorchHat : WearingHat
	{
		public GraphicsModule parent;

		public int anchorSprite;

		public float rotation;
		public float headRadius;

		public bool flipX;
		public bool flipY;

		public Vector2 basePos;
		public float baseRot;

		// from HolyFire
		public LightSource[] lightSources;
		public Vector2[] getToPositions;
		public float[] getToRads;
		public float[,] hues;
		public LightSource flatLightSource;

		public override HatType hatType => HatType.Torch;

		public WearingTorchHat(GraphicsModule parent, int anchorSprite, float rotation, float headRadius)
		{
			this.parent = parent;
			this.anchorSprite = anchorSprite;
			this.rotation = rotation;
			this.headRadius = headRadius;
			parent.owner.room.AddObject(this);
		}

        // Constants for sLeaser sprite index (higher index appears over lower)
		public const int botIndex = 0;
		public const int fireIndex = 1;

		public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
		{
			this.lightSources = new LightSource[3];
			this.getToPositions = new Vector2[this.lightSources.Length];
			this.getToRads = new float[this.lightSources.Length];
			this.hues = new float[this.lightSources.Length, 2];
			/*
			for (int i = 0; i < this.lightSources.Length; i++)
			{
				this.lightSources[i] = new LightSource(placedObject.pos, false, Custom.HSL2RGB(Mathf.Lerp(0.01f, 0.07f, (float)i / (float)(this.lightSources.Length - 1)), 1f, 0.5f), this);
				placedInRoom.AddObject(this.lightSources[i]);
				this.lightSources[i].setAlpha = new float?(1f);
			}
			this.flatLightSource = new LightSource(placedObject.pos, false, new Color(1f, 1f, 1f), this);
			this.flatLightSource.flat = true;
			placedInRoom.AddObject(this.flatLightSource);
			*/

			sLeaser.sprites = new FSprite[2];
			sLeaser.sprites[botIndex] = new FSprite("SpiderLeg3B", true);
			sLeaser.sprites[fireIndex] = new FSprite("deerEyeB", true);

			this.AddToContainer(sLeaser, rCam, null);
		}

		public override void ParentDrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
		{
			if (sLeaser.sprites.Length > this.anchorSprite)
			{
				this.basePos.Set(sLeaser.sprites[this.anchorSprite].x, sLeaser.sprites[this.anchorSprite].y);
				this.baseRot = sLeaser.sprites[this.anchorSprite].rotation;

				this.flipX = (sLeaser.sprites[this.anchorSprite].scaleX > 0f);
				this.flipY = (sLeaser.sprites[this.anchorSprite].scaleY < 0f);
			}
		}

		public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
		{
			Vector2 drawPos = new Vector2(this.basePos.x, this.basePos.y); // increase y so hat doesn't go over eyes

			Vector2 upDir = new Vector2(Mathf.Cos((this.rotation + this.baseRot) * -0.017453292f), Mathf.Sin((this.rotation + this.baseRot) * -0.017453292f));
			Vector2 rightDir = -Custom.PerpendicularVector(upDir);
            if (flipY) upDir *= -1;
            if (flipX) rightDir *= -1;
			drawPos += upDir * this.headRadius;

			/* Crown */
            sLeaser.sprites[botIndex].SetPosition(drawPos);
			sLeaser.sprites[botIndex].rotation = this.rotation + this.baseRot;
			sLeaser.sprites[botIndex].scaleY = (this.flipX ? -1f : 1f);

			/* Flame */
			Vector2 firePosition = drawPos + upDir * 3;
			sLeaser.sprites[fireIndex].SetPosition(firePosition);
			// sLeaser.sprites[fireIndex].rotation = this.rotation + this.baseRot;
			sLeaser.sprites[fireIndex].scaleY = (this.flipX ? -1f : 1f);

			if (base.slatedForDeletetion || rCam.room != this.room || this.room != this.parent.owner.room)
			{
				sLeaser.CleanSpritesAndRemove();
			}
			else {
				/* Fire particles */
				this.room.AddObject(new FlameParticle(firePosition));
			}
		}

		public override void Update(bool eu)
		{
			base.Update(eu);


			GraphicsModule graphicsModule = this.parent;
			if (((graphicsModule != null) ? graphicsModule.owner : null) == null || this.parent.owner.slatedForDeletetion || base.slatedForDeletetion)
			{
				this.Destroy();
			}
			else if (this.parent.owner.room != null)
			{
			}

			if (base.slatedForDeletetion)
			{
				base.RemoveFromRoom();
			}
		}

		public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
		{
			sLeaser.sprites[botIndex].color = new Color(0.15f, 0.07f, 0.2f);
		}
	}
}

