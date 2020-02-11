using DrPodcastApi.Controllers;
using DrPodcastApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPodcastApi.Tests
{
	[TestClass]
	public class TestPodcastsController
	{
		[TestMethod]
		public async Task GetPodcastAsync_UknownPodcastId_ShouldReturnNotFound()
		{
			var testPodcastId = "unknown-podcast";

			var mockRepo = new Mock<IPodcastRepository>();
			mockRepo.Setup(repo => repo.GetPodcastAsync(testPodcastId, null))
				.ReturnsAsync(GetTestPodcasts().FirstOrDefault(p => p.Id == testPodcastId));

			var controller = new PodcastsController(mockRepo.Object);
			var result = await controller.GetPodcastAsync(testPodcastId, null);

			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
		}

		[TestMethod]
		public async Task GetPodcastAsync_ExistingPodcastId_ShouldReturnCorrectPodcast()
		{
			var testPodcastId = "test-podcast-2";

			var mockRepo = new Mock<IPodcastRepository>();
			mockRepo.Setup(repo => repo.GetPodcastAsync(testPodcastId, null))
				.ReturnsAsync(GetTestPodcasts().FirstOrDefault(p => p.Id == testPodcastId));

			var controller = new PodcastsController(mockRepo.Object);
			var result = await controller.GetPodcastAsync(testPodcastId, null);

			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result.Value, typeof(Podcast));
			Assert.AreEqual(testPodcastId,result.Value.Id);
		}

		[TestMethod]
		public async Task GetPodcastAsync_FilterLimit_ShouldReturnCorrectNumberOfEpisodes()
		{
			var testPodcastId = "test-podcast-1";
			var testFilter = new PodcastFilter { Limit = 3 };

			var mockRepo = new Mock<IPodcastRepository>();
			mockRepo.Setup(repo => repo.GetPodcastAsync(testPodcastId, testFilter))
				.ReturnsAsync(() => {
					var podcast = GetTestPodcasts().FirstOrDefault(p => p.Id == testPodcastId);
					podcast.Episodes = podcast.Episodes.GetRange(0, testFilter.Limit.Value);
					return podcast;
				});

			var controller = new PodcastsController(mockRepo.Object);
			var result = await controller.GetPodcastAsync(testPodcastId, testFilter);

			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result.Value, typeof(Podcast));
			Assert.AreEqual(testFilter.Limit, result.Value.Episodes.Count);
		}

		[TestMethod]
		public async Task GetPodcastAsync_FilterPublicationDate_ShouldNotReturnEpisodesOutOfRange()
		{
			var testPodcastId = "test-podcast-1";
			var testFilter = new PodcastFilter
			{
				PublicationDateStart = new DateTimeOffset(2020, 01, 24, 14, 30, 00, TimeSpan.FromHours(2)),
				PublicationDateEnd = new DateTimeOffset(2020, 02, 01, 00, 00, 00, TimeSpan.FromHours(2))
			};

			var mockRepo = new Mock<IPodcastRepository>();
			mockRepo.Setup(repo => repo.GetPodcastAsync(testPodcastId, testFilter))
				.ReturnsAsync(() => {
					var podcast = GetTestPodcasts().FirstOrDefault(p => p.Id == testPodcastId);
					podcast.Episodes = podcast.Episodes
					.Where(e => e.PublicationDate >= testFilter.PublicationDateStart
						&& e.PublicationDate <= testFilter.PublicationDateEnd)
					.ToList();
					return podcast;
				});

			var controller = new PodcastsController(mockRepo.Object);
			var result = await controller.GetPodcastAsync(testPodcastId, testFilter);

			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result.Value, typeof(Podcast));
			Assert.AreEqual(0, result.Value.Episodes.Where(e => e.PublicationDate < testFilter.PublicationDateStart).Count());
			Assert.AreEqual(0, result.Value.Episodes.Where(e => e.PublicationDate > testFilter.PublicationDateEnd).Count());
		}

		private List<Podcast> GetTestPodcasts()
		{
			var podcasts = new List<Podcast>
			{
				new Podcast
				{
					Id = "test-podcast-1",
					Title = "Test Podcast 1",
					Description = "Description of test podcast 1.",
					Url = new Uri("https://example.com/podcasts/test-podcast-1"),
					Categories = new List<string> { "test-category-1" },
					Episodes = new List<PodcastEpisode>
					{
						new PodcastEpisode
						{
							Id = "100005",
							Title = "Test Podcast 1 Episode 5",
							Description = "Description of episode 5 of test podcast 1.",
							PublicationDate = new DateTimeOffset(2020, 02, 07, 14, 30, 00, TimeSpan.FromHours(2))
						},
						new PodcastEpisode
						{
							Id = "100004",
							Title = "Test Podcast 1 Episode 4",
							Description = "Description of episode 4 of test podcast 1.",
							PublicationDate = new DateTimeOffset(2020, 01, 31, 14, 30, 00, TimeSpan.FromHours(2))
						},
						new PodcastEpisode
						{
							Id = "100003",
							Title = "Test Podcast 1 Episode 3",
							Description = "Description of episode 3 of test podcast 1.",
							PublicationDate = new DateTimeOffset(2020, 01, 24, 14, 30, 00, TimeSpan.FromHours(2))
						},
						new PodcastEpisode
						{
							Id = "100002",
							Title = "Test Podcast 1 Episode 2",
							Description = "Description of episode 2 of test podcast 1.",
							PublicationDate = new DateTimeOffset(2020, 01, 17, 14, 30, 00, TimeSpan.FromHours(2))
						},
						new PodcastEpisode
						{
							Id = "100001",
							Title = "Test Podcast 1 Episode 1",
							Description = "Description of episode 1 of test podcast 1.",
							PublicationDate = new DateTimeOffset(2020, 01, 10, 14, 30, 00, TimeSpan.FromHours(2))
						}
					}
				},
				new Podcast
				{
					Id = "test-podcast-2",
					Title = "Test Podcast 2",
					Description = "Description of test podcast 2.",
					Url = new Uri("https://example.com/podcasts/test-podcast-2"),
					Categories = new List<string> { "test-category-2" },
					Episodes = new List<PodcastEpisode>
					{
						new PodcastEpisode
						{
							Id = "100005",
							Title = "Test Podcast 2 Episode 5",
							Description = "Description of episode 5 of test podcast 2.",
							PublicationDate = new DateTimeOffset(2019, 02, 07, 14, 30, 00, TimeSpan.FromHours(2))
						},
						new PodcastEpisode
						{
							Id = "200004",
							Title = "Test Podcast 2 Episode 4",
							Description = "Description of episode 4 of test podcast 2.",
							PublicationDate = new DateTimeOffset(2019, 01, 31, 14, 30, 00, TimeSpan.FromHours(2))
						},
						new PodcastEpisode
						{
							Id = "200003",
							Title = "Test Podcast 2 Episode 3",
							Description = "Description of episode 3 of test podcast 2.",
							PublicationDate = new DateTimeOffset(2019, 01, 24, 14, 30, 00, TimeSpan.FromHours(2))
						},
						new PodcastEpisode
						{
							Id = "200002",
							Title = "Test Podcast 2 Episode 2",
							Description = "Description of episode 2 of test podcast 2.",
							PublicationDate = new DateTimeOffset(2019, 01, 17, 14, 30, 00, TimeSpan.FromHours(2))
						},
						new PodcastEpisode
						{
							Id = "200001",
							Title = "Test Podcast 2 Episode 1",
							Description = "Description of episode 1 of test podcast 2.",
							PublicationDate = new DateTimeOffset(2019, 01, 10, 14, 30, 00, TimeSpan.FromHours(2))
						}
					}
				}
			};

			return podcasts;
		}
	}
}
