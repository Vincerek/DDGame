using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace project
{
    public class Rand
    {
        public int Run(int min, int max)
        {
            int range = (max - min) + 1;
            Random rng = new Random();
            return min + rng.Next() % range;
        }
    }

    public class Hero
    {
        public string Name;
        private int Strength;
        private int Dexterity;
        private int Intelligence;
        public double HP;
        public double MP;
        public int classID;

        //private Spell spell;
        public int isFrostbited = -1;
        public int isBurning = -1;
        public int isPoisoned = -1;
        public int isBleeding = -1;

        private void Init(int strength = 10, int dexterity = 10, int intelligence = 10, int classID=0)
        {
            this.Strength = strength;
            this.Dexterity = dexterity;
            this.Intelligence = intelligence;
            this.classID = classID;
            HP = 200 + 10*strength;
            MP = 10 + (3 * intelligence);
        }

        public int GetStrength() { return this.Strength; }
        public int GetDexterity() { return this.Dexterity; }
        public int GetIntelligence() { return this.Intelligence; }

        public void UpStrength() { this.Strength += 5; this.HP += 5; }
        public void UpDexterity() { this.Dexterity += 5; }
        public void UpIntelligence() { this.Intelligence += 5; this.MP += 15; }

        public Hero(string name, int myclass)
        {
            Name = name;
            switch (myclass)
            {
                case 1: Init(15, 10, 5, 1); break; //warrior
                case 2: Init(5, 15, 10, 2); break; //assassin
                case 3: Init(5, 5, 20, 3); break; //sorcerer
                default: Init(); break;
            }
        }

        public void Attack(Hero enemy)
        {
            Rand rand = new Rand();
            double damage = Strength * rand.Run(10, 30) / 10;
            if (rand.Run(0, 40) > enemy.GetDexterity())
            {

                Console.WriteLine($"\nHit!\n{this.Name} has striked {enemy.Name} for {damage} damage!");//
                enemy.HP -= damage;
                Console.WriteLine($"{enemy.Name} now has {enemy.HP} health\n");//
            }
            else Console.WriteLine($"\n{this.Name} is trying to Attack {enemy.Name} for {damage}... \n{enemy.Name} has dodged!\n");
        }

        public void SorcerersSpellBook(Hero enemy)
        {
            Console.Write("  1:Meditate, 2:Heal, 3:Flare, 4:Avalanche Lance... ");
            int opt = int.Parse(Console.ReadLine());

            switch (opt)
            {
                case 1: Meditate(); break;
                case 2: Heal(this.MP); break;
                case 3: Flare(enemy, this.MP); break;
                case 4: AvalancheLance(enemy, this.MP); break;
            }

            Console.WriteLine();
        }
        public void AssassinsSpellBook(Hero enemy)
        {
            Console.Write("  1:Meditate, 2:Corrupted Blade, 3:Nocturnal Slash, 4.Frostbite Laceration ... ");
            int opt = int.Parse(Console.ReadLine());

            switch (opt)
            {
                case 1: Meditate(); break;
                case 2: CorruptedBlade(enemy,this.MP); break;
                case 3: NocturnalSlash(enemy, this.MP); break;
                case 4: FrostbiteLaceration(enemy, this.MP); break;
            }

            Console.WriteLine();
        }

        public void LevelUp()
        {
            Console.Write("  1:Strength, 2:Dexterity, 3:Intelligence ... ");
            int opt = int.Parse(Console.ReadLine());

            switch (opt)
            {
                case 1: UpStrength(); Console.Write($"{this.Name} is stronger"); break;
                case 2: UpDexterity(); Console.Write($"{this.Name} is more agile"); break;
                case 3: UpIntelligence(); Console.Write($"{this.Name} is smarter"); break;
                default : UpStrength(); Console.Write($"{this.Name} is stronger"); break;
            }

            Console.WriteLine();
        }
        // Regeneration heals every turn, if target of regeneration is bleeding. Character wont heal this round.
        public void regeneration()
        {
            double heal = 2+(2*(this.Intelligence+this.Strength) / 20);
            if (this.isBleeding > 0) heal = 0;
            this.HP += Math.Round(heal);
            Console.WriteLine($"{this.Name} regenerated {heal} HP");
        }
        //Burning does constant damage for 3 turns.
        public int burning()
        {

            if (this.isBurning < 0) return this.isBurning = -1;
            else if (this.isBurning == 0)
            {
                Console.WriteLine($"{this.Name} has extinguished");
                return this.isBurning -= 1;
            }
            double damage = 7;
            this.HP -= damage;
            Console.WriteLine($"{this.Name} lost {damage} HP due to burning, {this.isBurning} turns left till extinguishing");
            return this.isBurning-=1;
        }
        //Bleeding deals Damage,heal halves from healing spells and stops regenration.
        public int bleeding()
        {

            if (this.isBleeding < 0) return this.isBleeding = -1;
            else if (this.isBleeding == 0)
            {
                Console.WriteLine($"{this.Name} is no longer bleeding");
                return this.isBleeding -= 1;
            }
            double damage = 3;
            this.HP -= damage;
            Console.WriteLine($"{this.Name} lost {damage} HP due to bleeding, {this.isBleeding} turns to stop the bleeding");
            return this.isBleeding -= 1;
        }
        //Poison deals 10% current health.
        public int poisoned()
        {

            if (this.isPoisoned < 0) return this.isPoisoned = -1;
            else if (this.isPoisoned == 0)
            {
                Console.WriteLine($"{this.Name} has found antidote");
                return this.isPoisoned -= 1;
            }
            double damage = this.HP*0.1;
            this.HP -= Math.Round(damage,0);
            Console.WriteLine($"{this.Name} lost {damage} HP due to being poisoned, {this.isPoisoned} turns left to find antidote");
            return this.isPoisoned -= 1;
        }
        //Frostbite makes next attack that would apply frostbite deal double damage.
        public int frostbite()
        {

            if (this.isFrostbited < 0) return this.isFrostbited = -1;
            else if (this.isFrostbited == 0)
            {
                Console.WriteLine($"{this.Name} is no longer cold");
                return this.isFrostbited -= 1;
            }
            Console.WriteLine($"{this.Name} is cold, {this.isFrostbited} turns left to warm up");
            return this.isFrostbited -= 1;
        }
        //Spells
        public void Meditate()
        {

            int HPRegen = this.Intelligence * 2;
            int MPRegen = (this.Intelligence / 5);
            this.MP += HPRegen;
            this.HP += MPRegen;
            Console.WriteLine($"\n{this.Name} has meditated for {HPRegen} HP and {MPRegen} MP\n");
        }

        public void Heal(double mana)
        {
            int manaCost = 50;
            if (mana >= manaCost)
            {
                Rand rand = new Rand();
                double heal = this.Intelligence * rand.Run(10, 20) / 10;
                if (this.isBleeding > 0) heal /= 2;
                this.HP += heal;
                Console.WriteLine($"\n{this.Name} has healed {heal} HP \n");
            }
            else
            {
                Console.WriteLine($"\nNot enough mana, casting Meditate!\n");
                Meditate();
            }

        }
        public void Flare(Hero enemy, double mana)
        {
            int manaCost= 40;
            if (mana >= manaCost)
            {
                Rand rand = new Rand();
                double damage = this.Intelligence * rand.Run(10, 15) / 10;
                if (rand.Run(0, 100) > enemy.GetDexterity())
                {
                    Console.WriteLine($"\nHit!\n{this.Name} has scorched {enemy.Name} for {damage} damage, and he's now burning!");
                    enemy.HP -= damage;
                    enemy.isBurning = 3;
                    Console.WriteLine($"{enemy.Name} now has {enemy.HP} health\n");
                }
                else Console.WriteLine($"\n{enemy.Name} has dodged your flare!\n");
            }
            else
            {
                Console.WriteLine($"\nNot enough mana, casting Meditate!\n");
                Meditate();
            }
            
        }
        public void AvalancheLance(Hero enemy, double mana)
        {
            int manaCost = 30;
            if (mana >= manaCost)
            {
                Rand rand = new Rand();
                double damage = this.Intelligence * rand.Run(10, 15) / 10;
                if (rand.Run(0, 100) > enemy.GetDexterity())
                {
                    if (enemy.isFrostbited > 0)
                    {
                        Console.WriteLine($"\nHit!\n{this.Name} has pierced {enemy.Name} twice for {2 * damage} damage!");
                        enemy.HP -= 2 * damage;
                        enemy.isFrostbited = 0;
                        Console.WriteLine($"{enemy.Name} now has {enemy.HP} health\n");
                    }
                    else
                    {
                        Console.WriteLine($"\nHit!\n{this.Name} has thrusted {enemy.Name} for {damage} damage, and he's under frostbite effect!");
                        enemy.HP -= damage;
                        enemy.isFrostbited = 2;
                        Console.WriteLine($"{enemy.Name} now has {enemy.HP} health\n");
                    }
                }
                else Console.WriteLine($"\n{enemy.Name} has dodged your Lance!\n");
            }
            else
            {
                Console.WriteLine($"\nNot enough mana, casting Meditate!\n");
                Meditate();
            }

        }
        public void CorruptedBlade(Hero enemy, double mana)
        {
            int manaCost = 20;
            if (mana >= manaCost)
            {
                Rand rand = new Rand();
                double damage = this.Dexterity * rand.Run(5, 10) / 10;
                if (rand.Run(0, 100) > enemy.GetDexterity())
                {
                    Console.WriteLine($"\nHit!\n{this.Name} has stabbed {enemy.Name} for {damage} damage, and he's now poisoned!");
                    enemy.HP -= damage;
                    enemy.isPoisoned = 3;
                    Console.WriteLine($"{enemy.Name} now has {enemy.HP} health\n");
                }
                else Console.WriteLine($"\n{enemy.Name} has dodged your stab!\n");
            }
            else
            {
                Console.WriteLine($"\nNot enough mana, casting Meditate!\n");
                Meditate();
            }

        }
        public void NocturnalSlash(Hero enemy, double mana)
        {
            int manaCost = 20;
            if (mana >= manaCost)
            {
                Rand rand = new Rand();
                double damage = this.Dexterity * rand.Run(5, 10) / 10;
                if (rand.Run(0, 100) > enemy.GetDexterity())
                {
                    Console.WriteLine($"\nHit!\n{this.Name} has slashed {enemy.Name} for {damage} damage, and he's now bleeding!");
                    enemy.HP -= damage;
                    enemy.isBleeding = 5;
                    Console.WriteLine($"{enemy.Name} now has {enemy.HP} health\n");
                }
                else Console.WriteLine($"\n{enemy.Name} has dodged your slash!\n");
            }
            else
            {
                Console.WriteLine($"\nNot enough mana, casting Meditate!\n");
                Meditate();
            }

        }
        public void FrostbiteLaceration(Hero enemy, double mana)
        {
            int manaCost = 20;
            if (mana >= manaCost)
            {
                Rand rand = new Rand();
                double damage = this.Dexterity * rand.Run(10, 15) / 10;
                if (rand.Run(0, 100) > enemy.GetDexterity())
                {
                    if (enemy.isFrostbited > 0)
                    {
                        Console.WriteLine($"\nHit!\n{this.Name} has slashed {enemy.Name} twice for {2*damage} damage!");
                        enemy.HP -= 2*damage;
                        enemy.isFrostbited = 0;
                        Console.WriteLine($"{enemy.Name} now has {enemy.HP} health\n");
                    }
                    else
                    {
                        Console.WriteLine($"\nHit!\n{this.Name} has slashed {enemy.Name} for {damage} damage, and he's under frostbite effect!");
                        enemy.HP -= damage;
                        enemy.isFrostbited = 2;
                        Console.WriteLine($"{enemy.Name} now has {enemy.HP} health\n");
                    }
                }
                else Console.WriteLine($"\n{enemy.Name} has dodged your slash!\n");
            }
            else
            {
                Console.WriteLine($"\nNot enough mana, casting Meditate!\n");
                Meditate();
            }

        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            bool combatWon;  
            //int enemyAttack = 0;
            Hero player1 = HeroCreator(); 
            Hero player2 = EnemyGenerator();

            Console.WriteLine();

            combat(player1, player2);
            if (combatWon)
            {
                Console.WriteLine($"{player1.Name} achived glorious Victory!");
                Console.WriteLine($"You have 3 level ups to spend");
                //combat(player1, player1);
            }
            else Console.WriteLine($"{player1.Name} has been Executed!");


            /**            Sprawdzanie który z graczy ma turę
            bool playerTurn(int turn)
            {
                if (turn % 2 == 1)
                {
                    return true;
                }
                else return false;
            }*/
            //Kreator postaci
            Hero HeroCreator()
            {
                string heroName;
                int heroClass;
                Console.WriteLine("Name your character:");
                heroName = Console.ReadLine();
                Console.WriteLine("Select your class: 1.Warrior 2.Assassin 3.Sorcerer");
                heroClass = int.Parse(Console.ReadLine());
                Hero player = new Hero(heroName, heroClass);
                return player;

            }
            //Tworzenie przeciwnika losowo
            Hero EnemyGenerator()
            {
                Random rand = new Random();
                int RandomClass = rand.Next(4);
                Hero enemy = new Hero(RandName(), RandomClass);
                return enemy;
            }
            //Funkcja odpowiadająca za walkę
            void combat(Hero player, Hero enemy)
            {
                combatWon = false;
                int turn = 1;

                while (player.HP > 0 && enemy.HP > 0)
                {
                    Console.WriteLine($"\nIt's turn number {turn}\n");
                    environment();


                    Console.WriteLine();
                    Console.WriteLine(player.Name + ": Str:{0} Dex:{1} Int:{2} HP:{3} MP:{4} ", player.GetStrength(), player.GetDexterity(), player.GetIntelligence(), player.HP, player.MP);
                    Console.WriteLine(enemy.Name + ": Str:{0} Dex:{1} Int:{2} HP:{3} MP:{4} \n", enemy.GetStrength(), enemy.GetDexterity(), enemy.GetIntelligence(), enemy.HP, enemy.MP);
                    

                    switch (player.classID)
                    {
                        case 1:
                            break;
                        case 2:
                            AssassinTurn();
                            break;
                        case 3:
                            SorcererTurn();
                            break;
                    }


                    switch (enemy.classID)
                        {                     
                            case 0: enemy.Attack(player); break;
                            case 1: enemy.Attack(player); break;
                            case 2: enemy.FrostbiteLaceration(player, enemy.MP); break;
                            case 3: enemy.Flare(player, enemy.MP); break;
                        }
                        /** Vs mode
                        Console.WriteLine("Your Turn: " + enemy.Name);
                        Console.Write("1:Attack, 2:Spellbook, 3:LevelUp ... ");
                        int opt = int.Parse(Console.ReadLine());

                        switch (opt)
                        {
                            case 1:
                                enemy.Attack(player);
                                break;

                            case 2:
                                enemy.SpellBook(player);
                                break;

                            case 3:
                                enemy.LevelUp();
                                break;
                            default:
                                enemy.Attack(player);
                                break;
                        }*/


                    Console.WriteLine();
                    turn++;
                    
                }
                if (player.HP > 0)  combatWon = true; else  combatWon = false;

                // TODO: Win

                void environment()
                {
                    if (turn != 1)
                    {
                        player.regeneration();
                        enemy.regeneration();
                        player.burning();
                        enemy.burning();
                        player.bleeding();
                        enemy.bleeding();
                        player.poisoned();
                        enemy.poisoned();
                        player.frostbite();
                        enemy.frostbite();
                    }
                }

                void SorcererTurn()
                {
                    Console.WriteLine("Your Turn: " + player.Name);
                    Console.Write("1:Attack, 2:Spellbook, 3:LevelUp ... ");
                    int opt = int.Parse(Console.ReadLine());
                    switch (opt)
                    {
                        case 1:
                            player.Attack(enemy);
                            break;
                        case 2:
                            player.SorcerersSpellBook(enemy);
                            break;
                        case 3:
                            player.LevelUp();
                            break;
                    }
                }
                void AssassinTurn()
                {
                    Console.WriteLine("Your Turn: " + player.Name);
                    Console.Write("1:Attack, 2:Elemental Daggers, 3:LevelUp ... ");
                    int opt = int.Parse(Console.ReadLine());
                    switch (opt)
                    {
                        case 1:
                            player.Attack(enemy);
                            break;
                        case 2:
                            player.AssassinsSpellBook(enemy);
                            break;
                        case 3:
                            player.LevelUp();
                            break;
                    }
                }

            }

            string RandName()
            {
                string[] maleNames = new string[] { "Aaron","Amaro", "Altair", "Arthur", "Axel", "Abraham", "Bruce", "Cameron", "Cole", "Edward", "Kyros", "Isaac","John","Kai","Lee","Leonardo","Kyle","Luke","Xavier","Clark" };
                string[] femaleNames = new string[] { "Abby", "Chloe", "Venice", "Sasha","Gorlock","Emma","Elizabeth" };
                string[] lastNames = new string[] { "Kent", "Crane", "West", "Hope", "Crane","Gray","Carter","Yearwood","Oakleaf","\"The Lightbringer\"","\"The Last of Many\"","\"The First Human\"","\"The Destroyer\"","Kent","Wayne","Driscoll","Lightfoot" };
                string FirstName;
                string LastName;
                string FullName;
                Random rand = new Random();
                if (rand.Next(1, 2) == 1)
                {
                    FirstName = maleNames[rand.Next(0, maleNames.Length - 1)];
                }
                else
                {
                    FirstName = femaleNames[rand.Next(0, femaleNames.Length - 1)];
                }
                LastName = lastNames[rand.Next(0, lastNames.Length - 1)];

                return FullName = $"{FirstName} {LastName}";
            }
        }
    }
}