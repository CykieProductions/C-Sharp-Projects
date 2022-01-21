using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turnbased_RPG_ConsoleApp
{
    class EnemyManager : Basic
    {
        static List<Enemy> enemyList = new List<Enemy>();

        public static Enemy GetEnemy(string name, List<Actor> heroes, int lv = 1)
        {
            var enemy = enemyList.Find((x) => x.name == name);
            if (enemy == null)
                return null;

            var newInstance = new Enemy(enemy);
            newInstance.level = lv.Clamp(1, int.MaxValue);
            newInstance.heroList = heroes;

            return newInstance;
        }

        public static void ConstructAllEnemies()
        {
            enemyList.Add(new Enemy("Growfa", lv: 1, elmt: Element.PLANT, mhp: 12, mcp: 30, atk: 4, def: 0, spAtk: 2, spd: 1, exp: 3, new List<SkillBase>()
            {
                SkillManager.Vine_Whip,
                SkillManager.Vine_Whip,
                SkillManager.Waste_Jumping
            }));

            enemyList.Add(new Enemy("Flarix", lv: 1, elmt: Element.FIRE, mhp: 20, mcp: 12, atk: 4, def: 0, spAtk: 5, spd: 1, exp: 8, new List<SkillBase>()
            {
                SkillManager.Fireball,
                SkillManager.Waste_Stare
            }));

            enemyList.Add(new Enemy("Plugry", lv: 1, elmt: Element.ELECTRIC, mhp: 18, mcp: 12, atk: 3, def: 0, spAtk: 8, spd: 2, exp: 10, new List<SkillBase>()
            {
                //SkillManager.Fireball,
                SkillManager.Waste_Short_Circut
            }));
        }
    }
}
