using System;
using System.Collections.Generic;
using TrainingProblem;

namespace Metaheuristic
{
    public class Solution
    {

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
    }
}
