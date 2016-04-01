using System;

namespace TrainingProblem
{
	public class Agency : City
	{
		private int nbPers;

		public Agency() : base()
		{
			nbPers = 0;
		}

		public Agency(string csvLine) : base(csvLine)
		{
			nbPers = csvLine.Split (';')[5];
		}
	}
}

