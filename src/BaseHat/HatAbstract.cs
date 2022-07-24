using System;
using System.Linq;
using Fisobs;
using Fisobs.Core;
using UnityEngine;

namespace HatWorld
{
    public class HatAbstract : AbstractPhysicalObject
    {
        public string hatType;
        
        public HatAbstract(World world, WorldCoordinate pos, EntityID ID, string hatType) : base(world, HatFisob.Instance.Type, null, pos, ID) {
            this.hatType = hatType;
        }
        public HatAbstract(World world, WorldCoordinate pos, EntityID ID, Type hatType) : base(world, HatFisob.Instance.Type, null, pos, ID) {
            this.hatType = hatType.Namespace + "." + hatType.Name;
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
                    realizedObject = (PhysicalObject) Activator.CreateInstance(type, this, this.world);
                    Debug.Log("Hatworld Realize() success: " + realizedObject);
                    base.Realize();
                }
                else
                {
                    Debug.Log("HatWorld Realize() failure: cannot find " + hatType);
                }
            }
        }

    }
}
