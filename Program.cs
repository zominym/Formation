using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;

using TrainingProblem;
using Metaheuristic;

namespace LieuxDeFormation
{
	class MainClass
	{
		const string citiesFile = "../../LieuxPossiblesTEST.txt";
		const string agenciesFile = "../../ListeAgences_TEST.txt";

		// BROUILLON
		/*
		 * 
		 * - Une solution : Hashmap d'agences qui ont un centre de formation (une ville)
		 * 		- Remarque : On laisse les personnes d'une agence dans le même centre de formation.
		 * - La fonction à minimiser : TODO
		 * - Le voisinnage : Echanger les centres de formation de deux agences
		 * 		- Remarque : On peut mettre une limite à la distance maximale d'un déplacement (On ne peut pas affecter une agence à un centre éloigné de plus de X (e.g. 500) Km
		 * - Le croisement :
		 * - La mutation :
		 * 
		 * 
		 * 
		 * 
		 * 
         */


		public static void Main (string[] args)
		{
            // Set current thread culture to en - US.
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
			//Console.ForegroundColor = ConsoleColor.White;

			Console.WriteLine("LOADING AGENCIES AND CITIES ...");
			List<Agency> agencies = loadAgencies();
			List<City> cities = loadCities();

			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("AGENCIES :");
			foreach (Agency a in agencies) {
				Console.WriteLine(a);
			}

			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("CITIES :");
			foreach (City c in cities) {
				Console.WriteLine(c);
			}

			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("TESTS :");

			List<Agency> agencies2 = new List<Agency>();
			for (int i = 0; i < agencies.Count(); i++)
				agencies2.Add(agencies[(0) % agencies.Count()]);
			Solution sol = new Solution(agencies.ToArray(), (City[]) agencies2.ToArray());
			Console.WriteLine(sol);
			Console.WriteLine(agencies[1].distanceTo(agencies[0]));
			Console.WriteLine(agencies[2].distanceTo(agencies[0]));
			Console.WriteLine(agencies[3].distanceTo(agencies[0]));
			Console.WriteLine(agencies[4].distanceTo(agencies[0]));
			Console.WriteLine(agencies[5].distanceTo(agencies[0]));

			Console.WriteLine("COST :");
			Console.WriteLine(sol.cost());


			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("END");
            Console.ReadKey(true);
        }

		public static List<City> loadCities()
		{
			List<City> cities = new List<City>();
			StreamReader fs = new StreamReader(citiesFile);

			String line = fs.ReadLine();

			while((line = fs.ReadLine()) != null)
			{
				cities.Add(new City(line));
			}

			fs.Close();

			return cities;
		}

		public static List<Agency> loadAgencies()
		{
			List<Agency> agencies = new List<Agency>();
			StreamReader fs = new StreamReader(agenciesFile);

			String line = fs.ReadLine();

			while((line = fs.ReadLine()) != null)
			{
				agencies.Add(new Agency(line));
			}

			fs.Close();

			return agencies;
		}

        public Solution mutation(Solution s, City[] cities)
        {   
            Random rand = new Random();
            City[] mutant = s.Cities;
            mutant[rand.Next(mutant.Length)] = cities[rand.Next(cities.Length)];
            return new Solution(s.Agencies, mutant);
        }






















        public static void rainbowprintLine(string s, string style = "random", int cycle = 1)
		{
			rainbowprint (s + "\n", style, cycle);
		}

		public static void rainbowprint(string s, string style = "random", int cycle = 1)
		{
			Array values = Enum.GetValues (typeof(ConsoleColor));
			List<ConsoleColor> values2 = new List<ConsoleColor> ();
			int count = 0;
			values2.Add (ConsoleColor.Green);
			values2.Add (ConsoleColor.Blue);
			values2.Add (ConsoleColor.Magenta);
			values2.Add (ConsoleColor.Red);
			values2.Add (ConsoleColor.Yellow);
			values2.Add (ConsoleColor.White);
			values2.Add (ConsoleColor.Cyan);
			switch (style) {
			case "random":
				count = 0;
				Random random = new Random (Guid.NewGuid ().GetHashCode ());
				foreach (char c in s) {
					if (count >= cycle) {
						count = 0;
						Console.ForegroundColor = (ConsoleColor)values.GetValue (random.Next (values.Length - 1) + 1);
					}
					Console.Write (c);
					count++;
				}
				break;
			case "rainbow":
				int i = 0;
				count = 0;
				foreach (char c in s) {
					if (count >= cycle) {
						count = 0;
						Console.ForegroundColor = (ConsoleColor)values2 [i];
						if (i < values2.Count - 1)
							i++;
						else
							i = 0;
					}
					Console.Write (c);
					count++;
				}
				break;
			default:
				Console.WriteLine("UNSUPPORTED RAINBOWPRINT STYLE");
				break;
			}
		}

		public static async void coolRandPrint(string s) {
			int currentLineCursor = Console.CursorTop;
			Random rand = new Random();
			List<int> notok = Enumerable.Range(0, s.Length - 1).ToList();
			do {

				Console.SetCursorPosition(0, currentLineCursor);
				for (int i = 0; i < s.Length; i++) {
					if (notok.Contains(i)) {
						char c = s[notok[rand.Next(0, notok.Count)]];
						if (c == s[i])
							notok.Remove(i);
						Console.Write(c);
					} else {
						Console.Write(s[i]);
					}
				}
				await Task.Delay(100);
			} while (notok.Count > 0);
		}
	}
}
