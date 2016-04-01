using System;
using System.Collections.Generic;
using TrainingProblem;

namespace Metaheuristic
{
    public class Solution
    {

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

        public City Cities
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
            this.Cities[b] = this.Cities[a];
        }
    }
}
