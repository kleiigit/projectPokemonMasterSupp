using ProjetoPokemon.Entities.Enums;
using ProjetoPokemon.Entities.Services;

namespace ProjetoPokemon.Entities
{
    enum SetupBattle
    {
        Attacker,
        Defender,
    }
    class BattleModifications
    {
        public string? TrainerName { get; set; }
        public BoxPokemon TrainerBox { get; set; }
        public SetupBattle Setup { get; set; }
        public ProfilePokemon SelectedPokemon { get; set; }
        public Move UsedMove { get; set; } = new Move(0, TypePokemon.None, "No move", 0, 6);
        public ItemCard? UsedCard { get; set; }

        // modifications in battle
        public int NumberDices { get; set; }
        public int RollBonus { get; set; }
        public int EffectiveBonus { get; set; }
        public int Roll { get; set; }
        public int CardBonus { get; set; }
        public int StatusBonus { get; set; }
        public int TotalResult { get; set; }

        public BattleModifications(BoxPokemon trainerBox, SetupBattle setup, ProfilePokemon pokemon)
        {
            TrainerName = trainerBox.Nickname;
            TrainerBox = trainerBox;
            Setup = setup;
            SelectedPokemon = pokemon;
        }

        public void CalculateTotalPower()
        {
            TotalResult = Roll + EffectiveBonus + UsedMove.Power + SelectedPokemon.LevelPokemon() + StatusBonus + CardBonus;
        }
        public void DisplayBattleStatus()
        {
            BattleLog.AddLog($"\n{ToString()} used {UsedMove.Name} with total of {TotalResult}.");
            string logCalculation = $"lv: {SelectedPokemon.LevelPokemon()}, roll: {Roll}";
            if (EffectiveBonus != 0)
            {
                if (EffectiveBonus > 0) logCalculation += $" (+{EffectiveBonus} super effective)";
                else logCalculation += $" ({EffectiveBonus} not effective)";
            }

            logCalculation += $", power: {UsedMove.Power}";

            if (StatusBonus != 0) logCalculation += $", status: {StatusBonus}";
            if (CardBonus != 0) logCalculation += $", card: {CardBonus}";

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
