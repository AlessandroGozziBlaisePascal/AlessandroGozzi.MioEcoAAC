using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppACC.SharedKernel;
using AlessandroGozzi.AppCAA.Domain.ValueObjects;

namespace AlessandroGozzi.AppAutistici.Application.Mappers
{
    public static class ImageUrlMapper
    {
        public static Result<ImageUrl> ToImageUrlDomain(this string imageUrl) => ImageUrl.Create(imageUrl);
    }
}
