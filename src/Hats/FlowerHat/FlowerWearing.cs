using RWCustom;
using UnityEngine;

namespace HatWorld
{
	sealed class FlowerWearing : HatWearing
	{
		// Constants for sLeaser sprite index (higher index appears over lower)
		public const int vineIndex = 0;
		public const int vineNumber = 4;
		public const int petal3 = vineIndex + vineNumber + 0;
		public const int leaf1 = vineIndex + vineNumber + 1;
		public const int leaf2 = vineIndex + vineNumber + 2;
		public const int petal1 = vineIndex + vineNumber + 3;
		public const int petal2 = vineIndex + vineNumber + 4;
		public const int petal4 = vineIndex + vineNumber + 5;

		/* Vines from Jellyfish.tentacles
		 * Array of 2D arrays of vectors
		 * Each 2D array is one vine, each row is 1 vertice
		 * columns are start pos [i, 0] (column 0) / end pos [i, 1] (column 1) / length [i, 2] (column 2)
		 * eg. vines[j][i, 0] is the jth vine, ith vertice, start pos
		 */
		public Vector2[][,] vines;

		public float vinesWithdrawn;
        public Vector2 lastCamPos; // for AttachPos and ResetVines

        public bool initializeVines;

		public FlowerWearing(GraphicsModule parent) : base(parent) {
            initializeVines = false;
		}

		public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
		{
            // initialization
            this.lastCamPos = rCam.pos;
            this.vines = new Vector2[vineNumber][,]; // must be in InitiateSprites, not constructor, or FlowerWearing doesn't appear
            for (int i = 0; i < this.vines.Length; i++)
            {
                // one long vine (length 5) and short vine (length 4) per side
                if (i % 2 == 0)
                {
                    this.vines[i] = new Vector2[5, 3];
                }
                else
                {
                    this.vines[i] = new Vector2[4, 3];
                }
            }

            // define sprites
            sLeaser.sprites = new FSprite[this.vines.Length + 6];
            sLeaser.sprites[leaf1] = new FSprite("haloGlyph4", true) { scale = 0.8f };
            sLeaser.sprites[leaf2] = new FSprite("haloGlyph5", true) { scale = 0.8f };

            sLeaser.sprites[petal1] = new FSprite("FaceA1", true) { scale = 0.8f };
            sLeaser.sprites[petal2] = new FSprite("Pebble5", true) { scale = 0.6f };
            sLeaser.sprites[petal3] = new FSprite("FaceA1", true) { scale = 0.8f };
            sLeaser.sprites[petal4] = new FSprite("FaceA1", true) { scale = 0.8f };

            // vines
            for (int i = 0; i < this.vines.Length; i++)
            {
                sLeaser.sprites[vineIndex + i] = TriangleMesh.MakeLongMesh(this.vines[i].GetLength(0), false, true);
                sLeaser.sprites[vineIndex + i].isVisible = false;
            }

            this.AddToContainer(sLeaser, rCam, null);
		}

