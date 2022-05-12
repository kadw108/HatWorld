using RWCustom;
using UnityEngine;

namespace HatWorld
{
	// Hat that is being worn by slugcat
	// Uses code from FestiveWorld mod
	sealed class WearingWizardHat : WearingHat
	{
		public GraphicsModule parent;

		public int anchorSprite;

		public float rotation;
		public float headRadius;

		public bool flipX;
		public bool flipY;

		public Vector2 basePos;
		public float baseRot;

		public Vector2 tuftPos;
		public Vector2 lastTuftPos;
		public Vector2 tuftVel;

		public override HatType hatType => HatType.Wizard;

		public WearingWizardHat(GraphicsModule parent, int anchorSprite, float rotation, float headRadius)
		{
			this.parent = parent;
			this.anchorSprite = anchorSprite;
			this.rotation = rotation;
			this.headRadius = headRadius;
			parent.owner.room.AddObject(this);
		}

		public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
		{
			sLeaser.sprites = new FSprite[3];
			TriangleMesh.Triangle[] array = new TriangleMesh.Triangle[]
			{
				new TriangleMesh.Triangle(0, 1, 2),
				new TriangleMesh.Triangle(1, 2, 3),
				new TriangleMesh.Triangle(2, 3, 4),
				new TriangleMesh.Triangle(3, 4, 5),
				new TriangleMesh.Triangle(4, 5, 6),
				new TriangleMesh.Triangle(5, 6, 7),
				new TriangleMesh.Triangle(6, 7, 8)
			};
			TriangleMesh triangleMesh = new TriangleMesh("Futile_White", array, false, false);
			sLeaser.sprites[0] = triangleMesh;
			sLeaser.sprites[1] = new FSprite("JetFishEyeA", true);
			sLeaser.sprites[2] = new FSprite("LizardScaleA6", true);
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
			Vector2 basePosVector = this.basePos;
			Vector2 tuftBobble = new Vector2(Mathf.Cos((this.rotation + this.baseRot) * -0.017453292f), Mathf.Sin((this.rotation + this.baseRot) * -0.017453292f));
			Vector2 vector3 = -Custom.PerpendicularVector(tuftBobble);
			if (this.flipY)
			{
				tuftBobble *= -1f;
			}
			if (this.flipX)
			{
				vector3 *= -1f;
			}
			basePosVector += tuftBobble * this.headRadius;
			Vector2 vector4 = basePosVector + tuftBobble * 20f;
			sLeaser.sprites[2].SetPosition(basePosVector);
			sLeaser.sprites[2].rotation = this.rotation + this.baseRot;
			sLeaser.sprites[2].scaleY = (this.flipX ? -1f : 1f);
			if (!Custom.DistLess(this.tuftPos, vector4, 20f))
			{
				this.tuftPos = vector4 + (this.tuftPos - vector4).normalized * 20f;
				if (!Custom.DistLess(this.lastTuftPos, this.tuftPos, 20f))
				{
					this.lastTuftPos = this.tuftPos + (this.lastTuftPos - this.tuftPos).normalized * 20f;
				}
			}
			Vector2 tuftLocation = Vector2.Lerp(this.lastTuftPos, this.tuftPos, timeStacker);
			sLeaser.sprites[1].SetPosition(tuftLocation);

			/* Triangle code */
			TriangleMesh triangleMesh = (TriangleMesh)sLeaser.sprites[0];
			Vector2 vector6 = basePosVector - vector3 * 7f;
			Vector2 vector7 = basePosVector + vector3 * 7f;
			Vector2 vector8 = Vector2.Lerp(vector6, vector4, 0.5f);
			Vector2 vector9 = Vector2.Lerp(vector7, vector4, 0.5f);
			int i = 0;
			int num = triangleMesh.vertices.Length;
			while (i < num)
			{
				bool flag = i % 2 == 1;
				float num2 = (float)(i / 2) / (float)(num - 1) * 2f;
				Vector2 vector10 = Vector2.Lerp(Vector2.Lerp(flag ? vector7 : vector6, flag ? vector9 : vector8, num2), Vector2.Lerp(flag ? vector9 : vector8, Vector2.Lerp(this.lastTuftPos, this.tuftPos, timeStacker), num2), num2);
				triangleMesh.MoveVertice(i, vector10);
				i++;
			}

			if (this.parent.culled && !this.parent.lastCulled)
			{
				for (int j = 0; j < 3; j++)
				{
					sLeaser.sprites[0].isVisible = !this.parent.culled;
				}
			}
			if (base.slatedForDeletetion || rCam.room != this.room || this.room != this.parent.owner.room)
			{
				sLeaser.CleanSpritesAndRemove();
			}
		}

		public override void Update(bool eu)
		{
			base.Update(eu);
			this.lastTuftPos = this.tuftPos;
			GraphicsModule graphicsModule = this.parent;
			if (((graphicsModule != null) ? graphicsModule.owner : null) == null || this.parent.owner.slatedForDeletetion || base.slatedForDeletetion)
			{
				this.Destroy();
			}
			else if (this.parent.owner.room != null)
			{
				Vector2 vector = this.basePos;
				Vector2 vector2 = new Vector2(Mathf.Cos((this.rotation + this.baseRot) * -0.017453292f), Mathf.Sin((this.rotation + this.baseRot) * -0.017453292f));
				Vector2 vector3 = -Custom.PerpendicularVector(vector2);
				if (this.flipY)
				{
					vector2 *= -1f;
				}
				if (this.flipX)
				{
					vector3 *= -1f;
				}
				vector += vector2 * 20f;
				this.tuftVel.y = this.tuftVel.y - this.parent.owner.gravity;
				this.tuftVel += vector3 * ((Vector2.Dot(vector3, this.tuftPos - vector) > 0f) ? 1.5f : -1.5f);
				this.tuftVel += (vector - this.tuftPos) * 0.2f;
				this.tuftVel *= 0.6f;
				this.tuftPos += this.tuftVel;
				if (!Custom.DistLess(this.tuftPos, vector, 13f))
				{
					this.tuftPos = vector + (this.tuftPos - vector).normalized * 13f;
				}
			}
			if (base.slatedForDeletetion)
			{
				base.RemoveFromRoom();
			}
		}

		/*
		public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
		{
			if (newContatiner == null)
			{
				newContatiner = rCam.ReturnFContainer("Items");
			}
			for (int i = 0; i < sLeaser.sprites.Length; i++)
			{
				sLeaser.sprites[i].RemoveFromContainer();
			}
			for (int j = 0; j < sLeaser.sprites.Length; j++)
			{
				newContatiner.AddChild(sLeaser.sprites[j]);
			}
		}
		*/

		public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
		{
			sLeaser.sprites[0].color = Color.blue;
			sLeaser.sprites[1].color = Color.yellow;
			sLeaser.sprites[2].color = Color.yellow;
		}
	}
}

