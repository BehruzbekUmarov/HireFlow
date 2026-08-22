using HireFlow.Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace HireFlow.Infrastructure.Implementations.Storage;

public class LocalFileStorageService : IFileStorageService
{
	private readonly string _baseUploadPath;
	private readonly string _baseUrl;

	public LocalFileStorageService(IConfiguration configuration)
	{
		// Where files are physically saved on disk
		_baseUploadPath = configuration["FileStorage:BasePath"]
			?? Path.Combine(Directory.GetCurrentDirectory(), "uploads");

		// Base URL to access files publicly
		_baseUrl = configuration["FileStorage:BaseUrl"]
			?? "http://localhost:5000/uploads";
	}

	public async Task<string> SaveAsync(
		Stream fileStream,
		string fileName,
		string folder,
		CancellationToken cancellationToken = default)
	{
		// Create folder if it doesn't exist
		var folderPath = Path.Combine(_baseUploadPath, folder);
		Directory.CreateDirectory(folderPath);

		// Generate unique filename — prevents overwriting and guessing
		var extension = Path.GetExtension(fileName).ToLowerInvariant();
		var uniqueFileName = $"{Guid.NewGuid()}{extension}";
		var filePath = Path.Combine(folderPath, uniqueFileName);

		// Save file to disk
		await using var fileOutput = new FileStream(filePath, FileMode.Create);
		await fileStream.CopyToAsync(fileOutput, cancellationToken);

		// Return the public URL
		return $"{_baseUrl}/{folder}/{uniqueFileName}";
	}

	public Task DeleteAsync(string fileUrl)
	{
		// Convert URL back to file path
		var relativePath = fileUrl.Replace(_baseUrl, string.Empty).TrimStart('/');
		var filePath = Path.Combine(_baseUploadPath, relativePath);

		if (File.Exists(filePath))
			File.Delete(filePath);

		return Task.CompletedTask;
	}
}
