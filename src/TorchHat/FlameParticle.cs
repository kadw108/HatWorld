using RWCustom;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HatWorld {
    // From HolyFire.FlameParticle in game code
    public class FlameParticle : CosmeticSprite
    {
        public float lifeTime;
        public float life;
        public float lastLife;

        public FlameParticle(Vector2 pos)
        {
            this.pos = pos;
            this.lastPos = pos;
            this.vel = Custom.RNV() * 1.5f * Random.value;
            this.vel.y += 2; // Prevent fire from covering face when you travel up (eg. pole climbing)
            this.life = 1f;
            this.lifeTime = Mathf.Lerp(7f, 15f, Random.value);
        }

        public override void Update(bool eu)
        {
            base.Update(eu);
            this.vel *= 0.8f;
            this.vel.y = this.vel.y + 0.4f;
            this.vel += Custom.RNV() * Random.value * 0.5f;
            this.lastLife = this.life;
            this.life -= 1f / this.lifeTime;
            if (this.life < 0f)
            {
                this.Destroy();
            }
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[1];
            sLeaser.sprites[0] = new FSprite("deerEyeB", true);
            this.AddToContainer(sLeaser, rCam, null);
            base.InitiateSprites(sLeaser, rCam);
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            sLeaser.sprites[0].x = Mathf.Lerp(this.lastPos.x, this.pos.x, timeStacker);
            sLeaser.sprites[0].y = Mathf.Lerp(this.lastPos.y, this.pos.y, timeStacker);
            float num = Mathf.Lerp(this.lastLife, this.life, timeStacker);
            sLeaser.sprites[0].scale = num * 0.9f;
            // sLeaser.sprites[0].color = Custom.HSL2RGB(Mathf.Lerp(0.01f, 0.08f, num), 1f, Mathf.Lerp(0.5f, 1f, Mathf.Pow(num, 3f)));
            sLeaser.sprites[0].color = Custom.HSL2RGB(Mathf.Lerp(0.06f, 0.14f, num), 1f, Mathf.Lerp(0.5f, 0.9f, Mathf.Pow(num, 3f)));

            base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
        }
    }
}
