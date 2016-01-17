using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK;
using SharpDX;
using System.Drawing;
using EloBuddy;

namespace OneKeyToWin_AIO_Sebby
{

    class Activator
    {
        private Menu Config = Program.Config;
        public static Orbwalking.Orbwalker Orbwalker = Program.Orbwalker;
        private Obj_AI_Hero Player { get { return ObjectManager.Player; } }
        public int Muramana = 3042;
        public int Manamune = 3004;
        //private int [] SmiteDamage = { 390, 410, 430, 450, 480, 510, 540, 570, 600, 640, 680, 720, 760, 800, 850, 900, 950, 1000};
        private SpellSlot heal, barrier, ignite, exhaust, flash, smite , teleport;

        public static Items.Item

            //Cleans
            Mikaels = new Items.Item(3222, 600f),
            Quicksilver = new Items.Item(3140, 0),
            Mercurial = new Items.Item(3139, 0),
            Dervish = new Items.Item(3137, 0),
            //REGEN
            Potion = new Items.Item(2003, 0),
            ManaPotion = new Items.Item(2004, 0),
            Flask = new Items.Item(2041, 0),
            Biscuit = new Items.Item(2010, 0),
            Refillable = new Items.Item(2031, 0),
            Hunter = new Items.Item(2032, 0),
            Corrupting = new Items.Item(2033, 0),
            //attack
            Botrk = new Items.Item(3153, 550f),
            Cutlass = new Items.Item(3144, 550f),
            Youmuus = new Items.Item(3142, 650f),
            Hydra = new Items.Item(3074, 440f),
            Hydra2 = new Items.Item(3077, 440f),
            HydraTitanic = new Items.Item(3748, 150f),
            Hextech = new Items.Item(3146, 700f),
            FrostQueen = new Items.Item(3092, 850f),
            
            //def
            FaceOfTheMountain = new Items.Item(3401, 600f),
            Zhonya = new Items.Item(3157, 0),
            Seraph = new Items.Item(3040, 0),
            Solari = new Items.Item(3190, 600f),
            Randuin = new Items.Item(3143, 400f);
        
        public void LoadOKTW()
        {

            teleport = Player.GetSpellSlot("SummonerTeleport");
            heal = Player.GetSpellSlot("summonerheal");
            barrier = Player.GetSpellSlot("summonerbarrier");
            ignite = Player.GetSpellSlot("summonerdot");
            exhaust = Player.GetSpellSlot("summonerexhaust");
            flash = Player.GetSpellSlot("summonerflash");

            smite = Player.GetSpellSlot("summonersmite");
            if (smite == SpellSlot.Unknown) { smite = Player.GetSpellSlot("itemsmiteaoe"); }
            if (smite == SpellSlot.Unknown) { smite = Player.GetSpellSlot("s5_summonersmiteplayerganker"); }
            if (smite == SpellSlot.Unknown) { smite = Player.GetSpellSlot("s5_summonersmitequick"); }
            if (smite == SpellSlot.Unknown) { smite = Player.GetSpellSlot("s5_summonersmiteduel"); }

            if (smite != SpellSlot.Unknown)
            {
                Config.SubMenu("Activator OKTW©").SubMenu("Summoners").SubMenu("Smite").AddItem(new MenuItem("SmiteEnemy", "Auto Smite enemy under 50% hp").SetValue(true));
                Config.SubMenu("Activator OKTW©").SubMenu("Summoners").SubMenu("Smite").AddItem(new MenuItem("SmiteEnemyKS", "Auto Smite enemy KS").SetValue(true));
                Config.SubMenu("Activator OKTW©").SubMenu("Summoners").SubMenu("Smite").AddItem(new MenuItem("Smite", "Auto Smite mobs OKTW").SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Toggle)));
                Config.SubMenu("Activator OKTW©").SubMenu("Summoners").SubMenu("Smite").AddItem(new MenuItem("Rdragon", "Dragon", true).SetValue(true));
                Config.SubMenu("Activator OKTW©").SubMenu("Summoners").SubMenu("Smite").AddItem(new MenuItem("Rbaron", "Baron", true).SetValue(true));
                Config.SubMenu("Activator OKTW©").SubMenu("Summoners").SubMenu("Smite").AddItem(new MenuItem("Rherald", "Herald", true).SetValue(true));
                Config.SubMenu("Activator OKTW©").SubMenu("Summoners").SubMenu("Smite").AddItem(new MenuItem("Rred", "Red", true).SetValue(true));
                Config.SubMenu("Activator OKTW©").SubMenu("Summoners").SubMenu("Smite").AddItem(new MenuItem("Rblue", "Blue", true).SetValue(true));
                Config.Item("Smite").Permashow(true);
            }

