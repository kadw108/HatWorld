using System;
using RWCustom;
using UnityEngine;

namespace HatWorld
{
	public class WingWearing : HatWearing
	{
		public float[] wingLengths;
		public float wingsFolded;
		public float lastWingsFolded;
		public float wingFlapCycle;
		public float lastWingFlapCycle;

		public float wingsStartedUp = 1f;

		public int wingPairs = 2;

        // Constants for sLeaser sprite index (higher index appears over lower)
		public const int wingLeft1 = 1;
		public const int wingRight1 = 0;
		public const int circleLeft = 2;
		public const int circleRight = 3;

		public float defaultRotat;
		public Vector2[,] bodyRotations;

		public ChunkDynamicSoundLoop soundLoop;

		public WingWearing(GraphicsModule parent) : base(parent) {
			this.defaultRotat = Mathf.Lerp(-5f, 5f, UnityEngine.Random.value);
			this.bodyRotations = new Vector2[3, 2];
			for (int i = 0; i < this.bodyRotations.GetLength(0); i++)
			{
				this.bodyRotations[i, 0] = Custom.DegToVec(this.defaultRotat);
				this.bodyRotations[i, 1] = Custom.DegToVec(this.defaultRotat);
			}
			this.wingLengths = new float[2] { 25f, 25f };

			this.soundLoop = new ChunkDynamicSoundLoop(parent.owner.firstChunk);
			this.soundLoop.sound = SoundID.Centiwing_Fly_LOOP;
			this.soundLoop.Pitch = 0.9f;
		}

		public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
		{
			sLeaser.sprites = new FSprite[4];
            sLeaser.sprites[wingLeft1] = new CustomFSprite("CentipedeWing");
            sLeaser.sprites[wingLeft1].shader = rCam.room.game.rainWorld.Shaders["CicadaWing"];
            sLeaser.sprites[wingRight1] = new CustomFSprite("CentipedeWing");
            sLeaser.sprites[wingRight1].shader = rCam.room.game.rainWorld.Shaders["CicadaWing"];
			sLeaser.sprites[circleLeft] = new FSprite("Circle4") { scale = 0.7f };
			sLeaser.sprites[circleRight] = new FSprite("Circle4") { scale = 0.7f };

			this.AddToContainer(sLeaser, rCam, null);
		}

		public override void ChildDrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
		{
            this.drawPos.y += 2 * Math.Abs(rightDir.y); // increase hat height based on direction slugcat is facing, needed for different head sprites
			this.drawPos += 2 * upDir;

			sLeaser.sprites[circleLeft].SetPosition(drawPos - 2 * rightDir);
			sLeaser.sprites[circleRight].SetPosition(drawPos + 2 * rightDir);

			for (int k = 0; k < 2; k++)
			{
				int num10 = 0;
				Vector2 wingWidth = new Vector2(0, 1); // wing width and orientation, originally for direction of centipede travel, (1, 0) flips wings 90 deg (bad)
				Vector2 vector11 = Custom.PerpendicularVector(wingWidth);
				Vector2 vector12 = this.RotatAtChunk(num10, timeStacker);
				Vector2 vector13 = this.WingPos(k, num10, wingWidth, vector11, vector12, timeStacker); // tip of wing
				Vector2 vector14 = (k == wingLeft1) ?
					sLeaser.sprites[circleLeft].GetPosition() :
					sLeaser.sprites[circleRight].GetPosition();
				(sLeaser.sprites[k] as CustomFSprite).MoveVertice(1, vector13 + wingWidth * 2f);
				(sLeaser.sprites[k] as CustomFSprite).MoveVertice(0, vector13 - wingWidth * 2f);
				(sLeaser.sprites[k] as CustomFSprite).MoveVertice(2, vector14 + wingWidth * 2f);
				(sLeaser.sprites[k] as CustomFSprite).MoveVertice(3, vector14 - wingWidth * 2f);
			}

			if (base.slatedForDeletetion || rCam.room != this.room || this.room != this.parent.owner.room)
			{
				this.soundLoop.Volume = 0f;
			} else
            {
                this.soundLoop.Volume = 0.5f;
            }
		}

