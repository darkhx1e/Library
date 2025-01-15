using System.Security.Cryptography;
using SixLabors.ImageSharp;

namespace Library.Backend.Utils;

public class CoverImageUtils
{
    public static async Task ValidateCoverImage(IFormFile file)
    {
        const long maxFileSize = 4 * 1024 * 1024; // 2 mb

        if (file.Length > maxFileSize)
            throw new CustomException("File size is too big, maximum is " + maxFileSize,
                StatusCodes.Status400BadRequest);

        var allowedExtensions = new[] { ".jpg", ".png", ".jpeg", ".bmp" };
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(fileExtension))
            throw new CustomException("Invalid file format. Only JPG, JPEG, PNG, and BMP are allowed.",
                StatusCodes.Status400BadRequest);

        using var image = await Task.Run(() => Image.Load(file.OpenReadStream()));
        if (image.Height <= image.Width)
            throw new CustomException("The image height must be greater than its width.",
                StatusCodes.Status400BadRequest);
    }

    public static async Task<string> ComputeCoverImageHash(IFormFile file)
    {
        using var sha256 = SHA256.Create();
        await using var stream = file.OpenReadStream();
        var hashBytes = await sha256.ComputeHashAsync(stream);
        return Convert.ToBase64String(hashBytes);
    }
}