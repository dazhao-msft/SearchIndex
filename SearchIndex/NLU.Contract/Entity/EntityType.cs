//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.BizQA.NLU.Contract
{
    public class EntityType
    {
        private static Dictionary<string, List<string>> s_convertableTypes = new Dictionary<string, List<string>>();

        static EntityType()
        {
            foreach (var lst in EntityTypeString.MutualConvertibleTypes)
            {
                foreach (var tp in lst)
                {
                    s_convertableTypes.Add(tp, lst);
                }
            }

            foreach (var kvp in EntityTypeString.DirectionalConvertibleTypes)
            {
                if (s_convertableTypes.ContainsKey(kvp.Key))
                {
                    s_convertableTypes[kvp.Key].AddRange(kvp.Value);
                }
                else
                {
                    s_convertableTypes[kvp.Key] = kvp.Value;
                }
            }
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public DomainScope DomainScope { get; set; }

        public PartnerScope PartnerScope { get; set; }

        public string TableName { get; set; }

        public string ColumnName { get; set; }

        public bool IsKeyCell { get; set; } = false;

        public string BaseType { get; set; }

        public List<string> AlternativeTables { get; set; }

        public EntityType Clone()
        {
            var entityType = new EntityType()
            {
                Id = Id,
                Name = Name,
                DomainScope = DomainScope,
                PartnerScope = PartnerScope,
                TableName = TableName,
                ColumnName = ColumnName,
                IsKeyCell = IsKeyCell,
                AlternativeTables = AlternativeTables,
            };

            return entityType;
        }

        // Evaluate whether source type can be converted to target type
        public static bool IsConvertible(string sourceType, string targetType)
        {
            if (sourceType.Equals(targetType, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (s_convertableTypes.TryGetValue(sourceType, out var lst) && lst.IndexOf(targetType) >= 0)
            {
                return true;
            }

            return false;
        }

        public static void AddConvertible(string sourceType, string targetType)
        {
            if (s_convertableTypes.TryGetValue(sourceType, out var lst))
            {
                lst.Add(targetType);
            }
            else
            {
                s_convertableTypes.Add(sourceType, new List<string>() { targetType });
            }
        }
    }
}
