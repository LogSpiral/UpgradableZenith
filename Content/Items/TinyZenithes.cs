using Microsoft.Build.Execution;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using static UpgradableZenith.Content.Items.TinyZenith;

namespace UpgradableZenith.Content.Items
{
    public abstract class TinyZenith : ModItem
    {



        public struct ZenithInfo
        {

            public int shownItemID;           //4956//这个是原版天顶的物品id
            public int damage;                //190//原版天顶的物品伤害
            public int crit;                  //10暴击，10%
            public float knockBack;           //6.5f击退
            //public float shootSpeed;          //16f射速，但是实际情况更复杂一些
            public int value;                 //20金
            public int rare;                  //10
            public int useAnimation;          //30使用动画时间

            public int[] projectileItemTypes;
            public bool combined;
            public ZenithInfo(int shownItemID, int damage, int crit, float knockBack, int value, int rare, int useAnimation, int[] projectileItemTypes, bool combined = false)
            {
                this.shownItemID = shownItemID;
                this.damage = damage;
                this.crit = crit;
                this.knockBack = knockBack;
                this.value = value;
                this.rare = rare;
                this.useAnimation = useAnimation;
                this.projectileItemTypes = projectileItemTypes;
                this.combined = combined;
            }
        }
        public abstract ZenithInfo Info { get; }
        public override string Texture => $"Terraria/Images/Item_{ItemID.Zenith}";
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            //spriteBatch.Draw(TextureAssets.MagicPixel.Value, position, new Rectangle(0, 0, 1, 1), Main.DiscoColor, 0, new Vector2(.5f), 4, 0, 0);
            var info = Info;
            foreach (int showId in info.combined ? info.projectileItemTypes : [info.shownItemID])
            {
                Color color = itemColor;
                Rectangle rect = frame;
                //int showId = info.shownItemID;
                //if (info.combined) showId = Main.rand.Next(info.projectileItemTypes);
                bool inRecipe = origin == default;
                ItemSlot.DrawItem_GetColorAndScale(new Item(showId), inRecipe ? 1 : Main.inventoryScale, ref color, 32f, ref rect, out var itemLight, out var finalDrawScale);

                spriteBatch.Draw(TextureAssets.Item[showId].Value, position + (inRecipe ? new Vector2(28 * scale) : default), null, drawColor, 0, TextureAssets.Item[showId].Size() * .5f, finalDrawScale, 0, 0);
                //spriteBatch.DrawString(FontAssets.MouseText.Value, origin.ToString(), position, Color.Red);
                //spriteBatch.Draw(TextureAssets.Item[showId].Value, position, null, drawColor, 0, origin, scale, 0, 0);

            }
            //spriteBatch.DrawString(FontAssets.MouseText.Value, origin.ToString(), position, Main.DiscoColor);
            float angle = UpgradableZenithSystem.Timer / 30f;
            float r = (float)Math.Cos(angle) + 1;
            float l = (float)Math.Sin(angle * MathHelper.E / 2f) + 1.5f;
            l *= .67f;
            for (int n = 0; n < 3; n++)
            {
                spriteBatch.Draw(TextureAssets.Item[ItemID.Zenith].Value, position + (MathHelper.TwoPi / 3 * n + angle).ToRotationVector2() * r, frame, drawColor with { A = 0 } * .5f * .33f * l, 0, origin, scale, 0, 0);

            }
            return false;
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            var info = Info;

            foreach (int showId in info.combined ? info.projectileItemTypes : [info.shownItemID])
            {

                //int showId = Info.shownItemID;
                //if (Info.combined) showId = Main.rand.Next(Info.projectileItemTypes);
                spriteBatch.Draw(TextureAssets.Item[showId].Value, Item.Center - Main.screenPosition, null, lightColor, rotation, TextureAssets.Item[showId].Size() * .5f, scale, 0, 0);

            }
            float angle = UpgradableZenithSystem.Timer / 30f;
            float r = (float)Math.Cos(angle) + 1;
            float l = (float)Math.Sin(angle * MathHelper.E / 2f) + 1.5f;
            l *= .67f;
            for (int n = 0; n < 3; n++)
            {
                spriteBatch.Draw(TextureAssets.Item[ItemID.Zenith].Value, Item.Center - Main.screenPosition + (MathHelper.TwoPi / 3 * n + angle).ToRotationVector2() * r, null, lightColor with { A = 0 } * .5f * .33f * l, rotation, new Vector2(56) * .5f, scale, 0, 0);

            }
            return false;
        }
        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.width = 56;
            Item.height = 56;
            Item.UseSound = SoundID.Item169;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Melee;
            Item.shoot = ProjectileID.FinalFractal;
            Item.shootSpeed = 16f;
            Item.knockBack = 6.5f;
            Item.value = Item.sellPrice(0, 20, 0, 0);
            Item.crit = 10;
            Item.noUseGraphic = true;
            Item.noMelee = true;

            //这下总对了((

            ZenithInfo info = Info;
            Item.damage = info.damage;
            Item.crit = info.crit;
            Item.knockBack = info.knockBack;
            //Item.shootSpeed = info.shootSpeed;
            Item.rare = info.rare;
            Item.useAnimation = /*info.useAnimation;*/
            Item.useTime = info.useAnimation / 3;//使用效果时间是使用动画时间的1/3,如果会发射弹幕，就会发射三次

