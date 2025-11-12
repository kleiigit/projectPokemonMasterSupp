
using ProjetoPokemon.Enums;
using ProjetoPokemon.Services;

namespace ProjetoPokemon.Entities
{
    enum SetupBattle
    {
        Attacker,
        Defender,
    }
    class BattlerManager
    {
        public string? TrainerName { get; private set; }
        public BoxPokemon? TrainerBox { get; }
        public ProfilePokemon SelectedPokemon { get; set; }
        public SetupBattle Setup { get; set; }
        public Move UsedMove { get; private set; } = new Move(0, TypePokemon.None, "No move", 0, 6);
        public ItemCard? UsedCard { get; set; }
        public List<EffectType> BuffsAndDebuffs { get; set; } = new List<EffectType>();
        public Move? LastMove { get; set; } // torment setup effect

        // temp status
        public ItemCard? AttachCard { get; set; }

        // modifications in battle
        public int NumberDices { get; set; }
        public int RollBonus { get; set; }
        public int EffectiveBonus { get; set; }
        public int Roll { get; set; }
        public int CardBonus { get; set; }
        public int StatusBonus { get; set; }
        public int WeatherBonus { get; set; }
        public int TotalResult { get; set; }

        public BattlerManager(string? trainerName, BoxPokemon? trainerBox, ProfilePokemon selectedPokemon, SetupBattle setup) // trainer setup
        {
            TrainerName = trainerName;
            TrainerBox = trainerBox;
            SelectedPokemon = selectedPokemon;
            AttachCard = selectedPokemon.AttachCard;
            Setup = setup;
        }
        public BattlerManager(ProfilePokemon selectedPokemon, SetupBattle setup) // wild pokemon
        {
            SelectedPokemon = selectedPokemon;
            AttachCard = selectedPokemon.AttachCard;
            Setup = setup;
        }
        public BattlerManager(Pokemon selectedPokemon, SetupBattle setup) // No ProfilePokemon
        {
            SelectedPokemon = new ProfilePokemon(selectedPokemon, selectedPokemon.Name, 0);
            AttachCard = SelectedPokemon.AttachCard;
            Setup = setup;
        }

        public void TypeSelectMove(Move? autoMove, BattlerManager target)
        {
            if (autoMove == null)
            {
                if (TrainerName == null || TrainerName == "TESTE")
                {
                    SelectMove(RandomMovePokemon());
                    BattleLog.AddLog($"=> {this} used {UsedMove.Name}.\n" + UsedMove + "\n");
                }
                else
                {
                    SelectMove(SelectMovePokemon(target));
                    BattleLog.AddLog($"=> {this} used {UsedMove.Name}.\n" + UsedMove + "\n");
                }
            }
            else UsedMove = autoMove;
        }
        public void SetPower(int n)
        {
            UsedMove.ChangePower(n);
        }
        public void SelectMove(Move? move)
        {
            if(move == null) UsedMove = new Move(0, TypePokemon.None, "No move", 0, 6);
            else UsedMove = move.Copy();
        }

