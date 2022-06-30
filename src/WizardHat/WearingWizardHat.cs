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

        // Constants for sLeaser sprite index (higher index appears over lower)
        public int coneIndex = 0;
        public int tuftIndex = 1;
        public int beltIndex = 2;
        public int botIndex = 3;

		public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
		{
			sLeaser.sprites = new FSprite[4];
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
			TriangleMesh cone = new TriangleMesh("Futile_White", array, false, false);
			sLeaser.sprites[coneIndex] = cone;
			sLeaser.sprites[tuftIndex] = new FSprite("mouseEyeA5", true);
			sLeaser.sprites[beltIndex] = new FSprite("LizardScaleA6", true);
			sLeaser.sprites[beltIndex].scaleY = 0.8f;
			sLeaser.sprites[botIndex] = new FSprite("SpearFragment2", true);
			sLeaser.sprites[botIndex].scaleY = 1.6f;
			sLeaser.sprites[botIndex].scaleX = 1.7f;
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
			Vector2 drawPos = new Vector2(this.basePos.x, this.basePos.y + 2); // increase y so hat doesn't go over eyes

			Vector2 upDir = new Vector2(Mathf.Cos((this.rotation + this.baseRot) * -0.017453292f), Mathf.Sin((this.rotation + this.baseRot) * -0.017453292f));
			Vector2 rightDir = -Custom.PerpendicularVector(upDir);
            if (flipY) upDir *= -1;
            if (flipX) rightDir *= -1;
			drawPos += upDir * this.headRadius;

			/* Brim */
			sLeaser.sprites[botIndex].SetPosition(drawPos);
			sLeaser.sprites[botIndex].rotation = this.rotation + this.baseRot;
			sLeaser.sprites[botIndex].scaleY = (this.flipX ? -1.6f : 1.6f);

			/* Tuft */
			const float TUFTNUM = 25f; // some combination of height and stretch, changing it too much ruins the tuft bobble
			Vector2 targetTuftPos = drawPos + upDir * (TUFTNUM + 10);
			if (!Custom.DistLess(this.tuftPos, targetTuftPos, TUFTNUM))
			{
				this.tuftPos = targetTuftPos + (this.tuftPos - targetTuftPos).normalized * TUFTNUM;
				if (!Custom.DistLess(this.lastTuftPos, this.tuftPos, TUFTNUM))
				{
					this.lastTuftPos = this.tuftPos + (this.lastTuftPos - this.tuftPos).normalized * TUFTNUM;
				}
			}
			Vector2 tuftLocation = Vector2.Lerp(this.lastTuftPos, this.tuftPos, timeStacker);
			sLeaser.sprites[tuftIndex].SetPosition(tuftLocation);

            /* Belt */
            Vector2 beltLocation = sLeaser.sprites[botIndex].GetPosition() + upDir * 3;
			sLeaser.sprites[beltIndex].rotation = this.rotation + this.baseRot;
			sLeaser.sprites[beltIndex].scaleY = (this.flipX ? -0.8f : 0.8f);
            sLeaser.sprites[beltIndex].SetPosition(beltLocation);

            /* Cone (triangle) */
            TriangleMesh cone = (TriangleMesh)sLeaser.sprites[0];
            Vector2 coneTip = Vector2.Lerp(lastTuftPos, tuftPos, timeStacker);
            for (int i = 0, len = cone.vertices.Length; i < len; i++)
            {
                bool r = i % 2 == 1;
                float h = i / 2 / (float)(len - 1) * 2f;

                Vector2 coneBase;
                if (r)
                    coneBase = drawPos - rightDir * 7f;
                else
                    coneBase = drawPos + rightDir * 7f;
                Vector2 coneMid = Vector2.Lerp(coneBase, targetTuftPos, 0.5f);

                Vector2 pos = Vector2.Lerp(Vector2.Lerp(coneBase, coneMid, h), Vector2.Lerp(coneMid, coneTip, h), h);
                cone.MoveVertice(i, pos);
            }


			if (this.parent.culled && !this.parent.lastCulled)
			{
				for (int j = 0; j < 3; j++)
				{
					sLeaser.sprites[coneIndex].isVisible = !this.parent.culled;
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
				Vector2 rightDir = -Custom.PerpendicularVector(vector2);
				if (this.flipY)
				{
					vector2 *= -1f;
				}
				if (this.flipX)
				{
					rightDir *= -1f;
				}
				vector += vector2 * 20f;
				this.tuftVel.y = this.tuftVel.y - this.parent.owner.gravity;
				this.tuftVel += rightDir * ((Vector2.Dot(rightDir, this.tuftPos - vector) > 0f) ? 1.5f : -1.5f);
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
			sLeaser.sprites[coneIndex].color = new Color(0.35f, 0.4f, 0.8f); // blue

			sLeaser.sprites[tuftIndex].color = new Color(1f, 0.80f, 0.49f); // orange yellow
            // sLeaser.sprites[tuftIndex].color = new Color(0.8f, 0.8f, 0.9f); // silver blue
			// sLeaser.sprites[tuftIndex].color = new Color(1f, 0.96f, 0.55f); // light yellow

			sLeaser.sprites[beltIndex].color = sLeaser.sprites[tuftIndex].color;
            sLeaser.sprites[botIndex].color = sLeaser.sprites[coneIndex].color;
		}
	}
}

