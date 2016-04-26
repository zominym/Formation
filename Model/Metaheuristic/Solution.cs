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
		public Tuple<Agency, City>[] _tuples = new Tuple<Agency, City>[MainClass.getAgencies().Count];
		private Random rand = LieuxDeFormation.MainClass.rand;
		public static double nbSuccess = 1;
		public static double nbTries = 1;

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

		public Solution(int nbCentres) {
			List<City> _cities = LieuxDeFormation.MainClass.getCities();
			List<Agency> _agencies = LieuxDeFormation.MainClass.getAgencies();
			List<City> previous = new List<City>();
			id = ID + 1;
			ID++;
			int tirage, capacityRequired;
			for (int i = 0; i < nbCentres; i++) {
				do
				{
					tirage = rand.Next(_cities.Count);
					capacityRequired = _agencies[i].getNbPers() + this.getNbPers(_cities[tirage]);
				} while (capacityRequired > CITYCAPACITY || previous.Contains(_cities[tirage]));

				_tuples[i] = new Tuple<Agency, City>(_agencies[i], _cities[tirage]);
				previous.Add(_cities[tirage]);
			}
			_cities = getUsedCities();
			for (int i = nbCentres; i < _agencies.Count; i++) {
				do
				{
					tirage = rand.Next(_cities.Count);
					capacityRequired = _agencies[i].getNbPers() + this.getNbPers(_cities[tirage]);
				} while (capacityRequired > CITYCAPACITY);

				_tuples[i] = new Tuple<Agency, City>(_agencies[i], _cities[tirage]);
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
				return calculateCost();
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

		public Solution swap(City a, City b){
			Solution temp = new Solution(this);
			for (int i = 0; i < temp._tuples.Length; i++) {
				if (temp._tuples[i].Item2.getId() == a.getId())
					temp._tuples[i] = new Tuple<Agency, City>(temp._tuples[i].Item1, b);
				else if (temp._tuples[i].Item2.getId() == b.getId())
					temp._tuples[i] = new Tuple<Agency, City>(temp._tuples[i].Item1, a);
			}
			return temp;
		}

		public Solution give(City a, City b){
			Solution temp = new Solution(this);
			int nbPers = 0;
			for (int i = 0; i < temp._tuples.Length; i++) {
				if (temp._tuples[i].Item2.getId() == a.getId()) {
					temp._tuples[i] = new Tuple<Agency, City>(temp._tuples[i].Item1, b);
					nbPers += temp._tuples[i].Item1.getNbPers();
				}
				else if (temp._tuples[i].Item2.getId() == b.getId())
					nbPers += temp._tuples[i].Item1.getNbPers();
				if (nbPers > CITYCAPACITY)
					return null;
			}
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
				double rnd = rand.NextDouble();
				if (rnd < 0.5)
					c = _cities[rand.Next(_cities.Count)];
				else {
					List<City> gUC = getUsedCities();
					c = gUC[rand.Next(gUC.Count)];
				}
				if (temp.getNbPers(c) + temp._tuples[n].Item1.getNbPers() <= CITYCAPACITY)
					loop = false;
			} while (loop);

			temp._cost = -1;
			temp._tuples[n] = new Tuple<Agency, City>(temp._tuples[n].Item1, c);

			return temp;
		}

		public Solution mutate2(int n) {
			Solution temp = new Solution(this);
			bool loop = true;
			do {
//				double centresIdeal = getPersTot()/CITYCAPACITY;
//				double offset = (centresIdeal - getUsedCities().Count);
//				double proba = 0;
//				if (offset != 0)
//					proba =  offset / centresIdeal;
				if (rand.NextDouble() < 0.5 /*nbSuccess / nbTries*/) {
					List<City> gUC = getUsedCities();
					City c = gUC[rand.Next(gUC.Count)];
					if (temp.getNbPers(c) + temp._tuples[n].Item1.getNbPers() <= CITYCAPACITY) {
						loop = false;
						nbSuccess += 1;
						temp._tuples[n] = new Tuple<Agency, City>(temp._tuples[n].Item1, c);
					}
					//					if (c.distanceTo(temp._tuples[n].Item1) > 200)
					//						loop = true;
					nbTries += 1;
				}
				else if (rand.NextDouble() < 0.5) {
					List<City> cities = LieuxDeFormation.MainClass.getCities();
					City c = cities[rand.Next(cities.Count)];
					temp.swap(temp._tuples[n].Item2, c);
					loop = false;
				}
				else {
					// échanger deux agences de deux centres ouverts entre elles
					int i1 = rand.Next(_tuples.Length);
					int i2 = rand.Next(_tuples.Length);

					Agency a1 = temp._tuples[i1].Item1;
					Agency a2 = temp._tuples[i2].Item1;

					City c1 = temp._tuples[i1].Item2;
					City c2 = temp._tuples[i2].Item2;

					if (temp.getNbPers(c1) + a2.getNbPers() - a1.getNbPers() <= CITYCAPACITY
						&& temp.getNbPers(c2) + a1.getNbPers() - a2.getNbPers() <= CITYCAPACITY) {
						loop = false;
						temp._tuples[i1] = new Tuple<Agency, City>(a1, c2);
						temp._tuples[i2] = new Tuple<Agency, City>(a2, c1);
					}
				}
			} while (loop);

//			temp._cost = -1;

			return temp;
		}

		public List<City> getUsedCities() {
			List<City> ret = new List<City>();
			for (int i = 0; i < _tuples.Length; i++) {
				if (_tuples[i] != null && !ret.Contains(_tuples[i].Item2))
					ret.Add(_tuples[i].Item2);
			}
			return ret;
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
            double dist = 0;
			double totalTripFee = 0;
            double agenciesFee = 0;
//            List<City> centers = new List<City>();

            for(int i = 0; i < _tuples.Length; i++)
            {
				dist += _tuples[i].Item1.distanceTo(_tuples[i].Item2) * _tuples[i].Item1.getNbPers();
            }
			totalTripFee = dist * TRANSPORTFEE * 2;
			agenciesFee = getUsedCities().Count * AGENCYFEE;
			return totalTripFee + agenciesFee;
        }

		public bool containCity(List<City> cities, City city) {
			foreach (City c in cities)
				if (c.getId() == city.getId())
					return true;
			return false;
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
			return str + " ; nbPersTot : " + sum + " ; nbCentres : " + getUsedCities().Count;
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
