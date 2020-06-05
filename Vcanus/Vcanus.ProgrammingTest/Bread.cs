using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Vcanus.ProgrammingTest
{
    class Bread
    {
        static string File
        {
            get; set;
        }
        static string[] GetFiles(string combine) => Directory.GetFiles(combine, File, SearchOption.AllDirectories);
        internal static void GetRecipe(string file)
        {
            File = string.Concat(file, ".json");
            var path = Directory.GetParent(Environment.CurrentDirectory);
            string[] files = null;

            while (files == null || files.Length == 0)
            {
                files = GetFiles(path.FullName);
                path = Directory.GetParent(path.FullName);
            }
            var constructor = new List<SearchResult>();
            using (var sr = new StreamReader(files.First()))
                if (sr != null)
                    foreach (var parse in JArray.Parse(sr.ReadToEnd()))
                    {
                        var temp = JsonConvert.DeserializeObject<SearchResult>(parse.ToString());
                        SearchResult values = JsonConvert.DeserializeObject<SearchResult>(temp.Recipe.ToString());

                        switch (temp.BreadType)
                        {
                            case "cream":
                                constructor.Add(new Cream
                                {
                                    BreadType = temp.BreadType,
                                    Flour = values.Flour,
                                    Water = values.Water,
                                    TypeAmount = values.Cream
                                });
                                break;

                            case "sugar":
                                constructor.Add(new Sugar
                                {
                                    BreadType = temp.BreadType,
                                    Flour = values.Flour,
                                    Water = values.Water,
                                    TypeAmount = values.Sugar
                                });
                                break;

                            case "butter":
                                constructor.Add(new Butter
                                {
                                    BreadType = temp.BreadType,
                                    Flour = values.Flour,
                                    Water = values.Water,
                                    TypeAmount = values.Butter
                                });
                                break;
                        }
                    }
            foreach (var property in constructor)
                Console.WriteLine("\nbreadType: " + property.BreadType + "\nrecipe\nflour: " + property.Flour + "\nwater: " + property.Water + "\n" + property.GetType().Name.ToLower() + ": " + property.TypeAmount);
        }
    }
}