using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace temsAPI.Contracts
{
    /// <summary>
    /// Fetches items based on a filter
    /// </summary>
    /// <typeparam name="T">Type of fetched items</typeparam>
    /// <typeparam name="T_Filter">Filter type</typeparam>
    public interface IFetcher<T, T_Filter>
    {
        Task<IEnumerable<T>> Fetch(T_Filter filter);
        Task<int> GetAmount(T_Filter filter);
    }
}
