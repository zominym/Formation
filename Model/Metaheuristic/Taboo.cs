using System;
using TrainingProblem;
using System.Collections.Generic;
using System.IO;

namespace Metaheuristic
{
	public class Taboo
	{

		/*
		 * "longitude1";"latitude1";"nbpersonne1";"longitude2";"latitude2"
		 *
		 *	"cout";"distanceTotale";"nbcentres"
		 */

		StreamWriter sw;

//		List<City> _cities;
//		List<Agency> _agencies;
		List<double> _visited;
		Random rand = LieuxDeFormation.MainClass.rand;


		public Taboo(List<Agency> agencies, List<City> cities)
		{
//			_agencies = agencies;
//			_cities = cities;
			_visited = new List<double>();
			sw = new StreamWriter(File.Create("outputs/" + DateTime.Now.ToString("ITERS dd_mm_yy HH-mm-ss") + ".csv"));
		}

		public Solution run(int nbIter) {
			int nbVoisins = LieuxDeFormation.MainClass.getAgencies().Count;
			Solution min = new Solution("peu de centres");
//			Solution min = new Solution();
			Solution s = min;
			sw.WriteLine("\"cout\";\"distanceTotale\";\"nbcentres\"");
			sw.WriteLine(s.toCSVShort());
            
            //Console.Write("INIT --> " + "cost actuel : " + s.Cost);
            
            //			Console.WriteLine(s.toStringShort());
            for (int i = 0; i < nbIter; i++) {
//				if (s.getPersTot() != 522)
//					Console.WriteLine("Alerte 1 : " + " id : " + s.id + " getPersTot() : " + s.getPersTot());
				s = visit(s);
				if (s.Cost < min.Cost)
					min = s;

				sw.WriteLine(s.toCSVShort());
//				_visited.Add(s.Cost);
//				else
//					Console.WriteLine("On a empiré");
                LieuxDeFormation.MainClass.print(i, s.Cost, min.Cost, s.getUsedCities().Count);
//				Console.WriteLine(Solution.nbSuccess/Solution.nbTries);
//				s.calculateCostBavard();
//				Console.WriteLine(s.toStringShort());
			}
			sw.Close();
			return s;
		}

		public Solution visit2(Solution s) {
			//			Console.WriteLine("Alerte 2 : " + " id : " + s.id + " getPersTot() : " + s.getPersTot());
			Solution min = null;
			for (int i = 0; i < s._tuples.Length; i++) {
				Solution sp = s.mutate2(i);
				if (i == 0)
					min = sp;
				if (sp.Cost < min.Cost /*&& !alreadyVisited(sp)*/)
					min = sp;
				//				else
				//					Console.WriteLine(sp.Cost + " worst than " + min.Cost);
				//				Console.WriteLine("Alerte 3 : " + " id : " + s.id + " getPersTot() : " + s.getPersTot());
			}
			return min;
		}

		public Solution visit3(Solution s) {
			//			Console.WriteLine("Alerte 2 : " + " id : " + s.id + " getPersTot() : " + s.getPersTot());
			Solution min = null;
			for (int i = 0; i < 100 /*s._tuples.Length*/; i++) {
				Solution sp = s.mutate3(rand.Next(s._tuples.Length)); // s.mutate(i);
				if (i == 0)
					min = sp;
				if (sp.Cost < min.Cost /*&& !alreadyVisited(sp)*/)
					min = sp;
				//				else
				//					Console.WriteLine(sp.Cost + " worst than " + min.Cost);
				//				Console.WriteLine("Alerte 3 : " + " id : " + s.id + " getPersTot() : " + s.getPersTot());
			}
			return min;
		}

		public Solution visit(Solution s) {
//			Console.WriteLine("Alerte 2 : " + " id : " + s.id + " getPersTot() : " + s.getPersTot());
			Solution min = null;
			List<City> gUC = s.getUsedCities();
			for (int i = 0; i < s._tuples.Length; i++) {
				Solution sp = s.mutate(i, gUC);
				if (i == 0)
					min = sp;
				if (sp.Cost < min.Cost /*&& !alreadyVisited(sp)*/)
					min = sp;
//				else
//					Console.WriteLine(sp.Cost + " worst than " + min.Cost);
//				Console.WriteLine("Alerte 3 : " + " id : " + s.id + " getPersTot() : " + s.getPersTot());
			}
			foreach (City c in gUC) {
				List<City> cities = LieuxDeFormation.MainClass.getCities();
				Solution sp;
				do {
					City city = cities[rand.Next(cities.Count)];
					sp = s.give(c, city);
				} while(sp == null);
				if (sp.Cost < min.Cost /*&& !alreadyVisited(sp)*/)
					min = sp;
			}
			return min;
		}

		public bool alreadyVisited(Solution sol) {
			// Il est très peu probable que deux solutions différentes aient le même Cost, on simplifie donc la complexité en estimant que deux solutions de même Cost sont égales
			foreach (double x in _visited) {
				if (sol.Cost == x) {
//					Console.WriteLine("Already visited");
					return true;
				}
			}
//			Console.WriteLine("Not already visited");
			return false;
		}

	}
}

