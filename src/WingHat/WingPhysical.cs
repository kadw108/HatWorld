using UnityEngine;
using RWCustom;

namespace HatWorld
{
    sealed class WingPhysical : HatPhysical
    {
		public float wingFlapCycle;
        public float wingFlapShift;

        // Constants for sLeaser sprite index (higher index appears over lower)
		public const int wingLeft1 = 1;
		public const int wingRight1 = 0;
        public const int circleLeft = 2;
        public const int circleRight = 3;

        public override HatWearing getWornHat(GraphicsModule graphicsModule)
        {
            return new WingWearing(graphicsModule);
        }

		public float[] wingLengths;

        public WingPhysical(HatAbstract abstr, World world) : base(abstr, world) {
            base.gravity = 0.8f;
            base.buoyancy = 1.8f;
            base.bounce = 0.3f;
			this.wingLengths = new float[2] { 23f, 23f };
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

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            // Setup
            base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
            for (int j = 2; j < sLeaser.sprites.Length; j++)
            {
                sLeaser.sprites[j].rotation = hatRotation;
            }
            sLeaser.sprites[circleLeft].SetPosition(drawPos - rightDir * 2);
            sLeaser.sprites[circleRight].SetPosition(drawPos + rightDir * 2);

			for (int k = 0; k < 2; k++)
			{
				int num10 = 0;
				Vector2 wingWidth = new Vector2(0, 1); // wing width and orientation, originally for direction of centipede travel, (1, 0) flips wings 90 deg (bad)

                Vector2 shift = (k == wingLeft1) ?
                    new Vector2(-2, 2 + this.wingFlapShift).normalized * this.wingLengths[k] :
                    new Vector2(2, 2 + this.wingFlapShift).normalized * this.wingLengths[k];
                Vector2 vector13 = drawPos + shift;

                Vector2 vector14 = drawPos;
				(sLeaser.sprites[k] as CustomFSprite).MoveVertice(1, vector13 + wingWidth * 2f);
				(sLeaser.sprites[k] as CustomFSprite).MoveVertice(0, vector13 - wingWidth * 2f);
				(sLeaser.sprites[k] as CustomFSprite).MoveVertice(2, vector14 + wingWidth * 2f);
				(sLeaser.sprites[k] as CustomFSprite).MoveVertice(3, vector14 - wingWidth * 2f);
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

        public override void Update(bool eu)
        {
            base.Update(eu);

            this.wingFlapCycle += 0.4f + 0.5f * Random.value;
            this.wingFlapShift = Mathf.Sin(this.wingFlapCycle);
        }
    }
}

