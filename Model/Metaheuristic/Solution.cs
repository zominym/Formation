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


        public void mutation() {
            int MAX_TRIES = 5;
            this._cost = -1;

            // Selecting one tuple randomly
            int targeted = rand.Next(_tuples.Length);
            // Saving the current city
            Tuple<Agency, City> old = _tuples[targeted];
            do
            {
                // Restore the previous one
                _tuples[targeted] = old;
                // Targeting one Tuple
                targeted = rand.Next(_tuples.Length);
                // Saving current state
                old = _tuples[targeted];
                // Affecting to an other city
                _tuples[targeted] = new Tuple<Agency, City>(_tuples[targeted].Item1, _tuples[rand.Next(_tuples.Length)].Item2);
                MAX_TRIES--;
            } while (!this.validateCities() && MAX_TRIES > 0);

            if (MAX_TRIES <= 0)
            {
                //Console.Write("▀ ");
                // Restore and validate
                _tuples[targeted] = old;

                MAX_TRIES = 50;
                Dictionary<int, City> olders = new Dictionary<int, City>();
                do
                {
                    // Restore previous state
                    if (olders.Count > 0)
                        foreach (int i in olders.Keys)
                            _tuples[i] = new Tuple<Agency, City>(_tuples[i].Item1, olders[i]);

                    olders.Clear();

                    // Chose a targeted city
                    City chosen = _tuples[rand.Next(_tuples.Length)].Item2;
                    // Chose a new location for the occurences of the city
                    City newLocation = MainClass.getCities()[rand.Next(MainClass.getCities().Count)];

                    // Change the targeted city in all tuples concerned
                    for (int i = 0; i < _tuples.Length; ++i)
                    {
                        if (_tuples[i].Item2 == chosen)
                        {
                            // Saving current state
                            olders.Add(i, _tuples[i].Item2);
                            // Changing location
                            _tuples[i] = new Tuple<Agency, City>(_tuples[i].Item1, newLocation);
                        }

                    }
                    MAX_TRIES--;
                } while (!this.validateCities() && MAX_TRIES > 0);


                if (MAX_TRIES <= 0)
                {
                    //Console.Write("▄ ");

                    // Restore
                    if (olders.Count > 0)
                        foreach (int i in olders.Keys)
                            _tuples[i] = new Tuple<Agency, City>(_tuples[i].Item1, olders[i]);

                    // Randomise one tuple
                    _tuples[targeted] = new Tuple<Agency, City>(_tuples[targeted].Item1, MainClass.getCities()[rand.Next(MainClass.getCities().Count)]);

                    // Last check
                    if (!this.validateCities())
                        _tuples[targeted] = old;
                }

            }
        }
                
		private void swap(City a, City b){
			for (int i = 0; i < _tuples.Length; i++) {
				if (_tuples[i].Item2.getId() == a.getId())
					_tuples[i] = new Tuple<Agency, City>(_tuples[i].Item1, b);
				else if (_tuples[i].Item2.getId() == b.getId())
					_tuples[i] = new Tuple<Agency, City>(_tuples[i].Item1, a);
			}
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
				if (c.distanceTo(temp._tuples[n].Item1) > 200)
					loop = true;
			} while (loop);

			temp._cost = -1;
			temp._tuples[n] = new Tuple<Agency, City>(temp._tuples[n].Item1, c);

			return temp;
		}

		public Solution mutate2(int n) {
			Solution temp = new Solution(this);
			City c;
			bool loop = true;
			do {
				double rnd = rand.NextDouble();
				if (rnd < 0.5) {
					List<City> cities = LieuxDeFormation.MainClass.getCities();
					c = cities[rand.Next(cities.Count)];
					temp.swap(temp._tuples[n].Item2, c);
					loop = false;
				}
				else if (rnd < 1) {
					List<City> gUC = getUsedCities();
					c = gUC[rand.Next(gUC.Count)];
					if (temp.getNbPers(c) + temp._tuples[n].Item1.getNbPers() <= CITYCAPACITY)
						loop = false;
//					if (c.distanceTo(temp._tuples[n].Item1) > 200)
//						loop = true;
				}
				else {
					// échanger deux agences de deux centres ouverts entre elles
					c = null;
				}
			} while (loop);

			temp._cost = -1;
			temp._tuples[n] = new Tuple<Agency, City>(temp._tuples[n].Item1, c);

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

        public List<Solution> recombination(Solution mother)
        {
            int MAX_TRIES = 10;
            Solution son, daughter;

            return null;
        }

        public List<Solution> crossover(Solution mother)
		{
            int MAX_TRIES = 10;
//			Console.WriteLine("Debut du crossover");
			Solution son, daughter;
			
            while (MAX_TRIES > 0)
            {
				son = new Solution(this);
                daughter = new Solution(mother);

				for(int i = rand.Next(_tuples.Length); i < _tuples.Length ; ++i)
                {
                    Agency a = this._tuples[i].Item1;
                    son._tuples[i] = new Tuple<Agency, City>(a, mother._tuples[i].Item2);
                    daughter._tuples[i] = new Tuple<Agency, City>(a, this._tuples[i].Item2);
                }
               
				if (son.validateCities() && daughter.validateCities())
                {
                    List<Solution> result = new List<Solution>();
                    result.Add(son);
                    result.Add(daughter);
                    return result;
                }
                    
                MAX_TRIES--;
			}
            Console.Write("■ ");
            return null;
        }

        public Boolean validateCities()
        {
            Dictionary<City, int> cities = new Dictionary<City, int>();
            for (int i = 0; i < _tuples.Length; ++i)
            {
                City c = _tuples[i].Item2;
                if (!cities.ContainsKey(c))
                    cities.Add(c, _tuples[i].Item1.getNbPers());
                else
                {
                    cities[c] += _tuples[i].Item1.getNbPers();
                    if (cities[c] > CITYCAPACITY)
                        return false;
                }
                    
            }
            
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
				if (!containCity(centers, _tuples[i].Item2))
                {
                    agenciesFee += AGENCYFEE;
					centers.Add(_tuples[i].Item2);
                }
            }

            return tripFee + agenciesFee;
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
			str += "COST : " + this.calculateCost();
			str += "\nID : " + id;
            return str;
		}

		public string toStringShort() {
			string str = "";
			int sum = 0;
            int nbCenters = 0, number = 0;
			foreach (City c in LieuxDeFormation.MainClass.getCities())
			{
                number = this.getNbPers(c);
                str += " " + number;
				sum += number;
                if (number > 0)
                    nbCenters++;
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
