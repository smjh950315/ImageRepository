using Cyh.Net.Data.Predicate;
using System.Text.Json.Serialization;
#pragma warning disable CS8618 // Non-nullable field is uninitialized.
namespace ImgRepo.Model.Query
{
    public class DynamicalQueryCondition
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("operand")]
        public int Operand { get; set; }

        [JsonPropertyName("operator")]
        public int Operator { get; set; }

        [JsonPropertyName("constant")]
        public string Constant { get; set; }

        public ExpressionData ToExpressionData()
        {
            return new ExpressionData
            {
                MemberName = this.Name,
                LinkType = (LinkType)this.Operand,
                CompareType = (CompareType)this.Operator,
                ConstantValue = this.Constant
            };
        }
    }
}
