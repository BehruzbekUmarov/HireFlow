using MediatR;

namespace HireFlow.Application.Features.Cv.Commands.DeleteCv;

public record DeleteCvCommand(long CvId) : IRequest;
