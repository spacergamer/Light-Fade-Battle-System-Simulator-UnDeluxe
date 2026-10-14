using System;
using System.Collections.Generic;

namespace ShadowLightPrototype
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== SHADOW LIGHT ===");

            Player player = ChooseCharacter();

            ChooseWeapon(player);
            SelectSkills(player, 3);

            Enemy galanga = new Enemy("Galanga");

            BattleSystem.StartBattle(player, galanga);

            Console.ReadKey();
        }

        static Player ChooseCharacter()
        {
            Console.WriteLine("\nEscolha um personagem:\n");

            Console.WriteLine("1 - Lukas");
            Console.WriteLine("ATK:16 DEF:7 IQ:115 AGI:14\n");

            Console.WriteLine("2 - Brina");
            Console.WriteLine("ATK:8 DEF:15 IQ:123 AGI:12\n");

            Console.WriteLine("3 - Ninbom");
            Console.WriteLine("ATK:12 DEF:10 IQ:117 AGI:20\n");

            Console.WriteLine("4 - Hannah");
            Console.WriteLine("ATK:14 DEF:3 IQ:134 AGI:13\n");

            string choice = Console.ReadLine();

            return choice switch
            {
                "2" => new Player("Brina", 8, 15, 123, 12),
                "3" => new Player("Ninbom", 12, 10, 117, 20),
                "4" => new Player("Hannah", 14, 3, 134, 13),
                _ => new Player("Lukas", 16, 7, 115, 14),
            };
        }

        static void ChooseWeapon(Player p)
        {
            Console.WriteLine("\nTipo de arma:");
            Console.WriteLine("1 - Branca");
            Console.WriteLine("2 - Fogo");

            if (Console.ReadLine() == "2")
            {
                p.WeaponType = WeaponType.Firearm;
                Console.WriteLine("1 - Pistola (8 balas)");
                Console.WriteLine("2 - Revólver (6 balas)");

                if (Console.ReadLine() == "2")
                    p.EquipFirearm("Revólver", 6, 14);
                else
                    p.EquipFirearm("Pistola", 8, 11);
            }
            else
            {
                p.WeaponType = WeaponType.Melee;
                Console.WriteLine("1 - Faca (Sangramento)");
                Console.WriteLine("2 - Bastão (Atordoar)");

                if (Console.ReadLine() == "2")
                    p.EquipMelee("Bastão", 16, MeleeEffect.Stun);
                else
                    p.EquipMelee("Faca", 12, MeleeEffect.Bleed);
            }
        }

        static void SelectSkills(Player p, int max)
        {
            Console.WriteLine("\n=== SELEÇÃO DE HABILIDADES ===");

            while (p.Skills.Count < max)
            {
                Console.WriteLine($"\nEscolha ({p.Skills.Count}/{max})");

                for (int i = 0; i < SkillShop.Skills.Count; i++)
                    Console.WriteLine($"{i} - {SkillShop.Skills[i].Name}");

                if (int.TryParse(Console.ReadLine(), out int choice) &&
                    choice >= 0 && choice < SkillShop.Skills.Count)
                {
                    var skill = SkillShop.Skills[choice];

                    if (p.Skills.Exists(s => s.Name == skill.Name))
                    {
                        Console.WriteLine("Você já escolheu essa habilidade.");
                        continue;
                    }

                    p.Skills.Add(new Skill(skill.Name));
                    Console.WriteLine($"Selecionado: {skill.Name}");
                }
            }
        }
    }

    enum WeaponType { Melee, Firearm }
    enum MeleeEffect { None, Bleed, Stun }

    class Player
    {
        public string Name;

        public int HP = 100;
        public int ATK;
        public int DEF;
        public int AGI;
        public int IQ;

        public WeaponType WeaponType;
        public string WeaponName;

        public int Ammo;
        public int MaxAmmo;
        public int FirearmDamage;

        public int MeleeDamage;
        public MeleeEffect MeleeEffect;

        public bool IsBleeding;
        public bool IsBurning;
        public bool IsPoisoned;
        public bool CannotEvade;
        public bool GuaranteedDodge;

        public int BuffAttack = 0;
        public int BuffDefense = 0;
        public int BuffAccuracy = 0;
        public int BuffCritChance = 0;
        public int BuffDodge = 0;

        public int TurnsAttack = 0;
        public int TurnsDefense = 0;
        public int TurnsAccuracy = 0;
        public int TurnsCrit = 0;
        public int TurnsDodge = 0;

        public int MedKit = 3;
        public int Antidote = 3;
        public int Water = 3;

        public List<Skill> Skills = new();

        public Player(string n, int atk, int def, int iq, int agi)
        {
            Name = n;
            ATK = atk;
            DEF = def;
            IQ = iq;
            AGI = agi;
        }

        public void EquipFirearm(string name, int ammo, int dmg)
        {
            WeaponName = name;
            Ammo = ammo;
            MaxAmmo = ammo;
            FirearmDamage = dmg;
        }

        public void EquipMelee(string name, int dmg, MeleeEffect effect)
        {
            WeaponName = name;
            MeleeDamage = dmg;
            MeleeEffect = effect;
        }

        public void ApplyDamage(int dmg)
        {
            int totalDef = DEF + BuffDefense;
            int final = Math.Max(0, dmg - totalDef);
            HP -= final;
            Console.WriteLine($"Você sofreu {final} de dano.");
        }

        public bool Alive() => HP > 0;
    }

    class Enemy
    {
        public string Name;
        public int HP;
        public int ATK;
        public int DEF;
        public int IQ;
        public int AGI;

        public int BleedTurns = 0;
        bool PhaseTwoTriggered = false;

        Random rng = new();

        public Enemy(string n)
        {
            Name = n;
            HP = 150;
            ATK = 11;
            DEF = 7;
            IQ = 119;
            AGI = 12;
        }

        public void ApplyDamage(int dmg)
        {
            int final = Math.Max(0, dmg - DEF);
            HP -= final;

            Console.WriteLine($"{Name} sofreu {final}.");

            if (final > 15)
                Console.WriteLine("Galanga: \"Tch... então você consegue me ferir.\"");

            if (HP <= 60 && !PhaseTwoTriggered)
            {
                PhaseTwoTriggered = true;
                Console.WriteLine("\nGalanga: \"Heh... agora eu paro de brincar.\"");
                ATK += 4;
                AGI += 5;
            }

            if (HP <= 0)
                Console.WriteLine("Galanga: \"Então era você... desde o início...\"");
        }

        public void SpecialAttack(Player p)
        {
            int roll = rng.Next(100);

            if (roll < 33)
            {
                Console.WriteLine("Galanga usa Golpe Incendiário.");
                Console.WriteLine("Galanga: \"Queime.\"");
                p.IsBurning = true;
                p.ApplyDamage(ATK + 6);
            }
            else if (roll < 66)
            {
                Console.WriteLine("Galanga usa Lâmina Tóxica.");
                Console.WriteLine("Galanga: \"A dor vem devagar.\"");
                p.IsPoisoned = true;
                p.ApplyDamage(ATK + 3);
            }
            else
            {
                Console.WriteLine("Galanga usa Investida Brutal.");
                Console.WriteLine("Galanga: \"CAIA!\"");
                p.ApplyDamage(ATK + 8);
            }
        }

        public void TauntRandom()
        {
            int roll = rng.Next(100);

            if (roll < 20)
                Console.WriteLine("Galanga: \"Você é lento.\"");
            else if (roll < 40)
                Console.WriteLine("Galanga: \"Isso é tudo que você tem?\"");
            else if (roll < 60)
                Console.WriteLine("Galanga: \"Eu já venci antes de começar.\"");
            else if (roll < 80)
                Console.WriteLine("Galanga: \"Fraco.\"");
            else
                Console.WriteLine("Galanga apenas sorri.");
        }

        public bool Alive() => HP > 0;
    }

    static class BattleSystem
    {
        static Random rng = new();

        public static void StartBattle(Player p, Enemy e)
        {
            Console.WriteLine($"\nGalanga: \"Então você é {p.Name}...\"");

            if (rng.Next(100) < 25)
            {
                e.TauntRandom();
            }

            while (p.Alive() && e.Alive())
            {
                UpdateBuffs(p);
                ApplyStatus(p, e);

                Console.WriteLine($"\nHP:{p.HP} DEF:{p.DEF + p.BuffDefense} MUNI:{p.Ammo}");
                Console.WriteLine("A-Atacar D-Defender E-Esquivar I-Inventário H-Habilidade R-Recarregar");

                string input = Console.ReadLine().ToUpper();

                switch (input)
                {
                    case "H": UseSkillMenu(p, e); break;
                    case "R":
                        if (p.WeaponType == WeaponType.Firearm)
                        {
                            p.Ammo = p.MaxAmmo;
                            Console.WriteLine("Recarregando.");
                        }
                        break;
                    case "I": OpenInventory(p); break;
                    case "D":
                        p.BuffDefense = 5;
                        p.TurnsDefense = 1;
                        Console.WriteLine("Você se preparou para defender.");
                        break;
                    case "E": AttemptEvade(p); break;
                    default: AttackLogic(p, e); break;
                }

                if (!e.Alive()) break;

                Console.WriteLine("\nGalanga avança...");
                Console.WriteLine("T-Tomar Dano | V-Desviar");

                string defChoice = Console.ReadLine().ToUpper();

                if (defChoice == "V" && !p.CannotEvade)
                {
                    if (p.GuaranteedDodge || rng.Next(100) < p.AGI + p.BuffDodge + 20)
                    {
                        Console.WriteLine("Você desviou.");
                        p.GuaranteedDodge = false;
                    }
                    else
                    {
                        Console.WriteLine("Falhou na esquiva.");
                        EnemyAttack(p, e);
                    }
                }
                else
                {
                    EnemyAttack(p, e);
                }

                p.CannotEvade = false;
            }

            if (p.Alive())
            {
                Console.WriteLine("\n=== GALANGA DERROTADO ===");
                PlayerVictoryLine(p.Name);
            }
            else
            {
                Console.WriteLine("\n=== VOCÊ FOI DERROTADO ===");
                PlayerDeathLine(p.Name);
            }
        }

        static void PlayerVictoryLine(string name)
        {
            switch (name)
            {
                case "Lukas":
                    Console.WriteLine("Lukas: \"Você lutou bem... mas não o suficiente.\"");
                    break;
                case "Brina":
                    Console.WriteLine("Brina: \"Estratégia supera força bruta.\"");
                    break;
                case "Ninbom":
                    Console.WriteLine("Ninbom: \"Rápido demais pra você acompanhar.\"");
                    break;
                case "Hannah":
                    Console.WriteLine("Hannah: \"Eu já tinha calculado sua derrota.\"");
                    break;
            }

            Console.WriteLine("Galanga cai lentamente.");
        }

        static void PlayerDeathLine(string name)
        {
            switch (name)
            {
                case "Lukas":
                    Console.WriteLine("Lukas: \"Eu... ainda... posso...\"");
                    break;
                case "Brina":
                    Console.WriteLine("Brina: \"Impossível... eu errei os cálculos...\"");
                    break;
                case "Ninbom":
                    Console.WriteLine("Ninbom: \"Droga... não fui rápido o bastante...\"");
                    break;
                case "Hannah":
                    Console.WriteLine("Hannah: \"Isso... não estava... na previsão...\"");
                    break;
            }

            Console.WriteLine("Galanga: \"Eu já sabia o resultado.\"");
        }

        static void UpdateBuffs(Player p)
        {
            HandleBuff(ref p.TurnsAttack, ref p.BuffAttack);
            HandleBuff(ref p.TurnsDefense, ref p.BuffDefense);
            HandleBuff(ref p.TurnsAccuracy, ref p.BuffAccuracy);
            HandleBuff(ref p.TurnsCrit, ref p.BuffCritChance);
            HandleBuff(ref p.TurnsDodge, ref p.BuffDodge);
        }

        static void HandleBuff(ref int turns, ref int value)
        {
            if (turns > 0)
            {
                turns--;
                if (turns == 0)
                    value = 0;
            }
        }

        static void EnemyAttack(Player p, Enemy e)
        {
            Console.WriteLine("\nGalanga avança.");

            if (rng.Next(100) < 40)
                e.SpecialAttack(p);
            else
            {
                Console.WriteLine("Galanga: \"Desapareça.\"");
                p.ApplyDamage(e.ATK);
            }

            if (!p.Alive())
                Console.WriteLine("Galanga: \"Era só isso?\"");
        }

        static void AttemptEvade(Player p)
        {
            if (rng.Next(100) < p.AGI + p.BuffDodge)
                Console.WriteLine("Você se posicionou melhor.");
            else
                Console.WriteLine("Movimento falhou.");
        }

        static void AttackLogic(Player p, Enemy e)
        {
            if (p.WeaponType == WeaponType.Firearm)
            {
                if (p.Ammo <= 0)
                {
                    Console.WriteLine("Sem munição.");
                    return;
                }

                Console.WriteLine("I-Tiro IQ | G-Tiro AGI");
                string mode = Console.ReadLine().ToUpper();
                p.Ammo--;

                if (mode == "I")
                {
                    p.CannotEvade = true;
                    Console.WriteLine("1-Cabeça 2-Tórax 3-Pernas 4-Braços");

                    if (!int.TryParse(Console.ReadLine(), out int part))
                        return;

                    int iqReal = (p.IQ - 100) / 2;
                    int d6 = rng.Next(1, 7);

                    if (d6 + iqReal + p.BuffAccuracy >= 8)
                    {
                        int bonus = part switch
                        {
                            1 => 20,
                            2 => 12,
                            3 => 6,
                            _ => 4
                        };

                        int total = p.FirearmDamage + bonus + p.BuffAttack;

                        if (rng.Next(100) < p.BuffCritChance)
                        {
                            total *= 2;
                            Console.WriteLine("SMAAASH!!");
                        }

                        e.ApplyDamage(total);
                    }
                    else
                        Console.WriteLine("Errou.");
                }
                else
                {
                    int d100 = rng.Next(1, 101);

                    int bonus = d100 switch
                    {
                        <= 10 => 20,
                        <= 40 => 12,
                        <= 70 => 6,
                        _ => 4
                    };

                    int total = p.FirearmDamage + bonus + p.BuffAttack;

                    if (rng.Next(100) < p.BuffCritChance)
                    {
                        total *= 2;
                        Console.WriteLine("SMAAASH!!");
                    }

                    e.ApplyDamage(total);
                }
            }
            else
            {
                int d20 = rng.Next(1, 21);
                int total = p.MeleeDamage + p.BuffAttack;

                if (d20 >= 18)
                {
                    total *= 2;
                    Console.WriteLine("SMAAASH!!");
                }

                e.ApplyDamage(total);

                if (p.MeleeEffect == MeleeEffect.Bleed)
                {
                    e.BleedTurns = 3;
                    Console.WriteLine("Galanga está sangrando.");
                }
            }
        }

        static void ApplyStatus(Player p, Enemy e)
        {
            if (p.IsBurning)
                p.ApplyDamage(5);

            if (p.IsPoisoned)
                p.ApplyDamage(3);

            if (e.BleedTurns > 0)
            {
                e.ApplyDamage(4);
                e.BleedTurns--;
            }
        }

        static void OpenInventory(Player p)
        {
            Console.WriteLine("\n1-Kit Médico 2-Antídoto 3-Água");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    if (p.MedKit > 0)
                    {
                        p.HP += 30;
                        p.MedKit--;
                    }
                    break;
                case "2":
                    if (p.Antidote > 0)
                    {
                        p.IsPoisoned = false;
                        p.Antidote--;
                    }
                    break;
                case "3":
                    if (p.Water > 0)
                    {
                        p.IsBurning = false;
                        p.Water--;
                        p.HP += 10;
                    }
                    break;
            }
        }

        static void UseSkillMenu(Player p, Enemy e)
        {
            for (int i = 0; i < p.Skills.Count; i++)
                Console.WriteLine($"{i} - {p.Skills[i].Name}");

            if (int.TryParse(Console.ReadLine(), out int choice) &&
                choice >= 0 && choice < p.Skills.Count)
                p.Skills[choice].Use(p, e);
        }
    }

    class Skill
    {
        public string Name;
        public Skill(string n) => Name = n;

        public void Use(Player p, Enemy e)
        {
            switch (Name)
            {
                case "Servente de Pedreiro":
                    p.BuffAttack = 12;
                    p.TurnsAttack = 2;
                    break;
                case "Casca Grossa":
                    p.BuffDefense = 10;
                    p.TurnsDefense = 2;
                    break;
                case "Pistoleiro":
                    p.BuffAccuracy = 15;
                    p.TurnsAccuracy = 2;
                    break;
                case "Medicina":
                    p.BuffDefense = 6;
                    p.TurnsDefense = 3;
                    break;
                case "Ousado":
                    p.BuffCritChance = 20;
                    p.TurnsCrit = 2;
                    break;
                case "Sortudo":
                    p.BuffDodge = 25;
                    p.TurnsDodge = 2;
                    break;
                case "Olho de Escopeta":
                    p.BuffAccuracy = 25;
                    p.TurnsAccuracy = 1;
                    break;
                case "Homem Mosca":
                    p.BuffDodge = 40;
                    p.TurnsDodge = 1;
                    break;
            }
        }
    }

    static class SkillShop
    {
        public static List<Skill> Skills = new List<Skill>
        {
            new Skill("Servente de Pedreiro"),
            new Skill("Casca Grossa"),
            new Skill("Pistoleiro"),
            new Skill("Medicina"),
            new Skill("Ousado"),
            new Skill("Sortudo"),
            new Skill("Olho de Escopeta"),
            new Skill("Homem Mosca")
        };
    }
}