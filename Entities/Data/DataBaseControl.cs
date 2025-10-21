using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using ProjetoPokemon.Entities;
using ProjetoPokemon.Entities.Enums;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace ProjetoPokemon
{
    internal static class DataBaseControl //Faz o controle dos arquivos xlsx
    {
        private static readonly string pathData;
        private static readonly string movesPath;
        private static readonly string pokemonPath;
        private static readonly string trainerPath;
        private static readonly string itemCardPath;

        // Bloco estático executado apenas uma vez quando a classe é usada
        static DataBaseControl()
        {
            string exeDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            string relativePath = Path.Combine(exeDirectory, @"..\..\..\Entities\Data");
            pathData = Path.GetFullPath(relativePath);

            movesPath = Path.Combine(pathData, "MovesPokemon.xlsx");
            pokemonPath = Path.Combine(pathData, "Pokemon.xlsx");
            trainerPath = Path.Combine(pathData, "ProfileTrainer.txt");
            itemCardPath = Path.Combine(pathData, "ItemCard.xlsx");
        }

        public static void DataBase(List<Pokemon> pokemons, List<Move> movesList, List<ItemCard> cardList, List<BoxPokemon> profiles)
        {
            try
            {
                // --- CARREGA MOVES ---
                using (var workbook = new XLWorkbook(movesPath))
                {
                    var worksheet = workbook.Worksheet(1);
                    var rows = worksheet.RangeUsed().RowsUsed().Skip(1);
                    int[] validDiceSides = { 4, 6, 8, 12, 20, 100 };

                    foreach (var row in rows)
                    {
                        try
                        {
                            if (!int.TryParse(row.Cell(1).GetString(), out int moveID) || moveID == 0) { Console.WriteLine($"Error in line {row.RowNumber()} (Move without ID number!)"); continue; }

                            string moveNameEng = row.Cell(2).GetString();
                            string moveNamePtBr = row.Cell(8).GetString();
                            string moveNameEsp = row.Cell(9).GetString();
                            string moveNameJp = row.Cell(10).GetString();

                            if (string.IsNullOrWhiteSpace(moveNameEng))
                                moveNameEng = "No name";

                            if (!int.TryParse(row.Cell(3).GetString(), out int power) || power < 0)
                            {
                                Console.WriteLine($"Error in Power of Move ID {moveID}, changed to 0.)");
                                power = 0;
                                continue;
                            }

                            // DICE SIDES
                            string diceStr = row.Cell(5).GetString().Trim();
                            int diceSides = 0;
                            if (!string.IsNullOrEmpty(diceStr) && int.TryParse(diceStr, out int resultDice) && validDiceSides.Contains(resultDice))
                            {
                                diceSides = resultDice;
                            }
                            else if (!string.IsNullOrEmpty(diceStr))
                            {
                                Console.WriteLine($"Invalid dice sides '{diceStr}' for Pokémon ID {moveID}. Setting to 0.");
                                diceSides = 0;
                            }

                            // Type Move
                            string typeStr = row.Cell(4).GetString();
                            if (string.IsNullOrWhiteSpace(typeStr))
                            {
                                Console.WriteLine($"Erro type of Move ID {moveID}.");
                                continue;
                            }

                            typeStr = char.ToUpper(typeStr[0]) + typeStr.Substring(1).ToLower();
                            if (!Enum.TryParse(typeStr, out TypePokemon type))
                            {
                                Console.WriteLine($"Erro Move ID '{moveID}' had a type: {typeStr}");
                                continue;
                            }

                            // RED DICE
                            int efRoll = 0;
                            string efRollStr = row.Cell(7).GetString().Trim();
                            if (!string.IsNullOrEmpty(efRollStr) && int.TryParse(efRollStr, out int resultRedDice) && resultRedDice < 7 && resultRedDice >= 0)
                            {
                                efRoll = resultRedDice;
                            }
                            else if (!string.IsNullOrEmpty(efRollStr)) { 
                                Console.WriteLine($"Invalid effect dice sides '{efRollStr}' for Pokémon ID {moveID}. Setting to 0.");
                                efRoll = 0;
                            }

                            List<EffectMove> effectMoves = new();
                            string effectMove = row.Cell(6).GetString().ToUpper();
                            if (!string.IsNullOrEmpty(effectMove))
                            {
                                string[] effectVet = effectMove.Split(';');
                                foreach (string effect in effectVet)
                                {
                                    try
                                    {
                                        string[] parts = effect.Split('.');
                                        if (parts.Length < 2)
                                        {
                                            effectMoves.Add(new EffectMove(Enum.Parse<EffectType>(parts[0].Trim())));
                                            continue;
                                        }

                                        char targetEffect = char.Parse(parts[0].Trim());
                                        EffectType effectSetup = Enum.Parse<EffectType>(parts[1].Trim());
                                        effectMoves.Add(new EffectMove(targetEffect, effectSetup));
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Erro ao ler efeito em MoveID {moveID}: {ex.Message}");
                                    }
                                    
                                }
                            }

                            Move move = new Move(moveID, type, moveNameEng, power, effectMoves, diceSides, efRoll);

                            if (!movesList.Any(p => p.MoveID == move.MoveID))
                                movesList.Add(move);
                            else
                                Console.WriteLine($"Duplicata detectada: Move '{moveNameEng}' com ID {move.MoveID}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Erro ao processar linha de Move: {ex.Message}");
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
                        try
                        {
                            // ID
                            if (!int.TryParse(row.Cell(1).GetString(), out int numberID) || numberID == 0) { Console.WriteLine($"Error in line {row.RowNumber()} (Pokémon without ID number!)"); continue; }

                            // NAME
                            string name = row.Cell(2).GetString();
                            if (string.IsNullOrWhiteSpace(name))
                            {
                                Console.WriteLine($"Pokémon ID {numberID} Error (Name error).");
                                continue;
                            }

                            // TYPE
                            string typeStr = row.Cell(3).GetString();
                            if (string.IsNullOrEmpty(typeStr) || !Enum.TryParse<TypePokemon>(typeStr, true, out TypePokemon type))
                            {
                                Console.WriteLine($"Pokémon ID {numberID} {name} tem tipo inválido: '{typeStr}'");
                                continue;
                            }

                            // STAB TYPE
                            string stabStr = row.Cell(4).GetString();
                            TypePokemon stabType = string.IsNullOrEmpty(stabStr)
                                ? TypePokemon.None
                                : Enum.TryParse(char.ToUpper(stabStr[0]) + stabStr.Substring(1).ToLower(), out TypePokemon stabTemp)
                                    ? stabTemp
                                    : TypePokemon.None;

                            // STAGE
                            if (!int.TryParse(row.Cell(5).GetString(), out int stage) || stage < 0)
                            {
                                Console.WriteLine($"Error Pokémon ID {numberID} {name} (Pokemon without stage!)");
                                continue;
                            }

                            // EVOLUTION POKEMON ID
                            int toEvolveID = string.IsNullOrEmpty(row.Cell(6).GetString()) ? 0 : row.Cell(6).GetValue<int>();
                            if (toEvolveID < 0)
                            {
                                Console.WriteLine($"Error in the evolution ID of Pokémon ID {numberID} {name}, changed to 0.");
                                toEvolveID = 0;
                            }

                            // LEVEL BASE
                            if (!int.TryParse(row.Cell(7).GetString(), out int levelBase) || levelBase <= 0)
                            {
                                Console.WriteLine($"Error in Level of Pokémon ID {numberID} {name}, changed to 0.)");
                                levelBase = 0;
                                continue;
                            }

                            // EXP TO EVOLVE
                            string expStr = row.Cell(8).GetString().Trim();
                            int expToEvolve = 0;
                            if (!string.IsNullOrEmpty(expStr))
                            {
                                if (!expStr.All(char.IsDigit) || !int.TryParse(expStr, out expToEvolve) || expToEvolve < 0 || expToEvolve > 6)
                                {
                                    Console.WriteLine($"Invalid evolve experience for Pokémon ID {numberID} {name} ('{expStr}'). Setting to 0.");
                                    expToEvolve = 0;
                                }
                            }

                            // GEN DEX
                            int generation = row.Cell(9).GetValue<int>();

                            // COLOR TOKEN
                            string colorStr = row.Cell(10).GetString();
                            if (string.IsNullOrWhiteSpace(colorStr))
                            {
                                Console.WriteLine($"Pokémon 'ID {numberID} {name}' ignorado (coluna ColorToken vazia).");
                                continue;
                            }
                            if (!Enum.TryParse(colorStr, out ColorToken color))
                            {
                                Console.WriteLine($"Pokémon 'ID {numberID} {name}' ignorado (ColorToken inválido: {colorStr}).");
                                continue;
                            }

                            // BACKGROUND
                            string bgStr = row.Cell(11).GetString();
                            if (string.IsNullOrWhiteSpace(bgStr))
                            {
                                Console.WriteLine($"Pokémon '{name}' ignorado (coluna Background vazia).");
                                continue;
                            }
                            if (!Enum.TryParse(bgStr, out TokenBackGround background))
                            {
                                Console.WriteLine($"Pokémon '{name}' ignorado (Background inválido: {bgStr}).");
                                continue;
                            }

                            // ABILITY
                            string abilityStr = row.Cell(12).GetString().Trim().ToUpper();
                            List<Ability> abilities = new List<Ability>();

                            if (string.IsNullOrEmpty(abilityStr))
                            {
                                abilities.Add(Ability.None);
                            }
                            else
                            {
                                string[] abilityNames = abilityStr.Split(';', StringSplitOptions.RemoveEmptyEntries);

                                foreach (var nameAbi in abilityNames)
                                {
                                    string trimmedName = nameAbi.Trim();

                                    if (Enum.TryParse<Ability>(trimmedName, true, out Ability parsedAbility))
                                    {
                                        abilities.Add(parsedAbility);
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Ability '{trimmedName}' not found for Pokémon ID {numberID} {name}. Inserting None.");
                                        abilities.Add(Ability.None);
                                    }

                                }

                            }

                            // SHINY
                            bool shiny = bool.TryParse(row.Cell(15).GetString().Trim(), out bool isShiny) && isShiny;


                            // FORM
                            string? form = string.IsNullOrEmpty(row.Cell(16).GetString()) ? null : row.Cell(16).GetString();

                            // MOVES
                            List<Move> pokeMoves = new();
                            string moveString = row.Cell(13).GetString();
                            if (!string.IsNullOrEmpty(moveString))
                            {
                                string[] moveVet = moveString.Split(';');
                                foreach (var move in moveVet)
                                {
                                    if (int.TryParse(move.Trim(), out int moveID))
                                    {
                                        Move? moveFound = movesList.Find(m => m.MoveID == moveID);
                                        if (moveFound != null)
                                            pokeMoves.Add(moveFound);
                                        else
                                            Console.WriteLine($"Pokémon '{name}' referencia MoveID inexistente: {moveID}");
                                    }
                                }
                            }
                            else pokeMoves.Add(new Move(0, TypePokemon.None, "Sem ataque", 0, 6));

                            // INSTANCE
                                Pokemon pokemon = new Pokemon(
                                    numberID, name, type, stabType, stage, toEvolveID, form,
                                    levelBase, expToEvolve, generation,
                                    color, background, abilities, shiny, pokeMoves);
                            if (!pokemons.Any(p => p.NumberID == pokemon.NumberID))
                                pokemons.Add(pokemon);
                            else
                                Console.WriteLine($"Duplicata detectada: Pokémon '{name}' com ID {pokemon.NumberID}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Erro ao processar linha {row.RowNumber()} de Pokémon: {ex.Message}");
                        }
                    }
                    Console.WriteLine();
                }

                // --- CARREGA ITEM ---
                using (var workbook = new XLWorkbook(itemCardPath))
                {
                    var worksheet = workbook.Worksheet(1);
                    var rows = worksheet.RangeUsed().RowsUsed().Skip(1);

                    foreach (var row in rows)
                    {
                        try
                        {
                            if (!int.TryParse(row.Cell(1).GetString(), out int numberID) || numberID == 0)
                                continue;

                            string name = row.Cell(2).GetString();
                            string description = row.Cell(5).GetString();

                            List<EffectCard> effectCard = new();
                            string effectStrg = row.Cell(4).GetString();
                            if (!string.IsNullOrEmpty(effectStrg))
                            {
                                string[] effectVet = effectStrg.Split(';');
                                foreach (string effect in effectVet)
                                {
                                    try
                                    {
                                        string[] parts = effect.Split('.');
                                        if (parts.Length < 3)
                                        {
                                            effectCard.Add(new EffectCard(char.Parse(parts[0].Trim()), Enum.Parse<EffectType>(parts[1])));
                                            continue;
                                        }

                                        char targetEffect = char.Parse(parts[0].Trim());
                                        EffectType effectSetup = Enum.Parse<EffectType>(parts[1].Trim());
                                        int bonus = int.Parse(parts[2].Trim());
                                        effectCard.Add(new EffectCard(targetEffect, effectSetup, bonus));
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Erro ao ler efeito em Item '{name}': {ex.Message}");
                                    }
                                }
                            }

                            string typeStr = row.Cell(3).GetString();
                            if (!Enum.TryParse(char.ToUpper(typeStr[0]) + typeStr.Substring(1).ToLower(), out TypeItemCard type))
                            {
                                Console.WriteLine($"Item '{name}' possui tipo inválido: {typeStr}");
                                continue;
                            }

                            ItemCard itemCard = new ItemCard(numberID, name, type, effectCard, description);

                            if (!cardList.Any(p => p.Id == itemCard.Id))
                                cardList.Add(itemCard);
                            else
                                Console.WriteLine($"Duplicata detectada: Item '{name}' com ID {itemCard.Id}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Erro ao processar linha de Item: {ex.Message}");
                        }
                    }
                }

                // --- CARREGA PERFIS (BOXES) ---
                if (!File.Exists(trainerPath))
                {
                    Console.WriteLine("Arquivo de perfil não encontrado!");
                    return;
                }

                string fileContent = File.ReadAllText(trainerPath);
                string[] profileBlocks = Regex.Split(fileContent, @"};");

                foreach (var block in profileBlocks)
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(block)) continue;
                        profiles.Add(BoxPokemon.FromText(block, pokemons, cardList));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao processar profile: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro geral na carga do banco de dados: {ex.Message}");
            }
        }

        public static void SaveProfiles(List<BoxPokemon> profiles)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(trainerPath))
                {
                    foreach (var profile in profiles)
                    {
                        writer.WriteLine(profile.SaveProfile());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar profiles: {ex.Message}");
            }
        }
    }
}
