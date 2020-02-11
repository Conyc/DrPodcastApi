using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPodcastApi.Models
{
	public interface IPodcastRepository
	{
		Task<Podcast> GetPodcastAsync(string id);
		Task<Podcast> GetPodcastAsync(string id, PodcastFilter filter);
	}
}
