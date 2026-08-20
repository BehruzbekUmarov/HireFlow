using MediatR;

namespace HireFlow.Application.Features.Cv.Commands.SetDefaultCv;

public record SetDefaultCvCommand(long CvId) : IRequest;