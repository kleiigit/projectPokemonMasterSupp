
using ProjetoPokemon.Entities;
using ProjetoPokemon.Entities.Battlers;
using ProjetoPokemon.Enums;
using ProjetoPokemon.Services;

namespace ProjetoPokemon.Managers
{
    internal class CombatManager
    {
        private static readonly ConsoleColor atkColor = ConsoleColor.Cyan;
        private static readonly ConsoleColor defColor = ConsoleColor.Magenta;
        public Battler BattlerAttacker { get; set; }
        public Battler BattlerDefender { get; set; }

        // Construct
        public CombatManager() { }
        public CombatManager(BoxPokemon ProfileAtk, BoxPokemon ProfileDef) // trainer vs trainer setup
        {
            BattlerAttacker = BattlerPlayer.CreateTrainer(ProfileAtk, atkColor, SetupBattle.Attacker);
            BattlerDefender = BattlerPlayer.CreateTrainer(ProfileDef, defColor, SetupBattle.Defender);
        }
        public CombatManager(BoxPokemon ProfileAtk, ColorToken wildColor) // wild setup
        {
            BattlerAttacker = BattlerPlayer.CreateTrainer(ProfileAtk, atkColor, SetupBattle.Attacker);
            BattlerDefender = new Battler(WildGeneratorService.WildByColor(wildColor), SetupBattle.Defender);
        }
        public CombatManager(Battler ProfileAtk, Battler ProfileDef) // clone Battletest setup
        {
            BattlerAttacker = BattlerTest.CloneBattler(ProfileAtk);
            BattlerDefender = BattlerTest.CloneBattler(ProfileDef);
        }

        public void Setup(Move? autoMove) // selecinar os moves e itens
        {
            // check debuf nerf
            BattlerAttacker.Total.NumberDices = BattlerAttacker.BuffsAndDebuffs.Any(p => p == EffectType.NERF) ? -1 : 0;
            BattlerDefender.Total.NumberDices = BattlerDefender.BuffsAndDebuffs.Any(p => p == EffectType.NERF) ? -1 : 0;

            // move half level effect
            CombatControl.ApplyHalfLevelEffect(BattlerAttacker, BattlerDefender);
            CombatControl.ApplyHalfLevelEffect(BattlerDefender, BattlerAttacker);

            // check condition status
            if (BattlerAttacker.CheckCondition()) BattlerAttacker.SetUsedMove(BattlerAttacker.SelectMove(BattlerDefender));
            if (BattlerDefender.CheckCondition()) BattlerDefender.SetUsedMove(BattlerDefender.SelectMove(BattlerAttacker));

            // use Item
            if (BattlerAttacker is BattlerPlayer playerAtk && BattlerAttacker.BuffsAndDebuffs.Any(p => p != EffectType.NOITEM))
            {
                BattlerAttacker.UsedCard = playerAtk.TrainerBox.UseItemCard(TypeItemCard.Battle);
                if (BattlerAttacker.UsedCard != null)
                {
                    
                    BattlerAttacker.UsedCard.BattleCard(BattlerAttacker, BattlerDefender);
                }
            }
            if (BattlerDefender is BattlerPlayer playerDef && BattlerDefender.BuffsAndDebuffs.Any(p => p != EffectType.NOITEM))
            {
                BattlerDefender.UsedCard = playerDef.TrainerBox.UseItemCard(TypeItemCard.Battle);
                if (BattlerDefender.UsedCard != null)
                {
                    BattleLog.AddLog($"\n{playerDef.TrainerName} used the item card {BattlerDefender.UsedCard.Name}!");
                    BattlerDefender.UsedCard.BattleCard(BattlerDefender, BattlerAttacker);
                }
            }

            Calculation(BattlerAttacker, BattlerDefender);
            Calculation(BattlerDefender, BattlerAttacker);
            BattlerAttacker.Total.TotalPower(BattlerAttacker);
            BattlerDefender.Total.TotalPower(BattlerDefender);
            BattleLog.ShowSetupLogs(BattlerAttacker, BattlerDefender);
            VictoryChanceService.ShowResult();

            if (BattlerAttacker.Total.TotalResult == BattlerDefender.Total.TotalResult) // empate
            {
                if (BattlerDefender is BattlerPlayer)
                {
                    do
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Tie! Roll the dice again.\n");
                        Console.ResetColor();
                        Console.ReadLine();
                        BattlerAttacker.Total.Roll = CombatControl.RollDices(BattlerAttacker);
                        BattlerDefender.Total.Roll = CombatControl.RollDices(BattlerDefender);
                        // battle log
                        //BattleLog.ClearLogs();
                        BattlerAttacker.Total.TotalPower(BattlerAttacker);
                        BattlerDefender.Total.TotalPower(BattlerDefender);
                    }
                    while (BattlerAttacker.Total.TotalResult == BattlerDefender.Total.TotalResult);
                    VictoryChanceService.ShowResult();
                }
                else BattlerDefender.Pokemon.Conditions = StatusConditions.KNOCKED; // selvagem
            }

