using RWCustom;
using UnityEngine;

namespace HatWorld
{
	public class FountainWearing : HatWearing
	{
		/*
		 * FountainWearing has a waterJetCustom which creates JetWaterCustom which derives from WaterParticleCustom
		 * This structure allows customizing the WaterDrip in WaterParticleCustom */

		// Constants for sLeaser sprite index (higher index appears over lower)
		public const int botDisk = 1;
		public const int pole = 0;
		public const int topDisk = 2;
        public const int topCircle = 3;

		public ChunkDynamicSoundLoop soundLoop;

        public JetWaterEmitter[] waterJets = new JetWaterEmitter[2];

        public FountainWearing(GraphicsModule parent) : base(parent) {
			this.soundLoop = new ChunkDynamicSoundLoop(parent.owner.firstChunk);
            this.soundLoop.sound = SoundID.Water_Surface_Calm_LOOP;
            this.soundLoop.Volume = 0.6f;
            this.soundLoop.Pitch = 1.6f;
        }

		public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
		{
			sLeaser.sprites = new FSprite[4];
			sLeaser.sprites[botDisk] = new FSprite("QuarterPips2", true) { scaleX = 0.5f };
            sLeaser.sprites[pole] = new FSprite("LizardScaleA1", true) { scaleX = 0.4f };
            sLeaser.sprites[topDisk] = new FSprite("QuarterPips2", true) { scaleY = 0.5f, scaleX = 0.3f };
            sLeaser.sprites[topCircle] = new FSprite("Circle4", true);
			this.AddToContainer(sLeaser, rCam, null);
		}

		public override void ChildDrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
		{
            for (int j = 0; j < sLeaser.sprites.Length; j++)
            {
                sLeaser.sprites[j].rotation = this.rotation + this.baseRot;
            }

            sLeaser.sprites[botDisk].SetPosition(drawPos + upDir * 1);
            sLeaser.sprites[botDisk].rotation += 180f;

			sLeaser.sprites[pole].SetPosition(drawPos + upDir * 8);
            sLeaser.sprites[pole].rotation += 90f;

            sLeaser.sprites[topDisk].SetPosition(drawPos + upDir * 9);
            sLeaser.sprites[topDisk].rotation += 180f;

            sLeaser.sprites[topCircle].SetPosition(drawPos + upDir * 16);

            for (int i = 0; i < waterJets.Length; i++)
            {
                if (this.waterJets[i] != null)
                {
                    this.waterJets[i].Update();
                }
                if (this.waterJets[i] == null)
                {
                    if (this.room != null)
                    {
                        this.waterJets[i] = new JetWaterEmitter(this.room);
                    }
                }
                else if (this.waterJets[i].Dead)
                {
                    this.waterJets[i] = null;
                }
                else
                {
                    if (i != 2)
                    {
                        this.waterJets[i].NewParticle(drawPos + upDir * 7 + camPos, parent.owner.firstChunk.vel + new Vector2(4 + (-8 * i), 0) + upDir * 8, 3.5f, 0.3f);
                    }
                    else
                    {
                        this.waterJets[i].NewParticle(drawPos + upDir * 7 + camPos, parent.owner.firstChunk.vel + upDir * 8, 2f, 0f);
                    }
                }
            }

            if (base.slatedForDeletetion || rCam.room != this.room || this.room != this.parent.owner.room)
            {
                this.soundLoop.Volume = 0f;
            }
            else
            {
                this.soundLoop.Volume = 0.6f;
            }
        }

		public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
		{
            sLeaser.sprites[botDisk].color = new Color(0.62f, 0.70f, 0.96f); // blue-gray
            // sLeaser.sprites[botDisk].color = new Color(0.23f, 0.25f, 0.44f); // indigo
            // sLeaser.sprites[botDisk].color = new Color(0.25f, 0.27f, 0.50f); // lighter indigo
            // sLeaser.sprites[topDisk].color = new Color(0.35f, 0.62f, 0.79f); // cobalt
            sLeaser.sprites[pole].color = new Color(0.72f, 0.73f, 1f); // light blue
            sLeaser.sprites[topDisk].color = sLeaser.sprites[botDisk].color;
		}

        public override void ChildUpdate(bool eu)
        {
            if (this.soundLoop.Volume > 0f)
            {
                this.soundLoop.Update();
            }
        }
    }
}

