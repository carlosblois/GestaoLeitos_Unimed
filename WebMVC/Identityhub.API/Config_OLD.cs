using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identityhub.API
{
    public class Config_OLD
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
            new ApiResource("api1", "My API"),
            new ApiResource("dame_api", "Dame API"),
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
                {
                    new Client
                    {
                    ClientId = "client",
                    //AccessTokenLifetime = 60 * 60 * 24,
                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    // secret for authentication
                    ClientSecrets = {new Secret("secret".Sha256())},
                    // scopes that client has access to
                    AllowedScopes = { "api1" }
                    },
                    new Client
                    {
                    ClientId = "ro.client",
                    //AccessTokenLifetime = 60 * 60 * 24,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets = { new Secret("secret".Sha256())},
                    AllowedScopes = { "api1" }
                    },
                    new Client {
                        ClientId = "dameswaggerui",
                        ClientName = "Swagger UI for dame_api",
                        AllowedGrantTypes = GrantTypes.Implicit,
                        AllowAccessTokensViaBrowser = true,
                        RedirectUris = { $"http://localhost:59820/swagger/oauth2-redirect.html" },
                        PostLogoutRedirectUris = { $"http://localhost:59820/swagger/" },
                        AllowedScopes = { "dame_api" }
                    }//,
                    //new Client
                    //    {
                    //        ClientId = "dameswaggerui",
                    //        ClientName = "Swagger UI for dame_api",
                    //        AllowedGrantTypes = GrantTypes.Implicit,
                    //        AllowAccessTokensViaBrowser = true,
                    //        RedirectUris = {"http://localhost:5001/o2c.html"},
                    //        AllowedScopes = { "dame_api" }
                    //    }
                };

        }

        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                SubjectId = "1",
                Username = "alice",
                Password = "password"
                },
                new TestUser
                {
                SubjectId = "2",
                Username = "bob",
                Password = "password"
                }
            };
        }

    }
}
