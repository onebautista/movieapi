using ApiPeliculas.DTOs;
using ApiPeliculas.Entities;
using ApiPeliculas.Helpers;
using ApiPeliculas.Services;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace ApiPeliculas.Controllers
{
    [ApiController]
    [Route("api/actors")]
    public class ActorController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IDistributedCache cache;

        //inyectamos 
        public ActorController(ApplicationDbContext context, IMapper mapper, IDistributedCache cache)
        {
            this.context = context;
            this.mapper = mapper;   
            this.cache = cache;
        }


        [HttpGet]
        public async Task<ActionResult<List<ActorDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = context.Actors.AsQueryable();
            await HttpContext.InsertPaginationParam(queryable, paginationDTO.CantRegPorPagina);

            var cacheKey = $"listadoActores_{paginationDTO.CantRegPorPagina}";
            string serializedList;
            Console.WriteLine("Key " + cacheKey);
            var actorList = new List<Actor>();
            var redisActorList = await cache.GetAsync(cacheKey);
            Console.WriteLine("redis " + redisActorList);

            if(redisActorList != null )  //si  esta en cache
            {
                serializedList = Encoding.UTF8.GetString(redisActorList);
                actorList = JsonConvert.DeserializeObject<List<Actor>>(serializedList);
            } 
            else
            {
                actorList = await queryable.Paginar(paginationDTO).ToListAsync();
                serializedList = JsonConvert.SerializeObject(actorList);
                redisActorList = Encoding.UTF8.GetBytes(serializedList);

                var options = new DistributedCacheEntryOptions()
                                .SetAbsoluteExpiration(DateTime.Now.AddMinutes(25)) 
                                .SetSlidingExpiration(TimeSpan.FromMinutes(15));       

                await cache.SetAsync(cacheKey, redisActorList, options);
            }
             
            return mapper.Map<List<ActorDTO>>(actorList);
            
            
            //llamada al helper de paginacion
            //var queryable = context.Actors.AsQueryable();
            //await HttpContext.InsertPaginationParam(queryable, paginationDTO.CantRegPorPagina);

            ////var entity = await context.Actors.ToListAsync(); no pagination
            //var entity = await queryable.Paginar(paginationDTO).ToListAsync();

            //return mapper.Map<List<ActorDTO>>(entity);    //go to helpers to add map
        }

        [HttpGet("{id}", Name = "getActor")]
        public async Task<ActionResult<ActorDTO>> Get(int id)
        {
            var entity  = await context.Actors.FirstOrDefaultAsync(x => x.Id == id); 
            if (entity == null) { return NotFound(); }
            return mapper.Map<ActorDTO>(entity);    
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] ActorCreationDTO actorCreationDTO)
        {
            var entity = mapper.Map<Actor>(actorCreationDTO);
            context.Add(entity);
            await context.SaveChangesAsync();
            var dto = mapper.Map<ActorDTO>(entity);

            return new CreatedAtRouteResult("getActor", new { id = entity.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromForm] ActorCreationDTO actorCreationDTO)
        {
            var entity = mapper.Map<Actor>(actorCreationDTO); 
            entity.Id = id;
            context.Entry(entity).State = EntityState.Modified;

            // hacer que solo los campos modificados se actualizen
            //var actorDB = await context.Actors.FirstOrDefaultAsync(x => x.Id == id);

            //if (actorDB == null) { return NotFound(); }

            //actorDB = mapper.Map(actorCreationDTO, actorDB);

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<ActorPatchDTO> patchDocument)
        {
            if(patchDocument == null) { return BadRequest(); }

            var entityDb = await context.Actors.FirstOrDefaultAsync(x => x.Id == id);

            if(entityDb == null) { return NotFound(); }

            var entityDto = mapper.Map<ActorPatchDTO>(entityDb);

            patchDocument.ApplyTo(entityDto, ModelState);

            var isValid = TryValidateModel(entityDto);
            if(!isValid) { return BadRequest(); }

            mapper.Map(entityDto, entityDb);
            await context.SaveChangesAsync();

            return NoContent();  //code 204

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await context.Actors.AnyAsync(x => x.Id == id);
            if (!exist) { return NotFound(); }

            var cacheKey = $"actorDeletedID_{id}";
            Console.WriteLine("cache " + cacheKey);
            if (cacheKey != null) {
                context.Remove(new Actor() { Id = id });
                await context.SaveChangesAsync();
                Console.WriteLine("id " + id);
                await cache.RemoveAsync(cacheKey);
            }

            return NoContent();

            //var exist = await context.Actors.AnyAsync(x => x.Id == id);
            //if (!exist) { return NotFound(); }
            //context.Remove(new Actor() { Id = id });
            //await context.SaveChangesAsync();
            //return NoContent();
        }
        

    }
}
