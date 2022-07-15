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
		public const int wingLeft1 = 0;
		public const int wingRight1 = 1;

		public float defaultRotat;
		public Vector2[,] bodyRotations;

		public WingWearing(GraphicsModule parent) : base(parent) {
			this.defaultRotat = Mathf.Lerp(-5f, 5f, Random.value);
			this.bodyRotations = new Vector2[3, 2];
			for (int i = 0; i < this.bodyRotations.GetLength(0); i++)
			{
				this.bodyRotations[i, 0] = Custom.DegToVec(this.defaultRotat);
				this.bodyRotations[i, 1] = Custom.DegToVec(this.defaultRotat);
			}
			this.wingLengths = new float[2] { 20f, 20f };
			/*
			for (int j = 0; j < this.wingLengths.Length; j++)
			{
				float num = (float)j / (float)(this.wingLengths.Length - 1);
				float num2 = Mathf.Sin(Mathf.Pow(Mathf.InverseLerp(0.5f, 0f, num), 0.75f) * 3.1415927f);
				num2 *= 1f - num;
				float num3 = Mathf.Sin(Mathf.Pow(Mathf.InverseLerp(1f, 0.5f, num), 0.75f) * 3.1415927f);
				num3 *= num;
				num2 = 0.5f + 0.5f * num2;
				num3 = 0.5f + 0.5f * num3;
				this.wingLengths[j] = Mathf.Lerp(3f, Custom.LerpMap(0.5f, 0.5f, 1f, 60f, 80f), Mathf.Max(num2, num3) - Mathf.Sin(num * 3.1415927f) * 0.25f);
			}
			*/
		}

		public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
		{
			sLeaser.sprites = new FSprite[2];
            sLeaser.sprites[wingLeft1] = new CustomFSprite("CentipedeWing");
            sLeaser.sprites[wingLeft1].shader = rCam.room.game.rainWorld.Shaders["CicadaWing"];
            sLeaser.sprites[wingRight1] = new CustomFSprite("CentipedeWing");
            sLeaser.sprites[wingRight1].shader = rCam.room.game.rainWorld.Shaders["CicadaWing"];

			this.AddToContainer(sLeaser, rCam, null);
		}

		public override void ChildDrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
		{
			this.drawPos += upDir * 6f;
			/*
            for (int j = 0; j < sLeaser.sprites.Length; j++)
            {
				sLeaser.sprites[j].rotation = this.rotation + this.baseRot;
            }
			*/

			// sLeaser.sprites[wingLeft1].SetPosition(drawPos);
			// sLeaser.sprites[wingLeft1].SetPosition(drawPos/5);
			// sLeaser.sprites[wingLeft1].scaleY = (this.flipX ? -1f : 1f);
			// sLeaser.sprites[wingRight1].SetPosition(drawPos/5);
			// sLeaser.sprites[wingRight1].scaleY = (this.flipX ? -1f : 1f);

			for (int k = 0; k < 2; k++)
			{
				int num10 = 0;
				Vector2 chunkDraw = Custom.DirVec(this.ChunkDrawPos(0, timeStacker), this.ChunkDrawPos(1, timeStacker));
				Vector2 vector11 = Custom.PerpendicularVector(chunkDraw);
				Vector2 vector12 = this.RotatAtChunk(num10, timeStacker);
				Vector2 vector13 = this.WingPos(k, num10, chunkDraw, vector11, vector12, timeStacker);
				Vector2 vector14 = this.ChunkDrawPos(num10, timeStacker) + (headRadius + 1) * ((k != 0) ? 1f : -1f) * vector11 * vector12.y;
				Debug.Log("hatworld wing " + k + " chunkDraw " + chunkDraw + " vector13 " + vector13 + " vector14 " + vector14 + " " + (vector13 + chunkDraw * 2f));
				(sLeaser.sprites[k] as CustomFSprite).MoveVertice(1, vector13 + chunkDraw * 2f);
				(sLeaser.sprites[k] as CustomFSprite).MoveVertice(0, vector13 - chunkDraw * 2f);
				(sLeaser.sprites[k] as CustomFSprite).MoveVertice(2, vector14 + chunkDraw * 2f);
				(sLeaser.sprites[k] as CustomFSprite).MoveVertice(3, vector14 - chunkDraw * 2f);

				/*
				// colors
				Vector2 lhs = Custom.DegToVec(Custom.AimFromOneVectorToAnother(vector13, vector14) + Custom.VecToDeg(vector12));
				float num11 = Mathf.InverseLerp(0.85f, 1f, Vector2.Dot(lhs, Custom.DegToVec(45f))) * Mathf.Abs(Vector2.Dot(Custom.DegToVec(45f + Custom.VecToDeg(vector12)), chunkDraw));
				Vector2 lhs2 = Custom.DegToVec(Custom.AimFromOneVectorToAnother(vector14, vector13) + Custom.VecToDeg(vector12));
				float b2 = Mathf.InverseLerp(0.85f, 1f, Vector2.Dot(lhs2, Custom.DegToVec(45f))) * Mathf.Abs(Vector2.Dot(Custom.DegToVec(45f + Custom.VecToDeg(vector12)), -chunkDraw));
				num11 = Mathf.Pow(Mathf.Max(num11, b2), 0.5f);
				(sLeaser.sprites[k] as CustomFSprite).verticeColors[0] = Custom.HSL2RGB(0.99f - 0.4f * Mathf.Pow(num11, 2f), 1f, 0.5f + 0.5f * num11, 0.5f + 0.5f * num11);
				(sLeaser.sprites[k] as CustomFSprite).verticeColors[1] = Custom.HSL2RGB(0.99f - 0.4f * Mathf.Pow(num11, 2f), 1f, 0.5f + 0.5f * num11, 0.5f + 0.5f * num11);
				(sLeaser.sprites[k] as CustomFSprite).verticeColors[2] = Color.Lerp(new Color(this.blackColor.r, this.blackColor.g, this.blackColor.b), new Color(1f, 1f, 1f), 0.5f * num11);
				(sLeaser.sprites[k] as CustomFSprite).verticeColors[3] = Color.Lerp(new Color(this.blackColor.r, this.blackColor.g, this.blackColor.b), new Color(1f, 1f, 1f), 0.5f * num11);
				*/
				Debug.Log("hatworld wing " + k + " position " + sLeaser.sprites[k].GetPosition());
			}
		}

		public override void ChildUpdate(bool eu)
		{
			this.lastWingFlapCycle = this.wingFlapCycle;
			this.wingFlapCycle += Mathf.Pow(this.wingsStartedUp, 3f);
			this.lastWingsFolded = this.wingsFolded;
			this.wingsFolded = 1f - this.wingsStartedUp;
		}

		public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
		{
			for (int k = 0; k < 2; k++) {
                for (int i = 0; i < 4; i++)
                {
                    (sLeaser.sprites[k] as CustomFSprite).verticeColors[i] = new Color(1f, 0, 0);
                }
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
			return this.ChunkDrawPos(wing, timeStacker) + vector * this.wingLengths[wing];
		}
		public Vector2 ChunkDrawPos(int indx, float timeStacker)
		{
			// return Vector2.Lerp(this.centipede.bodyChunks[indx].lastPos, this.centipede.bodyChunks[indx].pos, timeStacker);
			return this.basePos;
		}

		// Token: 0x0600180E RID: 6158 RVA: 0x00136A40 File Offset: 0x00134C40
		public float VerticalWingFlapAtChunk(int chunk, float timeStacker)
		{
			return Mathf.Sin((Mathf.Lerp(this.lastWingFlapCycle, this.wingFlapCycle, timeStacker) + (float)chunk * 1.8f) * 3.1415927f * 0.3f + ((chunk % 2 != 10) ? 0f : 3.1415927f)) * (1f - Mathf.Lerp(this.lastWingsFolded, this.wingsFolded, timeStacker));
		}

		// Token: 0x0600180F RID: 6159 RVA: 0x00136AAC File Offset: 0x00134CAC
		public float HorizontalWingFlapAtChunk(int chunk, Vector2 chunkRotat, float timeStacker)
		{
			return Mathf.Cos((Mathf.Lerp(this.lastWingFlapCycle, this.wingFlapCycle, timeStacker) + (float)chunk * Custom.LerpMap(chunkRotat.y, -1f, 1f, 1.8f, 0.6f)) * 3.1415927f * 0.3f) * (1f - Mathf.Lerp(this.lastWingsFolded, this.wingsFolded, timeStacker));
		}
		public Vector2 RotatAtChunk(int chunk, float timeStacker)
		{
            return Vector3.Slerp(Vector3.Slerp(this.bodyRotations[0, 1], this.bodyRotations[0, 0], timeStacker), Vector3.Slerp(this.bodyRotations[1, 1], this.bodyRotations[1, 0], timeStacker), Mathf.InverseLerp(0f, (float)(1 / 2), (float)chunk));
			/*
			if (chunk <= this.centipede.bodyChunks.Length / 2)
			{
				return Vector3.Slerp(Vector3.Slerp(this.bodyRotations[0, 1], this.bodyRotations[0, 0], timeStacker), Vector3.Slerp(this.bodyRotations[1, 1], this.bodyRotations[1, 0], timeStacker), Mathf.InverseLerp(0f, (float)(this.centipede.bodyChunks.Length / 2), (float)chunk));
			}
			return Vector3.Slerp(Vector3.Slerp(this.bodyRotations[1, 1], this.bodyRotations[1, 0], timeStacker), Vector3.Slerp(this.bodyRotations[2, 1], this.bodyRotations[2, 0], timeStacker), Mathf.InverseLerp((float)(this.centipede.bodyChunks.Length / 2), (float)(this.centipede.bodyChunks.Length - 1), (float)chunk));
			*/
		}
	}
}

