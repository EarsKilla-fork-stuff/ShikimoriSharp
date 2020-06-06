﻿using System.Threading.Tasks;
using ShikimoriSharp.Bases;
using ShikimoriSharp.Classes;
using ShikimoriSharp.Settings;

namespace ShikimoriSharp.UpdatableInformation
{
    public class Clubs : ApiBase
    {
        public Clubs(ApiClient apiClient) : base(Version.v1, apiClient)
        {
        }

        public async Task<Club[]> GetClubs(ClubsRequestSettings settings = null, AccessToken personalInformation = null)
        {
            return await Request<Club[], ClubsRequestSettings>("clubs", settings, personalInformation);
        }

        public async Task<ClubID> GetClub(long id, AccessToken personalInformation)
        {
            return await Request<ClubID>($"clubs/{id}", personalInformation);
        }

        public async Task<Club> UpdateClub(long id, AccessToken personalInformation, UpdateClubSettings club = null)
        {
            return await SendJson<Club>($"clubs/{id}", club, personalInformation, "PUT");
        }

        public async Task<Anime[]> GetAnimes(long id)
        {
            return await Request<Anime[]>($"clubs/{id}/animes");
        }

        public async Task<Manga[]> GetMangas(long id)
        {
            return await Request<Manga[]>($"clubs/{id}/mangas");
        }

        public async Task<Manga[]> GetRanobe(long id)
        {
            return await Request<Manga[]>($"clubs/{id}/ranobe");
        }

        public async Task<Character[]> GetCharacters(long id)
        {
            return await Request<Character[]>($"clubs/{id}/characters");
        }

        public async Task<User[]> GetMembers(long id)
        {
            return await Request<User[]>($"clubs/{id}/members");
        }

        public async Task<ClubImage[]> GetImages(long id)
        {
            return await Request<ClubImage[]>($"clubs/{id}/images");
        }

        public async Task Join(long id, AccessToken personalInformation)
        {
            await NoResponseRequest($"clubs/{id}/join", personalInformation);
        }

        public async Task Leave(long id, AccessToken personalInformation)
        {
            await NoResponseRequest($"clubs/{id}/leave", personalInformation);
        }
    }
}