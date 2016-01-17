using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using SharpDX.Direct3D9;


namespace OneKeyToWin_AIO_Sebby.Core
{
    class OKTWdraws
    {
        private Menu Config = Program.Config;
        public static Orbwalking.Orbwalker Orbwalker = Program.Orbwalker;
        private Obj_AI_Hero Player { get { return ObjectManager.Player; } }
        public Spell Q, W, E, R, DrawSpell;
        public static Font Tahoma13, Tahoma13B, TextBold;
        private float spellFarmTimer = 0, IntroTimer = Game.Time;
        private Render.Sprite Intro;

        public void LoadOKTW()
        {
            Config.SubMenu("About OKTW©").AddItem(new MenuItem("logo", "Intro logo OKTW").SetValue(true));

            if (Config.Item("logo").GetValue<bool>())
            {
                Intro = new Render.Sprite(LoadImg("intro"), new Vector2((Drawing.Width / 2) - 500, (Drawing.Height / 2) - 350));
                Intro.Add(0);
                Intro.OnDraw();
            }

            Utility.DelayAction.Add(7000, () => Intro.Remove());


            Config.SubMenu("Utility, Draws OKTW©").AddItem(new MenuItem("disableDraws", "Disable Utility draws").SetValue(false));
            //Config.SubMenu("Utility, Draws OKTW©").AddItem(new MenuItem("disableChampion", "Disable AIO champion (utility mode only) need F5").SetValue(false));
            Config.SubMenu("Utility, Draws OKTW©").SubMenu("Draw AAcirlce OKTW© style").AddItem(new MenuItem("OrbDraw", "Draw AAcirlce OKTW© style").SetValue(false));
            Config.SubMenu("Utility, Draws OKTW©").SubMenu("Draw AAcirlce OKTW© style").AddItem(new MenuItem("orb", "Orbwalker target OKTW© style").SetValue(true));
            Config.SubMenu("Utility, Draws OKTW©").SubMenu("Draw AAcirlce OKTW© style").AddItem(new MenuItem("1", "pls disable Orbwalking > Drawing > AAcirlce"));
            Config.SubMenu("Utility, Draws OKTW©").SubMenu("Draw AAcirlce OKTW© style").AddItem(new MenuItem("2", "My HP: 0-30 red, 30-60 orange,60-100 green"));

            Config.SubMenu("Utility, Draws OKTW©").SubMenu("ChampionInfo").AddItem(new MenuItem("championInfo", "Game Info").SetValue(true));
            Config.SubMenu("Utility, Draws OKTW©").SubMenu("ChampionInfo").AddItem(new MenuItem("ShowKDA", "Show flash and R info").SetValue(true));
            Config.SubMenu("Utility, Draws OKTW©").SubMenu("ChampionInfo").AddItem(new MenuItem("GankAlert", "Gank Alert").SetValue(true));

            Config.SubMenu("Utility, Draws OKTW©").SubMenu("ChampionInfo").AddItem(new MenuItem("posX", "posX").SetValue(new Slider(20, 100, 0)));
            Config.SubMenu("Utility, Draws OKTW©").SubMenu("ChampionInfo").AddItem(new MenuItem("posY", "posY").SetValue(new Slider(10, 100, 0)));

            Config.SubMenu("Utility, Draws OKTW©").AddItem(new MenuItem("HpBar", "Dmg indicators BAR OKTW© style").SetValue(true));
            Config.SubMenu("Utility, Draws OKTW©").AddItem(new MenuItem("ShowClicks", "Show enemy clicks").SetValue(true));
            Config.SubMenu("Utility, Draws OKTW©").AddItem(new MenuItem("SS", "SS notification").SetValue(true));
            Config.SubMenu("Utility, Draws OKTW©").AddItem(new MenuItem("showWards", "Show hidden objects, wards").SetValue(true));
            Config.SubMenu("Utility, Draws OKTW©").AddItem(new MenuItem("minimap", "Mini-map hack").SetValue(true));
            if (Program.AIOmode != 2)
            {
                Config.SubMenu(Player.ChampionName).SubMenu("Farm").SubMenu("SPELLS FARM TOGGLE").AddItem(new MenuItem("spellFarm", "OKTW spells farm").SetValue(true)).Show();
                Config.SubMenu(Player.ChampionName).SubMenu("Farm").SubMenu("SPELLS FARM TOGGLE").AddItem(new MenuItem("spellFarmMode", "SPELLS FARM TOGGLE MODE").SetValue(new StringList(new[] { "Scroll down", "Scroll press", "Key toggle", "Disable" }, 1)));
                Config.SubMenu(Player.ChampionName).SubMenu("Farm").SubMenu("SPELLS FARM TOGGLE").AddItem(new MenuItem("spellFarmKeyToggle", "Key toggle").SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Toggle)));
                Config.SubMenu(Player.ChampionName).SubMenu("Farm").SubMenu("SPELLS FARM TOGGLE").AddItem(new MenuItem("showNot", "Show notification").SetValue(true));
                Config.Item("spellFarm").Permashow(true);
            }
            Tahoma13B = new Font( Drawing.Direct3DDevice, new FontDescription
               { FaceName = "Tahoma", Height = 14, Weight = FontWeight.Bold, OutputPrecision = FontPrecision.Default, Quality = FontQuality.ClearType });

