using HatWorld.src.HatManager;
using OptionalUI;
using UnityEngine;

namespace HatWorld
{
    public class HatWorldConfig : OptionInterface
    {
        public HatWorldConfig(HatWorldMain mainPlugin) : base(mainPlugin) { }

		public override void Initialize()
		{
			base.Initialize();
			this.Tabs = new OpTab[1];
			this.Tabs[0] = new OpTab("");

			CreateMainAndWear();
			CreateSpawn();
			CreateEffectsBox();
		}

		private void CreateMainAndWear()
        {
			OpLabel title = new OpLabel(30f, 550f, "HatWorld", true);
			OpLabel author = new OpLabel(30f, 520f, $"Author: kadw (based on code from Dual and Slime_Cubed) | Version 1.0.0", false);

			OpRect wearRect = new OpRect(new Vector2(15f, 320f), new Vector2(220f, 185f));

			OpLabel wearDesc = new OpLabel(30f, 480f, "Wear/Remove Hat Button:", false);
			int[] playerButtonX = new int[] { 440, 405, 370, 335 };
			OpLabel player1 = new OpLabel(30f, playerButtonX[0], "Player 1 (Survivor)", false);
			OpLabel player2 = new OpLabel(30f, playerButtonX[1], "Player 2 (Monk)", false);
			OpLabel player3 = new OpLabel(30f, playerButtonX[2], "Player 3 (Hunter)", false);
			OpLabel player4 = new OpLabel(30f, playerButtonX[3], "Player 4 (Nightcat)", false);

			OpKeyBinder wear1 = new OpKeyBinder(
				new Vector2(150f, playerButtonX[0]),
				new Vector2(70f, 10f),
				"kadw.hatworld", "hatworld_wearKey1",
				"S",
				false);
			OpKeyBinder wear2 = new OpKeyBinder(
				new Vector2(150f, playerButtonX[1]),
				new Vector2(70f, 10f),
				"kadw.hatworld", "hatworld_wearKey2",
				"S",
				false);
			OpKeyBinder wear3 = new OpKeyBinder(
				new Vector2(150f, playerButtonX[2]),
				new Vector2(70f, 10f),
				"kadw.hatworld", "hatworld_wearKey3",
				"S",
				false);
			OpKeyBinder wear4 = new OpKeyBinder(
				new Vector2(150f, playerButtonX[3]),
				new Vector2(70f, 10f),
				"kadw.hatworld", "hatworld_wearKey4",
				"S",
				false);

			this.Tabs[0].AddItems(player1, player2, player3, player4,
				wear1, wear2, wear3, wear4,
				wearRect, wearDesc,
				title, author);
        }

		private void CreateSpawn()
        {
			OpLabel desc = new OpLabel(30f, 100f, "I don't recommend using the spawn hat button if you're using this mod for the first time.\n"+
				"Go out and discover things! Or not, if you really don't want to...", false);

			OpRect spawnRect = new OpRect(new Vector2(285f, 320f), new Vector2(220f, 185f));

			OpLabel spawnDesc = new OpLabel(300f, 480f, "Spawn Hat Button:", false);
			int[] playerButtonX = new int[] { 440, 405, 370, 335 };
			OpLabel player1 = new OpLabel(300f, playerButtonX[0], "Player 1 (Survivor)", false);
			OpLabel player2 = new OpLabel(300f, playerButtonX[1], "Player 2 (Monk)", false);
			OpLabel player3 = new OpLabel(300f, playerButtonX[2], "Player 3 (Hunter)", false);
			OpLabel player4 = new OpLabel(300f, playerButtonX[3], "Player 4 (Nightcat)", false);

			OpKeyBinder spawn1 = new OpKeyBinder(
				new Vector2(420f, playerButtonX[0]),
				new Vector2(70f, 10f),
				"kadw.hatworld", "hatworld_spawnKey1",
				"A",
				false);
			OpKeyBinder spawn2 = new OpKeyBinder(
				new Vector2(420f, playerButtonX[1]),
				new Vector2(70f, 10f),
				"kadw.hatworld", "hatworld_spawnKey2",
				"A",
				false);
			OpKeyBinder spawn3 = new OpKeyBinder(
				new Vector2(420f, playerButtonX[2]),
				new Vector2(70f, 10f),
				"kadw.hatworld", "hatworld_spawnKey3",
				"A",
				false);
			OpKeyBinder spawn4 = new OpKeyBinder(
				new Vector2(420f, playerButtonX[3]),
				new Vector2(70f, 10f),
				"kadw.hatworld", "hatworld_spawnKey4",
				"A",
				false);

			this.Tabs[0].AddItems(desc,
				player1, player2, player3, player4,
				spawn1, spawn2, spawn3, spawn4,
				spawnRect, spawnDesc);
        }

		private void CreateEffectsBox()
        {
            this.Tabs[0].AddItems(new UIelement[]
{
				new OpCheckBox(new Vector2(30f, 250f), "hatworld_effectsOn", true)
				{
					description = "If on, hats can grant special abilities to the wearer."
				},
				new OpLabel(65f, 250f, "Hats can grant special abilities to the wearer (turn off to make hats purely cosmetic).", false)
            });
		}

		public override void ConfigOnChange()
		{
			base.ConfigOnChange();
			if (config.ContainsKey("hatworld_wearKey1"))
			{
				PlayerHatManager.wearHatKeys[0] = OpKeyBinder.StringToKeyCode(config["hatworld_wearKey1"]);
			}
			if (config.ContainsKey("hatworld_wearKey2"))
			{
				PlayerHatManager.wearHatKeys[1] = OpKeyBinder.StringToKeyCode(config["hatworld_wearKey2"]);
			}
			if (config.ContainsKey("hatworld_wearKey3"))
			{
				PlayerHatManager.wearHatKeys[2] = OpKeyBinder.StringToKeyCode(config["hatworld_wearKey3"]);
			}
			if (config.ContainsKey("hatworld_wearKey4"))
			{
				PlayerHatManager.wearHatKeys[3] = OpKeyBinder.StringToKeyCode(config["hatworld_wearKey4"]);
			}

			if (config.ContainsKey("hatworld_spawnKey1"))
			{
				PlayerHatManager.createHatKeys[0] = OpKeyBinder.StringToKeyCode(config["hatworld_spawnKey1"]);
			}
			if (config.ContainsKey("hatworld_spawnKey2"))
			{
				PlayerHatManager.createHatKeys[1] = OpKeyBinder.StringToKeyCode(config["hatworld_spawnKey2"]);
			}
			if (config.ContainsKey("hatworld_spawnKey3"))
			{
				PlayerHatManager.createHatKeys[2] = OpKeyBinder.StringToKeyCode(config["hatworld_spawnKey3"]);
			}
			if (config.ContainsKey("hatworld_spawnKey4"))
			{
				PlayerHatManager.createHatKeys[3] = OpKeyBinder.StringToKeyCode(config["hatworld_spawnKey4"]);
			}

			if (config.ContainsKey("hatworld_effectsOn"))
            {
				bool.TryParse(config["hatworld_effectsOn"], out HatWearing.effectsOn);
            }
		}
	}
}
