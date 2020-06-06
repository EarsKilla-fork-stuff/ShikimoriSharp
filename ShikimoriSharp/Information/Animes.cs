﻿using System.Threading.Tasks;
using ShikimoriSharp.AdditionalRequests;
using ShikimoriSharp.Bases;
using ShikimoriSharp.Classes;
using ShikimoriSharp.Settings;

namespace ShikimoriSharp.Information
{
    public class Animes : ApiBase
    {
        public Animes(ApiClient apiClient) : base(Version.v1, apiClient)
        {
        }

        public async Task<Anime[]> GetAnime()
        {
            return await Request<Anime[]>("animes");
        }

        public async Task<Anime[]> GetAnime(AnimeRequestSettings settings, AccessToken personalInformation = null)
        {
            return await Request<Anime[], AnimeRequestSettings>("animes", settings, personalInformation);
        }

        public async Task<AnimeID> GetAnime(long id, AccessToken personalInformation = null)
        {
            return await Request<AnimeID>($"animes/{id}", personalInformation);
        }

        public async Task<Role[]> GetRoles(long id)
        {
            return await Request<Role[]>($"animes/{id}/roles");
        }

        public async Task<Anime[]> GetSimilar(long id)
        {
            return await Request<Anime[]>($"animes/{id}/similar");
        }

        public async Task<Related[]> GetRelated(long id)
        {
            return await Request<Related[]>($"animes/{id}/related");
        }

        public async Task<Screenshots[]> GetScreenshots(long id)
        {
            return await Request<Screenshots[]>($"animes/{id}/screenshots");
        }

        public async Task<Franchise> GetFranchise(long id)
        {
            return await Request<Franchise>($"animes/{id}/franchise");
        }

        public async Task<ExternalLinks[]> GetExternalLinks(long id)
        {
            return await Request<ExternalLinks[]>($"animes/{id}/external_links");
        }

        public async Task<Topic[]> GetTopics(long id)
        {
            return await Request<Topic[]>($"animes/{id}/topics");
        }

        public async Task<Topic[]> GetTopics(long id, AnimeTopicSettings settings)
        {
            return await Request<Topic[], AnimeTopicSettings>($"animes/{id}/topics", settings);
        }
    }
}