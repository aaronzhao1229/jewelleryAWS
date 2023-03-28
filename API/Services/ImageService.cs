using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace API.Services
{
    public class ImageService
    {
        private readonly Cloudinary _cloudinary;
        public ImageService(IConfiguration config)
        {
            var acc = new Account
            (
                config["Cloudinary:CloudName"],
                config["Cloudinary:ApiKey"],
                config["Cloudinary:ApiSecret"]
            );

            _cloudinary = new Cloudinary(acc);
            // this will give us access to the cloudinary and every service which we can use to upload and do whatever we want with the images and inside this service.
            
        }

        public async Task<ImageUploadResult> AddImageAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream(); // by using using, when we've finished with this method, then this stream of data will be disposed automatically for us by dotnet
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream)
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
                // inside the uploadResult, there will be an error object if it failed or it will be the URL where we can access the image and also a public ID so that we we can reger to that image in cloudinary and do something if we need to.
            }
            return uploadResult;
        }

        public async Task<DeletionResult> DeleteImageAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);

            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result;
        }
    }
}