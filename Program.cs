using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using ProjetoPokemon.Entities;
using ProjetoPokemon.Entities.Enums;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using ProjetoPokemon.Entities.Data;

namespace ProjetoPokemon
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Pokemon> pokemons = new List<Pokemon>();
            List<Move> movesList = new List<Move>();
            DataBaseControl.DataBase(pokemons, movesList);

            foreach (var p in pokemons)
            {
                Console.WriteLine(p);
            }
        }
    }
}