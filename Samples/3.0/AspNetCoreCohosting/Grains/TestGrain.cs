using System.Collections.Generic;
using System.Threading.Tasks;
using ASPNetCoreHostedServices.Interfaces;
using Orleans.Runtime;

namespace ASPNetCoreHostedServices.Grains
{
    public class TestGrain : Orleans.Grain, ITestGrain
    {
        private readonly IPersistentState<TestCache> _cache;

        public TestGrain([PersistentState("archive", "ArchiveStorage")]
            IPersistentState<TestCache> archive)
        {
            _cache = archive;
        }


        public async Task<string> SayHi(string ipnut)

        {
            this._cache.State.List.Add(ipnut);

            await this._cache.WriteStateAsync();

            return $"You said: '{ipnut}', I say: Hello!";
        }

        public Task<IEnumerable<string>> ShowTime() => Task.FromResult<IEnumerable<string>>(this._cache.State.List);

        public async Task Clear()
        {
            await this._cache.ClearStateAsync();
        }
    }

    public class TestCache
    {
        public List<string> List { get; } = new List<string>();
    }
}