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
			nbPers = int.Parse(csvLine.Split (';')[5]);
		}

		public override string ToString ()
		{
			return id + " : " + name + " ; " + codepostal + " (" + longitude + ";" + latitude + ") : " + nbPers;
		}

		public int getNbPers()
		{
			return nbPers;
		}
	}
}