            Result();
        }
        public static void Calculation(Battler attacker, Battler defender) // modificações de combate
        {
            CombatControl.MoveEffect(attacker, defender);

            attacker.Total.Roll = CombatControl.RollDices(attacker); // rolagem do dado do move, de acordo com o efeito
            attacker.Total.EffectiveBonus = EffectiveTypeService.GetTypeModifier(attacker.GetUsedMoveType(), defender.Pokemon.Pokemon.Type); // effetivo de tipo
            attacker.Total.StatusBonus = CombatControl.ConditionMod(attacker); // penalidade de status

            attacker.Total.WeatherBonus = FieldControlService.WeatherBonus(attacker); // efeito do clima
            if (!attacker.Pokemon.CanAttack) attacker.SelectMove(null);


            // downed effect
            if (attacker.GetUsedMoveType() == TypePokemon.Ground && defender.HasBuffEffect(EffectType.DOWNED) && attacker.Total.EffectiveBonus < 0)
            {
                BattleLog.AddLog($"{defender.Pokemon.Name} downed and can’t resist Ground-type attacks.");
                attacker.Total.EffectiveBonus = 0;
            }
            // torment effect
            if (attacker.HasBuffEffect(EffectType.ENRAGED) && attacker.MovesPokemon.Where(p => p.CanUse == true).Count() > 0)
            {
                attacker.RechargeMove();
                if (attacker.LastMove != null) attacker.LastMove.CanUse = true;
            }
            // dragon rage, imune
            if (attacker.CheckEffectUsedMove(EffectType.IMMUNE))
            {
                attacker.Total.EffectiveBonus = 0;
            }
            //dream eater
            if (defender.Pokemon.Conditions == StatusConditions.SLEEP && attacker.GetUsedMoveID() == 157)
            {
                BattleLog.AddLog(attacker + " doubled the attack’s power by devouring the opponent’s dreams!");
                attacker.Total.RollBonus += attacker.Total.Roll + attacker.Total.RollBonus;
            }
            attacker.LastMove = attacker.GetUsedMove();

            attacker.Total.TotalPower(attacker);
        }

        public void Result() // resultado do combate
        {
            ProfilePokemon pokemonAtk = BattlerAttacker.Pokemon;
            ProfilePokemon pokemonDef = BattlerDefender.Pokemon;

            string victoryPokemon = BattlerAttacker.Total.TotalResult >= BattlerDefender.Total.TotalResult ? BattlerAttacker.ToString() : BattlerDefender.ToString();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n" + victoryPokemon + " won the battle!\n");
            Console.ResetColor();

            // KNOCKED OUT, POISONED check and level up

            if (BattlerAttacker.Total.TotalResult > BattlerDefender.Total.TotalResult) BattlerDefender.Pokemon.Conditions = StatusConditions.KNOCKED;
            else if (BattlerAttacker.Total.TotalResult < BattlerDefender.Total.TotalResult) BattlerAttacker.Pokemon.Conditions = StatusConditions.KNOCKED;
            if (pokemonAtk.Conditions == StatusConditions.POISONED) BattleConditions.PoisonRoll(BattlerAttacker);
            if (pokemonDef.Conditions == StatusConditions.POISONED) BattleConditions.PoisonRoll(BattlerDefender);
            if (pokemonAtk.Conditions == StatusConditions.KNOCKED) Console.WriteLine(BattlerAttacker + " has been knocked out!");
            else if (BattlerAttacker.CheckEffectUsedMove(EffectType.BOOST))
            {
                Console.WriteLine(BattlerAttacker.GetUsedMoveName() + " got a boost!");
                BattlerAttacker.GetUsedMove().BoostPower();
            }
            if (pokemonDef.Conditions == StatusConditions.KNOCKED) Console.WriteLine(BattlerDefender + " has been knocked out!");
            else if (BattlerDefender.CheckEffectUsedMove(EffectType.BOOST))
            {
                Console.WriteLine(BattlerDefender.GetUsedMoveName() + " got a boost!");
                BattlerDefender.GetUsedMove().BoostPower();
            }
            if (pokemonAtk.LevelPokemon() <= pokemonDef.LevelPokemon() && pokemonAtk.Conditions != StatusConditions.KNOCKED)
                pokemonAtk.LevelUpPokemon();

            if (pokemonDef.LevelPokemon() <= pokemonAtk.LevelPokemon() && pokemonDef.Conditions != StatusConditions.KNOCKED)
            {
                if (BattlerDefender is BattlerPlayer)
                {
                    pokemonDef.LevelUpPokemon();
                }
            }
            Console.ReadLine();
        }
    }
}
