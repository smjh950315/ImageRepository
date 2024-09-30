using ImgRepo.Model.Interface;
using ImgRepo.Model.ViewModel;
using System.Linq.Expressions;

namespace ImgRepo.Model
{
    public static class Extensions
    {
        public static void ReadBasicInformation(this BasicDetails basicDetails, IBasicEntityInformation basicEntityInformation)
        {
            basicDetails.Id = basicEntityInformation.Id;
            basicDetails.Name = basicEntityInformation.Name;
            basicDetails.Description = basicEntityInformation.Description;
        }
    }
}
