/*
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
        public ShapeCustom holoShape;

        public float holoFade;
        public float lastHoloFade;
        public float holoErrors;
        public float lastHoloErrors;
        public int hologramCounter;

        public AntennaWearing(GraphicsModule parent) : base(parent) {
			this.soundLoop = new ChunkDynamicSoundLoop(parent.owner.firstChunk);
            this.soundLoop.sound = SoundID.Zapper_LOOP;
            this.soundLoop.Volume = 0.1f;
            this.soundLoop.Pitch = 0.7f;

            this.holoShape = new ShapeCustom(null, NSHSwarmer.Shape.ShapeType.Main, new Vector3(0f, 0f, 0f), 0f, 0f);
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

            Vector2 vector = basePos;
            Vector2 vector5 = basePos;
            int num5 = 6;
            this.holoShape.Draw(sLeaser, rCam, timeStacker, vector, camPos, ref num5, Custom.VecToDeg(Vector3.Slerp(vector3, new Vector2(0f, 1f), 0.51f)), Mathf.Lerp(this.lastHoloErrors, this.holoErrors, timeStacker), num4, false, ref vector5, ref 1f, ref to, ref this.directionsPower);
            vector5 /= 1f;
            sLeaser.sprites[5].isVisible = true;
            sLeaser.sprites[5].x = vector5.x - camPos.x;
            sLeaser.sprites[5].y = vector5.y - camPos.y;
            sLeaser.sprites[5].alpha = Custom.SCurve(Mathf.Pow(Mathf.InverseLerp(15f, 500f, 1f), 0.5f), 0.8f) * Mathf.Pow(num4, 0.4f) * 0.4f;
            sLeaser.sprites[5].scaleX = (40f + Mathf.Lerp(Custom.SCurve(Mathf.InverseLerp(5f, 300f, 1f), 0.8f) * 120f, to, 0.5f)) * Mathf.Pow(num4, 1.4f) / 8f;
            sLeaser.sprites[5].scaleY = (40f + Mathf.Lerp(Custom.SCurve(Mathf.InverseLerp(5f, 300f, 1f), 0.8f) * 120f, to, 0.5f)) * (0.5f + 0.5f * Mathf.Pow(num4, 0.4f)) / 8f;
            // sLeaser.sprites[5].rotation = Custom.VecToDeg(this.lazyDirection);

            if (base.slatedForDeletetion || rCam.room != this.room || this.room != this.parent.owner.room)
            {
                this.soundLoop.Volume = 0f;
            }
            else
            {
                this.soundLoop.Volume = 0.6f;

                if (this.room != null) {
                    // from ElectricCat
                    for (int j = 0; j < (int)Mathf.Lerp(4f, 5f, 0.15f); j++)
                    {
                        Vector2 b = new Vector2(0f, Random.Range(-15f, 10f));
                        this.room.AddObject(new Spark(drawPos + b, Custom.RNV() * Mathf.Lerp(4f, 14f, Random.value), new Color(1f, 0.86f, .35f), null, 2, 14));
                    }
                }
            }
        }

		public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
		{
            sLeaser.sprites[botDisk].color = new Color(0.23f, 0.25f, 0.34f); // indigo
            sLeaser.sprites[pole].color = new Color(0.9f, 0.9f, 0.9f); // light blue
            sLeaser.sprites[topDisk].color = sLeaser.sprites[botDisk].color;
            sLeaser.sprites[topCircle].color = Color.white;
		}

        public override void ChildUpdate(bool eu)
        {
            this.lastHoloFade = this.holoFade;
            this.lastHoloErrors = this.holoErrors;

            this.hologramCounter--;
            this.hologramCounter = Custom.IntClamp(this.hologramCounter, 0, 160);

            if (this.hologramCounter > 60) // && this.grabbedBy.Count == 0)

            {
                this.holoFade = Custom.LerpAndTick(this.holoFade, Mathf.InverseLerp(60f, 160f, (float)this.hologramCounter), 0.06f, 0.016666668f);
            }
            else

            {
                this.holoFade = Mathf.Max(0f, this.holoFade - 0.083333336f);
            }
            if (this.holoFade > 0f || this.lastHoloFade > 0f)

            {
                float[,] a = new float[2, 3];
                this.holoShape.Update(this.hologramCounter < 100 || UnityEngine.Random.value < 0.05f, this.holoErrors, this.holoFade, new Vector2(0, 1), 0, ref a); // base.firstChunk.pos - base.firstChunk.lastPos, Custom.VecToDeg(this.lazyDirection), ref this.directionsPower);
            }
            else

            {
                this.holoShape.ResetUpdate(basePos);
            }

            if (this.soundLoop.Volume > 0f)
            {
                this.soundLoop.Update();
            }
        }
    }
}
*/
