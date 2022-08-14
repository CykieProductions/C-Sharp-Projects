using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using static Turnbased_RPG_ConsoleApp.Basic;

namespace Turnbased_RPG_ConsoleApp
{
    public static class WorldManager
    {
        static List<Area> areaList = new List<Area>();
        public static List<Area> visitedAreas = new List<Area>();
        //public static List<int> visitedAreaIndexes = new List<int>();
        public static Area curArea;
        public static int areaProgress;
        static int curAreaIndex;

        #region Travel Stuff
        public static void FinishedBattle(List<Hero> heroes, bool fledFromEncounter = false)
        {
            var prevEncounter = GetAreaByName(curArea.name).encounters[areaProgress];

            if ((prevEncounter.enemies == null || prevEncounter.enemies.Count == 0) && prevEncounter.enemyStats != null)//fill possible enmies with new enemies from saved stats
            {
                prevEncounter.enemies = new List<Enemy>();
                for (int e = 0; e < prevEncounter.enemyStats.Count; e++)
                {
                    prevEncounter.enemies.Add(new Enemy(prevEncounter.enemyStats[e]));
                }
            }

            for (int i = 0; i < prevEncounter.enemies.Count; i++)//for saving
            {
                prevEncounter.enemies[i].hp = prevEncounter.enemies[i].maxHp;
                prevEncounter.enemies[i].cp = prevEncounter.enemies[i].maxCp;
            }

            curArea.encounters[areaProgress] = prevEncounter;

            if (fledFromEncounter == false)
            {
                prevEncounter.hasBeenBeaten = true;
                curArea.encounters[areaProgress] = prevEncounter;

                areaProgress++;
            }
            else
            {
                var change = RandomInt(-1, 1);

                if (prevEncounter.isFixedEncounter)//must go back a battle if you run from a fixed encounter
                    change = -1;
                if (areaProgress + change < 0)//don't send out of the area
                    change = 0;

                curArea.encounters[areaProgress] = prevEncounter;//this data is put into visitedAreas on Save
                areaProgress += change;
                if (change < 0)
                    print("You ran the wrong direction and lost progress");
                else if (change == 0)
                    print("You were able to circle back to the spot you ran from");
                else
                    print("You managed to make progress dispite the rush");

            }

            //Custom event
            if (curArea.events != null && curArea.events.Exists(x => x.progressIndex == areaProgress && x.alreadyDone == false))
            {
                var e = curArea.events.Find(x => x.progressIndex == areaProgress);
                e.action.Invoke();
                e.alreadyDone = true;
                curArea.events[curArea.events.IndexOf(curArea.events.Find(x => x.progressIndex == areaProgress))] = e;
                areaList[areaList.IndexOf(GetAreaByName(curArea.name))] = curArea;//Set the area list version of this area to match curArea*v
            }

            if (areaProgress >= curArea.encounters.Count)//Reached the end of the area
            {
                var next = areaList[(areaList.IndexOf(GetAreaByName(curArea.name)/*unmodified area*/) + 1).Clamp(0, areaList.Count - 1)];
                visitedAreas[visitedAreas.IndexOf(visitedAreas.Find(x => x.name == curArea.name))] = curArea;//match the versions
                areaList[areaList.IndexOf(areaList.Find(x => x.name == curArea.name))] = curArea;//match the versions
                Travel(next.name, heroes);//No longer resets the area event save data*^
            }
            else if (curArea.checkpoints != null && curArea.checkpoints.ToList().Exists(x => x.index == areaProgress && x.isActive == false))
            {
                RestoreAtCheckpoint(heroes, true);
                curArea.checkpoints[curArea.checkpoints.ToList().IndexOf(curArea.checkpoints.ToList().Find(x => x.index == areaProgress))].isActive = true;
            }
            else if (curArea.checkpoints != null && curArea.checkpoints.ToList().Exists(x => x.index == areaProgress))
            {
                RestoreAtCheckpoint(heroes);
            }
        }

        static void RestoreAtCheckpoint(List<Hero> heroes, bool firstTime = false)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            if (firstTime)
                print("You were able to set up a base camp");
            else
                print("You arrived at the base camp");

