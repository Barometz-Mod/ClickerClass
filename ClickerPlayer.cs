using ClickerClass.Buffs;
using ClickerClass.Items;
using ClickerClass.NPCs;
using ClickerClass.Projectiles;
using ClickerClass.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ClickerClass
{
	public partial class ClickerPlayer : ModPlayer
	{
		//Key presses
		public double pressedAutoClick;
		public int clickerClassTime = 0;

		//-Clicker-
		//Misc
		public Color clickerColor = new Color(0, 0, 0, 0);
		/// <summary>
		/// Visual indicator that the cursor is inside clicker radius
		/// </summary>
		public bool clickerInRange = false;
		/// <summary>
		/// Visual indicator that the cursor is inside mech radius
		/// </summary>
		public bool clickerInRangeMech = false;
		public bool clickerSelected = false;
		public bool clickerAutoClick = false;
		public int clickerPerSecondTimer = 0;
		public int clickerPerSecond = 0;
		public int clickerTotal = 0;
		public int clickAmount = 0;

		//Armor
		public int clickerSetTimer = 0;
		public float clickerMechSetRatio = 0f;
		public float clickerMechSetAngle = 0f;
		/// <summary>
		/// Calculated after clickerRadius is calculated, and if the mech set is worn
		/// </summary>
		public Vector2 clickerMechSetPosition = Vector2.Zero;
		public bool clickerMechSet = false;
		public bool clickerMiceSetAllowed = true;
		public bool clickerMiceSet = false;
		public bool clickerPrecursorSetAllowed = true;
		public bool clickerPrecursorSet = false;
		public bool clickerOverclockSetAllowed = true;
		public bool clickerOverclockSet = false;

		//Acc
		public bool clickerChocolateChipAcc = false;
		public bool clickerEnchantedLED = false;
		public bool clickerEnchantedLED2 = false;
		public bool clickerAutoClickAcc = false;
		public bool clickerStickyAcc = false;
		public bool clickerMilkAcc = false;
		public bool clickerCookieAcc = false;
		public bool clickerCookieAcc2 = false;
		public int clickerCookieAccTimer = 0;

		//Stats
		public int clickerDamageFlat = 0;
		public int clickerBonus = 0;
		public int clickerCrit = 4;
		public float clickerDamage = 1f;
		public float clickerBonusPercent = 1f;
		/// <summary>
		/// Effective clicker radius in pixels when multiplied by 100
		/// </summary>
		public float clickerRadius = 1f;

		/// <summary>
		/// Clicker radius in pixels
		/// </summary>
		public float ClickerRadiusReal => clickerRadius * 100;
		/// <summary>
		/// Mech radius in pixels
		/// </summary>
		public float ClickerRadiusMech => ClickerRadiusReal * 0.5f;

		//Helper methods
		/// <summary>
		/// Returns the position from the ratio and angle
		/// </summary>
		public Vector2 CalculateMechPosition()
		{
			float length = clickerMechSetRatio * ClickerRadiusReal;
			Vector2 direction = clickerMechSetAngle.ToRotationVector2();
			return direction * length;
		}

		/// <summary>
		/// Construct ratio and angle from position
		/// </summary>
		public void SetMechPosition(Vector2 position)
		{
			//Construct ratio and angle from position
			Vector2 toPosition = position - player.Center;
			float length = toPosition.Length();
			float radius = ClickerRadiusReal;
			float ratio = length / radius;
			clickerMechSetRatio = ratio;
			clickerMechSetAngle = toPosition.ToRotation();
		}

		public override void ResetEffects()
		{
			//-Clicker-
			//Misc
			clickerColor = new Color(0, 0, 0, 0);
			clickerInRange = false;
			clickerInRangeMech = false;
			clickerSelected = false;

			//Armor
			clickerMiceSetAllowed = true;
			clickerMechSet = false;
			clickerMiceSetAllowed = true;
			clickerMiceSet = false;
			clickerPrecursorSetAllowed = true;
			clickerPrecursorSet = false;
			clickerOverclockSetAllowed = true;
			clickerOverclockSet = false;

			//Acc
			clickerChocolateChipAcc = false;
			clickerEnchantedLED = false;
			clickerEnchantedLED2 = false;
			clickerStickyAcc = false;
			clickerAutoClickAcc = false;
			clickerMilkAcc = false;
			clickerCookieAcc = false;
			clickerCookieAcc2 = false;

			//Stats
			clickerDamage = 1f;
			clickerBonusPercent = 1f;
			clickerRadius = 1f;
			clickerDamageFlat = 0;
			clickerCrit = 4;
			clickerBonus = 0;
		}

		public override void Initialize()
		{
			clickerTotal = 0;
		}

		public override TagCompound Save()
		{
			return new TagCompound
			{
				{"clickerTotal", clickerTotal}
			};
		}

		public override void Load(TagCompound tag)
		{
			clickerTotal = tag.GetInt("clickerTotal");
		}

		public override void ProcessTriggers(TriggersSet triggersSet)
		{
			// checks for frozen, webbed and stoned
			if (player.CCed)
			{
				return;
			}

			if (ClickerClass.AutoClickKey.JustPressed)
			{
				if (Math.Abs(clickerClassTime - pressedAutoClick) > 60)
				{
					pressedAutoClick = clickerClassTime;

					Main.PlaySound(SoundID.MenuTick, player.position);
					clickerAutoClick = clickerAutoClick ? false : true;
				}
			}
		}

		public override void PostUpdateEquips()
		{
			clickerClassTime++;
			if (clickerClassTime > 36000)
			{
				clickerClassTime = 0;
			}

			if (!clickerAutoClickAcc)
			{
				clickerAutoClick = false;
			}

			if (clickerSetTimer > 0)
			{
				clickerSetTimer--;
			}
			
			if (!clickerMechSet)
			{
				clickerMechSetPosition = Vector2.Zero;
				clickerMechSetRatio = 0f;
				clickerMechSetAngle = 0f;
			}

			if (/*player.HeldItem.damage > 0 && */player.HeldItem.modItem is ClickerItem clickerItem && clickerItem.isClicker)
			{
				//TODO phaseCheck code could be here aswell?
				clickerSelected = true;
				if (clickerItem.radiusBoost > 0f || clickerItem.radiusBoostPrefix > 0f)
				{
					clickerRadius += clickerItem.radiusBoost + clickerItem.radiusBoostPrefix;
				}
				if (Vector2.Distance(Main.MouseWorld, player.Center) < ClickerRadiusReal && Collision.CanHit(new Vector2(player.Center.X, player.Center.Y - 12), 1, 1, Main.MouseWorld, 1, 1))
				{
					clickerInRange = true;
				}
				if (clickerMechSet)
				{
					//Important: has to be after final clickerRadius calculation because it depends on it
					clickerMechSetPosition = player.Center + CalculateMechPosition();
				}
				if (Vector2.Distance(Main.MouseWorld, clickerMechSetPosition) < ClickerRadiusMech && Collision.CanHit(clickerMechSetPosition, 1, 1, Main.MouseWorld, 1, 1))
				{
					clickerInRangeMech = true;
				}
				clickerColor = clickerItem.clickerColorItem;
			}

			if (player.HasBuff(ModContent.BuffType<Haste>()))
			{
				player.armorEffectDrawShadow = true;
			}

			//Armor
			int head = 0;
			int body = 1;
			int legs = 2;
			int vanityHead = 10;
			int vanityBody = 11;
			int vanityLegs = 12;

			Item itemHead = player.armor[head];
			Item itemBody = player.armor[body];
			Item itemLegs = player.armor[legs];

			Item itemVanityHead = player.armor[vanityHead];
			Item itemVanityBody = player.armor[vanityBody];
			Item itemVanityLegs = player.armor[vanityLegs];

			if (player.wereWolf || player.merman)
			{
				clickerMiceSetAllowed = false;
				clickerPrecursorSetAllowed = false;
				clickerOverclockSetAllowed = false;
				//clickerOverclockSetAllowed = false;
			}

			if (itemVanityHead.type > 0)
			{
				if (itemVanityHead.type != ModContent.ItemType<MiceMask>())
				{
					clickerMiceSetAllowed = false;
				}
				if (itemVanityHead.type != ModContent.ItemType<PrecursorHelmet>())
				{
					clickerPrecursorSetAllowed = false;
				}
				if (itemVanityHead.type != ModContent.ItemType<OverclockHelmet>())
				{
					clickerOverclockSetAllowed = false;
				}
				//if (itemVanityHead.type != ModContent.ItemType<OverclockHelmet>())
				//{
				//	clickerOverclockSetAllowed = false;
				//}
			}
			if (itemVanityBody.type > 0)
			{
				if (itemVanityBody.type != ModContent.ItemType<MiceSuit>())
				{
					clickerMiceSetAllowed = false;
				}
				if (itemVanityBody.type != ModContent.ItemType<PrecursorBreastplate>())
				{
					clickerPrecursorSetAllowed = false;
				}
				if (itemVanityBody.type != ModContent.ItemType<OverclockSuit>())
				{
					clickerOverclockSetAllowed = false;
				}
				//if (itemVanityBody.type != ModContent.ItemType<OverclockSuit>())
				//{
				//	clickerOverclockSetAllowed = false;
				//}
			}
			if (itemVanityLegs.type > 0)
			{
				if (itemVanityLegs.type != ModContent.ItemType<MiceBoots>())
				{
					clickerMiceSetAllowed = false;
				}
				if (itemVanityLegs.type != ModContent.ItemType<PrecursorGreaves>())
				{
					clickerPrecursorSetAllowed = false;
				}
				if (itemVanityLegs.type != ModContent.ItemType<OverclockBoots>())
				{
					clickerOverclockSetAllowed = false;
				}
				//if (itemVanityLegs.type != ModContent.ItemType<OverclockBoots>())
				//{
				//	clickerOverclockSetAllowed = false;
				//}
			}

			if (clickerOverclockSet && clickerOverclockSetAllowed)
			{
				Lighting.AddLight(player.position, 0.3f, 0.075f, 0.075f);
			}
			if (clickerPrecursorSet && clickerPrecursorSetAllowed)
			{
				Lighting.AddLight(player.position, 0.2f, 0.15f, 0.05f);
			}
			if (clickerMiceSet && clickerMiceSetAllowed)
			{
				Lighting.AddLight(player.position, 0.1f, 0.1f, 0.3f);
			}
			//if (clickerMiceSet && clickerMiceSetAllowed)
			//{
			//	Lighting.AddLight(player.position, 0.1f, 0.1f, 0.3f);
			//}

			//Acc
			//Cookie acc
			if ((clickerCookieAcc || clickerCookieAcc2) && clickerSelected)
			{
				clickerCookieAccTimer++;
				if (player.whoAmI == Main.myPlayer && clickerCookieAccTimer > 600)
				{
					int radius = (int)(95 * clickerRadius);
					if (radius > 350)
					{
						radius = 350;
					}

					//Circles give me a damn headache...
					double r = radius * Math.Sqrt(Main.rand.NextFloat(0f, 1f));
					double theta = Main.rand.NextFloat(0f, 1f) * 2 * 3.14;
					double xOffset = player.Center.X + r * Math.Cos(theta);
					double yOffset = player.Center.Y + r * Math.Sin(theta);

					if (clickerCookieAcc2 && Main.rand.NextFloat() <= 0.1f)
					{
						Projectile.NewProjectile((float)(xOffset), (float)(yOffset), 0f, 0f, mod.ProjectileType("CookiePro"), 0, 0f, player.whoAmI, 1f);
					}
					else
					{
						Projectile.NewProjectile((float)(xOffset), (float)(yOffset), 0f, 0f, mod.ProjectileType("CookiePro"), 0, 0f, player.whoAmI);
					}

					clickerCookieAccTimer = 0;
				}

				//Cookie Click
				if (player.whoAmI == Main.myPlayer)
				{
					for (int i = 0; i < 1000; i++)
					{
						Projectile cookieProjectile = Main.projectile[i];

						if (cookieProjectile.active && cookieProjectile.type == ModContent.ProjectileType<CookiePro>() && cookieProjectile.owner == player.whoAmI)
						{
							if (Main.mouseLeft && Main.mouseLeftRelease && Vector2.Distance(cookieProjectile.Center, Main.MouseWorld) < 30)
							{
								if (cookieProjectile.ai[0] == 1f)
								{
									Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 4);
									player.AddBuff(mod.BuffType("CookieBuff"), 600);
									player.HealLife(10);
									for (int k = 0; k < 10; k++)
									{
										Dust dust = Dust.NewDustDirect(cookieProjectile.Center, 20, 20, 87, Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f), 0, default, 1.15f);
										dust.noGravity = true;
									}
								}
								else
								{
									Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 2);
									player.AddBuff(mod.BuffType("CookieBuff"), 300);
									for (int k = 0; k < 10; k++)
									{
										Dust dust = Dust.NewDustDirect(cookieProjectile.Center, 20, 20, 0, Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f), 75, default, 1.5f);
										dust.noGravity = true;
									}
								}
								cookieProjectile.Kill();
							}
						}
					}
				}
			}

			//Milk acc
			if (clickerMilkAcc)
			{
				float bonusDamage = (float)(clickerPerSecond + 0.015f);
				if (bonusDamage >= 0.15f)
				{
					bonusDamage = 0.15f;
				}
				clickerDamage += bonusDamage;

				clickerPerSecondTimer++;
				if (clickerPerSecondTimer > 60)
				{
					clickerPerSecond = 0;
					clickerPerSecondTimer = 0;
				}
			}
			else
			{
				clickerPerSecondTimer = 0;
				clickerPerSecond = 0;
			}
		}

		public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			if (player.HeldItem.modItem is ClickerItem clickerItem && clickerItem.isClicker)
			{
				if (target.GetGlobalNPC<ClickerGlobalNPC>().embrittle)
				{
					damage += 8;
				}
			}
		}

		public override void OnHitNPCWithProj(Projectile projectile, NPC target, int damage, float knockback, bool crit)
		{

		}

		public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
		{

		}

		public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
		{

		}

		public override void ModifyDrawLayers(List<PlayerLayer> layers)
		{
			int index = layers.IndexOf(PlayerLayer.HeldItem);
			if (index != -1)
			{
				layers.Insert(index + 1, WeaponGlow);
			}
			index = layers.IndexOf(PlayerLayer.Head);
			if (index != -1)
			{
				layers.Insert(index + 1, HeadGlow);
			}
			index = layers.IndexOf(PlayerLayer.Legs);
			if (index != -1)
			{
				layers.Insert(index + 1, LegsGlow);
			}
			index = layers.IndexOf(PlayerLayer.Body);
			if (index != -1)
			{
				layers.Insert(index + 1, BodyGlow);
			}
			index = layers.IndexOf(PlayerLayer.Arms);
			if (index != -1)
			{
				layers.Insert(index + 1, ArmsGlow);
			}
			index = layers.IndexOf(PlayerLayer.MiscEffectsFront);
			if (index != -1)
			{
				layers.Insert(index + 1, MiscEffects);
			}

			WeaponGlow.visible = true;
			HeadGlow.visible = true;
			LegsGlow.visible = true;
			ArmsGlow.visible = true;
			MiscEffects.visible = true;
		}

		//Head
		public static readonly PlayerLayer HeadGlow = new PlayerLayer("ClickerClass", "HeadGlow", PlayerLayer.Head, delegate (PlayerDrawInfo drawInfo)
		{
			Player drawPlayer = drawInfo.drawPlayer;
			Mod mod = ModLoader.GetMod("ClickerClass");
			ClickerPlayer modPlayer = drawPlayer.GetModPlayer<ClickerPlayer>();
			Color color = drawPlayer.GetImmuneAlphaPure(Color.White, drawInfo.shadow);
			Texture2D texture = null;

			if (drawInfo.shadow != 0f || drawInfo.drawPlayer.invis)
			{
				return;
			}

			if (modPlayer.clickerMiceSet && modPlayer.clickerMiceSetAllowed)
			{
				texture = mod.GetTexture("Glowmasks/MiceMask_Glow");
			}
			if (modPlayer.clickerPrecursorSet && modPlayer.clickerPrecursorSetAllowed)
			{
				texture = mod.GetTexture("Glowmasks/PrecursorHelmet_Glow");
				color *= 0.5f;
			}
			if (modPlayer.clickerOverclockSet && modPlayer.clickerOverclockSetAllowed)
			{
				texture = mod.GetTexture("Glowmasks/OverclockHelmet_Glow");
				color *= 0.75f;
			}

			if (texture == null)
			{
				return;
			}

			Vector2 drawPos = drawInfo.position - Main.screenPosition + new Vector2(drawPlayer.width / 2 - drawPlayer.bodyFrame.Width / 2, drawPlayer.height - drawPlayer.bodyFrame.Height + 4f) + drawPlayer.headPosition;
			DrawData drawData = new DrawData(texture, drawPos.Floor() + drawInfo.headOrigin, drawPlayer.bodyFrame, color, drawPlayer.headRotation, drawInfo.headOrigin, 1f, drawInfo.spriteEffects, 0)
			{
				shader = drawInfo.headArmorShader
			};
			Main.playerDrawData.Add(drawData);
		});

		//Body
		public static readonly PlayerLayer BodyGlow = new PlayerLayer("ClickerClass", "BodyGlow", PlayerLayer.Body, delegate (PlayerDrawInfo drawInfo)
		{
			Player drawPlayer = drawInfo.drawPlayer;
			Mod mod = ModLoader.GetMod("ClickerClass");
			ClickerPlayer modPlayer = drawPlayer.GetModPlayer<ClickerPlayer>();
			Color color = drawPlayer.GetImmuneAlphaPure(Color.White, drawInfo.shadow);
			Texture2D texture = null;

			if (drawInfo.shadow != 0f || drawInfo.drawPlayer.invis)
			{
				return;
			}

			if (modPlayer.clickerMiceSet && modPlayer.clickerMiceSetAllowed)
			{
				if (drawPlayer.Male)
				{
					texture = mod.GetTexture("Glowmasks/MiceSuit_Glow");
				}
				else
				{
					texture = mod.GetTexture("Glowmasks/MiceSuitFemale_Glow");
				}
			}
			if (modPlayer.clickerPrecursorSet && modPlayer.clickerPrecursorSetAllowed)
			{
				if (drawPlayer.Male)
				{
					texture = mod.GetTexture("Glowmasks/PrecursorBreastplate_Glow");
					color *= 0.5f;
				}
				else
				{
					texture = mod.GetTexture("Glowmasks/PrecursorBreastplateFemale_Glow");
					color *= 0.5f;
				}
			}
			if (modPlayer.clickerOverclockSet && modPlayer.clickerOverclockSetAllowed)
			{
				if (drawPlayer.Male)
				{
					texture = mod.GetTexture("Glowmasks/OverclockSuit_Glow");
					color *= 0.75f;
				}
				else
				{
					texture = mod.GetTexture("Glowmasks/OverclockSuitFemale_Glow");
					color *= 0.75f;
				}
			}

			if (texture == null)
			{
				return;
			}

			Vector2 drawPos = drawInfo.position - Main.screenPosition + new Vector2(drawPlayer.width / 2 - drawPlayer.bodyFrame.Width / 2, drawPlayer.height - drawPlayer.bodyFrame.Height + 4f) + drawPlayer.bodyPosition;
			DrawData drawData = new DrawData(texture, drawPos.Floor() + drawPlayer.bodyFrame.Size() / 2, drawPlayer.bodyFrame, color, drawPlayer.bodyRotation, drawInfo.bodyOrigin, 1f, drawInfo.spriteEffects, 0)
			{
				shader = drawInfo.bodyArmorShader
			};
			Main.playerDrawData.Add(drawData);
		});

		//Arms
		public static readonly PlayerLayer ArmsGlow = new PlayerLayer("ClickerClass", "ArmsGlow", PlayerLayer.Arms, delegate (PlayerDrawInfo drawInfo)
		{
			Player drawPlayer = drawInfo.drawPlayer;
			Mod mod = ModLoader.GetMod("ClickerClass");
			ClickerPlayer modPlayer = drawPlayer.GetModPlayer<ClickerPlayer>();
			Color color = drawPlayer.GetImmuneAlphaPure(Color.White, drawInfo.shadow);
			Texture2D texture = null;

			if (drawInfo.shadow != 0f || drawInfo.drawPlayer.invis)
			{
				return;
			}

			if (modPlayer.clickerMiceSet && modPlayer.clickerMiceSetAllowed)
			{
				texture = mod.GetTexture("Glowmasks/MiceSuitArm_Glow");
			}
			if (modPlayer.clickerPrecursorSet && modPlayer.clickerPrecursorSetAllowed)
			{
				texture = mod.GetTexture("Glowmasks/PrecursorBreastplateArm_Glow");
				color *= 0.5f;
			}
			if (modPlayer.clickerOverclockSet && modPlayer.clickerOverclockSetAllowed)
			{
				texture = mod.GetTexture("Glowmasks/OverclockSuitArm_Glow");
				color *= 0.75f;
			}

			if (texture == null)
			{
				return;
			}

			Vector2 drawPos = drawInfo.position - Main.screenPosition + new Vector2(drawPlayer.width / 2 - drawPlayer.bodyFrame.Width / 2, drawPlayer.height - drawPlayer.bodyFrame.Height + 4f) + drawPlayer.bodyPosition;
			DrawData drawData = new DrawData(texture, drawPos.Floor() + drawPlayer.bodyFrame.Size() / 2, drawPlayer.bodyFrame, color, drawPlayer.bodyRotation, drawInfo.bodyOrigin, 1f, drawInfo.spriteEffects, 0)
			{
				shader = drawInfo.bodyArmorShader
			};
			Main.playerDrawData.Add(drawData);
		});

		//Legs
		public static readonly PlayerLayer LegsGlow = new PlayerLayer("ClickerClass", "LegsGlow", PlayerLayer.Legs, delegate (PlayerDrawInfo drawInfo)
		{
			Player drawPlayer = drawInfo.drawPlayer;
			Mod mod = ModLoader.GetMod("ClickerClass");
			ClickerPlayer modPlayer = drawPlayer.GetModPlayer<ClickerPlayer>();
			Color color = drawPlayer.GetImmuneAlphaPure(Color.White, drawInfo.shadow);
			Texture2D texture = null;

			if (drawInfo.shadow != 0f || drawInfo.drawPlayer.invis)
			{
				return;
			}

			if (modPlayer.clickerMiceSet && modPlayer.clickerMiceSetAllowed)
			{
				texture = mod.GetTexture("Glowmasks/MiceBoots_Glow");
			}
			if (modPlayer.clickerPrecursorSet && modPlayer.clickerPrecursorSetAllowed)
			{
				texture = mod.GetTexture("Glowmasks/PrecursorGreaves_Glow");
				color *= 0.5f;
			}
			if (modPlayer.clickerOverclockSet && modPlayer.clickerOverclockSetAllowed)
			{
				texture = mod.GetTexture("Glowmasks/OverclockBoots_Glow");
				color *= 0.75f;
			}

			if (texture == null)
			{
				return;
			}

			Vector2 drawPos = drawInfo.position - Main.screenPosition + new Vector2(drawPlayer.width / 2 - drawPlayer.legFrame.Width / 2, drawPlayer.height - drawPlayer.legFrame.Height + 4f) + drawPlayer.legPosition;
			DrawData drawData = new DrawData(texture, drawPos.Floor() + drawInfo.legOrigin, drawPlayer.legFrame, color, drawPlayer.legRotation, drawInfo.legOrigin, 1f, drawInfo.spriteEffects, 0)
			{
				shader = drawInfo.legArmorShader
			};
			Main.playerDrawData.Add(drawData);
		});

		public static readonly PlayerLayer WeaponGlow = new PlayerLayer("ClickerClass", "WeaponGlow", PlayerLayer.HeldItem, delegate (PlayerDrawInfo drawInfo)
		{
			Player drawPlayer = drawInfo.drawPlayer;
			Mod mod = ModLoader.GetMod("ClickerClass");
			ClickerPlayer modPlayer = drawPlayer.GetModPlayer<ClickerPlayer>();

			if (drawInfo.shadow != 0f || drawPlayer.dead || drawPlayer.frozen || drawPlayer.itemAnimation <= 0)
			{
				return;
			}

			//Fragment Pickaxe
			if (drawPlayer.HeldItem.type == mod.ItemType("MicePickaxe"))
			{
				Texture2D weaponGlow = mod.GetTexture("Glowmasks/MicePickaxe_Glow");
				Vector2 position = new Vector2((int)(drawInfo.itemLocation.X - Main.screenPosition.X), (int)(drawInfo.itemLocation.Y - Main.screenPosition.Y));
				Vector2 origin = new Vector2(drawPlayer.direction == -1 ? weaponGlow.Width : 0, drawPlayer.gravDir == -1 ? 0 : weaponGlow.Height);
				DrawData drawData = new DrawData(weaponGlow, position, null, new Color(255, 255, 255, 0) * 0.8f, drawPlayer.itemRotation, origin, drawPlayer.HeldItem.scale, drawInfo.spriteEffects, 0);
				Main.playerDrawData.Add(drawData);
			}

			//Fragment Hamaxe
			if (drawPlayer.HeldItem.type == mod.ItemType("MiceHamaxe"))
			{
				Texture2D weaponGlow = mod.GetTexture("Glowmasks/MiceHamaxe_Glow");
				Vector2 position = new Vector2((int)(drawInfo.itemLocation.X - Main.screenPosition.X), (int)(drawInfo.itemLocation.Y - Main.screenPosition.Y));
				Vector2 origin = new Vector2(drawPlayer.direction == -1 ? weaponGlow.Width : 0, drawPlayer.gravDir == -1 ? 0 : weaponGlow.Height);
				DrawData drawData = new DrawData(weaponGlow, position, null, new Color(255, 255, 255, 0) * 0.8f, drawPlayer.itemRotation, origin, drawPlayer.HeldItem.scale, drawInfo.spriteEffects, 0);
				Main.playerDrawData.Add(drawData);
			}
		});

		public static readonly PlayerLayer MiscEffects = new PlayerLayer("ClickerClass", "MiscEffects", PlayerLayer.MiscEffectsFront, delegate (PlayerDrawInfo drawInfo)
		{
			Mod mod = ModLoader.GetMod("ClickerClass");
			Player drawPlayer = drawInfo.drawPlayer;
			ClickerPlayer modPlayer = drawPlayer.GetModPlayer<ClickerPlayer>();

			if (drawInfo.shadow != 0f || drawPlayer.dead) return;

			if (Main.gameMenu) return;

			if (modPlayer.clickerSelected)
			{
				bool phaseCheck = false;
				if (drawPlayer.HeldItem.modItem is ClickerItem clickerItem && clickerItem.isClicker)
				{
					if (clickerItem.itemClickerEffect.Contains("Phase Reach"))
					{
						phaseCheck = true;
					}
				}

				if (!phaseCheck)
				{
					if (modPlayer.clickerMechSet && modPlayer.clickerMechSetRatio > 0)
					{
						float glow = modPlayer.clickerInRangeMech ? 0.6f : 0f;

						Color outer = modPlayer.clickerColor * (0.2f + glow);
						int drawX = (int)(drawPlayer.Center.X - Main.screenPosition.X);
						int drawY = (int)(drawPlayer.Center.Y + drawPlayer.gfxOffY - Main.screenPosition.Y);
						Vector2 center = new Vector2(drawX, drawY);
						Vector2 drawPos = center + modPlayer.CalculateMechPosition().Floor();

						Texture2D texture = mod.GetTexture("Glowmasks/MechanicalSetBonus_Glow");
						DrawData drawData = new DrawData(texture, drawPos, null, Color.White, 0f, texture.Size() / 2, 1f, SpriteEffects.None, 0)
						{
							ignorePlayerRotation = true
						};
						Main.playerDrawData.Add(drawData);
					}
				}
			}
		});
	}
}