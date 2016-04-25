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


		public Taboo(List<Agency> agencies, List<City> cities)
		{
//			_agencies = agencies;
//			_cities = cities;
			_visited = new List<double>();
		}

		public Solution run(int nbIter, int nbVoisins) {
			Solution min = new Solution();
			Solution s = min;
			Console.WriteLine("INIT --> " + "cost actuel : " + s.Cost);
			for (int i = 0; i < nbIter; i++) {
//				if (s.getPersTot() != 522)
//					Console.WriteLine("Alerte 1 : " + " id : " + s.id + " getPersTot() : " + s.getPersTot());
				s = visit(s, nbVoisins);
				if (s.Cost < min.Cost)
					min = s;
				_visited.Add(s.Cost);
//				else
//					Console.WriteLine("On a empiré");
				Console.WriteLine("Iter : " + i + " --> " + "cost actuel : " + s.Cost + " cost min : " + min.Cost);
			}
			return s;
		}

		public Solution visit(Solution s, int nbVoisins) {
//			Console.WriteLine("Alerte 2 : " + " id : " + s.id + " getPersTot() : " + s.getPersTot());
			Solution min = null;
			for (int i = 0; i < nbVoisins; i++) {
				Solution sp = s.mutate(i);
				if (i == 0)
					min = sp;
				if (sp.Cost < min.Cost && !alreadyVisited(sp))
					min = sp;
//				else
//					Console.WriteLine(sp.Cost + " worst than " + min.Cost);
//				Console.WriteLine("Alerte 3 : " + " id : " + s.id + " getPersTot() : " + s.getPersTot());
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

