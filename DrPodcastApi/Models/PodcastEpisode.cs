using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPodcastApi.Models
{
	public class PodcastEpisode
	{
		public string Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public DateTimeOffset PublicationDate { get; set; }
	}
}