            if (flash != SpellSlot.Unknown)
            {
                //Config.SubMenu("Activator OKTW©").SubMenu("Summoners").SubMenu("Flash").AddItem(new MenuItem("Flash", "Flash max range").SetValue(true));

            }
            if (exhaust != SpellSlot.Unknown)
            {
                Config.SubMenu("Activator OKTW©").SubMenu("Summoners").SubMenu("Exhaust").AddItem(new MenuItem("Exhaust", "Exhaust").SetValue(true));
                Config.SubMenu("Activator OKTW©").SubMenu("Summoners").SubMenu("Exhaust").AddItem(new MenuItem("Exhaust1", "Exhaust if Channeling Important Spell ").SetValue(true));
                Config.SubMenu("Activator OKTW©").SubMenu("Summoners").SubMenu("Exhaust").AddItem(new MenuItem("Exhaust2", "Always in combo").SetValue(false));
            }
            if (heal != SpellSlot.Unknown)
            {
                Config.SubMenu("Activator OKTW©").SubMenu("Summoners").SubMenu("Heal").AddItem(new MenuItem("Heal", "Heal").SetValue(true));
                Config.SubMenu("Activator OKTW©").SubMenu("Summoners").SubMenu("Heal").AddItem(new MenuItem("AllyHeal", "AllyHeal").SetValue(true));
            }
            if (barrier != SpellSlot.Unknown)
            {
                Config.SubMenu("Activator OKTW©").SubMenu("Summoners").AddItem(new MenuItem("Barrier", "Barrier").SetValue(true));

            }
            if (ignite != SpellSlot.Unknown)
            {
                Config.SubMenu("Activator OKTW©").SubMenu("Summoners").AddItem(new MenuItem("Ignite", "Ignite").SetValue(true));
            }

            Config.SubMenu("Activator OKTW©").AddItem(new MenuItem("pots", "Potion, ManaPotion, Flask, Biscuit").SetValue(true));

            Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("Botrk").AddItem(new MenuItem("Botrk", "Botrk").SetValue(true));
            Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("Botrk").AddItem(new MenuItem("BotrkKS", "Botrk KS").SetValue(true));
            Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("Botrk").AddItem(new MenuItem("BotrkLS", "Botrk LifeSaver").SetValue(true));
            Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("Botrk").AddItem(new MenuItem("BotrkCombo", "Botrk always in combo").SetValue(false));

            Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("Cutlass").AddItem(new MenuItem("Cutlass", "Cutlass").SetValue(true));
            Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("Cutlass").AddItem(new MenuItem("CutlassKS", "Cutlass KS").SetValue(true));
            Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("Cutlass").AddItem(new MenuItem("CutlassCombo", "Cutlass always in combo").SetValue(true));

            Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("Cutlass").AddItem(new MenuItem("Hextech", "Hextech").SetValue(true));
            Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("Cutlass").AddItem(new MenuItem("HextechKS", "Hextech KS").SetValue(true));
            Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("Cutlass").AddItem(new MenuItem("HextechCombo", "Hextech always in combo").SetValue(true));

            Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("Youmuus").AddItem(new MenuItem("Youmuus", "Youmuus").SetValue(true));
            Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("Youmuus").AddItem(new MenuItem("YoumuusR", "LucianR, TwitchR, AsheQ").SetValue(true));
            Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("Youmuus").AddItem(new MenuItem("YoumuusKS", "Youmuus KS").SetValue(true));
            Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("Youmuus").AddItem(new MenuItem("YoumuusCombo", "Youmuus always in combo").SetValue(false));

            Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("Hydra").AddItem(new MenuItem("Hydra", "Hydra").SetValue(true));
            Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("HydraTitanic").AddItem(new MenuItem("HydraTitanic", "Hydra Titanic").SetValue(true));

            Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("Muramana").AddItem(new MenuItem("Muramana", "Muramana").SetValue(true));

            Config.SubMenu("Activator OKTW©").SubMenu("Offensives").SubMenu("FrostQueen").AddItem(new MenuItem("FrostQueen", "FrostQueen").SetValue(true));