		public override void ChildDrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
		{
            lastCamPos = camPos;

            if (this.initialized && !this.initializeVines)
            {
                // Vine positions must be initialized/reset after basePos is set in ParentDrawSprites (this.initialized == true)
                this.ResetVines();
                this.initializeVines = true;
            }

            for (int j = 0; j < sLeaser.sprites.Length; j++)
            {
                if (j < vineIndex || j >= vineIndex + vineNumber) // don't change vines, it ruins vine position code
                {
                    sLeaser.sprites[j].rotation = this.rotation; // + this.baseRot not needed
                }
            }

            Vector2 upDir = new(0, 1);
            Vector2 rightDir = new(1, 0);
            drawPos += upDir * 3;

            /* leaves */
            sLeaser.sprites[leaf1].SetPosition(drawPos - rightDir * 3);
            sLeaser.sprites[leaf2].SetPosition(drawPos + rightDir * 3);
            sLeaser.sprites[petal2].SetPosition(drawPos - rightDir * 3);

            sLeaser.sprites[leaf1].rotation += 90f;
            sLeaser.sprites[leaf2].rotation += 90f;

            /* flowers */
            sLeaser.sprites[petal1].SetPosition(drawPos + upDir * 1f + rightDir * 3f);
            sLeaser.sprites[petal1].rotation -= 20f;

            sLeaser.sprites[petal4].SetPosition(drawPos);
            sLeaser.sprites[petal4].rotation += 30f;

            sLeaser.sprites[petal3].SetPosition(drawPos - upDir * 1f - rightDir * 2f);
            sLeaser.sprites[petal3].rotation += 90f;


            /* vines */
            for (int j = 0; j < this.vines.Length; j++)
            {
                // j is index for this.vines, vineIndex + j is index for sLeaser.sprites
                float num = 0f;

                Vector2 a = this.AttachPos(j, timeStacker, camPos);

                // Debug.Log("hatworld wearingvines camera " + camPos + " " + lastCamPos + " pos " + a + " " + drawPos + " " + (a - camPos) + " " + basePos);
                // Debug.Log("hatworld wearingvines vines " + vines[0][0, 0] + " " + vines[0][0, 1] + vines[0][0, 2]);

                for (int k = 0; k < this.vines[j].GetLength(0); k++)
                {
                    Vector2 vector3 = Vector2.Lerp(this.vines[j][k, 1], this.vines[j][k, 0], timeStacker);
                    float num2 = 0.5f;
                    Vector2 normalized = (a - vector3).normalized;
                    Vector2 a2 = Custom.PerpendicularVector(normalized);
                    float d = Vector2.Distance(a, vector3) / 5f;

                    (sLeaser.sprites[vineIndex + j] as TriangleMesh).MoveVertice(k * 4, a - normalized * d - a2 * (num2 + num) * 0.5f - camPos);
                    (sLeaser.sprites[vineIndex + j] as TriangleMesh).MoveVertice(k * 4 + 1, a - normalized * d + a2 * (num2 + num) * 0.5f - camPos);
                    (sLeaser.sprites[vineIndex + j] as TriangleMesh).MoveVertice(k * 4 + 2, vector3 + normalized * d - a2 * num2 - camPos);
                    (sLeaser.sprites[vineIndex + j] as TriangleMesh).MoveVertice(k * 4 + 3, vector3 + normalized * d + a2 * num2 - camPos);
                    sLeaser.sprites[vineIndex + j].isVisible = true;

                    a = vector3;
                    num = num2;
                }
            }
		}

		public override void ChildUpdate(bool eu)
		{
            this.vinesWithdrawn = Custom.LerpAndTick(this.vinesWithdrawn, 0.5f, 0.1f, 0.016666668f); // first 0.5f could be 0f, 1f
            float num = Mathf.Lerp(10f, 1f, this.vinesWithdrawn);
            for (int i = 0; i < this.vines.Length; i++)
            {
                for (int j = 0; j < this.vines[i].GetLength(0); j++)
                {
                    float t = (float)j / (float)(this.vines[i].GetLength(0) - 1);
                    this.vines[i][j, 1] = this.vines[i][j, 0];
                    this.vines[i][j, 0] += this.vines[i][j, 2];

                    // this.vines[i][j, 2] -= this.rotation * Mathf.InverseLerp(4f, 0f, (float)j) * 0.8f;
                   // Cannot do since WearingHat stores rotation as float, not Vector2

                    this.vines[i][j, 2] *= Custom.LerpMap(this.vines[i][j, 2].magnitude, 1f, 10f, 1f, 0.5f, Mathf.Lerp(1.4f, 0.4f, t));
                    this.vines[i][j, 2] += Custom.RNV() * 0.2f;
                    this.vines[i][j, 2] *= 0.999f;
                    this.vines[i][j, 2].y -= this.room.gravity * 0.6f;

                    /* Physics collision causes odd vine behavior
                    SharedPhysics.TerrainCollisionData terrainCollisionData = new SharedPhysics.TerrainCollisionData(this.vines[i][j, 0], this.vines[i][j, 1], this.vines[i][j, 2], 1f, new IntVector2(0, 0), false);
                    terrainCollisionData = SharedPhysics.HorizontalCollision(this.room, terrainCollisionData);
                    terrainCollisionData = SharedPhysics.VerticalCollision(this.room, terrainCollisionData);
                    terrainCollisionData = SharedPhysics.SlopesVertically(this.room, terrainCollisionData);
                    this.vines[i][j, 0] = terrainCollisionData.pos;
                    this.vines[i][j, 2] = terrainCollisionData.vel;
                    */
            }
                for (int k = 0; k < this.vines[i].GetLength(0); k++)
                {
                    if (k > 0)
                    {
                        Vector2 normalized = (this.vines[i][k, 0] - this.vines[i][k - 1, 0]).normalized;
                        float num2 = Vector2.Distance(this.vines[i][k, 0], this.vines[i][k - 1, 0]);
                        this.vines[i][k, 0] += normalized * (num - num2) * 0.5f;
                        this.vines[i][k, 2] += normalized * (num - num2) * 0.5f;
                        this.vines[i][k - 1, 0] -= normalized * (num - num2) * 0.5f;
                        this.vines[i][k - 1, 2] -= normalized * (num - num2) * 0.5f;
                        if (k > 1)
                        {
                            normalized = (this.vines[i][k, 0] - this.vines[i][k - 2, 0]).normalized;
                            this.vines[i][k, 2] += normalized * 0.2f;
                            this.vines[i][k - 2, 2] -= normalized * 0.2f;
                        }
                    }
                    else
                    {
                        this.vines[i][k, 0] = this.AttachPos(i, 1f, lastCamPos);
                        this.vines[i][k, 2] *= 0f;
                    }
                }
            }
        }

