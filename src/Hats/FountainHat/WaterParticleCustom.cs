using RWCustom;
using UnityEngine;

namespace HatWorld
{
    // Token: 0x020001A7 RID: 423
    public class WaterParticleCustom : SplashWater.WaterParticle
    {
        // Token: 0x06000AA6 RID: 2726 RVA: 0x00069B4C File Offset: 0x00067D4C
        public override void Update(bool eu)
        {
            this.lastPos = this.pos;
            this.pos += this.vel;
            this.evenUpdate = eu;

            this.lastLife = this.life;
            this.life -= 1f / this.lifeTime;
            this.makeSoundCounter--;
            this.vel *= 0.97f;
            this.vel.y = this.vel.y - 0.9f;
            this.lastRad = this.rad;
            this.rad += Mathf.InverseLerp(1f, 20f, this.vel.magnitude);
            if (this.rad < this.amount)
            {
                this.rad = Mathf.Lerp(this.rad, this.amount, 0.1f);
            }
            if (this.rad > this.amount * 1.5f)
            {
                this.life -= 0.05f;
            }
            if (this.rad > this.amount * 2f)
            {
                this.rad = this.amount * 2f;
            }
            if (this.room.GetTile(this.pos).Solid && !this.room.GetTile(this.lastPos).Solid)
            {
                IntVector2? intVector = SharedPhysics.RayTraceTilesForTerrainReturnFirstSolid(this.room, this.room.GetTilePosition(this.lastPos), this.room.GetTilePosition(this.pos));
                FloatRect floatRect = Custom.RectCollision(this.pos, this.lastPos, this.room.TileRect(intVector.Value).Grow(2f));
                this.pos = floatRect.GetCorner(FloatRect.CornerLabel.D);
                if (floatRect.GetCorner(FloatRect.CornerLabel.B).x < 0f)
                {
                    this.vel.x = Mathf.Abs(this.vel.x) * 0.4f;
                }
                else if (floatRect.GetCorner(FloatRect.CornerLabel.B).x > 0f)
                {
                    this.vel.x = -Mathf.Abs(this.vel.x) * 0.4f;
                }
                else if (floatRect.GetCorner(FloatRect.CornerLabel.B).y < 0f)
                {
                    this.vel.y = Mathf.Abs(this.vel.y) * 0.4f;
                }
                else if (floatRect.GetCorner(FloatRect.CornerLabel.B).y > 0f)
                {
                    this.vel.y = -Mathf.Abs(this.vel.y) * 0.4f;
                }
                // this.room.AddObject(new WaterDrip(this.pos, this.vel + Custom.RNV() * this.vel.magnitude, true));
                if (UnityEngine.Random.value > 0.9)
                {
                    this.room.AddObject(new WaterDrip(this.pos, this.vel + Custom.RNV() * 2, false));
                }
                this.life -= 0.06f;
            }
        }
    }
}