using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
namespace OneKeyToWin_AIO_Sebby
{
    class YasuoWall
    {
        public Vector3 YasuoPosition { get; set; }
        public float CastTime { get; set; }
        public Vector3 CastPosition { get; set; }
        public float WallLvl { get; set; }

        public YasuoWall()
        {
            CastTime = 0;
        }
    }

    class UnitIncomingDamage
    {
        public int TargetNetworkId { get; set; }
        public float Time { get; set; }
        public double Damage { get; set; }
        public bool Skillshot { get; set; }
    }


    class OktwCommon
    {
        private static int LastAATick = Utils.GameTimeTickCount;
        public static bool YasuoInGame = false;

        public static bool 
            blockMove = false,
            blockAttack = false,
            blockSpells = false;

        public static Obj_AI_Hero Player { get { return ObjectManager.Player; } }
        public static Orbwalking.Orbwalker Orbwalker = Program.Orbwalker;
        private static YasuoWall yasuoWall = new YasuoWall();

        private static List<UnitIncomingDamage> IncomingDamageList = new List<UnitIncomingDamage>();
        private static List<Obj_AI_Hero> ChampionList = new List<Obj_AI_Hero>();

        public void LoadOKTW()
        {
            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                ChampionList.Add(hero);
                if (hero.IsEnemy && hero.ChampionName == "Yasuo")
                    YasuoInGame = true;
            }

            Obj_AI_Base.OnIssueOrder += Obj_AI_Base_OnIssueOrder;
            Spellbook.OnCastSpell +=Spellbook_OnCastSpell;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnDoCast += Obj_AI_Base_OnDoCast;
            Orbwalking.AfterAttack += afterAttack;
        }

        private void afterAttack(AttackableUnit unit, AttackableUnit target)
        {
            
        }

        private void OnUpdate(EventArgs args)
        {
            if (Program.LagFree(4))
            {
                float time = Game.Time - 2;
                IncomingDamageList.RemoveAll(damage => time < damage.Time);
            }
        }

        public static bool CanCombo()
        {
            return !Player.IsWindingUp;
        }

        public static double GetIncomingDamage(Obj_AI_Hero target, float time = 0.5f, bool skillshots = true )
        {
            double totalDamage = 0;

            foreach (var damage in IncomingDamageList.Where(damage => damage.TargetNetworkId == target.NetworkId && Game.Time - time < damage.Time ))
            {
                if(skillshots)
                {
                    totalDamage += damage.Damage;
                }
                else
                {
                    if (!damage.Skillshot)
                        totalDamage += damage.Damage;
                }
            }

            return totalDamage;
        }

        private void Obj_AI_Base_OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (args.Target != null)
            {
                if (args.Target.Type == GameObjectType.obj_AI_Hero && !sender.IsMelee && args.Target.Team != sender.Team)
                {
                    IncomingDamageList.Add(new UnitIncomingDamage { Damage = sender.GetSpellDamage((Obj_AI_Base)args.Target, args.SData.Name), TargetNetworkId = args.Target.NetworkId, Time = Game.Time, Skillshot = false });
                }
            }
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            /////////////////  HP prediction
            if (args.Target != null )
            {
                if (args.Target.Type == GameObjectType.obj_AI_Hero && args.Target.Team != sender.Team && sender.IsMelee)
                {
                    
                    IncomingDamageList.Add(new UnitIncomingDamage { Damage = sender.GetSpellDamage((Obj_AI_Base)args.Target, args.SData.Name), TargetNetworkId = args.Target.NetworkId, Time = Game.Time, Skillshot = false });
                }
            }
            else 
            {
                foreach (var champion in ChampionList.Where(champion => champion.IsValid && !champion.IsDead && champion.Team != sender.Team && champion.Distance(sender) < 2000))
                {
                    var castArea = champion.Distance(args.End) * (args.End - champion.ServerPosition).Normalized() + champion.ServerPosition;
                    if (castArea.Distance(champion.ServerPosition) < champion.BoundingRadius / 2)
                        IncomingDamageList.Add(new UnitIncomingDamage { Damage = sender.GetSpellDamage(champion, args.SData.Name), TargetNetworkId = champion.NetworkId, Time = Game.Time, Skillshot = true });
                }
            }

            //////////////////////////

            if (!YasuoInGame)
                return;

