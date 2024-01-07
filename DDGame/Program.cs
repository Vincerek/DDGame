using System;
using System.ComponentModel;
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

        private void Init(int strength = 10, int dexterity = 10, int intelligence = 10, int classID=0)
        {
            this.Strength = strength;
            this.Dexterity = dexterity;
            this.Intelligence = intelligence;
            this.classID = classID;
            HP = 50 + strength;
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
            double damage = Strength * rand.Run(5, 10) / 10;
            if (rand.Run(0, 100) > enemy.GetDexterity())
            {

                Console.WriteLine($"\nHit!\n{this.Name} has striked {enemy.Name} for {damage} damage!");//
                enemy.HP -= damage;
                Console.WriteLine($"{enemy.Name} now has {enemy.HP} health\n");//
            }
            else Console.WriteLine($"\n{enemy.Name} has dodged!\n");
        }

        public void SpellBook(Hero enemy)
        {
            Console.Write("  1:Meditate, 2:Heal, 3:Flare ... ");
            int opt = int.Parse(Console.ReadLine());

            switch (opt)
            {
                case 1: Meditate(); break;
                case 2: Heal(this.MP); break;
                case 3: Flare(enemy, this.MP); break;
            }

            Console.WriteLine();
        }

        public void LevelUp()
        {
            Console.Write("  1:Strength, 2:Dexterity, 3:Intelligence ... ");
            int opt = int.Parse(Console.ReadLine());

            switch (opt)
            {
                case 1: UpStrength(); break;
                case 2: UpDexterity(); break;
                case 3: UpIntelligence(); break;
                default : UpStrength(); break;
            }

            Console.WriteLine();
        }
        public void regeneration()
        {
            double heal = 2*(this.Intelligence+this.Strength) / 20;
            this.HP += Math.Round(heal);
            //Console.WriteLine($"\n{this.Name} has regenerated {heal} HP \n");
        }

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
            int manaCost = 20;
            if (mana >= manaCost)
            {
                Rand rand = new Rand();
                double heal = this.Intelligence * rand.Run(5, 10) / 10;
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
                    Console.WriteLine($"\nHit!\n{this.Name} has lit up {enemy.Name} for {damage} damage!");
                    enemy.HP -= damage;
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



        // public void Spell()
        // {
        //   // TODO: 3 spell's
        // }



    }

    class Program
    {
        static void Main(string[] args)
        {
            Hero player1 = HeroCreator();
            Hero player2 = EnemyGenerator();

            Console.WriteLine();

            combat(player1, player2);


            //Sprawdzanie który z graczy ma turę
            bool playerTurn(int turn)
            {
                if (turn % 2 == 1)
                {
                    return true;
                }
                else return false;
            }
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
            Hero EnemyGenerator()
            {
                Random rand = new Random();
                int RandomClass = rand.Next(1, 3);
                Hero enemy = new Hero(RandName(), RandomClass);
                return enemy;
            }
            //Funkcja odpowiadająca za wygraną
            void combat(Hero player, Hero enemy)
            {
                int turn = 1;

                while (player.HP > 0 && enemy.HP > 0)
                {
                    Console.WriteLine("\nIt's turn number " + turn);

                    if (playerTurn(turn))player.regeneration(); else enemy.regeneration();
                    Console.WriteLine(player.Name + ": Str:{0} Dex:{1} Int:{2} HP:{3} MP:{4} ", player.GetStrength(), player.GetDexterity(), player.GetIntelligence(), player.HP, player.MP);
                    Console.WriteLine(enemy.Name + ": Str:{0} Dex:{1} Int:{2} HP:{3} MP:{4} \n", enemy.GetStrength(), enemy.GetDexterity(), enemy.GetIntelligence(), enemy.HP, enemy.MP);
                    
                    if (playerTurn(turn))
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
                                player.SpellBook(enemy);
                                break;

                            case 3:
                                player.LevelUp();
                                break;
                        }
                    }
                    else
                    {
                        switch (enemy.classID)
                        {
                            case 0: enemy.Attack(player); break;
                            case 1: enemy.Attack(player); break;
                            case 2: enemy.Attack(player); break;
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
                    }

                    Console.WriteLine();
                    turn++;
                }
                // TODO: Win
                

            }

            string RandName()
            {
                string[] maleNames = new string[] { "Aaron", "Altair", "Arthur", "Axel", "Abraham", "Bruce", "Cameron", "Cole", "Edward", "Kyros", "Isaac","John","Kai","Lee","Leonardo","Kyle","Luke","Xavier","Clark" };
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