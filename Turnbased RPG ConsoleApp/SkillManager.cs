using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using static Turnbased_RPG_ConsoleApp.Basic;

namespace Turnbased_RPG_ConsoleApp
{
    public static class SkillManager
    {
        public static Action<string> callForSkill;
        public static SkillBase requestedSkill;

        public static Skill<Actor> Skip_Turn;
        public static Skill<Actor> Sleep;
        public static Skill<Actor> Immobile;
        public static Skill<Actor> Waste_Stare;
        public static Skill<Actor> Waste_Roar;
        public static Skill<Actor> Waste_Short_Circut;
        public static Skill<Actor> Waste_Jumping;


        public static Skill<Actor, Actor> Poison_Powder;
        public static Skill<Actor, List<Actor>> Poison_Cloud;

        //Testing
        public static Skill<Actor, List<Actor>> Damage_Allies_Test;
        public static Skill<Actor, List<Actor>> Poison_Allies_Test;
        public static Skill<Actor, List<Actor>> Ignite_Allies_Test;
        public static Skill<Actor, List<Actor>> Restore_CP_Allies_Test;
        //

        public static Skill<Actor, Actor> Attack;
        public static Skill<Actor> Guard;
        public static Skill<Actor, Actor> Scan_Lash;

        //Fire moves
        /// <summary>Level 1; Hit one</summary>
        public static Skill<Actor, Actor> Fireball;
        /// <summary>Level 2; Hit one</summary>
        public static Skill<Actor, Actor> Flame_Burst;
        /// <summary>Level 3; Hit one</summary>
        public static Skill<Actor, Actor> Eruption;
        /// <summary>Level 1; Hit all</summary>
        public static Skill<Actor, List<Actor>> Flare_Fall;
        /// <summary>Level 2; Hit all</summary>
        public static Skill<Actor, List<Actor>> Blazing_Vortex;
        /// <summary>Level 3; Hit all</summary>
        public static Skill<Actor, List<Actor>> Supernova;

        //Water moves
        /// <summary>Level 1; Hit one</summary>
        public static Skill<Actor, Actor> Water_Pulse;
        /// <summary>Level 2; Hit one</summary>
        public static Skill<Actor, Actor> Water_Jet;
        /// <summary>Level 3; Hit one</summary>
        public static Skill<Actor, Actor> Hydo_Cannon;
        //public static Skill<Actor, Actor> Torpedo;//Physical
        /// <summary>Level 1; Hit all</summary>
        public static Skill<Actor, List<Actor>> Waterfall;
        /// <summary>Level 2; Hit all</summary>
        public static Skill<Actor, List<Actor>> Deluge;
        /// <summary>Level 3; Hit all</summary>
        public static Skill<Actor, List<Actor>> Tsunami;

        //Plant moves
        /// <summary>Level 1; Hit one</summary>
        public static Skill<Actor, Actor> Vine_Whip;
        /// <summary>Level 2; Hit one</summary>
        public static Skill<Actor, Actor> Leaf_Cutter;
        /// <summary>Level 3; Hit one</summary>
        public static Skill<Actor, Actor> Needle_Tomb;
        /// <summary>Level 1; Hit all</summary>
        public static Skill<Actor, List<Actor>> Leaf_Storm;
        /// <summary>Level 2; Hit all</summary>
        public static Skill<Actor, List<Actor>> Root_Wave;
        /// <summary>Level 3; Hit all</summary>
        public static Skill<Actor, List<Actor>> Thorn_Canopy;

        //Ground moves
        /// <summary>Level 1; Hit one</summary>
        public static Skill<Actor, Actor> Pebble_Blast;
        /// <summary>Level 2; Hit one</summary>
        public static Skill<Actor, Actor> Geo_Shift;
        /// <summary>Level 3; Hit one</summary>
        public static Skill<Actor, Actor> Fissure;
        /// <summary>Level 1; Hit all</summary>
        public static Skill<Actor, List<Actor>> Rock_Slide;
        /// <summary>Level 2; Hit all</summary>
        public static Skill<Actor, List<Actor>> Spire_Wall;
        /// <summary>Level 3; Hit all</summary>
        public static Skill<Actor, List<Actor>> Earthquake;

        //Air moves
        /// <summary>Level 1; Hit one</summary>
        public static Skill<Actor, Actor> Wind_Slash;
        /// <summary>Level 2; Hit one</summary>
        public static Skill<Actor, Actor> Air_Cannon;
        /// <summary>Level 3; Hit one</summary>
        public static Skill<Actor, Actor> Sonic_Boom;
        /// <summary>Level 1; Hit all</summary>
        public static Skill<Actor, List<Actor>> Slash_Storm;
        /// <summary>Level 2; Hit all</summary>
        public static Skill<Actor, List<Actor>> Sky_Crusher;
        /// <summary>Level 3; Hit all</summary>
        public static Skill<Actor, List<Actor>> Hurricane;

        //Electric moves
        /// <summary>Level 1; Hit one</summary>
        public static Skill<Actor, Actor> Charge_Bolt;
        /// <summary>Level 2; Hit one</summary>
        public static Skill<Actor, Actor> Taser_Grip;
        /// <summary>Level 3; Hit one</summary>
        public static Skill<Actor, Actor> Ion_Overload;
        /// <summary>Level 1; Hit all</summary>
        public static Skill<Actor, List<Actor>> Electro_Wave;
        /// <summary>Level 2; Hit all</summary>
        public static Skill<Actor, List<Actor>> Tesla_Cannon;
        /// <summary>Level 3; Hit all</summary>
        public static Skill<Actor, List<Actor>> Gigawatt_Dischage;

