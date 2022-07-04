using RWCustom;
using UnityEngine;

namespace HatWorld
{
	// Hat that is being worn by slugcat
	// Uses code from FestiveWorld mod
	public abstract class WearingHat : UpdatableAndDeletable, IDrawable
	{
		public GraphicsModule parent { get; }
		public int anchorSprite { get; }
		public float rotation { get; }
		public float headRadius { get; }

		// for ParentDrawSprites
		public Vector2 basePos;
		public float baseRot;
		public bool flipX;
		public bool flipY;

		// for DrawSprites
		public Vector2 drawPos;
		public Vector2 upDir;
		public Vector2 rightDir;

		public bool initialized;

		public WearingHat(GraphicsModule parent, int anchorSprite, float rotation, float headRadius)
		{
			this.parent = parent;
			this.anchorSprite = anchorSprite;
			this.rotation = rotation;
			this.headRadius = headRadius;
			this.initialized = false; // ParentDrawSprites must run to set basePos and drawPos
			parent.owner.room.AddObject(this);
		}

		// method to get type of hat, required so HatWorldPlugin knows which type of hat to give back when worn hat is removed
		public abstract HatType hatType { get; }

		public abstract void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam);

		public abstract void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette);

		public virtual void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
			if (this.initialized)
            {
                drawPos = this.basePos;

                upDir = new Vector2(Mathf.Cos((this.rotation + this.baseRot) * -0.017453292f), Mathf.Sin((this.rotation + this.baseRot) * -0.017453292f));
                rightDir = -Custom.PerpendicularVector(upDir);
                if (flipY) upDir *= -1;
                if (flipX) rightDir *= -1;
                drawPos += upDir * this.headRadius;

                if (base.slatedForDeletetion || rCam.room != this.room || this.room != this.parent.owner.room)
                {
                    sLeaser.CleanSpritesAndRemove();
                }

				ChildDrawSprites(sLeaser, rCam, timeStacker, camPos);
            }
        }
		public abstract void ChildDrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos);

		public override void Update(bool eu)
        {
			base.Update(eu);

			GraphicsModule graphicsModule = this.parent;
			if (((graphicsModule != null) ? graphicsModule.owner : null) == null || this.parent.owner.slatedForDeletetion || base.slatedForDeletetion)
			{
				this.Destroy();
			}
			else if (this.parent.owner.room != null && this.initialized)
            {
				ChildUpdate(eu);
            }
		}
		public virtual void ChildUpdate(bool eu) { }

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
		public void ParentDrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
		{
			if (sLeaser.sprites.Length > this.anchorSprite)
			{
				this.basePos = sLeaser.sprites[this.anchorSprite].GetPosition();
				this.baseRot = sLeaser.sprites[this.anchorSprite].rotation;

				this.flipX = (sLeaser.sprites[this.anchorSprite].scaleX > 0f);
				this.flipY = (sLeaser.sprites[this.anchorSprite].scaleY < 0f);

				this.initialized = true; // basePos and baseRot have been set, program can now update/draw as normal
			}
		}

	}
}
