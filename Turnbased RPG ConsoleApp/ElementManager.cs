using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turnbased_RPG_ConsoleApp
{
    public static class Element
    {
        static Type omni = new Type();
        static Type none = new Type();

        static Type fire = new Type();
        static Type water = new Type();
        static Type plant = new Type();

        static Type ground = new Type();
        static Type air = new Type();
        static Type electric = new Type();

        public static Type IGNORE_ALL { get { return omni; } }
        public static Type NONE { get { return none; } }
        public static Type FIRE { get { return fire; } }
        public static Type WATER { get { return water; } }
        public static Type PLANT { get { return plant; } }
        public static Type GROUND { get { return ground; } }
        public static Type AIR { get { return air; } }
        public static Type ELECTRIC { get { return electric; } }


        public static Type GetElementByName(string name)
        {
            var field = typeof(Element).GetField(name.ToLower(), BindingFlags.NonPublic | BindingFlags.Static);

            if (field.GetValue(null) is Type)
            {
                //Basic.print((field.GetValue(null) as Type).nameFromEnum.ToString());
                return field.GetValue(null) as Type;
            }
            return null;
        }

        public static float CheckAttackAgainst(Type attackType, Actor target)
        {
            float mult = 1;

            if (attackType.effectiveAgainst.Contains(target.element))
                mult = 1.75f;
            else if (attackType.ineffectiveAgainst.Contains(target.element))
                mult = 0.5f;
            else if (attackType.uselessAgainst.Contains(target.element))
                mult = 0;

            return mult;
        }

        public static void ConstructAllElements()
        {
            omni.nameFromEnum = Type.Name.OMNI;

            #region NONE
            none.nameFromEnum = Type.Name.NONE;
            none.ineffectiveAgainst.Add(ground);
            none.uselessAgainst.Add(air);
            //Neutral damage from everything
            #endregion

            #region FIRE
            fire.nameFromEnum = Type.Name.FIRE;
            fire.effectiveAgainst.Add(plant);
            fire.ineffectiveAgainst.Add(water);
            fire.ineffectiveAgainst.Add(ground);
            //fire.ineffectiveAgainst.Add(fire);
            fire.uselessAgainst.Add(fire);
            //Neutral against air and electric
            #endregion

            #region WATER
            water.nameFromEnum = Type.Name.WATER;
            water.effectiveAgainst.Add(fire);
            water.effectiveAgainst.Add(ground);
            water.ineffectiveAgainst.Add(plant);
            water.ineffectiveAgainst.Add(water);
            //water.uselessAgainst.Add();
            //Neutral against air and electric
            #endregion

            #region PLANT
            plant.nameFromEnum = Type.Name.PLANT;
            plant.effectiveAgainst.Add(water);
            plant.effectiveAgainst.Add(ground);
            plant.ineffectiveAgainst.Add(fire);
            //plant.uselessAgainst.Add();
            //Neutral against air, electric, and self
            #endregion

            #region GROUND
            ground.nameFromEnum = Type.Name.GROUND;
            ground.effectiveAgainst.Add(fire);
            ground.effectiveAgainst.Add(electric);
            ground.ineffectiveAgainst.Add(water);
            ground.uselessAgainst.Add(air);
            //Neutral against plant and self
            #endregion

            #region AIR
            air.nameFromEnum = Type.Name.AIR;
            air.effectiveAgainst.Add(fire);
            air.ineffectiveAgainst.Add(electric);
            //air.uselessAgainst.Add();
            //Neutral against water, plant, ground, and self
            #endregion

            #region ELECTRIC
            electric.nameFromEnum = Type.Name.ELECTRIC;
            electric.effectiveAgainst.Add(air);
            electric.effectiveAgainst.Add(water);
            electric.ineffectiveAgainst.Add(plant);
            electric.ineffectiveAgainst.Add(fire);
            electric.uselessAgainst.Add(ground);
            //Neutral against self
            #endregion
        }

        public class Type
        {
            public enum Name
            {
                OMNI, NONE, FIRE, WATER, PLANT, GROUND, AIR, ELECTRIC
            }
            public Name nameFromEnum = Name.NONE;


            public List<Type> effectiveAgainst = new List<Type>();
            public List<Type> ineffectiveAgainst = new List<Type>();
            public List<Type> uselessAgainst = new List<Type>();

            /*public Type type { get { return _type; } }
            public List<Type> strengths { get { return _strengths; } }
            public List<Type> weaknesses { get { return _weaknesses; } }*/

            /*public Element(Type type, List<Type> strengths, List<Type> weaknesses)
            {
                _type = type;
                effectiveAgainst = strengths;
                ineffectiveAgainst = weaknesses;
            }*/
        }
    }
}