        //Healing
        public static Skill<Actor, Actor> Healing_Powder;
        public static Skill<Actor, Actor> Super_Healing_Powder;
        public static Skill<Actor, Actor> Ultra_Healing_Powder;
        public static Skill<Actor, List<Actor>> Healing_Cloud;
        public static Skill<Actor, List<Actor>> Ultra_Healing_Cloud;

        public static Skill<Actor, Actor> Curing_Powder;
        public static Skill<Actor, List<Actor>> Curing_Cloud;
        public static Skill<Actor, Actor> Pheonix_Powder;
        public static Skill<Actor, Actor> Ultra_Pheonix_Powder;
        public static Skill<Actor, List<Actor>> Pheonix_Cloud;


        public static SkillBase GetSkillByName(string name)
        {
            callForSkill.Invoke(name);
            var skill = requestedSkill;

            if (skill == null)//use reflections as back up
            {
                name = name.Replace(' ', '_');
                var field = typeof(SkillManager).GetField(name, BindingFlags.Public | BindingFlags.Static);

                if (field.GetValue(null) is SkillBase)
                {
                    //print((field.GetValue(null) as SkillBase).skillName);
                    return field.GetValue(null) as SkillBase;
                }
                return null;
            }
            else
            {
                requestedSkill = null;
                return skill;
            }
        }

        public static int SpecialAttackFormula(Actor user, int rank = 1, int baseValue = 32, float scalar = 0.36f, bool varyDamage = true)
            {
                float rankDamper = 0.75f;
                if (rank >= 3)
                    rankDamper = 0.64f;

                int amount = (int)(baseValue * Math.Sqrt(Math.Pow(user.specialAttack, (rank * rankDamper).Clamp(1, 50))) * scalar);
                if (varyDamage)
                    amount += RandomInt(-(int)(amount * 0.1f), (int)(amount * 0.1f));
                return amount;
            }
        public static int BasicAttackFormula(Actor user, int rank = 1, float fluxPercent = 0.1f, int fluxMin = 2)
            {
                float rankDamper = 1.4f;

                //print((rank * rankDamper).Clamp(1, 50));
                int damage = (int)( (user.attack * Math.Sqrt(rank).Clamp(1, float.MaxValue)) + (1.25f * Math.Sqrt(user.level * 0.75f).Clamp(1, 100)) * Math.Pow(rank, rankDamper).Clamp(1, 50) );
                damage += RandomInt(-(int)(damage * fluxPercent).Clamp(fluxMin, float.MaxValue), (int)(damage * fluxPercent).Clamp(fluxMin, float.MaxValue));

                if (rank >= 3)
                    damage += (int)(user.attack * Math.Sqrt(rank).Clamp(1, float.MaxValue));

                return damage;
            }

        public static int SpecialHealingFormula(Actor user, int rank = 1, int baseValue = 30, float scalar = 1f)
            {
                float rankDamper = 1f;
                if (rank >= 3)
                    rankDamper = 1.2f;

                return (int)(baseValue * Math.Sqrt(rank * rankDamper).Clamp(1, 50) * scalar * (rank * 0.75f).Clamp(1, 50) );
            }

