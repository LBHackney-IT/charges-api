using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace ChargesApi.V1.UseCase.Helpers
{
    public static class Helper
    {
        public static async Task<byte[]> GetBytes(this IFormFile formFile)
        {
            using (var memoryStream = new MemoryStream())
            {
                await formFile.CopyToAsync(memoryStream).ConfigureAwait(false);
                return memoryStream.ToArray();
            }
        }
    }
}
