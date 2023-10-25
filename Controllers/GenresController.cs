using ApiPeliculas.DTOs;
using ApiPeliculas.Entities;
using ApiPeliculas.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Text;
using System.Xml.Linq;

namespace ApiPeliculas.Controllers
{
    [ApiController]
    [Route("api/genres")]
    public class GenresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly ICacheService cacheService;
        private readonly IDistributedCache cache;

        //injectamos clase mapper
        public GenresController(ApplicationDbContext context, 
                                IMapper mapper, 
                                IDistributedCache cache,
                                ICacheService cacheService)
        {
            this.context = context;
            this.mapper = mapper;
            this.cache = cache;
            this.cacheService = cacheService;
        }
        [HttpGet("all")]
        public async Task<ActionResult<List<Genre>>> GetAll()
        {
            List<Genre> genresList = new();
            var result = cacheService.GetAllkeys("genrelist*");
            
            //listCustomFields = listCustomFields.OrderBy(n => n).ToList();
            if (result != null) {
                for (int i = 0; i < result.Count; i++)
                {
                    genresList.Add(cacheService.GetData<Genre>($"{result[i]}"));
                    //genresList.Sort(delegate (Genre c1, Genre c2) {
                    //    return (c1.Id.CompareTo(c2.Id));
                    //});

                    //genresList = cacheService.GetData<Genre>($"gentelists:{result[i]}");

                }
            }
            
                var expirationTime = DateTimeOffset.Now.AddMinutes(10);
                genresList = await context.Genres.ToListAsync();

                for (int i = 0; i < genresList.Count; i++)
                {
                    cacheService.SetData<Genre>($"genrelist:{genresList[i].Id}", genresList[i], expirationTime);
                }
                
            
             //return data;
             return genresList;
      
        }

        [HttpGet]
        public async Task<ActionResult<List<GenreDTO>>> Get()
        {
            var cacheKey = "listadoGeneros";
            string serializedList;
            var genreList = new List<Genre>();
            var redisGenreList = await cache.GetAsync(cacheKey);
            
                if (redisGenreList != null)
                {
                    serializedList = Encoding.UTF8.GetString(redisGenreList);
                    genreList = JsonConvert.DeserializeObject<List<Genre>>(serializedList);
                }
                else
                {
                    genreList = await context.Genres.ToListAsync();
                    serializedList = JsonConvert.SerializeObject(genreList);
                    redisGenreList = Encoding.UTF8.GetBytes(serializedList);
                 
                    var options = new DistributedCacheEntryOptions()
                                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(25))  // expirationTime 
                                    .SetSlidingExpiration(TimeSpan.FromMinutes(15));      // caducidad no peticiones en 2m expira    

                    await cache.SetAsync(cacheKey, redisGenreList, options);
                }
         
            var dtos = mapper.Map<List<GenreDTO>>(genreList);            
            return dtos;

            //version without redis
            //var entity = await context.Genres.ToListAsync();
            //var dtos = mapper.Map<List<GenreDTO>>(entity);
            //return dtos;
        }


        [HttpGet("{id:int}", Name = "getGenre")]
        public async Task<ActionResult<GenreDTO>> Get(int id)
        {
                var cacheKey = $"genrelist:{id}";
                string serializedList;
                Genre glist = null;
                var redisGenreList = await cache.GetAsync(cacheKey);

                if (redisGenreList != null)
                {
                    serializedList = Encoding.UTF8.GetString(redisGenreList);
                    glist = JsonConvert.DeserializeObject<Genre>(serializedList);
                }
                else
                {
                    //get genre from db
                    glist = await context.Genres.FirstOrDefaultAsync(x => x.Id == id);
                    serializedList = JsonConvert.SerializeObject(glist);
                    redisGenreList = Encoding.UTF8.GetBytes(serializedList);

                    await cache.SetAsync(cacheKey, redisGenreList);
                }

                if (glist == null) { return NotFound(); }
                return mapper.Map<GenreDTO>(glist);
            

            //var entity = await context.Genres.FirstOrDefaultAsync(x => x.Id == id);
            //if (entity == null) { return NotFound(); }
            //var dto = mapper.Map<GenreDTO>(entity);
            //return dto;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] GenreCreationDTO genreCreationDTO)
        {
            var entity = mapper.Map<Genre>(genreCreationDTO);
            context.Add(entity);
            await context.SaveChangesAsync();
            var genreDTO = mapper.Map<GenreDTO>(entity);
            cacheService.GetAllkeys("genrelist*");

            return new CreatedAtRouteResult("getGenre", new { id = genreDTO.Id }, genreDTO);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] GenreCreationDTO genreCreationDTO)
        {
            var cache = cacheService.GetData<Genre>($"genrelist:{id}");
            var entity = mapper.Map<Genre>(genreCreationDTO);
            entity.Id = id;
            if (cache != null)
            {
                cacheService.RemoveData($"genrelist:{id}");
                context.Entry(entity).State = EntityState.Modified;
                await context.SaveChangesAsync();
                cacheService.SetData<Genre>($"genrelist:{id}",  entity , DateTimeOffset.Now.AddMinutes(10));

            }
            return NoContent();


            //var entity = mapper.Map<Genre>(genreCreationDTO);
            //entity.Id = id;
            //context.Entry(entity).State = EntityState.Modified;
            //await context.SaveChangesAsync();
            //return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await context.Genres.AnyAsync(x => x.Id == id);
            
            if (exist) {
                context.Remove(new Genre() { Id = id });
                //await cache.RemoveAsync($"genrelist:{id}");
                cacheService.RemoveData($"genrelist:{id}");
                await context.SaveChangesAsync();
                return NoContent();
            }

            return NotFound();
        }
    }
}
