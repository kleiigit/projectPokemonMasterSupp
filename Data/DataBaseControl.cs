using ClosedXML.Excel;
using ProjetoPokemon.Entities;
using ProjetoPokemon.Enums;
using ProjetoPokemon.Services;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ProjetoPokemon.Data
{
    internal static class DataBaseControl //Faz o controle dos arquivos xlsx
    {
        private static readonly string pathData;
        private static readonly string movesPath;
        private static readonly string pokemonPath;
        private static readonly string trainerPath;
        private static readonly string itemCardPath;
        private static readonly string preferencesPath;

        // Bloco estático executado apenas uma vez quando a classe é usada
        static DataBaseControl()
        {
            string exeDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            string relativePath = Path.Combine(exeDirectory, @"..\..\..\Data");
            pathData = Path.GetFullPath(relativePath);

            movesPath = Path.Combine(pathData, "MovesPokemon.xlsx");
            pokemonPath = Path.Combine(pathData, "Pokemon.xlsx");
            trainerPath = Path.Combine(pathData, "ProfileTrainer.txt");
            itemCardPath = Path.Combine(pathData, "ItemCard.xlsx");
            preferencesPath = Path.Combine(pathData, "Preferences.txt");
        }
        public static void DataBase()
        {
            try
            {
                // --- CARREGA PREFERENCES ---
                LoadPreferencesData();
                // --- CARREGA MOVES ---
                LoadMoveData();
                // --- CARREGA POKÉMON ---
                LoadPokemonData();
                // --- CARREGA ITEM ---
                LoadItemData();
                // --- CARREGA PERFIS (BOXES) ---
                LoadTrainerBoxData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro geral na carga do banco de dados: {ex.Message}");
            }
        }
        private static void LoadPokemonData()
        {
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
                            Console.WriteLine($"Pokémon ID {numberID} Error (NickPokemon error).");
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
                        string cellValue = row.Cell(6).GetString(); int[] toEvolveID;
                        if (string.IsNullOrEmpty(cellValue)) toEvolveID = Array.Empty<int>();
                        else
                        {
                            toEvolveID = cellValue
                                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                                .Select(s =>
                                {
                                    if (int.TryParse(s, out int id) && id >= 0)
                                        return id;
                                    Console.WriteLine($"Error in the evolution ID of Pokémon ID {numberID} {name}, value '{s}' changed to 0.");
                                    return 0;
                                })
                                .ToArray();
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
                                    Move? moveFound = DataLists.GetMoveID(moveID);
                                    if (moveFound != null)
                                        pokeMoves.Add(moveFound);
                                    else
                                        Console.WriteLine($"Pokémon '{name}' referencia MoveID inexistente: {moveID}");
                                }
                                else
                                {
                                    Move? moveFound = DataLists.GetMoveName(move);
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
                            numberID, name, type, stabType, stage, toEvolveID,
                            levelBase, expToEvolve, generation,
                            color, background, abilities, pokeMoves);
                        if (!DataLists.AllPokemons.Any(p => p.NumberID == pokemon.NumberID))
                            DataLists.AddPokemon(pokemon);
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
        }
        private static void LoadMoveData()
        {
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
                        else if (!string.IsNullOrEmpty(efRollStr))
                        {
                            Console.WriteLine($"Invalid effect dice sides '{efRollStr}' for Pokémon ID {moveID}. Setting to 0.");
                            efRoll = 0;
                        }

                        List<EffectManager> effectMoves = new();
                        string effectMove = row.Cell(6).GetString().ToUpper();
                        if (!string.IsNullOrEmpty(effectMove))
                        {
                            string[] effectVet = effectMove.Split(';');
                            foreach (string effect in effectVet)
                            {
                                EffectManager effectAdd = EffectSetup.EffectStance(effect, moveID);
                                if (effectAdd != null) effectMoves.Add(effectAdd);
                                else { Console.WriteLine($"Error Effect {effect} is null, Move ID {moveID}"); continue; }
                            }
                        }

                        Move move = new Move(moveID, type, moveNameEng, power, effectMoves, diceSides, efRoll);

                        if (!DataLists.AllMoves.Any(p => p.MoveID == move.MoveID))
                            DataLists.AddMove(move);
                        else
                            Console.WriteLine($"Duplicata detectada: Move '{moveNameEng}' com ID {move.MoveID}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao processar linha de Move: {ex.Message}");
                    }
                }
            }
        }
        private static void LoadItemData()
        {
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
                        if (string.IsNullOrWhiteSpace(name))
                        {
                            Console.WriteLine($"Item ID {numberID} Error (NickPokemon error).");
                            continue;
                        }
                        string description = row.Cell(6).GetString();

                        List<EffectManager> effectCard = new();
                        string effectStrg = row.Cell(4).GetString();
                        if (!string.IsNullOrEmpty(effectStrg))
                        {
                            string[] effectVet = effectStrg.Split(';');
                            foreach (var effect in effectVet)
                            {
                                EffectManager effectAdd = EffectSetup.EffectStance(effect, numberID);
                                if (effectAdd != null) effectCard.Add(effectAdd);
                                else { Console.WriteLine($"Error Effect {effect} is null, Item ID {numberID}"); continue; }
                            }
                        }

                        string typeStr = row.Cell(3).GetString();
                        if (!Enum.TryParse(char.ToUpper(typeStr[0]) + typeStr.Substring(1).ToLower(), out TypeItemCard type))
                        {
                            Console.WriteLine($"Item '{name}' possui tipo inválido: {typeStr}");
                            continue;
                        }
                        if (!Enum.TryParse(row.Cell(5).GetString(), out RarityCard rarity))
                        {
                            Console.WriteLine($"Item '{name}' possui raridade inválida: {row.Cell(6).GetString()}");
                            continue;
                        }

                        ItemCard itemCard = new ItemCard(numberID, name, rarity, type, effectCard, description);

                        if (!DataLists.AllItemCards.Any(p => p.Id == itemCard.Id))
                            DataLists.AddItemCard(itemCard);
                        else
                            Console.WriteLine($"Duplicata detectada: Item '{name}' com ID {itemCard.Id}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao processar linha de Item: {ex.Message}");
                    }
                }
            }
        }
        private static void LoadTrainerBoxData()
        {
            if (!File.Exists(trainerPath))
            {
                Console.WriteLine("Arquivo de preferências não encontrado! Criando novo arquivo...");

                // Cria o diretório, se não existir
                Directory.CreateDirectory(Path.GetDirectoryName(trainerPath)!);

                // Cria o arquivo vazio (ou com conteúdo padrão, se desejar)
                File.WriteAllText(trainerPath, string.Empty);

                // Opcional: retornar ou continuar, conforme sua lógica
                return;
            }

            string fileContent = File.ReadAllText(trainerPath);
            string[] profileBlocks = Regex.Split(fileContent, @"};");

            foreach (var block in profileBlocks)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(block)) continue;
                    DataLists.AddProfile(BoxPokemon.FromText(block));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao processar profile: {ex.Message}");
                }
            }
        }
        private static void LoadPreferencesData()
        {
            if (!File.Exists(preferencesPath))
            {
                Console.WriteLine("Arquivo de preferências não encontrado! Criando novo arquivo...");

                Directory.CreateDirectory(Path.GetDirectoryName(preferencesPath)!);

                // Criação padrão com "language = 0"
                using (StreamWriter sw = new StreamWriter(preferencesPath))
                {
                    sw.WriteLine("# Configurações iniciais do jogo");
                    sw.WriteLine("# language = 0 (Português), 1 (Inglês), 2 (Espanhol), 3 (Japanese)");
                    sw.WriteLine("language = 0");
                }
            }
            // Lê todas as linhas do arquivo
            string[] lines = File.ReadAllLines(preferencesPath);

            // Dicionário para armazenar as configurações lidas
            Dictionary<string, string> preferences = new(StringComparer.OrdinalIgnoreCase);

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
                    continue;

                string[] parts = line.Split('=', 2, StringSplitOptions.TrimEntries);
                if (parts.Length == 2)
                    preferences[parts[0].ToLower()] = parts[1];
            }

            // Interpreta o idioma
            int langValue = 0; // padrão
            if (preferences.TryGetValue("language", out string? value))
            {
                if (!int.TryParse(value, out langValue))
                {
                    Console.WriteLine($"Valor inválido de 'language' encontrado: {value}. Usando padrão 0 (Português).");
                    langValue = 0;
                }
            }

            // Define o idioma global (exemplo)
            Languages currentLanguage = (Languages)langValue;
            Console.WriteLine($"Idioma definido: {currentLanguage}");
        }

        public static void SaveProfiles()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(trainerPath))
                {
                    foreach (var profile in DataLists.AllProfiles)
                    {
                        writer.WriteLine(profile.SaveProfile());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar profiles: {ex.Message}");
            }
            Console.WriteLine("Save...");
        }
    }
}

