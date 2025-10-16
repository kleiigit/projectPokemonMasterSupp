using AuxiliarCampanha.Entities.Services;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Wordprocessing;
using ProjetoPokemon.Entities.Enums;
using ProjetoPokemon.Services;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;

namespace ProjetoPokemon.Entities.Services
{
    static class BattleSimService // classe que controla as simulações de combate
    {
        static int attackerDicesRoll = 0;
        static int defenderDicesRoll = 0;
        public static void BattleControl(Pokemon attacker, Pokemon defender)
        {
            int totalAttacker = 0;
            int totalDefender = 0;
            Move attackerMove = attacker.SelectMove();
            Move defenderMove = defender.SelectMove();

            // Efeitos de cartas deveriam ser colocados aqui.

            EffectMove(attackerMove, true);
            EffectMove(defenderMove, false);
            // Effeito do move é ativado aqui

            while(totalDefender == totalAttacker)
            {
                int rollA = RollCombatDices(attackerMove, attackerDicesRoll);
                int effvA = EffectiveTypeService.GetTypeModifier(attackerMove.Type, defender.Type);
                int rollB = RollCombatDices(defenderMove, defenderDicesRoll);
                int effvB = EffectiveTypeService.GetTypeModifier(defenderMove.Type, attacker.Type);

                totalAttacker = (rollA + effvA) + attackerMove.Power + attacker.LevelBase; // falta outros modificadores
                Console.WriteLine($"{attacker.Name} usou {attackerMove.Name} com total de " + totalAttacker);
                Console.WriteLine($"lv: {attacker.LevelBase}, roll ({rollA}/{effvA}), power: {attackerMove.Power}");

                totalDefender = (rollB + effvB) + defenderMove.Power + defender.LevelBase; // falta outros modificadores
                Console.WriteLine($"{defender.Name} usou {defenderMove.Name} com total de " + totalDefender);
                Console.WriteLine($"lv: {defender.LevelBase}, roll ({rollB}/{effvB}), power: {defenderMove.Power}");

                if (totalAttacker == totalDefender)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Empate! Role novamente os dados.\n");
                    Console.ResetColor();
                    Console.ReadLine();
                }
                else
                {
                    string victoryPokemon = totalAttacker > totalDefender ? "Atacante " + attacker.Name : "Defensor " + defender.Name;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(victoryPokemon + " venceu a batalha!");
                    Console.ResetColor();
                }
            }
            

        }
        private static int RollCombatDices(Move move, int moveDicesRoll)
        {
            // Se for 0 → rola 1 dado
            // Se for positivo → rola 2 dados e pega o MAIOR
            // Se for negativo → rola 2 dados e pega o MENOR

            int diceCount = (moveDicesRoll == 0) ? 1 : 2;
            int[] rolls = new int[diceCount];

            for (int i = 0; i < diceCount; i++)
            {
                int roll = DiceRollService.RollDice(1, move.DiceSides);

                // Corrige valores fora do limite (7 → 5, 8 → 6)
                if (roll == 7) roll = 5;
                else if (roll == 8) roll = 6;

                rolls[i] = roll;
                Console.WriteLine($"{move.Name} - Rolagem {i + 1}: {roll}");
            }

            int result;

            if (moveDicesRoll < 0)
                result = rolls.Min(); // Desvantagem → pega o menor
            else if (moveDicesRoll > 0)
                result = rolls.Max(); // Vantagem → pega o maior
            else
                result = rolls[0]; // Normal → único dado

            return result;
        }

        private static void EffectMove(Move userMove, bool isAttacker)
        {
            if (userMove.Effects == null)
            {
                return;
            }
            int redDice = DiceRollService.RollD6();
            if (userMove.EffectRoll > 1) Console.WriteLine($"Efeito de {userMove.Name}: ({redDice}/{userMove.EffectRoll})");
            if (redDice >= userMove.EffectRoll)
            {
                
                foreach (var effect in userMove.Effects)
                    {
                  
                        if (effect.EffectType == EffectType.TWODICES)
                        {
                       
                            // Determina quem é o alvo com base em TargetEffect e no contexto do atacante
                            if (effect.TargetEffect == 'B')
                            {
                                if (isAttacker)
                                {
                                    defenderDicesRoll -= 1;
                                   
                                }
                                else
                                {
                                    attackerDicesRoll -= 1;
                                    
                                }
                            }
                            else if (effect.TargetEffect == 'W')
                            {
                                if (isAttacker)
                                {
                                    attackerDicesRoll += 1;
                                    
                                }
                                else
                                {
                                    defenderDicesRoll += 1;
                                    
                                }
                            }
                        }
                    }
                
            }
        }

    }
}
