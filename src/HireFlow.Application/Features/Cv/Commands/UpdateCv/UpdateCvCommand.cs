using HireFlow.Application.DTOs.Cv.Requests;
using HireFlow.Application.DTOs.Cv.Responses;
using MediatR;

namespace HireFlow.Application.Features.Cv.Commands.UpdateCv;

public record UpdateCvCommand(long CvId, UpdateCvRequest Request) : IRequest<CvDto>;
