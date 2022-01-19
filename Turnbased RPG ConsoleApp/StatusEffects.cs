using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turnbased_RPG_ConsoleApp
{
    public static class StatusEffect
    {
        //static Type none = new Type("NONE");

        static Type poisoned = new Type("POISONED", 5, 8, (target) =>
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Basic.print(target.name + " felt the effects of the poison"); ;
            Console.ForegroundColor = ConsoleColor.White;
            
            var baseDamage = (target.maxHp / 10f);
            target.ModifyHealth( -(int)(baseDamage + Basic.RandomInt(-(int)(baseDamage / 5).Clamp(2, 10), (int)(baseDamage / 5).Clamp(2, 10) )).Clamp(1, float.MaxValue), Element.IGNORE_ALL);

        }, "{0} recovered from the poison");

        static Type flaming = new Type("FLAMING", 1, 5, (target) =>
        {
            var baseDamage = (target.maxHp / 5f);
            target.ModifyHealth( -(int)(baseDamage + Basic.RandomInt(-(int)(baseDamage / 5).Clamp(2, 10), (int)(baseDamage / 5).Clamp(2, 10) )), Element.FIRE);
        }, "{0} is no longer on fire");

        static Type sleeping = new Type("SLEEPING", 2, 4, (target) =>
        {
            target.nextAction = GameManager.Sleep;
            target.targets.Clear();
        }, "{0} woke up");

        /*static Type ground = new Type();
        static Type air = new Type();
        static Type electic = new Type();*/

        //public static Type NONE { get { return none; } }
        public static Type POISONED { get { return poisoned; } }
        public static Type FLAMING { get { return flaming; } }
        public static Type SLEEPING { get { return sleeping; } }
        /*public static Type GROUND { get { return ground; } }
        public static Type AIR { get { return air; } }
        public static Type ELECTRIC { get { return electic; } }*/

        public class Type : Basic
        {
            public string name;

            Action<Actor> turnAction;
            int minPossibleTurns = 1;
            int maxPossibleTurns = 8;

            int turnLimit;
            int turnsActive = 0;

            string removeText = "{0} went back to normal";

            public Type(string n, int minPT, int maxPT, Action<Actor> action, string rtext = "{0} went back to normal")
            {
                name = n;
                turnAction = action;

                if (maxPT < 0)
                    maxPT = int.MaxValue;

                minPossibleTurns = minPT;
                maxPossibleTurns = maxPT;

                removeText = rtext;
            }
            public Type(Type dupe)
            {
                name = dupe.name;
                turnAction = dupe.turnAction;

                minPossibleTurns = dupe.minPossibleTurns;
                maxPossibleTurns = dupe.maxPossibleTurns;

                removeText = dupe.removeText;
            }


            public bool TryInflict(Actor target)
            {
                target.statusEffects.RemoveAll((m) => m == null);
                //print(target.name + " has " + target.statusEffects.Count + " status effects. Can add more: " + (target.statusEffects.Count < target.statusEffectCapacity));

                bool alreadyInflicted = target.statusEffects.Exists((n) => n.name == name);
                bool successful = target.statusEffects.Count < target.statusEffectCapacity && !alreadyInflicted;//if list small enough and not already inflicted

                if (successful)
                {
                    var dupe = new Type(this);
                    target.statusEffects.Add(dupe);
                    dupe.turnLimit = RandomInt(minPossibleTurns, maxPossibleTurns);
                }
                else if (alreadyInflicted)
                    print(target.name + " is already " + name.ToLower());

                return successful;
            }

            public void PerformAction(Actor target)
            {
                turnAction.Invoke(target);

                turnsActive++;

                if (turnsActive >= turnLimit)
                {
                    target.statusEffects[target.statusEffects.IndexOf(this)] = null;

                    Console.ForegroundColor = ConsoleColor.Blue;
                    print(string.Format(removeText, target.name));
                    Console.ForegroundColor = ConsoleColor.White;
                }

            }

        }
    }
}
