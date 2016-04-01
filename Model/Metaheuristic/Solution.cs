using System;
using System.Collections.Generic;
using TrainingProblem;

namespace Metaheuristic
{
    public class Solution
    {
        const double transportFee = 0.4;
        const int agencyFee = 3000;

        private Agency[] _agencies;
        private City[] _cities;
        private List<Solution> _neighbors = null;

        public Solution(Agency[] agencies, City[] cities){
            _agencies = agencies;
            _cities = cities;
        }

        public List<Solution> Neighbors
        {
            get { 
                if (_neighbors == null)
                    buildNeighborhood();
                return _neighbors;
            }
        }

        public Agency[] Agencies
        {
            get { return _agencies; }
        }

        public City[] Cities
        {
            get { return _cities; }
        }
            
        private void buildNeighborhood()
        {
            _neighbors = new List<Solution>();
            for (int i = 0; i < Agencies.Length; i++)
            {
                for (int j = i + 1; j < Agencies.Length; j++) 
                {
                    Solution tmp = new Solution(Agencies, Cities);
                    tmp.swap(i, j);
                    _neighbors.Add(tmp);
                }
            }
        }

        private void swap(int a, int b){
            City tmp = this.Cities[a];
            this.Cities[a] = this.Cities[b];
            this.Cities[b] = tmp;
		}

		public void mutate(City[] cities)
		{   
			Random rand = new Random();
			_cities[rand.Next(_cities.Length)] = cities[rand.Next(cities.Length)];
		}

        public double cost()
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
