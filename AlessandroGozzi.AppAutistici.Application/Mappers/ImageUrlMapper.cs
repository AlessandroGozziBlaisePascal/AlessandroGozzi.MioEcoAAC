using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.AppAAC.Domain.ValueObjects;
using AlessandroGozzi.AppAAC.SharedKernel;

namespace AlessandroGozzi.AppAAC.Application.Mappers
{
    public static class ImageUrlMapper
    {
        public static Result<ImageUrl> ToImageUrlDomain(this string imageUrl) => ImageUrl.Create(imageUrl);
    }
}
