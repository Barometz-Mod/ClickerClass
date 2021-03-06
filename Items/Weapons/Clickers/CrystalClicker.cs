using ClickerClass.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ClickerClass.Items.Weapons.Clickers
{
	public class CrystalClicker : ClickerWeapon
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
			DisplayName.SetDefault("Crystal Clicker");

			ClickEffect.Dazzle = ClickerSystem.RegisterClickEffect(mod, "Dazzle", "Dazzle", "Inflicts the Confused debuff", 8, new Color(200, 50, 255, 0), delegate (Player player, Vector2 position, int type, int damage, float knockBack)
			{
				Main.PlaySound(SoundID.Item, (int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, 28);
				Projectile.NewProjectile(Main.MouseWorld.X, Main.MouseWorld.Y, 0f, 0f, ModContent.ProjectileType<CrystalClickerPro>(), 0, knockBack, player.whoAmI);
			});
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			SetRadius(item, 3.1f);
			SetColor(item, new Color(200, 50, 255, 0));
			SetDust(item, 86);
			AddEffect(item, ClickEffect.Dazzle);

			item.damage = 34;
			item.width = 30;
			item.height = 30;
			item.knockBack = 2f;
			item.value = 90000;
			item.rare = 4;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.CrystalShard, 8);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
