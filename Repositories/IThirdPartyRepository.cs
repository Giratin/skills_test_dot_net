using System.Collections.Generic;
using skillset_test.Dtos;
using System.Threading.Tasks;


namespace skillset_test.Repositories
{
    public interface IThirdPartyRepository
    {
        Task<IEnumerable<Post>> FilterData(string tag, string sortBy, string direction);
        IEnumerable<Post> SortData(IEnumerable<Post> posts, string sortBy, string direction);
    }
}