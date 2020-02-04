using System.Collections.Generic;

namespace IndexModels
{
    public struct EntityMetadata
    {
        public static readonly IReadOnlyDictionary<string, EntityMetadata> Default = new Dictionary<string, EntityMetadata>()
        {
            { "account", new EntityMetadata("account", "accountid", "name") },
            { "contact", new EntityMetadata("contact", "contactid", "fullname") },
            { "lead", new EntityMetadata("lead", "leadid", "fullname") },
            { "opportunity", new EntityMetadata("opportunity", "opportunityid", "name") },
            { "systemuser", new EntityMetadata("systemuser", "systemuserid", "fullname") },
        };

        public EntityMetadata(string entityName, string entityIdName, string entityPrimaryFieldName)
        {
            EntityName = entityName;
            EntityIdName = entityIdName;
            EntityPrimaryFieldName = entityPrimaryFieldName;
        }

        public string EntityName { get; }

        public string EntityIdName { get; }

        public string EntityPrimaryFieldName { get; }
    }
}
