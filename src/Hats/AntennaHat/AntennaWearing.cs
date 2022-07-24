using RWCustom;
using UnityEngine;

namespace HatWorld
{
	public class AntennaWearing : HatWearing
	{
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
            }
            else
            {
                this.soundLoop.Volume = 0.3f;

                if (this.room != null) {
                    // from ElectricCat
                    for (int j = 0; j < (int)Mathf.Lerp(4f, 5f, 0.15f); j++)
                    {
                        this.room.AddObject(new Spark(drawPos + upDir * 16 + camPos, Custom.RNV() * Mathf.Lerp(0.5f, 2f, Random.value), new Color(1f, 0.9f, .51f), null, 1, 3));
                    }
                }
            }
        }

		public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
		{
            sLeaser.sprites[botDisk].color = new Color(0.23f, 0.25f, 0.34f); // gray
            sLeaser.sprites[pole].color = new Color(0.9f, 0.9f, 0.9f); // light gray
            sLeaser.sprites[topDisk].color = sLeaser.sprites[botDisk].color;
            sLeaser.sprites[topCircle].color = new Color(1f, 0.95f, 0.46f); // gold
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
                (wearer as Player).slugcatStats.runspeedFac *= 1.15f;
            }
        }

        public override void RemoveHatEffects(Creature wearer)
        {
            if (wearer is Player)
            {
                (wearer as Player).slugcatStats.runspeedFac *= (1 / 1.15f);
            }
        }

    }
}