            Tahoma13 = new Font( Drawing.Direct3DDevice, new FontDescription
                { FaceName = "Tahoma", Height = 14, OutputPrecision = FontPrecision.Default, Quality = FontQuality.ClearType });

            TextBold = new Font(Drawing.Direct3DDevice, new FontDescription
                {FaceName = "Impact", Height = 30, Weight = FontWeight.Normal, OutputPrecision = FontPrecision.Default, Quality = FontQuality.ClearType});

            Q = new Spell(SpellSlot.Q);
            E = new Spell(SpellSlot.E);
            W = new Spell(SpellSlot.W);
            R = new Spell(SpellSlot.R);



            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;
            Game.OnWndProc += Game_OnWndProc;

        }

        private void Game_OnWndProc(WndEventArgs args)
        {
            if (Program.AIOmode == 2)
                return;
            if (Config.Item("spellFarm") == null || Config.Item("spellFarmMode").GetValue<StringList>().SelectedIndex == 3)
            return;

            if ((Config.Item("spellFarmMode").GetValue<StringList>().SelectedIndex == 0 && args.Msg == 0x20a)
                || (Config.Item("spellFarmMode").GetValue<StringList>().SelectedIndex == 1 && args.Msg == 520)
                )
            {
                if (!Config.Item("spellFarm").GetValue<bool>())
                {
                    Config.Item("spellFarm").SetValue<bool>(true);
                    spellFarmTimer = Game.Time;

                    if(Config.Item("farmQ", true) != null)
                        Config.Item("farmQ", true).SetValue<bool>(true);

                    if (Config.Item("farmW", true) != null)
                        Config.Item("farmW", true).SetValue<bool>(true);

                    if (Config.Item("farmE", true) != null)
                        Config.Item("farmE", true).SetValue<bool>(true);

                    if (Config.Item("farmR", true) != null)
                        Config.Item("farmR", true).SetValue<bool>(true);
                }
                else
                {
                    Config.Item("spellFarm").SetValue<bool>(false);
                    spellFarmTimer = Game.Time;

                    if (Config.Item("farmQ", true) != null)
                        Config.Item("farmQ", true).SetValue<bool>(false);

                    if (Config.Item("farmW", true) != null)
                        Config.Item("farmW", true).SetValue<bool>(false);

                    if (Config.Item("farmE", true) != null)
                        Config.Item("farmE", true).SetValue<bool>(false);

                    if (Config.Item("farmR", true) != null)
                        Config.Item("farmR", true).SetValue<bool>(false);
                }
            }
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (Program.LagFree(0) && Program.AIOmode != 2 && Config.Item("spellFarmMode").GetValue<StringList>().SelectedIndex != 3 && Config.Item("spellFarm") != null && Config.Item("spellFarmMode").GetValue<StringList>().SelectedIndex == 2 && Config.Item("spellFarmKeyToggle").GetValue<KeyBind>().Active != Config.Item("spellFarm").GetValue<bool>())
            {
                if (Config.Item("spellFarmKeyToggle").GetValue<KeyBind>().Active )
                {
                    Config.Item("spellFarm").SetValue<bool>(true);
                    spellFarmTimer = Game.Time;

                    if (Config.Item("farmQ", true) != null)
                        Config.Item("farmQ", true).SetValue<bool>(true);

                    if (Config.Item("farmW", true) != null)
                        Config.Item("farmW", true).SetValue<bool>(true);

                    if (Config.Item("farmE", true) != null)
                        Config.Item("farmE", true).SetValue<bool>(true);

                    if (Config.Item("farmR", true) != null)
                        Config.Item("farmR", true).SetValue<bool>(true);
                }
                else
                {
                    Config.Item("spellFarm").SetValue<bool>(false);
                    spellFarmTimer = Game.Time;

                    if (Config.Item("farmQ", true) != null)
                        Config.Item("farmQ", true).SetValue<bool>(false);

                    if (Config.Item("farmW", true) != null)
                        Config.Item("farmW", true).SetValue<bool>(false);

                    if (Config.Item("farmE", true) != null)
                        Config.Item("farmE", true).SetValue<bool>(false);

                    if (Config.Item("farmR", true) != null)
                        Config.Item("farmR", true).SetValue<bool>(false);
                }
            }
        }

        private static System.Drawing.Bitmap LoadImg(string imgName)
        {
            var bitmap = Resource1.ResourceManager.GetObject(imgName) as System.Drawing.Bitmap;
            if (bitmap == null)
            {
                Console.WriteLine(imgName + ".png not found.");
            }
            return bitmap;
        }

        public static void drawText(string msg, Vector3 Hero, System.Drawing.Color color, int weight = 0)
        {
            var wts = Drawing.WorldToScreen(Hero);
            Drawing.DrawText(wts[0] - (msg.Length) * 5, wts[1] + weight, color, msg);
        }

        public static void DrawFontTextScreen(Font vFont, string vText, float vPosX, float vPosY, ColorBGRA vColor)
        {
            vFont.DrawText(null, vText, (int)vPosX, (int)vPosY, vColor);
        }

        public static void DrawFontTextMap(Font vFont, string vText, Vector3 Pos, ColorBGRA vColor)
        {
            var wts = Drawing.WorldToScreen(Pos);
            vFont.DrawText(null, vText, (int)wts[0] , (int)wts[1], vColor);
        }

        public static void drawLine(Vector3 pos1, Vector3 pos2, int bold, System.Drawing.Color color)
        {
            var wts1 = Drawing.WorldToScreen(pos1);
            var wts2 = Drawing.WorldToScreen(pos2);

            Drawing.DrawLine(wts1[0], wts1[1], wts2[0], wts2[1], bold, color);
        }

        private void Drawing_OnEndScene(EventArgs args)
        {
            if (Config.Item("minimap").GetValue<bool>())
            {
                foreach (var enemy in Program.Enemies)
                {
                    if (!enemy.IsVisible)
                    {
                        var ChampionInfoOne = Core.OKTWtracker.ChampionInfoList.Find(x => x.NetworkId == enemy.NetworkId);
                        if (ChampionInfoOne != null)
                        {
                            var wts = Drawing.WorldToMinimap(ChampionInfoOne.LastVisablePos);
                            DrawFontTextScreen(Tahoma13, enemy.ChampionName[0].ToString() + enemy.ChampionName[1].ToString(), wts[0], wts[1], Color.Yellow);
                        }
                    }
                }
            }
            if (Config.Item("showWards").GetValue<bool>())
            {
                foreach (var obj in OKTWward.HiddenObjList)
                {
                    if (obj.type == 1)
                    {
                        Utility.DrawCircle(obj.pos, 100, System.Drawing.Color.Yellow, 3, 20,true);
                    }

                    if (obj.type == 2)
                    {
                        Utility.DrawCircle(obj.pos, 100, System.Drawing.Color.HotPink, 3, 20,true);
                    }

                    if (obj.type == 3)
                    {
                        Utility.DrawCircle(obj.pos, 100, System.Drawing.Color.Orange, 3, 20,true);
                    }
                }
            }   
        }

        private void OnDraw(EventArgs args)
        {
           
            if (Config.Item("disableDraws").GetValue<bool>())
                return;

            if (Config.Item("showWards").GetValue<bool>())
            {
                var circleSize = 30;
                foreach (var obj in OKTWward.HiddenObjList)
                {
                    if (obj.type == 1)
                    {
                        OktwCommon.DravTriangleOKTW(circleSize, obj.pos, System.Drawing.Color.Yellow);
                        DrawFontTextMap(Tahoma13, "" + (int)(obj.endTime - Game.Time), obj.pos, SharpDX.Color.Yellow);
                    }

                    if (obj.type == 2)
                    {
                        OktwCommon.DravTriangleOKTW(circleSize, obj.pos, System.Drawing.Color.HotPink);
                        DrawFontTextMap(Tahoma13, "VW", obj.pos, SharpDX.Color.HotPink);
                    }
                    if (obj.type == 3)
                    {
                        OktwCommon.DravTriangleOKTW(circleSize, obj.pos, System.Drawing.Color.Orange);
                        DrawFontTextMap(Tahoma13, "! " + (int)(obj.endTime - Game.Time), obj.pos, SharpDX.Color.Orange);
                    }
                }
            }

            if (Program.AIOmode != 2 && Config.Item("showNot").GetValue<bool>() && Config.Item("spellFarm") != null && spellFarmTimer + 1 > Game.Time)
            {
                if (Config.Item("spellFarm").GetValue<bool>())
                    DrawFontTextScreen(TextBold, "SPELLS FARM ON", Drawing.Width * 0.5f, Drawing.Height * 0.4f, SharpDX.Color.GreenYellow);
                else
                    DrawFontTextScreen(TextBold, "SPELLS FARM OFF", Drawing.Width * 0.5f , Drawing.Height * 0.4f, SharpDX.Color.OrangeRed);
            }
            

            bool blink = true;

            if ((int)(Game.Time * 10) % 2 == 0)
                blink = false;

            var HpBar = Config.Item("HpBar").GetValue<bool>();
            var championInfo = Config.Item("championInfo").GetValue<bool>();
            var GankAlert = Config.Item("GankAlert").GetValue<bool>();
            var ShowKDA = Config.Item("ShowKDA").GetValue<bool>();
            var ShowClicks = Config.Item("ShowClicks").GetValue<bool>();
            float posY = ((float)Config.Item("posY").GetValue<Slider>().Value * 0.01f) * Drawing.Height;
            float posX = ((float)Config.Item("posX").GetValue<Slider>().Value * 0.01f) * Drawing.Width;
            float positionDraw = 0;
            float positionGang = 500;
            int Width = 103;
            int Height = 8;
            int XOffset = 10;
            int YOffset = 20;
            var FillColor = System.Drawing.Color.GreenYellow;
            var Color = System.Drawing.Color.Azure;
            float offset = 0;
                
            foreach (var enemy in Program.Enemies)
            {
                if (Config.Item("SS").GetValue<bool>())
                {
                    offset += 0.15f;
                    if (!enemy.IsVisible && !enemy.IsDead)
                    {
                        var ChampionInfoOne = OKTWtracker.ChampionInfoList.Find(x => x.NetworkId == enemy.NetworkId);
                        if (ChampionInfoOne != null && enemy != Program.jungler)
                        {
                            if ((int)(Game.Time * 10) % 2 == 0 && Game.Time - ChampionInfoOne.LastVisableTime > 3 && Game.Time - ChampionInfoOne.LastVisableTime < 7)
                            {
                                DrawFontTextScreen(TextBold, "SS " + enemy.ChampionName + " " + (int)(Game.Time - ChampionInfoOne.LastVisableTime), Drawing.Width * offset, Drawing.Height * 0.02f, SharpDX.Color.OrangeRed);
                            }
                            if (Game.Time - ChampionInfoOne.LastVisableTime >= 7)
                            {
                                DrawFontTextScreen(TextBold, "SS " + enemy.ChampionName + " " + (int)(Game.Time - ChampionInfoOne.LastVisableTime), Drawing.Width * offset, Drawing.Height * 0.02f, SharpDX.Color.OrangeRed);
                            }
                        }
                    }
                }
                
                if (enemy.IsValidTarget() && ShowClicks)
                {
                    var lastWaypoint = enemy.GetWaypoints().Last().To3D();
                    if (lastWaypoint.IsValid())
                    {
                        drawLine(enemy.Position, lastWaypoint, 1, System.Drawing.Color.Red);
                            
                        if (enemy.GetWaypoints().Count() > 1)
                            DrawFontTextMap(Tahoma13, enemy.ChampionName, lastWaypoint, SharpDX.Color.WhiteSmoke);
                    }
                }

                if (HpBar && enemy.IsHPBarRendered && Render.OnScreen(Drawing.WorldToScreen(enemy.Position)))
                {
                    var barPos = enemy.HPBarPosition;

                    float QdmgDraw = 0, WdmgDraw = 0, EdmgDraw = 0, RdmgDraw = 0, damage = 0; ;

                    if (Q.IsReady())
                        damage = damage + Q.GetDamage(enemy);

                    if (W.IsReady() && Player.ChampionName != "Kalista")
                        damage = damage + W.GetDamage(enemy);

                    if (E.IsReady())
                        damage = damage + E.GetDamage(enemy);

                    if (R.IsReady())
                        damage = damage + R.GetDamage(enemy);

                    if (Q.IsReady())
                        QdmgDraw = (Q.GetDamage(enemy) / damage);

                    if (W.IsReady() && Player.ChampionName != "Kalista")
                        WdmgDraw = (W.GetDamage(enemy) / damage);

                    if (E.IsReady())
                        EdmgDraw = (E.GetDamage(enemy) / damage);

                    if (R.IsReady())
                        RdmgDraw = (R.GetDamage(enemy) / damage);

                    var percentHealthAfterDamage = Math.Max(0, enemy.Health - damage) / enemy.MaxHealth;

                    var yPos = barPos.Y + YOffset;
                    var xPosDamage = barPos.X + XOffset + Width * percentHealthAfterDamage;
                    var xPosCurrentHp = barPos.X + XOffset + Width * enemy.Health / enemy.MaxHealth;

                    float differenceInHP = xPosCurrentHp - xPosDamage;
                    var pos1 = barPos.X + XOffset + (107 * percentHealthAfterDamage);

                    for (int i = 0; i < differenceInHP; i++)
                    {
                        if (Q.IsReady() && i < QdmgDraw * differenceInHP)
                            Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, System.Drawing.Color.Cyan);
                        else if (W.IsReady() && i < (QdmgDraw + WdmgDraw) * differenceInHP)
                            Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, System.Drawing.Color.Orange);
                        else if (E.IsReady() && i < (QdmgDraw + WdmgDraw + EdmgDraw) * differenceInHP)
                            Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, System.Drawing.Color.Yellow);
                        else if (R.IsReady() && i < (QdmgDraw + WdmgDraw + EdmgDraw + RdmgDraw) * differenceInHP)
                            Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, System.Drawing.Color.YellowGreen);
                    }
                }

                var kolor = System.Drawing.Color.GreenYellow;

                if (enemy.IsDead)
                    kolor = System.Drawing.Color.Gray;
                else if (!enemy.IsVisible)
                    kolor = System.Drawing.Color.OrangeRed;

                var kolorHP = System.Drawing.Color.GreenYellow;

                if (enemy.IsDead)
                    kolorHP = System.Drawing.Color.GreenYellow;
                else if ((int)enemy.HealthPercent < 30)
                    kolorHP = System.Drawing.Color.Red;
                else if ((int)enemy.HealthPercent < 60)
                    kolorHP = System.Drawing.Color.Orange;

                if (championInfo)
                {
                    positionDraw += 15;
                    DrawFontTextScreen(Tahoma13, "" + enemy.Level, posX - 25, posY + positionDraw, SharpDX.Color.White);
                    DrawFontTextScreen(Tahoma13, enemy.ChampionName, posX, posY + positionDraw, SharpDX.Color.White);

                    if (true)
                    {
                        var ChampionInfoOne = Core.OKTWtracker.ChampionInfoList.Find(x => x.NetworkId == enemy.NetworkId);
                        if (Game.Time - ChampionInfoOne.FinishRecallTime < 4 )
                        {
                            DrawFontTextScreen(Tahoma13, "FINISH" , posX - 90, posY + positionDraw, SharpDX.Color.GreenYellow);
                        }
                        else if (ChampionInfoOne.StartRecallTime <= ChampionInfoOne.AbortRecallTime && Game.Time - ChampionInfoOne.AbortRecallTime < 4)
                        {
                            DrawFontTextScreen(Tahoma13, "ABORT", posX - 90, posY + positionDraw, SharpDX.Color.Yellow);
                        }
                        else if (Game.Time - ChampionInfoOne.StartRecallTime < 8)
                        {

                                int recallPercent = (int)(((Game.Time - ChampionInfoOne.StartRecallTime) / 8) * 100);
                                float recallX1 = posX - 90;
                                float recallY1 = posY + positionDraw + 3;
                                float recallX2 = (recallX1 + ((int)recallPercent / 2)) + 1;
                                float recallY2 = posY + positionDraw + 3;
                                Drawing.DrawLine(recallX1, recallY1, recallX1 + 50, recallY2, 8, System.Drawing.Color.Red);
                                Drawing.DrawLine(recallX1, recallY1, recallX2, recallY2, 8, System.Drawing.Color.White);
                        }
                    }

                    if (ShowKDA)
                    {
                        var fSlot = enemy.Spellbook.Spells[4];
                        if (fSlot.Name != "summonerflash")
                            fSlot = enemy.Spellbook.Spells[5];

                        if (fSlot.Name == "summonerflash")
                        {
                            var fT = fSlot.CooldownExpires - Game.Time;
                            if (fT < 0)
                                DrawFontTextScreen(Tahoma13, "F rdy", posX + 110, posY + positionDraw, SharpDX.Color.GreenYellow);
                            else
                                DrawFontTextScreen(Tahoma13, "F " + (int)fT, posX + 110, posY + positionDraw, SharpDX.Color.Yellow);
                        }

                        if (enemy.Level > 5)
                        {
                            var rSlot = enemy.Spellbook.Spells[3];
                            var t = rSlot.CooldownExpires - Game.Time;

                            if (t < 0)
                                DrawFontTextScreen(Tahoma13, "R rdy", posX + 145, posY + positionDraw, SharpDX.Color.GreenYellow);
                            else
                                DrawFontTextScreen(Tahoma13, "R " + (int)t, posX + 145, posY + positionDraw, SharpDX.Color.Yellow);
                        }
                        else
                            DrawFontTextScreen(Tahoma13, "R ", posX + 145, posY + positionDraw, SharpDX.Color.Yellow);
                    }
                    
                    //Drawing.DrawText(posX - 70, posY + positionDraw, kolor, enemy.Level + " lvl");
                }

                var Distance = Player.Distance(enemy.Position);
                if (GankAlert && !enemy.IsDead && Distance > 1200)
                {
                    var wts = Drawing.WorldToScreen(ObjectManager.Player.Position.Extend(enemy.Position, positionGang));

                    wts[0] = wts[0];
                    wts[1] = wts[1] + 15;

                    if ((int)enemy.HealthPercent > 0)
                        Drawing.DrawLine(wts[0], wts[1], (wts[0] + ((int)enemy.HealthPercent) / 2) + 1, wts[1], 8, kolorHP);

                    if ((int)enemy.HealthPercent < 100)
                        Drawing.DrawLine((wts[0] + ((int)enemy.HealthPercent) / 2), wts[1], wts[0] + 50, wts[1], 8, System.Drawing.Color.White);

                    if (Distance > 3500 && enemy.IsVisible)
                    {
                        DrawFontTextMap(Tahoma13, enemy.ChampionName,Player.Position.Extend(enemy.Position, positionGang) , SharpDX.Color.White);
                    }
                    else if (!enemy.IsVisible)
                    {
                        var ChampionInfoOne = Core.OKTWtracker.ChampionInfoList.Find(x => x.NetworkId == enemy.NetworkId);
                        if (ChampionInfoOne != null )
                        {
                            if (Game.Time - ChampionInfoOne.LastVisableTime > 3 && Game.Time - ChampionInfoOne.LastVisableTime < 7)
                            {
                                if(blink)
                                     DrawFontTextMap(Tahoma13, "SS " + enemy.ChampionName + " " + (int)(Game.Time - ChampionInfoOne.LastVisableTime), Player.Position.Extend(enemy.Position, positionGang), SharpDX.Color.Yellow);
                            }
                            else
                            {
                                DrawFontTextMap(Tahoma13, "SS " + enemy.ChampionName + " " + (int)(Game.Time - ChampionInfoOne.LastVisableTime), Player.Position.Extend(enemy.Position, positionGang), SharpDX.Color.Yellow);
                            }
                        }
                        else
                            DrawFontTextMap(Tahoma13, "SS " + enemy.ChampionName, Player.Position.Extend(enemy.Position, positionGang), SharpDX.Color.Yellow);
                    }
                    else if (blink)
                    {
                        DrawFontTextMap(Tahoma13B, enemy.ChampionName, Player.Position.Extend(enemy.Position, positionGang), SharpDX.Color.OrangeRed);
                    }

                    if (Distance < 3500 && enemy.IsVisible && !Render.OnScreen(Drawing.WorldToScreen(Player.Position.Extend(enemy.Position, Distance + 500))))
                    {
                        drawLine(Player.Position.Extend(enemy.Position, 100), Player.Position.Extend(enemy.Position, positionGang - 100), (int)((3500 - Distance) / 300), System.Drawing.Color.OrangeRed);
                    }
                    else if (Distance < 3500 && !enemy.IsVisible && !Render.OnScreen(Drawing.WorldToScreen(Player.Position.Extend(enemy.Position, Distance + 500))))
                    {
                        var need = Core.OKTWtracker.ChampionInfoList.Find(x => x.NetworkId == enemy.NetworkId);
                        if (need != null && Game.Time - need.LastVisableTime < 5)
                        {
                            drawLine(Player.Position.Extend(enemy.Position, 100), Player.Position.Extend(enemy.Position, positionGang - 100), (int)((3500 - Distance) / 300), System.Drawing.Color.Gray);
                        }
                    }
                }
                positionGang = positionGang + 100;
            }

            if (Program.AIOmode != 2)
            {
                DrawOrbwalkerRange();
                DrawOrbwalkerTarget();
            }
            else
            {
                Drawing.DrawText(Drawing.Width * 0.2f, Drawing.Height * 1f, System.Drawing.Color.Cyan, "OKTW AIO only utility mode ON");
            }
        }

        private void DrawOrbwalkerRange()
        {
            if (Config.Item("OrbDraw").GetValue<bool>())
            {
                var aaRange = ObjectManager.Player.AttackRange + ObjectManager.Player.BoundingRadius * 2;
                if (Player.HealthPercent > 60)
                    Utility.DrawCircle(ObjectManager.Player.Position, aaRange, System.Drawing.Color.GreenYellow, 1, 1);
                else if (Player.HealthPercent > 30)
                    Utility.DrawCircle(ObjectManager.Player.Position, aaRange, System.Drawing.Color.Orange, 1, 1);
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, aaRange, System.Drawing.Color.Red, 1, 1);
            }
        }

        private void DrawOrbwalkerTarget()
        {
            if (Config.Item("orb").GetValue<bool>())
            {
                var orbT = Orbwalker.GetTarget();
                if (orbT.IsValidTarget())
                {
                    if (orbT.Health > orbT.MaxHealth * 0.6)
                        Utility.DrawCircle(orbT.Position, orbT.BoundingRadius, System.Drawing.Color.GreenYellow, 1, 1);
                    else if (orbT.Health > orbT.MaxHealth * 0.3)
                        Utility.DrawCircle(orbT.Position, orbT.BoundingRadius, System.Drawing.Color.Orange, 1, 1);
                    else
                        Utility.DrawCircle(orbT.Position, orbT.BoundingRadius, System.Drawing.Color.Red, 1, 1);
                }
            }
        }
    }
}
