using System.Collections.Generic;
using UnityEngine;

namespace HatWorld
{
    // Identical to WaterJet but uses JetWaterCustom instead of JetWater
    public class JetWaterEmitter : SplashWater.WaterJet
    {
        public JetWaterEmitter(Room room) : base(room)
        {
            this.particles = new List<JetWaterCustom>();
            this.restingParticles = new List<JetWaterCustom>();
        }

        public new void NewParticle(Vector2 emissionPoint, Vector2 emissionForce, float amount, float initRad)
        {
            JetWaterCustom jetWater;
            if (this.restingParticles.Count > 0 && this.restingParticles[0].killCounter > 1)
            {
                jetWater = this.restingParticles[0];
                this.restingParticles.RemoveAt(0);
            }
            else
            {
                jetWater = new JetWaterCustom(this);
                this.room.AddObject(jetWater);
            }
            this.particles.Add(jetWater);
            jetWater.Reset(emissionPoint, emissionForce, amount, initRad);
            if (this.counter < 2 && this.particles.Count > 2)
            {
                jetWater.otherParticle = this.particles[this.particles.Count - 2];
            }
            this.counter = 0;
        }

        // Token: 0x0400090C RID: 2316
        public new List<JetWaterCustom> particles;

        // Token: 0x0400090D RID: 2317
        public new List<JetWaterCustom> restingParticles;
    }
}
