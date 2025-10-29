
using ProjetoPokemon.Entities.Enums;
using ProjetoPokemon.Entities.Services;

namespace ProjetoPokemon.Entities.Profiles
{
    enum SetupBattle
    {
        Attacker,
        Defender,
    }
    class BattlerManager
    {
        public string? TrainerName { get; }
        public BoxPokemon? TrainerBox { get; }
        public ProfilePokemon SelectedPokemon { get; set; }
        public SetupBattle Setup { get; set; }
        public Move UsedMove { get; set; } = new Move(0, TypePokemon.None, "No move", 0, 6);
        public ItemCard? UsedCard { get; set; }

        // modifications in battle
        public int NumberDices { get; set; }
        public int RollBonus { get; set; }
        public int EffectiveBonus { get; set; }
        public int Roll { get; set; }
        public int CardBonus { get; set; }
        public int StatusBonus { get; set; }
        public int WeatherBonus { get; set; }
        public int TotalResult { get; set; }

        public BattlerManager(string? trainerName, BoxPokemon? trainerBox, ProfilePokemon selectedPokemon, SetupBattle setup)
        {
            TrainerName = trainerName;
            TrainerBox = trainerBox;
            SelectedPokemon = selectedPokemon;
            Setup = setup;
        }

        public BattlerManager(ProfilePokemon selectedPokemon, SetupBattle setup)
        {
            SelectedPokemon = selectedPokemon;
            Setup = setup;
        }
        public static BattlerManager CreateBattlerTrainer(BoxPokemon trainer, ConsoleColor color, SetupBattle setup)
        {
            return new BattlerManager(trainer.Nickname, trainer, trainer.SelectPokemon(color), setup);
        }
        public void CalculateTotalPower()
        {
            TotalResult = (RollBonus + Roll + EffectiveBonus + UsedMove.Power) + SelectedPokemon.LevelPokemon() + StatusBonus + CardBonus + WeatherBonus;
        }
        public void DisplayBattleStatus()
        {
            BattleLog.AddLog($"\n{ToString()} used {UsedMove.Name} with total of {TotalResult}.");
            string logCalculation = $"lv: {SelectedPokemon.LevelPokemon()}, roll: {Roll}";
            if (RollBonus != 0) logCalculation += $"+{RollBonus}";
            if (EffectiveBonus != 0)
            {
                if (EffectiveBonus > 0) logCalculation += $" (+{EffectiveBonus} super effective)";
                else logCalculation += $" ({EffectiveBonus} not effective)";
            }

            logCalculation += $", power: {UsedMove.Power}";

            if (StatusBonus != 0) logCalculation += $", status: {StatusBonus}";
            if (CardBonus != 0) logCalculation += $", card: {CardBonus}";
            if (WeatherBonus != 0) logCalculation += $", weather: {WeatherBonus}";

            BattleLog.AddLog(logCalculation);
        }
        public override string ToString()
        {
            string nickname = SelectedPokemon.Name;
            if (nickname != SelectedPokemon.Pokemon.Name)
            { 
                nickname += $" ({SelectedPokemon.Pokemon.Name})";
            }
            return $"{TrainerName}'s {nickname}";
        }
    }
}
