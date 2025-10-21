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
            .ForMember(dest => dest.CreatorUsername, opt => opt.MapFrom(src => src.CreatedByUser != null ? src.CreatedByUser.Username : "Unknown"));

        CreateMap<CreateQuizRequestDto, QuizModel>();
        CreateMap<UpdateQuizRequestDto, QuizModel>();

        // QUESTION mappings
        CreateMap<QuestionModel, QuestionDto>()
            .ForMember(dest => dest.Options, opt => opt.MapFrom(src => new List<string>
            {
                src.CorrectAnswer,
                src.Answer2,
                src.Answer3,
                src.Answer4
            }))
            .ForMember(dest => dest.CorrectOptionIndex, opt => opt.MapFrom(src => 0)); // CorrectAnswer is always at index 0
    }
}