        public static void OutOfCP(string n)
            {
                print($"{n} didn't have enough CP");
            }
        public static void ConstructAllSkills()
        {

            Scan_Lash = new Skill<Actor, Actor>("Scan Lash", (user, target) =>
            {
                print($"{user.name} used {Scan_Lash.skillName} on {target.name}");
                //user.Attack(target);
                target.ModifyHealth(-2, Scan_Lash);

                Console.ForegroundColor = ConsoleColor.Cyan;
                print("-".Repeat(36));
                print($"Scan results for {target.name}:");
                print($"LV: {target.level}");
                print($"ELMT: {target.element.nameFromEnum}");

                target.statusEffects.RemoveAll(x => x == null);
                if (target.statusEffects.Count > 0)
                {
                    print($"STATUS: ", true);
                    for (int i = 0; i < target.statusEffects.Count; i++)
                    {
                        if (i > 0)
                            print(", ", true);
                        print(target.statusEffects[i].name, true);
                    }
                    print("");
                }

                print($"HP: {target.hp}/{target.maxHp}");
                print($"CP: {target.cp}/{target.maxCp}");
                //print("------------------------------------");//36
                print("-".Repeat(36));
                Console.ForegroundColor = ConsoleColor.White;
                Console.ReadKey(true);
            }
            , 0, SkillBase.TargetType.TARGET_SINGLE_OPPONENT, Element.IGNORE_ALL);

            Guard = new Skill<Actor>("Guard", (user) =>
            {
                //this move should ignore speed and happen before anything
                //it stops when the next turn order is decided
                print(user.name + " is guarding");
                user.isGuarding = true;
            }, pri: 2);

            #region TURN WASTING
            Skip_Turn = new Skill<Actor>("Do Nothing", (user) =>
            {
                if(user.hp > 0)
                    print($"{user.name} did nothing");
            });

            Sleep = new Skill<Actor>("Sleep", (user) =>
            {
                print($"{user.name} is fast asleep");
                user.ModifyConra(RandomInt(1, 3));
            });

            Immobile = new Skill<Actor>("Can't Move", (user) =>
            {
                print($"{user.name} couldn't move");
            });

            Waste_Stare = new Skill<Actor>("Stare", (user) =>
            {
                print($"{user.name} stared you down");
            });

            Waste_Roar = new Skill<Actor>("Roar", (user) =>
            {
                print($"{user.name} let out an intense roar");
            });

            Waste_Short_Circut = new Skill<Actor>("Short Circut", (user) =>
            {
                print($"{user.name} short circuited");
            });

            Waste_Jumping = new Skill<Actor>("Jump Around", (user) =>
            {
                print($"{user.name} is jumping around");
            });
            #endregion

            #region TESTING
            Damage_Allies_Test = new Skill<Actor, List<Actor>>("Damage Allies", (user, targets) =>
            {
                if (user.cp >= Damage_Allies_Test.skillCost)
                {
                    print($"{user.name} used {Damage_Allies_Test.skillName}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);
                    int amount = SpecialAttackFormula(user, 1);

                    foreach (var target in targets)
                    {
                        amount += RandomInt(-(int)(amount * 0.1f), (int)(amount * 0.1f));
                        target.ModifyHealth(-amount);
                    }

                    user.cp -= Damage_Allies_Test.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 0, SkillBase.TargetType.TARGET_ALL_ALLIES, skillType: SkillBase.SkillType.HEALING);
            Poison_Allies_Test = new Skill<Actor, List<Actor>>("Poison Allies", (user, targets) =>
            {
                var curSkill = Poison_Allies_Test;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} used {curSkill.skillName}");

                    foreach (var target in targets)
                    {
                        int amount = RandomInt(1, 3);
                        target.ModifyHealth(-amount, curSkill);

                        if (Chance(3, 3))
                        {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            StatusEffect.POISONED.TryInflict(target);
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 0, SkillBase.TargetType.TARGET_ALL_ALLIES, Element.IGNORE_ALL, SkillBase.SkillType.HEALING);
            Ignite_Allies_Test = new Skill<Actor, List<Actor>>("Ignite Allies", (user, targets) =>
            {
                var curSkill = Ignite_Allies_Test;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} used {curSkill.skillName}");

                    foreach (var target in targets)
                    {
                        int amount = RandomInt(1, 3);
                        target.ModifyHealth(-amount, curSkill);

                        if (Chance(3, 3))
                        {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            StatusEffect.FLAMING.TryInflict(target);
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 0, SkillBase.TargetType.TARGET_ALL_ALLIES, Element.IGNORE_ALL, SkillBase.SkillType.HEALING);
            Restore_CP_Allies_Test = new Skill<Actor, List<Actor>>("Restore CP of Allies", (user, targets) =>
            {
                var curSkill = Restore_CP_Allies_Test;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} used {curSkill.skillName}");

                    foreach (var target in targets)
                    {
                        target.ModifyConra(target.maxCp);
                    }

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 0, SkillBase.TargetType.TARGET_ALL_ALLIES, Element.IGNORE_ALL, SkillBase.SkillType.HEALING);
            #endregion

            #region STATUS EFFECT GIVING
            Poison_Powder = new Skill<Actor, Actor>("Poison Powder", (user, target) =>
            {
                var curSkill = Poison_Powder;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} produced {curSkill.skillName}");
                    
                    int amount = RandomInt(1, 3);
                    target.ModifyHealth(-amount, curSkill);

                    if (Chance(3, 3))
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        StatusEffect.POISONED.TryInflict(target);
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 5, SkillBase.TargetType.TARGET_SINGLE_OPPONENT, Element.PLANT, SkillBase.SkillType.INFLICTING);

            Poison_Cloud = new Skill<Actor, List<Actor>>("Poison Cloud", (user, targets) =>
            {
                var curSkill = Poison_Cloud;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} produced {curSkill.skillName}");

                    foreach (var target in targets)
                    {
                        int amount = RandomInt(1, 3);
                        target.ModifyHealth(-amount, curSkill);

                        if (Chance(2, 3))
                        {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            StatusEffect.POISONED.TryInflict(target);
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 14, SkillBase.TargetType.TARGET_ALL_OPPONENTS, Element.PLANT, SkillBase.SkillType.INFLICTING);
            #endregion

            #region ATTACKS
            Attack = new Skill<Actor, Actor>("Attack", (user, target) =>
            {
                print($"{user.name} attacked {target.name}");
                //user.Attack(target);
                target.ModifyHealth(-BasicAttackFormula(user, 1));
            }
            , 0, SkillBase.TargetType.TARGET_SINGLE_OPPONENT);

                #region FIRE
            Fireball = new Skill<Actor, Actor>("Fireball", (user, target) =>
            {
                var curSkill = Fireball;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} shot out a fireball");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);
                    int amount = SpecialAttackFormula(user);
                    target.ModifyHealth(-amount, curSkill);

                    if (Chance(1, 6))
                        StatusEffect.FLAMING.TryInflict(target);

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 4, SkillBase.TargetType.TARGET_SINGLE_OPPONENT, Element.FIRE);

            Flame_Burst = new Skill<Actor, Actor>("Flame Burst", (user, target) =>
            {
                var curSkill = Flame_Burst;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} used {curSkill.skillName}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);
                    int amount = SpecialAttackFormula(user, 2);
                    target.ModifyHealth(-amount, curSkill);

                    if (Chance(1, 6))
                        StatusEffect.FLAMING.TryInflict(target);

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 8, SkillBase.TargetType.TARGET_SINGLE_OPPONENT, Element.FIRE);

            Eruption = new Skill<Actor, Actor>("Eruption", (user, target) =>
            {
                var curSkill = Eruption;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} used {curSkill.skillName}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);
                    int amount = SpecialAttackFormula(user, 3);
                    target.ModifyHealth(-amount, curSkill);

                    if (Chance(1, 3))
                        StatusEffect.FLAMING.TryInflict(target);

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 24, SkillBase.TargetType.TARGET_SINGLE_OPPONENT, Element.FIRE);

            Flare_Fall = new Skill<Actor, List<Actor>>("Flare Fall", (user, targets) =>
            {
                var curSkill = Flare_Fall;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} used {curSkill.skillName}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);

                    foreach (var target in targets)
                    {
                        int amount = SpecialAttackFormula(user, 1);
                        target.ModifyHealth(-amount, curSkill);

                        if (Chance(1, 8))
                            StatusEffect.FLAMING.TryInflict(target);
                    }

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 8, SkillBase.TargetType.TARGET_ALL_OPPONENTS, Element.FIRE);

            Blazing_Vortex = new Skill<Actor, List<Actor>>("Blazing Vortex", (user, targets) =>
            {
                var curSkill = Blazing_Vortex;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} used {curSkill.skillName}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);
                    foreach (var target in targets)
                    {
                        int amount = SpecialAttackFormula(user, 2);
                        target.ModifyHealth(-amount, curSkill);

                        if (Chance(1, 4))
                            StatusEffect.FLAMING.TryInflict(target);
                    }

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 16, SkillBase.TargetType.TARGET_ALL_OPPONENTS, Element.FIRE);

            Supernova = new Skill<Actor, List<Actor>>("Supernova", (user, targets) =>
            {
                var curSkill = Supernova;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} used {curSkill.skillName}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);
                    foreach (var target in targets)
                    {
                        int amount = SpecialAttackFormula(user, 3);
                        target.ModifyHealth(-amount, curSkill);

                        if (Chance(1, 2))
                            StatusEffect.FLAMING.TryInflict(target);
                    }

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 50, SkillBase.TargetType.TARGET_ALL_OPPONENTS, Element.FIRE);
                #endregion
            
                #region WATER
            Water_Pulse = new Skill<Actor, Actor>("Water Pulse", (user, target) =>
            {
                var curSkill = Water_Pulse;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} used {curSkill.skillName}");

                    target.ModifyHealth(-SpecialAttackFormula(user, 1), curSkill);
                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 4, SkillBase.TargetType.TARGET_SINGLE_OPPONENT, Element.WATER);

            Water_Jet = new Skill<Actor, Actor>("Water Jet", (user, target) =>
            {
                var curSkill = Water_Jet;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} used {curSkill.skillName}");

                    target.ModifyHealth(-SpecialAttackFormula(user, 2), curSkill);

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 8, SkillBase.TargetType.TARGET_SINGLE_OPPONENT, Element.WATER);

            Hydo_Cannon = new Skill<Actor, Actor>("Hydo Cannon", (user, target) =>
            {
                var curSkill = Hydo_Cannon;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} used {curSkill.skillName}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);
                    int amount = (int)(SpecialAttackFormula(user, 3) * 0.75f);

                    if (!target.isGuarding && Chance(1, 3))
                        StatusEffect.CONFUSED.TryInflict(target);

                    target.ModifyHealth(-amount, curSkill);

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 28, SkillBase.TargetType.TARGET_SINGLE_OPPONENT, Element.WATER);

            Waterfall = new Skill<Actor, List<Actor>>("Waterfall", (user, targets) =>
            {
                var curSkill = Waterfall;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} used {curSkill.skillName}");

                    foreach (var target in targets)
                    {
                        int amount = SpecialAttackFormula(user, 1);
                        target.ModifyHealth(-amount, curSkill);
                    }

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 9, SkillBase.TargetType.TARGET_ALL_OPPONENTS, Element.WATER);

            Deluge = new Skill<Actor, List<Actor>>("Deluge", (user, targets) =>
            {
                var curSkill = Deluge;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} used {curSkill.skillName}");

                    foreach (var target in targets)
                    {
                        int amount = SpecialAttackFormula(user, 2);
                        target.ModifyHealth(-amount, curSkill);
                    }

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 16, SkillBase.TargetType.TARGET_ALL_OPPONENTS, Element.WATER);

            Tsunami = new Skill<Actor, List<Actor>>("Tsunami", (user, targets) =>
            {
                var curSkill = Tsunami;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} used {curSkill.skillName}");

                    foreach (var target in targets)
                    {
                        int amount = (int)(SpecialAttackFormula(user, 3) * 0.75f);

                        if (Chance(1, 3))
                            StatusEffect.CONFUSED.TryInflict(target);

                        target.ModifyHealth(-amount, curSkill);
                    }

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 45, SkillBase.TargetType.TARGET_ALL_OPPONENTS, Element.WATER);
                #endregion

                #region PLANT
            Vine_Whip = new Skill<Actor, Actor>("Vine Whip", (user, target) =>
            {
                var curSkill = Vine_Whip;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} used {curSkill.skillName}");

                    target.ModifyHealth(-BasicAttackFormula(user, 2), curSkill);
                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 3, SkillBase.TargetType.TARGET_SINGLE_OPPONENT, Element.PLANT);

            Leaf_Cutter = new Skill<Actor, Actor>("Leaf Cutter", (user, target) =>
            {
                var curSkill = Leaf_Cutter;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} used {curSkill.skillName}");

                    int amount = (int)(SpecialAttackFormula(user, 1) * 0.5 + BasicAttackFormula(user, 2));
                    target.ModifyHealth(-amount, curSkill);

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 9, SkillBase.TargetType.TARGET_SINGLE_OPPONENT, Element.PLANT);

            Needle_Tomb = new Skill<Actor, Actor>("Needle Tomb", (user, target) =>
            {
                var curSkill = Needle_Tomb;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} used {curSkill.skillName}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);
                    int amount = (int)( SpecialAttackFormula(user, 1) * 0.75 + BasicAttackFormula(user, 3));

                    if (Chance(1, 5))
                        StatusEffect.PARALYZED.TryInflict(target);

                    target.ModifyHealth(-amount, curSkill);

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 18, SkillBase.TargetType.TARGET_SINGLE_OPPONENT, Element.PLANT);

            Leaf_Storm = new Skill<Actor, List<Actor>>("Leaf Storm", (user, targets) =>
            {
                var curSkill = Leaf_Storm;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} used {curSkill.skillName}");

                    foreach (var target in targets)
                    {
                        int amount = (int)(SpecialAttackFormula(user, 1) * 0.1f + BasicAttackFormula(user, 2));
                        target.ModifyHealth(-amount, curSkill);
                    }

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 9, SkillBase.TargetType.TARGET_ALL_OPPONENTS, Element.PLANT);

            Root_Wave = new Skill<Actor, List<Actor>>("Root Wave", (user, targets) =>
            {
                var curSkill = Root_Wave;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} used {curSkill.skillName}");

                    foreach (var target in targets)
                    {
                        int amount = (int)(SpecialAttackFormula(user, 1) * 0.5 + BasicAttackFormula(user, 2));
                        target.ModifyHealth(-amount, curSkill);
                    }

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 16, SkillBase.TargetType.TARGET_ALL_OPPONENTS, Element.PLANT);

            Thorn_Canopy = new Skill<Actor, List<Actor>>("Thorn Canopy", (user, targets) =>
            {
                var curSkill = Thorn_Canopy;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} used {curSkill.skillName}");

                    foreach (var target in targets)
                    {
                        if (Chance(1, 5))
                            StatusEffect.PARALYZED.TryInflict(target);
                        int amount = (int)(SpecialAttackFormula(user, 1) * 0.75 + BasicAttackFormula(user, 3));
                        target.ModifyHealth(-amount, curSkill);
                    }

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 38, SkillBase.TargetType.TARGET_ALL_OPPONENTS, Element.PLANT);
            #endregion

                #region GROUND
            Pebble_Blast = new Skill<Actor, Actor>("Pebble Blast", (user, target) =>
            {
                var curSkill = Pebble_Blast;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} used {curSkill.skillName}");

                    int total = 0;
                    for (int i = 0; i < 6; i++)//can hit 6 times
                    {
                        int amount = (int)(BasicAttackFormula(user, 1) / 3f).Clamp(2, int.MaxValue);
                        if (i <= 1)
                        {
                            total += amount;
                            target.ModifyHealth(-amount, curSkill);
                        }
                        else if (Chance(1, 2))
                        {
                            total += amount;
                            target.ModifyHealth(-amount, curSkill);
                        }
                        Thread.Sleep(10);//For randomization
                    }
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    print($"{total} total damage on {target.name}!");
                    Console.ForegroundColor = ConsoleColor.White;

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 4, SkillBase.TargetType.TARGET_SINGLE_OPPONENT, Element.GROUND);

            Geo_Shift = new Skill<Actor, Actor>("Geo Shift", (user, target) =>
            {
                var curSkill = Geo_Shift;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} used {curSkill.skillName}");

                    int amount = BasicAttackFormula(user, 3) + (int)(SpecialAttackFormula(user, 1) * 0.25f);
                    target.ModifyHealth(-amount, curSkill);

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 8, SkillBase.TargetType.TARGET_SINGLE_OPPONENT, Element.GROUND);

            Fissure = new Skill<Actor, Actor>("Fissure", (user, target) =>
            {
                var curSkill = Fissure;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} used {curSkill.skillName}");

                    int amount = BasicAttackFormula(user, 4) + (int)(SpecialAttackFormula(user, 1) * 0.75f);
                    target.ModifyHealth(-amount, curSkill);

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 24, SkillBase.TargetType.TARGET_SINGLE_OPPONENT, Element.GROUND);

            Rock_Slide = new Skill<Actor, List<Actor>>("Rock Slide", (user, targets) =>
            {
                var curSkill = Rock_Slide;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} used {curSkill.skillName}");

                    foreach (var target in targets)
                    {
                        int total = 0;
                        for (int i = 0; i < 4; i++)//can hit 4 times
                        {
                            int amount = (int)(BasicAttackFormula(user, 1) / 2f).Clamp(2, int.MaxValue);
                            if (i <= 1)
                            {
                                total += amount;
                                target.ModifyHealth(-amount, curSkill);
                            }
                            else if (Chance(1, 2))
                            {
                                total += amount;
                                target.ModifyHealth(-amount, curSkill);
                            }
                            Thread.Sleep(10);//For randomization
                        }
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        print($"{total} total damage on {target.name}!");
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 8, SkillBase.TargetType.TARGET_ALL_OPPONENTS, Element.GROUND);

            Spire_Wall = new Skill<Actor, List<Actor>>("Spire Wall", (user, targets) =>
            {
                var curSkill = Spire_Wall;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} used {curSkill.skillName}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);
                    foreach (var target in targets)
                    {
                        int amount = BasicAttackFormula(user, 3) + (int)(SpecialAttackFormula(user, 1) * 0.25f);
                        target.ModifyHealth(-amount, curSkill);
                    }

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 16, SkillBase.TargetType.TARGET_ALL_OPPONENTS, Element.GROUND);

            Earthquake = new Skill<Actor, List<Actor>>("Earthquake", (user, targets) =>
            {
                var curSkill = Earthquake;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} used {curSkill.skillName}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);
                    foreach (var target in targets)
                    {
                        int amount = BasicAttackFormula(user, 4) + (int)(SpecialAttackFormula(user, 1) * 0.75f);
                        target.ModifyHealth(-amount, curSkill);
                    }

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 50, SkillBase.TargetType.TARGET_ALL_OPPONENTS, Element.GROUND);
                #endregion

                #region AIR
            Wind_Slash = new Skill<Actor, Actor>("Wind Slash", (user, target) =>
            {
                var curSkill = Wind_Slash;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} used {curSkill.skillName}");

                    target.ModifyHealth(-SpecialAttackFormula(user, 1), curSkill);
                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 4, SkillBase.TargetType.TARGET_SINGLE_OPPONENT, Element.AIR);

            Air_Cannon = new Skill<Actor, Actor>("Air Cannon", (user, target) =>
            {
                var curSkill = Air_Cannon;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} used {curSkill.skillName}");

                    target.ModifyHealth(-SpecialAttackFormula(user, 2), curSkill);

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 8, SkillBase.TargetType.TARGET_SINGLE_OPPONENT, Element.AIR);

            Sonic_Boom = new Skill<Actor, Actor>("Sonic Boom", (user, target) =>
            {
                var curSkill = Sonic_Boom;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} used {curSkill.skillName}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);
                    int amount = (SpecialAttackFormula(user, 3));

                    target.ModifyHealth(-amount, curSkill);

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 28, SkillBase.TargetType.TARGET_SINGLE_OPPONENT, Element.AIR);

            Slash_Storm = new Skill<Actor, List<Actor>>("Slash Storm", (user, targets) =>
            {
                var curSkill = Slash_Storm;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} used {curSkill.skillName}");

                    foreach (var target in targets)
                    {
                        int amount = SpecialAttackFormula(user, 1);
                        target.ModifyHealth(-amount, curSkill);
                    }

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 7, SkillBase.TargetType.TARGET_ALL_OPPONENTS, Element.AIR);

            Sky_Crusher = new Skill<Actor, List<Actor>>("Sky Crusher", (user, targets) =>
            {
                var curSkill = Sky_Crusher;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} used {curSkill.skillName}");

                    foreach (var target in targets)
                    {
                        int amount = SpecialAttackFormula(user, 2);
                        target.ModifyHealth(-amount, curSkill);
                    }

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 14, SkillBase.TargetType.TARGET_ALL_OPPONENTS, Element.AIR);

            Hurricane = new Skill<Actor, List<Actor>>("Tsunami", (user, targets) =>
            {
                var curSkill = Tsunami;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} used {curSkill.skillName}");

                    foreach (var target in targets)
                    {
                        int amount = SpecialAttackFormula(user, 3);

                        target.ModifyHealth(-amount, curSkill);
                    }

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 38, SkillBase.TargetType.TARGET_ALL_OPPONENTS, Element.AIR);
                #endregion

                #region ELECTRIC
            Charge_Bolt = new Skill<Actor, Actor>("Charge Bolt", (user, target) =>
            {
                var curSkill = Charge_Bolt;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} used {curSkill.skillName}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);
                    int amount = SpecialAttackFormula(user);
                    target.ModifyHealth(-amount, curSkill);

                    if (Chance(1, 6))
                        StatusEffect.PARALYZED.TryInflict(target);

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 4, SkillBase.TargetType.TARGET_SINGLE_OPPONENT, Element.ELECTRIC);

