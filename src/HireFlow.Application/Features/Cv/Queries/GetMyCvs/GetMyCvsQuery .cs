using HireFlow.Application.DTOs.Cv.Responses;
using MediatR;

namespace HireFlow.Application.Features.Cv.Queries.GetMyCvs;

public record GetMyCvsQuery : IRequest<List<CvDto>>;
