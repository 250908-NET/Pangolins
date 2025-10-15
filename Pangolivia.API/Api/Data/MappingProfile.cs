using AutoMapper;
using Pangolivia.Models;
using Pangolivia.DTOs;

namespace Pangolivia.Data;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Quiz, QuizDto>().ReverseMap();
        CreateMap<Quiz, CreateQuizDto>().ReverseMap();
        CreateMap<Quiz, UpdateQuizDto>().ReverseMap();
    }
}

