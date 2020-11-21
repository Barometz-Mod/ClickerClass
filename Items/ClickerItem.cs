using Terraria;
using Terraria.ModLoader;

namespace ClickerClass.Items
{
	/// <summary>
	/// Convenience class that every clicker class related item inherits from
	/// </summary>
	public abstract class ClickerItem : ModItem
	{
		/// <summary>
		/// Call this in the inherited class as base.SetStaticDefaults() at the start of SetStaticDefaults
		/// </summary>
		public override void SetStaticDefaults()
		{
			ClickerSystem.RegisterClickerItem(this);
		}

		/// <summary>
		/// Call in <see cref="ModItem.SetDefaults"/> for a clicker item to make it display total click count in the tooltip
		/// </summary>
		/// <param name="item">The clicker class item</param>
		public static void SetDisplayTotalClicks(Item item)
		{
			if (ClickerSystem.IsClickerItem(item, out ClickerItemCore clickerItem))
			{
				clickerItem.isClickerDisplayTotal = true;
			}
		}
	}
}
