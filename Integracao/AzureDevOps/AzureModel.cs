using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArmsFW.Integracao.DevOps
{
    public class WorkItemPayloadResponse
    {
        public WorkItemPayloadResponse()
        {
            Fields = new Fields();
            Links = new Links();
        }
        public int? Id { get; set; }
        public int? Rev { get; set; }
        public Fields Fields { get; set; }
        public Links Links { get; set; }
        public Uri Url { get; set; }
    }

    public class Fields
    {
        [JsonProperty("System.AreaPath", NullValueHandling = NullValueHandling.Ignore)]
        public string SystemAreaPath { get; set; }

        [JsonProperty("System.TeamProject", NullValueHandling = NullValueHandling.Ignore)]
        public string SystemTeamProject { get; set; }

        [JsonProperty("System.IterationPath", NullValueHandling = NullValueHandling.Ignore)]
        public string SystemIterationPath { get; set; }

        [JsonProperty("System.WorkItemType", NullValueHandling = NullValueHandling.Ignore)]
        public string SystemWorkItemType { get; set; }

        [JsonProperty("System.State", NullValueHandling = NullValueHandling.Ignore)]
        public string SystemState { get; set; }

        [JsonProperty("System.Reason", NullValueHandling = NullValueHandling.Ignore)]
        public string SystemReason { get; set; }

        [JsonProperty("System.AssignedTo", NullValueHandling = NullValueHandling.Ignore)]
        public Identidade SystemAssignedTo { get; set; }

        [JsonProperty("System.CreatedDate", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime SystemCreatedDate { get; set; }

        [JsonProperty("System.CreatedBy", NullValueHandling = NullValueHandling.Ignore)]
        public Identidade SystemCreatedBy { get; set; }

        [JsonProperty("System.ChangedDate", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime SystemChangedDate { get; set; }

        [JsonProperty("System.ChangedBy", NullValueHandling = NullValueHandling.Ignore)]
        public Identidade SystemChangedBy { get; set; }

        [JsonProperty("System.CommentCount", NullValueHandling = NullValueHandling.Ignore)]
        public int SystemCommentCount { get; set; }

        [JsonProperty("System.Title", NullValueHandling = NullValueHandling.Ignore)]
        public string SystemTitle { get; set; }

        [JsonProperty("Microsoft.VSTS.Common.StateChangeDate", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime MicrosoftVSTSCommonStateChangeDate { get; set; }

        [JsonProperty("Custom.NOMEDOPROJETO", NullValueHandling = NullValueHandling.Ignore)]
        public string CustomNOMEDOPROJETO { get; set; }

        [JsonProperty("Custom.TIPOLOGIA", NullValueHandling = NullValueHandling.Ignore)]
        public string CustomTIPOLOGIA { get; set; }

        [JsonProperty("Custom.FASE", NullValueHandling = NullValueHandling.Ignore)]
        public string CustomFASE { get; set; }

        [JsonProperty("href", NullValueHandling = NullValueHandling.Ignore)]
        public string href { get; set; }
    }

    public class Identidade
    {
        public string DisplayName { get; set; }
        public Uri Url { get; set; }
        public IdentidadeLinks Links { get; set; }
        public Guid? Id { get; set; }
        public string UniqueName { get; set; }
        public Uri ImageUrl { get; set; }
        public string Descriptor { get; set; }
    }

    public class IdentidadeLinks
    {
        public Link Avatar { get; set; }
    }

    public class Link
    {
        public Uri Href { get; set; }
    }

    public class Links
    {
        public Link Self { get; set; }
        public Link WorkItemUpdates { get; set; }
        public Link WorkItemRevisions { get; set; }
        public Link WorkItemComments { get; set; }
        public Link Html { get; set; }
        public Link WorkItemType { get; set; }
        public Link Fields { get; set; }
    }
}
