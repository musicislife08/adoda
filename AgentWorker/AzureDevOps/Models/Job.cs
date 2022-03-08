using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AgentWorker.AzureDevOps.Models
{
    public class Job
    {
        [JsonPropertyName("requestId")] public int RequestId { get; set; }

        [JsonPropertyName("queueTime")] public DateTimeOffset QueueTime { get; set; }

        [JsonPropertyName("assignTime")] public DateTimeOffset AssignTime { get; set; }

        [JsonPropertyName("receiveTime")] public DateTimeOffset ReceiveTime { get; set; }

        [JsonPropertyName("finishTime")] public DateTimeOffset FinishTime { get; set; }

        [JsonPropertyName("result")] public int Result { get; set; }

        [JsonPropertyName("serviceOwner")] public string ServiceOwner { get; set; }

        [JsonPropertyName("hostId")] public string HostId { get; set; }

        [JsonPropertyName("scopeId")] public string ScopeId { get; set; }

        [JsonPropertyName("planType")] public string PlanType { get; set; }

        [JsonPropertyName("planId")] public string PlanId { get; set; }

        [JsonPropertyName("jobId")] public string JobId { get; set; }

        [JsonPropertyName("demands")] public List<string> Demands { get; set; }

        [JsonPropertyName("definition")] public Definition Definition { get; set; }


        [JsonPropertyName("data")] public Data Data { get; set; }

        [JsonPropertyName("poolId")] public int PoolId { get; set; }

        [JsonPropertyName("orchestrationId")] public string OrchestrationId { get; set; }

        [JsonPropertyName("matchesAllAgentsInPool")]
        public bool MatchesAllAgentsInPool { get; set; }

        [JsonPropertyName("priority")] public int Priority { get; set; }

        public bool IsCompleted
        {
            get { return this.FinishTime.UtcDateTime > new DateTime(2000, 1, 1).ToUniversalTime(); }
        }

        public override string ToString()
        {
            return
                $"Completed:{this.IsCompleted}; QT:{QueueTime}; AT:{this.AssignTime}; RT:{this.ReceiveTime};FT:{this.FinishTime}";
        }
    }
}