		public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
		{
			for (int k = 0; k < 2; k++) {
                for (int i = 0; i < 4; i++)
                {
					(sLeaser.sprites[k] as CustomFSprite).verticeColors[0] = new Color(1f, 0.7f, 0.53f); // lighter red-orange
					(sLeaser.sprites[k] as CustomFSprite).verticeColors[1] = new Color(1f, 0.7f, 0.53f); 
					(sLeaser.sprites[k] as CustomFSprite).verticeColors[2] = Color.white;
					(sLeaser.sprites[k] as CustomFSprite).verticeColors[3] = Color.white; 
                }
            }
            sLeaser.sprites[circleLeft].color = new Color(0.92f, 0.43f, 0.36f);
            sLeaser.sprites[circleRight].color = sLeaser.sprites[circleLeft].color;
		}

        public override void AddHatEffects(Creature wearer)
        {
			wearer.gravity = 0.6f;
        }
        public override void RemoveHatEffects(Creature wearer)
        {
			wearer.gravity = 0.9f;
        }

        /* From CentipedeGraphics */
        public override void ChildUpdate(bool eu)
		{
			this.lastWingFlapCycle = this.wingFlapCycle;
			this.wingFlapCycle += Mathf.Pow(this.wingsStartedUp, 3f);
			this.lastWingsFolded = this.wingsFolded;
			this.wingsFolded = 1f - this.wingsStartedUp;

			/*
			// doesn't have intended effect
			if (Math.Abs(this.parent.owner.firstChunk.vel.y) > 1 && (this.parent.owner.gravity > 0))
            {
				this.wingsStartedUp = 1f;
            }
			else
            {
				this.wingsStartedUp = 0f;
            }
			*/

			if (this.soundLoop.Volume > 0)
            {
                this.soundLoop.Update();
            }
		}

		public Vector2 WingPos(int side, int wing, Vector2 dr, Vector2 prp, Vector2 chunkRotat, float timeStacker)
		{
			float t = (float)wing / (float)(this.wingPairs - 1);
			float f = this.VerticalWingFlapAtChunk(wing, timeStacker);
			float num = this.HorizontalWingFlapAtChunk(wing, chunkRotat, timeStacker);
			Vector2 b = dr * Mathf.Lerp(Mathf.Lerp(-1f, 1f, t), num * chunkRotat.y, 0.5f * Mathf.Abs(chunkRotat.y)) * Mathf.Abs(chunkRotat.y);
			Vector2 a = Vector2.Lerp(prp * ((side != 0) ? 1f : -1f) * chunkRotat.y, prp * Mathf.Pow(Mathf.Abs(f), 0.5f) * Mathf.Sign(f) * Mathf.Abs(chunkRotat.x), Mathf.Abs(chunkRotat.x));
			Vector2 vector = a + b;
			vector = Vector2.Lerp(vector, vector.normalized, Mathf.Pow(Mathf.Abs(chunkRotat.y), 2f));
			Vector2 to = (dr * Mathf.Lerp(1f, -1f, t) + Vector2.Lerp(prp * ((side != 0) ? 1f : -1f) * chunkRotat.y, prp * -chunkRotat.x, Mathf.Abs(chunkRotat.x))) * 0.5f;
			vector = Vector2.Lerp(vector, to, Mathf.Lerp(this.lastWingsFolded, this.wingsFolded, timeStacker));
			return drawPos + vector * this.wingLengths[wing];
		}
		public float VerticalWingFlapAtChunk(int chunk, float timeStacker)
		{
			return Mathf.Sin((Mathf.Lerp(this.lastWingFlapCycle, this.wingFlapCycle, timeStacker) + (float)chunk * 1.8f) * 3.1415927f * 0.3f + ((chunk % 2 != 10) ? 0f : 3.1415927f)) * (1f - Mathf.Lerp(this.lastWingsFolded, this.wingsFolded, timeStacker));
		}
		public float HorizontalWingFlapAtChunk(int chunk, Vector2 chunkRotat, float timeStacker)
		{
			return Mathf.Cos((Mathf.Lerp(this.lastWingFlapCycle, this.wingFlapCycle, timeStacker) + (float)chunk * Custom.LerpMap(chunkRotat.y, -1f, 1f, 1.8f, 0.6f)) * 3.1415927f * 0.3f) * (1f - Mathf.Lerp(this.lastWingsFolded, this.wingsFolded, timeStacker));
		}
		public Vector2 RotatAtChunk(int chunk, float timeStacker)
		{
            return Vector3.Slerp(Vector3.Slerp(this.bodyRotations[0, 1], this.bodyRotations[0, 0], timeStacker), Vector3.Slerp(this.bodyRotations[1, 1], this.bodyRotations[1, 0], timeStacker), Mathf.InverseLerp(0f, (float)(1 / 2), (float)chunk));
		}
	}
}

