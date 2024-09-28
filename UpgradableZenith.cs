using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.UI;
using Terraria.Graphics;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using UpgradableZenith.Content.Items;

namespace UpgradableZenith
{
    // Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
    public class UpgradableZenith : Mod
    {

    }
    public class UpgradableZenithSystem : ModSystem
    {
        public static Dictionary<int, FinalFractalHelper.FinalFractalProfile> vanillaProfiles = new();
        public override void PostSetupContent()
        {
            vanillaProfiles.Clear();
            foreach (var p in FinalFractalHelper._fractalProfiles)
                vanillaProfiles.Add(p.Key, p.Value);

            FinalFractalHelper._fractalProfiles[ItemID.BreakerBlade] = new(122, Color.LightCyan);
            FinalFractalHelper._fractalProfiles[ItemID.CobaltSword] = new(80, Color.Blue);
            FinalFractalHelper._fractalProfiles[ItemID.PalladiumSword] = new(78, Color.Orange);
            FinalFractalHelper._fractalProfiles[ItemID.MythrilSword] = new(82, new Color(11, 64, 98));
            FinalFractalHelper._fractalProfiles[ItemID.OrichalcumSword] = new(76, Color.HotPink);
            FinalFractalHelper._fractalProfiles[ItemID.AdamantiteSword] = new(85, Color.Red);
            FinalFractalHelper._fractalProfiles[ItemID.TitaniumSword] = new(85, Color.Cyan);



            if (Main.netMode != NetmodeID.Server)
                foreach (var i in FinalFractalHelper._fractalProfiles.Keys)
                    Main.instance.LoadItem(i);
            base.PostSetupContent();
        }
        public override void Load()
        {
            On_FinalFractalHelper.GetRandomProfileIndex += On_FinalFractalHelper_GetRandomProfileIndex; ;
            base.Load();
        }

        private int On_FinalFractalHelper_GetRandomProfileIndex(On_FinalFractalHelper.orig_GetRandomProfileIndex orig)
        {
            List<int> list = (ModContent.GetInstance<UpgradeableZenithConfig>().UseVanillaZenithModify ? FinalFractalHelper._fractalProfiles : vanillaProfiles).Keys.ToList();
            int index = Main.rand.Next(list.Count);
            if (list[index] == 4956)
            {
                list.RemoveAt(index);
                index = Main.rand.Next(list.Count);
            }
            return list[index];
        }

        public override void Unload()
        {
            On_FinalFractalHelper.GetRandomProfileIndex -= On_FinalFractalHelper_GetRandomProfileIndex;
            FinalFractalHelper._fractalProfiles.Clear();
            foreach (var p in vanillaProfiles)
                FinalFractalHelper._fractalProfiles.Add(p.Key, p.Value);
            base.Unload();
        }
        public static int Timer;
        public override void UpdateUI(GameTime gameTime)
        {
            Timer++;
            base.UpdateUI(gameTime);
        }

    }
    public class UpgradeableZenithConfig : ModConfig
    {
        public static bool FTWDamageImprove;
        public override void OnChanged()
        {
            FTWDamageImprove = DamageImproveInFTW;
            base.OnChanged();
        }
        public override ConfigScope Mode => ConfigScope.ServerSide;
        [DefaultValue(true)]
        public bool UseSpecificHitEffect = true;

        [DefaultValue(false)]
        public bool UseVanillaZenithModify = false;

        [DefaultValue(true)]
        public bool DamageImproveInFTW = true;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool ExtraRecipeInFTW = true;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool RecipeEZInFTW = true;
        //public Dictionary<ItemDefinition, ZenithProfile> zenithInfo
        //{
        //    get => (from pair in FinalFractalHelper._fractalProfiles select KeyValuePair.Create(new ItemDefinition(pair.Key), new ZenithProfile() { color = pair.Value.trailColor, length = pair.Value.trailWidth })).ToDictionary();
        //}

        //public class ZenithProfile
        //{
        //    public Color color;
        //    public float length;
        //}
    }


}
