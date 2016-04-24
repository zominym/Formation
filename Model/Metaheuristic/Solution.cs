﻿using System;
using System.Collections.Generic;
using TrainingProblem;
using LieuxDeFormation;

namespace Metaheuristic
{
    public class Solution
    {
		double TRANSPORTFEE = 0.4;
		int AGENCYFEE = 3000;
		const int CITYCAPACITY = 60;

		private double _cost = -1;
		private List<City> _cities = new List<City>();
        private List<Solution> _neighbors = null;
		private Tuple<Agency, City>[] _tuples = new Tuple<Agency, City>[MainClass.getAgencies().Count];
		private Random rand = LieuxDeFormation.MainClass.rand;

		// Constructeur de solution aléatoire
		public Solution() {
            foreach (City city in MainClass.getCities())
                _cities.Add(new City(city));
            int tirage, capacityRequired, i = 0;
            foreach(Agency a in MainClass.getAgencies())
            {
                do
                {
                    tirage = rand.Next(_cities.Count);
                    capacityRequired = a.getNbPers() + _cities[tirage].getNbPers();
                } while (capacityRequired > CITYCAPACITY);

                _cities[tirage].setNbPers(capacityRequired);

                _tuples[i] = new Tuple<Agency, City>(a, _cities[tirage]);

                i++;
            }
           
        }

		// Constructeur de solution aléatoire avec distance maximum entre deux villes
		public Solution(int distanceMax, int increment = 10, int refusMax = 100) {
			foreach (City city in MainClass.getCities())
				_cities.Add(new City(city));

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

					if (c.getNbPers() + a.getNbPers() <= CITYCAPACITY)
						loop = false;

					if (a.distanceTo(c) > distanceMax) {
						loop = true;
						refus++;
					}
				} while (loop);
                _tuples[i] = new Tuple<Agency, City>(a, c);
                i++;
				c.setNbPers(c.getNbPers() + a.getNbPers());
			}

		}

		public Solution (Solution s) {
			_cost = s.Cost;
			for (int i = 0; i < _tuples.Length; i++)
				_tuples[i] = new Tuple<Agency, City>(s._tuples[i].Item1, new City(s._tuples[i].Item2));
            _cities = new List<City>();
            foreach (City c in s._cities)
                _cities.Add(new City(c));
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

        public List<City> Cities
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

			temp._tuples[a].Item2.setNbPers(temp._tuples[a].Item2.getNbPers() + temp._tuples[a].Item1.getNbPers() - temp._tuples[b].Item1.getNbPers());
			temp._tuples[b].Item2.setNbPers(temp._tuples[b].Item2.getNbPers() + temp._tuples[b].Item1.getNbPers() - temp._tuples[a].Item1.getNbPers());

			if (temp._tuples[a].Item2.getNbPers() > CITYCAPACITY || temp._tuples[b].Item2.getNbPers() > CITYCAPACITY)
				return null;
			return temp;
		}

		public Solution mutate(){   
            /*List<City> cities = MainClass.getCities();
			Solution temp = new Solution(this);
			bool loop = true;
			do {
				int idx = rand.Next(_tuples.Length);
				City c = cities[rand.Next(cities.Count)];
				temp._tuples[idx].Item2.setNbPers(temp._tuples[idx].Item2.getNbPers() - temp._tuples[idx].Item1.getNbPers());
				temp._tuples[idx] = Tuple.Create(temp._tuples[idx].Item1, c);
				temp._tuples[idx].Item2.setNbPers(temp._tuples[idx].Item2.getNbPers() + temp._tuples[idx].Item1.getNbPers());
				temp._cost = -1;

				if (temp._tuples[idx].Item2.getNbPers() <= CITYCAPACITY)
					loop = false;
			} while (loop);
			return temp;*/
            return new Solution();
		}

        public Solution crossover(Solution y)
		{
			//TODO crossover renvoie parfois des solutions avec des villes ayant plus de 100 personnes !!!!!!
//			Console.WriteLine("Debut du crossover");
			Solution temp;
			bool loop = true;
			do {
				temp = new Solution(this);
				temp._cities = new List<City>();              

				int i = 0;
//				Console.WriteLine("CROSSOVER");
				foreach (Agency a in MainClass.getAgencies()) {

//					Console.WriteLine("PARENT 1 : ");
//					Console.WriteLine(this._tuples[i].Item2);
//					Console.WriteLine("PARENT 2 : ");
//					Console.WriteLine(y._tuples[i].Item2);
					City c;
					if (rand.NextDouble() < 0.5) {
//						Console.WriteLine("SELECTED PARENT 1");
						c = new City(this._tuples[i].Item2);
					}
					else {
//						Console.WriteLine("SELECTED PARENT 2");
						c = new City(y._tuples[i].Item2);
					}
					City cp = temp.retrieve(c);
					if (cp != null)
						c = cp;
					else
						temp._cities.Add(c);
					temp._tuples[i] = new Tuple<Agency, City>(a, c);

					i++;
				}
				temp.updateCities();
				if (temp.validateCities()) {
					loop = false;
//					Console.WriteLine(temp);
				}
			} while (loop);

//			Console.WriteLine(temp.toStringShort());
//			Console.WriteLine("Fin du crossover");
            return temp;
        }

		public void updateCities()
		{
			foreach (City c in _cities) {
				c.setNbPers(0);
			}
			for (int i = 0; i < _tuples.Length; i++) {
				_tuples[i].Item2.setNbPers(_tuples[i].Item2.getNbPers() + _tuples[i].Item1.getNbPers());
			}

		}

        public Boolean validateCities()
        {
            foreach (City c in _cities)
                if (c.getNbPers() > CITYCAPACITY)
                    return false;
            return true;
        }

		public City retrieve(City c)
		{
			City ret = null;
			foreach (City city in _cities) {
				if (city.Equals(c))
					ret = c;
			}
			return ret;
		}

        private double calculateCost()
        {
            double tripFee = 0;
            double agenciesFee = 0;
            List<City> centers = new List<City>();

            for(int i = 0; i < _tuples.Length; i++)
            {
				tripFee += _tuples[i].Item1.distanceTo(_tuples[i].Item2) * TRANSPORTFEE * _tuples[i].Item1.getNbPers();
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
            return str;
		}

		public string toStringShort() {
			string str = "";
			for (int i = 0; i < _tuples.Length; i++)
			{
				str += " " + _tuples[i].Item2.getNbPers();
			}
			return str;
		}
    }
}
