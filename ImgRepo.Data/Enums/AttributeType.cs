using ImgRepo.Data.Interface;

namespace ImgRepo.Data.Enums
{
    /// <summary>
    /// 屬性類型
    /// </summary>
    public class AttributeType
    {
        public const long Invalid = 0;
        public const long Name = 1;
        public const long Tag = 2;
        public const long Category = 3;
        public const long Website = 4;

        static readonly Dictionary<string, long> TypeNameIdRecords;

        static readonly Dictionary<string, AttributeMetaData> AttributeMetaDatas;

        static AttributeType()
        {
            TypeNameIdRecords ??= new();
            typeof(AttributeType)
                .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Where(f => f.FieldType == typeof(long))
                .ToList()
                .ForEach(f => TypeNameIdRecords.Add(f.Name.ToLower(), (long)f.GetValue(null)!));
            AttributeMetaDatas ??= new();
        }

        /// <summary>
        /// 註冊物件屬性DB模型
        /// </summary>
        /// <param name="attrTypeName">物件屬性名稱</param>
        /// <param name="modelType">物件屬性DB模型</param>
        /// <exception cref="ArgumentException"></exception>
        public static void RegisterAttributeMetaData(string attrTypeName, Type modelType)
        {
            string _attrTypeName = attrTypeName.ToLower();
            long typeId = GetAttributeTypeId(_attrTypeName);
            if (typeId == Invalid)
            {
                throw new ArgumentException($"Attribute type name '{attrTypeName}' is not found.");
            }
            if (AttributeMetaDatas.ContainsKey(_attrTypeName)) return;
            AttributeMetaDatas.Add(_attrTypeName, new AttributeMetaData
            {
                TypeId = typeId,
                ModelType = modelType
            });
        }

        /// <summary>
        /// 註冊物件屬性DB模型
        /// </summary>
        /// <param name="attrTypeName">物件屬性名稱</param>
        /// <typeparam name="T">物件屬性DB模型</typeparam>
        /// <exception cref="ArgumentException"></exception>
        public static void RegisterAttributeMetaData<T>(string attrTypeName) where T : IBasicEntityAttribute => RegisterAttributeMetaData(attrTypeName, typeof(T));

        /// <summary>
        /// 使用物件屬性名稱取得物件屬性類型ID
        /// </summary>
        /// <param name="attrTypeName"></param>
        /// <returns>物件屬性類型ID</returns>
        public static long GetAttributeTypeId(string attrTypeName)
        {
            return TypeNameIdRecords.TryGetValue(attrTypeName.ToLower(), out long typeId) ? typeId : Invalid;
        }

        /// <summary>
        /// 使用物件屬性類型取得物件屬性類型ID
        /// </summary>
        /// <param name="modelType">物件屬性類型</param>
        /// <returns>物件屬性類型ID</returns>
        public static long GetAttributeTypeId(Type modelType)
        {
            return AttributeMetaDatas.Values.FirstOrDefault(x => x.ModelType == modelType)?.TypeId ?? Invalid;
        }

        /// <summary>
        /// 使用物件屬性類型取得物件屬性類型ID
        /// </summary>
        /// <typeparam name="T">物件屬性類型</typeparam>
        /// <returns>物件屬性類型ID</returns>
        public static long GetAttributeTypeId<T>() => GetAttributeTypeId(typeof(T));

        /// <summary>
        /// 使用物件名稱取得物件屬性DB模型的中繼資料
        /// </summary>
        /// <param name="attrTypeName"></param>
        /// <returns>物件屬性DB模型的中繼資料</returns>
        public static AttributeMetaData? GetAttributeMetaData(string attrTypeName)
        {
            return AttributeMetaDatas.TryGetValue(attrTypeName, out AttributeMetaData? metaData) ? metaData : null;
        }

        /// <summary>
        /// 使用物件屬性類型ID取得物件屬性DB模型
        /// </summary>
        /// <param name="attrTypeId"></param>
        /// <returns>物件屬性DB模型</returns>
        public static Type? GetAttributeType(long attrTypeId)
        {
            return AttributeMetaDatas.Values.FirstOrDefault(m => m.TypeId == attrTypeId)?.ModelType;
        }

        /// <summary>
        /// 使用物件屬性名稱取得物件屬性DB模型
        /// </summary>
        /// <param name="attrTypeName">物件屬性名稱</param>
        /// <returns>DB模型</returns>
        public static Type? GetAttributeType(string attrTypeName)
        {
            return GetAttributeType(GetAttributeTypeId(attrTypeName));
        }
    }
}
