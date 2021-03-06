using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace ClickerClass.Items.Weapons.Clickers
{
	public class SpectreClicker : ClickerWeapon
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
			DisplayName.SetDefault("Spectre Clicker");
			Tooltip.SetDefault("Click on an enemy within sight to damage them");

			ClickEffect.PhaseReach = ClickerSystem.RegisterClickEffect(mod, "PhaseReach", "Phase Reach", "Damage enemies regardless of location", 1, new Color(100, 255, 255, 0), null);
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			SetRadius(item, 5f);
			SetColor(item, new Color(100, 255, 255, 0));
			SetDust(item, 88);
			AddEffect(item, ClickEffect.PhaseReach);

			item.damage = 50;
			item.width = 30;
			item.height = 30;
			item.knockBack = 1f;
			item.value = 450000;
			item.rare = 8;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.SpectreBar, 8);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
