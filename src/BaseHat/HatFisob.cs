// Finished for now.
using HatFisobs;

namespace HatWorld
{
    internal class HatFisob : Fisob
    {
        public static readonly HatFisob Instance = new HatFisob();
        private static readonly HatProperties properties = new HatProperties();

        private HatFisob() : base("custom_fisob") { }

        public override AbstractPhysicalObject Parse(World world, EntitySaveData saveData)
        {
            // taken from Centishields
            string[] p = saveData.CustomData.Split(';');

            if (p.Length < 7)
            {
                p = new string[7];
            }

            return new HatAbstract(world, saveData.Pos, saveData.ID, (HatType)(int.TryParse(p[6], out var h) ? h : 0));
            /* I don't even know how to use {} with object constructors
            {
                // ID = p[0],
                // type = p[1],
                // room = p[3],
                // x = int.TryParse(p[0], out var a) ? a : 0,
                // y = int.TryParse(p[0], out var a) ? a : 0,
                hatType = (HatAbstract.HatType) (int.TryParse(p[6], out var h) ? h : 0),
                // abstractNode = p[7]??
            };
            */
        }

        public override SandboxState GetSandboxState(MultiplayerUnlocks unlocks)
        {
            return SandboxState.Unlocked;
        }

        public override FisobProperties GetProperties(PhysicalObject forObject)
        {
            return properties;
        }
    }
}
