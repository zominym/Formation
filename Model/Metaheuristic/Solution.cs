using System;
using System.Collections.Generic;
using TrainingProblem;
using LieuxDeFormation;

namespace Metaheuristic
{
    public class Solution
    {
		static int ID = 0;
		double TRANSPORTFEE = 0.4;
		int AGENCYFEE = 3000;
		const int CITYCAPACITY = 60;

		private double _cost = -1;
        private List<Solution> _neighbors = null;
		private Tuple<Agency, City>[] _tuples = new Tuple<Agency, City>[MainClass.getAgencies().Count];
		private Random rand = LieuxDeFormation.MainClass.rand;

		public int id;

		// Constructeur de solution aléatoire
		public Solution() {
			List<City> _cities = LieuxDeFormation.MainClass.getCities();
			id = ID + 1;
			ID++;
            int tirage, capacityRequired, i = 0;
            foreach(Agency a in MainClass.getAgencies())
            {
                do
                {
                    tirage = rand.Next(_cities.Count);
					capacityRequired = a.getNbPers() + this.getNbPers(_cities[tirage]);
                } while (capacityRequired > CITYCAPACITY);

                _tuples[i] = new Tuple<Agency, City>(a, _cities[tirage]);

                i++;

//				Console.WriteLine(a);
//				Console.WriteLine(this.toStringShort());
            }
           
        }

		// Constructeur de solution aléatoire avec distance maximum entre deux villes
		public Solution(int distanceMax, int increment = 10, int refusMax = 100) {
			id = ID + 1;
			ID++;
			List<City> _cities = LieuxDeFormation.MainClass.getCities();

			City c = new City();
            int i = 0;
			foreach (Agency a in MainClass.getAgencies()) {
				int refus = 0;
				bool loop = true;
				do {
					c = _cities[rand.Next(_cities.Count)];

					if (refus >= refusMax) {
						refus = 0;
						distanceMax += increment;
					}

					if (this.getNbPers(c) + a.getNbPers() <= CITYCAPACITY)
						loop = false;

					if (a.distanceTo(c) > distanceMax) {
						loop = true;
						refus++;
					}
				} while (loop);
                _tuples[i] = new Tuple<Agency, City>(a, c);
                i++;
			}

		}

		public Solution (Solution s) {
			id = ID + 1;
			ID++;
			_cost = -1;
			for (int i = 0; i < _tuples.Length; i++)
				_tuples[i] = s._tuples[i];
			_neighbors = s._neighbors;
		}

        public List<Solution> Neighbors
        {
            get { 
				if (_neighbors == null)
                    buildNeighborhood();
                return _neighbors;
            }
        }

		public double Cost {
			get { 
				if (_cost == -1)
					_cost = calculateCost();
				return _cost;
			}
		}
            
        private void buildNeighborhood()
        {
            _neighbors = new List<Solution>();
            for (int i = 0; i < _tuples.Length; i++)
            {
                for (int j = i + 1; j < _tuples.Length; j++) 
                {
                    Solution tmp = swap(i, j);
					if (tmp != null)
                    	_neighbors.Add(tmp);
                }
            }
        }

		private Solution swap(int a, int b){
			Solution temp = new Solution(this);
			City tmp = temp._tuples[a].Item2;
			temp._tuples[a] = Tuple.Create(temp._tuples[a].Item1, temp._tuples[b].Item2);
			temp._tuples[b] = Tuple.Create(temp._tuples[b].Item1, tmp);
			temp._cost = -1;

			if (temp.getNbPers(_tuples[a].Item2) > CITYCAPACITY || temp.getNbPers(_tuples[b].Item2) > CITYCAPACITY)
				return null;
			return temp;
		}

		public Solution mutate(){   
            List<City> cities = MainClass.getCities();
			Solution temp = new Solution(this);
//            Console.WriteLine("BEFORE : "+this);
			bool loop = true;
			do {
//				Console.WriteLine("BEFORE MUTATE : " + temp.toStringShort());
				int idx = rand.Next(_tuples.Length);
				City c = cities[rand.Next(cities.Count)];
				temp._tuples[idx] = new Tuple<Agency, City>(temp._tuples[idx].Item1, c);
				temp._cost = -1;
//				Console.WriteLine("AFTER MUTATE : " + temp.toStringShort());
				if (temp.getNbPers(_tuples[idx].Item2) <= CITYCAPACITY)
					loop = false;
			} while (loop);
//            Console.WriteLine("NOW : "+temp);
			return temp;
            //return new Solution();
		}

