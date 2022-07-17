using System;
using RWCustom;
using SplashWater;
using UnityEngine;

namespace HatWorld
{
	public class JetWaterCustom : WaterParticleCustom
	{
		public WaterJet jet;
		public JetWaterCustom op;

		public Vector2 lingerPos;
		public Vector2 lastLingerPos;
		public Vector2 lingerPosVel;
		public Vector2 lingerOtherDir;
		public float lingerOtherRad;
		public float lingerOpacity;
		public float lastLingerOpacity;

		public bool goToRest;
		public int killCounter;

		// Token: 0x06000AA7 RID: 2727 RVA: 0x00069EBC File Offset: 0x000680BC
		public JetWaterCustom(WaterJet jet) { }

		// Token: 0x17000175 RID: 373
		// (get) Token: 0x06000AA8 RID: 2728 RVA: 0x00069EC4 File Offset: 0x000680C4
		// (set) Token: 0x06000AA9 RID: 2729 RVA: 0x00069EF9 File Offset: 0x000680F9
		public JetWaterCustom otherParticle
		{
			get
			{
				if (this.op != null && this.op.goToRest)
				{
					this.op = null;
				}
				return this.op;
			}
			set
			{
				this.op = value;
			}
		}

		// Token: 0x06000AAA RID: 2730 RVA: 0x00069F04 File Offset: 0x00068104
		public override void Reset(Vector2 pos, Vector2 vel, float amount, float initRad)
		{
			base.Reset(pos, vel, amount, initRad);
			this.lingerPos = pos;
			this.lastLingerPos = this.lingerPos;
			this.lingerPosVel = vel;
			this.lingerOtherDir = vel.normalized;
			this.lingerOtherRad = initRad;
			this.lingerOpacity = 1f;
			this.lastLingerOpacity = 1f;
			this.killCounter = 0;
			this.goToRest = false;
		}

		// Token: 0x06000AAB RID: 2731 RVA: 0x00069F70 File Offset: 0x00068170
		public override void Update(bool eu)
		{
			if (this.lastLife < 0f)
			{
				this.goToRest = true;
				this.lastLingerPos = this.lingerPos;
				this.lastPos = this.pos;
				this.lastLife = this.life;
				this.lastRad = this.rad;
				this.otherParticle = null;
			}
			if (this.goToRest)
			{
				this.killCounter++;
				if (this.killCounter == 100)
				{
					this.Destroy();
				}
				return;
			}
			if (this.room.PointSubmerged(this.pos))
			{
				this.life -= 0.02f;
				this.room.waterObject.WaterfallHitSurface(this.pos.x - 5f, this.pos.x + 5f, 0.01f); // Mathf.InverseLerp(-17f, -20f, this.vel.y));
				this.vel.y = Mathf.Abs(this.vel.y) * 0.4f;
				this.rad = (this.rad + 2f) * 1.5f;
				/*
				if (this.makeSoundCounter <= 0 && this.vel.magnitude > 4f)
				{
					this.room.PlaySound(SoundID.Splashing_Water_Into_Water_Surface, this.pos, Mathf.InverseLerp(4f, 14f, this.vel.magnitude), 1f);
					this.makeSoundCounter = int.MaxValue;
				}
				*/
			}
			else
			{
				this.vel.y = this.vel.y - 0.9f;
			}
			base.Update(eu);
			this.lastLingerPos = this.lingerPos;
			if (this.otherParticle != null)
			{
				Vector2 to = Vector2.Lerp(this.vel, this.otherParticle.vel, 0.5f);
				this.vel = Vector2.Lerp(this.vel, to, 0.1f);
				this.otherParticle.vel = Vector2.Lerp(this.otherParticle.vel, to, 0.1f);
				this.lastLingerPos = this.otherParticle.lastPos;
				this.lingerPos = this.otherParticle.pos;
				this.lingerPosVel = this.otherParticle.vel;
				this.lastLingerOpacity = this.otherParticle.Opactiy(0f);
				this.lingerOpacity = this.otherParticle.Opactiy(1f);
				if (UnityEngine.Random.value < 0.025f)
				{
					this.otherParticle = null;
				}
			}
			else
			{
				this.lingerPos += this.lingerPosVel;
				if (this.room.PointSubmerged(this.lingerPos))
				{
					this.lingerPos.y = this.room.FloatWaterLevel(this.lingerPos.x);
				}
				this.lingerPosVel.y = this.lingerPosVel.y - 0.9f;
				this.lastLingerOpacity = this.lingerOpacity;
				this.lingerOpacity = Mathf.Max(this.lingerOpacity - 0.025f, 0f);
			}
		}

		// Token: 0x06000AAC RID: 2732 RVA: 0x0006A29C File Offset: 0x0006849C
		public Vector2 OtherPos(float timeStacker)
		{
			if (this.otherParticle != null)
			{
				this.lastLingerPos = this.otherParticle.lastPos;
				this.lingerPos = this.otherParticle.pos;
				this.lingerPosVel = this.otherParticle.vel;
			}
			return Vector2.Lerp(this.lastLingerPos, this.lingerPos, timeStacker);
		}

		// Token: 0x06000AAD RID: 2733 RVA: 0x0006A2FC File Offset: 0x000684FC
		public Vector2 OtherDir(float timeStacker)
		{
			if (this.otherParticle != null)
			{
				this.lingerOtherDir = Custom.DirVec(Vector2.Lerp(this.otherParticle.lastPos, this.otherParticle.pos, timeStacker), this.otherParticle.OtherPos(timeStacker));
			}
			return this.lingerOtherDir;
		}

		// Token: 0x06000AAE RID: 2734 RVA: 0x0006A34D File Offset: 0x0006854D
		public float Rad(float timeStacker)
		{
			return Mathf.Lerp(this.lastRad, this.rad, timeStacker);
		}

