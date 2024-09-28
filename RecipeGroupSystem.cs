using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using UpgradableZenith.Content.Items;
using Terraria.Localization;

namespace UpgradableZenith
{
    public class RecipeGroupSystem:ModSystem
    {
        public const string CopperShortSwordRG = "UpgradableZenith:CopperShortSword";
        public const string EnchantedSwordRG = "UpgradableZenith:EnchantedSword";
        public const string StarfuryRG = "UpgradableZenith:Starfury";
        public const string LightBaneRG = "UpgradableZenith:LightBane";
        public const string BloodButchererRG = "UpgradableZenith:BloodBUtcherer";
        public const string BladeOfGrassRG = "UpgradableZenith:BladeOfGrass";
        public const string FieryGreatSwordRG = "UpgradableZenith:FieryGreatSword";
        public const string MuramasaRG = "UpgradableZenith:Muramasa";
        public const string BeeKeeperRG = "UpgradableZenith:BeekeeperRG";
        public const string NightsEdgeRG = "UpgradableZenith:NightsEdge";
        public const string ExcaliburRG = "UpgradableZenith:Excalibur";
        public const string TrueNightsEdgeRG = "UpgradableZenith:TrueNightsEdge";
        public const string TrueExcaliburRG = "UpgradableZenith:TrueExcalibur";
        public const string SeedlerRG = "UpgradableZenith:Seedler";
        public const string TheHorseManRG = "UpgradableZenith:TheHorseManRG";
        public const string TerraBladeRG = "UpgradableZenith:TerraBladeRG";
        public const string StarWrathRG = "UpgradableZenith:StarWrath";
        public const string MeowmereRG = "UpgradableZenith:Meowmere";
        public const string InfluexWaverRG = "UpgradableZenith:InfluexWaver";
        public const string TerragrimRG = "UpgradableZenith:Terragrim";
        public const string BreakerBladeRG = "UpgradableZenith:BreakerBlade";
        public const string CobaltSwordRG = "UpgradableZenith:CobaltSword";
        public const string PalladiumSwordRG = "UpgradableZenith:PalladiumSword";
        public const string MythrilSwordRG = "UpgradableZenith:MythrilSword";
        public const string OrichalcumSwordRG = "UpgradableZenith:OrichalcumSword";
        public const string AdamantiteSwordRG = "UpgradableZenith:AdamantiteSword";
        public const string TitaniumSwordRG = "UpgradableZenith:TitaniumSword";

