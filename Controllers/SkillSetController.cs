using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using skillset_test.Repositories;
using skillset_test.Dtos;
using Microsoft.EntityFrameworkCore;


namespace skillset_test.Controllers
{
    [Route("[controller]")]
    public class SkillSetController : ControllerBase
    {
        private readonly IThirdPartyRepository _thirdPartyRepository;
        private readonly DataContext _dataContext;
        public SkillSetController(IThirdPartyRepository thirdPartyRepository, DataContext dataContext)
        {
            _thirdPartyRepository = thirdPartyRepository;
            _dataContext = dataContext;
        }
        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string tag, [FromQuery] string sortBy, [FromQuery] string direction)
        {
            // tag = tech,health


            if (tag == null)
            {
                return BadRequest("Tag is required");
            }
            var sortByArray = new List<string>() { "id", "likes", "reads", "popularity" };
            if (sortBy != null && !sortByArray.Contains(sortBy))
            {
                return BadRequest("SortBy is invalid");
            }
            var directionArray = new List<string>() { "asc", "desc" };
            if (direction != null && !directionArray.Contains(direction))
            {
                return BadRequest("Direction is invalid");
            }

            // tag can be array delimted by ,
            var tags = tag.Split(',');
            IEnumerable<Post> posts = new List<Post>();

            foreach (var oneTag in tags)
            {
                posts = posts.Concat(await _thirdPartyRepository.FilterData(oneTag, sortBy, direction));
            }

            if (sortBy != null)
            {
                posts = _thirdPartyRepository.SortData(posts, sortBy, direction);
            }


            return Ok(posts);
            // var posts = _thirdPartyRepository.FilterData(tag, sortBy, direction).Result;
            // return Ok(posts);
        }


        // httpGet post-store
        // httpGet post-{id}
        [HttpGet("/post-store")]
        public async Task<IActionResult> PostStore([FromQuery] string tag)
        {
            var tags = tag.Split(',');

            IEnumerable<Post> posts = new List<Post>();
            foreach (var oneTag in tags)
            {
                posts = posts.Concat(await _thirdPartyRepository.FilterData(oneTag, null, null));
            }

            var numberOfInsertions = 0;
            foreach (var post in posts)
            {
                var postExists = await _dataContext.Posts.AnyAsync(x => x.Id == post.Id);
                if (!postExists)
                {
                    _dataContext.Posts.Add(post);
                    _dataContext.SaveChanges();
                    numberOfInsertions++;
                }
            }

            // foreach (var post in posts)
            // {
            //     //insert post into database
            //     try
            //     {
            //         _dataContext.Add(post);
            //         _dataContext.SaveChanges();
            //         numberOfInsertions++;
            //     }
            //     //catch database duplicate key constraint exception
            //     catch (DbUpdateException de)
            //     {
            //         Exception innerException = de;
            //         while (innerException.InnerException != null)
            //         {
            //             innerException = innerException.InnerException;
            //         }

            //         if (innerException.Message.Contains("duplicate key"))
            //         {
            //             Console.WriteLine("duplicate key");
            //             Console.WriteLine("====================");
            //             Console.WriteLine(innerException.Message);
            //             Console.WriteLine("====================");
            //             Console.WriteLine("====================");
            //             numberOfRejections++;
            //             continue;
            //         }
            //         continue;
            //     }

            // }
            return Ok(new { retrieved = posts.Count(), inserted = numberOfInsertions });
        }


        [HttpPost("/filter")]
        public async Task<IActionResult> FilterPosts(
            [FromQuery] int limit, [FromQuery] int offset,
            Dictionary<string, string> body)
        {

            // if(operation.Equals("like")){
            //     return Ok(await _dataContext.Posts.Skip(offset).Take(limit).Where(x => EF.Functions.Like(x[filterBy], "%" + value + "%")).ToListAsync());
            // }
            // var posts = await _dataContext.Posts.Skip(offset).Take(limit).ToListAsync();
            var condition = "";
            var paginate = limit != 0 ? $"LIMIT {limit} OFFSET {offset}" : "";

            
            if (body.ContainsKey("operation"))
            {
                var operation = body["operation"];

// {
//   "operation": ">",
//   "filterBy": "reads",
//   "value": "12"
// }
                var filterBy = body["filterBy"].First().ToString().ToUpper() + body["filterBy"].Substring(1,body["filterBy"].Length -1).ToString();
                var value = body["value"];
                var values = "";
                if (operation.ToString().ToLower().Equals("like"))
                {
                    values = $"'%{value}%'";
                }
                else if (operation.ToString().ToLower().Equals("between"))
                {
                    var array = value.ToString().Split(' ');
                    values = $"{array[0]} AND {array[1]}";
                }
                else
                {
                    values = value.ToString();
                }
                condition = !filterBy.Equals("") ? $"where \"{filterBy}\" {operation} {values}" : "";
            }


            var sql = $"SELECT * FROM public.\"Posts\" {condition}  {paginate}";

            var posts = await _dataContext.Posts.FromSqlRaw(sql).ToListAsync();

            return Ok(posts);


        }
    }


}