            print("HP and CP fully restored!");
            for (int i = 0; i < heroes.Count; i++)
            {
                heroes[i].hp = heroes[i].maxHp;
                heroes[i].cp = heroes[i].maxCp;
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Travel(string locationName, List<Hero> heroes, int checkpointNum = -1)
        {
            Area location = GetAreaByName(locationName);
            if (string.IsNullOrEmpty(location.name))
                return;

            curAreaIndex = areaList.IndexOf(location);
            if (!visitedAreas.Exists(x => x.name == location.name))//contains a fresh version of the current location
            {
                print("\nYou made it to " + location.name);
                RestoreAtCheckpoint(heroes, true);
                //visitedAreaIndexes.Add(curAreaIndex);
                visitedAreas.Add(areaList[curAreaIndex]);
            }
            else
            {
                print("\nYou warped to " + location.name);
                if (checkpointNum < 0)//is at the beginning
                    RestoreAtCheckpoint(heroes, false);
            }
            curArea = location;

            if (checkpointNum < 0)//go to the beginning
            {
                areaProgress = 0;
            }
            else
            {
                areaProgress = curArea.checkpoints[checkpointNum].index;
                RestoreAtCheckpoint(heroes, curArea.checkpoints[checkpointNum].isActive);
            }
        }
        #endregion

        public static void ConstructAllAreas(List<Actor> heroes)
        {
            areaList.Clear();

            areaList.Add(new Area("Sorecord Forest", new List<Enemy>()
            {
               EnemyManager.GetEnemy("Growfa", heroes),
               EnemyManager.GetEnemy("Skoka", heroes)
            }, eCount: 10, new List<int>() { 75, 25}, new List<EnemyManager.Encounter>()
            { //fixed encounters
                new EnemyManager.Encounter(0, 1, cr: true, enemiesToAdd: EnemyManager.GetEnemy("Growfa", heroes, lv: 2)),
                new EnemyManager.Encounter(9, 3, true, false, EnemyManager.GetEnemy("Elder Skoka", heroes), EnemyManager.GetEnemy("Growfa", heroes, lv: 3), EnemyManager.GetEnemy("Growfa", heroes, lv: 3))
            }));

            areaList.Add(new Area("Sunstroke Plateau", new List<Enemy>()
            {
               EnemyManager.GetEnemy("Growfa", heroes, lv: 6, eRate: 20, sRange: new int[2]{ 1, 8 }),
               EnemyManager.GetEnemy("Skoka", heroes, lv: 4, eRate: 10),
               EnemyManager.GetEnemy("Flarix", heroes, lv: 3, eRate: 50),
               EnemyManager.GetEnemy("Plugry", heroes, lv: 3, eRate: 30),
               EnemyManager.GetEnemy("Elder Skoka", heroes, sRange: new int[2]{ 9, 999 }),
               EnemyManager.GetEnemy("Growfa", heroes, lv: 12, sRange: new int[2]{ 9, 999 })
            }, eCount: 18, nIEW: new List<int>() { 50, 40, 10}, new List<EnemyManager.Encounter>()
            { //fixed encounters
                new EnemyManager.Encounter(0, 1, cr: true, enemiesToAdd: EnemyManager.GetEnemy("Skoka", heroes, lv: 3)),
                new EnemyManager.Encounter(17, 1, cr: false, enemiesToAdd: EnemyManager.GetEnemy("Leviac", heroes))
            }, chkpnts: new (int index, string name, bool isActive)[] { (12, "Flooded Cave Entrance", false) }));

            areaList.Add(new Area("Furstone Trail", new List<Enemy>()
            {
                EnemyManager.GetEnemy("Growfa", heroes, lv: 1, eRate: 20, new int[2]{ 0, 4 }),
                EnemyManager.GetEnemy("Growfa", heroes, lv: 4, eRate: 20, new int[2]{ 3, 999 }),
                EnemyManager.GetEnemy("Flarix", heroes, lv: 1, eRate: 30, new int[2]{ 1, 4 }),
                EnemyManager.GetEnemy("Flarix", heroes, lv: 2, eRate: 30, new int[2]{ 3, 999 }),
                EnemyManager.GetEnemy("Plugry", heroes, lv: 4, eRate: 10, new int[2]{ 5, 999 })
            }, 8, new List<int>() { 75, 25 }, chkpnts: new (int index, string name, bool isActive)[] { (6, "Furstone Village", false) }, evts: new List<(int, Action, bool)>
            {
                ( 6, () => 
                {
                    print("\nWhile walking through a kindly village, you met a timid, yet determined young man who's goals aligned with your own.");
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    var foo = new Hero(Program.GetHeroByName("Foo")); Program.curHeroes.Add(foo); print($"{foo.name} joined the party!"); 
                    Console.ForegroundColor = ConsoleColor.White;
                }, false)
            }));

            curArea = areaList[curAreaIndex];
            visitedAreas.Add(curArea);
        }

        public static Area GetAreaByName(string name)
        {
            return areaList.Find(a => a.name == name);
        }

        public class Area
        {
            public string name;

            [Newtonsoft.Json.JsonIgnore] public List<Enemy> possibleEnemies;
            public List<Enemy.Stats> possibleEnemyStats;

            //public int encounterCount;
            /// <summary> The number of enemies in an encounter will be (index + 1), and will be choosen by the integer at that index out of 100 </summary>
            public List<int> numInEncouterWeights;
            public List<EnemyManager.Encounter> encounters;
            public (int index, string name, bool isActive)[] checkpoints;

            [Newtonsoft.Json.JsonIgnore]
            public List<(int progressIndex, Action action, bool alreadyDone)> events;
            public bool[] eventsTriggeredState;

            /// <summary>
            /// Construct a new Area
            /// </summary>
            /// <param name="n">the name of the area</param>
            /// <param name="enemies">possible enemies in an area</param>
            /// <param name="eCount">the number of encounters</param>
            /// <param name="nIEW">the number of enemies in an encounter will be (index + 1), and will be choosen by the integer at that index out of 100</param>
            public Area(string n, List<Enemy> enemies, int eCount, List<int>nIEW, List<EnemyManager.Encounter> fixedEncounters = null
                , (int index, string name, bool isActive)[] chkpnts = null, List<(int, Action, bool)> evts = null)
            {
                if (n == null)
                {
                    //Json Constructor executes before loading so this code didn't work //moved to GenerateEncounter
                    /*if ((possibleEnemies == null || possibleEnemies.Count == 0) && possibleEnemyStats != null)
                    {
                        possibleEnemies = new List<Enemy>();
                        for (int i = 0; i < possibleEnemyStats.Count; i++)
                        {
                            possibleEnemies.Add(new Enemy(possibleEnemyStats[i]));
                        }
                    }*/
                    return;
                }

                name = n;
                possibleEnemies = enemies;

                possibleEnemyStats = new List<Enemy.Stats>();
                for (int i = 0; i < enemies.Count; i++)
                {
                    possibleEnemyStats.Add(enemies[i].GetStats());
                }

                //encounterCount = eCount;
                numInEncouterWeights = nIEW;

                encounters = new List<EnemyManager.Encounter>();
                for (int i = 0; i < eCount; i++)
                {
                    if (fixedEncounters == null || fixedEncounters.Find(x => x.index == i).enemies == null)
                        encounters.Add(new EnemyManager.Encounter(i, new List<Enemy>()));//fill spot with a randomly generated encounter
                    else
                        encounters.Add(fixedEncounters.Find(x => x.index == i));//fill spot with proper fixed encounter
                }

                checkpoints = chkpnts;
                events = evts;
            }

            public List<Enemy> GenerateEncounter(int i)
            {
                if ((possibleEnemies == null || possibleEnemies.Count == 0)/* && possibleEnemyStats != null*/)
                {
                    possibleEnemies = GetAreaByName(name).possibleEnemies;//fill possible enemies with new enemies from saved stats
                    /*for (int ind = 0; ind < possibleEnemies.Count; ind++)
                    {
                        possibleEnemies[ind] = new Enemy(possibleEnemies[ind]);
                    }Unneeded*/
                    /*if (possibleEnemyStats.Count < GetAreaByName(name).possibleEnemies.Count)
                    {
                        //This should probably be the default way, but I don't want chance breaking something
                        possibleEnemies = GetAreaByName(name).possibleEnemies;
                    }
                    else
                    {
                        possibleEnemies = new List<Enemy>();
                        for (int e = 0; e < possibleEnemyStats.Count; e++)
                        {
                            possibleEnemies.Add(new Enemy(possibleEnemyStats[e]));
                        }
                    }*/
                }

                var encounter = encounters[i];
                if (!(encounter.canRepeat == false && encounter.hasBeenBeaten == true))
                {
                    encounter = new EnemyManager.Encounter(GetAreaByName(name).encounters[i]);//should make the if statement below obsolete
                }

                //Turn the enemy stats of a fixed encounter, into real enemies //Shouldn't be needed anymore, but I don't want to chance something breaking
                if ((encounter.enemies == null || encounter.enemies.Count == 0) && encounter.enemyStats != null && encounter.enemyStats.Count != 0)
                {
                    if (encounter.isFixedEncounter)
                        encounter = new EnemyManager.Encounter(encounter);
                }

                //Don't repeat one time encounter
                if ((encounter.isFixedEncounter && encounter.hasBeenBeaten && !encounter.canRepeat))
                {
                    encounter.enemies = null;
                    encounter.numOfEnemies = -1;
                }

                if (encounter.numOfEnemies <= 0)
                    encounter.numOfEnemies = numInEncouterWeights.IndexOf(numInEncouterWeights.RandomElementByWeight(x => x)) + 1;

                if (encounter.enemies == null || encounter.enemies.Count == 0)
                {
                    encounter.enemies = new List<Enemy>();
                    for (int e = 0; e < encounter.numOfEnemies; e++)
                    {
                        //Min Spawn Range should be <= progress and Max should be >= progress
                        List<Enemy> narrowedList = possibleEnemies.Where(x => x.spawnRange[0] <= areaProgress && x.spawnRange[1] >= areaProgress).ToList();
                        encounter.enemies.Add(new Enemy (narrowedList.RandomElementByWeight(x=>x.encounterWeight)) );
                        Thread.Sleep(TimeSpan.FromSeconds(0.01));//For randomization
                    }
                }

                //encounters[i] = encounter;
                return encounter.enemies;
            }


        }
    }
}
