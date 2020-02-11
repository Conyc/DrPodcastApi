using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Rss;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;

namespace DrPodcastApi.Models
{
	public class PodcastRepository : IPodcastRepository
	{
		public async Task<Podcast> GetPodcastAsync(string id)
		{
			return await GetPodcastAsync(id, null);
		}

		public async Task<Podcast> GetPodcastAsync(string id, PodcastFilter filter)
		{
			var podcast = new Podcast
			{
				Id = id,
				Categories = new List<string>(),
				Episodes = new List<PodcastEpisode>()
			};

			string feedUrl = string.Format("https://www.dr.dk/mu/feed/{0}.xml?format=podcast", id);

			try
			{
				using (XmlReader xmlReader = XmlReader.Create(feedUrl, new XmlReaderSettings { Async = true }))
				{
					var feedReader = new RssFeedReader(xmlReader);
					while (await feedReader.Read())
					{
						switch (feedReader.ElementType)
						{
							// Read item
							case SyndicationElementType.Item:
								// Skip all items if limit is set at less than 1
								if (filter?.Limit != null && filter.Limit < 1)
								{
									return podcast;
								}

								var item = await feedReader.ReadItem();

								// Skip item if publication date is outside requested range
								if ((filter?.PublicationDateStart != null && item.Published < filter.PublicationDateStart)
									|| (filter?.PublicationDateEnd != null && item.Published > filter.PublicationDateEnd))
								{
									continue;
								}

								var episode = new PodcastEpisode
								{
									Id = item.Id,
									Title = item.Title,
									Description = item.Description,
									PublicationDate = item.Published
								};

								podcast.Episodes.Add(episode);

								// Limit has been reached. We're done here.
								if (filter?.Limit != null && filter.Limit.Value <= podcast.Episodes.Count)
								{
									return podcast;
								}
								break;
							// Read link
							case SyndicationElementType.Link:
								var link = await feedReader.ReadLink();
								podcast.Url = link.Uri;
								break;
							// Read category
							case SyndicationElementType.Category:
								var category = await feedReader.ReadCategory();
								podcast.Categories.Add(category.Name);
								break;
							// Read content
							default:
								var content = await feedReader.ReadContent();
								switch (content.Name)
								{
									case "title":
										podcast.Title = content.Value;
										break;
									case "description":
										podcast.Description = content.Value;
										break;
								}
								break;
						}
					}
				}
			}
			// Return null if a podcast feed cannot be found for the given ID
			// DR RSS feed incorrectly returns an empty 200 OK response instead of 404 Not Found, which results in a XML parse error
			catch (Exception ex) when (ex is FileNotFoundException || ex is XmlException)
			{
				return null;
			}

			return podcast;
		}
	}
}
