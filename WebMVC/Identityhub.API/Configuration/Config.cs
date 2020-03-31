using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Identityhub.API.Configuration
{
    public class Config
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
            new ApiResource("api1", "My API"),
            new ApiResource("dame_api", "Dame API"),
            new ApiResource("modulo_api", "Modulo API"),
            new ApiResource("usuario_api", "usuario API"),
            new ApiResource("administrativo_api", "administrativo API"),
            new ApiResource("operacional_api", "operacional API"),
            new ApiResource("configuracao_api", "configuracao API")
            };
        }

        public static IEnumerable<Client> GetClients(IConfiguration Config)
        {
            //string ClientSecretsx = new Secret("secret".Sha256());
            Int32 lifetime = Convert.ToInt32(Config["AccessTokenLifetime"]);
            return new List<Client>
                {
                    new Client
                    {
                    ClientId = "clientoperacional",
                    AccessTokenLifetime = lifetime,
                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    // secret for authentication
                    ClientSecrets = {new Secret("secret".Sha256())},
                    // scopes that client has access to
                    AllowedScopes = { "operacional_api" }
                    },
                    new Client
                    {
                    ClientId = "clientconfiguracao",
                    AccessTokenLifetime = lifetime,
                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    // secret for authentication
                    ClientSecrets = {new Secret("secret".Sha256())},
                    // scopes that client has access to
                    AllowedScopes = { "configuracao_api" }
                    },
                    new Client
                    {
                    ClientId = "clientadministrativo",
                    AccessTokenLifetime = lifetime,
                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    // secret for authentication
                    ClientSecrets = {new Secret("secret".Sha256())},
                    // scopes that client has access to
                    AllowedScopes = { "administrativo_api" }
                    },
                    new Client
                    {
                    ClientId = "clientusuario",
                    AccessTokenLifetime = lifetime,
                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    // secret for authentication
                    ClientSecrets = {new Secret("secret".Sha256())},
                    // scopes that client has access to
                    AllowedScopes = { "usuario_api" }
                    },
                    new Client
                    {
                    ClientId = "clientmodulo",
                    AccessTokenLifetime = lifetime,
                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    // secret for authentication
                    ClientSecrets = {new Secret("secret".Sha256())},
                    // scopes that client has access to
                    AllowedScopes = { "modulo_api" }
                    },
                    new Client
                    {
                    ClientId = "client",
                    AccessTokenLifetime = lifetime,
                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    // secret for authentication
                    ClientSecrets = {new Secret("secret".Sha256())},
                    // scopes that client has access to
                    AllowedScopes = { "dame_api" }
                    },
                    new Client
                    {
                    ClientId = "ro.client",
                    AccessTokenLifetime = lifetime,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets = { new Secret("secret".Sha256())},
                    AllowedScopes = { "dame_api" }
                    },
                    new Client {
                        ClientId = "dameswaggerui",
                        ClientName = "Swagger UI for dame_api",
                        AllowedGrantTypes = GrantTypes.Implicit,
                        AllowAccessTokensViaBrowser = true,
                        RedirectUris = { Config["dame_url"] +"/swagger/oauth2-redirect.html" },
                        PostLogoutRedirectUris = { Config["dame_url"] + "/swagger/" },
                        AllowedScopes = { "dame_api" }
                    },
                    new Client {
                        ClientId = "moduloswaggerui",
                        ClientName = "Swagger UI for modulo_api",
                        AllowedGrantTypes = GrantTypes.Implicit,
                        AllowAccessTokensViaBrowser = true,
                        RedirectUris = { Config["modulo_url"] +"/swagger/oauth2-redirect.html" },
                        PostLogoutRedirectUris = { Config["modulo_url"] +"/swagger/" },
                        AllowedScopes = { "modulo_api" }
                    },
                    new Client {
                        ClientId = "usuarioswaggerui",
                        ClientName = "Swagger UI for usuario_api",
                        AllowedGrantTypes = GrantTypes.Implicit,
                        AllowAccessTokensViaBrowser = true,
                        RedirectUris = { Config["usuario_url"] +"/swagger/oauth2-redirect.html" },
                        PostLogoutRedirectUris = {Config["usuario_url"] +"/swagger/" },
                        AllowedScopes = { "usuario_api" }
                    },
                    new Client {
                        ClientId = "administrativoswaggerui",
                        ClientName = "Swagger UI for administrativo_api",
                        AllowedGrantTypes = GrantTypes.Implicit,
                        AllowAccessTokensViaBrowser = true,
                        RedirectUris = { Config["administrativo_url"] +"/swagger/oauth2-redirect.html" },
                        PostLogoutRedirectUris = {Config["administrativo_url"] +"/swagger/" },
                        AllowedScopes = { "administrativo_api" }
                    },
                    new Client {
                        ClientId = "operacionalswaggerui",
                        ClientName = "Swagger UI for operacional_api",
                        AllowedGrantTypes = GrantTypes.Implicit,
                        AllowAccessTokensViaBrowser = true,
                        RedirectUris = { Config["operacional_url"] +"/swagger/oauth2-redirect.html" },
                        PostLogoutRedirectUris = {Config["operacional_url"] +"/swagger/" },
                        AllowedScopes = { "operacional_api" }
                    },
                    new Client {
                        ClientId = "configuracaoswaggerui",
                        ClientName = "Swagger UI for configuracao_api",
                        AllowedGrantTypes = GrantTypes.Implicit,
                        AllowAccessTokensViaBrowser = true,
                        RedirectUris = { Config["configuracao_url"] +"/swagger/oauth2-redirect.html" },
                        PostLogoutRedirectUris = {Config["configuracao_url"] +"/swagger/" },
                        AllowedScopes = { "configuracao_api" }
                    }

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

        // ApiResources define the apis in your system
        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
            new ApiResource("api1", "My API"),
            new ApiResource("dame_api", "Dame API")
            };
        }

        // Identity resources are data like user ID, name, or email address of a user
        // see: http://docs.identityserver.io/en/release/configuration/resources.html
        public static IEnumerable<IdentityResource> GetResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }

        // client want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients(Dictionary<string,string> clientsUrl)
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
                    AllowedScopes = { "dame_api" }
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
            };
        }
    }
}