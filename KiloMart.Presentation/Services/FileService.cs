﻿namespace KiloMart.Presentation.Services;

public static class FileService
{
    private static readonly string[] AllowedImageTypes = ["image/jpeg",
        "image/png",
        "image/gif",
        "image/bmp",
        "image/jpg",
        "image/webp"];

    public static async Task<string> SaveImageFileAsync(IFormFile file, string webRootPath, Guid fileName, string subDirectory = "images")
    {
        if (file is null || file.Length == 0)
            throw new ArgumentException("No file was uploaded.");

        if (!AllowedImageTypes.Contains(file.ContentType))
            throw new ArgumentException("The uploaded file is not a valid image type.");

        var extension = GetFileExtension(file.ContentType)??
            throw new ArgumentException("Unsupported image format.");

        var imageUrl = subDirectory + "/" + $"{fileName}{extension}";
        var path = Path.Combine(webRootPath, subDirectory, $"{fileName}{extension}");

        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        using (var stream = new FileStream(path, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return imageUrl;
    }

    private static string? GetFileExtension(string contentType)
    {
        return contentType switch
        {
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            "image/gif" => ".gif",
            "image/jpg" => ".jpg",
            "image/bmp" => ".bmp",
            "image/webp" => ".webp",
            _ => null
        };
    }
}
