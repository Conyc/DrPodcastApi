using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPodcastApi.Models
{
	public class Podcast
	{
		public string Id { get; set; }
		public string Title { get; set; }
		public Uri Url { get; set; }
		public string Description { get; set; }
		public List<string> Categories { get; set; }
		public List<PodcastEpisode> Episodes { get; set; }
	}
}
