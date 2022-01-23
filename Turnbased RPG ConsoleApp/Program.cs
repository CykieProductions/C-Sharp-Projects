using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Turnbased_RPG_ConsoleApp
{
    class Program : Basic
    {
        static void Main(string[] args)
        {
            string input = "";
            bool choseTurnAction = false;

            bool gameIsRunning = true;
            bool isInBattle = false;
            int totalExpFromBattle = 0;

            SkillManager.ConstructAllSkills();
            Element.ConstructAllElements();
            EnemyManager.ConstructAllEnemies();


            List<Hero> heroes = new List<Hero>();
            heroes.Add(new Hero("Cyclone", 1, Element.NONE, mhp: 28, mcp: 16, def: 1, inxp: 4, skillDict: new Dictionary<int, SkillBase>()
            {
                [2] = SkillManager.Healing_Powder,
                [4] = SkillManager.Curing_Powder,
                [10] = SkillManager.Poision_Powder,
                [12] = SkillManager.Super_Healing_Powder,
                [15] = SkillManager.Curing_Cloud,

                [18] = SkillManager.Pheonix_Powder,

                [22] = SkillManager.Healing_Cloud,
                [25] = SkillManager.Poision_Cloud,
                [30] = SkillManager.Ultra_Healing_Powder,
                [40] = SkillManager.Ultra_Healing_Cloud,
            }));
            heroes[0].skills.Add(SkillManager.Scan_Lash);

            //heroes.Add(new Hero("Shady", 1, Element.PLANT, 30, 9, 6, spd: 1));
            /*heroes.Add(new Hero("Foo", 1, Element.NONE, 12, 20, 3, spd: 0, inxp: 3, growthScalar: 3.2f, skillDict: new Dictionary<int, SkillBase>()
            {
                [3] = SkillManager.Fireball,
                [7] = SkillManager.Healing_Powder,
                [12] = SkillManager.Flare_Fall,
                [16] = SkillManager.Flame_Burst,
                [25] = SkillManager.Blazing_Vortex,
                [34] = SkillManager.Eruption,
                [42] = SkillManager.Supernova,
            }));*/
            heroes[0].exp = 5000;
            /*heroes[0].LearnSkill(SkillManager.Healing_Powder);
            heroes[0].LearnSkill(SkillManager.Super_Healing_Powder);
            heroes[0].LearnSkill(SkillManager.Ultra_Healing_Powder);
            heroes[0].LearnSkill(SkillManager.Healing_Cloud);
            heroes[0].LearnSkill(SkillManager.Ultra_Healing_Cloud);
            heroes[0].LearnSkill(SkillManager.Curing_Cloud);
            heroes[0].LearnSkill(SkillManager.Curing_Powder);
            heroes[0].LearnSkill(SkillManager.Poision_Cloud);
            heroes[0].LearnSkill(SkillManager.Ultra_Pheonix_Powder);
            heroes[0].LearnSkill(SkillManager.Pheonix_Cloud);
            heroes[0].LearnSkill(SkillManager.Pheonix_Powder);
            heroes[0].LearnSkill(SkillManager.Poision_Powder);*

            heroes[0].LearnSkill(SkillManager.Leaf_Storm);
            heroes[0].LearnSkill(SkillManager.Root_Wave);
            heroes[0].LearnSkill(SkillManager.Thorn_Canopy);
            heroes[0].LearnSkill(SkillManager.Flare_Fall);
            heroes[0].LearnSkill(SkillManager.Blazing_Vortex);
            heroes[0].LearnSkill(SkillManager.Supernova);*/

            //heroes[1].exp = 5000;
            /*heroes[1].LearnSkill(SkillManager.Damage_Allies_Test);
            heroes[1].LearnSkill(SkillManager.Poison_Allies_Test);
            heroes[1].LearnSkill(SkillManager.Ignite_Allies_Test);*/
            //heroes[1].LearnSkill(SkillManager.Restore_CP_Allies_Test);

            WorldManager.ConstructAllAreas(heroes.ToList<Actor>());

            List<Enemy> enemies = new List<Enemy>();
            /*temp
            isInBattle = true;
            enemies.Add(new Enemy("???", 1, mhp: 1));
            */

            while (gameIsRunning)
            {
                bool HeroChooseTarget(Hero hero, List<Actor> targetPool = null)
                {
                    if (targetPool == null)
                    {
                        if (hero.nextAction.targetType == SkillBase.TargetType.TARGET_SINGLE_OPPONENT || hero.nextAction.targetType == SkillBase.TargetType.TARGET_ALL_OPPONENTS)
                            targetPool = enemies.ToList<Actor>();
                        else if (hero.nextAction.targetType == SkillBase.TargetType.TARGET_SINGLE_ALLY || hero.nextAction.targetType == SkillBase.TargetType.TARGET_ALL_ALLIES)
                            targetPool = heroes.ToList<Actor>();//targetPool = activeHeroes.ToList<Actor>();
                        else if (hero.nextAction.targetType == SkillBase.TargetType.TARGET_EVERYONE)
                            targetPool = enemies.ToList<Actor>().Union(heroes.ToList<Actor>()).ToList();
                    }

                    bool isSingleTargeting = hero.nextAction.targetType != SkillBase.TargetType.TARGET_ALL_OPPONENTS && hero.nextAction.targetType != SkillBase.TargetType.TARGET_ALL_ALLIES
                        && hero.nextAction.targetType != SkillBase.TargetType.TARGET_EVERYONE;

                    if (targetPool.Count > 1 && isSingleTargeting)
                    {
                        print("Choose a target");
                        for (int e = 0; e < targetPool.Count; e++)
                        {
                            print((e + 1) + ". " + targetPool[e].name, true);

                            if (targetPool[e] is Hero)
                                print(" (HP: " + targetPool[e].hp + ")", true);

                            print("  ", true);
                        }
                        print("");
                        while (hero.targets.Count < 1 || hero.targets[0] == null)
                        {
                            input = Console.ReadLine();
                            if (int.TryParse(input, out int num))
                            {
                                if (num > 0 && targetPool.Count >= num && targetPool[num - 1] != null)
                                    hero.targets.Add(targetPool[num - 1]);
                                else
                                    return false;
                            }
                            else
                                break;
                        }
                    }
                    else if (!isSingleTargeting)
                        hero.targets = targetPool;
                    else
                        hero.targets.Add(targetPool[0]);

                    return true;
                }

                void DisplaySkillMenu(Hero hero, List<SkillBase> skills = null)
                {
                    bool choseTarget = false;
                    if (skills == null)
                        skills = hero.skills;

                    if (skills.Count > 0)
                    {
                        print(hero.name + " | HP: " + hero.hp + " | CP: " + hero.cp);
                        print("Choose a skill");

                        for (int s = 0; s < skills.Count; s++)
                        {
                            string elementText = "";
                            if (skills[s].skillCost >= 100)
                                elementText += " ";
                            else if (skills[s].skillCost >= 10)
                                elementText += "  ";
                            else
                                elementText += "   ";

                            //if (skills[s].element != Element.NONE)
                                elementText += "| Elmt: " + skills[s].element.nameFromEnum.ToString() + "  \t";

                            //tabstops occur every 8 spaces
                            //print((s + 1 + ".  " + skills[s].skillName).Length);
                            string tabs = "\t";
                            string afterDotSpaces = " ";
                            if (s + 1 < 10)
                                afterDotSpaces += " ";

                            if ((s + 1 + "." + afterDotSpaces + skills[s].skillName).Length < 24)
                                tabs += "\t";
                            if ((s + 1 + "." + afterDotSpaces + skills[s].skillName).Length < 16)
                                tabs += "\t";
                            if ((s + 1 + "." + afterDotSpaces + skills[s].skillName).Length < 8)
                                tabs += "\t";


                            print((s + 1) + ".", true);

                            //Initial space if single digit
                            if (s + 1 < 10)
                                print(" ", true);
                            if (skills[s].skillType != SkillBase.SkillType.DAMAGING && skills[s].skillType != SkillBase.SkillType.INFLICTING)//if (s % 2 == 0)
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.BackgroundColor = ConsoleColor.DarkBlue;
                            }
                            else if (skills[s].skillType == SkillBase.SkillType.DAMAGING)
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.BackgroundColor = ConsoleColor.Magenta;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.BackgroundColor = ConsoleColor.DarkMagenta;
                            }
                            print(" ", true);


                            print(skills[s].skillName + tabs + " | CP: " + skills[s].skillCost + elementText);

                            /*if (s + 1 < skills.Count)
                                print("");*/

                            Console.ForegroundColor = ConsoleColor.White;
                            Console.BackgroundColor = ConsoleColor.Black;
                        }

                        while (hero.nextAction == null)
                        {
                            input = Console.ReadLine();
                            if (int.TryParse(input, out int num))
                            {
                                if (num > 0 && skills.Count >= num && skills[num - 1] != null)
                                {
                                    hero.nextAction = skills[num - 1];
                                    choseTurnAction = true;
                                }
                                else
                                {
                                    choseTurnAction = false;
                                    break;
                                }
                            }
                            else
                                break;

                            if (choseTurnAction)
                            {
                                print(" " + hero.nextAction.skillName + " ");

                                choseTarget = false;
                                /*while (!choseTarget)//This line prevents canceling this action until next party member
                                {*/
                                choseTarget = HeroChooseTarget(hero);

                                if (choseTarget)
                                    choseTurnAction = true;
                                else
                                    choseTurnAction = false;
                                //}
                            }

                            if (!isInBattle && hero.targets.Count > 0)
                            {
                                if (hero.hp <= 0)
                                {
                                    print(hero.name + " is unconscious");
                                    return;
                                }

                                choseTurnAction = false;

                                if (hero.targets[0].hp <= 0 && hero.nextAction.skillType != SkillBase.SkillType.REVIVAL)
                                {
                                    print(hero.targets[0].name + " wasn't there. Targeting " + hero.name + " instead");
                                    hero.targets[0] = hero;
                                }

                                if (hero.targets.Count == 1)
                                    (hero.nextAction as Skill<Actor, Actor>).Use(hero, hero.targets[0]);
                                else if (hero.targets.Count > 1)
                                    (hero.nextAction as Skill<Actor, List<Actor>>).Use(hero, hero.targets);
                            }
                        }
                        print("");
                    }
                }

                bool CheckIfDefeated(Actor target)
                {
                    bool result = false;
                    if (target.hp <= 0)
                    {
                        target.nextAction = null;
                        //target.targets.Clear();//causes issues if you down yourself

                        target.Defeat();//Heroes aren't considered fully defeated until here
                        if (enemies.Contains(target))
                        {
                            totalExpFromBattle += (target as Enemy).expYield;
                            enemies.Remove(target as Enemy);
                        }

                        result = true;
                    }

                    return result;
                }

                #region OVERWORLD LOGIC
                while (!isInBattle && gameIsRunning)//Overworld Logic
                {
                    /*Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.White;*/
                    print($"Location: {WorldManager.curArea.name} | Battles left: {WorldManager.curArea.encounters.Count - WorldManager.areaProgress}");
                    print("1. NEXT BATTLE  2. CONRA  3. STATS  4. QUIT\t");
                    /*Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;*/

                    input = Console.ReadLine();
                    if (!int.TryParse(input, out int num))
                        continue;
                    print("");

                    switch (num)
                    {
                        case 1://Start Battle Logic
                            isInBattle = true;

                            //Add the enemies
                            {
                                enemies = new List<Enemy>(WorldManager.curArea.GenerateEncounter(WorldManager.areaProgress));//modifying the enemies list will no longer affect the WorldManager

                                /*var playerParty = heroes.ToList<Actor>();
                                //enemies.Add(EnemyManager.GetEnemy("Flarix", playerParty));
                                if(heroes[0].level>2)
                                    enemies.Add(EnemyManager.GetEnemy("Growfa", playerParty, 50));
                                else
                                    enemies.Add(EnemyManager.GetEnemy("Growfa", playerParty));
                                /*enemies.Add(new Enemy("Ethra", 1, Element.AIR, mhp: 10));
                                enemies.Add(new Enemy("Vinerlily", 1, Element.PLANT, mhp: 10));
                                enemies.Add(new Enemy("Terradon", 1, Element.GROUND, mhp: 10));*/
                                //enemies.Add(new Enemy("Warvy", 1, Element.WATER, mhp: 10));
                            }

                            //Alter enemy names if needed
                            {
                                List<string> names = new List<string>();
                                for (int i = 0; i < enemies.Count; i++)
                                {
                                    if (names.Contains(enemies[i].name + " (E)"))
                                        enemies[i].name += " (id: " + (i + 1) + ")";
                                    else if (names.Contains(enemies[i].name + " (D)"))
                                        enemies[i].name += " (E)";
                                    else if (names.Contains(enemies[i].name + " (C)"))
                                        enemies[i].name += " (D)";
                                    else if (names.Contains(enemies[i].name + " (B)"))
                                        enemies[i].name += " (C)";
                                    else if (names.Contains(enemies[i].name))
                                        enemies[i].name += " (B)";

                                    names.Add(enemies[i].name);
                                }
                                for (int i = 0; i < enemies.Count; i++)
                                {
                                    if (names.Contains(enemies[i].name + " (B)"))
                                        enemies[i].name += " (A)";
                                }
                            }
                            
                            print("You encountered " + enemies[0].name, true);
                            if (enemies.Count > 1)
                                print(" and its cohorts", true);
                            print("");
                            break;

                        case 2://skill menu
                            print("Which party member?");
                            for (int i = 0; i < heroes.Count; i++)
                            {
                                print((i + 1) + ". " + heroes[i].name + "  ", true);
                            }
                            print("");

                            input = Console.ReadLine();
                            if (!int.TryParse(input, out num) || (int.TryParse(input, out num) && (num == 0 || num > heroes.Count)))
                            {
                                print("Back\n");
                                continue;
                            }

                            print("");
                            Hero hero = heroes[num - 1];
                            List<SkillBase> overworldSkills = new List<SkillBase>();
                            for (int i = 0; i < hero.skills.Count; i++)
                            {
                                //print(hero.skills[i].skillType.ToString());
                                if (hero.skills[i].skillType != SkillBase.SkillType.DAMAGING && hero.skills[i].skillType != SkillBase.SkillType.INFLICTING)
                                    overworldSkills.Add(hero.skills[i]);
                            }
                            hero.targets.Clear();
                            hero.nextAction = null;
                            overworldSkills = hero.GetSortSkills(overworldSkills);
                            DisplaySkillMenu(hero, overworldSkills);
                            print("");

                            break;

                        case 3:
                            print("Which party member?");
                            for (int i = 0; i < heroes.Count; i++)
                            {
                                print( (i + 1) + ". " + heroes[i].name + "  ", true);
                            }
                            print("");

                            input = Console.ReadLine();
                            if (!int.TryParse(input, out num) || (int.TryParse(input, out num) && (num == 0 || num > heroes.Count)))
                            {
                                print("Back\n");
                                continue;
                            }

                            print("");
                            heroes[num - 1].DisplayStatus();
                            print("");

                            break;

                        case 4:
                            gameIsRunning = false;
                            break;
                        default:
                            continue;
                            //break;
                    }
                }
                #endregion

                #region BATTLE LOGIC
                while (isInBattle)//Battle Logic
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;

                    List<Hero> activeHeroes = new List<Hero>();
                    for (int h = 0; h < heroes.Count; h++)
                    {
                        if (heroes[h].hp > 0)
                        {
                            heroes[h].isDefeated = false;
                            activeHeroes.Add(heroes[h]);
                            heroes[h].skills = heroes[h].GetSortSkills(); ;
                        }
                        else if (!heroes[h].isDefeated)
                        {
                            if(CheckIfDefeated(heroes[h]))
                            {
                                heroes[h].targets.Clear();
                            }
                        }
                    }

                    #region HERO CHOICE LOGIC
                    for (int i = 0; i < heroes.Count; i++)//Heroes are choosing
                    {
                        while (!choseTurnAction && isInBattle)
                        {
                            if (heroes[i].isDefeated)
                                break;

                            bool choseTarget = false;
                            var hero = heroes[i];

                            print("What will " + hero.name + " do?");
                            print("HP: " + hero.hp + " | CP: " + hero.cp);

                            if (hero == activeHeroes[0])
                                print("1. ATTACK  2. CONRA  f. FLEE");
                            else
                                print("1. ATTACK  2. CONRA");

                            input = Console.ReadLine();

                            if (int.TryParse(input, out int num))
                            {
                                hero.targets.Clear();
                                hero.nextAction = null;

                                switch (num)
                                {
                                    case 0://go back
                                        hero.nextAction = null;
                                        i = (i - 1).Clamp(0, heroes.Count - 1);
                                        heroes[i].nextAction = null;//Prevents attacking after failed run attempt
                                        print("Back");
                                        continue;

                                    case 1://basic attack
                                        hero.nextAction = SkillManager.Attack;

                                        print(" " + hero.nextAction.skillName + " ");

                                        choseTarget = false;
                                        /*while (!choseTarget)//This line prevents canceling this action until next party member
                                        {*/
                                            choseTarget = HeroChooseTarget(hero);
                                            if (choseTarget)
                                                choseTurnAction = true;
                                        //}

                                        break;

                                    case 2://skill menu
                                        DisplaySkillMenu(hero);
                                        break;

                                    default://skip turn
                                        hero.nextAction = SkillManager.Skip_Turn;

                                        print(" " + hero.nextAction.skillName + " ");

                                        choseTurnAction = true;
                                        break;
                                }
                                //set choseAction = true; for each case
                            }
                            else if (input.ToLower() == "f" && hero == activeHeroes[0])
                            {
                                print("You attempted to run");
                                Thread.Sleep(TimeSpan.FromSeconds(.25));
                                print(".", true);
                                Thread.Sleep(TimeSpan.FromSeconds(.25));
                                print(".", true);
                                Thread.Sleep(TimeSpan.FromSeconds(.25));
                                print(".", true);
                                Thread.Sleep(TimeSpan.FromSeconds(.75));
                                if (RandomInt(0, 2) == 0)
                                {
                                    print(" But your path was blocked");
                                    i = heroes.Count - 1;
                                    choseTurnAction = true;
                                }
                                else
                                {
                                    print(" And suceeded!");
                                    i = heroes.Count - 1;
                                    isInBattle = false;
                                    choseTurnAction = true;
                                    WorldManager.FinishedBattle(true);
                                }
                            }

                            if (choseTurnAction)
                            {

                            }
                        }
                        choseTurnAction = false;
                    }
                    print("");//Space
                    #endregion

                    for (int i = 0; i < enemies.Count; i++)//Enemies are choosing
                    {
                        //enemies[i].nextAction = SkillManager.Attack;
                        enemies[i].nextAction = enemies[i].decideTurnAction.Invoke();
                        enemies[i].ChooseTarget(heroes.ToList<Actor>(), enemies[i].nextAction.targetType);
                        //enemies[i].Attack(heroes[RandomInt(0, heroes.Count - 1)]);
                        Thread.Sleep(TimeSpan.FromSeconds(0.01));
                    }

                    Actor[] allActors = new Actor[heroes.Count + enemies.Count];
                    #region DETERMINE THE TURN ORDER
                    for (int ei = 0; ei < allActors.Length; ei++)
                    {
                        int fastestSpeed = 0;

                        for (int i = 0; i < heroes.Count; i++)//compare the hero speeds
                        {
                            if (allActors.Contains(heroes[i]))
                                continue;

                            if (allActors[ei] == null || heroes[i].speed > fastestSpeed)
                            {
                                fastestSpeed = heroes[i].speed;
                                allActors[ei] = heroes[i];
                            }
                            else if (heroes[i].speed == fastestSpeed)//Decide if they win the speed tie by a coin flip
                            {
                                var r = RandomInt(0, 1);
                                //print("Coin Flip: " + r);
                                if (r == 1)
                                    allActors[ei] = heroes[i];
                            }
                        }

                        for (int i = 0; i < enemies.Count; i++)//compare the enemy speeds
                        {
                            if (allActors.Contains(enemies[i]))
                                continue;

                            if (enemies[i].speed > fastestSpeed)
                            {
                                fastestSpeed = enemies[i].speed;
                                allActors[ei] = enemies[i];
                            }
                            else if (enemies[i].speed == fastestSpeed)//Decide if they win the speed tie by a coin flip
                            {
                                var r = RandomInt(0, 1);
                                //print("Coin Flip: " + r);
                                if (r == 1)
                                    allActors[ei] = enemies[i];
                            }
                        }

                        //print(allActors[ei].name + " has a speed of " + allActors[ei].speed);
                    }
                    #endregion

                    #region PLAY TURN
                    for (int i = 0; i < allActors.Length; i++)
                    {
                        if (!isInBattle)
                            break;
                        var curActor = allActors[i];
                        if (curActor.hp <= 0)//skip actors that were defeated before their turn
                            continue;

                        curActor.statusEffects.RemoveAll((m) => m == null);
                        //Status effect actions
                        for (int se = 0; se < curActor.statusEffects.Count; se++)
                        {
                            curActor.statusEffects[se].PerformAction(curActor);
                        }
                        //
                        if (CheckIfDefeated(curActor))
                        {
                            curActor.targets.Clear();//Skip turn action won't say anything for downed actors
                        }

                        if (curActor.nextAction == null)
                            curActor.nextAction = SkillManager.Skip_Turn;

                        if (curActor.nextAction.targetType != SkillBase.TargetType.NO_TARGET)
                        {
                            List<Actor> targets = curActor.targets;
                            //print(targets[0].name + "'s HP: " + targets[0].hp);
                            if (targets.Count == 1 && (curActor.nextAction.targetType == SkillBase.TargetType.TARGET_SINGLE_OPPONENT ||
                                curActor.nextAction.targetType == SkillBase.TargetType.TARGET_SINGLE_ALLY))
                            {
                                if (targets[0].hp <= 0 && curActor.nextAction.skillType != SkillBase.SkillType.REVIVAL)
                                {
                                    if (targets[0] is Enemy)//target the next enemy over
                                    {
                                        print(targets[0].name + " isn't there. Targeting ", true);
                                        targets[0] = enemies[(enemies.IndexOf(targets[0] as Enemy) + 1).Clamp(0, enemies.Count - 1)];//defeated enemies not included
                                        print(targets[0].name + " instead");
                                    }
                                    else if ((curActor is Hero) && heroes.Contains(targets[0]))//if a hero was the target, then target self
                                        targets[0] = curActor;
                                    else if (targets[0] is Hero)//target the next hero over
                                    {
                                        print(targets[0].name + " isn't there. Targeting ", true);
                                        foreach (var hero in heroes)
                                        {
                                            if (hero.hp > 0)
                                            {
                                                targets[0] = hero;
                                                break;
                                            }
                                        }
                                        print(targets[0].name + " instead");
                                    }
                                }
                                
                                (curActor.nextAction as Skill<Actor, Actor>).Use(curActor, targets[0]);
                            }
                            else if (targets.Count != 0)
                            {
                                var moddedTargetList = targets;
                                for (int t = 0; t < targets.Count; t++)
                                {
                                    if ((targets[t].hp <= 0 && curActor.nextAction.skillType != SkillBase.SkillType.REVIVAL) || (targets[t] is Enemy && !enemies.Contains(targets[t]) ) )
                                        moddedTargetList.Remove(targets[t]);
                                }
                                (curActor.nextAction as Skill<Actor, List<Actor>>).Use(curActor, moddedTargetList);
                            }
                            else
                            {
                                if (curActor.nextAction.targetType == SkillBase.TargetType.TARGET_EVERYONE)
                                {
                                    List<Actor> everyone = heroes.ToList<Actor>().Concat(enemies.ToList<Actor>()).ToList();
                                    everyone.RemoveAll((a) => a.hp <= 0);
                                    
                                    (curActor.nextAction as Skill<Actor, List<Actor>>).Use(curActor, everyone);
                                }
                                else if (curActor.nextAction.targetType == SkillBase.TargetType.TARGET_ANYONE)
                                {
                                    //print("targeting anyone now");
                                    List<Actor> everyone = heroes.ToList<Actor>().Concat(enemies.ToList<Actor>()).ToList();
                                    everyone.RemoveAll((a) => a.hp <= 0);
                                    targets.Add(everyone[RandomInt(0, everyone.Count - 1)]);
                                    (curActor.nextAction as Skill<Actor, Actor>).Use(curActor, curActor.targets[0]);
                                }
                                else if (curActor.nextAction.targetType == SkillBase.TargetType.TARGET_ALL_OPPONENTS)
                                {
                                    List<Actor> newTargets;
                                    if (curActor is Enemy)
                                        newTargets = heroes.ToList<Actor>();
                                    else
                                        newTargets = enemies.ToList<Actor>();

                                    newTargets.RemoveAll((a) => a.hp <= 0);
                                    targets = newTargets;
                                    (curActor.nextAction as Skill<Actor, List<Actor>>).Use(curActor, targets);
                                }
                                else if (curActor.nextAction.targetType == SkillBase.TargetType.TARGET_ALL_ALLIES)
                                {
                                    List<Actor> newTargets;
                                    if (curActor is Enemy)
                                        newTargets = enemies.ToList<Actor>();
                                    else
                                        newTargets = heroes.ToList<Actor>();

                                    newTargets.RemoveAll((a) => a.hp <= 0);
                                    targets = newTargets;
                                    (curActor.nextAction as Skill<Actor, List<Actor>>).Use(curActor, targets);
                                }
                                else
                                {
                                    curActor.nextAction = SkillManager.Skip_Turn;
                                    (curActor.nextAction as Skill<Actor>).Use(curActor);
                                }
                            }

                            foreach (var target in targets)
                            {
                                if (target == null || ((target as Hero) != null && (target as Hero).isDefeated))//target was defeated too soon //shouldn't need this line
                                    continue;

                                CheckIfDefeated(target);
                            }

                        }
                        else
                        {
                            (curActor.nextAction as Skill<Actor>).Use(curActor);
                        }

                        //Make sure actions don't carry over into next turn
                        curActor.nextAction = null;
                        curActor.targets.Clear();
                        curActor.statusEffects.RemoveAll((m) => m == null);


                        //Status effect duration check
                        if (curActor.hp > 0)
                        {
                            for (int se = 0; se < curActor.statusEffects.Count; se++)
                            {
                                curActor.statusEffects[se].TryRemoveEffect(curActor);
                            }
                        }
                        //

                        //Check if player won
                        if (enemies.Count == 0)//Win
                        {
                            activeHeroes = new List<Hero>();
                            for (int h = 0; h < heroes.Count; h++)
                            {
                                if (heroes[h].hp > 0)
                                {
                                    heroes[h].exp += totalExpFromBattle;
                                    activeHeroes.Add(heroes[h]);

                                    //Check status effect duration
                                    for (int se = 0; se < heroes[h].statusEffects.Count; se++)
                                    {
                                        if (heroes[h].statusEffects[se] == null)
                                            continue;
                                        heroes[h].statusEffects[se].turnsActive++;//manual increment at the end of battle
                                        heroes[h].statusEffects[se].TryRemoveEffect(heroes[h]);
                                    }
                                    heroes[h].statusEffects.RemoveAll((m) => m == null);
                                }
                            }


                            Console.ForegroundColor = ConsoleColor.Green;
                            print("YOU WIN!");
                            Console.ForegroundColor = ConsoleColor.White;

                            if (activeHeroes.Count == 1)
                                print(activeHeroes[0].name + " gained " + totalExpFromBattle + " EXP");
                            else
                                print("The party gained " + totalExpFromBattle + " EXP");

                            Thread.Sleep(TimeSpan.FromSeconds(0.25));

                            for (int h = 0; h < heroes.Count; h++)
                            {
                                heroes[h].targets.Clear();
                                heroes[h].nextAction = null;

                                while (heroes[h].exp >= heroes[h].neededExp)
                                {
                                    heroes[h].LevelUp();
                                    Thread.Sleep(TimeSpan.FromSeconds(0.01));
                                }
                                Thread.Sleep(TimeSpan.FromSeconds(0.5));
                            }

                            isInBattle = false;
                            totalExpFromBattle = 0;
                            WorldManager.FinishedBattle();
                        }
                        //Check if player lost
                        else
                        {
                            foreach (var hero in heroes)
                            {
                                if (hero.hp > 0)//if a hero is still alive, then start the next turn
                                    break;
                                else if (!hero.isDefeated)
                                    hero.Defeat();

                                hero.targets.Clear();
                                hero.nextAction = null;

                                if (hero == heroes[heroes.Count - 1])//Only say "you lost" once
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    print("YOU LOST");
                                    Console.ForegroundColor = ConsoleColor.White;
                                    isInBattle = false;
                                    gameIsRunning = false;
                                    totalExpFromBattle = 0;
                                }
                            }
                        }
                        Thread.Sleep(TimeSpan.FromSeconds(0.01));//For randomization
                    }
                    print("");
                    #endregion
                }
                #endregion
            }
            Console.ReadKey();
        }

    }
}
