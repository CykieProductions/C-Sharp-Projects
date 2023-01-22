using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Turnbased_RPG_ConsoleApp.Basic;

namespace Turnbased_RPG_ConsoleApp
{
    public static class EnemyManager
    {
        static List<Enemy> enemyList = new List<Enemy>();

        public struct Encounter
        {
            public int index;

            [Newtonsoft.Json.JsonIgnore] public List<Enemy> enemies;
            public List<Enemy.Stats> enemyStats;
            /*[Newtonsoft.Json.JsonProperty]
            List<string> enemyNames;*/
            public int numOfEnemies;

            public bool isFixedEncounter;
            public bool canRepeat;
            public bool hasBeenBeaten;

            public Encounter(int idx, int numOfE = -1, bool ife = true, bool cr = false, params Enemy[] enemiesToAdd)
            {
                index = idx;

                enemies = enemiesToAdd.ToList();
                enemyStats = new List<Enemy.Stats>();
                for (int i = 0; i < enemies.Count; i++)
                {
                    enemyStats.Add(enemies[i].GetStats());
                }


                /*enemyNames = new List<string>();
                foreach (var e in enemies)
                {
                    enemyNames.Add(e.name);
                }*/

                numOfE = enemies.Count;
                numOfEnemies = enemies.Count;
                isFixedEncounter = ife;
                canRepeat = cr;
                hasBeenBeaten = false;

            }
            public Encounter(int idx, List<Enemy> enemiesToAdd, int numOfE = -1, bool ife = false, bool cr = false)
            {
                index = idx;
                enemies = enemiesToAdd;
                enemyStats = new List<Enemy.Stats>();
                for (int i = 0; i < enemies.Count; i++)
                {
                    enemyStats.Add(enemies[i].GetStats());
                }

                /*enemyNames = new List<string>();
                foreach (var e in enemies)
                {
                    enemyNames.Add(e.name);
                }*/

                if (numOfE <= 0)
                    numOfE = enemies.Count;
                numOfEnemies = numOfE;
                isFixedEncounter = ife;
                canRepeat = cr;
                hasBeenBeaten = false;
            }
            //Newtonsoft Json JsonConstructor
            public Encounter(Encounter encounter)
            {
                //Json Constructor executes before loading so this code initially failed //Constructor now called in Area.GenerateEncounter
                index = encounter.index;

                enemies = encounter.enemies;
                enemyStats = encounter.enemyStats;

                if ((enemies == null || enemies.Count == 0) && enemyStats != null)
                {
                    enemies = new List<Enemy>();
                    for (int i = 0; i < enemyStats.Count; i++)
                    {
                        enemies.Add(new Enemy(enemyStats[i]));
                    }
                }
                else if (enemies != null)
                {
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        enemies[i] = new Enemy(enemies[i]);//get a fresh version of the enemy
                    }
                }

                if (enemies != null)
                    numOfEnemies = enemies.Count;
                else
                    numOfEnemies = 0;
                isFixedEncounter = encounter.isFixedEncounter;
                canRepeat = encounter.canRepeat;
                hasBeenBeaten = encounter.hasBeenBeaten;

            }
        }

        public static Enemy GetEnemy(string name, List<Actor> heroes, int lv = -1, int eRate = 50, int[] sRange = null)
        {
            var request = enemyList.Find((x) => x.name == name);
            if (request == null)
                return null;

            if (lv < 0)
                lv = request.level;

            var enemy = new Enemy(request);
            enemy.level = lv.Clamp(1, int.MaxValue);
            enemy.heroList = heroes;
            int dif = (enemy.level - request.level);//.Clamp(1, 100;
            int levelBonus = (int)Math.Round(Math.Sqrt(dif * 0.8f * (dif - 1)) );

            enemy.maxHp += levelBonus;
            enemy.hp = enemy.maxHp;
            enemy.maxCp += levelBonus;
            enemy.cp = enemy.maxCp;
            enemy.attack += levelBonus;
            enemy.specialAttack += levelBonus / 2;
            enemy.defense += (int)Math.Sqrt(levelBonus) * 5;
            //enemy.speed += (int)Math.Sqrt(levelBonus) * 2;
            enemy.expYield = (int)(enemy.baseExp + dif + (levelBonus * 0.8f));

            enemy.encounterWeight = eRate;
            if (sRange == null)
            {
                enemy.spawnRange = new int[2] { 0, int.MaxValue };
            }
            else
            {
                enemy.spawnRange = sRange;
                if (sRange[0] > sRange[1])
                    enemy.spawnRange[0] = sRange[1];
                if (sRange[1] < sRange[0])
                    enemy.spawnRange[1] = sRange[0];
            }

            //Higher levels can ge better skills
            if (enemy.skillDictionary != null)
            {
                for (int i = 0; i < enemy.skillDictionary.Keys.Count; i++)
                {
                    var curKey = enemy.skillDictionary.Keys.ToArray()[i];
                    //print("Key #" + i + ": " + curKey + " = " + skillDictionary[curKey].skillName);
                    if (enemy.level >= Math.Abs(curKey))
                    {
                        if (enemy.skillDictionary[curKey] != null)
                        {
                            if (curKey >= 0)
                                enemy.skills.Add(enemy.skillDictionary[curKey]);
                            else
                                enemy.skills.RemoveAll(x => x == enemy.skillDictionary[curKey]);//Used to remove weaker moves
                        }

                    }
                }
            }

            return enemy;
        }

