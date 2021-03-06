﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace OneKeyToWin_AIO_Sebby
{
    class Annie
    {
        private Menu Config = Program.Config;
        public static Orbwalking.Orbwalker Orbwalker = Program.Orbwalker;
        public Spell Q, W, E, R;
        public float QMANA = 0, WMANA = 0, EMANA = 0, RMANA = 0;

        public GameObject Tibbers;
        public float TibbersTimer = 0;
        private bool HaveStun = false;
        private Obj_AI_Hero Player
        {
            get { return ObjectManager.Player; }
        }
        public void LoadOKTW()
        {
            Q = new Spell(SpellSlot.Q, 625f);
            W = new Spell(SpellSlot.W, 600f);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 625f);

            Q.SetTargetted(0.25f, 1400f);
            W.SetSkillshot(0.30f, 200f, float.MaxValue, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.20f, 250f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("wRange", "W range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("rRange", "R range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw only ready spells", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("autoQ", "Auto Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("harrasQ", "Harass Q", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("autoW", "Auto W", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("harrasW", "Harass W", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("autoE", "Auto E stack stun", true).SetValue(true));

            foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.IsEnemy))
                Config.SubMenu(Player.ChampionName).SubMenu("R Config").SubMenu("Ultimate Manager").AddItem(new MenuItem("UM" + enemy.ChampionName, enemy.ChampionName, true).SetValue(new StringList(new[] { "Normal", "Always", "Never" }, 0)));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("autoRks", "Auto R KS", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("autoRcombo", "Auto R Combo if stun is ready", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("rCount", "Auto R stun x enemies", true).SetValue(new Slider(3, 2, 5)));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("tibers", "TibbersAutoPilot", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmQ", "Farm Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmW", "Lane clear W", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("Mana", "LaneClear Mana", true).SetValue(new Slider(60, 100, 0)));

            Game.OnUpdate += Game_OnGameUpdate;
            GameObject.OnCreate += Obj_AI_Base_OnCreate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private void Obj_AI_Base_OnCreate(GameObject obj, EventArgs args)
        {
            if (obj.IsValid && obj.IsAlly && obj.Type == GameObjectType.obj_AI_Minion && obj.Name == "Tibbers")
            {
                Tibbers = obj;
                Program.debug("" + obj.Type);
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.HasBuff("Recall"))
                return;

            HaveStun = Player.HasBuff("pyromania_particle");

            SetMana();

            if (R.IsReady() && Program.LagFree(1) && !HaveTibers)
            {
                foreach (var enemy in Program.Enemies.Where(enemy => enemy.IsValidTarget(R.Range) && enemy.Health - OktwCommon.GetIncomingDamage(enemy) > 0 && OktwCommon.ValidUlt(enemy)))
                {
                    int Rmode = Config.Item("UM" + enemy.ChampionName, true).GetValue<StringList>().SelectedIndex;

                    if (Rmode == 2)
                        continue;
                   
                    var poutput = R.GetPrediction(enemy, true);
                    var aoeCount = poutput.AoeTargetsHitCount;

                    if (Rmode == 1)
                        R.Cast(poutput.CastPosition);

                    if (HaveStun && aoeCount >= Config.Item("rCount", true).GetValue<Slider>().Value && Config.Item("rCount", true).GetValue<Slider>().Value > 0)
                        R.Cast(poutput.CastPosition);
                    else if (Program.Combo && HaveStun && Config.Item("autoRcombo", true).GetValue<bool>())
                        R.Cast(poutput.CastPosition);
                    else if (Config.Item("autoRks", true).GetValue<bool>())
                    {
                        var comboDmg = OktwCommon.GetKsDamage(enemy, R);

                        if (W.IsReady() && RMANA + WMANA < Player.Mana)
                            comboDmg += W.GetDamage(enemy);

                        if (Q.IsReady() && RMANA + WMANA + QMANA < Player.Mana)
                            comboDmg += Q.GetDamage(enemy);

                        if (enemy.Health < comboDmg)
                            R.Cast(poutput.CastPosition);
                    }
                }
            }

            var t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (t.IsValidTarget())
            {
                if (W.IsReady() && Program.LagFree(2) && Config.Item("autoW", true).GetValue<bool>())
                {
                    var poutput = W.GetPrediction(t, true);
                    var aoeCount = poutput.AoeTargetsHitCount;

                    if (Program.Combo && RMANA + WMANA < Player.Mana)
                        W.Cast(poutput.CastPosition);
                    else if (Program.Farm && RMANA + WMANA + QMANA < Player.Mana && Config.Item("harrasW", true).GetValue<bool>())
                        W.Cast(poutput.CastPosition);
                    else
                    {
                        var wDmg = OktwCommon.GetKsDamage(t, W);
                        var qDmg = Q.GetDamage(t);
                        if (wDmg > t.Health)
                            W.Cast(poutput.CastPosition);
                        else if (qDmg + wDmg > t.Health && Player.Mana > QMANA + WMANA)
                            W.Cast(poutput.CastPosition);
                    }
                }

                if (Q.IsReady() && Program.LagFree(3) && Config.Item("autoQ", true).GetValue<bool>())
                {
                    if (Program.Combo && RMANA + WMANA < Player.Mana)
                        Q.Cast(t);
                    else if (Program.Farm && RMANA + WMANA + QMANA < Player.Mana && Config.Item("harrasQ", true).GetValue<bool>())
                        Q.Cast(t);
                    else
                    {
                        var qDmg = OktwCommon.GetKsDamage(t, Q);
                        var wDmg = W.GetDamage(t);
                        if (qDmg > t.Health)
                            Q.Cast(t);
                        else if (qDmg + wDmg > t.Health && Player.Mana > QMANA + WMANA)
                            Q.Cast(t);
                    }
                }
            }
            else if(Q.IsReady() || W.IsReady())
            {
                if (Config.Item("farmQ", true).GetValue<bool>())
                {
                    if (Config.Item("supportMode", true).GetValue<bool>())
                    {
                        if (Program.LaneClear && Player.Mana > RMANA + QMANA)
                            farm();
                    }
                    else
                    {
                        if (Q.IsReady() && (!HaveStun || Program.LaneClear) && Program.Farm)
                            farm();
                    }
                }
            }

            if (Program.LagFree(3))
            {
                if (!HaveStun)
                {
                    if (E.IsReady() && !Program.LaneClear && Config.Item("autoE", true).GetValue<bool>() && Player.Mana > RMANA + EMANA + QMANA + WMANA)
                        E.Cast();
                    else if (W.IsReady() && Player.InFountain())
                        W.Cast(Player.Position);
                }
                if (R.IsReady())
                {
                    if (Config.Item("tibers", true).GetValue<bool>() && HaveTibers)
                    {
                        var BestEnemy = TargetSelector.GetTarget(2000, TargetSelector.DamageType.Magical);
                        if (BestEnemy.IsValidTarget(2000) && Game.Time - TibbersTimer > 2)
                        {
                            Player.IssueOrder(GameObjectOrder.MovePet, BestEnemy.Position);
                            R.CastOnUnit(BestEnemy);
                            TibbersTimer = Game.Time;
                        }
                    }
                    else
                    {
                        Tibbers = null;
                    }
                }
            }
        }

        private void farm()
        {
            if(Program.LaneClear)
            { 
                var mobs = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (W.IsReady())
                        W.Cast(mob);
                    else if (Q.IsReady())
                        Q.Cast(mob);
                }
            }

            var minionsList = MinionManager.GetMinions(Player.ServerPosition, Q.Range);
            if (Q.IsReady())
            {
                var minion = minionsList.Where(x => HealthPrediction.LaneClearHealthPrediction(x, 200, 50) < Q.GetDamage(x) && x.Health > Player.GetAutoAttackDamage(x)).FirstOrDefault();
                    Q.Cast(minion);
            }
            else if (Program.LaneClear && W.IsReady() && Player.ManaPercent > Config.Item("Mana", true).GetValue<Slider>().Value && Config.Item("farmW", true).GetValue<bool>())
            {
                var farmLocation = W.GetCircularFarmLocation(minionsList, W.Width);
                if (farmLocation.MinionsHit > 1)
                    W.Cast(farmLocation.Position);
            }
        }

        private bool HaveTibers
        {
            get { return Player.HasBuff("infernalguardiantimer"); }
        }

        private void SetMana()
        {
            if ((Config.Item("manaDisable", true).GetValue<bool>() && Program.Combo) || Player.HealthPercent < 20)
            {
                QMANA = 0;
                WMANA = 0;
                EMANA = 0;
                RMANA = 0;
                return;
            }

            QMANA = Q.Instance.ManaCost;
            WMANA = W.Instance.ManaCost;
            EMANA = E.Instance.ManaCost;

            if (!R.IsReady() || HaveTibers)
                RMANA = 0;
            else 
                RMANA = R.Instance.ManaCost;
        }

        private void Drawing_OnDraw(EventArgs args)
        {

            if (Config.Item("qRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (Q.IsReady())
                        Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
                }
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
            }
            if (Config.Item("wRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (W.IsReady())
                        Utility.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Orange, 1, 1);
                }
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Orange, 1, 1);
            }

            if (Config.Item("rRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (R.IsReady())
                        Utility.DrawCircle(ObjectManager.Player.Position, R.Range + R.Width / 2, System.Drawing.Color.Gray, 1, 1);
                }
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, R.Range + R.Width / 2, System.Drawing.Color.Gray, 1, 1);
            }

        }
    }
}