            if (!sender.IsEnemy || sender.IsMinion || args.SData.IsAutoAttack() || sender.Type != GameObjectType.obj_AI_Hero)
                return;

            if (args.SData.Name == "YasuoWMovingWall")
            {
                yasuoWall.CastTime = Game.Time;
                yasuoWall.CastPosition = sender.Position.Extend(args.End, 400);
                yasuoWall.YasuoPosition = sender.Position;
                yasuoWall.WallLvl = sender.Spellbook.Spells[1].Level;
            }
        }

        public static bool CanHarras()
        {
            if ( !Player.IsWindingUp && !Player.UnderTurret(true) && Orbwalking.CanMove(50) && !ShouldWait())
                return true;
            else
                return false;
        }

        private static bool ShouldWait()
        {
            return
                MinionManager.GetMinions(Player.AttackRange + 300, MinionTypes.All, MinionTeam.Enemy).Any( minion =>
                            minion.IsValidTarget() && HealthPrediction.LaneClearHealthPrediction(minion, 300, 50) <= Player.GetAutoAttackDamage(minion));
        }


        public static float GetEchoLudenDamage(Obj_AI_Hero target)
        {
            float totalDamage = 0;

            if (Player.HasBuff("itemmagicshankcharge"))
            {
                if (Player.GetBuff("itemmagicshankcharge").Count == 100)
                {
                    totalDamage += (float)Player.CalcDamage(target, Damage.DamageType.Magical, 100 + 0.1 * Player.FlatMagicDamageMod);
                }
            }
            return totalDamage; 
        }

        private void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (blockSpells)
            {
                args.Process = false;
            }
        }

        private void Obj_AI_Base_OnIssueOrder(Obj_AI_Base sender, GameObjectIssueOrderEventArgs args)
        {
            if (!sender.IsMe)
                return;
            
            if (blockMove  && !args.IsAttackMove)
            {
                args.Process = false;
            } 
            if (blockAttack && args.IsAttackMove)
            {
                args.Process = false;
            }
        }

        public static bool CollisionYasuo(Vector3 from, Vector3 to)
        {
            if(!YasuoInGame)
                return false;

            if ( Game.Time - yasuoWall.CastTime > 4)
                return false;

            var level = yasuoWall.WallLvl;
            var wallWidth = (350 + 50 * level);
            var wallDirection = (yasuoWall.CastPosition.To2D() - yasuoWall.YasuoPosition.To2D()).Normalized().Perpendicular();
            var wallStart = yasuoWall.CastPosition.To2D() + wallWidth / 2f * wallDirection;
            var wallEnd = wallStart - wallWidth * wallDirection;

            if (wallStart.Intersection(wallEnd, to.To2D(), from.To2D()).Intersects)
            {
                return true;
            }
            return false;
        }

        public static bool IsFaced(Obj_AI_Hero target)
        {
            Vector2 LastWaypoint = target.GetWaypoints().Last();
            if (LastWaypoint.Distance(Player.Position) < target.Distance(Player.Position))
                return true;

            return false;
        }

        public static float GetKsDamage(Obj_AI_Hero t, Spell QWER)
        {
            var totalDmg = QWER.GetDamage(t);
            totalDmg -= t.HPRegenRate;

            if (totalDmg > t.Health)
            {
                if (Player.HasBuff("summonerexhaust"))
                    totalDmg = totalDmg * 0.6f;

                if (t.HasBuff("ferocioushowl"))
                    totalDmg = totalDmg * 0.7f;

                if (t.ChampionName == "Blitzcrank" && !t.HasBuff("BlitzcrankManaBarrierCD") && !t.HasBuff("ManaBarrier"))
                {
                    totalDmg -= t.Mana / 2f;
                }
            }

            totalDmg += (float)GetIncomingDamage(t);
            return totalDmg;
        }

        public static bool ValidUlt(Obj_AI_Base target)
        {
            if (target.HasBuffOfType(BuffType.PhysicalImmunity)
                || target.HasBuffOfType(BuffType.SpellImmunity)
                || target.IsZombie
                || target.IsInvulnerable
                || target.HasBuffOfType(BuffType.Invulnerability)
                || target.HasBuffOfType(BuffType.SpellShield)
                )
                return false;
            else
                return true;
        }

        public static bool CanMove(Obj_AI_Hero target)
        {
            if (target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare) || target.HasBuffOfType(BuffType.Knockup) ||
                target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Fear) || target.HasBuffOfType(BuffType.Knockback) ||
                target.HasBuffOfType(BuffType.Taunt) || target.HasBuffOfType(BuffType.Suppression) ||
                target.IsStunned || (target.IsChannelingImportantSpell() && !target.IsMoving) || target.MoveSpeed < 10)
            {
                return false;
            }
            else
                return true;
        }

        public static int CountEnemiesInRangeDeley(Vector3 position, float range, float delay)
        {
            int count = 0;
            foreach (var t in Program.Enemies.Where(t => t.IsValidTarget()))
            {
                Vector3 prepos = Prediction.GetPrediction(t, delay).CastPosition;
                if (position.Distance(prepos) < range)
                    count++;
            }
            return count;
        }

        public static void DrawLineRectangle(Vector3 start2, Vector3 end2, int radius, float width, System.Drawing.Color color)
        {
            Vector2 start = start2.To2D();
            Vector2 end = end2.To2D();
            var dir = (end - start).Normalized();
            var pDir = dir.Perpendicular();

            var rightStartPos = start + pDir * radius;
            var leftStartPos = start - pDir * radius;
            var rightEndPos = end + pDir * radius;
            var leftEndPos = end - pDir * radius;

            var rStartPos = Drawing.WorldToScreen(new Vector3(rightStartPos.X, rightStartPos.Y, ObjectManager.Player.Position.Z));
            var lStartPos = Drawing.WorldToScreen(new Vector3(leftStartPos.X, leftStartPos.Y, ObjectManager.Player.Position.Z));
            var rEndPos = Drawing.WorldToScreen(new Vector3(rightEndPos.X, rightEndPos.Y, ObjectManager.Player.Position.Z));
            var lEndPos = Drawing.WorldToScreen(new Vector3(leftEndPos.X, leftEndPos.Y, ObjectManager.Player.Position.Z));

            Drawing.DrawLine(rStartPos, rEndPos, width, color);
            Drawing.DrawLine(lStartPos, lEndPos, width, color);
            Drawing.DrawLine(rStartPos, lStartPos, width, color);
            Drawing.DrawLine(lEndPos, rEndPos, width, color);
        }

        public static List<Vector3> CirclePoints(float CircleLineSegmentN, float radius, Vector3 position)
        {
            List<Vector3> points = new List<Vector3>();
            for (var i = 1; i <= CircleLineSegmentN; i++)
            {
                var angle = i * 2 * Math.PI / CircleLineSegmentN;
                var point = new Vector3(position.X + radius * (float)Math.Cos(angle), position.Y + radius * (float)Math.Sin(angle), position.Z);
                points.Add(point);
            }
            return points;
        }

        public static void DravTriangleOKTW(float radius, Vector3 position , System.Drawing.Color color, float bold = 1 )
        {
            var positionV2 = Drawing.WorldToScreen(position);

            Vector2 a = new Vector2(positionV2.X + radius, positionV2.Y + radius / 2);
            Vector2 b = new Vector2(positionV2.X - radius, positionV2.Y + radius / 2);
            Vector2 c = new Vector2(positionV2.X, positionV2.Y - radius);

            Drawing.DrawLine(a[0], a[1], b[0], b[1], bold, color);
            Drawing.DrawLine(b[0], b[1], c[0], c[1], bold, color);
            Drawing.DrawLine(c[0], c[1], a[0], a[1], bold, color);

        }

        public static int GetBuffCount(Obj_AI_Base target, String buffName)
        {
            foreach (var buff in target.Buffs.Where(buff => buff.Name == buffName))
            {
                if (buff.Count == 0)
                    return 1;
                else
                    return buff.Count;
            }
            return 0;
        }

        public static int CountEnemyMinions(Obj_AI_Base target, float range)
        {
            var allMinions = MinionManager.GetMinions(target.Position, range);
            if (allMinions != null)
                return allMinions.Count;
            else
                return 0;
        }


        public static float GetPassiveTime(Obj_AI_Base target, String buffName)
        {
            return
                target.Buffs.OrderByDescending(buff => buff.EndTime - Game.Time)
                    .Where(buff => buff.Name == buffName)
                    .Select(buff => buff.EndTime)
                    .FirstOrDefault() - Game.Time;
        }
    }
}
