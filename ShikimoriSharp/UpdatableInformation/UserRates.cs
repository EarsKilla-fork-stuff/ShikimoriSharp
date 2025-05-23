﻿using System.Threading.Tasks;
using ShikimoriSharp.Bases;
using ShikimoriSharp.Classes;
using ShikimoriSharp.Settings;

namespace ShikimoriSharp.UpdatableInformation
{
    public class UserRates : ApiBase
    {
        public UserRates(ApiClient apiClient) : base(Version.v2, apiClient)
        {
        }

        public async Task<UserRate[]> GetUserRates(long id)
        {
            return await Request<UserRate[]>($"user_rates/{id}");
        }

        public async Task<UserRate[]> GetUsersRates(UserRatesSettings settings)
        {
            return await Request<UserRate[], UserRatesSettings>("user_rates", settings);
        }

        public async Task<UserRate> NewUserRate(NewUserRateSettings settings, AccessToken personalInformation)
        {
            Requires(personalInformation, new[] {"user_rates"});
            return await Request<UserRate, UserRateBase>("user_rates", settings.content, personalInformation, "POST", false, "user_rate");
        }

        public async Task<UserRate> EditUserRate(long id, UserRateEditSettings settings, AccessToken personalInformation)
        {
            Requires(personalInformation, new[] {"user_rates"});
            return await Request<UserRate, UserRateBase>($"user_rates/{id}", settings.content, personalInformation, "PUT", false, "user_rate");
        }

        public async Task<UserRate> Increment(long id, AccessToken personalInformation)
        {
            Requires(personalInformation, new[] {"user_rates"});
            return await Request<UserRate>($"user_rates/{id}/increment", personalInformation);
        }

        public async Task DeleteUserRate(long id, AccessToken personalInformation)
        {
            Requires(personalInformation, new[] {"user_rates"});
            await NoResponseRequest($"user_rates/{id}", personalInformation);
        }
    }
}