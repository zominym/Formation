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
		const string citiesFile = "../../LieuxPossibles.txt";
		const string agenciesFile = "../../ListeAgences_100.txt";

		const int MAXPERS = 60;
		static List<Agency> agencies;
		static List<City> cities;
		static public Random rand = new Random();


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
			agencies = loadAgencies();
			cities = loadCities();


//            Genetic algo = new Genetic(agencies, cities, 10000, 20);
//			Solution s = algo.getSolution();
//    		Console.WriteLine(s);
//			Console.WriteLine(s.toStringShort());


//			Tuple<int, int>[] tuples1 = new Tuple<int, int>[10];
//			Tuple<int, int>[] tuples2 = new Tuple<int, int>[10];
//			tuples1 [0] = new Tuple<int,int>(20,10);
//			tuples2 [0] = tuples1[0];
//			tuples1[0] = new Tuple<int,int>(30, 40);
//			Console.WriteLine(tuples2[0]);


			Taboo taboo = new Taboo(agencies, cities);
			Solution s = taboo.run(1000);
			Console.WriteLine(s);

			s.writeToCSV();


			//Solution sp = s.getGradientDescendSolution();
			//Console.WriteLine(sp);
			//Console.WriteLine(sp.toStringShort());



//			Solution s = new Solution(9);
//			Console.WriteLine(s);
//			Console.WriteLine(s.toStringShort());



//			for (int i = 0; i < 10; i++) {
//				Solution s = new Solution();
//				//onsole.WriteLine(s);
//				Console.WriteLine(s.toStringShort());
//			}


//			Console.WriteLine();
//			Console.WriteLine();
//			Console.WriteLine("AGENCIES :");
//			foreach (Agency a in agencies) {
//				Console.WriteLine(a);
//			}
//
//			Console.WriteLine();
//			Console.WriteLine();
//			Console.WriteLine("CITIES :");
//			foreach (City c in cities) {
//				Console.WriteLine(c);
//			}

//			Console.WriteLine();
//			Console.WriteLine();
//			Console.WriteLine("TESTS :");

			//List<City> cities2 = new List<City>();
			/*for (int i = 0; i < agencies.Count(); i++)
				cities2.Add(agencies[(i + 5) % agencies.Count()].toCity());
			Solution sol = new Solution(agencies.ToArray(), cities2.ToArray());*/

			/*Console.WriteLine("SOL :");
			Console.WriteLine(sol);

			Console.WriteLine("COST :");
			Console.WriteLine(sol.Cost);

			Console.WriteLine("MUTATING ...");
			sol.mutate(cities.ToArray());

			Console.WriteLine("SOL :");
			Console.WriteLine(sol);

			Console.WriteLine("COST :");
			Console.WriteLine(sol.Cost);

			Console.WriteLine("GRADIENT DESCENT ...");
			Solution betterSol = sol.getGradientDescendSolution();

			Console.WriteLine("SOL :");
			Console.WriteLine(betterSol);

			Console.WriteLine("COST :");
			Console.WriteLine(betterSol.Cost);

			Console.WriteLine();
			Console.WriteLine();
			*/

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


		public static List<Agency> getAgencies() {
			return agencies;
		}

		public static List<City> getCities() {
			return cities;
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
