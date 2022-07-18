using UnityEngine;
using RWCustom;

namespace HatWorld
{
    sealed class WizardPhysical : HatPhysical
    {
        // taken from FestiveWorld SantaHat
        public Vector2 tuftPos;
        public Vector2 lastTuftPos;
		public Vector2 tuftVel;

        // Constants for sLeaser sprite index (higher index appears over lower)
        public const int coneIndex = 0;
        public const int tuftIndex = 1;
        public const int beltIndex = 2;
        public const int botIndex = 3;

        public override HatWearing GetWornHat(GraphicsModule graphicsModule)
        {
            return new WizardWearing(graphicsModule);
        }
        public WizardPhysical(HatAbstract abstr, World world) : base(abstr, world) { }

        public override void Update(bool eu)
        {
            base.Update(eu);

            // taken from FestiveWorld SantaHat (with changes)
            this.lastTuftPos = this.tuftPos;

            if (this.room != null)
            {
                /*
                float rotationFloat = Custom.VecToDeg(this.rotation);
                Vector2 upDir = new Vector2(Mathf.Cos((rotationFloat) * -0.017453292f), Mathf.Sin((rotationFloat) * -0.017453292f));
                Vector2 rightDir = -Custom.PerpendicularVector(upDir);
                */
                Vector2 upDir = new Vector2(0, 1);
                Vector2 rightDir = new Vector2(1, 0);

                Vector2 tipPos = this.tuftPos;
                tipPos += upDir * 2f;
				this.tuftVel.y -= this.gravity;
				this.tuftVel += rightDir * ((Vector2.Dot(rightDir, this.tuftPos - tipPos) > 0f) ? 1f : -1f);
				this.tuftVel += (tipPos - this.tuftPos) * 0.2f;
				this.tuftVel *= 0.8f;
				this.tuftPos += this.tuftVel;
				if (!Custom.DistLess(this.tuftPos, tipPos, 6f))
				{
					this.tuftPos = tipPos + (this.tuftPos - tipPos).normalized * 6f;
				}
			}
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            // Taken from FestiveWorld SantaHat
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

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            // Setup
            base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
            for (int j = 0; j < sLeaser.sprites.Length; j++)
            {
                if (j != coneIndex) // the triangle vertices are set later on in this method, moving them twice puts them in the wrong place
                {
                    sLeaser.sprites[j].SetPosition(drawPos);
                    sLeaser.sprites[j].rotation = hatRotation;
                }
            }

			/* Tuft */
			const float TUFTNUM = 20f; // some combination of height and stretch, changing it too much ruins the tuft bobble
			Vector2 targetTuftPos = drawPos + upDir * (TUFTNUM + 4);
            targetTuftPos += new Vector2(2, 0);
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
            Vector2 beltLocation = drawPos + upDir * 3;
            sLeaser.sprites[beltIndex].SetPosition(beltLocation);

            /* Cone */
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

                Vector2 verticePos = Vector2.Lerp(Vector2.Lerp(coneBase, coneMid, h), Vector2.Lerp(coneMid, coneTip, h), h);
                cone.MoveVertice(i, verticePos);
            }

        }

        public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            //blackColor = palette.blackColor;
            //earthColor = Color.Lerp(palette.fogColor, palette.blackColor, 0.5f);

			sLeaser.sprites[coneIndex].color = new Color(0.35f, 0.4f, 0.8f); // blue

			sLeaser.sprites[tuftIndex].color = new Color(1f, 0.80f, 0.49f); // orange yellow
            // sLeaser.sprites[tuftIndex].color = new Color(0.8f, 0.8f, 0.9f); // silver blue
			// sLeaser.sprites[tuftIndex].color = new Color(1f, 0.96f, 0.55f); // light yellow

            sLeaser.sprites[beltIndex].color = sLeaser.sprites[tuftIndex].color;
            sLeaser.sprites[botIndex].color = sLeaser.sprites[coneIndex].color;
        }
    }
}