            // DEF
            Config.SubMenu("Activator OKTW©").SubMenu("Defensives").AddItem(new MenuItem("Randuin", "Randuin").SetValue(true));
            Config.SubMenu("Activator OKTW©").SubMenu("Defensives").AddItem(new MenuItem("FaceOfTheMountain", "FaceOfTheMountain").SetValue(true));
            Config.SubMenu("Activator OKTW©").SubMenu("Defensives").SubMenu("Zhonya").AddItem(new MenuItem("Zhonya", "Zhonya").SetValue(true));

            foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.Team != Player.Team))
            {
                var spell = enemy.Spellbook.Spells[3];
                    Config.SubMenu("Activator OKTW©").SubMenu("Defensives").SubMenu("Zhonya").AddItem(new MenuItem("spellZ" + spell.SData.Name, enemy.ChampionName + ": "+spell.Name).SetValue(spell.SData.TargettingType == SpellDataTargetType.Unit));
            }

            Config.SubMenu("Activator OKTW©").SubMenu("Defensives").AddItem(new MenuItem("Seraph", "Seraph").SetValue(true));
            Config.SubMenu("Activator OKTW©").SubMenu("Defensives").AddItem(new MenuItem("Solari", "Solari").SetValue(true));
            // CLEANSERS 
            Config.SubMenu("Activator OKTW©").SubMenu("Cleansers").AddItem(new MenuItem("Clean", "Quicksilver, Mikaels, Mercurial, Dervish").SetValue(true));

            foreach (var ally in ObjectManager.Get<Obj_AI_Hero>().Where(ally => ally.IsAlly))
                Config.SubMenu("Activator OKTW©").SubMenu("Cleansers").SubMenu("Mikaels allys").AddItem(new MenuItem("MikaelsAlly" + ally.ChampionName, ally.ChampionName).SetValue(true));

            Config.SubMenu("Activator OKTW©").SubMenu("Cleansers").AddItem(new MenuItem("cleanHP", "Use only under % HP").SetValue(new Slider(80, 100, 0)));
            Config.SubMenu("Activator OKTW©").SubMenu("Cleansers").SubMenu("Buff type").AddItem(new MenuItem("CleanSpells", "ZedR FizzR MordekaiserR PoppyR VladimirR").SetValue(true));
            Config.SubMenu("Activator OKTW©").SubMenu("Cleansers").SubMenu("Buff type").AddItem(new MenuItem("Stun", "Stun").SetValue(true));
            Config.SubMenu("Activator OKTW©").SubMenu("Cleansers").SubMenu("Buff type").AddItem(new MenuItem("Snare", "Snare").SetValue(true));
            Config.SubMenu("Activator OKTW©").SubMenu("Cleansers").SubMenu("Buff type").AddItem(new MenuItem("Charm", "Charm").SetValue(true));
            Config.SubMenu("Activator OKTW©").SubMenu("Cleansers").SubMenu("Buff type").AddItem(new MenuItem("Fear", "Fear").SetValue(true));
            Config.SubMenu("Activator OKTW©").SubMenu("Cleansers").SubMenu("Buff type").AddItem(new MenuItem("Suppression", "Suppression").SetValue(true));
            Config.SubMenu("Activator OKTW©").SubMenu("Cleansers").SubMenu("Buff type").AddItem(new MenuItem("Taunt", "Taunt").SetValue(true));
            Config.SubMenu("Activator OKTW©").SubMenu("Cleansers").SubMenu("Buff type").AddItem(new MenuItem("Blind", "Blind").SetValue(true));
            Game.OnUpdate += Game_OnGameUpdate;
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            //Drawing.OnDraw += Drawing_OnDraw;
        }

        private void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (Config.Item("HydraTitanic").GetValue<bool>() && Program.Combo && HydraTitanic.IsReady() && target.IsValid<Obj_AI_Hero>())
            {
                HydraTitanic.Cast();
                Orbwalking.ResetAutoAttackTimer();
            }
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsEnemy || sender.Type != GameObjectType.obj_AI_Hero)
                return;

            
            if (sender.Distance(Player.Position) > 1600)
                return;

            if (Zhonya.IsReady()  && Config.Item("Zhonya").GetValue<bool>())
            {
                if (Config.Item("spellZ" + args.SData.Name) != null && Config.Item("spellZ" + args.SData.Name).GetValue<bool>())
                {
                    if (args.Target != null && args.Target.NetworkId == Player.NetworkId)
                    {
                        Zhonya.Cast();
                    }
                    else
                    {
                        var castArea = Player.Distance(args.End) * (args.End - Player.ServerPosition).Normalized() + Player.ServerPosition;
                        if (castArea.Distance(Player.ServerPosition) < Player.BoundingRadius / 2)
                            Zhonya.Cast();
                    }
                }
            }

            if (CanUse(exhaust) && Config.Item("Exhaust").GetValue<bool>())
            {
                foreach (var ally in Program.Allies.Where(ally => ally.IsValid && !ally.IsDead && ally.HealthPercent < 51 && Player.Distance(ally.ServerPosition) < 700))
                {
                    double dmg = 0;
                    if (args.Target != null && args.Target.NetworkId == ally.NetworkId)
                    {
                        dmg = dmg + sender.GetSpellDamage(ally, args.SData.Name);
                    }
                    else
                    {
                        var castArea = ally.Distance(args.End) * (args.End - ally.ServerPosition).Normalized() + ally.ServerPosition;
                        if (castArea.Distance(ally.ServerPosition) < ally.BoundingRadius / 2)
                            dmg = dmg + sender.GetSpellDamage(ally, args.SData.Name);
                        else
                            continue;
                    }

                    if (ally.Health - dmg < ally.CountEnemiesInRange(700) * ally.Level * 40)
                        Player.Spellbook.CastSpell(exhaust, sender);
                }
            }
        }

        private void Survival()
        {
            if (Player.HealthPercent < 60 && (Seraph.IsReady() || Zhonya.IsReady()  || CanUse(barrier)))
            {
                double dmg = OktwCommon.GetIncomingDamage(Player, 1);
                var enemys = Player.CountEnemiesInRange(700);
                if(dmg > 0 || enemys > 0)
                { 
                    if (CanUse(barrier) && Config.Item("Barrier").GetValue<bool>())
                        {
                            var value = 95 + Player.Level * 20;
                            if (dmg > value && Player.HealthPercent < 50)
                                Player.Spellbook.CastSpell(barrier, Player);
                            else if (Player.Health - dmg < enemys * Player.Level * 20)
                                Player.Spellbook.CastSpell(barrier, Player);
                            else if (Player.Health - dmg < Player.Level * 10)
                                Seraph.Cast();
                        }

                        if (Seraph.IsReady() && Config.Item("Seraph").GetValue<bool>())
                        {
                            var value = Player.Mana * 0.2 + 150;
                            if (dmg > value && Player.HealthPercent < 50)
                                Seraph.Cast();
                            else if (Player.Health - dmg < enemys * Player.Level * 20)
                                Seraph.Cast();
                            else if (Player.Health - dmg < Player.Level * 10)
                                Seraph.Cast();
                        }

                    if (Zhonya.IsReady() && Config.Item("Zhonya").GetValue<bool>())
                    {
                        if (dmg > Player.Level * 35)
                        {
                            Zhonya.Cast();
                        }
                        else if (Player.Health - dmg < enemys * Player.Level * 20)
                        {
                            Zhonya.Cast();

                        }
                        else if (Player.Health - dmg < Player.Level * 10)
                        {
                            Zhonya.Cast();

                        }
                    }
                }
            }


            if (!Solari.IsReady() && !FaceOfTheMountain.IsReady() && !CanUse(heal) )
                return;

            foreach (var ally in Program.Allies.Where(ally => ally.IsValid && !ally.IsDead && ally.HealthPercent < 50 && Player.Distance(ally.ServerPosition) < 700))
            {
                double dmg = OktwCommon.GetIncomingDamage(ally, 1);
                var enemys = ally.CountEnemiesInRange(700);
                if (dmg == 0 && enemys == 0)
                    continue;

                if (CanUse(heal) && Config.Item("Heal").GetValue<bool>())
                {
                    if (!Config.Item("AllyHeal").GetValue<bool>() && !ally.IsMe)
                        return;

                    if (ally.Health - dmg < enemys * ally.Level * 15)
                        Player.Spellbook.CastSpell(heal, ally);
                    else if (ally.Health - dmg < ally.Level * 10)
                       Player.Spellbook.CastSpell(heal, ally);
                }

                if (Config.Item("Solari").GetValue<bool>() && Solari.IsReady() && Player.Distance(ally.ServerPosition) < Solari.Range)
                {
                    var value = 75 + (15 * Player.Level);
                    if (dmg > value && Player.HealthPercent < 50)
                        Solari.Cast();
                    else if (ally.Health - dmg < enemys * ally.Level * 15)
                        Solari.Cast();
                    else if (ally.Health - dmg < ally.Level * 10)
                        Solari.Cast();
                }

                if (Config.Item("FaceOfTheMountain").GetValue<bool>() && FaceOfTheMountain.IsReady() && Player.Distance(ally.ServerPosition) < FaceOfTheMountain.Range)
                {
                    var value = 0.1 * Player.MaxHealth;
                    if (dmg > value && Player.HealthPercent < 50)
                        FaceOfTheMountain.Cast(ally);
                    else if (ally.Health - dmg < enemys * ally.Level * 15)
                        FaceOfTheMountain.Cast(ally);
                    else if (ally.Health - dmg < ally.Level * 10)
                        FaceOfTheMountain.Cast(ally);
                }
            }
        }


        private void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (!Youmuus.IsReady() || !Config.Item("YoumuusR").GetValue<bool>())
                return;
            if (args.Slot == SpellSlot.R && (Player.ChampionName == "Twitch"))
            {
                Youmuus.Cast();
            }
            if (args.Slot == SpellSlot.Q && (Player.ChampionName == "Ashe" ))
            {
                Youmuus.Cast();
            }
        }

        private void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (Config.Item("Muramana").GetValue<bool>())
            {
                int Mur = Items.HasItem(Muramana) ? 3042 : 3043;
                if (Items.HasItem(Mur) && args.Target.IsEnemy && args.Target.IsValid<Obj_AI_Hero>() && Items.CanUseItem(Mur) && Player.Mana > Player.MaxMana * 0.3)
                {
                    if (!ObjectManager.Player.HasBuff("Muramana"))
                        Items.UseItem(Mur);
                }
                else if (ObjectManager.Player.HasBuff("Muramana") && Items.HasItem(Mur) && Items.CanUseItem(Mur))
                    Items.UseItem(Mur);
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.InFountain() || Player.IsRecalling() || Player.IsDead)
            {
                return;
            }

            Cleansers();
            Smite();
            Survival();

            if (!Program.LagFree(0))
                return;

            if (Config.Item("pots").GetValue<bool>())
                PotionManagement();
            
            Ignite();
            //Teleport();
            Exhaust();
            Offensive();
            Defensive();
            ZhonyaCast();
        }

        private void Teleport()
        {
            if (CanUse(teleport) && !Player.HasBuff("teleport"))
            {
                foreach (var ally in Program.Allies.Where(ally => ally.IsValid && !ally.IsDead  && ally.CountEnemiesInRange(1000) > 0 ))
                {
                    foreach (var enemy in Program.Enemies.Where(enemy => enemy.IsValid && !enemy.IsDead))
                    {
                        var distanceEA = enemy.Distance(ally);
                        if (distanceEA < 1000)
                        {
                            foreach (var obj in ObjectManager.Get<Obj_AI_Minion>().Where(obj => obj.IsAlly &&  distanceEA < obj.Position.Distance(ally.Position)))
                            {
                                Player.Spellbook.CastSpell(teleport, obj);
                            }
                        }
                    }
                }
            }
        }

        private void Smite()
        {
            if (CanUse(smite) )
            {
                var mobs = MinionManager.GetMinions(Player.ServerPosition, 520, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.Health);
                if (mobs.Count == 0 && (Player.GetSpellSlot("s5_summonersmiteplayerganker") != SpellSlot.Unknown || Player.GetSpellSlot("s5_summonersmiteduel") != SpellSlot.Unknown))
                {
                    var enemy = TargetSelector.GetTarget(500, TargetSelector.DamageType.True);
                    if (enemy.IsValidTarget() && enemy.HealthPercent < 50)
                    {
                        if(enemy.HealthPercent < 50 && Config.Item("SmiteEnemy").GetValue<bool>())
                            Player.Spellbook.CastSpell(smite, enemy);
                        
                        var smiteDmg = Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Smite);
                        
                        if ( Config.Item("SmiteEnemyKS").GetValue<bool>() && enemy.Health - OktwCommon.GetIncomingDamage(enemy) < smiteDmg)
                            Player.Spellbook.CastSpell(smite, enemy);
                    }
                }
                if (mobs.Count > 0 && Config.Item("Smite").GetValue<KeyBind>().Active)
                {
                    foreach (var mob in mobs)
                    {
                        if (((mob.SkinName == "SRU_Dragon" && Config.Item("Rdragon", true).GetValue<bool>())
                            || (mob.SkinName == "SRU_Baron" && Config.Item("Rbaron", true).GetValue<bool>())
                            || (mob.SkinName == "SRU_RiftHerald" && Config.Item("Rherald", true).GetValue<bool>())
                            || (mob.SkinName == "SRU_Red" && Config.Item("Rred", true).GetValue<bool>())
                            || (mob.SkinName == "SRU_Blue" && Config.Item("Rblue", true).GetValue<bool>()))
                            && mob.Health <= Player.GetSummonerSpellDamage(mob, Damage.SummonerSpell.Smite))
                        {
                            Player.Spellbook.CastSpell(smite, mob);
                        }
                    }
                }
            }
        }

        private void Exhaust()
        {
            if (CanUse(exhaust) && Config.Item("Exhaust").GetValue<bool>())
            {
                if (Config.Item("Exhaust1").GetValue<bool>())
                {
                    foreach (var enemy in Program.Enemies.Where(enemy => enemy.IsValidTarget(650) && enemy.IsChannelingImportantSpell()))
                    {
                        Player.Spellbook.CastSpell(exhaust, enemy);
                    }
                }

                if (Config.Item("Exhaust2").GetValue<bool>() && Program.Combo)
                {
                    var t = TargetSelector.GetTarget(650, TargetSelector.DamageType.Physical);
                    if (t.IsValidTarget())
                    {
                        Player.Spellbook.CastSpell(exhaust, t);
                    }
                }
            }
        }

        private void Ignite()
        {
            if (CanUse(ignite) && Config.Item("Ignite").GetValue<bool>())
            {
                var enemy = TargetSelector.GetTarget(600, TargetSelector.DamageType.True);
                if (enemy.IsValidTarget())
                {
                    var pred = enemy.Health - OktwCommon.GetIncomingDamage(enemy);
                    if (pred < 0)
                        return;

                    var IgnDmg = Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite) ;

                    if (pred <= IgnDmg && Player.ServerPosition.Distance(enemy.ServerPosition) > 500 && enemy.CountAlliesInRange(500) < 2)
                        Player.Spellbook.CastSpell(ignite, enemy);

                    if (pred <= 2 * IgnDmg)
                    {
                        if (enemy.PercentLifeStealMod > 10)
                            Player.Spellbook.CastSpell(ignite, enemy);

                        if (enemy.HasBuff("RegenerationPotion") || enemy.HasBuff("ItemMiniRegenPotion") || enemy.HasBuff("ItemCrystalFlask"))
                            Player.Spellbook.CastSpell(ignite, enemy);

                        if (enemy.Health > Player.Health)
                            Player.Spellbook.CastSpell(ignite, enemy);
                    }
                }
            }
        }

        private void ZhonyaCast()
        {
            if (Config.Item("Zhonya").GetValue<bool>() && Zhonya.IsReady())
            {
                float time = 10;
                if (Player.HasBuff("zedrdeathmark"))
                {
                    time = OktwCommon.GetPassiveTime(Player, "zedulttargetmark");
                }
                if (Player.HasBuff("FizzMarinerDoom"))
                {
                    time = OktwCommon.GetPassiveTime(Player, "FizzMarinerDoom");
                }
                if (Player.HasBuff("MordekaiserChildrenOfTheGrave"))
                {
                    time = OktwCommon.GetPassiveTime(Player, "MordekaiserChildrenOfTheGrave");
                }
                if (Player.HasBuff("VladimirHemoplague"))
                {
                    time = OktwCommon.GetPassiveTime(Player, "VladimirHemoplague");
                }
                if (time < 1 && time > 0)
                    Zhonya.Cast();
            }
        }

        private void Cleansers()
        {
            if (!Quicksilver.IsReady() && !Mikaels.IsReady() && !Mercurial.IsReady() && !Dervish.IsReady())
                return;

            if (Player.HealthPercent >= (float)Config.Item("cleanHP").GetValue<Slider>().Value || !Config.Item("Clean").GetValue<bool>())
                return;

            if (Player.HasBuff("zedrdeathmark") || Player.HasBuff("FizzMarinerDoom") || Player.HasBuff("MordekaiserChildrenOfTheGrave") || Player.HasBuff("PoppyDiplomaticImmunity") || Player.HasBuff("VladimirHemoplague"))
                Clean();

            if (Mikaels.IsReady())
            {
                foreach (var ally in Program.Allies.Where(
                    ally => ally.IsValid && !ally.IsDead && Config.Item("MikaelsAlly" + ally.ChampionName).GetValue<bool>() && Player.Distance(ally.Position) < Mikaels.Range 
                    && ally.HealthPercent < (float)Config.Item("cleanHP").GetValue<Slider>().Value))
                {
                    if (ally.HasBuff("zedrdeathmark") || ally.HasBuff("FizzMarinerDoom") || ally.HasBuff("MordekaiserChildrenOfTheGrave") || ally.HasBuff("PoppyDiplomaticImmunity") || ally.HasBuff("VladimirHemoplague"))
                        Mikaels.Cast(ally);
                    if (ally.HasBuffOfType(BuffType.Stun) && Config.Item("Stun").GetValue<bool>())
                        Mikaels.Cast(ally);
                    if (ally.HasBuffOfType(BuffType.Snare) && Config.Item("Snare").GetValue<bool>())
                        Mikaels.Cast(ally);
                    if (ally.HasBuffOfType(BuffType.Charm) && Config.Item("Charm").GetValue<bool>())
                        Mikaels.Cast(ally);
                    if (ally.HasBuffOfType(BuffType.Fear) && Config.Item("Fear").GetValue<bool>())
                        Mikaels.Cast(ally);
                    if (ally.HasBuffOfType(BuffType.Stun) && Config.Item("Stun").GetValue<bool>())
                        Mikaels.Cast(ally);
                    if (ally.HasBuffOfType(BuffType.Taunt) && Config.Item("Taunt").GetValue<bool>())
                        Mikaels.Cast(ally);
                    if (ally.HasBuffOfType(BuffType.Suppression) && Config.Item("Suppression").GetValue<bool>())
                        Mikaels.Cast(ally);
                    if (ally.HasBuffOfType(BuffType.Blind) && Config.Item("Blind").GetValue<bool>())
                        Mikaels.Cast(ally);
                }
            }

            if (Player.HasBuffOfType(BuffType.Stun) && Config.Item("Stun").GetValue<bool>())
                Clean();
            if (Player.HasBuffOfType(BuffType.Snare) && Config.Item("Snare").GetValue<bool>())
                Clean();
            if (Player.HasBuffOfType(BuffType.Charm) && Config.Item("Charm").GetValue<bool>())
                Clean();
            if (Player.HasBuffOfType(BuffType.Fear) && Config.Item("Fear").GetValue<bool>())
                Clean();
            if (Player.HasBuffOfType(BuffType.Stun) && Config.Item("Stun").GetValue<bool>())
                Clean();
            if (Player.HasBuffOfType(BuffType.Taunt) && Config.Item("Taunt").GetValue<bool>())
                Clean();
            if (Player.HasBuffOfType(BuffType.Suppression) && Config.Item("Suppression").GetValue<bool>())
                Clean();
            if (Player.HasBuffOfType(BuffType.Blind) && Config.Item("Blind").GetValue<bool>())
                Clean();
        }

        private void Clean()
        {
            if (Quicksilver.IsReady())
                Quicksilver.Cast();
            else if (Mercurial.IsReady())
                Mercurial.Cast();
            else if (Dervish.IsReady())
                Dervish.Cast(); 
        }

        private void Defensive()
        {
            if (Randuin.IsReady() && Config.Item("Randuin").GetValue<bool>() && Player.CountEnemiesInRange(Randuin.Range) > 0)
            {
                Randuin.Cast();
            } 
        }

        private void Offensive()
        {
            if (Botrk.IsReady() && Config.Item("Botrk").GetValue<bool>())
            {
                var t = TargetSelector.GetTarget(Botrk.Range, TargetSelector.DamageType.Physical);
                if (t.IsValidTarget())
                {
                    if (Config.Item("BotrkKS").GetValue<bool>() && Player.CalcDamage(t, Damage.DamageType.Physical, t.MaxHealth * 0.1) > t.Health - OktwCommon.GetIncomingDamage(t))
                        Botrk.Cast(t);
                    if (Config.Item("BotrkLS").GetValue<bool>() && Player.Health < Player.MaxHealth * 0.5)
                        Botrk.Cast(t);
                    if (Config.Item("BotrkCombo").GetValue<bool>() && Program.Combo)
                        Botrk.Cast(t);
                }
            }

            if (Hextech.IsReady() && Config.Item("Hextech").GetValue<bool>())
            {
                var t = TargetSelector.GetTarget(Hextech.Range, TargetSelector.DamageType.Magical);
                if (t.IsValidTarget())
                {
                    if (Config.Item("HextechKS").GetValue<bool>() && Player.CalcDamage(t, Damage.DamageType.Magical, 150 + Player.FlatMagicDamageMod * 0.4) > t.Health - OktwCommon.GetIncomingDamage(t))
                        Hextech.Cast(t);
                    if (Config.Item("HextechCombo").GetValue<bool>() && Program.Combo)
                        Hextech.Cast(t);
                }
            }

            if (Program.Combo && FrostQueen.IsReady() && Config.Item("FrostQueen").GetValue<bool>())
            {
                var t = TargetSelector.GetTarget(FrostQueen.Range, TargetSelector.DamageType.Magical);
                if (t.IsValidTarget())
                {
                    var predInput2 = new Core.PredictionInput
                    {
                        Aoe = true,
                        Collision = false,
                        Speed = 1200,
                        Delay = 0.25f,
                        Range = FrostQueen.Range,
                        From = Player.ServerPosition,
                        Radius = 200,
                        Unit = t,
                        Type = Core.SkillshotType.SkillshotCircle
                    };
                    var poutput2 = Core.Prediction.GetPrediction(predInput2);

                    if (poutput2.Hitchance >= Core.HitChance.High)
                        FrostQueen.Cast(poutput2.CastPosition);
                }
            }

            if (Cutlass.IsReady() && Config.Item("Cutlass").GetValue<bool>())
            {
                var t = TargetSelector.GetTarget(Cutlass.Range, TargetSelector.DamageType.Magical);
                if (t.IsValidTarget())
                {
                    if (Config.Item("CutlassKS").GetValue<bool>() && Player.CalcDamage(t, Damage.DamageType.Magical, 100) > t.Health - OktwCommon.GetIncomingDamage(t))
                        Cutlass.Cast(t);
                    if (Config.Item("CutlassCombo").GetValue<bool>() && Program.Combo)
                        Cutlass.Cast(t);
                }
            }

            if (Youmuus.IsReady() && Config.Item("Youmuus").GetValue<bool>())
            {
                var t = Orbwalker.GetTarget();

                if (t.IsValidTarget() && t is Obj_AI_Hero)
                {
                    if (Config.Item("YoumuusKS").GetValue<bool>() && t.Health < Player.MaxHealth * 0.6)
                        Youmuus.Cast();
                    if (Config.Item("YoumuusCombo").GetValue<bool>() && Program.Combo)
                        Youmuus.Cast();
                }
            }

            if (Config.Item("Hydra").GetValue<bool>())
            {
                if (Hydra.IsReady() && Player.CountEnemiesInRange(Hydra.Range) > 0)
                    Hydra.Cast();
                else if (Hydra2.IsReady() && Player.CountEnemiesInRange(Hydra2.Range) > 0)
                    Hydra2.Cast();
            }
        }

        private void PotionManagement()
        {
            if (Player.HasBuff("RegenerationPotion") || Player.HasBuff("ItemMiniRegenPotion") || Player.HasBuff("ItemCrystalFlaskJungle") || Player.HasBuff("ItemDarkCrystalFlask"))
                return;

            if (Potion.IsReady())
            {
                if (Player.Health + 200 < Player.MaxHealth && Player.CountEnemiesInRange(700) > 0)
                    Potion.Cast();
                else if (Player.Health < Player.MaxHealth * 0.6)
                    Potion.Cast();
                return;
            }
            else if (Biscuit.IsReady())
            {
                if (Player.Health + 250 < Player.MaxHealth && Player.CountEnemiesInRange(700) > 0)
                    Biscuit.Cast();
                else if (Player.Health < Player.MaxHealth * 0.6)
                    Biscuit.Cast();
                return;
            }
            else if (Hunter.IsReady())
            {
                if (Player.Health + 250 < Player.MaxHealth && Player.CountEnemiesInRange(700) > 0)
                    Hunter.Cast();
                else if (Player.Health < Player.MaxHealth * 0.6)
                    Hunter.Cast();
                return;
            }
            else if (Corrupting.IsReady())
            {
                if (Player.Health + 250 < Player.MaxHealth && Player.CountEnemiesInRange(700) > 0)
                    Corrupting.Cast();
                else if (Player.Health < Player.MaxHealth * 0.6)
                    Corrupting.Cast();

                return;
            }
            else if (Refillable.IsReady())
            {
                if (Player.Health + 250 < Player.MaxHealth && Player.CountEnemiesInRange(700) > 0)
                    Refillable.Cast();
                else if (Player.Health < Player.MaxHealth * 0.6)
                    Refillable.Cast();
                return;
            }
        }

        private bool CanUse(SpellSlot sum)
        {
            if (sum != SpellSlot.Unknown && Player.Spellbook.CanUseSpell(sum) == SpellState.Ready)
                return true;
            else
                return false;
        }
    }
}
