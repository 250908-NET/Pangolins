using AutoMapper;
using Pangolivia.API.Models;
using Pangolivia.API.DTOs;

namespace Pangolivia.API.Data;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
       
        // QUIZ mappings
        CreateMap<QuizModel, QuizDetailDto>()
            .ForMember(dest => dest.CreatorUsername, opt => opt.Ignore()); // handle username in service if needed

        CreateMap<QuizModel, QuizSummaryDto>()
            .ForMember(dest => dest.QuestionCount, opt => opt.MapFrom(src => src.Questions.Count))
            .ForMember(dest => dest.CreatorUsername, opt => opt.Ignore()); // to change later

        CreateMap<CreateQuizRequestDto, QuizModel>();
        CreateMap<UpdateQuizRequestDto, QuizModel>();

        // QUESTION mappings
        CreateMap<QuestionModel, QuestionDto>().ReverseMap();
    }
}
