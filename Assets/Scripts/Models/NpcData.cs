using System;
using Newtonsoft.Json;

namespace Models
{
    [Serializable]
    public class NpcData
    {
        public event Action OnFullNameChangedEvent;
        public event Action OnDescriptionChangedEvent;
        
        [JsonProperty] public string Id { get; private set; }
        [JsonProperty] public string FullName { get; private set; }
        [JsonProperty] public string Description { get; private set; }

        public NpcData(string id, string fullName, string description)
        {
            Id = id;
            FullName = fullName;
            Description = description;
        }
        
        public void SetId(string id)
        {
            Id = id;
        }

        public void SetFullName(string fullName)
        {
            FullName = fullName;
            OnFullNameChangedEvent?.Invoke();
        }
        
        public void SetDescription(string description)
        {
            Description = description;
            OnDescriptionChangedEvent?.Invoke();
        }
    }
}
