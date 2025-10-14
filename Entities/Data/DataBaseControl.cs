using ClosedXML.Excel;
using ProjetoPokemon.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjetoPokemon.Entities.Data
{
    internal static class DataBaseControl
    {
        static string pathData = "C:\\Users\\kleil\\OneDrive\\Documentos\\ProjetosC\\ProjetoPokemon\\Entities\\Data\\";

        public static void DataBase(List<Pokemon> pokemons, List<Move> movesList)
        {
            // --- CARREGA MOVES ---
            using (var workbook = new XLWorkbook(pathData + "MovesPokemon.xlsx"))
            {
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RangeUsed().RowsUsed().Skip(1);

                foreach (var row in rows)
                {
                    // - ID e Nomes
                    int moveID = row.Cell(1).GetValue<int>();
                    string moveNameEng = row.Cell(2).GetString();
                    string moveNamePtBr = row.Cell(8).GetString();
                    string moveNameEsp = row.Cell(9).GetString();
                    string moveNameJp = row.Cell(10).GetString();

                    // - Poder, Tipo e Dado
                    int power = row.Cell(3).GetValue<int>();
                    string diceSides = row.Cell(5).GetString();

                    string typeStr = row.Cell(4).GetString();
                    typeStr = char.ToUpper(typeStr[0]) + typeStr.Substring(1).ToLower();
                    TypePokemon type = Enum.Parse<TypePokemon>(typeStr);

                    // - Efeitos
                    List<EffectMove> effectMoves = new List<EffectMove>();
                    int efRoll = 0;
                    if (!string.IsNullOrEmpty(row.Cell(7).GetString()) && int.TryParse(row.Cell(7).GetString(), out int result))
                        efRoll = result;

                    string effectMove = row.Cell(6).GetString();
                    if (!string.IsNullOrEmpty(effectMove))
                    {
                        string[] effectVet = effectMove.Split(';');

                        for (int i = 0; i < effectVet.Length; i++)
                        {
                            string[] parts = effectVet[i].Split('.');

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

                    // ✅ Move SEMPRE é criado, mesmo sem efeitos
                    Move move = new Move(moveID, type, moveNameEng, power, effectMoves, diceSides, efRoll);
                    movesList.Add(move);
                }
            }

            // --- CARREGA POKÉMON ---
            using (var workbook = new XLWorkbook(pathData + "Pokemon.xlsx"))
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

                    // - Movimentos
                    List<Move> pokeMoves = new List<Move>();
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

                    Pokemon pokemon = new Pokemon(numberID, name, type, stabType, stage, toEvolveID, form,
                        levelBase, expToEvolve, generation,
                        color, background, new List<Ability>(), shiny, pokeMoves);

                    pokemons.Add(pokemon);
                }
            }
        }
    }
}