		// Token: 0x06000AAF RID: 2735 RVA: 0x0006A361 File Offset: 0x00068561
		public float OtherRad(float timeStacker)
		{
			if (this.otherParticle != null)
			{
				this.lingerOtherRad = this.otherParticle.Rad(timeStacker);
			}
			return this.lingerOtherRad;
		}

		// Handles opacity of water particles - how fast they fade away
		public float Opactiy(float timeStacker)
		{
			if (this.goToRest)
			{
				return 0f;
			}
			return Mathf.InverseLerp(this.amount * 2.27f, this.amount / 2f, Mathf.Lerp(this.lastRad, this.rad, timeStacker)) * Mathf.Pow(1f - Mathf.Lerp(this.lastLife, this.life, timeStacker), 0.1f) / Mathf.Lerp(Mathf.Max(1f, Vector2.Distance(Vector2.Lerp(this.lastPos, this.pos, timeStacker), this.OtherPos(timeStacker)) - 40f), 1f, 0.95f);
		}

		// Token: 0x06000AB1 RID: 2737 RVA: 0x0006A438 File Offset: 0x00068638
		public float OtherOpacity(float timeStacker)
		{
			if (this.goToRest)
			{
				return 0f;
			}
			if (this.otherParticle != null)
			{
				this.lingerOpacity = this.otherParticle.Opactiy(timeStacker);
				this.lastLingerOpacity = this.lingerOpacity;
				return this.lingerOpacity;
			}
			return Mathf.Lerp(this.lastLingerOpacity, this.lingerOpacity, timeStacker);
		}

		// Token: 0x06000AB2 RID: 2738 RVA: 0x0006A498 File Offset: 0x00068698
		public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
		{
			sLeaser.sprites = new FSprite[2];
			for (int i = 0; i < 2; i++)
			{
				TriangleMesh.Triangle[] tris = new TriangleMesh.Triangle[]
				{
					new TriangleMesh.Triangle(0, 1, 2),
					new TriangleMesh.Triangle(1, 2, 3),
					new TriangleMesh.Triangle(2, 3, 4),
					new TriangleMesh.Triangle(3, 4, 5)
				};
				sLeaser.sprites[i] = new TriangleMesh("Futile_White", tris, true, false);
				(sLeaser.sprites[i] as TriangleMesh).UVvertices[0] = new Vector2(0f, 0f);
				(sLeaser.sprites[i] as TriangleMesh).UVvertices[1] = new Vector2(1f, 0f);
				(sLeaser.sprites[i] as TriangleMesh).UVvertices[2] = new Vector2(0f, 0.5f);
				(sLeaser.sprites[i] as TriangleMesh).UVvertices[3] = new Vector2(1f, 0.5f);
				(sLeaser.sprites[i] as TriangleMesh).UVvertices[4] = new Vector2(0f, 1f);
				(sLeaser.sprites[i] as TriangleMesh).UVvertices[5] = new Vector2(1f, 1f);
				sLeaser.sprites[i].shader = rCam.room.game.rainWorld.Shaders["WaterSplash"];
			}
			this.AddToContainer(sLeaser, rCam, null);
		}

		// Token: 0x06000AB3 RID: 2739 RVA: 0x0006A66C File Offset: 0x0006886C
		public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
		{
			Vector2 vector = Vector2.Lerp(this.lastPos, this.pos, timeStacker);
			Vector2 vector2 = this.OtherPos(timeStacker);
			Vector2 vector3 = Custom.DirVec(vector, vector2);
			Vector2 a = Custom.PerpendicularVector(vector3);
			float num = this.Rad(timeStacker);
			float num2 = this.OtherRad(timeStacker);
			Vector2 a2 = Custom.PerpendicularVector(this.OtherDir(timeStacker));
			for (int i = 0; i < 2; i++)
			{
				Vector2 b = new Vector2(-1.5f, 1.5f) * (float)i;
				(sLeaser.sprites[i] as TriangleMesh).MoveVertice(0, vector - a * num - camPos + b);
				(sLeaser.sprites[i] as TriangleMesh).MoveVertice(1, vector + a * num - camPos + b);
				(sLeaser.sprites[i] as TriangleMesh).MoveVertice(2, vector - a * (num + num2) * 0.5f + vector3 * Vector2.Distance(vector, vector2) * 0.5f - camPos + b);
				(sLeaser.sprites[i] as TriangleMesh).MoveVertice(3, vector + a * (num + num2) * 0.5f + vector3 * Vector2.Distance(vector, vector2) * 0.5f - camPos + b);
				(sLeaser.sprites[i] as TriangleMesh).MoveVertice(4, vector2 - a2 * num2 - camPos + b);
				(sLeaser.sprites[i] as TriangleMesh).MoveVertice(5, vector2 + a2 * num2 - camPos + b);
				for (int j = 0; j < 4; j++)
				{
					(sLeaser.sprites[i] as TriangleMesh).verticeColors[j] = new Color(1f, 1f, (float)i, this.Opactiy(timeStacker));
				}
				for (int k = 4; k < 6; k++)
				{
                    (sLeaser.sprites[i] as TriangleMesh).verticeColors[k] = new Color(1f, 1f, (float)i, this.OtherOpacity(timeStacker));
				}
			}
			base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
		}

		// Token: 0x06000AB4 RID: 2740 RVA: 0x0006A910 File Offset: 0x00068B10
		public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
		{
			rCam.ReturnFContainer("Water").AddChild(sLeaser.sprites[0]);
			rCam.ReturnFContainer("GrabShaders").AddChild(sLeaser.sprites[1]);
		}
    }
}
