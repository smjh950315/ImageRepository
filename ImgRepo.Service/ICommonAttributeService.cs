using ImgRepo.Data.Interface;
using ImgRepo.Model.Common;

namespace ImgRepo.Service
{
    public interface ICommonAttributeService
    {
        IQueryable<BasicDetails> GetDetailsQueryable<TAttribute>() where TAttribute : class, IBasicEntityAttribute, new();
        BasicDetails? GetDetailById<TAttribute>(long id) where TAttribute : class, IBasicEntityAttribute, new();
    }
}
