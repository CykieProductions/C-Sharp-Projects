using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turnbased_RPG_ConsoleApp
{
    class SkillManager : Basic
    {
        public static Skill<Actor> Skip_Turn;
        public static Skill<Actor> Sleep;
        public static Skill<Actor> Immobile;
        public static Skill<Actor> Waste_Stare;
        public static Skill<Actor> Waste_Short_Circut;
        public static Skill<Actor> Waste_Jumping;


        public static Skill<Actor, Actor> Poision_Powder;
        public static Skill<Actor, List<Actor>> Poision_Cloud;

        //Testing
        public static Skill<Actor, List<Actor>> Damage_Allies_Test;
        public static Skill<Actor, List<Actor>> Poison_Allies_Test;
        public static Skill<Actor, List<Actor>> Ignite_Allies_Test;
        public static Skill<Actor, List<Actor>> Restore_CP_Allies_Test;
        //

        public static Skill<Actor, Actor> Attack;

        //Fire moves
        public static Skill<Actor, Actor> Fireball;
        public static Skill<Actor, Actor> Flame_Burst;
        public static Skill<Actor, Actor> Eruption;
        public static Skill<Actor, List<Actor>> Flare_Fall;
        public static Skill<Actor, List<Actor>> Blazing_Vortex;
        public static Skill<Actor, List<Actor>> Supernova;
        //Water moves
        public static Skill<Actor, Actor> Water_Pulse;
        public static Skill<Actor, Actor> Water_Jet;
        public static Skill<Actor, Actor> Hydo_Cannon;
        //public static Skill<Actor, Actor> Torpedo;//Physical
        public static Skill<Actor, List<Actor>> Waterfall;
        public static Skill<Actor, List<Actor>> Deluge;
        public static Skill<Actor, List<Actor>> Tsunami;
        //Plant moves
        public static Skill<Actor, Actor> Vine_Whip;
        public static Skill<Actor, Actor> Leaf_Cutter;
        public static Skill<Actor, Actor> Needle_Tomb;
        public static Skill<Actor, List<Actor>> Leaf_Storm;
        public static Skill<Actor, List<Actor>> Root_Wave;
        public static Skill<Actor, List<Actor>> Thorn_Canopy;
        //Ground moves
        public static Skill<Actor, Actor> Pebble_Blast;
        public static Skill<Actor, Actor> Geo_Shift;
        public static Skill<Actor, Actor> Fissure;
        public static Skill<Actor, List<Actor>> Rock_Slide;
        public static Skill<Actor, List<Actor>> Spire_Wall;
        public static Skill<Actor, List<Actor>> Earthquake;
        //Air moves
        public static Skill<Actor, Actor> Wind_Slash;
        public static Skill<Actor, Actor> Air_Cannon;
        public static Skill<Actor, Actor> Sonic_Boom;
        public static Skill<Actor, List<Actor>> Slash_Storm;
        public static Skill<Actor, List<Actor>> Sky_Crusher;
        public static Skill<Actor, List<Actor>> Hurricane;
        //Electric moves
        public static Skill<Actor, Actor> Charge_Bolt;
        public static Skill<Actor, Actor> Taser_Grip;
        public static Skill<Actor, Actor> Ion_Overload;
        public static Skill<Actor, List<Actor>> Electro_Wave;
        public static Skill<Actor, List<Actor>> Tesla_Cannon;
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


        public static void ConstructAllSkills()
        {
            int SpecialAttackFormula(Actor user, int rank = 1, int baseValue = 32, float scalar = 0.36f, bool varyDamage = true)
            {
                float rankDamper = 0.75f;
                if (rank >= 3)
                    rankDamper = 0.64f;

                int amount = (int)(baseValue * Math.Sqrt(Math.Pow(user.specialAttack, (rank * rankDamper).Clamp(1, 50))) * scalar);
                if (varyDamage)
                    amount += RandomInt(-(int)(amount * 0.1f), (int)(amount * 0.1f));
                return amount;
            }
            int AttackFormula(Actor user, int rank = 1, float fluxPercent = 0.1f, int fluxMin = 2)
            {
                float rankDamper = 2f;

                //print((rank * rankDamper).Clamp(1, 50));
                int damage = (int)( user.attack + (1.25f * Math.Sqrt(user.level * 0.75f).Clamp(1, 100)) * Math.Pow(rank, rankDamper).Clamp(1, 50) );
                damage += RandomInt(-(int)(damage * fluxPercent).Clamp(fluxMin, float.MaxValue), (int)(damage * fluxPercent).Clamp(fluxMin, float.MaxValue));
                return damage;
            }

            int SpecialHealingFormula(Actor user, int rank = 1, int baseValue = 30, float scalar = 1f)
            {
                float rankDamper = 1f;
                if (rank >= 3)
                    rankDamper = 1.2f;

                return (int)(baseValue * Math.Sqrt(rank * rankDamper).Clamp(1, 50) * scalar * (rank * 0.75f).Clamp(1, 50) );
            }

            void OutOfCP(string n)
            {
                print($"{n} didn't have enough CP");
            }


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

            Waste_Short_Circut = new Skill<Actor>("Short Circut", (user) =>
            {
                print($"{user.name} short circuited");
            });

            Waste_Jumping = new Skill<Actor>("Jump Around", (user) =>
            {
                print($"{user.name} is jumping around");
            });
            #endregion

            //For testing purposes
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
                            if (StatusEffect.POISONED.TryInflict(target))
                                print($"{target.name} got poisoned");
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
                            if (StatusEffect.FLAMING.TryInflict(target))
                                print($"{target.name} got set on fire");
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
            //

            #region STATUS EFFECT GIVING
            Poision_Powder = new Skill<Actor, Actor>("Poison Powder", (user, target) =>
            {
                var curSkill = Poision_Powder;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} produced {curSkill.skillName}");
                    
                    int amount = RandomInt(1, 3);
                    target.ModifyHealth(-amount, curSkill);

                    if (Chance(3, 3))
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        if (StatusEffect.POISONED.TryInflict(target))
                            print($"{target.name} got poisoned");
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 5, SkillBase.TargetType.TARGET_SINGLE_OPPONENT, Element.PLANT, SkillBase.SkillType.INFLICTING);

            Poision_Cloud = new Skill<Actor, List<Actor>>("Poison Cloud", (user, targets) =>
            {
                var curSkill = Poision_Cloud;
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
                            if (StatusEffect.POISONED.TryInflict(target))
                                print($"{target.name} got poisoned");
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
                target.ModifyHealth(-AttackFormula(user, 1));
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
                    int amount = SpecialAttackFormula(user, 3);

                    if (Chance(1, 5))
                        StatusEffect.PARALYZED.TryInflict(target);

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

                        if (Chance(1, 5))
                            StatusEffect.PARALYZED.TryInflict(target);

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

                    target.ModifyHealth(-AttackFormula(user, 2), curSkill);
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

                    int amount = (int)(SpecialAttackFormula(user, 1) * 0.5 + AttackFormula(user, 2));
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
                    int amount = (int)( SpecialAttackFormula(user, 1) * 0.75 + AttackFormula(user, 3));

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
                        int amount = (int)(SpecialAttackFormula(user, 1) * 0.1f + AttackFormula(user, 2));
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
                        int amount = (int)(SpecialAttackFormula(user, 1) * 0.5 + AttackFormula(user, 2));
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
                        int amount = (int)(SpecialAttackFormula(user, 1) * 0.75 + AttackFormula(user, 3));
                        target.ModifyHealth(-amount, curSkill);
                    }

                    user.cp -= curSkill.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 38, SkillBase.TargetType.TARGET_ALL_OPPONENTS, Element.PLANT);
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
            Random r = new Random();
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
    }
}
