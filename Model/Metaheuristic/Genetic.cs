using System;
using TrainingProblem;
using System.Collections.Generic;
using System.Linq;

namespace Metaheuristic
{
    public class Genetic
    {
        private Agency[] _agencies;
        private City[] _cities;
        private List<Solution> _population;
        private int _iterations, _populationSize;
        private static Random rand = new Random();

        public Genetic(Agency[] agencies, City[] cities, int iterations, int populationSize) {
            _agencies = agencies;
            _cities = cities;
            _iterations = iterations;
            _populationSize = populationSize;
        }

        int PopulationSize {
            get { return _populationSize;}
        }

        int Iterations {
            get { return _iterations; }
        }

        List<Solution> Population {
            get {
                if (_population == null)
                    buildPopulation();
                return _population;
            }
        }

        public void buildPopulation() {
            _population = new List<Solution>();
            for (int i = 0; i < PopulationSize; i++)
            {
                _population.Add(Solution());
            }
        }

        public List<Solution> getBestSolutions(List<Solution> population, int nbBest){
            List<Solution> bestSolutions = new List<Solution>();
            population.OrderBy(x => x.Cost);
            for (int i = 0; i < nbBest; i++) {
                bestSolutions.Add(population.ElementAt(i));
            }
            return bestSolutions;
        }

        public List<Solution> RouletteSelection(List<Solution> population, int nbToTake){
            double totalCost = 0;
            List<Solution> result = new List<Solution>();

            //Take the best solution
            population.OrderBy(x => x.Cost);
            int max = population.ElementAt(population.Count - 1);



            List<int> poids = new List<int>();
            foreach (Solution solution in population) {
                poids.Add(max - solution.Cost);
                totalCost += (max - solution.Cost);
            }


            for (int i = 0; i < nbToTake; ++i) {
                int pick = rand.NextDouble() * totalCost;
                for (int j = 0; j < poids.Count; j++) {
                    if (pick < poids[j]) {
                        result.Add(population.ElementAt(j));
                        break;
                    }
                    else
                        pick -= poids[j];
                }
            }


            return result;
        }

        public Solution getSolution(){
            buildPopulation();
            for (int i = 0; i < Iterations; i++) {
                
            }
            return null;
        }
    }
}

