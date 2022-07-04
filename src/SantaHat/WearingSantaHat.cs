using RWCustom;
using UnityEngine;

namespace HatWorld
{
	// Hat that is being worn by slugcat
	// Uses code from FestiveWorld mod
	sealed class WearingSantaHat : WearingHat
	{
		public Vector2 tuftPos;
		public Vector2 lastTuftPos;
		public Vector2 tuftVel;
		public override HatType hatType => HatType.Santa;

		public WearingSantaHat(GraphicsModule parent, int anchorSprite, float rotation, float headRadius)
			: base(parent, anchorSprite, rotation, headRadius) {}

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
			TriangleMesh cone = new TriangleMesh("Futile_White", array, false, false);
			sLeaser.sprites[0] = cone;
			sLeaser.sprites[1] = new FSprite("JetFishEyeA", true);
			sLeaser.sprites[2] = new FSprite("LizardScaleA6", true);
			this.AddToContainer(sLeaser, rCam, null);
		}

		public override void ChildDrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
		{
			Vector2 targetTuftPos = drawPos + upDir * 20f;

			// Rim
			sLeaser.sprites[2].SetPosition(drawPos);
			sLeaser.sprites[2].rotation = this.rotation + this.baseRot;
			sLeaser.sprites[2].scaleY = (this.flipX ? -1f : 1f);

			// Tuft
			if (!Custom.DistLess(this.tuftPos, targetTuftPos, 20f))
			{
				this.tuftPos = targetTuftPos + (this.tuftPos - targetTuftPos).normalized * 20f;
				if (!Custom.DistLess(this.lastTuftPos, this.tuftPos, 20f))
				{
					this.lastTuftPos = this.tuftPos + (this.lastTuftPos - this.tuftPos).normalized * 20f;
				}
			}
			Vector2 tuftLocation = Vector2.Lerp(this.lastTuftPos, this.tuftPos, timeStacker);
			sLeaser.sprites[1].SetPosition(tuftLocation);

            // Cone
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
		}

		public override void ChildUpdate(bool eu)
		{
			this.lastTuftPos = this.tuftPos;

            Vector2 drawPos = this.basePos;
            Vector2 upDir = new Vector2(Mathf.Cos((this.rotation + this.baseRot) * -0.017453292f), Mathf.Sin((this.rotation + this.baseRot) * -0.017453292f));
            Vector2 rightDir = -Custom.PerpendicularVector(upDir);
            if (this.flipY)
            {
                upDir *= -1f;
            }
            if (this.flipX)
            {
                rightDir *= -1f;
            }
            drawPos += upDir * 20f;

            this.tuftVel.y = this.tuftVel.y - this.parent.owner.gravity;
            this.tuftVel += rightDir * ((Vector2.Dot(rightDir, this.tuftPos - drawPos) > 0f) ? 1.5f : -1.5f);
            this.tuftVel += (drawPos - this.tuftPos) * 0.2f;
            this.tuftVel *= 0.6f;
            this.tuftPos += this.tuftVel;
            if (!Custom.DistLess(this.tuftPos, drawPos, 13f))
            {
                this.tuftPos = drawPos + (this.tuftPos - drawPos).normalized * 13f;
            }
		}

		public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
		{
			sLeaser.sprites[0].color = Color.red;
			sLeaser.sprites[1].color = Color.white;
			sLeaser.sprites[2].color = Color.white;
		}
	}
}
