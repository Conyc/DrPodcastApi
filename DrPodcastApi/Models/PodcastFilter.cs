using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPodcastApi.Models
{
	public class PodcastFilter
	{
		public DateTimeOffset? PublicationDateStart { get; set; }
		public DateTimeOffset? PublicationDateEnd { get; set; }
		public int? Limit { get; set; }
	}
}
