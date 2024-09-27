using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using UpgradableZenith.Content.Items;
using Microsoft.Xna.Framework;

namespace UpgradableZenith
{
    public class ZenithEffectModifier : GlobalProjectile
    {
        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (projectile.type != ProjectileID.FinalFractal)
                return;
            if (!ModContent.GetInstance<UpgradeableZenithConfig>().UseSpecificHitEffect)
                return;
            if (!target.CanBeChasedBy())
                return;
            Player player = Main.player[projectile.owner];
            int type = (int)projectile.ai[1];
            if (type == ItemID.BreakerBlade)
            {
                if (target.life >= target.lifeMax * .9f)
                {
                    modifiers.FinalDamage *= 2.5f;
                }
            }
            base.ModifyHitNPC(projectile, target, ref modifiers);
        }
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (projectile.type != ProjectileID.FinalFractal)
                return;
            if (!ModContent.GetInstance<UpgradeableZenithConfig>().UseSpecificHitEffect)
                return;
            if (!target.CanBeChasedBy())
                return;
            Player player = Main.player[projectile.owner];
            int type = (int)projectile.ai[1];
            if (new int[] { ItemID.NightsEdge, ItemID.Excalibur, ItemID.TrueNightsEdge, ItemID.TrueExcalibur, ItemID.TerraBlade }.Contains(type))
            {
                Vector2 positionInWorld = Main.rand.NextVector2FromRectangle(target.Hitbox);
                ParticleOrchestraSettings particleOrchestraSettings = default(ParticleOrchestraSettings);
                particleOrchestraSettings.PositionInWorld = positionInWorld;
                ParticleOrchestraSettings settings = particleOrchestraSettings;
                ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, type switch
                {
                    ItemID.NightsEdge => ParticleOrchestraType.NightsEdge,
                    ItemID.TrueNightsEdge => ParticleOrchestraType.TrueNightsEdge,
                    ItemID.Excalibur => ParticleOrchestraType.Excalibur,
                    ItemID.TrueExcalibur => ParticleOrchestraType.TrueExcalibur,
                    ItemID.TerraBlade => ParticleOrchestraType.TerraBlade
                }, settings, player.whoAmI);
            }

            if (!Main.rand.NextBool(3))
                //foreach (var i in FinalFractalHelper._fractalProfiles.Keys)
                //    Main.player[projectile.owner].QuickSpawnItem(null, i);
                switch (type)
                {
                    //case ItemID.Starfury://
                    case ItemID.InfluxWaver://
                    case ItemID.EnchantedSword://
                    case ItemID.Meowmere://
                                         //case ItemID.StarWrath://
                    case ItemID.TerraBlade:
                    case ItemID.Seedler://
                    case ItemID.Terragrim:
                    case ItemID.Zenith://
                                       //case ItemID.CopperShortsword:
                    case ItemID.TrueNightsEdge:
                    case ItemID.BladeofGrass://
                    case ItemID.TrueExcalibur:
                        {
                            float ai0 = 0;
                            float ai1 = 0;
                            float ai2 = 0;
                            if (type == ItemID.BladeofGrass)
                            {
                                ai0 = Main.rand.NextFloat(-0.25f, 0.25f);
                                //ai1 = 60;
                            }
                            Vector2 unit = Main.rand.NextVector2Unit();
                            Item item = new Item(type);
                            // + Main.rand.NextFloat(16, 64) * Main.rand.NextVector2Unit()
                            int projType = item.shoot;
                            if (type == ItemID.TrueNightsEdge)
                            {
                                if (Main.rand.NextBool(3))
                                {
                                    projType = ProjectileID.TrueNightsEdge;
                                    (ai0, ai1, ai2) = ((float)player.direction * player.gravDir, 32f, player.GetAdjustedItemScale(item));

                                }
                                else
                                {
                                    projType = ProjectileID.NightBeam;
                                }
                            }
                            if (type == ItemID.TrueExcalibur)
                            {
                                projType = ProjectileID.LightBeam;
                            }
                            if (type == ItemID.TerraBlade || type == ItemID.Terragrim)
                            {
                                if (Main.rand.NextBool(3))
                                {
                                    projType = ProjectileID.TerraBlade2Shot;
                                    (ai0, ai1, ai2) = ((float)player.direction * player.gravDir, 18f, player.GetAdjustedItemScale(item) * 1.5f);

                                }
                                else
                                {
                                    projType = ProjectileID.TerraBeam;
                                }
                            }
                            var proj = Projectile.NewProjectileDirect(player.GetSource_FromThis(), target.Center + unit * 128, -unit * 16, projType, damageDone, hit.Knockback, player.whoAmI, ai0, ai1, ai2);
                            if (type == ItemID.EnchantedSword || type == ItemID.TrueExcalibur || type == ItemID.TerraBlade)
                            {

                                proj.penetrate = -1;
                                proj.timeLeft = 30;
                                if (projType == ProjectileID.TerraBlade2Shot)
                                {
                                    proj.timeLeft = 90;
                                    proj.velocity *= 4;
                                }
                            }
                            break;
                        }
                    case ItemID.StarWrath:
                        {
                            Vector2 vector17 = target.Center;
                            float num41 = vector17.Y;
                            if (num41 > player.Center.Y - 200f)
                                num41 = player.Center.Y - 200f;
                            for (int num42 = 0; num42 < 1; num42++)
                            {
                                var pointPoisition = player.Center + new Vector2(-Main.rand.Next(0, 401) * player.direction, -600f);
                                pointPoisition.Y -= 100 * Main.rand.NextFloat(1, 3);
                                Vector2 vector18 = vector17 - pointPoisition;
                                Vector2 tarVec = target.Center - pointPoisition;

                                if (vector18.Y < 0f)
                                    vector18.Y *= -1f;

                                if (vector18.Y < 20f)
                                    vector18.Y = 20f;

                                vector18.Normalize();
                                vector18 *= 16;
                                Projectile.NewProjectile(player.GetProjectileSource_Item(new Item(ItemID.StarWrath)), pointPoisition, vector18 + Main.rand.Next(-40, 41) * 0.02f * Vector2.UnitY, ProjectileID.StarWrath, damageDone, hit.Knockback, player.whoAmI, 0f, num41);
                            }
                            break;
                        }
                    case ItemID.Starfury:
                        {
                            var pointPoisition = new Vector2(player.position.X + (float)player.width * 0.5f + (float)(Main.rand.Next(201) * -player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);

                            Vector2 vector59 = Main.MouseWorld - pointPoisition;
                            Vector2 mouseWorld2 = Main.MouseWorld;
                            Vector2 vec = mouseWorld2;
                            Vector2 vector60 = (pointPoisition - mouseWorld2).SafeNormalize(new Vector2(0f, -1f));
                            while (vec.Y > pointPoisition.Y && WorldGen.SolidTile(vec.ToTileCoordinates()))
                            {
                                vec += vector60 * 16f;
                            }
                            vector59.Normalize();
                            vector59 *= 16;
                            Projectile.NewProjectile(player.GetProjectileSource_Item(new Item(ItemID.Starfury)), pointPoisition, vector59, ProjectileID.Starfury, damageDone, hit.Knockback, player.whoAmI, 0f, vec.Y);
                            break;
                        }
                    case ItemID.BeeKeeper:
                    case ItemID.FieryGreatsword:
                    case ItemID.Muramasa:
                    case ItemID.BloodButcherer:
                        {
                            player._spawnTentacleSpikes = true;

                            player._spawnBloodButcherer = true;

                            player._spawnVolcanoExplosion = true;

                            player._spawnMuramasaCut = true;
                            player.ApplyNPCOnHitEffects(new Item(type), target.Hitbox, hit._sourceDamage, hit.Knockback, target.whoAmI, damageDone, damageDone);

                            break;
                        }

                    case ItemID.Excalibur:
                    case ItemID.NightsEdge:
                        //case ItemID.TrueNightsEdge:
                        {
                            //Item item = new Item(type);
                            //var unit = Main.rand.NextVector2Unit();
                            //var (ai0, ai1, ai2) = ((float)player.direction * player.gravDir, 32f, player.GetAdjustedItemScale(item) * 1.5f);

                            //var proj = Projectile.NewProjectileDirect(player.GetSource_FromThis(), target.Center + unit * 128, -unit * 16, item.shoot, damageDone, hit.Knockback, player.whoAmI, ai0, ai1, ai2);

                            break;
                        }
                    case ItemID.TheHorsemansBlade:
                        {
                            player.HorsemansBlade_SpawnPumpkin(target.whoAmI, damageDone, hit.Knockback);
                            break;

                        }
                    case ItemID.LightsBane:
                        {
                            float ai8 = 1f;
                            if (Main.rand.Next(100) < player.GetCritChance(DamageClass.Melee))
                            {
                                ai8 = 2f;
                                damageDone *= 2;
                            }
                            Projectile.NewProjectile(player.GetProjectileSource_Item(new Item(ItemID.LightsBane)), target.Center, Main.rand.NextVector2Unit() * 0.0001f, ProjectileID.LightsBane, damageDone, hit.Knockback, player.whoAmI, ai8);
                            break;
                        }
                    case ItemID.CobaltSword:
                        {
                            //player._spawnMuramasaCut = true;
                            //player.ApplyNPCOnHitEffects(new Item(ItemID.Muramasa), target.Hitbox, hit._sourceDamage * 21 / 20, hit.Knockback, target.whoAmI, damageDone, damageDone);
                            int num5 = Main.rand.Next(1, 4);
                            num5 = 1;
                            for (int j = 0; j < num5; j++)
                            {
                                NPC nPC = target;
                                Rectangle hitbox = nPC.Hitbox;
                                hitbox.Inflate(30, 16);
                                hitbox.Y -= 8;
                                Vector2 vector3 = Main.rand.NextVector2FromRectangle(hitbox);
                                Vector2 vector4 = hitbox.Center.ToVector2();
                                Vector2 spinningpoint = (vector4 - vector3).SafeNormalize(new Vector2(player.direction, player.gravDir)) * 8f;
                                Main.rand.NextFloat();
                                float num6 = (float)(Main.rand.Next(2) * 2 - 1) * ((float)Math.PI / 5f + (float)Math.PI * 4f / 5f * Main.rand.NextFloat());
                                num6 *= 0.5f;
                                spinningpoint = spinningpoint.RotatedBy(0.7853981852531433);
                                int num7 = 3;
                                int num8 = 10 * num7;
                                int num9 = 5;
                                int num10 = num9 * num7;
                                vector3 = vector4;
                                for (int k = 0; k < num10; k++)
                                {
                                    vector3 -= spinningpoint;
                                    spinningpoint = spinningpoint.RotatedBy((0f - num6) / (float)num8);
                                }

                                vector3 += nPC.velocity * num9;
                                var proj = Projectile.NewProjectileDirect(player.GetProjectileSource_Item(player.HeldItem), vector3, spinningpoint, ProjectileID.Muramasa, hit.Damage  * 21 / 30, 0f, player.whoAmI, num6);
                                proj.ArmorPenetration = 25;
                                proj.penetrate = 3;
                                proj.extraUpdates += 2;
                                proj.scale *= 2;
                                proj.localNPCHitCooldown = 20;
                            }
                            break;
                        }
                    case ItemID.PalladiumSword:
                        {
                            player.AddBuff(BuffID.RapidHealing, 180);
                            if (Main.rand.NextBool(5)) 
                            {
                                player.Heal(3);
                            }
                            break;
                        }
                    case ItemID.MythrilSword:
                        {
                            float num2 = Main.screenPosition.Y + Main.screenHeight;
                            float y2 = target.Center.X + Main.rand.NextFloat(-256, 256);
                            Vector2 vector2 = new Vector2(y2, num2);

                            Vector2 vec = target.Center - vector2;
                            vec += Main.rand.NextVector2Square(-5, 5);
                            vec = vec.SafeNormalize(default) * 36f;
                            Projectile.NewProjectile(player.GetProjectileSource_OnHit(target, 5), vector2, vec, ModContent.ProjectileType<MythrilZenith.MythrilLaser>(), hit.Damage, hit.Knockback, player.whoAmI, 1);
                            break;
                        }
                    case ItemID.OrichalcumSword:
                        {
                            int num = player.direction;
                            float num2 = Main.screenPosition.X;
                            if (num < 0)
                                num2 += (float)Main.screenWidth;

                            float y2 = Main.screenPosition.Y;
                            y2 += (float)Main.rand.Next(Main.screenHeight);
                            Vector2 vector2 = new Vector2(num2, y2);

                            Vector2 vec = target.Center - vector2;
                            vec += Main.rand.NextVector2Square(-5, 5);
                            vec = vec.SafeNormalize(default) * 24f;
                            Projectile.NewProjectile(player.GetProjectileSource_OnHit(target, 5), vector2, vec, ProjectileID.FlowerPetal, hit.Damage, hit.Knockback, player.whoAmI);
                            break;
                        }
                    case ItemID.AdamantiteSword:
                        {
                            if (Main.rand.NextBool(3))
                            {
                                Vector2 unit = Main.rand.NextVector2Unit();
                                Projectile.NewProjectile(projectile.GetSource_FromAI(), target.Center + unit * 64 + Main.rand.NextVector2Unit() * 32, unit * 64, ModContent.ProjectileType<AdamantiteZenith.AdamantiteMissile>(), hit.Damage, hit.Knockback, player.whoAmI, target.whoAmI);
                            }

                            break;
                        }
                    case ItemID.TitaniumSword:
                        {
                            if (Main.rand.NextBool(2))
                            {
                                Vector2 unit = Main.rand.NextVector2Unit();
                                Projectile.NewProjectile(projectile.GetSource_FromAI(), target.Center + unit * 64 + Main.rand.NextVector2Unit() * 32, unit * 64, ModContent.ProjectileType<TitaniumZenith.TitaniumSwordProj>(), hit.Damage * 3 / 2, hit.Knockback, player.whoAmI, target.whoAmI);
                            }
                            break;
                        }

                    default: 
                        {
                            Vector2 unit = Main.rand.NextVector2Unit();
                            Item item = new Item(type);
                            int projType = item.shoot;
                            var proj = Projectile.NewProjectileDirect(player.GetSource_FromThis(), target.Center + unit * 128, -unit * 16, projType, damageDone, hit.Knockback, player.whoAmI);
                            proj.penetrate = -1;
                            proj.timeLeft = 30;
                            break;
                        }
                        //case ItemID.BreakerBlade: 
                        //    {
                        //        //Main.NewText("毁灭吧!!");


                        //        break;
                        //    }
                }

            if (!Main.rand.NextBool(3))
                switch (type)
                {
                    case ItemID.NightsEdge:
                    case ItemID.TrueNightsEdge:
                    case ItemID.Excalibur:
                    case ItemID.TrueExcalibur:
                    case ItemID.TerraBlade:
                    case ItemID.TheHorsemansBlade:
                        {
                            float rate = Main.rand.NextFloat(2f, 8f);
                            var proj = Projectile.NewProjectileDirect(player.GetSource_FromThis(), target.Center, Main.rand.NextVector2Unit() * .0001f, ModContent.ProjectileType<UncenteredSwoosh>(), damageDone, hit.Knockback, player.whoAmI, (float)player.direction * player.gravDir * rate, 30f, 1f);
                            (proj.ModProjectile as UncenteredSwoosh).clonedType = type switch
                            {
                                ItemID.NightsEdge => ProjectileID.NightsEdge,
                                ItemID.TrueNightsEdge => ProjectileID.NightsEdge,
                                ItemID.Excalibur => ProjectileID.Excalibur,
                                ItemID.TrueExcalibur => ProjectileID.TrueExcalibur,
                                ItemID.TerraBlade => ProjectileID.TerraBlade2,
                                ItemID.TheHorsemansBlade => ProjectileID.TheHorsemansBlade
                            };
                            if (type == ItemID.NightsEdge)
                            {
                                proj = Projectile.NewProjectileDirect(player.GetSource_FromThis(), target.Center, Main.rand.NextVector2Unit(), ModContent.ProjectileType<UncenteredSwoosh>(), damageDone, hit.Knockback, player.whoAmI, (float)player.direction * player.gravDir * rate, 30f, 1.5f);
                                (proj.ModProjectile as UncenteredSwoosh).clonedType = ProjectileID.NightsEdge;
                            }
                            if (type == ItemID.TrueNightsEdge)
                            {


                            }
                            if (type == ItemID.TrueExcalibur)
                            {
                                proj = Projectile.NewProjectileDirect(player.GetSource_FromThis(), target.Center, Main.rand.NextVector2Unit() * 0.0001f, ModContent.ProjectileType<UncenteredSwoosh>(), damageDone, hit.Knockback, player.whoAmI, (float)player.direction * player.gravDir * rate, 30f, 1f);
                                (proj.ModProjectile as UncenteredSwoosh).clonedType = ProjectileID.Excalibur;
                            }
                            break;
                        }
                }

            base.OnHitNPC(projectile, target, hit, damageDone);
        }
    }
}
