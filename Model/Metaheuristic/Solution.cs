using System;
using System.Collections.Generic;
using TrainingProblem;
using LieuxDeFormation;

namespace Metaheuristic
{
    public class Solution
    {
        const double transportFee = 0.4;
        const int agencyFee = 3000;
        const int cityCapacity = 60;

		private double _cost = -1;
		private List<City> _cities = new List<City>();
        private List<Solution> _neighbors = null;
		private Tuple<Agency, City>[] _tuples = new Tuple<Agency, City>[MainClass.getAgencies().Count];

		// Constructeur de solution aléatoire
		public Solution() {
            Random rand = new Random();
            foreach (City city in MainClass.getCities())
                _cities.Add(new City(city));
            int tirage, capacityRequired, i=0;
            foreach(Agency a in MainClass.getAgencies())
            {
                do
                {
                    tirage = rand.Next(_cities.Count);
                    capacityRequired = a.getNbPers() + _cities[tirage].getNbPers();
                } while (capacityRequired <= cityCapacity);

                _cities[tirage].setNbPers(capacityRequired);

                _tuples[i] = new Tuple<Agency, City>(a, _cities[tirage]);

                i++;
            }
           
        }

		// Constructeur de solution aléatoire avec distance maximum entre deux villes
		public Solution(int distanceMax, int increment = 10, int refusMax = 100) {
			foreach (City city in MainClass.getCities())
				_cities.Add(new City(city));

			Random rand = new Random();
			City c = new City();

			foreach (Agency a in MainClass.getAgencies()) {
				int refus = 0;
				bool continuer = false;
				do {
					c = _cities[rand.Next(_cities.Count)];

					if (refus >= refusMax) {
						refus = 0;
						distanceMax += increment;
					}

					if (c.getNbPers() + a.getNbPers() <= 60)
						continuer = true;

					if (a.distanceTo(c) > distanceMax) {
						continuer = false;
						refus++;
					}
				} while (continuer);
				_tuples = _tuples + new Tuple(a, c);
				c.setNbPers(c.getNbPers() + a.getNbPers());
			}

		}

		public Solution (Solution s) {
			_cost = s.Cost;
			_tuples = s._tuples;
			_cities = s.Cities;
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

        public City[] Cities
        {
            get { return _cities; }
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
            for (int i = 0; i < Agencies.Length; i++)
            {
                for (int j = i + 1; j < Agencies.Length; j++) 
                {
                    Solution tmp = swap(i, j);
                    _neighbors.Add(tmp);
                }
            }
        }

		private Solution swap(int a, int b){
			Solution temp = new Solution(this);
			City tmp = temp.Cities[a];
			temp.Cities[a] = temp.Cities[b];
			temp.Cities[b] = tmp;
			temp._cost = -1;
			return temp;
		}

		public Solution mutate(City[] cities){   
			Solution temp = new Solution(this);
			Random rand = new Random();
			temp._cities[rand.Next(_cities.Length)] = cities[rand.Next(cities.Length)];
			temp._cost = -1;
			return temp;
		}

        private double calculateCost()
        {
            double tripFee = 0;
            double agenciesFee = 0;
            List<City> centers = new List<City>();

            for(int i = 0; i < Agencies.Length; i++)
            {
                City c = Cities[i];
                tripFee += Agencies[i].distanceTo(c) * transportFee * Agencies[i].getNbPers();
                if (!centers.Contains(c))
                {
                    agenciesFee += agencyFee;
                    centers.Add(c);
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
            for (int i = 0; i < Agencies.Length; i++)
            {
				str += "AGENCY " + " " + Agencies[i].getId();
                str += " ---> ";
				str += "CITY " + " " + Cities[i].getId();
				str += "\n";
            }
            return str;
        }
    }
}
