using RWCustom;
using UnityEngine;

namespace HatWorld
{
	public class AntennaWearing : HatWearing
	{
        // For glow
        public LightSource? lightSource;

        // Constants for sLeaser sprite index (higher index appears over lower)
        public const int botDisk = 1;
		public const int pole = 0;
		public const int topDisk = 2;
        public const int topCircle = 3;

		public ChunkDynamicSoundLoop soundLoop;

        public AntennaWearing(GraphicsModule parent) : base(parent) {
			this.soundLoop = new ChunkDynamicSoundLoop(parent.owner.firstChunk);
            this.soundLoop.sound = SoundID.Zapper_LOOP;
            this.soundLoop.Pitch = 0.7f;
        }

		public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
		{
			sLeaser.sprites = new FSprite[4];
			sLeaser.sprites[botDisk] = new FSprite("QuarterPips2", true) { scaleX = 0.6f };
            sLeaser.sprites[pole] = new FSprite("LizardScaleA1", true) { scaleY = 1.1f, scaleX = 0.3f };
            sLeaser.sprites[topDisk] = new FSprite("QuarterPips2", true) { scaleY = 0.5f, scaleX = 0.4f };
            sLeaser.sprites[topCircle] = new FSprite("Circle4", true) { scale = 0.7f };
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

            if (base.slatedForDeletetion || rCam.room != this.room || this.room != this.parent.owner.room)
            {
                this.soundLoop.Volume = 0f;

                if (this.lightSource != null && this.room != null)
                {
                    // Remove LightSource objects when changing rooms
                    this.room.RemoveObject(this.lightSource);
                }
                this.lightSource = null;
            }
            else
            {
                this.soundLoop.Volume = 0.45f;

                if (this.room != null) {
                    // from ElectricCat (add sparks)
                    for (int j = 0; j < (int)Mathf.Lerp(4f, 5f, 0.15f); j++)
                    {
                        this.room.AddObject(new Spark(
                            drawPos + upDir * 16 + camPos, Custom.RNV() * Mathf.Lerp(0.5f, 2f, Random.value),
                            // Color.Lerp(new Color(0f, 1f, 0f), new Color(.7f, .9f, .4f), Random.value), 
                            new Color(0f, 1f, 0f, 0.7f),
                            null, 1, 3));
                    }

                    // add glow
                    Vector2 glowPos = drawPos + upDir * 16 + camPos;
                    // From Lantern in game code
                    if (this.lightSource == null)
                    {
                        this.lightSource = new LightSource(glowPos, false, new Color(0.4f, 1f, 0.4f), this);
                        this.lightSource.affectedByPaletteDarkness = 0.5f;
                        this.lightSource.setRad = new float?(30f);
                        this.lightSource.setAlpha = new float?(0.6f);
                        this.room.AddObject(this.lightSource);
                    }
                    else
                    {
                        this.lightSource.setPos = new Vector2?(glowPos);
                    }
                }
            }
        }

		public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
		{
            sLeaser.sprites[botDisk].color = new Color(0.23f, 0.25f, 0.34f); // gray
            sLeaser.sprites[pole].color = new Color(0.9f, 1f, 0.9f); // light gray
            sLeaser.sprites[topDisk].color = sLeaser.sprites[botDisk].color;
            sLeaser.sprites[topCircle].color = new Color(0f, 1f, 0.15f); // green
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
                (wearer as Player).slugcatStats.runspeedFac *= 1.25f;
            }
        }

        public override void RemoveHatEffects(Creature wearer)
        {
            if (wearer is Player)
            {
                (wearer as Player).slugcatStats.runspeedFac *= (1 / 1.25f);
            }
        }

    }
}

