using Cyh.Net.Data.Predicate;
using ImgRepo.Model.Query;
using System.Diagnostics.CodeAnalysis;

namespace ImgRepo.Model
{
    public static class Extensions
    {
        /// <summary>
        /// 取得查詢條件的表達式資料所需資料
        /// </summary>
        /// <returns>是否成功</returns>
        public static bool TryGetExpressionData(this ApiCondition? queryGroup, [NotNullWhen(true)] out ExpressionData? expressionData)
        {
            if (queryGroup == null)
            {
                expressionData = null;
                return false;
            }
            expressionData = new ExpressionData
            {
                CompareType = (CompareType)queryGroup.CompareType,
                LinkType = (LinkType)queryGroup.LinkType,
                MemberName = "Value",
                ConstantValue = queryGroup.ConstantValue
            };
            return true;
        }
    }
}
