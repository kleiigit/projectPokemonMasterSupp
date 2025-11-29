
using ProjetoPokemon.Enums;
using ProjetoPokemon.Services;

namespace ProjetoPokemon.Entities.Battlers
{
    internal class BattlerTest : Battler
    {
        public int PointTier { get; set; } = 500;
        public Dictionary<Pokemon, double[]> winPokemon = new Dictionary<Pokemon, double[]>();
        public Dictionary<Pokemon, double[]> losePokemon = new Dictionary<Pokemon, double[]>();
        public Dictionary<Pokemon, double[]> tiePokemon = new Dictionary<Pokemon, double[]>();
        public BattlerTest(ProfilePokemon profile, SetupBattle setup) : base(profile, setup)
        {
        }
        public static Battler CloneBattler(Battler original)
        {
            // Clona o ProfilePokemon para evitar referência compartilhada
            Battler clone = new BattlerTest(original.Pokemon.Clone(), original.Setup);
            clone.Total = new TotalCalculationService
            {
                NumberDices = original.Total.NumberDices,
                RollBonus = original.Total.RollBonus,
                StatusBonus = original.Total.StatusBonus,
                WeatherBonus = original.Total.WeatherBonus
            };
            return clone;
        }

        public override Move SelectMove(Battler target) // IA Trainer move
        {
            List<Move> listMoves = GetMoveList();
            if (Pokemon.CanAttack == false || listMoves.Count == 0)
                return Move.Null();

            foreach (Move move in listMoves) { move.RateWin(VictoryChanceService.ChanceSimulator(move, this, target)); }


            return listMoves.OrderByDescending(p => p.Rate).First();
        }
    }
}
