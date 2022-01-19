using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turnbased_RPG_ConsoleApp
{
    class GameManager : Basic
    {
        public static Skill<Actor> Skip_Turn;
        public static Skill<Actor> Sleep;
        public static Skill<Actor> Waste_Stare;
        public static Skill<Actor> Waste_Short_Circut;


        public static Skill<Actor, Actor> Poision_Powder;
        public static Skill<Actor, List<Actor>> Poision_Cloud;

        public static Skill<Actor, Actor> Attack;
        public static Skill<Actor, List<Actor>> Damage_Allies_Test;

        public static Skill<Actor, Actor> Fireball;
        public static Skill<Actor, Actor> Flame_Burst;
        public static Skill<Actor, Actor> Eruption;
        public static Skill<Actor, List<Actor>> Flare_Fall;
        public static Skill<Actor, List<Actor>> Blazing_Vortex;
        public static Skill<Actor, List<Actor>> Supernova;

        public static Skill<Actor, Actor> Healing_Powder;
        public static Skill<Actor, Actor> Super_Healing_Powder;
        public static Skill<Actor, Actor> Ultra_Healing_Powder;
        public static Skill<Actor, List<Actor>> Healing_Cloud;
        public static Skill<Actor, List<Actor>> Ultra_Healing_Cloud;

        public static Skill<Actor, Actor> Cure_Powder;
        public static Skill<Actor, List<Actor>> Cure_Cloud;
        public static Skill<Actor, Actor> Pheonix_Powder;
        public static Skill<Actor, List<Actor>> Pheonix_Cloud;


        public static void ConstructAllSkills()
        {
            int SpecialDamageFormula(Actor user, int rank = 1, int baseValue = 32, float scalar = 0.36f)
            {
                float rankDamper = 0.75f;
                if (rank >= 3)
                    rankDamper = 0.64f;

                //print((rank * rankDamper).Clamp(1, 50));
                return (int)(baseValue * Math.Sqrt(Math.Pow(user.specialAttack, (rank * rankDamper).Clamp(1, 50) )) * scalar);
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

            Waste_Stare = new Skill<Actor>("Stare", (user) =>
            {
                print($"{user.name} stared you down");
            });

            Waste_Short_Circut = new Skill<Actor>("Do Nothing", (user) =>
            {
                print($"{user.name} short circuited");
            });
            #endregion

            //For testing purposes
            Damage_Allies_Test = new Skill<Actor, List<Actor>>("Damage Allies", (user, targets) =>
            {
                if (user.cp >= Damage_Allies_Test.skillCost)
                {
                    print($"{user.name} used {Damage_Allies_Test.skillName}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);
                    int amount = SpecialDamageFormula(user, 1);

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
            //

            #region STATUS EFFECT GIVING
            Poision_Powder = new Skill<Actor, Actor>("Poison Powder", (user, target) =>
            {
                var curSkill = Poision_Powder;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} produced {curSkill.skillName}");
                    
                    int amount = RandomInt(1, 3);
                    target.ModifyHealth(-amount, curSkill.element);

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
            , 5, SkillBase.TargetType.TARGET_SINGLE_OPPONENT, Element.PLANT);

            Poision_Cloud = new Skill<Actor, List<Actor>>("Poison Cloud", (user, targets) =>
            {
                var curSkill = Poision_Cloud;
                if (user.cp >= curSkill.skillCost)
                {
                    print($"{user.name} produced {curSkill.skillName}");

                    foreach (var target in targets)
                    {
                        int amount = RandomInt(1, 3);
                        target.ModifyHealth(-amount, curSkill.element);

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
            , 14, SkillBase.TargetType.TARGET_ALL_OPPONENTS, Element.PLANT);
            #endregion

            #region ATTACKS
            Attack = new Skill<Actor, Actor>("Attack", (user, target) =>
            {
                print($"{user.name} attacked {target.name}");
                user.Attack(target);
            }
            , 0, SkillBase.TargetType.TARGET_SINGLE_OPPONENT);


            Fireball = new Skill<Actor, Actor>("Fireball", (user, target) =>
            {
                if (user.cp >= Fireball.skillCost)
                {
                    print($"{user.name} shot out a fireball");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);
                    int amount = SpecialDamageFormula(user);

                    amount += RandomInt(-(int)(amount * 0.1f), (int)(amount * 0.1f));
                    target.ModifyHealth(-amount, Fireball.element);

                    user.cp -= Fireball.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 4, SkillBase.TargetType.TARGET_SINGLE_OPPONENT, Element.FIRE);

            Flame_Burst = new Skill<Actor, Actor>("Flame Burst", (user, target) =>
            {
                if (user.cp >= Flame_Burst.skillCost)
                {
                    print($"{user.name} used {Flame_Burst.skillName}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);
                    int amount = SpecialDamageFormula(user, 2);

                    amount += RandomInt(-(int)(amount * 0.1f), (int)(amount * 0.1f));
                    target.ModifyHealth(-amount, Flame_Burst.element);

                    user.cp -= Flame_Burst.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 8, SkillBase.TargetType.TARGET_SINGLE_OPPONENT, Element.FIRE);

            Eruption = new Skill<Actor, Actor>("Eruption", (user, target) =>
            {
                if (user.cp >= Eruption.skillCost)
                {
                    print($"{user.name} used {Eruption.skillName}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);
                    int amount = SpecialDamageFormula(user, 3);

                    amount += RandomInt(-(int)(amount * 0.1f), (int)(amount * 0.1f));
                    target.ModifyHealth(-amount, Eruption.element);

                    user.cp -= Eruption.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 24, SkillBase.TargetType.TARGET_SINGLE_OPPONENT, Element.FIRE);

            Flare_Fall = new Skill<Actor, List<Actor>>("Flare Fall", (user, targets) =>
            {
                if (user.cp >= Flare_Fall.skillCost)
                {
                    print($"{user.name} used {Flare_Fall.skillName}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);
                    int amount = SpecialDamageFormula(user, 1);

                    foreach (var target in targets)
                    {
                        amount += RandomInt(-(int)(amount * 0.1f), (int)(amount * 0.1f));
                        target.ModifyHealth(-amount, Flare_Fall.element);
                    }

                    user.cp -= Flare_Fall.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 8, SkillBase.TargetType.TARGET_ALL_OPPONENTS, Element.FIRE);

            Blazing_Vortex = new Skill<Actor, List<Actor>>("Blazing Vortex", (user, targets) =>
            {
                if (user.cp >= Blazing_Vortex.skillCost)
                {
                    print($"{user.name} used {Blazing_Vortex.skillName}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);
                    int amount = SpecialDamageFormula(user, 2);

                    foreach (var target in targets)
                    {
                        amount += RandomInt(-(int)(amount * 0.1f), (int)(amount * 0.1f));
                        target.ModifyHealth(-amount, Blazing_Vortex.element);
                    }

                    user.cp -= Blazing_Vortex.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 16, SkillBase.TargetType.TARGET_ALL_OPPONENTS, Element.FIRE);

            Supernova = new Skill<Actor, List<Actor>>("Supernova", (user, targets) =>
            {
                if (user.cp >= Supernova.skillCost)
                {
                    print($"{user.name} used {Supernova.skillName}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);
                    int amount = SpecialDamageFormula(user, 3);

                    foreach (var target in targets)
                    {
                        amount += RandomInt(-(int)(amount * 0.1f), (int)(amount * 0.1f));
                        target.ModifyHealth(-amount, Supernova.element);
                    }

                    user.cp -= Supernova.skillCost;
                }
                else
                    OutOfCP(user.name);
            }
            , 50, SkillBase.TargetType.TARGET_ALL_OPPONENTS, Element.FIRE);
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
