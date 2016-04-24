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

        public List<Solution> buildPopulation() {
            List<Solution> population;
            population = new List<Solution>();
            for (int i = 0; i < PopulationSize; i++)
            {
                population.Add(new Solution());
            }
            return population;
        }

        public Solution getBestSolution(List<Solution> population){
			Solution min = population.First();
            foreach (Solution solution in population) {
                if (solution.Cost < min.Cost)
                    min = solution;
            }
            return min;
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
            double ProbaCross = 0.5;
            List<Solution> nextPopulation, tmp, currentPopulation = buildPopulation();
            Solution bestSolution = getBestSolution(currentPopulation);
            for (int i = 0; i < Iterations; i++) {
				if (i % 10 == 0)
                	Console.Write(" ITE "+i);
				nextPopulation = RouletteSelection(currentPopulation, currentPopulation.Count / 2);
                tmp = new List<Solution>(nextPopulation);
                for (int j = nextPopulation.Count; j < currentPopulation.Count; j++) {
					if (ProbaCross > rand.NextDouble()) {
						int rand1 = rand.Next(tmp.Count);
						int rand2 = rand.Next(tmp.Count);
						Solution s = tmp[rand1].crossover(tmp[rand2]);
						nextPopulation.Add(s);
					}
                    else
                        nextPopulation.Add(tmp[rand.Next(tmp.Count)].mutate());
                }
                currentPopulation = nextPopulation;
				if (i % 100 == 0) {
					foreach (Solution s in currentPopulation)
						Console.WriteLine(s.Cost);

					Console.WriteLine(getBestSolution(currentPopulation).Cost);
				}
                if (bestSolution.Cost > getBestSolution(currentPopulation).Cost)
                    bestSolution = getBestSolution(currentPopulation);
            }
            return bestSolution.getGradientDescendSolution();
        }
    }
}

