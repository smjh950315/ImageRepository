using ImgRepo.Model.Interface;
using System.Linq.Expressions;
using System.Text.Json.Serialization;
#pragma warning disable CS8618 // Non-nullable field is uninitialized.
namespace ImgRepo.Model.ViewModel
{
    /// <summary>
    /// 物件基本資訊WEB用模型
    /// </summary>
    public class BasicDetails : BasicInfo
    {
        /// <summary>
        /// 物件資訊描述
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        public static Expression<Func<T, BasicDetails>> GetExpression<T>() where T : IBasicEntityInformation
        {
            return t => new BasicDetails
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
            };
        }
    }
}
