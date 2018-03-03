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
		static string citiesFile = "inputs/LieuxPossibles.txt";
		static string agenciesFile = "inputs/ListeAgences_100.txt";

		public const int MAXPERS = 60;
		static List<Agency> agencies;
		static List<City> cities;
		static public Random rand = new Random();

		static public double[,] distTab;

		// SETTINGS
		static public int algo = 0;


		public static void Main (string[] args)
		{
            // Set current thread culture to en - US.
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
			//Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("GUYL Bastien - PLATTEAU Jonathan - ZOMINY Marc");
			Console.WriteLine("OPTIMISATION DISCRETE 4A POLYTECH INFO");
			Console.WriteLine(" - PLACE AGENCIES/CITIES IN inputs FOLDER");
			Console.WriteLine(" - UPLOAD CSV OUTPUTS TO : opti.guyl.me TO SEE THE MAP AND GRAPHS");
			Console.WriteLine("");


			bool loop = false;
			do {
				Console.WriteLine("TYPE AGENCIES FILENAME");
				Console.WriteLine("(press enter for default = ListeAgences_100.txt)");
				agenciesFile = "inputs/" + consoleChoice("ListeAgences_100.txt");
				Console.Write("LOADING AGENCIES AND CITIES ... ");
				try {
					agencies = loadAgencies();
					cities = loadCities();
					loop = false;
				}
				catch (FileNotFoundException e) {
					Console.WriteLine("FILE NOT FOUND");
					loop = true;
				}
                catch (DirectoryNotFoundException e)
                {
                    Console.WriteLine("FILE NOT FOUND");
                    loop = true;
                }
            } while (loop);


			distTab = new double[cities.Count + agencies.Count, cities.Count + agencies.Count];
			for (int i = 0; i < cities.Count + agencies.Count; i++)
				for (int j = 0; j < cities.Count + agencies.Count; j++)
					distTab[i, j] = -1;
			Console.WriteLine("DONE");



			Console.WriteLine("START SETTING OPTIONS");
			Console.WriteLine("");

			Console.WriteLine("CHOOSE ALGORITHM :");
			Console.WriteLine("(press enter for default = 1)");
			Console.WriteLine("");
			Console.WriteLine("1 - DESCEND"); // nb ITeration ( : 2 000 )
			Console.WriteLine("2 - SIMULATED ANNEALING"); // nb Iteration ( : 50 000), mu(raison geometrique) (0 ~ 1 : 0.3) , temperature init (: 700)
			Console.WriteLine("3 - GENETIC"); // nb iteration ( : 10 000), population, proba mutation(0 ~ 1 : 0,001), proba magicTrick(0 ~ 1 : 0.1)
			Console.WriteLine();
			algo = consoleChoice(1, 3, 1);

			switch (algo) {
			case 1:
				{
					Console.Clear();
					Console.WriteLine("ALGORITHM : 1 - DESCEND");
					Console.WriteLine("TYPE nbIterations");
					Console.WriteLine("(PRESS ENTER FOR DEFAULT = 10 000)");
					Console.WriteLine("(RECOMMENDED 100 AGENCIES -> 10 000");
					Console.WriteLine("                 500 AGENCIES OR MORE -> 2 000");
					Console.WriteLine();
					int nbIter = consoleChoice(1, 1000000, 10000);
					Console.WriteLine();

					Taboo taboo = new Taboo(agencies, cities);
					Solution s = taboo.run(nbIter);
					Console.WriteLine();
					Console.WriteLine();
					Console.WriteLine(s);
					s.writeToCSV();

					break;
				}

			case 2:
				{
					Console.Clear();
					Console.WriteLine("ALGORITHM : 2 - SIMULATED ANNEALING"); // nb Iteration ( : 50 000), mu(raison geometrique) (0 ~ 1 : 0.3) , temperature init (: 700)
					Console.WriteLine("TYPE nbIterations");
					Console.WriteLine("(PRESS ENTER FOR DEFAULT = 50 000)");
					Console.WriteLine();
					int nbIter = consoleChoice(1, int.MaxValue, 50000);
					Console.WriteLine();

					// nb Iteration ( : 50 000), mu(raison geometrique) (0 ~ 1 : 0.3) , temperature init (: 700)
					Console.WriteLine("TYPE mu (geometry reason)");
					Console.WriteLine("(PRESS ENTER FOR DEFAULT = 50 000)");
					Console.WriteLine();
					double mu = consoleChoice(0, 1, 0.3);
					Console.WriteLine();

					// nb Iteration ( : 50 000), mu(raison geometrique) (0 ~ 1 : 0.3) , temperature init (: 700)
					Console.WriteLine("TYPE init temperature");
					Console.WriteLine("(PRESS ENTER FOR DEFAULT = 700)");
					Console.WriteLine();
					int temp = consoleChoice(0, int.MaxValue, 700);
					Console.WriteLine();

					SimulatedAnnealing sim = new SimulatedAnnealing(agencies, cities);
					Solution s = sim.run(nbIter, mu, temp);
					Console.WriteLine();
					Console.WriteLine();
					Console.WriteLine(s);
					s.writeToCSV();

					break;
				}

			case 3:
				{
					// nb iteration ( : 10 000), population, proba mutation(0 ~ 1 : 0,001), proba magicTrick(0 ~ 1 : 0.1)
					Console.Clear();
					Console.WriteLine("ALGORITHM : 3 - GENETIC");
					Console.WriteLine("TYPE nbIterations");
					Console.WriteLine("(PRESS ENTER FOR DEFAULT = 10 000)");
					Console.WriteLine();
					int nbIter = consoleChoice(1, int.MaxValue, 10000);
					Console.WriteLine();

					// nb iteration ( : 10 000), population, proba mutation(0 ~ 1 : 0,001), proba magicTrick(0 ~ 1 : 0.1)
					Console.WriteLine("TYPE population");
					Console.WriteLine("(PRESS ENTER FOR DEFAULT = 20)");
					Console.WriteLine();
					int popu = consoleChoice(0, int.MaxValue, 20);
					Console.WriteLine();

					// nb iteration ( : 10 000), population, proba mutation(0 ~ 1 : 0,001), proba magicTrick(0 ~ 1 : 0.1)
					Console.WriteLine("TYPE proba mutation");
					Console.WriteLine("(PRESS ENTER FOR DEFAULT = 0,001)");
					Console.WriteLine();
					double muta = consoleChoice(0, 1, 0.001);
					Console.WriteLine();

					// nb iteration ( : 10 000), population, proba mutation(0 ~ 1 : 0,001), proba magicTrick(0 ~ 1 : 0.1)
					Console.WriteLine("TYPE proba magicTrick");
					Console.WriteLine("(PRESS ENTER FOR DEFAULT = 0,1)");
					Console.WriteLine();
					double magi = consoleChoice(0, 1, 0.1);
					Console.WriteLine();

					Genetic gene = new Genetic(agencies, cities, nbIter, popu, muta, magi);
					Solution s = gene.getSolution();
					Console.WriteLine();
					Console.WriteLine();
					Console.WriteLine(s);
					s.writeToCSV();



					break;
				}
			}

            Console.WriteLine("END");
            Console.ReadKey(true);
        }

		public static List<City> loadCities()
		{
			List<City> cities = new List<City>();
            StreamReader fs = null;

            try { fs = new StreamReader(citiesFile); }
            catch (Exception) { throw; }

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

            StreamReader fs = null;

            try { fs = new StreamReader(agenciesFile); }
            catch (Exception) { throw; }

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

		public static int consoleChoice(int min, int max, int def = 0) {
			int choice = 0;
			do {
				Console.Write("? ");
				string str = Console.ReadLine();
				try {
					choice = int.Parse(keep(str, "0123456789"));
				}
				catch (FormatException) {
					choice = def;
				}
			} while (choice > max || choice < min);
			return choice;
		}

        private static bool firstPrinting = true;

        public static void print(int iteration, double cost, double min, int nbCenters)
        {
            if(firstPrinting)
            {
                Console.WriteLine("Iteration\tActual\t\tMin\t\tCenters");
                firstPrinting = false;
            }
            Console.SetCursorPosition(0, Console.CursorTop);
			Console.Write(iteration + "\t\t" + (int) cost + " \t\t" + (int) min + " \t\t" + nbCenters);
        }

		public static double consoleChoice(double min, double max, double def = 0) {
			double choice = 0;
			do {
				Console.Write("? ");
				string str = Console.ReadLine();
				try {
					choice = double.Parse(keep(str, "0123456789.,").Replace(".", ","));
				}
				catch (FormatException) {
					choice = def;
				}
			} while (choice > max || choice < min);
			return choice;
		}

		public static string consoleChoice(string def) {
			Console.Write("? ");
			string str = Console.ReadLine();
			if (str == "")
				str = def;
			return str;
		}

		public static string keep(string toFilter, string toKeep) {
			string ret = "";
			foreach (char c in toFilter)
				if (toKeep.Contains(c))
					ret += c;
			return ret;
		}
     
	}
}
