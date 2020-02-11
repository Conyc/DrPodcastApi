using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DrPodcastApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DrPodcastApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PodcastsController : ControllerBase
    {
        private readonly IPodcastRepository _podcastRepository;

        public PodcastsController(IPodcastRepository podcastRepository)
        {
            _podcastRepository = podcastRepository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Podcast>> GetPodcastAsync(string id, [FromQuery]PodcastFilter filter)
        {
            var podcast = await _podcastRepository.GetPodcastAsync(id, filter);

            if (podcast == null)
            {
                return NotFound();
            }

            return podcast;
        }
    }
}