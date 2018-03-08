using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WikiClientLibrary.Client;
using WikiClientLibrary.Pages;
using WikiClientLibrary.Sites;
using WikiClientLibrary.Wikibase;

namespace Wikitest
{
    public class Class1
    {
        public Entity GetEntity(string id)
        {
            WikiSite site = InitSite();

            var entity = new Entity(site, id);
            entity.RefreshAsync(EntityQueryOptions.FetchAllProperties).Wait();
            return entity;
        }

        private static WikiSite InitSite()
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddConsole(LogLevel.Information, true);

            var wikiClient = new WikiClient
            {
                // UA of Client Application. The UA of WikiClientLibrary will
                // be append to the end of this when sending requests.
                ClientUserAgent = "ConsoleTestApplication1/1.0",
                Logger = loggerFactory.CreateLogger<WikiClient>(),
            };

            var site = new WikiSite(wikiClient, "https://www.wikidata.org/w/api.php")
            {
                Logger = loggerFactory.CreateLogger<WikiSite>()
            };

            // Waits for the WikiSite to initialize
            site.Initialization.Wait();
            return site;
        }

        public Task<IList<OpenSearchResultEntry>> Search(string term)
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddConsole(LogLevel.Information, true);

            var wikiClient = new WikiClient
            {
                // UA of Client Application. The UA of WikiClientLibrary will
                // be append to the end of this when sending requests.
                ClientUserAgent = "ConsoleTestApplication1/1.0",
                Logger = loggerFactory.CreateLogger<WikiClient>(),
            };
            
            var uri = WikiSite.SearchApiEndpointAsync(wikiClient, "en.wikipedia.org").Result;
            
            var site = new WikiSite(wikiClient, uri);
            
            return site.OpenSearchAsync(term, 5);
        }

        public async Task<IEnumerable<string>> SearchPages(string term)
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddConsole(LogLevel.Information, true);

            var wikiClient = new WikiClient
            {
                // UA of Client Application. The UA of WikiClientLibrary will
                // be append to the end of this when sending requests.
                ClientUserAgent = "ConsoleTestApplication1/1.0",
                Logger = loggerFactory.CreateLogger<WikiClient>(),
            };
            
            var uri = WikiSite.SearchApiEndpointAsync(wikiClient, "en.wikipedia.org").Result;
            
            var site = new WikiSite(wikiClient, uri);
            
            var results = await site.OpenSearchAsync(term, 5);

            return results.Select(r => r.Title);
        }

        public async Task<IEnumerable<string>> SearchIdAsync(string term)
        {
            WikiSite site = await WikiDataSite();

            var titles = new[] { "Mount Everest", "Test_(Unix)", "Inexistent title", "Earth" };

            var enumerable = Entity.IdsFromSiteLinksAsync(site, "enwiki", new[] { "New York City" });

            return await enumerable.ToList();
        }

        private static async Task<WikiSite> WikiDataSite()
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddConsole(LogLevel.Information, true);

            var wikiClient = new WikiClient
            {
                // UA of Client Application. The UA of WikiClientLibrary will
                // be append to the end of this when sending requests.
                ClientUserAgent = "ConsoleTestApplication1/1.0",
                Logger = loggerFactory.CreateLogger<WikiClient>(),
            };

            var site = new WikiSite(wikiClient, "https://www.wikidata.org/w/api.php")
            {
                Logger = loggerFactory.CreateLogger<WikiSite>()
            };

            await site.Initialization;
            return site;
        }

        public Task<IEnumerable<Entity>> EntityFromTitle(string title) => EntityFromTitle(new string[] {title});

        public async Task<IEnumerable<Entity>> EntityFromTitle(IEnumerable<string> titles)
        {
            var site = await WikiDataSite();
            var ids = await Entity.IdsFromSiteLinksAsync(site, "enwiki", titles).ToList();

            List<Entity> results = new List<Entity>();

            foreach (var id in ids)
            {
                var entity = new Entity(site, id);
                await entity.RefreshAsync(EntityQueryOptions.FetchAliases | EntityQueryOptions.FetchClaims);
                results.Add(entity);

            }
            return results;
        }

        public async Task<IEnumerable<Entity>> EntityFromKeyword(string keywords)
        {
            var searchResults = await SearchPages(keywords);
            var result = await EntityFromTitle(searchResults);
            return result;
        }
    }
}