            Taser_Grip = new Skill<Actor, Actor>("Taser Grip", (user, target) =>
            {
                var curSkill = Taser_Grip;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} used {curSkill.skillName}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);
                    int amount = SpecialAttackFormula(user, 2);
                    target.ModifyHealth(-amount, curSkill);

                    if (Chance(1, 4))
                        StatusEffect.PARALYZED.TryInflict(target);

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 8, SkillBase.TargetType.TARGET_SINGLE_OPPONENT, Element.ELECTRIC);

            Ion_Overload = new Skill<Actor, Actor>("Ion Overload", (user, target) =>
            {
                var curSkill = Ion_Overload;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} used {curSkill.skillName}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);
                    int amount = SpecialAttackFormula(user, 3);
                    target.ModifyHealth(-amount, curSkill);

                    if (Chance(1, 2))
                        StatusEffect.PARALYZED.TryInflict(target);

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 28, SkillBase.TargetType.TARGET_SINGLE_OPPONENT, Element.ELECTRIC);

            Electro_Wave = new Skill<Actor, List<Actor>>("Electro Wave", (user, targets) =>
            {
                var curSkill = Electro_Wave;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} used {curSkill.skillName}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);

                    foreach (var target in targets)
                    {
                        int amount = SpecialAttackFormula(user, 1);
                        target.ModifyHealth(-amount, curSkill);

                        if (Chance(1, 6))
                            StatusEffect.PARALYZED.TryInflict(target);
                    }

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 8, SkillBase.TargetType.TARGET_ALL_OPPONENTS, Element.ELECTRIC);

            Tesla_Cannon = new Skill<Actor, List<Actor>>("Tesla Cannon", (user, targets) =>
            {
                var curSkill = Tesla_Cannon;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} used {curSkill.skillName}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);
                    foreach (var target in targets)
                    {
                        int amount = SpecialAttackFormula(user, 2);
                        target.ModifyHealth(-amount, curSkill);

                        if (Chance(1, 4))
                            StatusEffect.PARALYZED.TryInflict(target);
                    }

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 16, SkillBase.TargetType.TARGET_ALL_OPPONENTS, Element.ELECTRIC);

            Gigawatt_Dischage = new Skill<Actor, List<Actor>>("Gigawatt_Dischage", (user, targets) =>
            {
                var curSkill = Gigawatt_Dischage;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} used {curSkill.skillName}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);
                    foreach (var target in targets)
                    {
                        int amount = SpecialAttackFormula(user, 3);
                        target.ModifyHealth(-amount, curSkill);

                        if (Chance(1, 2))
                            StatusEffect.PARALYZED.TryInflict(target);
                    }

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 50, SkillBase.TargetType.TARGET_ALL_OPPONENTS, Element.ELECTRIC);
                #endregion

            #endregion

            #region HEALING AND CURING
            Healing_Powder = new Skill<Actor, Actor>("Healing Powder", (user, target) =>
            {
                if (user.cp >= Healing_Powder.skillCost)
                {
                    print($"{user.name} produced {Healing_Powder.skillName}");

                    int amount = SpecialHealingFormula(user);
                    amount += RandomInt(-(int)(amount * 0.1f), (int)(amount * 0.1f) );
                    target.ModifyHealth(amount);
                    user.cp -= Healing_Powder.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 3, SkillBase.TargetType.TARGET_SINGLE_ALLY, skillType: SkillBase.SkillType.HEALING);

            Super_Healing_Powder = new Skill<Actor, Actor>("Super Healing Powder", (user, target) =>
            {
                if (user.cp >= Super_Healing_Powder.skillCost)
                {
                    print($"{user.name} produced {Super_Healing_Powder.skillName}");

                    int amount = SpecialHealingFormula(user, 2);
                    amount += RandomInt(-(int)(amount * 0.1f), (int)(amount * 0.1f) );
                    target.ModifyHealth(amount);
                    user.cp -= Super_Healing_Powder.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 8, SkillBase.TargetType.TARGET_SINGLE_ALLY, skillType: SkillBase.SkillType.HEALING);

            //Full heal
            Ultra_Healing_Powder = new Skill<Actor, Actor>("Ultra Healing Powder", (user, target) =>
            {
                if (user.cp >= Ultra_Healing_Powder.skillCost)
                {
                    print($"{user.name} produced {Ultra_Healing_Powder.skillName}");

                    /*int amount = SpecialHealingFormula(user, 4);
                    amount += RandomInt(-(int)(amount * 0.1f), (int)(amount * 0.1f) );
                    target.ModifyHealth(amount);*/
                    target.ModifyHealth(target.maxHp);
                    user.cp -= Ultra_Healing_Powder.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 24, SkillBase.TargetType.TARGET_SINGLE_ALLY, skillType: SkillBase.SkillType.HEALING);

            Healing_Cloud = new Skill<Actor, List<Actor>>("Healing Cloud", (user, targets) =>
            {
                if (user.cp >= Healing_Cloud.skillCost)
                {
                    print($"{user.name} produced a {Healing_Cloud.skillName}");

                    int amount = SpecialHealingFormula(user, 2);

                    foreach (var target in targets)
                    {
                        amount += RandomInt(-(int)(amount * 0.1f), (int)(amount * 0.1f));
                        target.ModifyHealth(amount);
                    }

                    user.cp -= Healing_Cloud.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 18, SkillBase.TargetType.TARGET_ALL_ALLIES, skillType: SkillBase.SkillType.HEALING);

            Ultra_Healing_Cloud = new Skill<Actor, List<Actor>>("Ultra Healing Cloud", (user, targets) =>
            {
                if (user.cp >= Ultra_Healing_Cloud.skillCost)
                {
                    print($"{user.name} produced an {Ultra_Healing_Cloud.skillName}");

                    int amount = SpecialHealingFormula(user, 4);

                    foreach (var target in targets)
                    {
                        amount += RandomInt(-(int)(amount * 0.1f), (int)(amount * 0.1f));
                        target.ModifyHealth(amount);
                    }

                    user.cp -= Ultra_Healing_Cloud.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 50, SkillBase.TargetType.TARGET_ALL_ALLIES, skillType: SkillBase.SkillType.HEALING);

            
            Curing_Powder = new Skill<Actor, Actor>("Curing Powder", (user, target) =>
            {
                var curSkill = Curing_Powder;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} produced {curSkill.skillName}");

                    target.statusEffects.RemoveAll((m) => m == null);
                    if (target.statusEffects.Count > 0)
                    {
                        for (int i = 0; i < target.statusEffects.Count; i++)
                        {
                            target.statusEffects[i].TryRemoveEffect(target, true);
                        }
                    }
                    else
                        print("It had no visible effect on " + target.name);

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 5, SkillBase.TargetType.TARGET_SINGLE_ALLY, skillType: SkillBase.SkillType.CURING);

            Curing_Cloud = new Skill<Actor, List<Actor>>("Curing Cloud", (user, targets) =>
            {
                var curSkill = Curing_Cloud;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} produced a {curSkill.skillName}");
                    foreach (var target in targets)
                    {
                        target.statusEffects.RemoveAll((m) => m == null);
                        if (target.statusEffects.Count > 0)
                        {
                            for (int i = 0; i < target.statusEffects.Count; i++)
                            {
                                target.statusEffects[i].TryRemoveEffect(target, true);
                            }
                        }
                        else
                            print("It had no visible effect on " + target.name);
                    }

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 22, SkillBase.TargetType.TARGET_ALL_ALLIES, skillType: SkillBase.SkillType.CURING);


            Pheonix_Powder = new Skill<Actor, Actor>("Pheonix Powder", (user, target) =>
            {
                var curSkill = Pheonix_Powder;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} produced {curSkill.skillName}");

                    target.ModifyHealth((int)(target.maxHp / 2f), curSkill);
                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 20, SkillBase.TargetType.TARGET_SINGLE_ALLY, skillType: SkillBase.SkillType.REVIVAL);

            //Revive to full health 
            Ultra_Pheonix_Powder = new Skill<Actor, Actor>("Ultra Pheonix Powder", (user, target) =>
            {
                var curSkill = Ultra_Pheonix_Powder;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} produced {curSkill.skillName}");

                    target.ModifyHealth((target.maxHp), curSkill);
                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 30, SkillBase.TargetType.TARGET_SINGLE_ALLY, skillType: SkillBase.SkillType.REVIVAL);

            Pheonix_Cloud = new Skill<Actor, List<Actor>>("Pheonix Cloud", (user, targets) =>
            {
                var curSkill = Pheonix_Cloud;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} produced {curSkill.skillName}");

                    foreach (var target in targets)
                    {
                        target.ModifyHealth((int)(target.maxHp / 2f), curSkill);
                    }
                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 42, SkillBase.TargetType.TARGET_ALL_ALLIES, skillType: SkillBase.SkillType.REVIVAL);

            #endregion

        }
    }


    public class Basic
    {
        /*public const string _UNDERLINE = "\x1B[4m";
        public const string _RESET = "\x1B[0m";*/
        static Random r = new Random();

        public static T print<T>(T text, bool sameLine = false)
        {
            if (sameLine)
                Console.Write(text);
            else
                Console.WriteLine(text);

            return text;
        }
        /// <summary>
        /// Generate a random integer between and including the min and max
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int RandomInt(int min, int max)
        {
            Random r2 = new Random(r.Next(-999, 999));
            Random r3 = new Random(r.Next(-r2.Next(), 999));

            int randInt = r3.Next(0, 3);
            if (randInt == 0)
                return r.Next(min, max + 1);
            else if (randInt == 1)
                return r2.Next(min, max + 1);
            else
                return r3.Next(min, max + 1);
        }

        /// <summary>
        /// Returns true based on a "x" in "y" chance
        /// </summary>
        /// <param name="x">must be 1 or greater and less than "y"</param>
        /// <param name="y">can't be lower than "x"</param>
        /// <returns>the result of the dice roll</returns>
        public static bool Chance(int x, int y)
        {
            return RandomInt(1, y.Clamp(1, int.MaxValue)) <= x.Clamp(1, y);
        }
    }

    public static class Extentions
    {
        public static string Repeat(this string value, int count)
        {
            string final = string.Empty;
            for (int i = 0; i < count; i++)
            {
                final += value;
            }

            return final;
        }

        /// <summary>
        /// Clamps any valid *number* between the min and max *number*
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="min">Inclusive minimum</param>
        /// <param name="max">Inclusive maximum</param>
        /// <returns></returns>
        public static T Clamp<T>(this T value, T min, T max)
        {
            dynamic v = value;
            if (v.GetType() != typeof(int) && v.GetType() != typeof(float))
                return v;

            if (v > max)
                v = max;
            else if (v < min)
                v = min;

            return v;
        }

        //https://stackoverflow.com/questions/56692/random-weighted-choice
        public static T RandomElementByWeight<T>(this IEnumerable<T> sequence, Func<T, float> weightSelector)
        {
            float totalWeight = sequence.Sum(weightSelector);
            // The weight we are after...
            float itemWeightIndex = (float)new Random().NextDouble() * totalWeight;
            float currentWeightIndex = 0;

            foreach (var item in from weightedItem in sequence select new { Value = weightedItem, Weight = weightSelector(weightedItem) })
            {
                currentWeightIndex += item.Weight;

                // If we've hit or passed the weight we are after for this item then it's the one we want....
                if (currentWeightIndex >= itemWeightIndex)
                    return item.Value;

            }

            return default(T);

        }
    }
}