		public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
		{
            sLeaser.sprites[leaf1].color = new Color(0.53f, 0.81f, 0.4f); // lighter olive
            sLeaser.sprites[leaf2].color = sLeaser.sprites[leaf1].color;

            sLeaser.sprites[petal1].color = new Color(1f, 0.8f, 0.38f); // yellow-orange
            sLeaser.sprites[petal2].color = new Color(1f, 0.97f, 0.57f); // light yellow
            sLeaser.sprites[petal3].color = new Color(1f, 0.41f, 0.65f); // lighter pink
            sLeaser.sprites[petal4].color = sLeaser.sprites[petal3].color;

            for (int i = 0; i < this.vines.Length; i++)
            {
                Color vineColor;
                if (i % 2 == 0)
                {
                    vineColor = new Color(0.50f, 0.84f, 0.22f); // green
                }
                else
                {
                    vineColor = new Color(0.85f, 1f, 0.73f); // very light green
                }

                for (int j = 0; j < (sLeaser.sprites[vineIndex + i] as TriangleMesh).verticeColors.Length; j++)
                {
                    (sLeaser.sprites[vineIndex + i] as TriangleMesh).verticeColors[j] = vineColor;
                }
            }
		}

        /* Methods from JellyFish (based on tentacle code) */

        /* Returns Vector2 representing position where vine j "attaches" to the hat */
        public Vector2 AttachPos(int j, float timeStacker, Vector2 camPos)
        {
            // Requires camPos since the vine/tentacle code relies on this method returning "real" position, not camera-adjusted
            // However basePos always returns the camera-adjusted (camPos has been subtracted) position of slugcat's head
            // Thus re-add camPos to a, to subtract it later in DrawSprites
            Vector2 a = (basePos + camPos) - new Vector2(0, 3);
            a += upDir * this.headRadius;
            a += new Vector2(0, 3);

            switch (j) // 0, 1 - right side of hat, 2, 3 - left side of hat
            {
                case 0:
                    a += new Vector2(-6, 0);
                    break;

                case 1:
                    a += new Vector2(-5, 0);
                    break;

                case 2:
                    a += new Vector2(6, 0);
                    break;

                case 3:
                    a += new Vector2(5, 0);
                    break;
            }
            return a;
        }

        public void ResetVines() // from JellyFish.ResetTentacles
        {
            // Debug.Log("hatworld reset vines " + basePos + " " + (basePos + lastCamPos) + " " + drawPos);
            for (int i = 0; i < this.vines.Length; i++)
            {
                for (int j = 0; j < this.vines[i].GetLength(0); j++)
                {
                    this.vines[i][j, 0] = (basePos + lastCamPos) + Custom.RNV();
                    // add lastCamPos to account for basePos being cam-adjusted

                    this.vines[i][j, 1] = this.vines[i][j, 0];
                    this.vines[i][j, 2] *= 0f;
                }
            }
        }

        public override void AddHatEffects(Creature wearer)
        {
            if (wearer is Player)
            {
                // Increase scav rep by 20
                float change = 2f;
                wearer.abstractCreature.world.game.session.creatureCommunities.InfluenceLikeOfPlayer(
                    CreatureCommunities.CommunityID.Scavengers,
                    wearer.abstractCreature.world.RegionNumber,
                    (wearer.State as PlayerState).playerNumber,
                    change * 0.1f, 0.75f, 0.25f);
            }
        }

        public override void RemoveHatEffects(Creature wearer)
        {
            if (wearer is Player)
            {
                // Decrease scav rep by 20
                float change = 2f;
                wearer.abstractCreature.world.game.session.creatureCommunities.InfluenceLikeOfPlayer(
                    CreatureCommunities.CommunityID.Scavengers,
                    wearer.abstractCreature.world.RegionNumber,
                    (wearer.State as PlayerState).playerNumber,
                    change * 0.1f, 0.75f, 0.25f);
            }
        }
    }
}

