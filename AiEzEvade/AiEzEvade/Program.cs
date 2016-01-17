using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;

namespace AiEzEvade
{
    class EEsettings
    {

        public int EvadeMode;
        
        public bool DodgeDangerous;
        public bool DodgeCircularSpells;
        public bool DodgeFOWSpells;
        public bool CheckSpellCollision;
        public bool ContinueMovement;

        public bool ClickOnlyOnce;
        public int TickLimiter;
        public int ReactionTime;
        public int SpellDetectionTime;
        public bool FastMovementBlock;

        public EEsettings(int evadeMode, bool dodgeDangerous, bool dodgeCircularSpells, bool dodgeFOWSpells, bool checkSpellCollision, bool continueMovement, bool clickOnlyOnce, int tickLimiter, int reactionTime, int spellDetectionTime, bool fastMovementBlock)
        {
            EvadeMode = evadeMode;
            DodgeDangerous = dodgeDangerous;
            DodgeCircularSpells = dodgeCircularSpells;
            DodgeFOWSpells = dodgeFOWSpells;
            CheckSpellCollision = checkSpellCollision;
            ContinueMovement = continueMovement;
            ClickOnlyOnce = clickOnlyOnce;
            TickLimiter = tickLimiter;
            ReactionTime = reactionTime;
            SpellDetectionTime = spellDetectionTime;
            FastMovementBlock = fastMovementBlock;
        }
    }

    class Program
    {
        static void Main(string[] args){CustomEvents.Game.OnGameLoad += Game_OnGameLoad;}

        public static EEsettings set = new EEsettings(2,true,true,true,false,false,true,100,100,100,false);
        public static Menu Config;
        public static Obj_AI_Hero Player { get { return ObjectManager.Player; } }
        public static int TickLimit = 0;
        public static int Mode = 3;

