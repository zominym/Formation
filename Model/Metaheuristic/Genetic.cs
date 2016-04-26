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

        public List<Solution> RouletteVladof(List<Solution> population, int nbToTake)
        {
            // place your bets, put your chips on the table
            List<Solution> result = new List<Solution>();
            Dictionary<double, Solution> wheel = new Dictionary<double, Solution>();

            double T = 0, sum = 0;
            foreach (Solution sol in population)
            {
                T += sol.Cost;
            }
            double last = 0;
            foreach (Solution sol in population)
            {
                sum += (T - sol.Cost);
                wheel.Add(last + (T - sol.Cost), sol);
                last = wheel.Last().Key;
            }
            
            // end of bets, nothing goes on the table
            double aleaJactaEst;
            for (int i = 0; i < nbToTake; ++i)
            {
                aleaJactaEst = rand.NextDouble() * sum;
                foreach (double key in wheel.Keys)
                {
                    if (key >= aleaJactaEst)
                    {
                        result.Add(wheel[key]);
                        break;
                    }
                }
                    
            }

            return result;
        }

        public List<Solution> RouletteSelection(List<Solution> population, int nbToTake){
            double totalEfficiency = 0;
            List<Solution> result = new List<Solution>();

            foreach (Solution sol in population)
            {
                totalEfficiency += 1 / sol.Cost;
            }

            List<Solution> tmp = new List<Solution>(population);

            int nbTaken = 0;
            while (nbTaken < nbToTake)
            {
                double incrementation = 0; //TODO Change the name
                double alpha = rand.NextDouble() * totalEfficiency;
//                Console.ReadLine();
                for (int i = 0; i < tmp.Count; i++) {
                    if (((1 / tmp[i].Cost) + incrementation) > alpha)
                    {
                        result.Add(tmp[i]);
                        tmp.Remove(tmp[i]);
                        totalEfficiency = 0;
                        foreach (Solution sol in tmp)
                        {
                            totalEfficiency += 1 / sol.Cost;
                        }
                        nbTaken++;
                        break;
                    }
                    else
                    {
                        incrementation += (1 / tmp[i].Cost);
                    }
                }
            }

            //Take the --best WORST solution
            /*Solution max = population.First();
            foreach (Solution solution in population) {
                if (max.Cost < solution.Cost)
                    max = solution;
            }*/

            /*
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
            }*/
            return result;
        }

        public Solution getSolution() {
            double ProbaCross = 0.99;
            List<Solution> nextPopulation, elite, crossResult, currentPopulation = buildPopulation();
            Solution bestSolution = getBestSolution(currentPopulation);
            
            for (int i = 0; i < Iterations; i++) {
                //Console.WriteLine(" ITE " + i);
				nextPopulation = RouletteVladof(currentPopulation, currentPopulation.Count / 2);
                elite = new List<Solution>(nextPopulation);
                
                while(nextPopulation.Count < currentPopulation.Count) {
                    if((crossResult = elite[rand.Next(elite.Count)].crossover(elite[rand.Next(elite.Count)])) != null)
                    nextPopulation.AddRange(crossResult);
                    if (ProbaCross > rand.NextDouble())
						nextPopulation[rand.Next(nextPopulation.Count)].mutate();
                }
                currentPopulation = nextPopulation;
				/*
					foreach (Solution s in currentPopulation) {
						//Console.WriteLine(s);
					}
					foreach (Solution s in currentPopulation) {
						//Console.WriteLine(s.Cost);
						//Console.WriteLine(s.id);
					}
                */
                Console.WriteLine(getBestSolution(currentPopulation).Cost);
                if (bestSolution.Cost > getBestSolution(currentPopulation).Cost)
                    bestSolution = getBestSolution(currentPopulation);
            }
            return bestSolution.getGradientDescendSolution();
        }
    }
}

