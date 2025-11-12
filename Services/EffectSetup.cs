using ProjetoPokemon.Entities;
using ProjetoPokemon.Enums;

namespace ProjetoPokemon.Services
{
    static class EffectSetup
    {
        public static EffectManager EffectStance(string effectText, int n)
        {
            try
            {
                string[] parts = effectText.Split('.');
                if (parts.Length < 2) return new EffectManager(Enum.Parse<EffectType>(parts[0].Trim()), n); // apenas o efeito
                if (parts.Length < 3) return new EffectManager(char.Parse(parts[0].Trim()), Enum.Parse<EffectType>(parts[1]), n); // alvo e efeito
                char targetEffect = char.Parse(parts[0].Trim());
                EffectType effectSetup = Enum.Parse<EffectType>(parts[1].Trim());
                string effectCond = "";
                if (!int.TryParse(parts[2], out int bonus)) effectCond = parts[2].Trim(); // pega alguma condicional

                return new EffectManager(targetEffect, effectSetup, bonus, effectCond, n);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao ler efeito: {ex.Message}");
                return null;
            }
        }
    }
}
