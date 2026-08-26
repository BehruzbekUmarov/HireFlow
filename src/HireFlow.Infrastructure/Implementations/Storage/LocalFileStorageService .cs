using HireFlow.Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace HireFlow.Infrastructure.Implementations.Storage;

public class LocalFileStorageService : IFileStorageService
{
	private readonly string _baseUploadPath;
	private readonly string _baseUrl;

	public LocalFileStorageService(IConfiguration configuration)
	{
		_baseUploadPath = configuration["FileStorage:BasePath"]
			?? Path.Combine(Directory.GetCurrentDirectory(), "uploads");

		_baseUrl = configuration["FileStorage:BaseUrl"]
			?? "http://localhost:5000/uploads";
	}

	public async Task<string> SaveAsync(
		Stream fileStream,
		string fileName,
		string folder,
		CancellationToken cancellationToken = default)
	{
		var folderPath = Path.Combine(_baseUploadPath, folder);
		Directory.CreateDirectory(folderPath);

		var extension = Path.GetExtension(fileName).ToLowerInvariant();
		var uniqueFileName = $"{Guid.NewGuid()}{extension}";
		var filePath = Path.Combine(folderPath, uniqueFileName);

		await using var fileOutput = new FileStream(filePath, FileMode.Create);
		await fileStream.CopyToAsync(fileOutput, cancellationToken);

		return $"{_baseUrl}/{folder}/{uniqueFileName}";
	}

	public Task DeleteAsync(string fileUrl)
	{
		var relativePath = fileUrl.Replace(_baseUrl, string.Empty).TrimStart('/');
		var filePath = Path.Combine(_baseUploadPath, relativePath);

		if (File.Exists(filePath))
			File.Delete(filePath);

		return Task.CompletedTask;
	}
}
