using System;
using TrainingProblem;
using System.Collections.Generic;

namespace Metaheuristic
{
	public class Taboo
	{

//		List<City> _cities;
//		List<Agency> _agencies;
		List<double> _visited;
		Random rand = LieuxDeFormation.MainClass.rand;


		public Taboo(List<Agency> agencies, List<City> cities)
		{
//			_agencies = agencies;
//			_cities = cities;
			_visited = new List<double>();
		}

		public Solution run(int nbIter) {
			int nbVoisins = LieuxDeFormation.MainClass.getAgencies().Count;
			Solution min = new Solution("peu de centres");
			Solution s = min;
			Console.WriteLine("INIT --> " + "cost actuel : " + s.Cost);
//			Console.WriteLine(s.toStringShort());
			for (int i = 0; i < nbIter; i++) {
//				if (s.getPersTot() != 522)
//					Console.WriteLine("Alerte 1 : " + " id : " + s.id + " getPersTot() : " + s.getPersTot());
				s = visit2(s);
				if (s.Cost < min.Cost)
					min = s;
//				_visited.Add(s.Cost);
//				else
//					Console.WriteLine("On a empiré");
				Console.WriteLine("ITER: " + i + " --> " + "actuel: " + s.Cost + " min: " + min.Cost + " centres: " + s.getUsedCities().Count);
				Console.WriteLine(Solution.nbSuccess/Solution.nbTries);
//				s.calculateCostBavard();
//				Console.WriteLine(s.toStringShort());
			}
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

		public Solution visit(Solution s) {
//			Console.WriteLine("Alerte 2 : " + " id : " + s.id + " getPersTot() : " + s.getPersTot());
			Solution min = null;
			for (int i = 0; i < s._tuples.Length; i++) {
				Solution sp = s.mutate(i);
				if (i == 0)
					min = sp;
				if (sp.Cost < min.Cost /*&& !alreadyVisited(sp)*/)
					min = sp;
//				else
//					Console.WriteLine(sp.Cost + " worst than " + min.Cost);
//				Console.WriteLine("Alerte 3 : " + " id : " + s.id + " getPersTot() : " + s.getPersTot());
			}
			foreach (City c in s.getUsedCities()) {
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

