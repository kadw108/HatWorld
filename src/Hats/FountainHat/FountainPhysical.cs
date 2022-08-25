using RWCustom;
using UnityEngine;

namespace HatWorld
{
    public class FountainPhysical : HatPhysical
    {
        // Constants for sLeaser sprite index (higher index appears over lower)
        public const int botDisk = 1;
        public const int pole = 0;

        public const int petal1 = 2;
        public const int petal2 = 3;
        public const int petal3 = 4;
        public const int petal4 = 5;

        public ChunkDynamicSoundLoop soundLoop;

        public JetWaterEmitter[] waterJets = new JetWaterEmitter[2];

        public static new HatWearing GetWornHat(GraphicsModule graphicsModule)
        {
            return new FountainWearing(graphicsModule);
        }

        public FountainPhysical(HatAbstract abstr, World world) : base(abstr, world)
        {
            this.soundLoop = new ChunkDynamicSoundLoop(this.firstChunk);
            this.soundLoop.sound = SoundID.Water_Surface_Calm_LOOP;
            this.soundLoop.Pitch = 1.7f;
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[6];
            sLeaser.sprites[botDisk] = new FSprite("QuarterPips2", true) { scaleY = 0.4f, scaleX = 0.5f };
            sLeaser.sprites[pole] = new FSprite("LizardScaleA1", true) { scaleY = 0.85f, scaleX = 0.5f };
            for (int i = petal1; i <= petal4; i++)
            {
                sLeaser.sprites[i] = new FSprite("KarmaPetal", true) { scaleY = 0.5f, scaleX = 0.2f };
            }
            this.AddToContainer(sLeaser, rCam, null);
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            base.DrawSprites(sLeaser, rCam, timeStacker, camPos);

            for (int j = 0; j < petal1; j++)
            {
                sLeaser.sprites[j].rotation = hatRotation;
            }

            sLeaser.sprites[botDisk].SetPosition(drawPos - upDir * 3);
            sLeaser.sprites[botDisk].rotation += 180f;

            sLeaser.sprites[pole].SetPosition(drawPos + upDir * 5);
            sLeaser.sprites[pole].rotation += 90f;

            for (int i = petal1; i <= petal4; i++)
            {
                float rotShift = 45 + (i - petal1) * 24 + hatRotation;
                sLeaser.sprites[i].rotation = rotShift;
                sLeaser.sprites[i].SetPosition(drawPos + upDir * 9 + Custom.DegToVec(rotShift) * 4);
            }

            for (int i = 0; i < waterJets.Length; i++)
            {
                if (this.waterJets[i] != null)
                {
                    this.waterJets[i].Update();
                }
                if (this.waterJets[i] == null)
                {
                    if (this.room != null)
                    {
                        this.waterJets[i] = new JetWaterEmitter(this.room);
                    }
                }
                else if (this.waterJets[i].Dead)
                {
                    this.waterJets[i] = null;
                }
                else
                {
                    this.waterJets[i].NewParticle(drawPos + upDir * 7 + camPos, firstChunk.vel + new Vector2(2 + i * -4, 0) + upDir * 7, 1.3f, 0.9f);
                }
            }

            if (base.slatedForDeletetion || rCam.room != this.room)
            {
                this.soundLoop.Volume = 0f;
            }
            else
            {
                this.soundLoop.Volume = 0.3f;
            }
        }

        public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            sLeaser.sprites[botDisk].color = new Color(0.46f, 0.46f, 0.85f); // blue-gray
            sLeaser.sprites[pole].color = new Color(0.62f, 0.63f, 1f); // bluish white

            for (int i = petal1; i <= petal4; i++)
            {
                sLeaser.sprites[i].color = new Color(.75f, .85f, 1f);
            }
        }

        public override void Update(bool eu)
        {
            base.Update(eu);
            if (this.soundLoop.Volume > 0f)
            {
                this.soundLoop.Update();
            }
        }

        // prevent water jets from disappearing when entering new room
        public override void NewRoom(Room newRoom)
        {
            base.NewRoom(newRoom);

            for (int i = 0; i < waterJets.Length; i++)
            {
                this.waterJets[i] = new JetWaterEmitter(this.room);
            }
        }
    }
}


