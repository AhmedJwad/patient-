﻿using HealthCare.Common.Enums;
using System.Drawing;

namespace HealthCare.API.Helpers
{
    public interface IBlobHelper
    {
        Task<Guid> UploadBlobAsync(IFormFile file, string containerName);

        Task<Guid> UploadBlobAsync(byte[] file, string containerName);

        Task<Guid> UploadBlobAsync(string image, string containerName);

        Task DeleteBlobAsync(Guid id, string containerName);
        Bitmap ToGrayscale(Bitmap bmp);
      

    }
}
