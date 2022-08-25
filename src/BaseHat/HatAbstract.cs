using System;
using Fisobs.Core;
using UnityEngine;

namespace HatWorld
{
    public class HatAbstract : AbstractPhysicalObject
    {
        public string hatType;

        public HatAbstract(World world, WorldCoordinate pos, EntityID ID, string hatType) : base(world, EnumExt_HatWorld.HatAbstract, null, pos, ID)
        {
            this.hatType = hatType;
        }
        public HatAbstract(World world, WorldCoordinate pos, EntityID ID, Type hatType) : base(world, EnumExt_HatWorld.HatAbstract, null, pos, ID)
        {
            this.hatType = hatType.ToString();
        }

        public override string ToString()
        {
            return this.SaveToString(this.hatType);
        }

        public override void Realize()
        {
            if (realizedObject == null)
            {
                Type type = HatWorldMain.GetType(hatType);

                if (type != null)
                {
                    realizedObject = (PhysicalObject)Activator.CreateInstance(type, this, this.world);
                    base.Realize();
                }
                else
                {
                    Debug.Log("HatWorld ERROR: Realize() failure. Cannot find " + hatType);
                }
            }
        }

    }
}
