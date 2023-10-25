using ApiPeliculas.DTOs;
using ApiPeliculas.Entities;
using AutoMapper;

namespace ApiPeliculas.Helpers
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles() { 
            CreateMap<Genre, GenreDTO>().ReverseMap();  //to convert entity to dto
            CreateMap<GenreCreationDTO, Genre>(); //To make post from dto to entity

            CreateMap<Actor, ActorDTO>().ReverseMap();
            CreateMap<ActorCreationDTO, Actor>()
                .ForMember(x => x.Photo, options => options.Ignore());

            CreateMap<ActorPatchDTO, Actor>().ReverseMap();
        }
    }
}