            if (Item.useAnimation == 0)
                Item.useAnimation = Item.useTime = 30;
        }

        public static bool GetZenithTarget(Vector2 searchCenter, float maxDistance, Player player, out int npcTargetIndex)
        {
            npcTargetIndex = 0;
            int? num = null;
            float num2 = maxDistance;
            for (int i = 0; i < 200; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy(player, false))
                {
                    float num3 = Vector2.Distance(searchCenter, npc.Center);
                    if (num2 > num3)
                    {
                        num = new int?(i);
                        num2 = num3;
                    }
                }
            }
            if (num == null)
            {
                return false;
            }
            npcTargetIndex = num.Value;
            return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var info = Info;
            if (info.projectileItemTypes == null || info.projectileItemTypes.Length == 0)
            {
                Main.NewText("发射列表不存在");
                return false;
            }
            FinalFractalHelper finalFractalHelper = new FinalFractalHelper();
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, true, true);
            float num6 = Main.mouseX + Main.screenPosition.X - vector.X;
            float num7 = Main.mouseY + Main.screenPosition.Y - vector.Y;
            int num166 = (player.itemAnimationMax - player.itemAnimation) / player.itemTime;
            Vector2 velocity_ = new Vector2(num6, num7);
            var vec = Main.MouseWorld;
            player.LimitPointToPlayerReachableArea(ref vec);
            Vector2 value7 = vec - player.MountedCenter;
            int num168;
            bool zenithTarget = GetZenithTarget(Main.MouseWorld, 400f, player, out num168);
            if (zenithTarget)
            {
                value7 = Main.npc[num168].Center - player.MountedCenter;
            }
            if (!Main.rand.NextBool(3))
                value7 += Main.rand.NextVector2Circular(150f, 150f);
            velocity_ = value7 / 2f;
            float ai5 = Main.rand.Next(-100, 101);//
            //if(player.ownedProjectileCounts[type] < 1)
            var proj = Projectile.NewProjectileDirect(source, player.Center, velocity_, type, damage, knockback, player.whoAmI, ai5, Main.rand.Next(info.projectileItemTypes));
            proj.frame = Main.rand.Next(26);
            proj.netUpdate = true;
            proj.netUpdate2 = true;
            proj.ArmorPenetration = 10;
            return false;
        }

        public void CreateTZRecipe(Dictionary<int, int> ingredients, Dictionary<string, int> RGs, KeyValuePair<int, int>? extraIngredients, int? tileID)
        {
            bool flag = extraIngredients != null;

            Recipe recipe = CreateRecipe();
            if (ingredients != null)
                foreach (var pair in ingredients)
                {
                    recipe.AddIngredient(pair.Key, pair.Value);
                }
            if (RGs != null)
                foreach (var pair in RGs)
                {
                    recipe.AddRecipeGroup(pair.Key, pair.Value);
                }
            if (tileID != null)
                recipe.AddTile(tileID.Value);
            if (flag)
                recipe.AddCondition(Condition.ForTheWorthyWorld);
            recipe.Register();
            if (flag)
            {
                recipe = recipe.Clone();
                recipe.RemoveCondition(Condition.ForTheWorthyWorld);
                recipe.AddCondition(Condition.NotForTheWorthy);
                recipe.AddIngredient(extraIngredients.Value.Key, extraIngredients.Value.Value);
                recipe.Register();
            }

        }

        public void CreateTZRecipe(Dictionary<int, int> ingredients, Dictionary<string, int> RGs, Dictionary<int, int> extraIngredients, int? tileID)
        {
            bool flag = extraIngredients != null;

            Recipe recipe = CreateRecipe();
            if (ingredients != null)
                foreach (var pair in ingredients)
                {
                    recipe.AddIngredient(pair.Key, pair.Value);
                }
            if (RGs != null)
                foreach (var pair in RGs)
                {
                    recipe.AddRecipeGroup(pair.Key, pair.Value);
                }
            if (tileID != null)
                recipe.AddTile(tileID.Value);
            if (flag)
                recipe.AddCondition(Condition.ForTheWorthyWorld);
            recipe.Register();
            if (flag)
            {
                recipe = recipe.Clone();
                recipe.RemoveCondition(Condition.ForTheWorthyWorld);
                recipe.AddCondition(Condition.NotForTheWorthy);
                if (extraIngredients != null)
                    foreach (var pair in extraIngredients)
                    {
                        recipe.AddIngredient(pair.Key, pair.Value);
                    }
                recipe.Register();
            }

        }
    }

    public class TerragrimZenith : TinyZenith
    {
        public override ZenithInfo Info => new(ItemID.Terragrim, 15, 4, 3, Item.sellPrice(0, 5), ItemRarityID.Green, 150, [ItemID.Terragrim]);
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.Terragrim).Register();
            base.AddRecipes();
        }
    }

    #region 星魔系
    public class CopperZenith : TinyZenith
    {
        public override ZenithInfo Info => new(ItemID.CopperShortsword, 1, 4, 4, Item.sellPrice(0, 0, 0, 70), 0, 180, [ItemID.CopperShortsword]);

        public override void AddRecipes()
        {
            //Recipe recipe = CreateRecipe();
            //recipe.AddIngredient(ItemID.CopperShortsword);
            //recipe.AddTile(TileID.WorkBenches);
            //recipe.AddCondition(Condition.ForTheWorthyWorld);
            ////recipe.ConsumeItemHooks += (Recipe recipe, int type, ref int amount) =>
            ////{
            ////    if (type == ItemID.FallenStar && Condition.ForTheWorthyWorld.IsMet())
            ////        amount = 0;
            ////};
            //recipe.Register();
            //recipe = recipe.Clone();
            //recipe.AddIngredient(ItemID.FallenStar);
            //recipe.AddCondition(Condition.NotForTheWorthy);
            //recipe.RemoveCondition(Condition.ForTheWorthyWorld);
            //recipe.AddIngredient(ItemID.FallenStar);
            //recipe.Register();

            CreateTZRecipe
                (
                    new() { { ItemID.CopperShortsword, 1 } },
                    null,
                    new KeyValuePair<int, int>(ItemID.FallenStar, 1),
                    TileID.WorkBenches
                );

        }
    }
    public class StarfuryZenith : TinyZenith
    {
        public override ZenithInfo Info => new(ItemID.Starfury, 18, 4, 5, Item.sellPrice(0, 1), 1, 165, [ItemID.Starfury]);
        public override void AddRecipes()
        {
            //CreateRecipe().AddIngredient(ItemID.Starfury).AddIngredient(ItemID.FallenStar, 49).AddTile(TileID.SkyMill).Register();

            CreateTZRecipe
                (
                    new() { { ItemID.Starfury, 1 } },
                    null,
                    new KeyValuePair<int, int>(ItemID.FallenStar, 49),
                    TileID.SkyMill
                );

            base.AddRecipes();
        }
    }
    public class EnchantedZenith : TinyZenith
    {
        public override ZenithInfo Info => new(ItemID.EnchantedSword, 14, 4, 4.25f, Item.sellPrice(0, 3), 1, 165, [ItemID.EnchantedSword]);
        public override void AddRecipes()
        {
            //CreateRecipe().AddIngredient(ItemID.EnchantedSword).AddIngredient(ItemID.ManaPotion, 10).AddTile(TileID.Books).Register();

            CreateTZRecipe
            (
               new() { { ItemID.EnchantedSword, 1 } },
                null,
             new KeyValuePair<int, int>(ItemID.ManaPotion, 10),
             TileID.Books
             );

            base.AddRecipes();
        }
    }
    public class EnchantedStarZenith : TinyZenith
    {
        public override ZenithInfo Info => new(ItemID.Starfury, 20, 4, 5, Item.sellPrice(0, 5), 2, 150, [ItemID.Starfury, ItemID.EnchantedSword, ItemID.Starfury, ItemID.EnchantedSword, ItemID.CopperShortsword], true);
        public override void AddRecipes()
        {

            CreateTZRecipe
            (
                null,
                new() { { RecipeGroupSystem.CopperShortSwordRG, 1 }, { RecipeGroupSystem.EnchantedSwordRG, 1 }, { RecipeGroupSystem.StarfuryRG, 1 } },
                new KeyValuePair<int, int>(ItemID.ManaCrystal, 1),
                TileID.Anvils
            );

            //Recipe recipe = CreateRecipe();
            //recipe.AddRecipeGroup(RecipeGroupSystem.CopperShortSwordRG);
            //recipe.AddRecipeGroup(RecipeGroupSystem.EnchantedSwordRG);
            //recipe.AddRecipeGroup(RecipeGroupSystem.StarfuryRG);
            //recipe.AddIngredient(ItemID.ManaCrystal);
            //recipe.AddTile(TileID.Anvils);
            //recipe.Register();

        }
    }
    #endregion
    #region 永夜系
    public class LightsBaneZenith : TinyZenith
    {
        public override ZenithInfo Info => new(ItemID.LightsBane, 18, 4, 5, Item.sellPrice(0, 0, 27), ItemRarityID.Blue, 135, [ItemID.LightsBane]);
        public override void AddRecipes()
        {
            CreateTZRecipe
            (
                 new() { { ItemID.LightsBane, 1 } },
                 null,
                 new KeyValuePair<int, int>(ItemID.ShadowScale, 30),
                 TileID.DemonAltar
             );
            //CreateRecipe().AddIngredient(ItemID.LightsBane).AddIngredient(ItemID.ShadowScale, 30).AddTile(TileID.DemonAltar).Register();
            base.AddRecipes();
        }
    }
    public class BloodButchererZenith : TinyZenith
    {
        public override ZenithInfo Info => new(ItemID.BloodButcherer, 24, 4, 5, Item.sellPrice(0, 0, 27), ItemRarityID.Blue, 165, [ItemID.BloodButcherer]);
        public override void AddRecipes()
        {
            CreateTZRecipe
            (
                 new() { { ItemID.BloodButcherer, 1 } },
                 null,
                 new KeyValuePair<int, int>(ItemID.TissueSample, 30),
                 TileID.DemonAltar
             );
            //CreateRecipe().AddIngredient(ItemID.BloodButcherer).AddIngredient(ItemID.TissueSample, 30).AddTile(TileID.DemonAltar).Register();
            base.AddRecipes();
        }
    }
    public class BladeOfGrassZenith : TinyZenith
    {
        public override ZenithInfo Info => new(ItemID.BladeofGrass, 16, 4, 4.5f, Item.sellPrice(0, 0, 54), ItemRarityID.Orange, 165, [ItemID.BladeofGrass]);
        public override void AddRecipes()
        {
            CreateTZRecipe
            (
                 new() { { ItemID.BladeofGrass, 1 } },
                 null,
                 new KeyValuePair<int, int>(ItemID.LifeCrystal, 3),
                 TileID.LivingLoom
             );
            //CreateRecipe().AddIngredient(ItemID.BladeofGrass).AddIngredient(ItemID.LifeCrystal, 3).AddTile(TileID.LivingLoom).Register();
            base.AddRecipes();
        }
    }
    public class MuramasaZenith : TinyZenith
    {
        public override ZenithInfo Info => new(ItemID.Muramasa, 24, 4, 3, Item.sellPrice(0, 1, 75), ItemRarityID.Green, 135, [ItemID.Muramasa]);
        public override void AddRecipes()
        {
            CreateTZRecipe
            (
                 new() { { ItemID.Muramasa, 1 } },
                 null,
                 new KeyValuePair<int, int>(ItemID.Bone, 500),
                 TileID.BoneWelder
             );
            //CreateRecipe().AddIngredient(ItemID.Muramasa).AddIngredient(ItemID.Bone, 500).AddTile(TileID.BoneWelder).Register();
            base.AddRecipes();
        }
    }
    public class FieryGreatZenith : TinyZenith
    {
        public override ZenithInfo Info => new(ItemID.FieryGreatsword, 25, 4, 6.5f, Item.sellPrice(0, 0, 54), ItemRarityID.Orange, 180, [ItemID.FieryGreatsword]);
        public override void AddRecipes()
        {
            CreateTZRecipe
            (
                 new() { { ItemID.FieryGreatsword, 1 } },
                 null,
                 new KeyValuePair<int, int>(ItemID.LavaBomb, 50),
                 TileID.Hellforge
             );
            //CreateRecipe().AddIngredient(ItemID.FieryGreatsword).AddIngredient(ItemID.LavaBomb, 50).AddTile(TileID.Hellforge).Register();
            base.AddRecipes();
        }
    }
    public class BeeKeeperZenith : TinyZenith
    {
        public override ZenithInfo Info => new(ItemID.BeeKeeper, 26, 4, 5.3f, Item.sellPrice(0, 2), ItemRarityID.Orange, 135, [ItemID.BeeKeeper]);
        public override void AddRecipes()
        {
            CreateTZRecipe
            (
                 new() { { ItemID.BeeKeeper, 1 } },
                 null,
                 new KeyValuePair<int, int>(ItemID.HoneyComb, 1),
                 TileID.HoneyDispenser
             );
            //CreateRecipe().AddIngredient(ItemID.BeeKeeper).AddIngredient(ItemID.BeeWax).AddTile(TileID.Anvils).Register();
            base.AddRecipes();
        }
    }
    public class PermanentNightZenith : TinyZenith
    {
        public override ZenithInfo Info => new(ItemID.NightsEdge, 35, 4, 4.5f, Item.sellPrice(0, 4), ItemRarityID.Orange, 120, [ItemID.LightsBane, ItemID.BloodButcherer, ItemID.Muramasa, ItemID.FieryGreatsword, ItemID.BladeofGrass, ItemID.BeeKeeper, ItemID.NightsEdge, ItemID.NightsEdge, ItemID.NightsEdge, ItemID.NightsEdge, ItemID.NightsEdge]);//
        public override void AddRecipes()
        {

            CreateTZRecipe
            (
                 new() { { ItemID.NightsEdge, 1 } },
                 null,
                 new KeyValuePair<int, int>(ItemID.Obsidian, 100),
                 TileID.DemonAltar
             );

            CreateTZRecipe
            (
                null,
                new() { { RecipeGroupSystem.LightBaneRG, 1 }, { RecipeGroupSystem.BladeOfGrassRG, 1 }, { RecipeGroupSystem.MuramasaRG, 1 }, { RecipeGroupSystem.FieryGreatSwordRG, 1 }, { RecipeGroupSystem.BeeKeeperRG, 1 } },
                new KeyValuePair<int, int>(ItemID.Obsidian, 100),
                TileID.DemonAltar
            );

            CreateTZRecipe
            (
                null,
                new() { { RecipeGroupSystem.BloodButchererRG, 1 }, { RecipeGroupSystem.BladeOfGrassRG, 1 }, { RecipeGroupSystem.MuramasaRG, 1 }, { RecipeGroupSystem.FieryGreatSwordRG, 1 }, { RecipeGroupSystem.BeeKeeperRG, 1 } },
                new KeyValuePair<int, int>(ItemID.Obsidian, 100),
                TileID.DemonAltar
            );
            /*
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.NightsEdge);
            recipe.AddRecipeGroup(RecipeGroupSystem.BeeKeeperRG);
            recipe.AddIngredient(ItemID.Obsidian, 100);
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();

            recipe = CreateRecipe();
            recipe.AddRecipeGroup(RecipeGroupSystem.LightBaneRG);
            recipe.AddRecipeGroup(RecipeGroupSystem.BladeOfGrassRG);
            recipe.AddRecipeGroup(RecipeGroupSystem.MuramasaRG);
            recipe.AddRecipeGroup(RecipeGroupSystem.FieryGreatSwordRG);
            recipe.AddRecipeGroup(RecipeGroupSystem.BeeKeeperRG);
            recipe.AddIngredient(ItemID.Obsidian, 100);
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();


            recipe = CreateRecipe();
            recipe.AddRecipeGroup(RecipeGroupSystem.BloodButchererRG);
            recipe.AddRecipeGroup(RecipeGroupSystem.BladeOfGrassRG);
            recipe.AddRecipeGroup(RecipeGroupSystem.MuramasaRG);
            recipe.AddRecipeGroup(RecipeGroupSystem.FieryGreatSwordRG);
            recipe.AddRecipeGroup(RecipeGroupSystem.BeeKeeperRG);
            recipe.AddIngredient(ItemID.Obsidian, 100);
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();*/
        }
    }
    #endregion

    #region 泰拉系
    public class ExcaliburZenith : TinyZenith
    {
        public override ZenithInfo Info => new(ItemID.Excalibur, 55, 4, 4.5f, Item.sellPrice(0, 4, 60), ItemRarityID.Pink, 90, [ItemID.Excalibur]);
        public override void AddRecipes()
        {

            CreateTZRecipe
            (
                 new() { { ItemID.Excalibur, 1 } },
                 null,
                 new Dictionary<int, int>() { { ItemID.SoulofFlight, 1 }, { ItemID.SoulofLight, 1 }, { ItemID.SoulofNight, 1 }, { ItemID.SoulofMight, 1 }, { ItemID.SoulofFright, 1 }, { ItemID.SoulofSight, 1 } },
                 TileID.MythrilAnvil
             );
            //Recipe recipe = CreateRecipe();
            //recipe.AddIngredient(ItemID.Excalibur);
            //recipe.AddIngredient(ItemID.SoulofFlight, 1);
            //recipe.AddIngredient(ItemID.SoulofLight, 1);
            //recipe.AddIngredient(ItemID.SoulofNight, 1);
            //recipe.AddIngredient(ItemID.SoulofMight, 1);
            //recipe.AddIngredient(ItemID.SoulofFright, 1);
            //recipe.AddIngredient(ItemID.SoulofSight, 1);
            //recipe.AddTile(TileID.MythrilAnvil);
            //recipe.Register();
            base.AddRecipes();
        }
    }
    public class TrueExcaliburZenith : TinyZenith
    {
        public override ZenithInfo Info => new(ItemID.TrueExcalibur, 70, 4, 4.5f, Item.sellPrice(0, 10), ItemRarityID.Yellow, 75, [ItemID.Excalibur, ItemID.TrueExcalibur]);
        public override void AddRecipes()
        {
            CreateTZRecipe
            (
                 new() { { ItemID.ChlorophyteBar, 24 } },
                 new() { { RecipeGroupSystem.ExcaliburRG, 1 } },
                 new KeyValuePair<int, int>(ItemID.BrokenHeroSword, 1),
                 TileID.MythrilAnvil
             );
            CreateTZRecipe
            (
                new() { { ItemID.TrueExcalibur, 1 } },
                null,
                new KeyValuePair<int, int>(ItemID.BrokenHeroSword, 1),
                TileID.MythrilAnvil
            );
            //CreateRecipe().AddRecipeGroup(RecipeGroupSystem.ExcaliburRG).AddIngredient(ItemID.BrokenHeroSword).AddTile(TileID.MythrilAnvil).Register();
            base.AddRecipes();
        }
    }
    public class TrueNightsEdgeZenith : TinyZenith
    {
        public override ZenithInfo Info => new(ItemID.TrueNightsEdge, 75, 4, 4.75f, Item.sellPrice(0, 10), ItemRarityID.Yellow, 120, [ItemID.LightsBane, ItemID.BloodButcherer, ItemID.Muramasa, ItemID.FieryGreatsword, ItemID.BladeofGrass, ItemID.BeeKeeper, ItemID.NightsEdge, ItemID.TrueNightsEdge]);
        public override void AddRecipes()
        {

            CreateTZRecipe
            (
                 new() { { ItemID.SoulofMight, 20 }, { ItemID.SoulofFright, 20 }, { ItemID.SoulofSight, 20 } },
                 new() { { RecipeGroupSystem.NightsEdgeRG, 1 } },
                 new KeyValuePair<int, int>(ItemID.BrokenHeroSword, 1),
                 TileID.MythrilAnvil
             );
            CreateTZRecipe
            (
                 new() { { ItemID.TrueNightsEdge, 1 } },
                 null,
                 new KeyValuePair<int, int>(ItemID.BrokenHeroSword, 1),
                 TileID.MythrilAnvil
             );
            //CreateRecipe().AddRecipeGroup(RecipeGroupSystem.NightsEdgeRG).AddIngredient(ItemID.BrokenHeroSword).AddTile(TileID.MythrilAnvil).Register();
            base.AddRecipes();
        }
    }
    public class TheHorseManZenith : TinyZenith
    {
        public override ZenithInfo Info => new(ItemID.TheHorsemansBlade, 100, 4, 7.5f, Item.sellPrice(0, 10), ItemRarityID.Yellow, 75, [ItemID.TheHorsemansBlade]);
        public override void AddRecipes()
        {
            CreateTZRecipe
            (
                 new() { { ItemID.TheHorsemansBlade, 1 } },
                 null,
                 new KeyValuePair<int, int>(ItemID.SpookyWood, 500),
                 TileID.MythrilAnvil
             );
            //CreateRecipe().AddIngredient(ItemID.TheHorsemansBlade).AddIngredient(ItemID.SpookyWood, 500).AddTile(TileID.MythrilAnvil).Register();
            base.AddRecipes();
        }
    }
    public class SeedlerZenith : TinyZenith
    {
        public override ZenithInfo Info => new(ItemID.Seedler, 85, 4, 6, Item.sellPrice(0, 10), ItemRarityID.Yellow, 75, [ItemID.Seedler]);
        public override void AddRecipes()
        {
            CreateTZRecipe
            (
                 new() { { ItemID.Seedler, 1 } },
                 null,
                 new KeyValuePair<int, int>(ItemID.ChlorophyteBar, 50),
                 TileID.MythrilAnvil
             );
            //CreateRecipe().AddIngredient(ItemID.Seedler).AddIngredient(ItemID.ChlorophyteBar, 50).AddTile(TileID.MythrilAnvil).Register();
            base.AddRecipes();
        }
    }
    public class TerraZentih : TinyZenith
    {
        public override ZenithInfo Info => new(ItemID.TerraBlade, 120, 4, 6.5f, Item.sellPrice(0, 40), ItemRarityID.Yellow, 60,
            [ItemID.LightsBane, ItemID.BloodButcherer, ItemID.Muramasa, ItemID.FieryGreatsword, ItemID.BladeofGrass, ItemID.BeeKeeper, ItemID.NightsEdge,
            ItemID.Excalibur,ItemID.TrueNightsEdge,ItemID.TrueNightsEdge,ItemID.Seedler,ItemID.TheHorsemansBlade,ItemID.TerraBlade]
        );
        public override void AddRecipes()
        {
            CreateTZRecipe
            (
                 new() { { ItemID.TerraBlade, 1 } },
                 new() { { RecipeGroupSystem.TheHorseManRG, 1 }, { RecipeGroupSystem.SeedlerRG, 1 } },
                 new KeyValuePair<int, int>(ItemID.BrokenHeroSword, 1),
                 TileID.MythrilAnvil
             );
            CreateTZRecipe
            (
                 new() { { ItemID.BrokenHeroSword, 1 } },
                 new() { { RecipeGroupSystem.TrueExcaliburRG, 1 }, { RecipeGroupSystem.TrueNightsEdgeRG, 1 }, { RecipeGroupSystem.TheHorseManRG, 1 }, { RecipeGroupSystem.SeedlerRG, 1 } },
                 new KeyValuePair<int, int>(ItemID.BrokenHeroSword, 1),
                 TileID.MythrilAnvil
             );
            //Recipe recipe = CreateRecipe();
            //recipe.AddIngredient(ItemID.TerraBlade);
            //recipe.AddRecipeGroup(RecipeGroupSystem.TheHorseManRG);
            //recipe.AddRecipeGroup(RecipeGroupSystem.SeedlerRG);
            //recipe.AddIngredient(ItemID.BrokenHeroSword);
            //recipe.AddTile(TileID.MythrilAnvil);
            //recipe.Register();

            //recipe = CreateRecipe();
            //recipe.AddIngredient(ItemID.BrokenHeroSword, 2);
            //recipe.AddRecipeGroup(RecipeGroupSystem.TheHorseManRG);
            //recipe.AddRecipeGroup(RecipeGroupSystem.SeedlerRG);
            //recipe.AddRecipeGroup(RecipeGroupSystem.TrueExcaliburRG);
            //recipe.AddRecipeGroup(RecipeGroupSystem.TrueNightsEdgeRG);
            //recipe.AddTile(TileID.MythrilAnvil);
            //recipe.Register();

            /*recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.TrueExcalibur);
            recipe.AddIngredient(ItemID.SoulofMight, 20);
            recipe.AddIngredient(ItemID.SoulofFright, 20);
            recipe.AddIngredient(ItemID.SoulofSight, 20);
            recipe.AddIngredient<PermanentNightZenith>();
            recipe.AddIngredient(ItemID.TheHorsemansBlade);
            recipe.AddIngredient(ItemID.Seedler);
            recipe.AddIngredient(ItemID.BrokenHeroSword, 2);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();

            recipe = CreateRecipe();
            recipe.AddIngredient<ExcaliburZenith>();
            recipe.AddIngredient(ItemID.ChlorophyteBar, 24);
            recipe.AddIngredient(ItemID.SoulofMight, 20);
            recipe.AddIngredient(ItemID.SoulofFright, 20);
            recipe.AddIngredient(ItemID.SoulofSight, 20);
            recipe.AddIngredient<PermanentNightZenith>();
            recipe.AddIngredient(ItemID.TheHorsemansBlade);
            recipe.AddIngredient(ItemID.Seedler);
            recipe.AddIngredient(ItemID.BrokenHeroSword, 2);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();

            recipe = CreateRecipe();
            recipe.AddIngredient<ExcaliburZenith>();
            recipe.AddIngredient(ItemID.ChlorophyteBar, 24);
            recipe.AddIngredient(ItemID.TrueNightsEdge);
            recipe.AddIngredient(ItemID.TheHorsemansBlade);
            recipe.AddIngredient(ItemID.Seedler);
            recipe.AddIngredient(ItemID.BrokenHeroSword, 2);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();*/

            base.AddRecipes();
        }
    }
    #endregion

    #region 顶天系
    public class MeowmereZenith : TinyZenith
    {
        public override ZenithInfo Info => new(ItemID.Meowmere, 175, 4, 6.5f, Item.sellPrice(0, 20), ItemRarityID.Cyan, 45, [ItemID.Meowmere]);
        public override void AddRecipes()
        {
            CreateTZRecipe
            (
                 new() { { ItemID.Meowmere, 1 } },
                 null,
                 new KeyValuePair<int, int>(ItemID.RainbowBrick, 30),
                 TileID.LunarCraftingStation
             );
            //CreateRecipe().AddIngredient(ItemID.Meowmere).AddIngredient(ItemID.RainbowBrick, 30).AddTile(TileID.LunarCraftingStation).Register();
            base.AddRecipes();
        }
    }
    public class StarwrathZenith : TinyZenith
    {
        public override ZenithInfo Info => new(ItemID.StarWrath, 125, 4, 6.5f, Item.sellPrice(0, 20), ItemRarityID.Cyan, 60, [ItemID.StarWrath]);
        public override void AddRecipes()
        {
            CreateTZRecipe
            (
                 new() { { ItemID.StarWrath, 1 } },
                 null,
                 new Dictionary<int, int>() { { ItemID.FragmentSolar, 5 }, { ItemID.FragmentNebula, 5 }, { ItemID.FragmentVortex, 5 }, { ItemID.FragmentStardust, 5 }, { ItemID.LunarBar, 5 } },
                 TileID.LunarCraftingStation
             );
            //CreateRecipe().AddIngredient(ItemID.StarWrath).AddIngredient(ItemID.FragmentSolar, 5).AddIngredient(ItemID.FragmentNebula, 5).AddIngredient(ItemID.FragmentVortex, 5).AddIngredient(ItemID.FragmentStardust, 5).AddIngredient(ItemID.LunarBar, 5).AddTile(TileID.LunarCraftingStation).Register();
            base.AddRecipes();
        }
    }
    public class InfluexWaverZenith : TinyZenith
    {
        public override ZenithInfo Info => new(ItemID.InfluxWaver, 100, 4, 4.5f, Item.sellPrice(0, 10), ItemRarityID.Cyan, 30, [ItemID.InfluxWaver]);
        public override void AddRecipes()
        {
            CreateTZRecipe
            (
                 new() { { ItemID.InfluxWaver, 1 } },
                 null,
                 new KeyValuePair<int, int>(ItemID.MartianConduitPlating, 100),
                 TileID.LunarCraftingStation
             );
            //CreateRecipe().AddIngredient(ItemID.InfluxWaver).AddIngredient(ItemID.MartianConduitPlating, 100).AddTile(TileID.LunarCraftingStation).Register();
            base.AddRecipes();
        }
    }
    public class Henitz : TinyZenith
    {
        public override ZenithInfo Info => new(ItemID.InfluxWaver, 150, 8, 7.5f, Item.sellPrice(0, 50), ItemRarityID.Cyan, 45,
            [ItemID.InfluxWaver, ItemID.Meowmere, ItemID.StarWrath], true
        );
        public override void AddRecipes()
        {
            //CreateTZRecipe
            //(
            //     null,
            //     new() { { RecipeGroupSystem.MeowmereRG, 1 }, { RecipeGroupSystem.StarWrathRG, 1 }, { RecipeGroupSystem.InfluexWaverRG, 1 } },
            //     [],
            //     TileID.LunarCraftingStation
            // );
            Recipe recipe = CreateRecipe();
            recipe.AddRecipeGroup(RecipeGroupSystem.MeowmereRG);
            recipe.AddRecipeGroup(RecipeGroupSystem.StarWrathRG);
            recipe.AddRecipeGroup(RecipeGroupSystem.InfluexWaverRG);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
            base.AddRecipes();
        }
    }
    #endregion
    public class PurifyZenith : TinyZenith
    {
        public override ZenithInfo Info => new(ItemID.Zenith, 200, 14, 6.5f, Item.sellPrice(1), ItemRarityID.Purple, 24, [ItemID.Zenith]);
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.Zenith).AddRecipeGroup(RecipeGroupSystem.TerragrimRG).AddTile(TileID.LunarCraftingStation).Register();
            base.AddRecipes();
        }
    }
    #region 其它
    public class BreakerZenith : TinyZenith
    {
        public override ZenithInfo Info => new(ItemID.BreakerBlade, 55, 4, 8f, Item.sellPrice(0, 3), ItemRarityID.LightRed, 180, [ItemID.BreakerBlade]);
        public override void AddRecipes()
        {
            CreateTZRecipe
            (
                 new() { { ItemID.BreakerBlade, 1 } },
                 null,
                 new KeyValuePair<int, int>(ItemID.TitanGlove, 1),
                 TileID.Anvils
             );
            base.AddRecipes();
        }
    }
    public class CobaltZenith : TinyZenith
    {
        //public class CobaltSlash : ModProjectile 
        //{
        //    public override void SetDefaults()
        //    {
        //        Projectile.ArmorPenetration = 15;
        //        Projectile.width = 20;
        //        Projectile.height = 20;
        //        Projectile.aiStyle = -1;
        //        Projectile.friendly = true;
        //        Projectile.tileCollide = false;
        //        Projectile.ignoreWater = true;
        //        Projectile.melee = true;
        //        Projectile.penetrate = 5;
        //        Projectile.usesLocalNPCImmunity = true;
        //        Projectile.localNPCHitCooldown = 4;
        //        Projectile.scale = 1f + (float)Main.rand.Next(30) * 0.01f;
        //        Projectile.extraUpdates = 2;
        //        Projectile.timeLeft = 10 * Projectile.MaxUpdates;
        //        base.SetDefaults();
        //    }
        //    public override void AI()
        //    {
        //        float num = (float)Math.PI / 2f;
        //        bool flag = true;
        //        if (type == 976)
        //            flag = false;

        //        if (flag)
        //        {
        //            alpha -= 10;
        //            int num2 = 100;
        //            if (alpha < num2)
        //                alpha = num2;
        //        }

        //        if (soundDelay == 0)
        //        {
        //            if (type == 977)
        //            {
        //                soundDelay = -1;
        //                SoundEngine.PlaySound(SoundID.Item1, position);
        //            }
        //            else if (type == 976)
        //            {
        //                soundDelay = -1;
        //            }
        //            else
        //            {
        //                soundDelay = 20 + Main.rand.Next(40);
        //                SoundEngine.PlaySound(SoundID.Item9, position);
        //            }
        //        }

        //        if (ai[0] != 0f)
        //        {
        //            if (type == 976)
        //            {
        //                velocity = velocity.RotatedBy(ai[0]);
        //            }
        //            else
        //            {
        //                int num3 = 10 * MaxUpdates;
        //                velocity = velocity.RotatedBy(ai[0] / (float)num3);
        //            }
        //        }

        //        velocity *= 0.96f;
        //        if (Main.rand.Next(8) == 0)
        //        {
        //            Dust dust = Dust.NewDustDirect(base.Center, 0, 0, 172, velocity.X * 0.1f, velocity.Y * 0.1f, 100, default(Color), 0.9f);
        //            dust.noGravity = true;
        //            dust.position = base.Center;
        //            dust.velocity = Main.rand.NextVector2Circular(1f, 1f) + velocity * 0.5f;
        //        }


        //        rotation = velocity.ToRotation() + num;
        //        tileCollide = false;
        //    }
        //    public override bool PreDraw(ref Color lightColor)
        //    {
        //        return false;
        //    }
        //}
        public override ZenithInfo Info => new(ItemID.CobaltSword, 35, 4, 5f, Item.sellPrice(0, 1, 38), ItemRarityID.LightRed, 120, [ItemID.CobaltSword]);
        public override void AddRecipes()
        {
            CreateTZRecipe
            (
                 new() { { ItemID.CobaltSword, 1 } },
                 null,
                 new KeyValuePair<int, int>(ItemID.CrystalShard, 30),
                 TileID.MythrilAnvil
             );
            base.AddRecipes();
        }
    }
    public class PalladiumZenith : TinyZenith
    {
        public override ZenithInfo Info => new(ItemID.PalladiumSword, 40, 4, 5.5f, Item.sellPrice(0, 1, 84), ItemRarityID.LightRed, 135, [ItemID.PalladiumSword]);
        public override void AddRecipes()
        {
            CreateTZRecipe
            (
                 new() { { ItemID.PalladiumSword, 1 } },
                 null,
                 new KeyValuePair<int, int>(ItemID.CrystalShard, 30),
                 TileID.MythrilAnvil
             );
            base.AddRecipes();
        }
    }
    public class MythrilZenith : TinyZenith
    {
        public override ZenithInfo Info => new(ItemID.MythrilSword, 40, 4, 6f, Item.sellPrice(0, 2, 7), ItemRarityID.LightRed, 120, [ItemID.MythrilSword]);
        public override void AddRecipes()
        {
            CreateTZRecipe
            (
                 new() { { ItemID.MythrilSword, 1 } },
                 null,
                 new KeyValuePair<int, int>(ItemID.PixieDust, 50),
                 TileID.MythrilAnvil
             );
            base.AddRecipes();
        }
        public class MythrilLaser : ModProjectile
        {
            public override string Texture => $"Terraria/Images/Item_{ItemID.MythrilSword}";
            public override bool PreDraw(ref Color lightColor)
            {
                Texture2D light = TextureAssets.Extra[98].Value;
                SpriteBatch spriteBatch = Main.spriteBatch;
                for (int n = 0; n < 10; n++)
                {
                    spriteBatch.Draw(light, Projectile.Center - Main.screenPosition - Projectile.velocity * n * 2, null, new Color(11, 64, 98, 0) * (1 / (1f + n)), Projectile.rotation - MathHelper.PiOver2, new Vector2(36), new Vector2(2, 16), 0, 0);
                    spriteBatch.Draw(light, Projectile.Center - Main.screenPosition - Projectile.velocity * n * 2, null, Color.White with { A = 0 } * (1 / (1f + n)), Projectile.rotation - MathHelper.PiOver2, new Vector2(36), new Vector2(2, 16) * .5f, 0, 0);
                }


                return false;
            }
            public override void AI()
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
                if (Projectile.timeLeft == 1 && Projectile.ai[0] > 0)
                {
                    Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, Projectile.velocity * new Vector2(1, -1), Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[0] - 1);
                }
                base.AI();
            }
            public override void SetDefaults()
            {
                Projectile.timeLeft = 48;
                Projectile.extraUpdates = 3;
                Projectile.penetrate = -1;
                Projectile.aiStyle = -1;
                Projectile.DamageType = DamageClass.Melee;
                Projectile.ignoreWater = true;
                Projectile.tileCollide = false;
                Projectile.friendly = true;
                Projectile.width = Projectile.height = 36;
                //Projectile.usesLocalNPCImmunity = true;
                //Projectile.localNPCHitCooldown = 10;
                base.SetDefaults();
            }
            public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
            {
                return base.Colliding(projHitbox, targetHitbox);
            }
        }
    }
    public class OrichalcumZenith : TinyZenith
    {
        public override ZenithInfo Info => new(ItemID.OrichalcumSword, 50, 4, 6f, Item.sellPrice(0, 2, 53), ItemRarityID.LightRed, 135, [ItemID.OrichalcumSword]);
        public override void AddRecipes()
        {
            CreateTZRecipe
            (
                 new() { { ItemID.OrichalcumSword, 1 } },
                 null,
                 new KeyValuePair<int, int>(ItemID.PixieDust, 50),
                 TileID.MythrilAnvil
             );
            base.AddRecipes();
        }
    }
    public class AdamantiteZenith : TinyZenith
    {

        public override ZenithInfo Info => new(ItemID.AdamantiteSword, 50, 4, 6f, Item.sellPrice(0, 2, 76), ItemRarityID.LightRed, 105, [ItemID.AdamantiteSword]);
        public override void AddRecipes()
        {
            CreateTZRecipe
            (
                 new() { { ItemID.AdamantiteSword, 1 } },
                 null,
                 new KeyValuePair<int, int>(ItemID.UnicornHorn, 10),
                 TileID.AdamantiteForge
             );
            base.AddRecipes();
        }
        public class AdamantiteMissile : ModProjectile
        {
            public override string Texture => base.Texture.Replace("AdamantiteZenith+", "");
            public override void AI()
            {
                if (Projectile.timeLeft <= 180)
                {
                    Projectile.friendly = Projectile.penetrate > 1;

                    NPC target = Main.npc[(int)Projectile.ai[0]];
                    if (target.CanBeChasedBy()/* && Projectile.friendly*/)
                    {
                        float t = 1 / (1 + 0.1f * Vector2.Distance(target.Center, Projectile.Center));
                        //float t = 0.05f;
                        //float t = Vector2.Distance(target.Center, Projectile.Center);
                        //t /= 64f;
                        //t = t * MathF.Exp(1 - t);
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, target.Center - Projectile.Center, t);
                        float l = Projectile.velocity.Length();
                        if (l < 16 && l != 0)
                        {
                            Projectile.velocity = Projectile.velocity / l * 16f;
                        }
                    }
                    else
                    {
                        foreach (var npc in Main.npc)
                        {
                            if (npc.CanBeChasedBy() && !npc.friendly && npc.active && Vector2.Distance(npc.Center, Projectile.Center) < 512)
                                Projectile.ai[0] = npc.whoAmI;
                        }

                    }


                    //Projectile.friendly = true;
                }
                else
                {
                    Projectile.friendly = false;
                }
                Projectile.rotation = Projectile.velocity.ToRotation();
                for (int n = 9; n > 0; n--)
                {
                    Projectile.oldPos[n] = Projectile.oldPos[n - 1];
                    Projectile.oldRot[n] = Projectile.oldRot[n - 1];
                }
                Projectile.oldPos[0] = Projectile.Center;
                Projectile.oldRot[0] = Projectile.rotation;
                base.AI();
            }
            public override bool PreDraw(ref Color lightColor)
            {
                SpriteBatch spriteBatch = Main.spriteBatch;
                float a = 1f;
                if (Projectile.timeLeft < 30) a = Projectile.timeLeft / 30f;
                for (int n = 0; n < 10; n++)
                {
                    Vector2 scaler = new Vector2(MathHelper.Lerp(1, 0, n / 10f), Vector2.Distance(Projectile.oldPos[n], n == 0 ? Projectile.Center : Projectile.oldPos[n - 1]) / 24f);
                    spriteBatch.Draw(TextureAssets.Extra[98].Value, Projectile.oldPos[n] - Main.screenPosition, null, Color.Red with { A = 0 } * MathHelper.Lerp(1, 0, n / 10f) * a, Projectile.oldRot[n] - MathHelper.PiOver2, new Vector2(36), scaler, 0, 0);

                    spriteBatch.Draw(TextureAssets.Extra[98].Value, Projectile.oldPos[n] - Main.screenPosition, null, Color.White with { A = 0 } * MathHelper.Lerp(1, 0, n / 10f) * a, Projectile.oldRot[n] - MathHelper.PiOver2, new Vector2(36), scaler * .75f, 0, 0);
                }
                spriteBatch.Draw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, Color.White * a, Projectile.rotation + MathHelper.PiOver4, new Vector2(20), 1, 0, 0);
                //RainbowRodDrawer rainbowRodDrawer = new RainbowRodDrawer();
                //rainbowRodDrawer.Draw(Projectile);
                //FlameLashDrawer flameLashDrawer = new FlameLashDrawer();
                //flameLashDrawer.Draw(Projectile);
                return false;
            }
            public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
            {
                return base.Colliding(projHitbox, targetHitbox);
            }

            public override void SetDefaults()
            {
                Projectile.timeLeft = 185;
                Projectile.penetrate = 4;
                Projectile.aiStyle = -1;
                Projectile.DamageType = DamageClass.Melee;
                Projectile.ignoreWater = true;
                Projectile.tileCollide = false;
                Projectile.friendly = true;
                Projectile.width = Projectile.height = 20;
                Projectile.usesLocalNPCImmunity = true;
                Projectile.localNPCHitCooldown = 10;
                base.SetDefaults();
            }
            public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
            {
                if (Projectile.penetrate == 2)
                {
                    Projectile.friendly = false;
                    if (Projectile.timeLeft > 30) Projectile.timeLeft = 30;
                }
                for (int n = 0; n < 30; n++)
                {
                    var dust = Dust.NewDustPerfect(target.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(0, 16), DustID.TheDestroyer, -Projectile.velocity + Main.rand.NextVector2Unit() * Main.rand.NextFloat(0, 8));
                    dust.noGravity = true;
                }
                base.OnHitNPC(target, hit, damageDone);
            }
        }
    }
    public class TitaniumZenith : TinyZenith
    {
        public override ZenithInfo Info => new(ItemID.TitaniumSword, 52, 4, 6f, Item.sellPrice(0, 3, 22), ItemRarityID.LightRed, 105, [ItemID.TitaniumSword]);

        public override void AddRecipes()
        {
            CreateTZRecipe
            (
                 new() { { ItemID.TitaniumSword, 1 } },
                 null,
                 new KeyValuePair<int, int>(ItemID.UnicornHorn, 10),
                 TileID.AdamantiteForge
             );
            base.AddRecipes();
        }
        public class TitaniumSwordProj : ModProjectile
        {
            public override string Texture => $"Terraria/Images/Item_{ItemID.TitaniumSword}";
            public override void AI()
            {
                if (Projectile.timeLeft <= 45)
                {
                    Projectile.velocity = new Vector2(0, 72f * (MathF.Pow(MathHelper.Clamp((46 - Projectile.timeLeft) / 30f, 0, 1), 4.0f) - 0.05f));
                    Projectile.Center = Vector2.Lerp(Projectile.Center, new Vector2(Main.npc[(int)Projectile.ai[0]].Center.X, Projectile.Center.Y), 0.5f);
                    Projectile.rotation = MathHelper.PiOver2;
                }
                else if (Projectile.timeLeft <= 75)
                {
                    NPC target = Main.npc[(int)Projectile.ai[0]];
                    Vector2 tarPos = target.Center - Vector2.UnitY * 384;
                    if (target.CanBeChasedBy())
                    {
                        float t = 1 / (1 + 0.05f * Vector2.Distance(tarPos, Projectile.Center));

                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, tarPos - Projectile.Center, t);
                        float l = Projectile.velocity.Length();
                        if (l < 16 && l != 0)
                        {
                            Projectile.velocity = Projectile.velocity / l * 16f;
                        }
                    }
                    else
                    {
                        foreach (var npc in Main.npc)
                        {
                            if (npc.CanBeChasedBy() && !npc.friendly && npc.active && Vector2.Distance(npc.Center, Projectile.Center) < 512)
                                Projectile.ai[0] = npc.whoAmI;
                        }

                    }

                    Projectile.rotation = Projectile.velocity.ToRotation();

                    Projectile.friendly = true;
                    //Projectile.friendly = Projectile.penetrate > 1;
                }
                else
                {
                    Projectile.friendly = false;
                    Projectile.rotation = Projectile.velocity.ToRotation();
                }
                for (int n = 9; n > 0; n--)
                {
                    Projectile.oldPos[n] = Projectile.oldPos[n - 1];
                    Projectile.oldRot[n] = Projectile.oldRot[n - 1];
                }
                Projectile.oldPos[0] = Projectile.Center;
                Projectile.oldRot[0] = Projectile.rotation;
                base.AI();
            }
            public override bool PreDraw(ref Color lightColor)
            {
                SpriteBatch spriteBatch = Main.spriteBatch;
                float a = 1f;
                if (Projectile.timeLeft < 15) a = Projectile.timeLeft / 15f;
                for (int n = 0; n < 10; n++)
                {
                    Vector2 scaler = new Vector2(MathHelper.Lerp(1, 0, n / 10f), Vector2.Distance(Projectile.oldPos[n], n == 0 ? Projectile.Center : Projectile.oldPos[n - 1]) / 24f);
                    spriteBatch.Draw(TextureAssets.Extra[98].Value, Projectile.oldPos[n] - Main.screenPosition, null, Color.Cyan with { A = 0 } * MathHelper.Lerp(1, 0, n / 10f) * a, Projectile.oldRot[n] - MathHelper.PiOver2, new Vector2(36), scaler, 0, 0);

                    spriteBatch.Draw(TextureAssets.Extra[98].Value, Projectile.oldPos[n] - Main.screenPosition, null, Color.White with { A = 0 } * MathHelper.Lerp(1, 0, n / 10f) * a, Projectile.oldRot[n] - MathHelper.PiOver2, new Vector2(36), scaler * .75f, 0, 0);
                }
                spriteBatch.Draw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, Color.White * a, Projectile.rotation + MathHelper.PiOver4, new Vector2(30), 1, 0, 0);
                if (Projectile.timeLeft < 45)
                {
                    float r = MathF.Pow(MathHelper.Clamp(Projectile.timeLeft - 35, 1f, 10) / 10f, 2.0f) * MathHelper.Pi;
                    Vector2 scaler = new Vector2(1, 4) * .75f;
                    float k = MathF.Sqrt(a) * .25f * MathHelper.SmoothStep(0, 1, (45 - Projectile.timeLeft) / 10f);
                    spriteBatch.Draw(TextureAssets.Extra[98].Value, Projectile.Center - Main.screenPosition, null, Color.Cyan with { A = 0 } * k, r, new Vector2(36), scaler * .75f, 0, 0);
                    spriteBatch.Draw(TextureAssets.Extra[98].Value, Projectile.Center - Main.screenPosition, null, Color.Cyan with { A = 0 } * k, -r, new Vector2(36), scaler * .75f, 0, 0);

                    spriteBatch.Draw(TextureAssets.Extra[98].Value, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 } * k, r, new Vector2(36), scaler, 0, 0);
                    spriteBatch.Draw(TextureAssets.Extra[98].Value, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 } * k, -r, new Vector2(36), scaler, 0, 0);



                }

                //RainbowRodDrawer rainbowRodDrawer = new RainbowRodDrawer();
                //rainbowRodDrawer.Draw(Projectile);
                //FlameLashDrawer flameLashDrawer = new FlameLashDrawer();
                //flameLashDrawer.Draw(Projectile);
                return false;
            }
            public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
            {
                return base.Colliding(projHitbox, targetHitbox);
            }

            public override void SetDefaults()
            {
                Projectile.timeLeft = 80;
                Projectile.penetrate = 5;
                Projectile.aiStyle = -1;
                Projectile.DamageType = DamageClass.Melee;
                Projectile.ignoreWater = true;
                Projectile.tileCollide = false;
                Projectile.friendly = true;
                Projectile.width = Projectile.height = 20;
                Projectile.usesLocalNPCImmunity = true;
                Projectile.localNPCHitCooldown = 10;
                Projectile.ArmorPenetration = 20;
                base.SetDefaults();
            }
        }
    }
    public class HardCoreZenith : TinyZenith
    {
        public override ZenithInfo Info => new(ItemID.TitaniumSword, 57, 4, 6f, Item.sellPrice(0, 10), ItemRarityID.Pink, 96, [ItemID.BreakerBlade, ItemID.CobaltSword, ItemID.PalladiumSword, ItemID.MythrilSword, ItemID.OrichalcumSword, ItemID.AdamantiteSword, ItemID.TitaniumSword], true);

        public override void AddRecipes()
        {
            CreateTZRecipe
            (
                 null,
                 new() { { RecipeGroupSystem.BreakerBladeRG, 1 }, { RecipeGroupSystem.CobaltSwordRG, 1 }, { RecipeGroupSystem.PalladiumSwordRG, 1 }, { RecipeGroupSystem.MythrilSwordRG, 1 }, { RecipeGroupSystem.OrichalcumSwordRG, 1 }, { RecipeGroupSystem.AdamantiteSwordRG, 1 }, { RecipeGroupSystem.TitaniumSwordRG, 1 } },
                 new KeyValuePair<int, int>(ItemID.WarriorEmblem, 1),
                 TileID.AdamantiteForge
             );
            base.AddRecipes();
        }
    }
    #endregion
    /*public class SimpleSword : ModItem
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.EnchantedSword}";
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.EnchantedSword);
            Item.useTime = Item.useAnimation;
            base.SetDefaults();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //SetDefaults();
            //* 0.0001f
            var proj = Projectile.NewProjectileDirect(source, player.Center, velocity, ModContent.ProjectileType<UncenteredSwoosh>(), damage, knockback, player.whoAmI, (float)player.direction * player.gravDir, player.itemAnimationMax, player.GetAdjustedItemScale(Item));
            (proj.ModProjectile as UncenteredSwoosh).clonedType = ProjectileID.NightsEdge;
            //proj.CloneDefaults(ctype);
            //proj.aiStyle = -1;
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
    }*/
    public class UncenteredSwoosh : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.TerraBlade2}";
        public override void CutTiles()
        {
            Vector2 vector2 = (Projectile.rotation - (float)Math.PI / 4f).ToRotationVector2() * 60f * Projectile.scale;
            Vector2 vector3 = (Projectile.rotation + (float)Math.PI / 4f).ToRotationVector2() * 60f * Projectile.scale;
            float num2 = 60f * Projectile.scale;
            Utils.PlotTileLine(Projectile.Center + vector2, Projectile.Center + vector3, num2, DelegateMethods.CutTiles);
            base.CutTiles();
        }
        public override void AI()
        {
            if (clonedType != 985 && clonedType != 973)
                SubAI_1();
            else
                SubAI_2();
            Vector2 boxPosition = Projectile.position;
            int boxWidth = Projectile.width;
            int boxHeight = Projectile.height;
            for (float num = -(float)Math.PI / 4f; num <= (float)Math.PI / 4f; num += (float)Math.PI / 2f)
            {
                Rectangle r = Utils.CenteredRectangle(Projectile.Center + (Projectile.rotation + num).ToRotationVector2() * 70f * Projectile.scale, new Vector2(60f * Projectile.scale, 60f * Projectile.scale));
                Projectile.EmitEnchantmentVisualsAt(r.TopLeft(), r.Width, r.Height);
            }
            base.AI();
        }
        public int clonedType;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(clonedType);
            base.SendExtraAI(writer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            clonedType = reader.ReadInt32();
            base.ReceiveExtraAI(reader);
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.melee = true;
            Projectile.penetrate = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ownerHitCheck = true;
            Projectile.ownerHitCheckDistance = 300f;
            Projectile.usesOwnerMeleeHitCD = true;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.timeLeft = 90;
            base.SetDefaults();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            var proj = Projectile;
            switch (clonedType)
            {
                case 972:
                    Main.instance.DrawProj_NightsEdge(proj);
                    break;
                case 973:
                    Main.instance.DrawProj_TrueNightsEdge(proj);
                    break;
                case 982:
                    Main.instance.DrawProj_Excalibur(proj);
                    break;
                case 997:
                    Main.instance.DrawProj_TheHorsemansBlade(proj);
                    break;
                case 984:
                    Main.instance.DrawProj_TerraBlade2(proj);
                    break;
                case 985:
                    Main.instance.DrawProj_TerraBlade2Shot(proj);
                    break;
                case 983:
                    Main.instance.DrawProj_TrueExcalibur(proj);
                    break;
            }
            return false;
        }
        /// <summary>
        /// 原版的剑气弹幕
        /// </summary>
        void SubAI_1()
        {
            if (Projectile.localAI[0] == 0f && clonedType == 984)
            {
                SoundEffectInstance soundEffectInstance = SoundEngine.PlaySound((SoundStyle?)SoundID.Item60, Projectile.position);
                if (soundEffectInstance != null)
                    soundEffectInstance.Volume *= 0.65f;
            }

            Projectile.localAI[0] += 1f;
            Player player = Main.player[Projectile.owner];
            float num = Projectile.localAI[0] / Projectile.ai[1];
            float num2 = Projectile.ai[0];
            float num3 = Projectile.velocity.ToRotation();
            float num4 = (float)Math.PI * num2 * num + num3 + num2 * (float)Math.PI + player.fullRotation;
            Projectile.rotation = num4;
            float num5 = 0.2f;
            float num6 = 1f;
            switch (clonedType)
            {
                case 982:
                    num5 = 0.6f;
                    break;
                case 997:
                    num5 = 0.6f;
                    break;
                case 983:
                    num5 = 1f;
                    num6 = 1.2f;
                    break;
                case 984:
                    num5 = 0.6f;
                    break;
            }

            //Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) - Projectile.velocity;
            Projectile.scale = num6 + num * num5;
            if (clonedType == 972)
            {
                if (Math.Abs(num2) < 0.2f)
                {
                    Projectile.rotation += (float)Math.PI * 4f * num2 * 10f * num;
                    float num7 = Utils.Remap(Projectile.localAI[0], 10f, Projectile.ai[1] - 5f, 0f, 1f);
                    Projectile.position += Projectile.velocity.SafeNormalize(Vector2.Zero) * (45f * num7);
                    Projectile.scale += num7 * 0.4f;
                }

                if (Main.rand.Next(2) == 0)
                {
                    float f = Projectile.rotation + Main.rand.NextFloatDirection() * ((float)Math.PI / 2f) * 0.7f;
                    Vector2 vector = Projectile.Center + f.ToRotationVector2() * 84f * Projectile.scale;
                    if (Main.rand.Next(5) == 0)
                    {
                        Dust dust = Dust.NewDustPerfect(vector, 14, null, 150, default(Color), 1.4f);
                        dust.noLight = (dust.noLightEmittence = true);
                    }

                    if (Main.rand.Next(2) == 0)
                        Dust.NewDustPerfect(vector, 27, new Vector2(player.velocity.X * 0.2f + (float)(player.direction * 3), player.velocity.Y * 0.2f), 100, default(Color), 1.4f).noGravity = true;
                }
            }

            if (clonedType == 982)
            {
                float num8 = Projectile.rotation + Main.rand.NextFloatDirection() * ((float)Math.PI / 2f) * 0.7f;
                Vector2 vector2 = Projectile.Center + num8.ToRotationVector2() * 84f * Projectile.scale;
                Vector2 vector3 = (num8 + Projectile.ai[0] * ((float)Math.PI / 2f)).ToRotationVector2();
                if (Main.rand.NextFloat() * 2f < Projectile.Opacity)
                {
                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center + num8.ToRotationVector2() * (Main.rand.NextFloat() * 80f * Projectile.scale + 20f * Projectile.scale), 278, vector3 * 1f, 100, Color.Lerp(Color.Gold, Color.White, Main.rand.NextFloat() * 0.3f), 0.4f);
                    dust2.fadeIn = 0.4f + Main.rand.NextFloat() * 0.15f;
                    dust2.noGravity = true;
                }

                if (Main.rand.NextFloat() * 1.5f < Projectile.Opacity)
                    Dust.NewDustPerfect(vector2, 43, vector3 * 1f, 100, Color.White * Projectile.Opacity, 1.2f * Projectile.Opacity);
            }

            if (clonedType == 997)
            {
                float num9 = Projectile.rotation + Main.rand.NextFloatDirection() * ((float)Math.PI / 2f) * 0.7f;
                _ = Projectile.Center + num9.ToRotationVector2() * 84f * Projectile.scale;
                Vector2 vector4 = (num9 + Projectile.ai[0] * ((float)Math.PI / 2f)).ToRotationVector2();
                if (Main.rand.NextFloat() * 2f < Projectile.Opacity)
                {
                    Dust dust3 = Dust.NewDustPerfect(Projectile.Center + num9.ToRotationVector2() * (Main.rand.NextFloat() * 80f * Projectile.scale + 20f * Projectile.scale), 6, vector4 * 4f, 0, default(Color), 0.4f);
                    dust3.noGravity = true;
                    dust3.scale = 1.4f;
                }
            }

            if (clonedType == 983)
            {
                float num10 = Projectile.rotation + Main.rand.NextFloatDirection() * ((float)Math.PI / 2f) * 0.7f;
                Vector2 vector5 = Projectile.Center + num10.ToRotationVector2() * 84f * Projectile.scale;
                Vector2 vector6 = (num10 + Projectile.ai[0] * ((float)Math.PI / 2f)).ToRotationVector2();
                if (Main.rand.NextFloat() < Projectile.Opacity)
                {
                    Dust dust4 = Dust.NewDustPerfect(Projectile.Center + num10.ToRotationVector2() * (Main.rand.NextFloat() * 80f * Projectile.scale + 20f * Projectile.scale), 278, vector6 * 1f, 100, Color.Lerp(Color.HotPink, Color.White, Main.rand.NextFloat() * 0.3f), 0.4f);
                    dust4.fadeIn = 0.4f + Main.rand.NextFloat() * 0.15f;
                    dust4.noGravity = true;
                }

                if (Main.rand.NextFloat() * 1.5f < Projectile.Opacity)
                    Dust.NewDustPerfect(vector5, 43, vector6 * 1f, 100, Color.White * Projectile.Opacity, 1.2f * Projectile.Opacity);
            }

            if (clonedType == 984)
            {
                float num11 = Projectile.rotation + Main.rand.NextFloatDirection() * ((float)Math.PI / 2f) * 0.7f;
                Vector2 vector7 = Projectile.Center + num11.ToRotationVector2() * 85f * Projectile.scale;
                Vector2 vector8 = (num11 + Projectile.ai[0] * ((float)Math.PI / 2f)).ToRotationVector2();
                Color value = new Color(64, 220, 96);
                Color value2 = new Color(15, 84, 125);
                Lighting.AddLight(Projectile.Center, value2.ToVector3());
                if (Main.rand.NextFloat() * 2f < Projectile.Opacity)
                {
                    Color value3 = Color.Lerp(value2, value, Utils.Remap(num, 0f, 0.6f, 0f, 1f));
                    value3 = Color.Lerp(value3, Color.White, Utils.Remap(num, 0.6f, 0.8f, 0f, 0.5f));
                    Dust dust5 = Dust.NewDustPerfect(Projectile.Center + num11.ToRotationVector2() * (Main.rand.NextFloat() * 80f * Projectile.scale + 20f * Projectile.scale), 278, vector8 * 1f, 100, Color.Lerp(value3, Color.White, Main.rand.NextFloat() * 0.3f), 0.4f);
                    dust5.fadeIn = 0.4f + Main.rand.NextFloat() * 0.15f;
                    dust5.noGravity = true;
                }

                if (Main.rand.NextFloat() < Projectile.Opacity)
                {
                    Color.Lerp(Color.Lerp(Color.Lerp(value2, value, Utils.Remap(num, 0f, 0.6f, 0f, 1f)), Color.White, Utils.Remap(num, 0.6f, 0.8f, 0f, 0.5f)), Color.White, Main.rand.NextFloat() * 0.3f);
                    Dust dust6 = Dust.NewDustPerfect(vector7, 107, vector8 * 3f, 100, default(Color) * Projectile.Opacity, 0.8f * Projectile.Opacity);
                    dust6.velocity += player.velocity * 0.1f;
                    dust6.velocity += new Vector2(player.direction, 0f);
                    dust6.position -= dust6.velocity * 6f;
                }
            }

            Projectile.scale *= Projectile.ai[2];
            if (Projectile.localAI[0] >= Projectile.ai[1])
                Projectile.Kill();
        }
        /// <summary>
        /// 能发射出去的，真永夜和泰拉刃
        /// </summary>
        void SubAI_2()
        {
            float num = 50f;
            float num2 = 15f;
            float num3 = Projectile.ai[1] + num;
            float num4 = num3 + num2;
            float num5 = 77f;
            if (clonedType == 985)
            {
                num = 0f;
                num3 = Projectile.ai[1];
                num4 = Projectile.ai[1] + 25f;
                num5 = num4;
            }

            if (Projectile.localAI[0] == 0f && clonedType == 973)
                SoundEngine.PlaySound(SoundID.Item8, Projectile.position);

            Projectile.localAI[0] += 1f;
            if (clonedType == 985 && Projectile.localAI[1] == 1f)
                Projectile.localAI[0] += 2f;

            if (clonedType == 973 && Projectile.damage == 0 && Projectile.localAI[0] < MathHelper.Lerp(num3, num4, 0.5f))
                Projectile.localAI[0] += 6f;

            Projectile.Opacity = Utils.Remap(Projectile.localAI[0], 0f, Projectile.ai[1], 0f, 1f) * Utils.Remap(Projectile.localAI[0], num3, num4, 1f, 0f);
            if (Projectile.localAI[0] >= num4)
            {
                Projectile.localAI[1] = 1f;
                Projectile.Kill();
                return;
            }

            Player player = Main.player[Projectile.owner];
            float fromValue = Projectile.localAI[0] / Projectile.ai[1];
            float num6 = Utils.Remap(Projectile.localAI[0], Projectile.ai[1] * 0.4f, num4, 0f, 1f);
            Projectile.direction = (Projectile.spriteDirection = (int)Projectile.ai[0]);
            if (clonedType == 973)
            {
                int num7 = 3;
                if (Projectile.damage != 0 && Projectile.localAI[0] >= num5 + (float)num7)
                    Projectile.damage = 0;

                if (Projectile.damage != 0)
                {
                    int num8 = 80;
                    bool flag = false;
                    float num9 = Projectile.velocity.ToRotation();
                    for (float num10 = -1f; num10 <= 1f; num10 += 0.5f)
                    {
                        Vector2 position = Projectile.Center + (num9 + num10 * ((float)Math.PI / 4f) * 0.25f).ToRotationVector2() * num8 * 0.5f * Projectile.scale;
                        Vector2 position2 = Projectile.Center + (num9 + num10 * ((float)Math.PI / 4f) * 0.25f).ToRotationVector2() * num8 * Projectile.scale;
                        if (!Collision.SolidTiles(Projectile.Center, 0, 0) && Collision.CanHit(position, 0, 0, position2, 0, 0))
                        {
                            flag = true;
                            break;
                        }
                    }

                    if (!flag)
                        Projectile.damage = 0;
                }

                fromValue = Projectile.localAI[0] / Projectile.ai[1];
                Projectile.localAI[1] += 1f;
                num6 = Utils.Remap(Projectile.localAI[1], Projectile.ai[1] * 0.4f, num4, 0f, 1f);
                Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) - Projectile.velocity + Projectile.velocity * num6 * num6 * num5;
                Projectile.rotation += Projectile.ai[0] * ((float)Math.PI * 2f) * (4f + Projectile.Opacity * 4f) / 90f;
                Projectile.scale = Utils.Remap(Projectile.localAI[0], Projectile.ai[1] + 2f, num4, 1.12f, 1f) * Projectile.ai[2];
                float f = Projectile.rotation + Main.rand.NextFloatDirection() * ((float)Math.PI / 2f) * 0.7f;
                Vector2 vector = Projectile.Center + f.ToRotationVector2() * 84f * Projectile.scale;
                if (Main.rand.Next(5) == 0)
                {
                    Dust dust = Dust.NewDustPerfect(vector, 14, null, 150, default(Color), 1.4f);
                    dust.noLight = (dust.noLightEmittence = true);
                }

                for (int i = 0; (float)i < 3f * Projectile.Opacity; i++)
                {
                    Vector2 vector2 = Projectile.velocity.SafeNormalize(Vector2.UnitX);
                    int num11 = ((Main.rand.NextFloat() < Projectile.Opacity) ? 75 : 27);
                    Dust dust2 = Dust.NewDustPerfect(vector, num11, Projectile.velocity * 0.2f + vector2 * 3f, 100, default(Color), 1.4f);
                    dust2.noGravity = true;
                    dust2.customData = Projectile.Opacity * 0.2f;
                }
            }

            if (clonedType != 985)
                return;

            Projectile.ownerHitCheck = Projectile.localAI[0] <= 6f;
            if (Projectile.localAI[0] >= MathHelper.Lerp(num3, num4, 0.65f))
                Projectile.damage = 0;

            float fromValue2 = 1f - (1f - num6) * (1f - num6);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.scale = Utils.Remap(fromValue2, 0f, 1f, 1.5f, 1f) * Projectile.ai[2];
            num6 = Utils.Remap(Projectile.localAI[0], Projectile.ai[1] / 2f, num4, 0f, 1f);
            Projectile.Opacity = Utils.Remap(Projectile.localAI[0], 0f, Projectile.ai[1] * 0.5f, 0f, 1f) * Utils.Remap(Projectile.localAI[0], num4 - 12f, num4, 1f, 0f);
            if (Projectile.velocity.Length() > 8f)
            {
                Projectile.velocity *= 0.94f;
                new Vector2(32f, 32f);
                float num12 = Utils.Remap(fromValue, 0.7f, 1f, 110f, 110f);
                if (Projectile.localAI[1] == 0f)
                {
                    bool flag2 = false;
                    for (float num13 = -1f; num13 <= 1f; num13 += 0.5f)
                    {
                        Vector2 position3 = Projectile.Center + (Projectile.rotation + num13 * ((float)Math.PI / 4f) * 0.25f).ToRotationVector2() * num12 * 0.5f * Projectile.scale;
                        Vector2 position4 = Projectile.Center + (Projectile.rotation + num13 * ((float)Math.PI / 4f) * 0.25f).ToRotationVector2() * num12 * Projectile.scale;
                        if (Collision.CanHit(position3, 0, 0, position4, 0, 0))
                        {
                            flag2 = true;
                            break;
                        }
                    }

                    if (!flag2)
                        Projectile.localAI[1] = 1f;
                }

                if (Projectile.localAI[1] == 1f && Projectile.velocity.Length() > 8f)
                    Projectile.velocity *= 0.8f;

                if (Projectile.localAI[1] == 1f)
                    Projectile.velocity *= 0.88f;
            }

            float num14 = Projectile.rotation + Main.rand.NextFloatDirection() * ((float)Math.PI / 2f) * 0.9f;
            Vector2 vector3 = Projectile.Center + num14.ToRotationVector2() * 85f * Projectile.scale;
            (num14 + Projectile.ai[0] * ((float)Math.PI / 2f)).ToRotationVector2();
            Color value = new Color(64, 220, 96);
            Color value2 = new Color(15, 84, 125);
            Lighting.AddLight(Projectile.Center + Projectile.rotation.ToRotationVector2() * 85f * Projectile.scale, value.ToVector3());
            for (int j = 0; j < 3; j++)
            {
                if (Main.rand.NextFloat() < Projectile.Opacity + 0.1f)
                {
                    Color.Lerp(Color.Lerp(Color.Lerp(value2, value, Utils.Remap(fromValue, 0f, 0.6f, 0f, 1f)), Color.White, Utils.Remap(fromValue, 0.6f, 0.8f, 0f, 0.5f)), Color.White, Main.rand.NextFloat() * 0.3f);
                    Dust dust3 = Dust.NewDustPerfect(vector3, 107, Projectile.velocity * 0.7f, 100, default(Color) * Projectile.Opacity, 0.8f * Projectile.Opacity);
                    dust3.scale *= 0.7f;
                    dust3.velocity += player.velocity * 0.1f;
                    dust3.position -= dust3.velocity * 6f;
                }
            }

            if (Projectile.damage == 0)
            {
                Projectile.localAI[0] += 3f;
                Projectile.velocity *= 0.76f;
            }

            if (Projectile.localAI[0] < 10f && (Projectile.localAI[1] == 1f || Projectile.damage == 0))
            {
                Projectile.localAI[0] += 1f;
                Projectile.velocity *= 0.85f;
                for (int k = 0; k < 4; k++)
                {
                    float num15 = Main.rand.NextFloatDirection();
                    float num16 = 1f - Math.Abs(num15);
                    num14 = Projectile.rotation + num15 * ((float)Math.PI / 2f) * 0.9f;
                    vector3 = Projectile.Center + num14.ToRotationVector2() * 85f * Projectile.scale;
                    Color.Lerp(Color.Lerp(Color.Lerp(value2, value, Utils.Remap(fromValue, 0f, 0.6f, 0f, 1f)), Color.White, Utils.Remap(fromValue, 0.6f, 0.8f, 0f, 0.5f)), Color.White, Main.rand.NextFloat() * 0.3f);
                    Dust dust4 = Dust.NewDustPerfect(vector3, 107, Projectile.velocity.RotatedBy(num15 * ((float)Math.PI / 4f)) * 0.2f * Main.rand.NextFloat(), 100, default(Color), 1.4f * num16);
                    dust4.velocity += player.velocity * 0.1f;
                    dust4.position -= dust4.velocity * Main.rand.NextFloat() * 3f;
                }
            }
        }
        /*
        void SubAI_1() 
        {
            if (Projectile.localAI[0] == 0f && clonedType == 984)
            {
                SoundEffectInstance soundEffectInstance = SoundEngine.PlaySound((SoundStyle?)SoundID.Item60, Projectile.position);
                if (soundEffectInstance != null)
                    soundEffectInstance.Volume *= 0.65f;
            }

            Projectile.localAI[0] += 1f;
            Player player = Main.player[Projectile.owner];
            float num = Projectile.localAI[0] / Projectile.ai[1];
            float num2 = Projectile.ai[0];
            float num3 = Projectile.velocity.ToRotation();
            float num4 = (float)Math.PI * num2 * num + num3 + num2 * (float)Math.PI + player.fullRotation;
            Projectile.rotation = num4;
            float num5 = 0.2f;
            float num6 = 1f;
            switch (clonedType)
            {
                case 982:
                    num5 = 0.6f;
                    break;
                case 997:
                    num5 = 0.6f;
                    break;
                case 983:
                    num5 = 1f;
                    num6 = 1.2f;
                    break;
                case 984:
                    num5 = 0.6f;
                    break;
            }

            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) - Projectile.velocity;
            Projectile.scale = num6 + num * num5;
            if (clonedType == 972)
            {
                if (Math.Abs(num2) < 0.2f)
                {
                    Projectile.rotation += (float)Math.PI * 4f * num2 * 10f * num;
                    float num7 = Utils.Remap(Projectile.localAI[0], 10f, Projectile.ai[1] - 5f, 0f, 1f);
                    Projectile.position += Projectile.velocity.SafeNormalize(Vector2.Zero) * (45f * num7);
                    Projectile.scale += num7 * 0.4f;
                }

                if (Main.rand.Next(2) == 0)
                {
                    float f = Projectile.rotation + Main.rand.NextFloatDirection() * ((float)Math.PI / 2f) * 0.7f;
                    Vector2 vector = Projectile.Center + f.ToRotationVector2() * 84f * Projectile.scale;
                    if (Main.rand.Next(5) == 0)
                    {
                        Dust dust = Dust.NewDustPerfect(vector, 14, null, 150, default(Color), 1.4f);
                        dust.noLight = (dust.noLightEmittence = true);
                    }

                    if (Main.rand.Next(2) == 0)
                        Dust.NewDustPerfect(vector, 27, new Vector2(player.velocity.X * 0.2f + (float)(player.direction * 3), player.velocity.Y * 0.2f), 100, default(Color), 1.4f).noGravity = true;
                }
            }

            if (clonedType == 982)
            {
                float num8 = Projectile.rotation + Main.rand.NextFloatDirection() * ((float)Math.PI / 2f) * 0.7f;
                Vector2 vector2 = Projectile.Center + num8.ToRotationVector2() * 84f * Projectile.scale;
                Vector2 vector3 = (num8 + Projectile.ai[0] * ((float)Math.PI / 2f)).ToRotationVector2();
                if (Main.rand.NextFloat() * 2f < Projectile.Opacity)
                {
                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center + num8.ToRotationVector2() * (Main.rand.NextFloat() * 80f * Projectile.scale + 20f * Projectile.scale), 278, vector3 * 1f, 100, Color.Lerp(Color.Gold, Color.White, Main.rand.NextFloat() * 0.3f), 0.4f);
                    dust2.fadeIn = 0.4f + Main.rand.NextFloat() * 0.15f;
                    dust2.noGravity = true;
                }

                if (Main.rand.NextFloat() * 1.5f < Projectile.Opacity)
                    Dust.NewDustPerfect(vector2, 43, vector3 * 1f, 100, Color.White * Projectile.Opacity, 1.2f * Projectile.Opacity);
            }

            if (clonedType == 997)
            {
                float num9 = Projectile.rotation + Main.rand.NextFloatDirection() * ((float)Math.PI / 2f) * 0.7f;
                _ = Projectile.Center + num9.ToRotationVector2() * 84f * Projectile.scale;
                Vector2 vector4 = (num9 + Projectile.ai[0] * ((float)Math.PI / 2f)).ToRotationVector2();
                if (Main.rand.NextFloat() * 2f < Projectile.Opacity)
                {
                    Dust dust3 = Dust.NewDustPerfect(Projectile.Center + num9.ToRotationVector2() * (Main.rand.NextFloat() * 80f * Projectile.scale + 20f * Projectile.scale), 6, vector4 * 4f, 0, default(Color), 0.4f);
                    dust3.noGravity = true;
                    dust3.scale = 1.4f;
                }
            }

            if (clonedType == 983)
            {
                float num10 = Projectile.rotation + Main.rand.NextFloatDirection() * ((float)Math.PI / 2f) * 0.7f;
                Vector2 vector5 = Projectile.Center + num10.ToRotationVector2() * 84f * Projectile.scale;
                Vector2 vector6 = (num10 + Projectile.ai[0] * ((float)Math.PI / 2f)).ToRotationVector2();
                if (Main.rand.NextFloat() < Projectile.Opacity)
                {
                    Dust dust4 = Dust.NewDustPerfect(Projectile.Center + num10.ToRotationVector2() * (Main.rand.NextFloat() * 80f * Projectile.scale + 20f * Projectile.scale), 278, vector6 * 1f, 100, Color.Lerp(Color.HotPink, Color.White, Main.rand.NextFloat() * 0.3f), 0.4f);
                    dust4.fadeIn = 0.4f + Main.rand.NextFloat() * 0.15f;
                    dust4.noGravity = true;
                }

                if (Main.rand.NextFloat() * 1.5f < Projectile.Opacity)
                    Dust.NewDustPerfect(vector5, 43, vector6 * 1f, 100, Color.White * Projectile.Opacity, 1.2f * Projectile.Opacity);
            }

            if (clonedType == 984)
            {
                float num11 = Projectile.rotation + Main.rand.NextFloatDirection() * ((float)Math.PI / 2f) * 0.7f;
                Vector2 vector7 = Projectile.Center + num11.ToRotationVector2() * 85f * Projectile.scale;
                Vector2 vector8 = (num11 + Projectile.ai[0] * ((float)Math.PI / 2f)).ToRotationVector2();
                Color value = new Color(64, 220, 96);
                Color value2 = new Color(15, 84, 125);
                Lighting.AddLight(Projectile.Center, value2.ToVector3());
                if (Main.rand.NextFloat() * 2f < Projectile.Opacity)
                {
                    Color value3 = Color.Lerp(value2, value, Utils.Remap(num, 0f, 0.6f, 0f, 1f));
                    value3 = Color.Lerp(value3, Color.White, Utils.Remap(num, 0.6f, 0.8f, 0f, 0.5f));
                    Dust dust5 = Dust.NewDustPerfect(Projectile.Center + num11.ToRotationVector2() * (Main.rand.NextFloat() * 80f * Projectile.scale + 20f * Projectile.scale), 278, vector8 * 1f, 100, Color.Lerp(value3, Color.White, Main.rand.NextFloat() * 0.3f), 0.4f);
                    dust5.fadeIn = 0.4f + Main.rand.NextFloat() * 0.15f;
                    dust5.noGravity = true;
                }

                if (Main.rand.NextFloat() < Projectile.Opacity)
                {
                    Color.Lerp(Color.Lerp(Color.Lerp(value2, value, Utils.Remap(num, 0f, 0.6f, 0f, 1f)), Color.White, Utils.Remap(num, 0.6f, 0.8f, 0f, 0.5f)), Color.White, Main.rand.NextFloat() * 0.3f);
                    Dust dust6 = Dust.NewDustPerfect(vector7, 107, vector8 * 3f, 100, default(Color) * Projectile.Opacity, 0.8f * Projectile.Opacity);
                    dust6.velocity += player.velocity * 0.1f;
                    dust6.velocity += new Vector2(player.direction, 0f);
                    dust6.position -= dust6.velocity * 6f;
                }
            }

            Projectile.scale *= Projectile.ai[2];
            if (Projectile.localAI[0] >= Projectile.ai[1])
                Projectile.Kill();
        }
        void SubAI_2() 
        {
            float num = 50f;
            float num2 = 15f;
            float num3 = Projectile.ai[1] + num;
            float num4 = num3 + num2;
            float num5 = 77f;
            if (clonedType == 985)
            {
                num = 0f;
                num3 = Projectile.ai[1];
                num4 = Projectile.ai[1] + 25f;
                num5 = num4;
            }

            if (Projectile.localAI[0] == 0f && clonedType == 973)
                SoundEngine.PlaySound(SoundID.Item8, Projectile.position);

            Projectile.localAI[0] += 1f;
            if (clonedType == 985 && Projectile.localAI[1] == 1f)
                Projectile.localAI[0] += 2f;

            if (clonedType == 973 && Projectile.damage == 0 && Projectile.localAI[0] < MathHelper.Lerp(num3, num4, 0.5f))
                Projectile.localAI[0] += 6f;

            Projectile.Opacity = Utils.Remap(Projectile.localAI[0], 0f, Projectile.ai[1], 0f, 1f) * Utils.Remap(Projectile.localAI[0], num3, num4, 1f, 0f);
            if (Projectile.localAI[0] >= num4)
            {
                Projectile.localAI[1] = 1f;
                Projectile.Kill();
                return;
            }

            Player player = Main.player[Projectile.owner];
            float fromValue = Projectile.localAI[0] / Projectile.ai[1];
            float num6 = Utils.Remap(Projectile.localAI[0], Projectile.ai[1] * 0.4f, num4, 0f, 1f);
            Projectile.direction = (Projectile.spriteDirection = (int)Projectile.ai[0]);
            if (clonedType == 973)
            {
                int num7 = 3;
                if (Projectile.damage != 0 && Projectile.localAI[0] >= num5 + (float)num7)
                    Projectile.damage = 0;

                if (Projectile.damage != 0)
                {
                    int num8 = 80;
                    bool flag = false;
                    float num9 = Projectile.velocity.ToRotation();
                    for (float num10 = -1f; num10 <= 1f; num10 += 0.5f)
                    {
                        Vector2 position = Projectile.Center + (num9 + num10 * ((float)Math.PI / 4f) * 0.25f).ToRotationVector2() * num8 * 0.5f * Projectile.scale;
                        Vector2 position2 = Projectile.Center + (num9 + num10 * ((float)Math.PI / 4f) * 0.25f).ToRotationVector2() * num8 * Projectile.scale;
                        if (!Collision.SolidTiles(Projectile.Center, 0, 0) && Collision.CanHit(position, 0, 0, position2, 0, 0))
                        {
                            flag = true;
                            break;
                        }
                    }

                    if (!flag)
                        Projectile.damage = 0;
                }

                fromValue = Projectile.localAI[0] / Projectile.ai[1];
                Projectile.localAI[1] += 1f;
                num6 = Utils.Remap(Projectile.localAI[1], Projectile.ai[1] * 0.4f, num4, 0f, 1f);
                Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) - Projectile.velocity + Projectile.velocity * num6 * num6 * num5;
                Projectile.rotation += Projectile.ai[0] * ((float)Math.PI * 2f) * (4f + Projectile.Opacity * 4f) / 90f;
                Projectile.scale = Utils.Remap(Projectile.localAI[0], Projectile.ai[1] + 2f, num4, 1.12f, 1f) * Projectile.ai[2];
                float f = Projectile.rotation + Main.rand.NextFloatDirection() * ((float)Math.PI / 2f) * 0.7f;
                Vector2 vector = Projectile.Center + f.ToRotationVector2() * 84f * Projectile.scale;
                if (Main.rand.Next(5) == 0)
                {
                    Dust dust = Dust.NewDustPerfect(vector, 14, null, 150, default(Color), 1.4f);
                    dust.noLight = (dust.noLightEmittence = true);
                }

                for (int i = 0; (float)i < 3f * Projectile.Opacity; i++)
                {
                    Vector2 vector2 = Projectile.velocity.SafeNormalize(Vector2.UnitX);
                    int num11 = ((Main.rand.NextFloat() < Projectile.Opacity) ? 75 : 27);
                    Dust dust2 = Dust.NewDustPerfect(vector, num11, Projectile.velocity * 0.2f + vector2 * 3f, 100, default(Color), 1.4f);
                    dust2.noGravity = true;
                    dust2.customData = Projectile.Opacity * 0.2f;
                }
            }

            if (clonedType != 985)
                return;

            Projectile.ownerHitCheck = Projectile.localAI[0] <= 6f;
            if (Projectile.localAI[0] >= MathHelper.Lerp(num3, num4, 0.65f))
                Projectile.damage = 0;

            float fromValue2 = 1f - (1f - num6) * (1f - num6);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.scale = Utils.Remap(fromValue2, 0f, 1f, 1.5f, 1f) * Projectile.ai[2];
            num6 = Utils.Remap(Projectile.localAI[0], Projectile.ai[1] / 2f, num4, 0f, 1f);
            Projectile.Opacity = Utils.Remap(Projectile.localAI[0], 0f, Projectile.ai[1] * 0.5f, 0f, 1f) * Utils.Remap(Projectile.localAI[0], num4 - 12f, num4, 1f, 0f);
            if (Projectile.velocity.Length() > 8f)
            {
                Projectile.velocity *= 0.94f;
                new Vector2(32f, 32f);
                float num12 = Utils.Remap(fromValue, 0.7f, 1f, 110f, 110f);
                if (Projectile.localAI[1] == 0f)
                {
                    bool flag2 = false;
                    for (float num13 = -1f; num13 <= 1f; num13 += 0.5f)
                    {
                        Vector2 position3 = Projectile.Center + (Projectile.rotation + num13 * ((float)Math.PI / 4f) * 0.25f).ToRotationVector2() * num12 * 0.5f * Projectile.scale;
                        Vector2 position4 = Projectile.Center + (Projectile.rotation + num13 * ((float)Math.PI / 4f) * 0.25f).ToRotationVector2() * num12 * Projectile.scale;
                        if (Collision.CanHit(position3, 0, 0, position4, 0, 0))
                        {
                            flag2 = true;
                            break;
                        }
                    }

                    if (!flag2)
                        Projectile.localAI[1] = 1f;
                }

                if (Projectile.localAI[1] == 1f && Projectile.velocity.Length() > 8f)
                    Projectile.velocity *= 0.8f;

                if (Projectile.localAI[1] == 1f)
                    Projectile.velocity *= 0.88f;
            }

            float num14 = Projectile.rotation + Main.rand.NextFloatDirection() * ((float)Math.PI / 2f) * 0.9f;
            Vector2 vector3 = Projectile.Center + num14.ToRotationVector2() * 85f * Projectile.scale;
            (num14 + Projectile.ai[0] * ((float)Math.PI / 2f)).ToRotationVector2();
            Color value = new Color(64, 220, 96);
            Color value2 = new Color(15, 84, 125);
            Lighting.AddLight(Projectile.Center + Projectile.rotation.ToRotationVector2() * 85f * Projectile.scale, value.ToVector3());
            for (int j = 0; j < 3; j++)
            {
                if (Main.rand.NextFloat() < Projectile.Opacity + 0.1f)
                {
                    Color.Lerp(Color.Lerp(Color.Lerp(value2, value, Utils.Remap(fromValue, 0f, 0.6f, 0f, 1f)), Color.White, Utils.Remap(fromValue, 0.6f, 0.8f, 0f, 0.5f)), Color.White, Main.rand.NextFloat() * 0.3f);
                    Dust dust3 = Dust.NewDustPerfect(vector3, 107, Projectile.velocity * 0.7f, 100, default(Color) * Projectile.Opacity, 0.8f * Projectile.Opacity);
                    dust3.scale *= 0.7f;
                    dust3.velocity += player.velocity * 0.1f;
                    dust3.position -= dust3.velocity * 6f;
                }
            }

            if (Projectile.damage == 0)
            {
                Projectile.localAI[0] += 3f;
                Projectile.velocity *= 0.76f;
            }

            if (Projectile.localAI[0] < 10f && (Projectile.localAI[1] == 1f || Projectile.damage == 0))
            {
                Projectile.localAI[0] += 1f;
                Projectile.velocity *= 0.85f;
                for (int k = 0; k < 4; k++)
                {
                    float num15 = Main.rand.NextFloatDirection();
                    float num16 = 1f - Math.Abs(num15);
                    num14 = Projectile.rotation + num15 * ((float)Math.PI / 2f) * 0.9f;
                    vector3 = Projectile.Center + num14.ToRotationVector2() * 85f * Projectile.scale;
                    Color.Lerp(Color.Lerp(Color.Lerp(value2, value, Utils.Remap(fromValue, 0f, 0.6f, 0f, 1f)), Color.White, Utils.Remap(fromValue, 0.6f, 0.8f, 0f, 0.5f)), Color.White, Main.rand.NextFloat() * 0.3f);
                    Dust dust4 = Dust.NewDustPerfect(vector3, 107, Projectile.velocity.RotatedBy(num15 * ((float)Math.PI / 4f)) * 0.2f * Main.rand.NextFloat(), 100, default(Color), 1.4f * num16);
                    dust4.velocity += player.velocity * 0.1f;
                    dust4.position -= dust4.velocity * Main.rand.NextFloat() * 3f;
                }
            }
        }
        */
    }
}
