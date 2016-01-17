using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;

namespace OneKeyToWin_AIO_Sebby.Core
{
    class OKTWlab
    {
        private GameObject obj;
        private float time = 0;
        private Vector3 from;
        private float castTime;
        public void LoadOKTW()
        {
            Obj_AI_Base.OnDelete += Obj_AI_Base_OnDelete;
            Obj_AI_Base.OnCreate += Obj_AI_Base_OnCreate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Drawing.OnDraw += Drawing_OnDraw;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Game.OnUpdate += Game_OnGameUpdate;
            Game.OnSendPacket += Game_OnSendPacket;
            Game.OnProcessPacket += Game_OnProcessPacket;
            Obj_AI_Base.OnBuffAdd += Obj_AI_Base_OnBuffAdd;
        }

        private void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            return;
            Program.debug("CAST");
        }

        private void Game_OnProcessPacket(GamePacketEventArgs args)
        {
            
        }

        private void Game_OnSendPacket(GamePacketEventArgs args)
        {
            return;
            if (args.GetPacketId() == 166)
            {
                //args.Process = false;
            }

            if (args.GetPacketId() != 102 && args.GetPacketId() != 215)
            {

                Program.debug(Game.Time + " PAck " + args.GetPacketId());
            }

        }

        private void Obj_AI_Base_OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffAddEventArgs args)
        {
            return;
            if (sender.IsMe)
                Program.debug(args.Buff.Name);
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
                // foreach (var buff in ObjectManager.Player.Buffs)
                // Program.debug(buff.Name);
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            return;
            //OktwCommon.DrawLineRectangle(Game.CursorPos, ObjectManager.Player.Position, 75, 1, System.Drawing.Color.DimGray);
            //drawText("Range " + ObjectManager.Player.Distance(Game.CursorPos), ObjectManager.Player.Position.Extend(Game.CursorPos, 400), System.Drawing.Color.Gray);


            int radius = 100;
            var start2 = ObjectManager.Player.ServerPosition;
            var end2 = Game.CursorPos;


            Vector2 start = start2.To2D();
            Vector2 end = end2.To2D();
            var dir = (end - start).Normalized();
            var pDir = dir.Perpendicular();

            var rightEndPos = end + pDir * radius;
            var leftEndPos = end - pDir * radius;

            
            var rEndPos = new Vector3(rightEndPos.X, rightEndPos.Y, ObjectManager.Player.Position.Z);
            var lEndPos  =new Vector3(leftEndPos.X, leftEndPos.Y, ObjectManager.Player.Position.Z);


            var step = start2.Distance(rEndPos) / 10;
            for (var i = 0; i < 10; i++)
            {
                var pr = start2.Extend(rEndPos, step * i);
                var pl = start2.Extend(lEndPos, step * i);
                Render.Circle.DrawCircle(pr, 50, System.Drawing.Color.Orange, 1);
                Render.Circle.DrawCircle(pl, 50, System.Drawing.Color.Orange, 1);
            }

            


            if (obj != null &&  obj.IsValid)
            {
                //Utility.DrawCircle(obj.Position, 100, System.Drawing.Color.Orange, 1, 1);
            }
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            //return;
            if (sender.IsMe && !args.SData.IsAutoAttack())
            {
                castTime = Game.Time;
                //Program.debug("speed: " +args.SData.MissileSpeed);
                //Program.debug("name: " + args.SData.Name);
                //Program.debug("" + args.SData.DelayTotalTimePercent);
                //time = Game.Time;
            }
        }

        private void Obj_AI_Base_OnCreate(GameObject sender, EventArgs args)
        {
            //return;
            if (sender.IsValid && sender.IsAlly && ObjectManager.Player.Distance(sender.Position) < 200)
            {
                //obj = sender;

                if (!sender.IsValid<MissileClient>())
                    return;

                MissileClient missile = (MissileClient)sender;
                if (missile.SData.LineWidth == 0)
                    return;
                Program.debug(" " + missile.SData.LineWidth + " " + missile.SData.MissileSpeed + " " + (Game.Time - castTime));
                if (missile.IsValid && missile.IsAlly && missile.SData.Name != null && (missile.SData.Name == "SivirQMissile" || missile.SData.Name == "SivirQMissileReturn"))
                {
                    
                }
                //Program.debug(""+);
                //Program.debug("cast time" +(time - Game.Time));
            }
        }

        private void Obj_AI_Base_OnDelete(GameObject sender, EventArgs args)
        {
            return;
            if (sender.IsValid)
            {
                
            }
        }

        public static void drawText(string msg, Vector3 Hero, System.Drawing.Color color, int weight = 0)
        {
            var wts = Drawing.WorldToScreen(Hero);
            Drawing.DrawText(wts[0] - (msg.Length) * 5, wts[1] + weight, color, msg);
        }
    }
}
