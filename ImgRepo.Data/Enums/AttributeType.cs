using ImgRepo.Data.Interface;

namespace ImgRepo.Data.Enums
{
    /// <summary>
    /// 屬性類型
    /// </summary>
    public class AttributeType
    {
        public const long Name = 0;
        public const long Tag = 1;
        public const long Category = 2;
        public const long Website = 3;

        static Dictionary<string, long> m_typeNameIdRecords;

        static Dictionary<string, AttributeMetaData> m_attributeMetaData;

        static AttributeType()
        {
            m_typeNameIdRecords ??= new();
            typeof(AttributeType)
                .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Where(f => f.FieldType == typeof(long))
                .ToList()
                .ForEach(f => m_typeNameIdRecords.Add(f.Name.ToLower(), (long)f.GetValue(null)!));
            m_attributeMetaData ??= new();
        }

        public static long GetAttributeTypeId(string attrTypeName)
        {
            if (m_typeNameIdRecords.TryGetValue(attrTypeName.ToLower(), out long typeId))
            {
                return typeId;
            }
            return 0;
        }

        public static void RegisterAttributeMetaData(string attrTypeName, Type modelType)
        {
            long typeId = GetAttributeTypeId(attrTypeName);
            if (typeId == 0)
            {
                throw new ArgumentException($"Attribute type name '{attrTypeName}' is not found.");
            }
            if (m_attributeMetaData.ContainsKey(attrTypeName)) return;
            m_attributeMetaData.Add(attrTypeName, new AttributeMetaData { TypeId = typeId, ModelType = modelType });
        }

        public static void RegisterAttributeMetaData<T>(string attrTypeName) where T : IBasicEntityAttribute => RegisterAttributeMetaData(attrTypeName, typeof(T));

        public static AttributeMetaData? GetAttributeMetaData(string attrTypeName)
        {
            if (m_attributeMetaData.TryGetValue(attrTypeName, out AttributeMetaData? metaData))
            {
                return metaData;
            }
            return null;
        }
    }
}
