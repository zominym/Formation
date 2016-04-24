using System;
using TrainingProblem;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Metaheuristic
{
    public class Genetic
    {
        //private List<Agency> _agencies;
        private List<City> _cities;
        private List<Solution> _population;
        private int _iterations, _populationSize;
		private static Random rand = LieuxDeFormation.MainClass.rand;

        public Genetic(List<Agency> agencies, List<City> cities, int iterations, int populationSize) {
            //_agencies = agencies;
            _cities = cities;
            _iterations = iterations;
            _populationSize = populationSize;
        }

        int PopulationSize {
            get { return _populationSize;}
        }

        List<City> Cities {
            get { return _cities; }
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
                _population.Add(new Solution());
            }
            Console.WriteLine(_population.Count);
        }

        public Solution getBestSolution(List<Solution> population){
            Solution max = population.ElementAt(0);
            foreach (Solution solution in population) {
                if (solution.Cost > max.Cost)
                    max = solution;
            }
            return max;
        }

        public List<Solution> RouletteSelection(List<Solution> population, int nbToTake){
            double totalCost = 0;
            List<Solution> result = new List<Solution>();

            //Take the best solution
            Solution max = population.First();
            foreach (Solution solution in population) {
                if (max.Cost < solution.Cost)
                    max = solution;
            }


            List<double> poids = new List<double>();
            foreach (Solution solution in population) {
                poids.Add(max.Cost - solution.Cost);
                totalCost += (max.Cost - solution.Cost);
            }
                
            while (nbToTake > 0)
            {
                double pick = rand.NextDouble() * totalCost;
                for (int j = 0; j < poids.Count; j++)
                {
                    if (pick <= poids[j]) {
                        result.Add(population.ElementAt(j));
                        nbToTake--;
                        break;
                    }
                    else
                        pick -= poids[j];
                }
            }
            return result;
        }

        public Solution getSolution(){
            double ProbaCross = 0.9;
            buildPopulation();
            List<Solution> nextPopulation, tmp, currentPopulation = Population;
            for (int i = 0; i < Iterations; i++) {
                Console.WriteLine("ITE "+i);
                nextPopulation = RouletteSelection(currentPopulation, currentPopulation.Count/2);
                tmp = new List<Solution>(nextPopulation);
                for (int j = nextPopulation.Count; j < currentPopulation.Count; j++) {
                    if(ProbaCross > rand.NextDouble())
                        nextPopulation.Add(tmp[rand.Next(tmp.Count)].crossover(tmp[rand.Next(tmp.Count)]));
                    else
                        nextPopulation.Add(tmp[rand.Next(tmp.Count)].mutate());
                }
                currentPopulation = nextPopulation;
            }
            return getBestSolution(currentPopulation).getGradientDescendSolution();
        }
    }
}

