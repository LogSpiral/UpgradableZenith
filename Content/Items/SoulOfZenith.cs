using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.Effects;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.Graphics;
using Terraria.GameContent;
using Terraria.GameContent.UI.Chat;
using Terraria.UI.Chat;
using Terraria.ModLoader.IO;

namespace UpgradableZenith.Content.Items
{
    public class SoulOfZenithLoot : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.MoonLordCore)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SoulOfZenith>()));
            }
            base.ModifyNPCLoot(npc, npcLoot);
        }
    }
    public class ColorSyncHelper : GlobalProjectile
    {
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            if (UpgradableZenithSystem.Timer % 60 == 0 && projectile.type == ProjectileID.FinalFractal)
            {
                int type = (int)projectile.ai[1];
                if (!FinalFractalHelper._fractalProfiles.ContainsKey(type))
                    SoulOfZenith.RegisterZenith(type);
            }
            return base.PreDraw(projectile, ref lightColor);
        }
    }
    public class SoulOfZenith : TinyZenith
    {
        ZenithInfo info;
        public override ZenithInfo Info
        {
            get
            {
                if (info.projectileItemTypes == null)
                    UpdateInfo();
                return info;

            }
        }
        public static void RegisterZenith(int type)
        {
            if (type != ItemID.None && !FinalFractalHelper._fractalProfiles.ContainsKey(type))
            {
                Main.instance.LoadItem(type);
                var tex = TextureAssets.Item[type].Value;
                var width = tex.Width;
                var height = tex.Height;
                var pixelColor = new Color[width * height];

                tex.GetData(pixelColor);
                Vector4 vcolor = default;
                float count = 0;
                for (int n = 0; n < pixelColor.Length; n++)
                {
                    if (pixelColor[n] != default && (n - width < 0 || pixelColor[n - width] != default) && (n - 1 < 0 || pixelColor[n - 1] != default) &&
                        (n + width >= pixelColor.Length || pixelColor[n + width] != default) && (n + 1 >= pixelColor.Length || pixelColor[n + 1] != default))
                    {
                        var weight = (float)((n + 1) % width * (height - n / width)) / width / height;
                        vcolor += pixelColor[n].ToVector4() * weight;
                        count += weight;
                    }
                    Vector2 coord = new Vector2(n % width, n / width);
                    coord /= new Vector2(width, height);
                }
                vcolor /= count;
                var newColor = new Color(vcolor.X, vcolor.Y, vcolor.Z, vcolor.W);
                FinalFractalHelper._fractalProfiles[type] = new(tex.Size().Length(), newColor);
            }
        }
        public void UpdateInfo()
        {
            if (Main.gameMenu) return;
            ZenithPlayer zenithPlayer = Main.LocalPlayer.GetModPlayer<ZenithPlayer>();
            var list = zenithPlayer.zenithizeWeapons;
            int id = ItemID.None;
            int damage = 0;
            int crit = 0;
            float knockBack = 0;
            int value = Item.sellPrice(0, 10, 0, 0);
            int useTime = 1000;
            List<int> types = new List<int>();
            foreach (var weapon in list)
            {
                if (weapon == null || weapon.type <= ItemID.None)
                    continue;
                if (id == ItemID.None && weapon.type != ItemID.None) id = weapon.type;
                if (damage < weapon.damage) damage = weapon.damage;
                if (crit < weapon.crit) crit = weapon.crit;
                if (knockBack < weapon.knockBack) knockBack = weapon.knockBack;
                value += weapon.value;
                if (useTime > weapon.useAnimation) useTime = weapon.useAnimation;
                types.Add(weapon.type);

                if (Main.netMode != NetmodeID.Server)
                    RegisterZenith(weapon.type);
            }
            if (useTime == 1000 || useTime == 0) useTime = 30;
            info = new ZenithInfo(id, damage, crit, knockBack, value, ItemRarityID.Purple, useTime, types.ToArray(), true);
        }
        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                var ui = ZenithSystem.instance.zenithUI;
                if (!ZenithUI.Visible)
                {
                    ui.Open();
                }
                else
                    ui.Close();


                var vec = Main.MouseScreen;
                float zoom = Main.GameZoomTarget * Main.ForcedMinimumZoom;
                vec = (Main.MouseScreen - Main.ScreenSize.ToVector2() * .5f) * zoom + Main.ScreenSize.ToVector2() * .5f;
                vec /= Main.UIScale;
                ui.MainPanel.Top.Set(vec.Y, 0);
                ui.MainPanel.Left.Set(vec.X, 0);


                //player.GetModPlayer<ZenithPlayer>().zenithizeWeapons.Clear();
                //player.GetModPlayer<ZenithPlayer>().zenithizeWeapons.Add(new Item(ItemID.FetidBaghnakhs));
                //player.GetModPlayer<ZenithPlayer>().zenithizeWeapons.Add(new Item(ItemID.TheHorsemansBlade));
                //ItemID.Sets.Deprecated[ItemID.FirstFractal] = false;
                //player.GetModPlayer<ZenithPlayer>().zenithizeWeapons.Add(new Item(ItemID.FirstFractal));
                //Item item;
                //do
                //{
                //    item = new Item(Main.rand.Next(1, ItemLoader.ItemCount));
                //} while (item.damage < 1);
                //player.GetModPlayer<ZenithPlayer>().zenithizeWeapons.Add(item);



                UpdateInfo();
                ui.Refresh();

                ui.Recalculate();

                SetDefaults();

                Item.UseSound = default;
            }
            else
            {
                Item.UseSound = SoundID.Item169;
            }
            return base.UseItem(player);
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
                return false;

            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
    }
    public class ZenithPlayer : ModPlayer
    {
        public Item[] zenithizeWeapons = new Item[200];
        //public List<Item> zenithizeWeapons = new();//[new Item(), new Item(), new Item(), new Item(), new Item(), new Item(), new Item(), new Item(), new Item(), new Item(), new Item(), new Item()];
        public override void SaveData(TagCompound tag)
        {
            //for (int i = 0; i < 200; i++)
            //    if (zenithizeWeapons[i] == null)
            //        zenithizeWeapons[i] = new Item();
            //zenithizeWeapons = new Item[200];
            for (int i = 0; i < 200; i++)
                if (zenithizeWeapons[i] == null)
                    zenithizeWeapons[i] = new Item();
            tag["weapons"] = zenithizeWeapons;
            base.SaveData(tag);
        }
        public override void LoadData(TagCompound tag)
        {
            //zenithizeWeapons = tag.Get<Item[]>("weapons") ?? new Item[200];

            var array = tag.Get<Item[]>("weapons");
            if (array != null && array.Length == 200)
                zenithizeWeapons = array;
            else
                for (int n = 0; n < 200; n++)
                    zenithizeWeapons[n] = new Item();
            base.LoadData(tag);
        }
    }
    public class ZenithUI : UIState
    {
        public static bool Visible;
        public MultiList multiList;
        public UIScrollbar UIScrollbar;
        public UIPanel MainPanel;
        public override void OnInitialize()
        {
            MainPanel = new UIPanel();
            MainPanel.MaxHeight.Set(1200, 0);
            MainPanel.Width.Set(600, 0);
            MainPanel.Left.Set(200, 0);
            MainPanel.Height.Set(1200, 0);
            multiList = new MultiList();
            multiList.Width.Set(600, 0);
            multiList.Height.Set(1200, 0);
            multiList.MaxHeight.Set(1200, 0);
            //UIScrollbar = new UIScrollbar();
            //UIScrollbar.Height.Set(0, 1);
            //UIScrollbar.HAlign = 1f;
            //UIScrollbar.SetView(100, 1000);
            //multiList.SetScrollbar(UIScrollbar);
            MainPanel.Append(multiList);
            //MainPanel.Append(UIScrollbar);
            Append(MainPanel);
            base.OnInitialize();
        }

        public void Refresh()
        {
            multiList.Clear();
            int c = 0;
            var array = Main.LocalPlayer.GetModPlayer<ZenithPlayer>().zenithizeWeapons.ToArray();
            foreach (var i in array)
            {
                ModItemSlot uIItemSlot = new ModItemSlot(0.85f, "UpgradableZenith/Content/Items/Infinite_Icons_Weapon", () => "请在这里放上近战武器");
                int index = c;
                uIItemSlot.OnItemChange += (sender, e) =>
                {
                    if (Main.LocalPlayer is not null && Main.LocalPlayer.TryGetModPlayer(out ZenithPlayer zenith))
                    {

                        zenith.zenithizeWeapons[index] = uIItemSlot.Item;
                        foreach (var i in Main.LocalPlayer.inventory)
                            if (i.ModItem is SoulOfZenith soz)
                            {
                                soz.UpdateInfo();
                                soz.SetDefaults();
                            }

                    }

                };
                uIItemSlot.OnUpdate += e =>
                {
                    if (Main.LocalPlayer is not null && Main.LocalPlayer.TryGetModPlayer(out ZenithPlayer zenith))
                    {
                        uIItemSlot.Item = zenith.zenithizeWeapons[index];
                    }
                };
                multiList.Add(uIItemSlot);

                c++;
            }

        }
        public void Open()
        {
            SoundEngine.PlaySound(SoundID.MenuOpen, Main.LocalPlayer.Center);
            //Refresh();
            Visible = true;
        }
        public void Close()
        {
            SoundEngine.PlaySound(SoundID.MenuClose, Main.LocalPlayer.Center);
            Visible = false;

        }
    }
    public class MultiList : UIList
    {
        public override void RecalculateChildren()
        {
            base.RecalculateChildren();
            int itemCount = 0;
            float pixels = 0f;
            foreach (var u in _items)
            {
                if (itemCount >= 10f)
                {
                    itemCount = 0;
                    pixels += 60f;
                }

                //// Filter Message
                //if (u is UIPanel and not ConciseUIModItem)
                //{
                //    pixels += u.Height.Pixels + 8f;
                //    continue;
                //}

                u.Left.Pixels = itemCount * 60f;
                u.Top.Pixels = pixels;
                itemCount++;
                u.Recalculate();
            }
            _innerListHeight = pixels + 120f;
        }
    }
    /// <summary>
    /// 一个仿原版制作的物品UI格，由于是单独的所以应该适配|  来自QOT
    /// </summary>
    public class ModItemSlot : UIElement
    {
        /// <summary>
        /// 无物品时显示的贴图
        /// </summary>
        private readonly Asset<Texture2D> _emptyTexture;
        public float emptyTextureScale = 1f;
        public float emptyTextureOpacity = 0.5f;
        private readonly Func<string> _emptyText; // 无物品的悬停文本

        public Item Item;
        public float Scale = 1f;

        /// <summary>
        /// 是否使用基于Shader的圆润边框
        /// </summary>
        public bool RoundBorder = true;
        /// <summary>
        /// 是否可交互，否则不能执行左右键操作
        /// </summary>
        public bool Interactable = true;
        /// <summary>
        /// 该槽位内的饰品/装备是否可在被右键时自动装备
        /// </summary>
        public bool AllowSwapEquip;
        /// <summary>
        /// 该槽位内的物品可否Alt键收藏
        /// </summary>
        public bool AllowFavorite;

        /// <summary>
        /// 物品槽UI元件
        /// </summary>
        /// <param name="scale">物品在槽内显示的大小，0.85是游戏内物品栏的大小</param>
        /// <param name="emptyTexturePath">当槽内无物品时，显示的贴图</param>
        /// <param name="emptyText">当槽内无物品时，悬停显示的文本</param>
        public ModItemSlot(float scale = 0.85f, string emptyTexturePath = null, Func<string> emptyText = null)
        {
            this.Width.Set(52, 0);
            this.Height.Set(52, 0);
            Item = new Item();
            Item.SetDefaults();
            Scale = scale;
            AllowSwapEquip = false;
            AllowFavorite = true;
            if (emptyTexturePath is not null && ModContent.HasAsset(emptyTexturePath))
            {
                _emptyTexture = ModContent.Request<Texture2D>(emptyTexturePath);
            }
            _emptyText = emptyText;
        }

        /// <summary>
        /// 改原版的<see cref="Main.cursorOverride"/>
        /// </summary>
        private void SetCursorOverride()
        {
            if (!Item.IsAir)
            {
                if (!Item.favorited && ItemSlot.ShiftInUse)
                {
                    Main.cursorOverride = CursorOverrideID.ChestToInventory; // 快捷放回物品栏图标
                }
                if (Main.keyState.IsKeyDown(Main.FavoriteKey))
                {
                    if (AllowFavorite)
                    {
                        Main.cursorOverride = CursorOverrideID.FavoriteStar; // 收藏图标
                    }
                    if (Main.drawingPlayerChat)
                    {
                        Main.cursorOverride = CursorOverrideID.Magnifiers; // 放大镜图标 - 输入到聊天框
                    }
                }
                void TryTrashCursorOverride()
                {
                    if (!Item.favorited)
                    {
                        if (Main.npcShop > 0)
                        {
                            Main.cursorOverride = CursorOverrideID.QuickSell; // 卖出图标
                        }
                        else
                        {
                            Main.cursorOverride = CursorOverrideID.TrashCan; // 垃圾箱图标
                        }
                    }
                }
                if (ItemSlot.ControlInUse && ItemSlot.Options.DisableLeftShiftTrashCan && !ItemSlot.ShiftForcedOn)
                {
                    TryTrashCursorOverride();
                }
                // 如果左Shift快速丢弃打开了，按原版物品栏的物品应该是丢弃，但是我们这应该算箱子物品，所以不丢弃
                //if (!ItemSlot.Options.DisableLeftShiftTrashCan && ItemSlot.ShiftInUse) {
                //    TryTrashCursorOverride();
                //}
            }
        }

        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (Item is null)
            {
                Item = new Item(0);
                ItemChange();
            }

            // 在Panel外的也有IsMouseHovering
            var dimensions = GetDimensions();
            bool isMouseHovering = dimensions.ToRectangle().Contains(Main.MouseScreen.ToPoint());

            int lastStack = Item.stack;
            int lastType = Item.type;
            // 我把右键长按放在这里执行了
            if (isMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true;
                if (Interactable)
                {
                    SetCursorOverride();
                    // 伪装，然后进行原版右键尝试
                    // 千万不要伪装成箱子，因为那样多人会传同步信息，然后理所当然得出Bug
                    if (Item is not null && !Item.IsAir)
                    {
                        ItemSlot.RightClick(ref Item, AllowSwapEquip ? ItemSlot.Context.InventoryItem : ItemSlot.Context.CreativeSacrifice);
                    }
                }
                DrawText();
            }
            if (lastStack != Item.stack || lastType != Item.type)
            {
                ItemChange(true);
                RightClickItemChange(Item.stack - lastStack, lastType != Item.type);
                Main.playerInventory = true;
            }

            Vector2 origin = GetDimensions().Position();

            //if (RoundBorder)
            //{
            //    var borderColor = Item.favorited ? UIStyle.ItemSlotBorderFav : UIStyle.ItemSlotBorder;
            //    var background = Item.favorited ? UIStyle.ItemSlotBgFav : UIStyle.ItemSlotBg;
            //    SDFRectangle.HasBorder(dimensions.Position(), dimensions.Size(),
            //        new Vector4(UIStyle.ItemSlotBorderRound), background, UIStyle.ItemSlotBorderSize, borderColor);
            //}

            // 这里设置inventoryScale原版也是这么干的
            float oldScale = Main.inventoryScale;
            Main.inventoryScale = Scale;

            // 假装自己是一个物品栏物品拿去绘制
            var temp = new Item[11];
            // 如果用圆润边框，就假装为ChatItem，不会绘制原版边框
            int context = RoundBorder ? ItemSlot.Context.ChatItem : ItemSlot.Context.InventoryItem;
            temp[10] = Item;
            ItemSlot.Draw(Main.spriteBatch, temp, context, 10, origin);

            Main.inventoryScale = oldScale;

            // 空物品的话显示空贴图
            if (Item.IsAir)
            {
                if (_emptyText is not null && isMouseHovering && Main.mouseItem.IsAir)
                {
                    Main.instance.MouseText(_emptyText.Invoke());
                }
                if (_emptyTexture is not null)
                {
                    origin = _emptyTexture.Size() / 2f;
                    spriteBatch.Draw(_emptyTexture.Value, GetDimensions().Center(), null, Color.White * emptyTextureOpacity, 0f, origin, emptyTextureScale, SpriteEffects.None, 0f);
                }
            }
        }

        /// <summary>
        /// 修改MouseText的
        /// </summary>
        public void DrawText()
        {
            if (!Item.IsAir && (Main.mouseItem is null || Main.mouseItem.IsAir))
            {
                Main.HoverItem = Item.Clone();
                Main.instance.MouseText(string.Empty);
            }
        }
        /// <summary>
        /// 强制性的可否放置判断，只有满足条件才能放置物品，没有例外|  来自QOT
        /// </summary>
        /// <param name="slotItem">槽内物品</param>
        /// <param name="mouseItem">手持物品</param>
        /// <returns>
        /// 强制判断返回值，判断放物类型
        /// 0: 不可放物<br/>
        /// 1: 两物品不同，应该切换<br/>
        /// 2: 两物品相同，应该堆叠<br/>
        /// 3: 槽内物品为空，应该切换<br/>
        /// </returns>
        public static byte CanPlaceInSlot(Item slotItem, Item mouseItem)
        {
            if (slotItem.IsAir)
                return 3;
            if (mouseItem.type != slotItem.type || mouseItem.prefix != slotItem.prefix)
                return 1;
            if (!slotItem.IsAir && slotItem.stack < slotItem.maxStack && ItemLoader.CanStack(slotItem, mouseItem))
                return 2;
            return 0;
        }
        public void LeftClickItem(ref Item placeItem)
        {
            // 放大镜图标 - 输入到聊天框
            if (Main.cursorOverride == CursorOverrideID.Magnifiers)
            {
                if (ChatManager.AddChatText(FontAssets.MouseText.Value, ItemTagHandler.GenerateTag(Item), Vector2.One))
                    SoundEngine.PlaySound(SoundID.MenuTick);
                return;
            }

            // 收藏图标
            if (Main.cursorOverride == CursorOverrideID.FavoriteStar)
            {
                Item.favorited = !Item.favorited;
                SoundEngine.PlaySound(SoundID.MenuTick);
                return;
            }

            // 垃圾箱图标
            if (Main.cursorOverride == CursorOverrideID.TrashCan)
            {
                // 假装自己是一个物品栏物品
                var temp = new Item[1];
                temp[0] = Item;
                ItemSlot.SellOrTrash(temp, ItemSlot.Context.InventoryItem, 0);
                return;
            }

            // 放回物品栏图标
            if (Main.cursorOverride == CursorOverrideID.ChestToInventory)
            {
                int oldStack = Item.stack;
                Item = Main.player[Main.myPlayer].GetItem(Main.myPlayer, Item, GetItemSettings.InventoryEntityToPlayerInventorySettings);
                if (Item.stack != oldStack) // 成功了
                {
                    if (Item.stack <= 0)
                        Item.SetDefaults();
                    SoundEngine.PlaySound(SoundID.Grab);
                }
                return;
            }

            if (Main.mouseItem.IsAir && Item.IsAir) return;

            // 常规单点
            if (placeItem is not null && CanPlaceItem(placeItem))
            {
                byte placeMode = CanPlaceInSlot(Item, placeItem);

                // type不同直接切换吧
                if (placeMode is 1 or 3)
                {
                    SwapItem(ref placeItem);
                    SoundEngine.PlaySound(SoundID.Grab);
                    Main.playerInventory = true;
                    return;
                }
                // type相同，里面的能堆叠，放进去
                if (placeMode is 2)
                {
                    ItemLoader.TryStackItems(Item, placeItem, out _);
                    SoundEngine.PlaySound(SoundID.Grab);
                }
            }
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            base.LeftMouseDown(evt);

            if (!Main.LocalPlayer.ItemTimeIsZero || Main.LocalPlayer.itemAnimation != 0 || !Interactable)
            {
                return;
            }

            if (Item is null)
            {
                Item = new Item();
                Item.SetDefaults();
                ItemChange();
            }

            int lastStack = Item.stack;
            int lastType = Item.type;
            int lastPrefix = Item.prefix;

            SetCursorOverride(); // Click在Update执行，因此必须在这里设置一次
            LeftClickItem(ref Main.mouseItem);

            if (lastStack != Item.stack || lastType != Item.type || lastPrefix != Item.prefix)
            {
                ItemChange();
            }
        }

        /// <summary>
        /// 可以在这里写额外的物品放置判定，第一个Item是当前槽位存储物品，第二个Item是<see cref="Main.mouseItem"/>
        /// </summary>
        public Func<Item, Item, bool> OnCanPlaceItem;
        public bool CanPlaceItem(Item item)
        {
            bool canPlace = true;

            if (Item is null)
            {
                Item = new Item();
                Item.SetDefaults();
            }

            if (OnCanPlaceItem is not null)
            {
                canPlace = OnCanPlaceItem.Invoke(Item, item);
            }

            return canPlace;
        }

        /// <summary>
        /// 物品改变后执行，可以写保存之类的
        /// </summary>
        public Action<Item, bool> OnItemChange;
        public virtual void ItemChange(bool rightClick = false)
        {
            if (Item is null)
            {
                Item = new Item();
                Item.SetDefaults();
            }

            if (OnItemChange is not null)
            {
                OnItemChange.Invoke(Item, rightClick);
            }
        }

        /// <summary>
        /// 右键物品改变了才执行
        /// </summary>
        public Action<Item, int, bool> OnRightClickItemChange;
        public virtual void RightClickItemChange(int stackChange, bool typeChange)
        {
            if (Item is null)
            {
                Item = new Item();
                Item.SetDefaults();
            }

            if (OnRightClickItemChange is not null)
            {
                OnRightClickItemChange.Invoke(Item, stackChange, typeChange);
            }
        }

        public void SwapItem(ref Item item)
        {
            Utils.Swap(ref item, ref Item);
        }

        public void Unload()
        {
            Item = null;
        }
    }
    public class ZenithSystem : ModSystem
    {
        public ZenithUI zenithUI;
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                instance = this;
                zenithUI = new ZenithUI();
                userInterface = new UserInterface();
                userInterface.SetState(zenithUI);
            }
        }
        public static ZenithSystem instance;
        public UserInterface userInterface;
        public static ModKeybind ShowScreenProjectorKeybind { get; private set; }
        public override void UpdateUI(GameTime gameTime)
        {
            if (ZenithUI.Visible)
            {
                userInterface?.Update(gameTime);
            }
            base.UpdateUI(gameTime);
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int MouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (MouseTextIndex != -1)
            {
                layers.Insert(MouseTextIndex, new LegacyGameInterfaceLayer(
                   "UpgradableZenith:ZenithUI",
                   delegate
                   {
                       if (ZenithUI.Visible)
                           zenithUI.Draw(Main.spriteBatch);
                       return true;
                   },
                   InterfaceScaleType.UI)
               );
            }
            base.ModifyInterfaceLayers(layers);
        }
    }
}
