using HireFlow.Application.DTOs.Cv.Requests;
using HireFlow.Application.DTOs.Cv.Responses;
using MediatR;

namespace HireFlow.Application.Features.Cv.Commands.CreateCv;

public record CreateCvCommand(CreateCvRequest Request) : IRequest<CvDto>;
