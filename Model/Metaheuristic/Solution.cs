using System;
using System.Collections.Generic;
using TrainingProblem;

namespace Metaheuristic
{
    public class Solution
    {
        const double transportFee = 0.4;
        const int agencyFee = 3000;

        private Dictionary<Agency, City> _value;
        private List<Solution> _neighbors;

        public Solution(Dictionary<Agency, City> value)
        {
            _value = value;
              
        }

        public Dictionary<Agency, City> Value
        {
            get { return _value; }
        }

        public List<Solution> Neighbors
        {
            get { return _neighbors; }
        }

        private void buildNeighborhood()
        {
            //TODO
        }

        public double cost()
        {
            double tripFee = 0;
            double agenciesFee = 0;
            List<City> centers = new List<City>();

            foreach(Agency a in this._value.Keys)
            {
                City c = this._value[a];
                tripFee += a.distanceTo(c) * transportFee * a.getNbPers();
                if(!centers.Contains(c))
                {
                    agenciesFee += agencyFee;
                    centers.Add(c);
                }
            }
            return tripFee + agenciesFee;
        }
    }
}