		public Solution mutate(int n) {
			List<City> _cities = LieuxDeFormation.MainClass.getCities();
			Solution temp = new Solution(this);
			City c;
			bool loop = true;
			do {
				c = _cities[rand.Next(_cities.Count)];
				if (temp.getNbPers(c) + temp._tuples[n].Item1.getNbPers() <= CITYCAPACITY)
					loop = false;
			} while (loop);

			temp._cost = -1;
			temp._tuples[n] = new Tuple<Agency, City>(temp._tuples[n].Item1, c);

			return temp;
		}

		public int getNbPers(City c) {
			int nb = 0;
			for (int i = 0; i < _tuples.Length; i++)
				if (_tuples[i] != null && _tuples[i].Item2.getId() == c.getId())
					nb += _tuples[i].Item1.getNbPers();
			return nb;
		}

        public List<Solution> crossover(Solution y)
		{
			//TODO crossover renvoie parfois des solutions avec des villes ayant plus de 100 personnes !!!!!!
//			Console.WriteLine("Debut du crossover");
			Solution fils;
            Solution fille;
			bool loop = true;
			do {
				fils = new Solution(this);
                fille = new Solution(this);

				int i = 0;
//				Console.WriteLine("CROSSOVER");
				foreach (Agency a in MainClass.getAgencies()) {

//					Console.WriteLine("PARENT 1 : ");
//					Console.WriteLine(this._tuples[i].Item2);
//					Console.WriteLine("PARENT 2 : ");
//					Console.WriteLine(y._tuples[i].Item2);
					City c, c2;
					if (rand.NextDouble() < 0.5) {
//						Console.WriteLine("SELECTED PARENT 1");
						c = new City(this._tuples[i].Item2);
                        c2 = new City(y._tuples[i].Item2);
                    }
					else {
//						Console.WriteLine("SELECTED PARENT 2");
						c = new City(y._tuples[i].Item2);
                        c2 = new City(this._tuples[i].Item2);
                    }
                    
                    fils._tuples[i] = new Tuple<Agency, City>(a, c);
                    fille._tuples[i] = new Tuple<Agency, City>(a, c2);

                    i++;
				}
				if (fils.validateCities() && fille.validateCities()) {
					loop = false;
//					Console.WriteLine(temp);
				}
			} while (loop);

            //			Console.WriteLine(temp.toStringShort());
            //			Console.WriteLine("Fin du crossover");
            List<Solution> result = new List<Solution>();
            result.Add(fils);
            result.Add(fille);
            return result;
        }

        public Boolean validateCities()
        {
			foreach (City c in LieuxDeFormation.MainClass.getCities())
				if (this.getNbPers(c) > CITYCAPACITY)
                    return false;
            return true;
        }

        private double calculateCost()
        {
            double tripFee = 0;
            double agenciesFee = 0;
            List<City> centers = new List<City>();

            for(int i = 0; i < _tuples.Length; i++)
            {
				tripFee += _tuples[i].Item1.distanceTo(_tuples[i].Item2) * TRANSPORTFEE * 2 * _tuples[i].Item1.getNbPers();
				if (!centers.Contains(_tuples[i].Item2))
                {
                    agenciesFee += AGENCYFEE;
					centers.Add(_tuples[i].Item2);
                }
            }
            return tripFee + agenciesFee;
        }

        public Solution getGradientDescendSolution()
        {
            Solution best = this, tmp;
            while ((tmp = best.getBestNeighbor()) != best)
                best = tmp;
            return best;
        }

        public Solution getBestNeighbor()
        {
            Solution best = this;
            foreach (Solution neighbor in Neighbors)
                if (neighbor.Cost < best.Cost)
                    best = neighbor;
            return best;
        }
            
			
        public override string ToString(){
            string str = "";
            for (int i = 0; i < _tuples.Length; i++)
            {
				str += "AGENCY " + " " + _tuples[i].Item1.getId();
                str += " ---> ";
				str += "CITY " + " " + _tuples[i].Item2.getId();
				str += "\n";
            }
			str += "COST : " + Cost;
			str += "\nID : " + id;
            return str;
		}

		public string toStringShort() {
			string str = "";
			int sum = 0;
			foreach (City c in LieuxDeFormation.MainClass.getCities())
			{
				str += " " + this.getNbPers(c);
				sum += this.getNbPers(c);
			}
			return str + " : " + sum;
		}

		public int getPersTot() {
			int sum = 0;
			foreach (City c in LieuxDeFormation.MainClass.getCities())
			{
				sum += this.getNbPers(c);
			}
			return sum;
		}

		public bool Equals(Solution sol)
		{
			for (int i = 0; i < _tuples.Length; i++) {
				if (_tuples[i].Item2.getId() != sol._tuples[i].Item2.getId())
					return false;
			}
			return true;
		}
    }
}
