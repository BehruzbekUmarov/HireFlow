namespace HireFlow.Application.Services.Interfaces;

public interface IFileStorageService
{
	Task<string> SaveAsync(
		Stream fileStream,
		string fileName,
		string folder,
		CancellationToken cancellationToken = default);

	Task DeleteAsync(string fileUrl);
}