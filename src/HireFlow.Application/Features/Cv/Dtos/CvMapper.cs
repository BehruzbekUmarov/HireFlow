using HireFlow.Application.DTOs.Cv.Responses;
using HireFlow.Domain.Entities;

namespace HireFlow.Application.Features.Cv.Dtos;

public static class CvMapper
{
	public static CvDto MapToDto(FreelancerCv cv) => new()
	{
		Id = cv.Id,
		Title = cv.Title,
		Summary = cv.Summary,
		Skills = cv.Skills,
		Experience = cv.Experience,
		Projects = cv.Projects,
		Education = cv.Education,
		Languages = cv.Languages,
		PortfolioUrl = cv.PortfolioUrl,
		YearsOfExperience = cv.YearsOfExperience,
		FileUrl = cv.FileUrl,
		IsDefault = cv.IsDefault,
		CreatedAt = cv.CreatedAt,
		UpdatedAt = cv.UpdatedAt
	};
}
