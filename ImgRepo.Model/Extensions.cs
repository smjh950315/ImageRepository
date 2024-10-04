using Cyh.Net;
using Cyh.Net.Data.Predicate;
using ImgRepo.Model.Query;
using System.Diagnostics.CodeAnalysis;

namespace ImgRepo.Model
{
    public static class Extensions
    {
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
                MemberName = queryGroup.MemberName,
                ConstantValue = queryGroup.ConstantValue
            };
            return true;
        }
    }
}
