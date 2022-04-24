using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using skillset_test.Dtos;
using System.Net.Http;
using System.Linq;

namespace skillset_test.Repositories
{
    public class ThirdPartyRepository : IThirdPartyRepository
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string BaseUrl = "https://api.assessment.skillset.technology/a74fsg46d/posts";
        public ThirdPartyRepository(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<Post>> FilterData(string tag, string sortBy, string direction)
        {
            var client = _httpClientFactory.CreateClient();
            var url = $"{BaseUrl}?tag={tag}";
            if(sortBy != null)
            {
                url += $"&sortBy={sortBy}";
            }
            if(direction != null)
            {
                url += $"&direction={direction}";
            }

            
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {

                var stringResponse = await response.Content.ReadAsStringAsync();
               
                if(stringResponse == "" || stringResponse == null)
                {
                    return await Task.FromResult(new List<Post>());;
                }

                var postMap = Newtonsoft.Json.JsonConvert.DeserializeObject<IDictionary<string, IEnumerable<Post>>>(stringResponse);
                var postDtos =  postMap["posts"];
                return postDtos;
            }
            else
            {
                return await Task.FromResult(new List<Post>());
            }
        }



        public IEnumerable<Post> SortData(IEnumerable<Post> posts, string sortBy, string direction)
        {
            if(sortBy == "id")
            {
                if(direction == "asc")
                {
                    return posts.OrderBy(x => x.Id);
                }
                else
                {
                    return posts.OrderByDescending(x => x.Id);
                }
            }
            else if(sortBy == "likes")
            {
                if(direction == "asc")
                {
                    return posts.OrderBy(x => x.Likes);
                }
                else
                {
                    return posts.OrderByDescending(x => x.Likes);
                }
            }
            else if(sortBy == "reads")
            {
                if(direction == "asc")
                {
                    return posts.OrderBy(x => x.Reads);
                }
                else
                {
                    return posts.OrderByDescending(x => x.Reads);
                }
            }
            else if(sortBy == "popularity")
            {
                if(direction == "asc")
                {
                    return posts.OrderBy(x => x.Popularity);
                }
                else
                {
                    return posts.OrderByDescending(x => x.Popularity);
                }
            }
            else
            {
                return posts;
            }
        }

    }

}