        private static void Game_OnGameLoad(EventArgs args)
        {
            Config = new Menu("AiEzEvade", "AiEzEvade", true);
            Config.AddToMainMenu();
            Config.SubMenu("LOW danger").AddItem(new MenuItem("LowHp", "Activation above % HP").SetValue(new Slider(70, 100, 0)));
            Config.SubMenu("LOW danger").AddItem(new MenuItem("LowEvadeMode", "Evade Mode").SetValue(new Slider(2, 2, 0)));
            Config.SubMenu("LOW danger").AddItem(new MenuItem("LowDodgeDangerous", "Dodge only Dangerous").SetValue(true));
            Config.SubMenu("LOW danger").AddItem(new MenuItem("LowDodgeCircularSpells", "Dodge Circular Spells").SetValue(false));
            Config.SubMenu("LOW danger").AddItem(new MenuItem("LowDodgeFOWSpells", "Dodge FOW Spells").SetValue(false));
            Config.SubMenu("LOW danger").AddItem(new MenuItem("LowCheckSpellCollision", "Check Spell Collision").SetValue(true));
            Config.SubMenu("LOW danger").AddItem(new MenuItem("LowClickOnlyOnce", "Click Only Once").SetValue(true));
            Config.SubMenu("LOW danger").AddItem(new MenuItem("LowTickLimiter", "Tick Limiter").SetValue(new Slider(100, 1000, 0)));
            Config.SubMenu("LOW danger").AddItem(new MenuItem("LowReactionTime", "Reaction Time").SetValue(new Slider(250, 1000, 0)));
            Config.SubMenu("LOW danger").AddItem(new MenuItem("LowSpellDetectionTime", "Spell Detection Time").SetValue(new Slider(100, 1000, 0)));
            Config.SubMenu("LOW danger").AddItem(new MenuItem("LowFastMovementBlock", "Fast Movement Block").SetValue(false));

            Config.SubMenu("MEDIUM danger").AddItem(new MenuItem("0", "Activation between LOW and HIGH"));
            Config.SubMenu("MEDIUM danger").AddItem(new MenuItem("MediumEvadeMode", "Evade Mode").SetValue(new Slider(2, 2, 0)));
            Config.SubMenu("MEDIUM danger").AddItem(new MenuItem("MediumDodgeDangerous", "Dodge only Dangerous").SetValue(false));
            Config.SubMenu("MEDIUM danger").AddItem(new MenuItem("MediumDodgeCircularSpells", "Dodge Circular Spells").SetValue(false));
            Config.SubMenu("MEDIUM danger").AddItem(new MenuItem("MediumDodgeFOWSpells", "Dodge FOW Spells").SetValue(true));
            Config.SubMenu("MEDIUM danger").AddItem(new MenuItem("MediumCheckSpellCollision", "Check Spell Collision").SetValue(true));
            Config.SubMenu("MEDIUM danger").AddItem(new MenuItem("MediumClickOnlyOnce", "Click Only Once").SetValue(true));
            Config.SubMenu("MEDIUM danger").AddItem(new MenuItem("MediumTickLimiter", "Tick Limiter").SetValue(new Slider(100, 1000, 0)));
            Config.SubMenu("MEDIUM danger").AddItem(new MenuItem("MediumReactionTime", "Reaction Time").SetValue(new Slider(200, 1000, 0)));
            Config.SubMenu("MEDIUM danger").AddItem(new MenuItem("MediumSpellDetectionTime", "Spell Detection Time").SetValue(new Slider(200, 1000, 0)));
            Config.SubMenu("MEDIUM danger").AddItem(new MenuItem("MediumFastMovementBlock", "Fast Movement Block").SetValue(false));

            Config.SubMenu("HIGH danger").AddItem(new MenuItem("HighHp", "Activation under % HP").SetValue(new Slider(35, 100, 0)));
            Config.SubMenu("HIGH danger").AddItem(new MenuItem("HighEvadeMode", "Evade Mode").SetValue(new Slider(1, 2, 0)));
            Config.SubMenu("HIGH danger").AddItem(new MenuItem("HighDodgeDangerous", "Dodge only Dangerous").SetValue(false));
            Config.SubMenu("HIGH danger").AddItem(new MenuItem("HighDodgeCircularSpells", "Dodge Circular Spells").SetValue(true));
            Config.SubMenu("HIGH danger").AddItem(new MenuItem("HighDodgeFOWSpells", "Dodge FOW Spells").SetValue(true));
            Config.SubMenu("HIGH danger").AddItem(new MenuItem("HighCheckSpellCollision", "Check Spell Collision").SetValue(true));
            Config.SubMenu("HIGH danger").AddItem(new MenuItem("HighClickOnlyOnce", "Click Only Once").SetValue(false));
            Config.SubMenu("HIGH danger").AddItem(new MenuItem("HighTickLimiter", "Tick Limiter").SetValue(new Slider(70, 1000, 0)));
            Config.SubMenu("HIGH danger").AddItem(new MenuItem("HighReactionTime", "Reaction Time").SetValue(new Slider(50, 1000, 0)));
            Config.SubMenu("HIGH danger").AddItem(new MenuItem("HighSpellDetectionTime", "Spell Detection Time").SetValue(new Slider(50, 1000, 0)));
            Config.SubMenu("HIGH danger").AddItem(new MenuItem("HighFastMovementBlock", "Fast Movement Block").SetValue(true));

            Game.OnUpdate += Game_OnGameUpdate;
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Utils.TickCount - TickLimit > 200)
            {
                TickLimit = Utils.TickCount;

                var newMode = 0;

                if(Player.HealthPercent < Config.Item("LowHp").GetValue<Slider>().Value)
                {
                    newMode = 2;
                }
                else if (Player.HealthPercent > Config.Item("HighHp").GetValue<Slider>().Value)
                {
                    newMode = 0;
                }
                else 
                {
                    newMode = 1;
                }

                if (newMode != Mode )
                {
                    if(newMode==0)
                    {
                        set = new EEsettings(
                            Config.Item("LowEvadeMode").GetValue<Slider>().Value,
                            Config.Item("LowDodgeDangerous").GetValue<bool>(),
                            Config.Item("LowDodgeCircularSpells").GetValue<bool>(),
                            Config.Item("LowDodgeFOWSpells").GetValue<bool>(),
                            Config.Item("LowCheckSpellCollision").GetValue<bool>(),
                            false,
                            Config.Item("LowClickOnlyOnce").GetValue<bool>(),
                            Config.Item("LowTickLimiter").GetValue<Slider>().Value,
                            Config.Item("LowReactionTime").GetValue<Slider>().Value,
                            Config.Item("LowSpellDetectionTime").GetValue<Slider>().Value, 
                            Config.Item("LowFastMovementBlock").GetValue<bool>());

                        SetEE(set);
                        Mode = newMode;
                        return;
                    }
                    if (newMode == 1)
                    {
                        set = new EEsettings(
                           Config.Item("MediumEvadeMode").GetValue<Slider>().Value,
                           Config.Item("MediumDodgeDangerous").GetValue<bool>(),
                           Config.Item("MediumDodgeCircularSpells").GetValue<bool>(),
                           Config.Item("MediumDodgeFOWSpells").GetValue<bool>(),
                           Config.Item("MediumCheckSpellCollision").GetValue<bool>(),
                           false,
                           Config.Item("MediumClickOnlyOnce").GetValue<bool>(),
                           Config.Item("MediumTickLimiter").GetValue<Slider>().Value,
                           Config.Item("MediumReactionTime").GetValue<Slider>().Value,
                           Config.Item("MediumSpellDetectionTime").GetValue<Slider>().Value,
                           Config.Item("MediumFastMovementBlock").GetValue<bool>());

                        SetEE(set);
                        Mode = newMode;
                        return;
                    }
                    if (newMode == 2)
                    {
                        set = new EEsettings(
                            Config.Item("HighEvadeMode").GetValue<Slider>().Value,
                            Config.Item("HighDodgeDangerous").GetValue<bool>(),
                            Config.Item("HighDodgeCircularSpells").GetValue<bool>(),
                            Config.Item("HighDodgeFOWSpells").GetValue<bool>(),
                            Config.Item("HighCheckSpellCollision").GetValue<bool>(),
                            false,
                            Config.Item("HighClickOnlyOnce").GetValue<bool>(),
                            Config.Item("HighTickLimiter").GetValue<Slider>().Value,
                            Config.Item("HighReactionTime").GetValue<Slider>().Value,
                            Config.Item("HighSpellDetectionTime").GetValue<Slider>().Value,
                            Config.Item("HighFastMovementBlock").GetValue<bool>());

                        SetEE(set);
                        Mode = newMode;
                        return;
                    }
                }
            }
        }

