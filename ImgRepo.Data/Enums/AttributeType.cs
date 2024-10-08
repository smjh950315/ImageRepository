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

        public static void RegisterAttributeMetaData<T>(string attrTypeName) where T : IBasicEntityAttribute => RegisterAttributeMetaData(attrTypeName, typeof(T));


        public static long GetAttributeTypeId(string attrTypeName)
        {
            return TypeNameIdRecords.TryGetValue(attrTypeName.ToLower(), out long typeId) ? typeId : Invalid;
        }

        public static long GetAttributeTypeId(Type modelType)
        {
            return AttributeMetaDatas.Values.FirstOrDefault(x => x.ModelType == modelType)?.TypeId ?? Invalid;
        }

        public static long GetAttributeTypeId<T>() => GetAttributeTypeId(typeof(T));

        public static AttributeMetaData? GetAttributeMetaData(string attrTypeName)
        {
            return AttributeMetaDatas.TryGetValue(attrTypeName, out AttributeMetaData? metaData) ? metaData : null;
        }

        public static Type? GetAttributeType(long attrTypeId)
        {
            return AttributeMetaDatas.Values.FirstOrDefault(m => m.TypeId == attrTypeId)?.ModelType;
        }
        public static Type? GetAttributeType(string attrTypeName)
        {
            return GetAttributeType(GetAttributeTypeId(attrTypeName));
        }
    }
}
