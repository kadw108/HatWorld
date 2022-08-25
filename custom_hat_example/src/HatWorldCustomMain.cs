using BepInEx;
using UnityEngine;
using HatWorld;
using System.Reflection;

namespace HatWorldCustomExample
{
    [BepInPlugin("kadw.hatworld_CustomExample", "HatWorld_CustomExample", "1.0.0")]
    public class HatWorldCustomMain : BaseUnityPlugin
    {
        public void OnEnable()
        {
            // Required to make this new hat exist
            HatWorldMain.AddType(typeof(RedBubblePhysical));

            // Required to create shelter icon/sandbox icon and add hat to sandbox
            HatWorldMain.hatFisob.AddIconAndSandbox("HatWorldCustomExample.RedBubblePhysical", EnumExt_HatWorldCustom.RedBubbleUnlockID, Color.red);
            // If you don't want the hat to be available in sandbox, use parent=null
            // eg. HatWorldMain.hatFisob.AddIconAndSandbox("HatWorldCustomExample.RedBubblePhysical", EnumExt_HatWorldCustom.RedBubbleUnlockID, Color.red, null);

            // Optional: make the hat spawn in a certain location, like colored pearls
            HatPlacer.AddSpawns(Assembly.GetExecutingAssembly().GetManifestResourceStream("HatWorldCustomExample.src.spawns.txt"));
        }
    }
}
