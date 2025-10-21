using ProjetoPokemon.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public ProfilePokemon Pokemon { get; set; }
        public Move UsedMove { get; set; } = new Move(0, TypePokemon.None, "Sem ataque", 0, 6);
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
            Pokemon = pokemon;
        }

        public void CalculateTotalPower()
        {
            TotalResult = Roll + EffectiveBonus + UsedMove.Power + Pokemon.LevelPokemon() + StatusBonus + CardBonus;
        }
        public string DisplayBattleStatus()
        {
            string battleLog = $"{ToString()} used {UsedMove.Name} with total of {TotalResult}\n";
            battleLog += $"lv: {Pokemon.LevelPokemon()}, roll: {Roll}";
            if (EffectiveBonus != 0)
            {
                if (EffectiveBonus > 0)
                    battleLog += $" (+{EffectiveBonus} super effective)";
                else
                    battleLog += $" ({EffectiveBonus} not effective)";
            }

            battleLog += $", power: {UsedMove.Power}";
            if (StatusBonus != 0)
            {
                battleLog += $", status: {StatusBonus}";
            }
            if (CardBonus != 0)
            {
                battleLog += $", card: {CardBonus}\n";
            }
            else { battleLog += ".\n"; }
            return battleLog;
        }

        public override string ToString()
        {
            string nickname = Pokemon.Name;
            if (nickname != Pokemon.Pokemon.Name)
            { 
                nickname += $" ({Pokemon.Pokemon.Name})";
            }
            return $"{TrainerName}'s {nickname}";
        }
    }
}
