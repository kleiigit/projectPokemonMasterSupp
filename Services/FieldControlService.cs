using ProjetoPokemon.Entities;
using ProjetoPokemon.Entities.Battlers;
using ProjetoPokemon.Enums;

namespace ProjetoPokemon.Services
{
    static class FieldControlService
    {
        private static FieldCards weatherCard = FieldCards.NORMAL;
        private static FieldCards fieldCard = FieldCards.NORMAL;
        private static FieldCards trapCard = FieldCards.NORMAL;
    
        public static int WeatherBonus(Battler pokemon)
        {
            TypePokemon type = pokemon.Pokemon.Pokemon.Type;
            switch (weatherCard)
            {
                case FieldCards.RAIN:
                    BattleLog.AddLog("It's raining heavily!!!");
                    if (pokemon.GetUsedMoveID() == 96 || pokemon.GetUsedMoveID() == 131)
                    { pokemon.SetEffect(new EffectManager(EffectType.PRECISION, 0)); BattleLog.AddLog(pokemon.GetUsedMoveName() + " now has Precision."); }
                    if (pokemon.GetUsedMoveType() == TypePokemon.Water) return 1;
                    else if (pokemon.GetUsedMoveType() == TypePokemon.Fire) return -1;
                    return 0;

                case FieldCards.SANDSTORM:
                    BattleLog.AddLog("We're in a sandstorm!!!");
                    if (type != TypePokemon.Rock &&
                        type != TypePokemon.Steel &&
                        type != TypePokemon.Ground) return -1;
                    return 0;

                case FieldCards.SUNNYDAY:
                    BattleLog.AddLog("The sun is shining brightly!!!");
                    if (pokemon.Pokemon.Conditions == StatusConditions.FROZEN)
                    { pokemon.Pokemon.Conditions = StatusConditions.NORMAL; BattleLog.AddLog(pokemon + " has been unfrozen and can attack!"); }
                    if (pokemon.GetUsedMoveType() == TypePokemon.Water) return -1;
                    else if (pokemon.GetUsedMoveType() == TypePokemon.Fire) return 1;
                    return 0;

                case FieldCards.SNOW:
                    BattleLog.AddLog("It's snowing heavily!!!");
                    if (pokemon.GetUsedMoveID() == 154)
                    { pokemon.SetEffect(new EffectManager(EffectType.PRECISION, 0)); BattleLog.AddLog(pokemon.GetUsedMoveName() + " now has Precision."); }
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
