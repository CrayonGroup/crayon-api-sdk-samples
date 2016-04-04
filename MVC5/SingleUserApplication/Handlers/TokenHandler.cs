using Crayon.Api.Sdk;
using Crayon.Api.Sdk.Domain.Tokens;
using SingleUserApplication.Models;
using System;

namespace SingleUserApplication.Handlers
{
    public class TokenHandler
    {
        private readonly CrayonApiClient _client;
        private readonly AppSettings _settings;
        private Token _token;

        public TokenHandler(CrayonApiClient client, AppSettings settings)
        {
            _client = client;
            _settings = settings;
        }

        public string GetUserToken()
        {
            if (_token != null && _token.ExpiresIn > TimeSpan.FromMinutes(5).TotalSeconds)
            {
                return _token.AccessToken;
            }

            _token = _client.Tokens.GetUserToken(_settings.CrayonClientId(), _settings.CrayonClientSecret(), _settings.CrayonUserName(), _settings.CrayonUserPassword()).GetData();
            return _token.AccessToken;
        }
    }
}