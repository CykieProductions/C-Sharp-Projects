﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turnbased_RPG_ConsoleApp
{
    public abstract class Actor : Basic
    {
        public string name = "???";
        
        public Element.Type element = Element.NONE;
        float elementalHealingMult = 0.25f;

        public List<StatusEffect.Type> statusEffects = new List<StatusEffect.Type>(3);
        public int statusEffectCapacity = 3;

        public int level = 1, maxHp, hp, maxCp, cp, attack = 1, defense = 0, specialAttack = 1, speed = 1;
        protected int maxLevel = 100;

        public SkillBase nextAction;
        public List<Actor> targets = new List<Actor>();

        public List<SkillBase> skills = new List<SkillBase>();

        public enum Ailments
        {
            NONE, POISONED, ON_FIRE, SLEEPING,
        }

        public List<Ailments> curStatusEffects = new List<Ailments>(3);

        public Actor (string n, int lv = 1, Element.Type elmt = null, int mhp = 10, int mcp = 5, int atk = 1, int def = 0, int pow = 1, int spd = 1)
        {
            if (elmt == null)
                elmt = Element.NONE;
            element = elmt;

            name = n;
            level = lv.Clamp(1, maxLevel);
            maxHp = mhp;
            maxCp = mcp;
            attack = atk;
            defense = def;
            specialAttack = pow;
            speed = spd;

            hp = maxHp;
            cp = maxCp;
        }

        public virtual void Attack(Actor target)
        {
            int damage = attack + (int)(level * 0.5f);
            damage += RandomInt(-2, 2);
            target.ModifyHealth(-damage);
        }

        public virtual void ModifyHealth(int value, Element.Type attackElement = null)
        {
            if (attackElement == null)
                attackElement = Element.NONE;

            if (value > 0)
            {
                if (attackElement == element)
                    value += (int)(value * elementalHealingMult);

                Console.ForegroundColor = ConsoleColor.Green;
                if (hp + value < maxHp)
                    print(name + " gained " + value + " HP");
                else
                    print(name + "'s HP is maxed out");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                float mult = Element.CheckAttackAgainst(attackElement, this);
                int newValue = (int)(value * mult);
                if (mult != 0)
                    newValue.Clamp(1, int.MaxValue);
                value = newValue;

                //print("Damage has been multiplied by " + mult);

                Console.ForegroundColor = ConsoleColor.Yellow;
                print(name + " took " + -value + " damage");
                Console.ForegroundColor = ConsoleColor.White;
            }

            hp = (hp + value).Clamp(0, maxHp);
        }

        public virtual void ModifyConra(int value, Element.Type attackElement = null)
        {
            if (attackElement == null)
                attackElement = Element.NONE;

            if (value > 0)
            {
                if (attackElement == element)
                    value += (int)(value * elementalHealingMult);

                Console.ForegroundColor = ConsoleColor.Cyan;
                if (cp + value < maxCp)
                    print(name + " gained " + value + " CP");
                else
                    print(name + "'s CP is maxed out");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                float mult = Element.CheckAttackAgainst(attackElement, this);
                value = (int)(value * mult);

                //print("CP lose has been multiplied by " + mult);

                Console.ForegroundColor = ConsoleColor.Blue;
                print(name + " lost " + -value + " CP");
                Console.ForegroundColor = ConsoleColor.White;
            }

            hp = (hp + value).Clamp(0, maxCp);
        }

        public virtual void Defeat()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            print(name + " was defeated!\n");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

    public class Enemy : Actor
    {
        public int expYield;
        public int baseExp = 4;

        public Enemy(string n, int lv = 1, Element.Type elmt = null,  int mhp = 10, int mcp = 5, int atk = 3, int def = 0, int pow = 1, int spd = 1) : base(n, lv, elmt, mhp, mcp, atk, def, pow, spd)
        {
            expYield = (int)( baseExp * (lv * 0.25f).Clamp(1, 5) );

            skills.Add(GameManager.Attack);
        }

        public void ChooseTarget(List<Actor> actorPool, SkillBase.TargetType targetType = SkillBase.TargetType.TARGET_SINGLE_OPPONENT)
        {
            targets.Clear();
            for (int i = 0; i < actorPool.Count; i++)//Remove defeated actors from potential targets list
            {
                if (actorPool[i].hp <= 0)
                    actorPool.RemoveAt(i);
            }

            if (targetType == SkillBase.TargetType.TARGET_SINGLE_OPPONENT || targetType == SkillBase.TargetType.TARGET_SINGLE_ALLY)
            {
                targets.Add(actorPool[ RandomInt(0, actorPool.Count - 1) ]);
            }
            else if (targetType != SkillBase.TargetType.NO_TARGET)
                targets = actorPool;
        }
    }

    public class Hero : Actor
    {
        int initExp;
        float expGrowthScaler;

        public int exp;
        public int neededExp;
        public bool isDefeated;

        public Dictionary<int, SkillBase> skillDictionary = null;

        public Hero(string n, int lv = 1, Element.Type elmt = null, int mhp = 10, int mcp = 5, int atk = 5, int def = 0, int pow = 1, int spd = 2, int inxp = 8, float growthScalar = 1.75f
            , Dictionary<int, SkillBase> skillDict = null) : base(n, lv, elmt, mhp, mcp, atk, def, pow, spd) 
        {
            initExp = inxp;
            expGrowthScaler = growthScalar;

            if (level < maxLevel)
                neededExp = (int)(initExp * (level - 1) * (expGrowthScaler * (int)(level * 0.1f)).Clamp(1, 5) + initExp);
            else
                neededExp = 999999;

            skillDictionary = skillDict;
        }

        public override void Defeat()
        {
            isDefeated = true;
            Console.ForegroundColor = ConsoleColor.Red;
            print(name + " blacked out!\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void LearnSkill(SkillBase skill)
        {
            if (skills.Contains(skill))
                return;

            Console.ForegroundColor = ConsoleColor.Magenta;
            skills.Add(skill);
            print(name + " learned " + skill.skillName + "!");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void LevelUp()
        {
            exp = (exp - neededExp).Clamp(0, 999999);
            if (level >= maxLevel)
            {
                neededExp = 999999;
                return;
            }
            else
            {
                neededExp = (int)(initExp * (level - 1) * (expGrowthScaler * (int)(level * 0.1f)).Clamp(1, 5) + initExp);
            }

            level++;
            Console.ForegroundColor = ConsoleColor.Green;
            //print("** " + name + " has leveled up! **");
            print("** " + name + " reached LEVEL " + level + "! **");
            Console.ForegroundColor = ConsoleColor.Cyan;

            int randInt = RandomInt(2, 5);
            maxHp += randInt;
            hp += randInt;
            print("HP increased by " + randInt);

            randInt = RandomInt(1, 3);
            maxCp += randInt;
            cp = maxCp;
            print("CP increased by " + randInt);

            randInt = RandomInt(1, 2);
            attack += randInt;
            print("ATTACK increased by " + randInt);

            randInt = RandomInt(0, 2);
            specialAttack += randInt;
            if (randInt > 0)
                print("SPECIAL ATTACK increased by " + randInt);

            if (skillDictionary != null)
            {
                for (int i = 0; i < skillDictionary.Keys.Count; i++)
                {
                    var curKey = skillDictionary.Keys.ToArray()[i];
                    //print("Key #" + i + ": " + curKey + " = " + skillDictionary[curKey].skillName);
                    if (level >= curKey)
                    {
                        if (skillDictionary[curKey] != null)
                        {
                            LearnSkill(skillDictionary[curKey]);
                        }

                    }
                }
            }

            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.ForegroundColor = ConsoleColor.White;
        }

        public void DisplayStatus()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            print(name);
            print("LV: " + level);
            //print("Element: " + element.nameFromEnum.ToString());
            //print("");

            if (hp <= 0)
                Console.ForegroundColor = ConsoleColor.Red;
            else if (hp < (int)(maxHp * .15f).Clamp(1, int.MaxValue))
                Console.ForegroundColor = ConsoleColor.Yellow;
            print("HP: " + hp + "/" + maxHp);
            Console.ForegroundColor = ConsoleColor.Cyan;

            if (cp <= (int)(maxCp * .15f))
                Console.ForegroundColor = ConsoleColor.Yellow;
            print("CP: " + cp + "/" + maxCp);
            Console.ForegroundColor = ConsoleColor.Cyan;

            print("Attack: " + attack);
            print("Special Attack: " + specialAttack);
            print("Defense: " + defense);
            print("Speed: " + speed);
            print("EXP: " + exp + "/" + neededExp);
            //print("EXP To Next Level: " + neededExp);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
