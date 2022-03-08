using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AgentWorker.AzureDevOps.Models
{
    public class JobRequest
    {
        [JsonPropertyName("requestId")] public int RequestId { get; set; }

        [JsonPropertyName("queueTime")] public DateTime QueueTime { get; set; }

        [JsonPropertyName("assignTime")] public DateTime AssignTime { get; set; }

        [JsonPropertyName("receiveTime")] public DateTime ReceiveTime { get; set; }

        [JsonPropertyName("lockedUntil")] public DateTime LockedUntil { get; set; }

        [JsonPropertyName("serviceOwner")] public string ServiceOwner { get; set; }

        [JsonPropertyName("hostId")] public string HostId { get; set; }

        [JsonPropertyName("scopeId")] public string ScopeId { get; set; }

        [JsonPropertyName("planType")] public string PlanType { get; set; }

        [JsonPropertyName("planId")] public string PlanId { get; set; }

        [JsonPropertyName("jobId")] public string JobId { get; set; }

        [JsonPropertyName("demands")] public List<string> Demands { get; set; }

        [JsonPropertyName("reservedAgent")] public ReservedAgent ReservedAgent { get; set; }

        [JsonPropertyName("definition")] public Definition Definition { get; set; }

        [JsonPropertyName("owner")] public Owner Owner { get; set; }

        [JsonPropertyName("data")] public Data Data { get; set; }

        [JsonPropertyName("poolId")] public int PoolId { get; set; }

        [JsonPropertyName("orchestrationId")] public string OrchestrationId { get; set; }

        [JsonPropertyName("matchesAllAgentsInPool")]
        public bool MatchesAllAgentsInPool { get; set; }

        [JsonPropertyName("priority")] public int Priority { get; set; }

        public override string ToString()
        {
            return
                $"Request: {RequestId}; Queued: {QueueTime}; Assigned: {AssignTime}; Pipeline: {Definition.Name}; Agent: {ReservedAgent.Name}";
        }
    }
}