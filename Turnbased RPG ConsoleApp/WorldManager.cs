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
        static List<int> visitedAreaIndexes = new List<int>();
        public static Area curArea;
        public static int areaProgress;
        static int curAreaIndex;//for saving

        public static void FinishedBattle(bool fledFromEncounter = false)
        {
            var prevEncounter = curArea.encounters[areaProgress];

            for (int i = 0; i < prevEncounter.enemies.Count; i++)
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

                curArea.encounters[areaProgress] = prevEncounter;
                areaProgress += change;
                if (change < 0)
                    print("You ran the wrong direction and lost progress");
                else if (change == 0)
                    print("You were able to circle back to the spot you ran from");
                else
                    print("You managed to make progress dispite the rush");

            }

            if (areaProgress >= curArea.encounters.Count)
                Travel(areaList[(areaList.IndexOf(curArea) + 1).Clamp(0, areaList.Count - 1)].name);
        }

        public static void Travel(string locationName, int checkpointNum = 0)
        {
            Area location = areaList.Find(a => a.name == locationName);
            if (string.IsNullOrEmpty(location.name))
                return;

            curAreaIndex = areaList.IndexOf(location);
            if (!visitedAreaIndexes.Contains(curAreaIndex))
            {
                print("You made it to " + location.name);
                visitedAreaIndexes.Add(curAreaIndex);
            }
            areaProgress = 0;
            curArea = location;
        }

        public static void ConstructAllAreas(List<Actor> heroes)
        {
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
               EnemyManager.GetEnemy("Growfa", heroes, 6, eRate: 20),
               EnemyManager.GetEnemy("Skoka", heroes, 4, eRate: 10),
               EnemyManager.GetEnemy("Flarix", heroes, eRate: 50),
               EnemyManager.GetEnemy("Plugry", heroes, eRate: 30)
            }, eCount: 10, nIEW: new List<int>() { 50, 40, 10}, new List<EnemyManager.Encounter>()
            { //fixed encounters
                new EnemyManager.Encounter(0, 1, cr: true, enemiesToAdd: EnemyManager.GetEnemy("Skoka", heroes, lv: 3)),
                new EnemyManager.Encounter(9, 3, true, false, EnemyManager.GetEnemy("Elder Skoka", heroes), EnemyManager.GetEnemy("Growfa", heroes, lv: 3), EnemyManager.GetEnemy("Growfa", heroes, lv: 3))
            }));


            curArea = areaList[curAreaIndex];
            visitedAreaIndexes.Add(curAreaIndex);
        }

        public class Area
        {
            public string name;
            public List<Enemy> possibleEnemies;
            //public int encounterCount;
            /// <summary>
            /// The number of enemies in an encounter will be (index + 1), and will be choosen by the integer at that index out of 100
            /// </summary>
            public List<int> numInEncouterWeights;

            public List<EnemyManager.Encounter> encounters;
            //public List<int> checkpointIndexes;


            /// <summary>
            /// Construct a new Area
            /// </summary>
            /// <param name="n">the name of the area</param>
            /// <param name="enemies">possible enemies in an area</param>
            /// <param name="eCount">the number of encounters</param>
            /// <param name="nIEW">the number of enemies in an encounter will be (index + 1), and will be choosen by the integer at that index out of 100</param>
            public Area(string n, List<Enemy> enemies, int eCount, List<int>nIEW, List<EnemyManager.Encounter> fixedEncounters)
            {
                name = n;
                possibleEnemies = enemies;
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
            }

            public List<Enemy> GenerateEncounter(int i)
            {
                var encounter = encounters[i];
                if (encounter.isFixedEncounter && encounter.hasBeenBeaten && !encounter.canRepeat)
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
                        encounter.enemies.Add(new Enemy (possibleEnemies.RandomElementByWeight(x=>x.encounterWeight)) );
                        Thread.Sleep(TimeSpan.FromSeconds(0.01));//For randomization
                    }
                }

                //encounters[i] = encounter;
                return encounter.enemies;
            }


        }
    }
}
