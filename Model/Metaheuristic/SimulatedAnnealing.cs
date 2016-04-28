using System;
using TrainingProblem;
using System.Collections.Generic;

namespace Metaheuristic
{
    public class SimulatedAnnealing
    {
        double t = 0.0;
        private Random rand = LieuxDeFormation.MainClass.rand;

        public SimulatedAnnealing(List<Agency> agencies, List<City> cities)
        {
            
        }

        public Solution run(int iteration, double micro, double initialTemperature){

            List<Solution> population = new List<Solution>();

            t = initialTemperature;
            double µ = micro;//0.5
            int param = (int) Math.Floor(0.10*iteration);
            Solution xmin, xn, xnn, x0 = new Solution();
            xmin = x0;
            xn = x0;
            double fmin = xmin.Cost;

            for (int i = 0; i < iteration; i++)
            {
                Console.WriteLine("ITER: " + i + " --> " + "actuel: " + xn.Cost + " min: " + xmin.Cost);
                Solution y = xn.mutate(rand.Next(LieuxDeFormation.MainClass.getAgencies().Count));//xn.Neighbors2[rand.Next(xn.Neighbors2.Count)];
                double Δf = (y.Cost/1) - (xn.Cost/1);
                if (Δf <= 0)
                {
                    xnn = y;
                    if ((xnn.Cost / 1) < (xmin.Cost / 1))
                        xmin = xnn;
                }
                else {
                    if (rand.NextDouble() <= Math.Exp(-Δf / t))
                        xnn = y;
                    else
                        xnn = xn;
                }
                if ((xn.Cost == xnn.Cost) & (t <= 0))
                    break;
                xn = xnn;
                if (i % param == 0)
                    t = µ * t;
            }
            return xmin;
        }
    }
}