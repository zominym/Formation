using System;

namespace TrainingProblem
{
	public class Center
	{
		City city;
		int nbPers;

		public Center()
		{

		}

		public Center(City c)
		{
			city = c;
			nbPers = 0;
		}

		public Center(City c, int n)
		{
			city = c;
			nbPers = n;
		}

		public Center(Center c)
		{
			city = c.getCity();
			nbPers = c.getNbPers();
		}

		public int getNbPers()
		{
			return nbPers;
		}

		public void setNbPers(int n)
		{
			nbPers = n;
		}
			
		public City getCity()
		{
			return city;
		}

	}
}

