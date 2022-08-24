using RWCustom;
using UnityEngine;

namespace HatWorld
{
	public class FountainWearing : HatWearing
	{
		/*
		 * FountainWearing has a JetWaterEmitter (WaterJet) which creates JetWaterCustom which derives from WaterParticleCustom
		 * This structure allows customizing the WaterParticleCustom methods */

		// Constants for sLeaser sprite index (higher index appears over lower)
		public const int pole = 0;

        public const int petal1 = 1;
        public const int petal2 = 2;
        public const int petal3 = 3;
        public const int petal4 = 4;

		public ChunkDynamicSoundLoop soundLoop;

        public JetWaterEmitter[] waterJets = new JetWaterEmitter[2];


        public FountainWearing(GraphicsModule parent) : base(parent) {
			this.soundLoop = new ChunkDynamicSoundLoop(parent.owner.firstChunk);
            this.soundLoop.sound = SoundID.Water_Surface_Calm_LOOP;
            this.soundLoop.Pitch = 1.6f;
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
		{
			sLeaser.sprites = new FSprite[5];
            sLeaser.sprites[pole] = new FSprite("LizardScaleA1", true) { scaleY = 0.35f, scaleX = 0.5f };

            for (int i = petal1; i <= petal4; i++)
            {
                sLeaser.sprites[i] = new FSprite("KarmaPetal", true) { scaleY = 0.55f, scaleX = 0.3f };
            }

			this.AddToContainer(sLeaser, rCam, null);
		}

		public override void ChildDrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
		{
            for (int j = 0; j < petal1; j++)
            {
                sLeaser.sprites[j].rotation = this.rotation + this.baseRot;
            }

			sLeaser.sprites[pole].SetPosition(drawPos + upDir * 4);
            sLeaser.sprites[pole].rotation += 90f;

            for (int i = petal1; i <= petal4; i++)
            {
                float rotShift = this.rotation + 45 + (i - petal1) * 39 + this.baseRot;
                sLeaser.sprites[i].rotation = rotShift;
                sLeaser.sprites[i].SetPosition(drawPos + upDir * 6 + Custom.DegToVec(rotShift) * 4);
            }

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
                    if (i != 2) // side water jets
                    {
                        this.waterJets[i].NewParticle(drawPos + upDir * 7 + camPos, parent.owner.firstChunk.vel + new Vector2(2 + (-4 * i), 0) + upDir * 4, 1.4f, 0.3f);
                    }
                    else // center water jet (removed for now)
                    {
                        this.waterJets[i].NewParticle(drawPos + upDir * 7 + camPos, parent.owner.firstChunk.vel + upDir * 8, 1f, 0f);
                    }
                }
            }

            if (base.slatedForDeletetion || rCam.room != this.room || this.room != this.parent.owner.room)
            {
                this.soundLoop.Volume = 0f;
            }
            else
            {
                this.soundLoop.Volume = 0.3f;
            }
        }

		public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
		{
            sLeaser.sprites[pole].color = new Color(0.72f, 0.73f, 1f); // light blue

            for (int i = petal1; i <= petal4; i++)
            {
                sLeaser.sprites[i].color = new Color(.6f + (i-petal1) * .1f, .9f, .9f + (i-petal1) * .3f); // light blue - light red
            }
		}

        public override void ChildUpdate(bool eu)
        {
            if (this.soundLoop.Volume > 0f)
            {
                this.soundLoop.Update();
            }
        }

        public override void AddHatEffects(Creature wearer)
        {
            if (wearer is Player)
            {
                On.Player.Update += Player_Update;
            }
        }

        public override void RemoveHatEffects(Creature wearer)
        {
            if (wearer is Player)
            {
                On.Player.Update -= Player_Update;
            }
        }

        public override void Destroy()
        {
            base.Destroy();
        }

        private void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig(self, eu);

            if (parent.owner == self)
            {
                (parent.owner as Player).swimForce = 0.7f;
            }
        }
    }
}

