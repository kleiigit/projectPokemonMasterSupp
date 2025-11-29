
using ProjetoPokemon.Enums;
using ProjetoPokemon.Services;

namespace ProjetoPokemon.Entities.Battlers
{
    internal class BattlerPlayer : Battler // player
    {
        public string? TrainerName { get; }
        public BoxPokemon TrainerBox { get; }
        public ItemCard? AttachCard { get; set; }

        public BattlerPlayer(SetupBattle setup, BoxPokemon trainerBox, ProfilePokemon profile)
            : base(profile, setup)
        {
            TrainerName = trainerBox.Nickname;
            TrainerBox = trainerBox;
            AttachCard = profile.AttachCard;
        }
        public override Move SelectMove(Battler trainerB) // player select
        {
            List<Move> listMoves = GetMoveList();

            if (Pokemon.CanAttack == false || listMoves.Count == 0)
                return Move.Null();

            // Calcula a taxa de vitória antes de exibir no menu
            foreach (Move move in listMoves) move.RateWin(VictoryChanceService.ChanceSimulator(move, this, trainerB));

            int moveIndex = ConsoleMenu.ShowMenu(ConsoleColor.Yellow,
                listMoves.Select(m => m.MoveMenu()).ToList(), $"Choose move: {ToString()} Lv.{Pokemon.LevelPokemon()} ({Pokemon.Pokemon.Type.ToString().ToUpper()}-type) \nVS " +
                $"{trainerB} Lv.{trainerB.Pokemon.LevelPokemon()} ({trainerB.Pokemon.Pokemon.ToString().ToUpper()}-type)");

            Move moveSelected = listMoves[moveIndex];
            return moveSelected;
        }
        public override ProfilePokemon ChangeRandomPokemon()
        {
            if (TrainerBox != null)
            {
                var availablePokemons = TrainerBox.ListPokemon
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
        public static BattlerPlayer CreateTrainer(BoxPokemon trainer, ConsoleColor colorMenu, SetupBattle setup)
        {
            return new BattlerPlayer(setup, trainer, trainer.SelectPokemon(colorMenu));
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
            return $"{status}{buffs}{TrainerName}'s {Pokemon.GetName()}";
        }
    }
}
