using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using static Turnbased_RPG_ConsoleApp.Basic;

namespace Turnbased_RPG_ConsoleApp
{
    public static class StatusEffect
    {
        //static Type none = new Type("NONE");

        static Type poisoned = new Type("POISONED", 5, 8, (target) =>
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            print(target.name + " felt the effects of the poison"); ;
            Console.ForegroundColor = ConsoleColor.White;

            var baseDamage = (target.maxHp / 10f);
            target.ModifyHealth(-(int)(baseDamage + RandomInt(-(int)(baseDamage / 5).Clamp(2, 10), (int)(baseDamage / 5).Clamp(2, 10))).Clamp(1, float.MaxValue), atkElmt: Element.IGNORE_ALL);

        }, "{0} got poisoned", "{0} recovered from the poison");

        static Type flaming = new Type("FLAMING", 1, 4, (target) =>
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            print(target.name + " got burned by the flames"); ;
            Console.ForegroundColor = ConsoleColor.White;

            var baseDamage = (target.maxHp / 3f).Clamp(1, 40);
            target.ModifyHealth(-(int)(baseDamage + RandomInt(-(int)(baseDamage / 5).Clamp(2, 10), (int)(baseDamage / 5).Clamp(2, 10))), atkElmt: Element.FIRE);
        }, "{0} got set on fire", "{0} is no longer on fire");

        static Type sleeping = new Type("SLEEPING", 2, 4, (target) =>
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            print(target.name + " is asleep"); ;
            Console.ForegroundColor = ConsoleColor.White;

            target.nextAction = SkillManager.Sleep;
            target.targets.Clear();
        }, "{0} got put in a deep sleep", "{0} woke up");

        static Type paralyzed = new Type("PARALYZED", 7, 12, (target) =>
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            print(target.name + " is fighting the paralysis"); ;
            Console.ForegroundColor = ConsoleColor.White;

            if (Chance(1, 2))
            {
                target.nextAction = SkillManager.Immobile;
                target.targets.Clear();
            }
        }, "{0} got paralyzed", "{0} is free of the paralysis");

        static Type confused = new Type("CONFUSED", 2, 5, (target) =>
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            print(target.name + " is confused");
            Console.ForegroundColor = ConsoleColor.White;

            if (Chance(1, 3))//ignore command and do a normal attack on anyone
            {
                target.nextAction = new Skill<Actor, Actor>(SkillManager.Attack);
                target.nextAction.targetType = SkillBase.TargetType.TARGET_ANYONE;
                target.targets.Clear();
            }
            else if (target.nextAction.targetType != SkillBase.TargetType.TARGET_EVERYONE && Chance(1, 4))//use intended move on the wrong target(s)
            {
                if (target.targets.Count == 1 && (target.nextAction.targetType == SkillBase.TargetType.TARGET_SINGLE_OPPONENT
                    || target.nextAction.targetType == SkillBase.TargetType.TARGET_SINGLE_ALLY))//Hit anyone
                {
                    target.nextAction = new Skill<Actor, Actor>(target.nextAction as Skill<Actor, Actor>);
                    target.nextAction.targetType = SkillBase.TargetType.TARGET_ANYONE;
                }
                else//Target the opposite group from the intended targets
                {
                    target.nextAction = new Skill<Actor, List<Actor>>(target.nextAction as Skill<Actor, List<Actor>>);
                    if (target.nextAction.targetType == SkillBase.TargetType.TARGET_ALL_ALLIES)
                        target.nextAction.targetType = SkillBase.TargetType.TARGET_ALL_OPPONENTS;
                    else if (target.nextAction.targetType == SkillBase.TargetType.TARGET_ALL_OPPONENTS)
                        target.nextAction.targetType = SkillBase.TargetType.TARGET_ALL_ALLIES;
                }
                target.targets.Clear();
            }

        },"{0} got confused", "{0} came to their senses");//change name to delusional?


        //public static Type NONE { get { return none; } }
        public static Type POISONED { get { return poisoned; } }
        public static Type FLAMING { get { return flaming; } }
        public static Type SLEEPING { get { return sleeping; } }
        public static Type PARALYZED { get { return paralyzed; } }
        public static Type CONFUSED { get { return confused; } }

        [Serializable]
        public class Type : Basic
        {
            public string name;

            Action<Actor> turnAction;
            Action<Actor> recoverAction;
            int minPossibleTurns = 1;
            int maxPossibleTurns = 8;

            int turnLimit;
            public int turnsActive = 0;

            string inflictText = "{0} isn't feeling well";
            string removeText = "{0} went back to normal";

            public Type(string n, int minPT, int maxPT, Action<Actor> tAction, string iText = "{0} isn't feeling well", string rtext = "{0} went back to normal", Action<Actor> recAction = null)
            {
                name = n;
                turnAction = tAction;

                if (maxPT < 0)
                    maxPT = int.MaxValue;

                minPossibleTurns = minPT;
                maxPossibleTurns = maxPT;

                inflictText = iText;
                removeText = rtext;

                recoverAction = recAction;
            }
            public Type(Type dupe)
            {
                name = dupe.name;
                turnAction = dupe.turnAction;

                minPossibleTurns = dupe.minPossibleTurns;
                maxPossibleTurns = dupe.maxPossibleTurns;

                removeText = dupe.removeText;
                recoverAction = dupe.recoverAction;
            }


            public bool TryInflict(Actor target, int duration = -1, bool showInflictText = true, bool performImmediately = false)
            {
                Thread.Sleep(TimeSpan.FromSeconds(0.01));//For randomizing
                target.statusEffects.RemoveAll((m) => m == null);
                //print(target.name + " has " + target.statusEffects.Count + " status effects. Can add more: " + (target.statusEffects.Count < target.statusEffectCapacity));

                bool alreadyInflicted = target.statusEffects.Exists((n) => n.name == name);
                bool successful = target.statusEffects.Count < target.statusEffectCapacity && !alreadyInflicted;//if list small enough and not already inflicted

                if (successful)
                {
                    var dupe = new Type(this);
                    target.statusEffects.Add(dupe);
                    if (duration < 0)
                        dupe.turnLimit = RandomInt(minPossibleTurns, maxPossibleTurns);
                    else
                        dupe.turnLimit = duration;

                    if (showInflictText)
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        print(string.Format(inflictText, target.name));
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    if(performImmediately)
                    {
                        PerformAction(target);
                        turnsActive--;
                    }

                }
                else if (alreadyInflicted)
                    print(target.name + " is already " + name.ToLower());

                return successful;
            }

            public void PerformAction(Actor target)
            {
                turnAction.Invoke(target);

                turnsActive++;
                //Check for duration at the end of turn
            }
            public void TryRemoveEffect(Actor target, bool forceRemove = false, bool activateRecoverAction = true)
            {
                if (turnsActive >= turnLimit || forceRemove)
                {
                    target.statusEffects[target.statusEffects.IndexOf(this)] = null;

                    Console.ForegroundColor = ConsoleColor.Blue;
                    print(string.Format(removeText, target.name));
                    Console.ForegroundColor = ConsoleColor.White;

                    if (activateRecoverAction && recoverAction != null)
                        recoverAction.Invoke(target);
                }
            }

        }
    }
}
