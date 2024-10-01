using ImgRepo.Model.Interface;
using System.Linq.Expressions;
using System.Text.Json.Serialization;
#pragma warning disable CS8618 // Non-nullable field is uninitialized.
namespace ImgRepo.Model.ViewModel
{
    public class BasicDetails
    {
        /// <summary>
        /// 物件資訊id
        /// </summary>
        [JsonPropertyName("id")]
        public long Id { get; set; }

        /// <summary>
        /// 物件資訊名稱
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// 物件資訊描述
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

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
