using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASPNetCoreHostedServices.Interfaces
{
    public interface ITestGrain : Orleans.IGrainWithIntegerKey
    {
        Task<string> SayHi(string ipnut);

        Task<IEnumerable<string>> ShowTime();

        Task Clear();
    }
}