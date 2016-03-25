using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;

namespace LieuxDeFormation
{
	class MainClass
	{


		// BROUILLON
		/*
		 * 
		 * - Une solution : Hashmap d'agences qui ont un centre de formation (une ville)
		 * 		- Remarque : On laisse les personnes d'une agence dans le même centre de formation.
		 * - La fonction à minimiser : TODO
		 * - Le voisinnage : Déplacer une agence d'un centre à un autre
		 * 		- Remarque : On peut mettre une limite à la distance maximale d'un déplacement (On ne peut pas affecter une agence à un centre éloigné de plus de X (e.g. 500) Km
		 * 
		 * 
		 * 
		 * 
		 * 
		 * /


		public static void Main (string[] args)
		{
            // Set current thread culture to en - US.
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");


            City c1 = new City ();
			City c2 = new City ();

			List<City> cities = loadCities();

			foreach (City c in cities) {
//				rainbowprintLine (c.ToString(), "rainbow", 5);

//				coolRandPrint (c.ToString());
				if (c.getId() == "lyon")
					c1 = c;
				if (c.getId() == "paris")
					c2 = c;
			}

//			rainbowprintLine (c1.ToString(), "rainbow", 5);
//			rainbowprintLine (c2.ToString(), "rainbow", 5);
//			rainbowprintLine (c1.distanceTo (c2).ToString(), "rainbow", 5);
//			rainbowprintLine ("END", "rainbow", 5);

			coolRandPrint (c1.ToString());
			coolRandPrint (c2.ToString());
			coolRandPrint (c1.distanceTo (c2).ToString());
			coolRandPrint ("END");

			//Console.ForegroundColor = Console.ForegroundColor;

			//Console.ForegroundColor = ConsoleColor.White;
			coolRandPrint("This is the test of a cool random print.");
            Console.ReadKey(true);
        }

		public static List<City> loadCities()
		{
			List<City> cities = new List<City> ();
			StreamReader fs = new StreamReader ("../../LieuxPossibles.txt");

			String line = fs.ReadLine();

			while((line = fs.ReadLine()) != null)
			{
				cities.Add(new City(line));
			}

			fs.Close ();

			return cities;
		}


		public static bool IsLinux
		{
			get
			{
				int p = (int) Environment.OSVersion.Platform;
				return (p == 4) || (p == 6) || (p == 128);
			}
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