        public const string GoldBarRG = "UpgradableZenith:GoldenBar";
        void FastGroupRegister(string name, string key, params int[] values)
        {
            RecipeGroup group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {name}", values);
            RecipeGroup.RegisterGroup(key, group);
        }
        public override void AddRecipeGroups()
        {
            FastGroupRegister("铜短剑", CopperShortSwordRG, ItemID.CopperShortsword, ModContent.ItemType<CopperZenith>());
            FastGroupRegister("附魔剑", EnchantedSwordRG, ItemID.EnchantedSword, ModContent.ItemType<EnchantedZenith>());
            FastGroupRegister("星怒", StarfuryRG, ItemID.Starfury, ModContent.ItemType<StarfuryZenith>());
            FastGroupRegister("光之驱逐", LightBaneRG, ItemID.LightsBane, ModContent.ItemType<LightsBaneZenith>());
            FastGroupRegister("血腥屠刀", BloodButchererRG, ItemID.BloodButcherer, ModContent.ItemType<BloodButchererZenith>());
            FastGroupRegister("草薙", BladeOfGrassRG, ItemID.BladeofGrass, ModContent.ItemType<BladeOfGrassZenith>());
            FastGroupRegister("炽焰巨剑", FieryGreatSwordRG, ItemID.FieryGreatsword, ModContent.ItemType<FieryGreatZenith>());
            FastGroupRegister("村正", MuramasaRG, ItemID.Muramasa, ModContent.ItemType<MuramasaZenith>());
            FastGroupRegister("养蜂人", BeeKeeperRG, ItemID.BeeKeeper, ModContent.ItemType<BeeKeeperZenith>());
            FastGroupRegister("永夜之刃", NightsEdgeRG, ItemID.NightsEdge, ModContent.ItemType<PermanentNightZenith>());
            FastGroupRegister("圣剑", ExcaliburRG, ItemID.Excalibur, ModContent.ItemType<ExcaliburZenith>());
            FastGroupRegister("真·永夜之刃", TrueNightsEdgeRG, ItemID.TrueNightsEdge, ModContent.ItemType<TrueNightsEdgeZenith>());
            FastGroupRegister("真·圣剑", TrueExcaliburRG, ItemID.TrueExcalibur, ModContent.ItemType<TrueExcaliburZenith>());
            FastGroupRegister("种子弯刀", SeedlerRG, ItemID.Seedler, ModContent.ItemType<SeedlerZenith>());
            FastGroupRegister("无头骑士之刃", TheHorseManRG, ItemID.TheHorsemansBlade, ModContent.ItemType<TheHorseManZenith>());
            FastGroupRegister("大地之刃", TerraBladeRG, ItemID.TerraBlade, ModContent.ItemType<TerraZentih>());
            FastGroupRegister("星辰之怒", StarWrathRG, ItemID.StarWrath, ModContent.ItemType<StarwrathZenith>());
            FastGroupRegister("喵刃", MeowmereRG, ItemID.Meowmere, ModContent.ItemType<MeowmereZenith>());
            FastGroupRegister("波涌之刃", InfluexWaverRG, ItemID.InfluxWaver, ModContent.ItemType<InfluexWaverZenith>());
            FastGroupRegister("次时代欧皇刀(划掉)泰拉魔刃", TerragrimRG, ItemID.Terragrim, ModContent.ItemType<TerragrimZenith>());
            FastGroupRegister("金锭", GoldBarRG, ItemID.GoldBar, ItemID.PlatinumBar);
            FastGroupRegister("毁灭刃", BreakerBladeRG, ItemID.TheBreaker, ModContent.ItemType<BreakerZenith>());
            FastGroupRegister("钴剑", CobaltSwordRG, ItemID.CobaltSword, ModContent.ItemType<CobaltZenith>());
            FastGroupRegister("钯金剑", PalladiumSwordRG, ItemID.PalladiumSword, ModContent.ItemType<PalladiumZenith>());
            FastGroupRegister("秘银剑", MythrilSwordRG, ItemID.MythrilSword, ModContent.ItemType<MythrilZenith>());
            FastGroupRegister("山铜剑", OrichalcumSwordRG, ItemID.OrichalcumSword, ModContent.ItemType<OrichalcumZenith>());
            FastGroupRegister("精金剑", AdamantiteSwordRG, ItemID.AdamantiteSword, ModContent.ItemType<AdamantiteZenith>());
            FastGroupRegister("钛金剑", TitaniumSwordRG, ItemID.TitaniumSword, ModContent.ItemType<TitaniumZenith>());
            base.AddRecipeGroups();
        }
        public override void AddRecipes()
        {

            for (int i = 0; i < 7; i++)
            {
                Recipe recipe = Recipe.Create(ItemID.Zenith);
                if (i % 2 == 1)
                {
                    recipe.AddRecipeGroup(CopperShortSwordRG);
                    recipe.AddRecipeGroup(EnchantedSwordRG);
                    recipe.AddRecipeGroup(StarfuryRG);
                }
                else
                {
                    recipe.AddIngredient<EnchantedStarZenith>();
                }
                if (i / 2 % 2 == 1)
                {
                    recipe.AddIngredient(ItemID.TerraBlade);
                    recipe.AddRecipeGroup(SeedlerRG);
                    recipe.AddRecipeGroup(TheHorseManRG);
                }
                else
                {
                    recipe.AddIngredient<TerraZentih>();

                }
                if (i / 4 % 2 == 1)
                {
                    recipe.AddRecipeGroup(MeowmereRG);
                    recipe.AddRecipeGroup(InfluexWaverRG);
                    recipe.AddRecipeGroup(StarWrathRG);
                }
                else
                {
                    recipe.AddIngredient<Henitz>();
                }
                recipe.AddTile(TileID.MythrilAnvil);
                recipe.Register();
            }
            if (ModContent.GetInstance<UpgradeableZenithConfig>().ExtraRecipeInFTW) 
            {
                Recipe recipe1 = Recipe.Create(ItemID.GoldenKey);
                recipe1.AddRecipeGroup(GoldBarRG, 5);
                recipe1.AddTile(TileID.Anvils);
                recipe1.AddCondition(Condition.ForTheWorthyWorld);
                recipe1.Register();


                recipe1 = Recipe.Create(ItemID.SkyMill);
                recipe1.AddRecipeGroup(GoldBarRG, 2);
                recipe1.AddTile(TileID.Anvils);
                recipe1.AddIngredient(ItemID.Cloud, 20);
                recipe1.AddIngredient(ItemID.RainCloud, 5);
                recipe1.AddCondition(Condition.ForTheWorthyWorld);
                recipe1.AddIngredient(ItemID.SunplateBlock, 30);
                recipe1.Register();

                recipe1 = Recipe.Create(ItemID.HoneyDispenser);
                recipe1.AddRecipeGroup(GoldBarRG, 2);
                recipe1.AddIngredient(ItemID.HoneyBlock, 20);
                recipe1.AddIngredient(ItemID.CrispyHoneyBlock, 5);
                recipe1.AddCondition(Condition.ForTheWorthyWorld);
                recipe1.AddIngredient(ItemID.Hive, 30);
                recipe1.AddTile(TileID.Anvils);
                recipe1.Register();

                recipe1 = Recipe.Create(ItemID.BoneWelder);
                recipe1.AddRecipeGroup(GoldBarRG, 2);
                recipe1.AddIngredient(ItemID.Bone, 100);
                recipe1.AddTile(TileID.DemonAltar);
                recipe1.AddCondition(Condition.ForTheWorthyWorld);
                recipe1.Register();
            }



            base.AddRecipes();
        }
    }
}
