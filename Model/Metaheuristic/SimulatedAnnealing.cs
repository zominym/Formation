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
            t = initialTemperature;
            double µ = micro;//0.5
            int param = (int) Math.Floor(0.05*iteration);
            Solution xmin, xn, xnn, x0 = new Solution();
            xmin = x0;
            xn = x0;
            double fmin = xmin.Cost;
            Console.WriteLine("Param: "+param);
            for (int i = 0; i < iteration; i++)
            {
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write("ITER: " + i + " --> " + "actuel: " + xn.Cost + " min: " + xmin.Cost + " t: "+t);
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
                if ((xn.Cost == xnn.Cost) & (t <= 1))
                    break;
                xn = xnn;
                if (i % param == 0)
                    t = µ * t;
            }
            return xmin;
        }
    }
}