        public Move RandomMovePokemon()
        {
            List<Move> listMoves = SelectedPokemon.GetMoveList(this);

            if (listMoves.Count == 0)
                return new Move(0, TypePokemon.None, "No Move", 0, 6);

            Random rnd = new Random();
            return listMoves[rnd.Next(listMoves.Count)];
        }
        public Move SelectMovePokemon(BattlerManager trainerB)
        {
            List<Move> listMoves = SelectedPokemon.GetMoveList(this);

            if (listMoves.Count == 0)
                return new Move(0, TypePokemon.None, "No Move", 0, 6);

            // Calcula a taxa de vitória antes de exibir no menu
            foreach (Move move in listMoves) move.RateWin(VictoryChanceService.ChanceSimulator(move, this, trainerB));

            int moveIndex = ConsoleMenu.ShowMenu(ConsoleColor.Yellow,
                listMoves.Select(m => m.MoveMenu()).ToList(), $"Choose move: {ToString()} Lv.{SelectedPokemon.LevelPokemon()} ({SelectedPokemon.Pokemon.Type.ToString().ToUpper()}-type) \nVS " +
                $"{trainerB} Lv.{trainerB.SelectedPokemon.LevelPokemon()} ({trainerB.SelectedPokemon.Pokemon.ToString().ToUpper()}-type)");

            Move moveSelected = listMoves[moveIndex];

            return moveSelected;
        }
        public Move AutoSelectMovePokemon(BattlerManager trainerB)
        {
            List<Move> listMoves = SelectedPokemon.GetMoveList(this);

            if (listMoves.Count == 0)
                return new Move(0, TypePokemon.None, "No Move", 0, 6);

            foreach (Move move in listMoves) move.RateWin(VictoryChanceService.ChanceSimulator(move, this, trainerB));


            return listMoves.OrderByDescending(p => p.Rate).First();

        }
        public void SetTrainerName(string name)
        {
            TrainerName = name;
        }
        public ProfilePokemon ChangeRandomPokemon()
        {
            if (TrainerBox != null)
            {
                var availablePokemons = TrainerBox.ListPokemon
                    .Where(p => p.Conditions != StatusConditions.KNOCKED
                                && p.NickPokemon != SelectedPokemon.NickPokemon).ToList();

                if (availablePokemons.Count > 0)
                {
                    int index = DiceRollService.RollDice(1, availablePokemons.Count - 1);
                    var newPokemon = availablePokemons[index];
                    BattleLog.AddLog($"Active Pokémon {SelectedPokemon.NickPokemon} is changed to " + newPokemon.NickPokemon);
                    Console.ReadLine();
                    return newPokemon;
                }
            }
            return SelectedPokemon;
        }
        public static BattlerManager CreateBattlerTrainer(BoxPokemon trainer, ConsoleColor color, SetupBattle setup)
        {
            return new BattlerManager(trainer.Nickname, trainer, trainer.SelectPokemon(color), setup);
        }
        public bool HasEffectType(EffectType effectType)
        {
            return BuffsAndDebuffs != null && BuffsAndDebuffs.Contains(effectType);
        }
        public void CalculateTotalPower()
        {
            TotalResult = (RollBonus + Roll) + EffectiveBonus + UsedMove.Power + SelectedPokemon.LevelPokemon() + StatusBonus + CardBonus + WeatherBonus;
        }
        public void DisplayBattleStatus()
        {
            BattleLog.AddLog($"\n{ToString()} used {UsedMove.Name} with total of {TotalResult}.");
            string logCalculation = $"{Roll} (Attack Roll) + {UsedMove.Power} (Attack Strength) + {SelectedPokemon.LevelPokemon()}  (Level)";
            if (RollBonus != 0) logCalculation += $" + {RollBonus} (Bonus)";
            if (EffectiveBonus != 0)
            {
                if (EffectiveBonus > 0) logCalculation += $" + {EffectiveBonus} (Effective Move)";
                else logCalculation += $" {EffectiveBonus} (Weak Move)";
            }

            logCalculation += $" ";

            if (StatusBonus != 0) logCalculation += $" + {StatusBonus} (Status)";
            if (CardBonus != 0) logCalculation += $" + {CardBonus} (Battle Card)";
            if (WeatherBonus != 0) logCalculation += $" + {WeatherBonus} (Weather))";

            BattleLog.AddLog(logCalculation);
        }
        public override string ToString()
        {
            string buffs = string.Empty;
            if(BuffsAndDebuffs.Count > 0)
            {
                foreach (var buff in BuffsAndDebuffs)
                {
                    buffs += buff.ToString() + " ";
                }
                buffs = "[" + buffs.Substring(0, buffs.Length - 1) + "]";
            }
            string status = string.Empty;
            if(SelectedPokemon.Conditions != StatusConditions.NORMAL) status = $"[{SelectedPokemon.Conditions.ToString()}] ";
            return $"{status}{buffs}{TrainerName}'s {SelectedPokemon.NicknamePokemon()}";
        }
    }
}
