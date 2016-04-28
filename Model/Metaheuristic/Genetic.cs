﻿using System;
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
            double costMin = min.Cost;
            double curentCost;
            foreach (Solution solution in population) {
                curentCost = solution.Cost;
                if (curentCost < costMin)
                {
                    min = solution;
                    costMin = curentCost;
                }
                    
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
            double ProbaMutation = 0.001;
            double ProbaMagic = 0.0;
            List<Solution> nextPopulation, elite, crossResult, currentPopulation = buildPopulation();
            Solution bestSolution = getBestSolution(currentPopulation), currentBest;
            int bestSolutionCost = (int) bestSolution.Cost;

            Console.WriteLine("Cout\t\tAgences\t\tProgression");

            for (int i = 0; i < Iterations; i++) {
                // Creation of the new population
                nextPopulation = new List<Solution>();

                // Selection of the representative
                elite = RouletteVladof(currentPopulation, currentPopulation.Count / 2);
                
                // Reproduction
                while(nextPopulation.Count < currentPopulation.Count) {
                    // Combinaison
                    if((crossResult = elite[rand.Next(elite.Count)].experiment(elite[rand.Next(elite.Count)])) != null)
                        nextPopulation.AddRange(crossResult);

                    // Mutation
                    if (nextPopulation.Count > 0)
                    {
                        if(rand.NextDouble() > (1 - ProbaMagic))
						    nextPopulation[rand.Next(nextPopulation.Count)].trick();
                        if(rand.NextDouble() > (1 - ProbaMutation))
                            nextPopulation[rand.Next(nextPopulation.Count)].badassMutation();
                    }
                        
                }

                // Next generation is the new generation
                currentPopulation = nextPopulation;
                
                // See who's the bigest
                currentBest = getBestSolution(currentPopulation);
                int currentBestCost = (int) currentBest.Cost;
                Console.SetCursorPosition(0, Console.CursorTop);              
                Console.Write(currentBestCost  + "\t\t" + currentBest.nbCenters + "\t\t" + i + "/" + Iterations);

                // Take the legend
                if (bestSolutionCost > currentBestCost)
                {
                    bestSolution = getBestSolution(currentPopulation);
                    bestSolutionCost = (int) bestSolution.Cost;
                }
                    
            }

            // Show the legend
            return bestSolution;
        }
    }
}

