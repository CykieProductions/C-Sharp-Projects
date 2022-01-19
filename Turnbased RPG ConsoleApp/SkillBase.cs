using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turnbased_RPG_ConsoleApp
{
    public class SkillBase
    {
        private string _skillName;
        //private string _skillDescription;
        private string _skillInfo;

        public string skillName { get { return _skillName; } }
        //public string skillDescription { get { return _skillDescription; } }
        public string skillInfo { get { return _skillInfo; } }

        public int skillCost = 0;

        public enum SkillType
        {
            COMBAT, HEALING, REVIVAL
        }
        public enum TargetType
        {
            NO_TARGET, TARGET_SINGLE_OPPONENT, TARGET_ALL_OPPONENTS, TARGET_SINGLE_ALLY, TARGET_ALL_ALLIES, TARGET_EVERYONE
        }

        public SkillType skillType = SkillType.COMBAT;
        public TargetType targetType;
        public Element.Type element = Element.NONE;


        public SkillBase(string name, int cost = 0, TargetType targetType = TargetType.NO_TARGET, Element.Type elmt = null, string info = "")
        {
            _skillName = name;
            //_skillDescription = d;
            _skillInfo = info;
            this.targetType = targetType;

            skillCost = cost;

            if (elmt == null)
                elmt = Element.NONE;
            element = elmt;

            skillType = SkillType.COMBAT;
        }
    }

    /*public class Skill : SkillBase //Skill with no targets
    {
        public Action action;

        public Skill(string n, string d, Action action, string info = "") : base(n, d, info)
        {
            this.action = action;
        }

        public void Use()
        {
            action.Invoke();
        }
    }*/

    public class Skill<T1> : SkillBase //Skill with 1 arguement
    {
        public Action<T1> action;

        public Skill(string name,  Action<T1> action, int cost = 0, TargetType targetType = TargetType.NO_TARGET
            , Element.Type elmt = null, string info = "") : base(name, cost, targetType, elmt, info)
        {
            this.action = action;
        }

        public void Use(T1 t1)
        {
            action.Invoke(t1);
        }
    }

    public class Skill<T1, T2> : SkillBase //Skill with 2 arguements
    {
        public Action<T1, T2> action;

        public Skill(string name, Action<T1, T2> action, int cost = 0, TargetType targetType = TargetType.TARGET_SINGLE_OPPONENT
            , Element.Type elmt = null, SkillType skillType = SkillType.COMBAT, string info = "") : base(name, cost, targetType, elmt, info)
        {
            this.action = action;
            this.skillType = skillType;
        }

        public void Use(T1 t1, T2 t2)
        {
            action.Invoke(t1, t2);
        }
    }
}
