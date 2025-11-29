
using DocumentFormat.OpenXml.Wordprocessing;
using ProjetoPokemon.Enums;
using ProjetoPokemon.Services;

namespace ProjetoPokemon.Entities.Battlers
{
    internal class BattlerGym : Battler
    {
        public Trainer Trainer { get; }

        public BattlerGym(Trainer trainer, ProfilePokemon profile, SetupBattle setup) : base(profile, setup)
        {
            Trainer = trainer;
        }

        public override Move SelectMove(Battler target) // IA Trainer move
        {
            List<Move> listMoves = GetMoveList();

            if (Pokemon.CanAttack == false || listMoves.Count == 0)
                return Move.Null();

            foreach (Move move in listMoves) { move.RateWin(VictoryChanceService.ChanceSimulator(move, this, target)); }


            return listMoves.OrderByDescending(p => p.Rate).First();
        }
        public override ProfilePokemon ChangeRandomPokemon()
        {
            if (Trainer.ListPokemon != null)
            {
                var availablePokemons = Trainer.ListPokemon
                    .Where(p => p.Conditions != StatusConditions.KNOCKED
                                && p.Name != Pokemon.Name).ToList();

                if (availablePokemons.Count > 0)
                {
                    int index = DiceRollService.RollDice(1, availablePokemons.Count - 1);
                    var newPokemon = availablePokemons[index];
                    BattleLog.AddLog($"Active Pokémon {Pokemon.Name} is changed to " + newPokemon.Name);
                    Console.ReadLine();
                    return newPokemon;
                }
            }
            return Pokemon;
        }
        public override string ToString()
        {
            string buffs = string.Empty;
            if (BuffsAndDebuffs.Count > 0)
            {
                foreach (var buff in BuffsAndDebuffs)
                {
                    buffs += buff.ToString() + " ";
                }
                buffs = "[" + buffs.Substring(0, buffs.Length - 1) + "]";
            }
            string status = string.Empty;
            if (Pokemon.Conditions != StatusConditions.NORMAL) status = $"[{Pokemon.Conditions.ToString()}] ";
            return $"{status}{buffs}{Trainer.Name}'s {Pokemon.GetName()}";
        }
    }
}
