using AHH.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHH.Research
{
	class Researcher
	{

		Dictionary<string, ResearchData> research;
		Dictionary<string, Research> currentResearch { get; set; }

		public Researcher()
		{
		}

		public void Update(Overseer os)
		{


		}

		public void ResearchComplete(Research rs, Overseer os)
		{

			currentResearch.Remove(rs.Data.Name);
		}

	}
}
