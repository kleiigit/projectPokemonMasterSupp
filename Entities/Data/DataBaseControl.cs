using System.Reflection;
using ClosedXML.Excel;
using ProjetoPokemon.Entities;
using ProjetoPokemon.Entities.Enums;

namespace ProjetoPokemon
{
    internal static class DataBaseControl //Faz o controle dos arquivos xlsx
    {
        private static readonly string pathData;
        private static readonly string movesPath;
        private static readonly string pokemonPath;

        // Bloco estático executado apenas uma vez quando a classe é usada
        static DataBaseControl()
        {
            string exeDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            string relativePath = Path.Combine(exeDirectory, @"..\..\..\Entities\Data");
            pathData = Path.GetFullPath(relativePath);

            movesPath = Path.Combine(pathData, "MovesPokemon.xlsx");
            pokemonPath = Path.Combine(pathData, "Pokemon.xlsx");
        }

        public static void DataBase(List<Pokemon> pokemons, List<Move> movesList)
        {
            // --- CARREGA MOVES ---
            using (var workbook = new XLWorkbook(movesPath))
            {
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RangeUsed().RowsUsed().Skip(1);

                foreach (var row in rows)
                {
                    int moveID = row.Cell(1).GetValue<int>();
                    string moveNameEng = row.Cell(2).GetString();
                    string moveNamePtBr = row.Cell(8).GetString();
                    string moveNameEsp = row.Cell(9).GetString();
                    string moveNameJp = row.Cell(10).GetString();

                    int power = row.Cell(3).GetValue<int>();
              
                    int diceSides = 0;
                    if (!string.IsNullOrEmpty(row.Cell(5).GetString()) && int.TryParse(row.Cell(5).GetString(), out int resultdice))
                        diceSides = resultdice;

                    string typeStr = row.Cell(4).GetString();
                    typeStr = char.ToUpper(typeStr[0]) + typeStr.Substring(1).ToLower();
                    TypePokemon type = Enum.Parse<TypePokemon>(typeStr);

                    List<EffectMove> effectMoves = new();
                    int efRoll = 0;
                    if (!string.IsNullOrEmpty(row.Cell(7).GetString()) && int.TryParse(row.Cell(7).GetString(), out int result))
                        efRoll = result;

                    string effectMove = row.Cell(6).GetString();
                    if (!string.IsNullOrEmpty(effectMove))
                    {
                        string[] effectVet = effectMove.Split(';');
                        foreach (string effect in effectVet)
                        {
                            string[] parts = effect.Split('.');
                            if (parts.Length < 2)
                            {
                                effectMoves.Add(new EffectMove(parts[0]));
                                continue;
                            }

                            char targetEffect = char.Parse(parts[0].Trim());
                            string effectSetup = parts[1].Trim();
                            effectMoves.Add(new EffectMove(targetEffect, effectSetup));
                        }
                    }

                    Move move = new Move(moveID, type, moveNameEng, power, effectMoves, diceSides, efRoll);

                    // Exceptions ERRO
                    if (!movesList.Any(p => p.MoveID == move.MoveID))
                    {
                        movesList.Add(move);
                    }
                    else
                    {
                        Console.WriteLine(move.Name + " possui erro em seu ID! \n");
                    }
                    
                    
                }
            }

            // --- CARREGA POKÉMON ---
            using (var workbook = new XLWorkbook(pokemonPath))
            {
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RangeUsed().RowsUsed().Skip(1);

                foreach (var row in rows)
                {
                    int numberID = row.Cell(1).GetValue<int>();
                    string name = row.Cell(2).GetString();

                    string typeStr = row.Cell(3).GetString();
                    typeStr = char.ToUpper(typeStr[0]) + typeStr.Substring(1).ToLower();
                    TypePokemon type = Enum.Parse<TypePokemon>(typeStr);

                    string stabStr = row.Cell(4).GetString();
                    TypePokemon stabType = string.IsNullOrEmpty(stabStr)
                        ? TypePokemon.None
                        : Enum.Parse<TypePokemon>(char.ToUpper(stabStr[0]) + stabStr.Substring(1).ToLower());

                    int stage = row.Cell(5).GetValue<int>();
                    int toEvolveID = string.IsNullOrEmpty(row.Cell(6).GetString()) ? 0 : row.Cell(6).GetValue<int>();
                    int levelBase = row.Cell(7).GetValue<int>();
                    int expToEvolve = (toEvolveID != 0) ? row.Cell(8).GetValue<int>() : 0;
                    int generation = row.Cell(9).GetValue<int>();
                    ColorToken color = Enum.Parse<ColorToken>(row.Cell(10).GetString());
                    TokenBackGround background = Enum.Parse<TokenBackGround>(row.Cell(11).GetString());
                    bool shiny = false;
                    string? form = string.IsNullOrEmpty(row.Cell(16).GetString()) ? null : row.Cell(16).GetString();

                    List<Move> pokeMoves = new();
                    string moveString = row.Cell(13).GetString();
                    string[] moveVet = moveString.Split(';');

                    foreach (var move in moveVet)
                    {
                        if (int.TryParse(move.Trim(), out int moveID))
                        {
                            Move? moveFound = movesList.Find(m => m.MoveID == moveID);
                            if (moveFound != null)
                                pokeMoves.Add(moveFound);
                        }
                    }

                    Pokemon pokemon = new Pokemon(
                        numberID, name, type, stabType, stage, toEvolveID, form,
                        levelBase, expToEvolve, generation,
                        color, background, new List<Ability>(), shiny, pokeMoves);

                    // Exceptions ERRO
                    if (!pokemons.Any(p => p.NumberID == pokemon.NumberID))
                    {
                        pokemons.Add(pokemon);
                    }
                    else
                    {
                        Console.WriteLine(pokemon.Name + " possui erro em seu ID! \n");
                    }
                }
            }
        }
    }
}