        public static void ConstructAllEnemies()
        {
            enemyList.Add(new Enemy("Growfa", lv: 1, elmt: Element.PLANT, mhp: 12, mcp: 30, atk: 3, def: 0, spAtk: 3, spd: 1, exp: 3, new List<SkillBase>()
            {
                SkillManager.Vine_Whip,
                SkillManager.Vine_Whip,//Dupe for higher chance
                SkillManager.Waste_Jumping
            }));
            enemyList.Add(new Enemy("Skoka", lv: 1, elmt: Element.NONE, mhp: 16, mcp: 30, atk: 4, def: 0, spAtk: 1, spd: 3, exp: 4, new List<SkillBase>()
            {
                SkillManager.Water_Pulse,
                SkillManager.Attack,//Dupe for higher chance
                SkillManager.Waste_Stare
            }));
            enemyList.Add(new Enemy("Elder Skoka", lv: 5, elmt: Element.NONE, mhp: 40, mcp: 120, atk: 3, def: 0, spAtk: 2, spd: 1, exp: 32, new List<SkillBase>()
            {
                SkillManager.Leaf_Storm,
                SkillManager.Leaf_Storm,
                SkillManager.Water_Pulse,
                SkillManager.Water_Pulse,
                SkillManager.Water_Pulse,
                SkillManager.Attack,
                SkillManager.Waste_Stare
            }));

            enemyList.Add(new Enemy("Flarix", lv: 1, elmt: Element.FIRE, mhp: 20, mcp: 12, atk: 4, def: 0, spAtk: 5, spd: 1, exp: 8, new List<SkillBase>()
            {
                SkillManager.Fireball,
                SkillManager.Waste_Stare
            }));

            enemyList.Add(new Enemy("Plugry", lv: 1, elmt: Element.ELECTRIC, mhp: 18, mcp: 12, atk: 3, def: 0, spAtk: 8, spd: 2, exp: 10, new List<SkillBase>()
            {
                SkillManager.Charge_Bolt,
                SkillManager.Waste_Short_Circut
            }));

            var leviac = new Enemy("Leviac", lv: 10, elmt: Element.WATER, mhp: /*250*/360, mcp: 80, atk: 5, def: 1, spAtk: 9, spd: 1, exp: 78, new List<SkillBase>()
            {
                SkillManager.Geo_Shift,
                new Skill<Actor, Actor>("Bite", (user, target) => {
                    print($"{user.name} bit {target.name}");
                    target.ModifyHealth(-SkillManager.BasicAttackFormula(user, 2));
                }, elmt: Element.NONE),
                new Skill<Actor, Actor>("Bite", (user, target) => {
                    print($"{user.name} bit {target.name}");
                    target.ModifyHealth(-SkillManager.BasicAttackFormula(user, 2));
                }, elmt: Element.NONE),
                SkillManager.Water_Pulse,
                SkillManager.Water_Pulse,
                SkillManager.Poison_Cloud,
                SkillManager.Waste_Roar,
                new Skill<Actor>("Dive", (user) => {
                    print($"{user.name} dove underwater");
                    for (int i = 0; i < user.statusEffects.Count; i++)
                    {
                        user.statusEffects[i].TryRemoveEffect(user, true);
                    }

                    new StatusEffect.Type("SUBMERGED", 2, 3, (x) =>
                    {
                        for (int i = 0; i < user.statusEffects.Count; i++)
                        {
                            user.statusEffects[i].TryRemoveEffect(user, true);
                        }

                        user.element = new Element.Type(Element.WATER);//New element that acts like WATER, but dodges everything except ELECTRIC
                        user.element.unfazedBy.Add(Element.NONE);
                        //user.element.unfazedBy.Add(Element.IGNORE_ALL);
                        user.element.unfazedBy.Add(Element.FIRE);
                        user.element.unfazedBy.Add(Element.WATER);
                        user.element.unfazedBy.Add(Element.PLANT);
                        user.element.unfazedBy.Add(Element.GROUND);
                        user.element.unfazedBy.Add(Element.AIR);

                        Console.ForegroundColor = ConsoleColor.Magenta;
                        print($"{user.name} is submerged");
                        Console.ForegroundColor = ConsoleColor.White;
                        user.nextAction = new Skill<Actor>("Preparing",(y) => print($"{user.name} is preparing something"), pri: 2);

                        if (Chance(2, 3))
                            user.ModifyConra((int)(user.maxCp * 0.25f));
                    },"DONT SHOW THIS", "{0} resurfaced", (x) => 
                    { user.element = Element.WATER; user.nextAction = SkillManager.Hydo_Cannon; SkillManager.Hydo_Cannon.Use(user, Program.curHeroes[RandomInt(0, Program.curHeroes.Count - 1)]); }).TryInflict(user, showInflictText: false, performImmediately: true);

                }, elmt: Element.WATER),
                new Skill<Actor>("Dive", (user) => {
                    print($"{user.name} dove underwater");
                    for (int i = 0; i < user.statusEffects.Count; i++)
                    {
                        user.statusEffects[i].TryRemoveEffect(user, true);
                    }

                    new StatusEffect.Type("SUBMERGED", 2, 3, (x) =>
                    {
                        for (int i = 0; i < user.statusEffects.Count; i++)
                        {
                            user.statusEffects[i].TryRemoveEffect(user, true);
                        }

                        user.element = new Element.Type(Element.WATER);//New element that acts like WATER, but dodges everything except ELECTRIC
                        user.element.unfazedBy.Add(Element.NONE);
                        //user.element.unfazedBy.Add(Element.IGNORE_ALL);
                        user.element.unfazedBy.Add(Element.FIRE);
                        user.element.unfazedBy.Add(Element.WATER);
                        user.element.unfazedBy.Add(Element.PLANT);
                        user.element.unfazedBy.Add(Element.GROUND);
                        user.element.unfazedBy.Add(Element.AIR);

                        Console.ForegroundColor = ConsoleColor.Magenta;
                        print($"{user.name} is submerged");
                        Console.ForegroundColor = ConsoleColor.White;
                        user.nextAction = new Skill<Actor>("Preparing",(y) => print($"{user.name} is preparing something"), pri: 2);

                        if (Chance(2, 3))
                            user.ModifyConra((int)(user.maxCp * 0.25f));
                    },"DONT SHOW THIS", "{0} resurfaced", (x) => 
                    { user.element = Element.WATER; user.nextAction = SkillManager.Hydo_Cannon; SkillManager.Hydo_Cannon.Use(user, Program.curHeroes[RandomInt(0, Program.curHeroes.Count - 1)]); }).TryInflict(user, showInflictText: false, performImmediately: true);

                }, elmt: Element.WATER)
            });
            enemyList.Add(new Enemy(leviac, ai: () =>
            {
                var skillPool = leviac.skills.ToList();//copy of the list
                if (Program.decidingEnemy.nextAction?.skillName == SkillManager.Hydo_Cannon.skillName)
                {
                    skillPool.RemoveAll(x => x.skillName == "Dive");//don't dive twice in a row
                }
                
                //Program.curHeroes;//test
                foreach (var skill in leviac.skills)
                {
                    if (skill == SkillManager.Attack && Chance(3, 3))//Don't normal attack
                        skillPool.Remove(SkillManager.Attack);
                    else if (skill.skillCost > leviac.cp && Chance(1, 2))//Leaves a small chance to use a skill that costs too much
                        skillPool.Remove(skill);
                }

                var randInt = RandomInt(0, skillPool.Count - 1);
                //don't leave an increased chance for a basic attack on this enemy

                return skillPool[randInt];

            }));
        }
    }
}
