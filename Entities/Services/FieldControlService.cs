using ProjetoPokemon.Entities.Enums;
using ProjetoPokemon.Entities.Profiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoPokemon.Entities.Services
{
    static class FieldControlService
    {
        private static FieldCards weatherCard = FieldCards.NORMAL;
        private static FieldCards fieldCard = FieldCards.NORMAL;
        private static FieldCards trapCard = FieldCards.NORMAL;
    
        public static int WeatherBonus(BattlerManager pokemon)
        {
            TypePokemon type = pokemon.SelectedPokemon.Pokemon.Type;
            switch (weatherCard)
            {
                case FieldCards.RAIN:
                    BattleLog.AddLog("It's raining heavily!!!");
                    if (pokemon.UsedMove.MoveID == 96 || pokemon.UsedMove.MoveID == 131)
                    { pokemon.UsedMove.Effects.Add(new EffectManager(EffectType.PRECISION, 0)); BattleLog.AddLog(pokemon.UsedMove.Name + " now has Precision."); }
                    if (pokemon.UsedMove.Type == TypePokemon.Water) return 1;
                    else if (pokemon.UsedMove.Type == TypePokemon.Fire) return -1;
                    return 0;

                case FieldCards.SANDSTORM:
                    BattleLog.AddLog("We're in a sandstorm!!!");
                    if (type != TypePokemon.Rock &&
                        type != TypePokemon.Steel &&
                        type != TypePokemon.Ground) return -1;
                    return 0;

                case FieldCards.SUNNYDAY:
                    BattleLog.AddLog("The sun is shining brightly!!!");
                    if (pokemon.SelectedPokemon.Conditions == StatusConditions.FROZEN)
                    { pokemon.SelectedPokemon.Conditions = StatusConditions.NORMAL; BattleLog.AddLog(pokemon + " has been unfrozen and can attack!"); }
                    if (pokemon.UsedMove.Type == TypePokemon.Water) return -1;
                    else if (pokemon.UsedMove.Type == TypePokemon.Fire) return 1;
                    return 0;

                case FieldCards.SNOW:
                    BattleLog.AddLog("It's snowing heavily!!!");
                    if (pokemon.UsedMove.MoveID == 154)
                    { pokemon.UsedMove.Effects.Add(new EffectManager(EffectType.PRECISION, 0)); BattleLog.AddLog(pokemon.UsedMove.Name + " now has Precision."); }
                    if (type != TypePokemon.Ice) return -1;
                    return 0;
                default:
                    return 0;
            }

        }
        public static void ChangeWeather(FieldCards weather)
        {
            weatherCard = weather;
        }
        public static void ChangeField(FieldCards field)
        {
            fieldCard = field;
        }
        public static void ChangeTrap(FieldCards trap)
        {
            trapCard = trap;
        }
        public static List<FieldCards> SaveFieldConfig()
        {
            List<FieldCards> saveField = new List<FieldCards>();
            saveField.Add(fieldCard);
            saveField.Add(trapCard);
            saveField.Add(weatherCard);

            return saveField;
        }
        public static void LoadFieldConfig(List<FieldCards> list)
        {
            fieldCard = list[0];
            trapCard = list[1];
            weatherCard = list[2];
        }
    }
    
}
