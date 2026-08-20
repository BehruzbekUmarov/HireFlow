using HireFlow.Application.DTOs.Cv.Responses;
using MediatR;

namespace HireFlow.Application.Features.Cv.Queries.GetCvById;

public record GetCvByIdQuery(long CvId) : IRequest<CvDto>;
