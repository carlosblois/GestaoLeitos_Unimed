using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;

namespace Services.Identity.API.Configuration
{
    public class Config
    {

        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                SubjectId = "1",                Username = "alice",
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

        // ApiResources define the apis in your system
        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                new ApiResource("usuario", "Usuario Service"),
                new ApiResource("modulo", "Modulo Service"),
            };
        }

        // client want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients()
        {
            {
                return new List<Client>
            {
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },
                    //ClientUri = $"{clientsUrl["Mvc"]}",                             
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowAccessTokensViaBrowser = false,
                    RequireConsent = false,
                    AllowOfflineAccess = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AllowedScopes = new List<string>
                    {
                        "usuario",
                        "modulo"
                    },
                    AccessTokenLifetime = 60*60*2, // 2 hours
                    IdentityTokenLifetime= 60*60*2 // 2 hours
                },

                new Client
                {
                    ClientId = "usuarioswaggerui",
                    ClientName = "Usuario Swagger UI",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    //RedirectUris = { $"{clientsUrl["UsuarioApi"]}/swagger/oauth2-redirect.html" },
                   // PostLogoutRedirectUris = { $"{clientsUrl["UsuarioApi"]}/swagger/" },

                    AllowedScopes =
                    {
                        "usuario"
                    }
                },
                new Client
                {
                    ClientId = "moduloswaggerui",
                    ClientName = "Modulo Swagger UI",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    //RedirectUris = { $"{clientsUrl["ModuloApi"]}/swagger/oauth2-redirect.html" },
                    //PostLogoutRedirectUris = { $"{clientsUrl["ModuloApi"]}/swagger/" },

                    AllowedScopes =
                    {
                        "modulo"
                    }
                }

            };
            }
        }

    }

}