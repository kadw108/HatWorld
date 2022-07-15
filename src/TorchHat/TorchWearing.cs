using RWCustom;
using UnityEngine;

namespace HatWorld
{
	// Based on HolyFire
	public class TorchWearing : HatWearing
	{
		// from HolyFire
		public LightSource[] lightSources;

		// from Lantern
		public float[] flicker;

        // Constants for sLeaser sprite index (higher index appears over lower)
		public const int crownIndex = 0;
		public const int gemIndex = 1;

		public TorchWearing(GraphicsModule parent) : base(parent)
		{
			lightSources = new LightSource[2];
			this.flicker = new float[3] {1f, 1f, 1f};
		}

		public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
		{

			/* add hat sprites */
			sLeaser.sprites = new FSprite[2];
			sLeaser.sprites[crownIndex] = new FSprite("SpiderLeg3B", true);
            sLeaser.sprites[crownIndex].scaleX = 1.4f;
            sLeaser.sprites[crownIndex].scaleY = 0.9f;
			sLeaser.sprites[gemIndex] = new FSprite("deerEyeA", true);
            sLeaser.sprites[gemIndex].scale = 0.5f;

			this.AddToContainer(sLeaser, rCam, null);
		}

		public override void ChildDrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
		{
			/* Crown */
            sLeaser.sprites[crownIndex].SetPosition(drawPos);
			sLeaser.sprites[crownIndex].rotation = this.rotation + this.baseRot;
			sLeaser.sprites[crownIndex].scaleY = (this.flipX ? -0.9f : 0.9f);

			/* Flame */
			Vector2 firePosition = drawPos + upDir * 3;
			sLeaser.sprites[gemIndex].SetPosition(firePosition);

			if (base.slatedForDeletetion || rCam.room != this.room || this.room != this.parent.owner.room)
			{
				for (int i = 0; i < this.lightSources.Length; i++)
                {
					if (this.lightSources[i] != null)
                    {
						// Remove LightSource objects when changing rooms
						this.room.RemoveObject(this.lightSources[i]);
                    }
					this.lightSources[i] = null;
                }
			}
			else
            {
				/* Fire particles */
				this.room.AddObject(new FlameParticle(firePosition));

                /* Light */
                Vector2 camAdjustedFirePos = firePosition + camPos;

                // From Lantern in game code
                if (this.lightSources[0] == null || this.lightSources[1] == null)
                {
                    // 0 is big light, 1 is small light

                    this.lightSources[0] = new LightSource(camAdjustedFirePos, false, new Color(0.7f, 0.4f, 0f), this);
                    this.lightSources[0].affectedByPaletteDarkness = 0.5f;
                    this.lightSources[0].setAlpha = new float?(0.8f);
                    this.room.AddObject(this.lightSources[0]);

                    this.lightSources[1] = new LightSource(camAdjustedFirePos, false, new Color(1f, 0.8f, 0.4f), this);
                    this.lightSources[1].affectedByPaletteDarkness = 0.9f;
					this.lightSources[1].setRad = new float?(40f);
					this.lightSources[1].setAlpha = new float?(1.9f);
                    this.room.AddObject(this.lightSources[1]);
                }
                else
                {
					this.lightSources[0].setRad = new float?(170f * this.flicker[0]);

                    for (int i = 0; i < this.lightSources.Length; i++)
                    {
						this.lightSources[i].setPos = new Vector2?(camAdjustedFirePos);
                    }
                }
            }
		}

		public override void ChildUpdate(bool eu)
		{
            // from Lantern
            this.flicker[1] = this.flicker[0];
            this.flicker[0] += Mathf.Pow(Random.value, 3f) * 0.1f * ((Random.value >= 0.5f) ? 1f : -1f);
            this.flicker[0] = Custom.LerpAndTick(this.flicker[0], this.flicker[2], 0.05f, 0.033333335f);
            if (Random.value < 0.2f)
            {
                this.flicker[2] = 1f + Mathf.Pow(Random.value, 3f) * 0.2f * ((Random.value >= 0.5f) ? 1f : -1f);
            }
            this.flicker[2] = Mathf.Lerp(this.flicker[2], 1f, 0.01f);
		}

		public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
		{
			// sLeaser.sprites[crownIndex].color = new Color(0.15f, 0.07f, 0.2f); // steel
			// sLeaser.sprites[crownIndex].color = new Color(0.86f, 0.55f, 0.11f); // gold
            sLeaser.sprites[crownIndex].color = new Color(0.82f, 0.62f, 0f); // gold 2
			// sLeaser.sprites[crownIndex].color = new Color(0.68f, 0.54f, 0.13f); // burnished gold
		}
	}
}