        private static void SetEE(EEsettings sets)
        {
            Menu.GetMenu("ezEvade", "ezEvade").Item("EvadeMode").SetValue<StringList>(new StringList(new[] { "Smooth", "Fastest", "Very Smooth" }, sets.EvadeMode));
            Menu.GetMenu("ezEvade", "ezEvade").Item("DodgeDangerous").SetValue<Boolean>(sets.DodgeDangerous);
            Menu.GetMenu("ezEvade", "ezEvade").Item("DodgeCircularSpells").SetValue<Boolean>(sets.DodgeCircularSpells);
            Menu.GetMenu("ezEvade", "ezEvade").Item("DodgeFOWSpells").SetValue<Boolean>(sets.DodgeFOWSpells);
            Menu.GetMenu("ezEvade", "ezEvade").Item("CheckSpellCollision").SetValue<Boolean>(sets.CheckSpellCollision);
            Menu.GetMenu("ezEvade", "ezEvade").Item("ContinueMovement").SetValue<Boolean>(sets.ContinueMovement);
            Menu.GetMenu("ezEvade", "ezEvade").Item("ContinueMovement").SetValue<Boolean>(sets.ClickOnlyOnce);
            Menu.GetMenu("ezEvade", "ezEvade").Item("TickLimiter").SetValue<Slider>(new Slider(sets.TickLimiter, 0, 500));
            Menu.GetMenu("ezEvade", "ezEvade").Item("ReactionTime").SetValue<Slider>(new Slider(sets.ReactionTime, 0, 500));
            Menu.GetMenu("ezEvade", "ezEvade").Item("SpellDetectionTime").SetValue<Slider>(new Slider(sets.SpellDetectionTime, 0, 1000));
            //Menu.GetMenu("ezEvade", "ezEvade").Item("FastMovementBlock").SetValue<Boolean>(sets.FastMovementBlock);
        }
    }
}
