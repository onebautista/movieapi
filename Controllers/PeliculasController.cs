using ApiPeliculas.DTOs;
using ApiPeliculas.Entities;
using ApiPeliculas.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiPeliculas.Controllers
{
    [ApiController]
    [Route("api/peliculas")]
    public class PeliculasController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IFileStorage fileStorage;
        private readonly string contenedor = "peliculas";

        public PeliculasController(ApplicationDbContext context, IMapper mapper, IFileStorage fileStorage)
        {
            this.context = context;
            this.mapper = mapper;
            this.fileStorage = fileStorage;
        }

        [HttpGet]
        public async Task<ActionResult<List<PeliculaDTO>>> Get()
        {
            var peliculas = await context.Peliculas.ToListAsync();
            return mapper.Map<List<PeliculaDTO>>(peliculas);
        }

        [HttpGet("{id}", Name = "GetPelicula")]
        public async Task<ActionResult<PeliculaDTO>> Get(int id) {
            var pelicula = await context.Peliculas.FirstOrDefaultAsync(x => x.Id == id);

            if (pelicula == null)
            {
                return NotFound();
            }

            return mapper.Map<PeliculaDTO>(pelicula);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] PeliculaCreacionDTO peliculaCreacionDTO)
        {
            var pelicula = mapper.Map<Pelicula>(peliculaCreacionDTO);
            //if(peliculaCreacionDTO != null)
            //{
            //    using (var memoryStream = new MemoryStream())
            //    {
            //        await peliculaCreacionDTO.Poster.CopyToAsync(memoryStream);
            //        var contenido = memoryStream.ToArray();
            //        var extension = Path.GetExtension(PeliculaCreacionDTO.Poster.FileName);
            //        pelicula.Poster = await al
            //    }     
            //}

            context.Add(pelicula);
            await context.SaveChangesAsync();
            var peliculaDTO = mapper.Map<PeliculaCreacionDTO>(pelicula);
            return new CreatedAtRouteResult("getPelicula", new { id = pelicula.Id }, peliculaDTO);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromForm] PeliculaCreacionDTO peliculaCreacionDTO)
        {
            var entity = mapper.Map<Actor>(peliculaCreacionDTO) ;
            entity.Id = id;
            context.Entry(entity).State = EntityState.Modified;

            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
