
using ProjetoPokemon.Enums;

namespace ProjetoPokemon.Services
{
    internal static class EffectiveTypeService
    {
        private static readonly Dictionary<(TypePokemon, TypePokemon), int> EffectivenessTable = new();
        static EffectiveTypeService()
        {
            //Normal
            TypeCompare(TypePokemon.Normal, -2, TypePokemon.Steel, TypePokemon.Ghost, TypePokemon.Rock);

            // Grass
            TypeCompare(TypePokemon.Grass, -2,
                TypePokemon.Steel, TypePokemon.Flying, TypePokemon.Dragon,
                TypePokemon.Fire, TypePokemon.Bug, TypePokemon.Poison, TypePokemon.Grass);
            TypeCompare(TypePokemon.Grass, +2,
                TypePokemon.Ground, TypePokemon.Water, TypePokemon.Rock);

            // Fire
            TypeCompare(TypePokemon.Fire, -2,
                TypePokemon.Dragon, TypePokemon.Water,TypePokemon.Fire, TypePokemon.Rock);
            TypeCompare(TypePokemon.Fire, +2,
                TypePokemon.Ice, TypePokemon.Grass, TypePokemon.Bug, TypePokemon.Steel);

            // Water
            TypeCompare(TypePokemon.Water, -2,
                TypePokemon.Dragon, TypePokemon.Water, TypePokemon.Grass);
            TypeCompare(TypePokemon.Water, +2,
                TypePokemon.Ground, TypePokemon.Rock, TypePokemon.Fire);

            // Fighting
            TypeCompare(TypePokemon.Fighting, -2,
                TypePokemon.Psychic, TypePokemon.Bug, TypePokemon.Poison,
                TypePokemon.Flying, TypePokemon.Ghost, TypePokemon.Fairy);
            TypeCompare(TypePokemon.Fighting, +2,
                TypePokemon.Normal, TypePokemon.Rock, TypePokemon.Ice, TypePokemon.Dark, TypePokemon.Steel);
            
            // Flying
            TypeCompare(TypePokemon.Flying, -2,
                TypePokemon.Steel, TypePokemon.Rock, TypePokemon.Electric);
            TypeCompare(TypePokemon.Flying, +2,
                TypePokemon.Fighting, TypePokemon.Bug, TypePokemon.Grass);

            // Poison
            TypeCompare(TypePokemon.Poison, -2,
                TypePokemon.Steel, TypePokemon.Ghost, TypePokemon.Rock, TypePokemon.Ground, TypePokemon.Poison);
            TypeCompare(TypePokemon.Poison, +2,
                TypePokemon.Grass, TypePokemon.Fairy);

            // Ground
            TypeCompare(TypePokemon.Ground, -2,
                TypePokemon.Grass, TypePokemon.Bug, TypePokemon.Flying);
            TypeCompare(TypePokemon.Ground, +2,
                TypePokemon.Poison, TypePokemon.Rock, TypePokemon.Electric, TypePokemon.Fire, TypePokemon.Steel);

            // Rock
            TypeCompare(TypePokemon.Rock, -2,
                TypePokemon.Steel, TypePokemon.Ground, TypePokemon.Fighting);
            TypeCompare(TypePokemon.Rock, +2,
                TypePokemon.Flying, TypePokemon.Bug, TypePokemon.Fire, TypePokemon.Ice);

            // Bug
            TypeCompare(TypePokemon.Bug, -2,
                TypePokemon.Steel, TypePokemon.Ghost, TypePokemon.Flying,
                TypePokemon.Fighting, TypePokemon.Poison, TypePokemon.Fire, TypePokemon.Fairy);
            TypeCompare(TypePokemon.Bug, +2,
                TypePokemon.Grass, TypePokemon.Psychic, TypePokemon.Dark);

            // Ghost
            TypeCompare(TypePokemon.Ghost, -2,
                TypePokemon.Dark, TypePokemon.Normal);
            TypeCompare(TypePokemon.Ghost, +2,
                TypePokemon.Ghost, TypePokemon.Psychic);

            // Electric
            TypeCompare(TypePokemon.Electric, -2,
                TypePokemon.Dragon, TypePokemon.Electric, TypePokemon.Grass, TypePokemon.Ground);
            TypeCompare(TypePokemon.Electric, +2,
                TypePokemon.Flying, TypePokemon.Water);

            // Psychic
            TypeCompare(TypePokemon.Psychic, -2,
                TypePokemon.Dark, TypePokemon.Steel, TypePokemon.Psychic);
            TypeCompare(TypePokemon.Psychic, +2,
                TypePokemon.Fighting, TypePokemon.Poison);

            // Ice
            TypeCompare(TypePokemon.Ice, -2,
                TypePokemon.Steel, TypePokemon.Ice, TypePokemon.Water, TypePokemon.Fire);
            TypeCompare(TypePokemon.Ice, +2,
                TypePokemon.Flying, TypePokemon.Ground, TypePokemon.Grass, TypePokemon.Dragon);

            // Dragon
            TypeCompare(TypePokemon.Dragon, -2,
                TypePokemon.Fairy, TypePokemon.Steel);
            TypeCompare(TypePokemon.Dragon, +2,
                TypePokemon.Dragon);

            // Dark
            TypeCompare(TypePokemon.Dark, -2,
                TypePokemon.Fairy, TypePokemon.Dark, TypePokemon.Fighting);
            TypeCompare(TypePokemon.Dark, +2,
                TypePokemon.Ghost, TypePokemon.Psychic);

            // Steel
            TypeCompare(TypePokemon.Steel, -2,
                TypePokemon.Steel, TypePokemon.Electric, TypePokemon.Water, TypePokemon.Fire);
            TypeCompare(TypePokemon.Steel, +2,
                TypePokemon.Rock, TypePokemon.Ice, TypePokemon.Fairy);

            // Fairy
            TypeCompare(TypePokemon.Fairy, -2,
                TypePokemon.Steel, TypePokemon.Poison, TypePokemon.Fire);
            TypeCompare(TypePokemon.Fairy, +2,
                TypePokemon.Fighting, TypePokemon.Dragon, TypePokemon.Dark);

        }

        private static void TypeCompare(TypePokemon attacker, int modifier, params TypePokemon[] defenders)
        {
            foreach (var defender in defenders)
                EffectivenessTable[(attacker, defender)] = modifier;
        }
        public static int GetTypeModifier(TypePokemon attacker, TypePokemon defender)
        {
            return EffectivenessTable.TryGetValue((attacker, defender), out int modifier) ? modifier : 0;
        }
    }
}
