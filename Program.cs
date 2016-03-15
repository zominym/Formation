using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LieuxDeFormation
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			City c1 = new City ();
			City c2 = new City ();

			List<City> cities = loadCities();

			foreach (City c in cities) {
				Console.WriteLine (c);
				if (c.getId() == "lyon")
					c1 = c;
				if (c.getId() == "paris")
					c2 = c;
			}

			Console.WriteLine (c1);
			Console.WriteLine (c2);
			Console.WriteLine (c1.distanceTo (c2));
			Console.WriteLine ("End");
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
	}
}
