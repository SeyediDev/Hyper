using System.ComponentModel.DataAnnotations; using System.ComponentModel.DataAnnotations.Schema; using Neo.Domain.Entities.Base;
namespace Hyper.Domain.Entities.Database; public abstract class SqlServerEntity : BaseEntity<string> { [NotMapped] public new string Id { get; set; } = string.Empty; }
[Table("ACT_GE_BYTEARRAY", Schema="dbo")] public sealed class SqlActGeBytearray : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int? Rev { get; set; }
    [Column("NAME_")] public string? Name { get; set; }
    [Column("DEPLOYMENT_ID_")] public string? DeploymentId { get; set; }
    [Column("BYTES_")] public string? Bytes { get; set; }
    [Column("GENERATED_")] public byte? Generated { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("TYPE_")] public int? Type { get; set; }
    [Column("CREATE_TIME_")] public DateTime? CreateTime { get; set; }
    [Column("ROOT_PROC_INST_ID_")] public string? RootProcInstId { get; set; }
    [Column("REMOVAL_TIME_")] public DateTime? RemovalTime { get; set; }
}
[Table("ACT_GE_PROPERTY", Schema="dbo")] public sealed class SqlActGeProperty : SqlServerEntity {
    [Column("NAME_")] public string Name { get; set; }
    [Column("VALUE_")] public string? Value { get; set; }
    [Column("REV_")] public int? Rev { get; set; }
}
[Table("ACT_GE_SCHEMA_LOG", Schema="dbo")] public sealed class SqlActGeSchemaLog : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("TIMESTAMP_")] public DateTime? Timestamp { get; set; }
    [Column("VERSION_")] public string? Version { get; set; }
}
[Table("ACT_HI_ACTINST", Schema="dbo")] public sealed class SqlActHiActinst : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("PARENT_ACT_INST_ID_")] public string? ParentActInstId { get; set; }
    [Column("PROC_DEF_KEY_")] public string? ProcDefKey { get; set; }
    [Column("PROC_DEF_ID_")] public string ProcDefId { get; set; }
    [Column("ROOT_PROC_INST_ID_")] public string? RootProcInstId { get; set; }
    [Column("PROC_INST_ID_")] public string ProcInstId { get; set; }
    [Column("EXECUTION_ID_")] public string ExecutionId { get; set; }
    [Column("ACT_ID_")] public string ActId { get; set; }
    [Column("TASK_ID_")] public string? TaskId { get; set; }
    [Column("CALL_PROC_INST_ID_")] public string? CallProcInstId { get; set; }
    [Column("CALL_CASE_INST_ID_")] public string? CallCaseInstId { get; set; }
    [Column("ACT_NAME_")] public string? ActName { get; set; }
    [Column("ACT_TYPE_")] public string ActType { get; set; }
    [Column("ASSIGNEE_")] public string? Assignee { get; set; }
    [Column("START_TIME_")] public DateTime StartTime { get; set; }
    [Column("END_TIME_")] public DateTime? EndTime { get; set; }
    [Column("DURATION_")] public decimal? Duration { get; set; }
    [Column("ACT_INST_STATE_")] public byte? ActInstState { get; set; }
    [Column("SEQUENCE_COUNTER_")] public decimal? SequenceCounter { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("REMOVAL_TIME_")] public DateTime? RemovalTime { get; set; }
}
[Table("ACT_HI_ATTACHMENT", Schema="dbo")] public sealed class SqlActHiAttachment : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int? Rev { get; set; }
    [Column("USER_ID_")] public string? UserId { get; set; }
    [Column("NAME_")] public string? Name { get; set; }
    [Column("DESCRIPTION_")] public string? Description { get; set; }
    [Column("TYPE_")] public string? Type { get; set; }
    [Column("TASK_ID_")] public string? TaskId { get; set; }
    [Column("ROOT_PROC_INST_ID_")] public string? RootProcInstId { get; set; }
    [Column("PROC_INST_ID_")] public string? ProcInstId { get; set; }
    [Column("URL_")] public string? Url { get; set; }
    [Column("CONTENT_ID_")] public string? ContentId { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("CREATE_TIME_")] public DateTime? CreateTime { get; set; }
    [Column("REMOVAL_TIME_")] public DateTime? RemovalTime { get; set; }
}
[Table("ACT_HI_BATCH", Schema="dbo")] public sealed class SqlActHiBatch : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("TYPE_")] public string? Type { get; set; }
    [Column("TOTAL_JOBS_")] public int? TotalJobs { get; set; }
    [Column("JOBS_PER_SEED_")] public int? JobsPerSeed { get; set; }
    [Column("INVOCATIONS_PER_JOB_")] public int? InvocationsPerJob { get; set; }
    [Column("SEED_JOB_DEF_ID_")] public string? SeedJobDefId { get; set; }
    [Column("MONITOR_JOB_DEF_ID_")] public string? MonitorJobDefId { get; set; }
    [Column("BATCH_JOB_DEF_ID_")] public string? BatchJobDefId { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("CREATE_USER_ID_")] public string? CreateUserId { get; set; }
    [Column("START_TIME_")] public DateTime StartTime { get; set; }
    [Column("END_TIME_")] public DateTime? EndTime { get; set; }
    [Column("REMOVAL_TIME_")] public DateTime? RemovalTime { get; set; }
}
[Table("ACT_HI_CASEACTINST", Schema="dbo")] public sealed class SqlActHiCaseactinst : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("PARENT_ACT_INST_ID_")] public string? ParentActInstId { get; set; }
    [Column("CASE_DEF_ID_")] public string CaseDefId { get; set; }
    [Column("CASE_INST_ID_")] public string CaseInstId { get; set; }
    [Column("CASE_ACT_ID_")] public string CaseActId { get; set; }
    [Column("TASK_ID_")] public string? TaskId { get; set; }
    [Column("CALL_PROC_INST_ID_")] public string? CallProcInstId { get; set; }
    [Column("CALL_CASE_INST_ID_")] public string? CallCaseInstId { get; set; }
    [Column("CASE_ACT_NAME_")] public string? CaseActName { get; set; }
    [Column("CASE_ACT_TYPE_")] public string? CaseActType { get; set; }
    [Column("CREATE_TIME_")] public DateTime CreateTime { get; set; }
    [Column("END_TIME_")] public DateTime? EndTime { get; set; }
    [Column("DURATION_")] public decimal? Duration { get; set; }
    [Column("STATE_")] public byte? State { get; set; }
    [Column("REQUIRED_")] public byte? Required { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("ACT_HI_CASEINST", Schema="dbo")] public sealed class SqlActHiCaseinst : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("CASE_INST_ID_")] public string CaseInstId { get; set; }
    [Column("BUSINESS_KEY_")] public string? BusinessKey { get; set; }
    [Column("CASE_DEF_ID_")] public string CaseDefId { get; set; }
    [Column("CREATE_TIME_")] public DateTime CreateTime { get; set; }
    [Column("CLOSE_TIME_")] public DateTime? CloseTime { get; set; }
    [Column("DURATION_")] public decimal? Duration { get; set; }
    [Column("STATE_")] public byte? State { get; set; }
    [Column("CREATE_USER_ID_")] public string? CreateUserId { get; set; }
    [Column("SUPER_CASE_INSTANCE_ID_")] public string? SuperCaseInstanceId { get; set; }
    [Column("SUPER_PROCESS_INSTANCE_ID_")] public string? SuperProcessInstanceId { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("ACT_HI_COMMENT", Schema="dbo")] public sealed class SqlActHiComment : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("TYPE_")] public string? Type { get; set; }
    [Column("TIME_")] public DateTime Time { get; set; }
    [Column("USER_ID_")] public string? UserId { get; set; }
    [Column("TASK_ID_")] public string? TaskId { get; set; }
    [Column("ROOT_PROC_INST_ID_")] public string? RootProcInstId { get; set; }
    [Column("PROC_INST_ID_")] public string? ProcInstId { get; set; }
    [Column("ACTION_")] public string? Action { get; set; }
    [Column("MESSAGE_")] public string? Message { get; set; }
    [Column("FULL_MSG_")] public string? FullMsg { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("REMOVAL_TIME_")] public DateTime? RemovalTime { get; set; }
    [Column("REV_")] public int Rev { get; set; }
}
[Table("ACT_HI_DEC_IN", Schema="dbo")] public sealed class SqlActHiDecIn : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("DEC_INST_ID_")] public string DecInstId { get; set; }
    [Column("CLAUSE_ID_")] public string? ClauseId { get; set; }
    [Column("CLAUSE_NAME_")] public string? ClauseName { get; set; }
    [Column("VAR_TYPE_")] public string? VarType { get; set; }
    [Column("BYTEARRAY_ID_")] public string? BytearrayId { get; set; }
    [Column("DOUBLE_")] public double? Double { get; set; }
    [Column("LONG_")] public decimal? Long { get; set; }
    [Column("TEXT_")] public string? Text { get; set; }
    [Column("TEXT2_")] public string? Text2 { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("CREATE_TIME_")] public DateTime? CreateTime { get; set; }
    [Column("ROOT_PROC_INST_ID_")] public string? RootProcInstId { get; set; }
    [Column("REMOVAL_TIME_")] public DateTime? RemovalTime { get; set; }
}
[Table("ACT_HI_DEC_OUT", Schema="dbo")] public sealed class SqlActHiDecOut : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("DEC_INST_ID_")] public string DecInstId { get; set; }
    [Column("CLAUSE_ID_")] public string? ClauseId { get; set; }
    [Column("CLAUSE_NAME_")] public string? ClauseName { get; set; }
    [Column("RULE_ID_")] public string? RuleId { get; set; }
    [Column("RULE_ORDER_")] public int? RuleOrder { get; set; }
    [Column("VAR_NAME_")] public string? VarName { get; set; }
    [Column("VAR_TYPE_")] public string? VarType { get; set; }
    [Column("BYTEARRAY_ID_")] public string? BytearrayId { get; set; }
    [Column("DOUBLE_")] public double? Double { get; set; }
    [Column("LONG_")] public decimal? Long { get; set; }
    [Column("TEXT_")] public string? Text { get; set; }
    [Column("TEXT2_")] public string? Text2 { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("CREATE_TIME_")] public DateTime? CreateTime { get; set; }
    [Column("ROOT_PROC_INST_ID_")] public string? RootProcInstId { get; set; }
    [Column("REMOVAL_TIME_")] public DateTime? RemovalTime { get; set; }
}
[Table("ACT_HI_DECINST", Schema="dbo")] public sealed class SqlActHiDecinst : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("DEC_DEF_ID_")] public string DecDefId { get; set; }
    [Column("DEC_DEF_KEY_")] public string DecDefKey { get; set; }
    [Column("DEC_DEF_NAME_")] public string? DecDefName { get; set; }
    [Column("PROC_DEF_KEY_")] public string? ProcDefKey { get; set; }
    [Column("PROC_DEF_ID_")] public string? ProcDefId { get; set; }
    [Column("PROC_INST_ID_")] public string? ProcInstId { get; set; }
    [Column("CASE_DEF_KEY_")] public string? CaseDefKey { get; set; }
    [Column("CASE_DEF_ID_")] public string? CaseDefId { get; set; }
    [Column("CASE_INST_ID_")] public string? CaseInstId { get; set; }
    [Column("ACT_INST_ID_")] public string? ActInstId { get; set; }
    [Column("ACT_ID_")] public string? ActId { get; set; }
    [Column("EVAL_TIME_")] public DateTime EvalTime { get; set; }
    [Column("REMOVAL_TIME_")] public DateTime? RemovalTime { get; set; }
    [Column("COLLECT_VALUE_")] public double? CollectValue { get; set; }
    [Column("USER_ID_")] public string? UserId { get; set; }
    [Column("ROOT_DEC_INST_ID_")] public string? RootDecInstId { get; set; }
    [Column("ROOT_PROC_INST_ID_")] public string? RootProcInstId { get; set; }
    [Column("DEC_REQ_ID_")] public string? DecReqId { get; set; }
    [Column("DEC_REQ_KEY_")] public string? DecReqKey { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("ACT_HI_DETAIL", Schema="dbo")] public sealed class SqlActHiDetail : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("TYPE_")] public string Type { get; set; }
    [Column("PROC_DEF_KEY_")] public string? ProcDefKey { get; set; }
    [Column("PROC_DEF_ID_")] public string? ProcDefId { get; set; }
    [Column("ROOT_PROC_INST_ID_")] public string? RootProcInstId { get; set; }
    [Column("PROC_INST_ID_")] public string? ProcInstId { get; set; }
    [Column("EXECUTION_ID_")] public string? ExecutionId { get; set; }
    [Column("CASE_DEF_KEY_")] public string? CaseDefKey { get; set; }
    [Column("CASE_DEF_ID_")] public string? CaseDefId { get; set; }
    [Column("CASE_INST_ID_")] public string? CaseInstId { get; set; }
    [Column("CASE_EXECUTION_ID_")] public string? CaseExecutionId { get; set; }
    [Column("TASK_ID_")] public string? TaskId { get; set; }
    [Column("ACT_INST_ID_")] public string? ActInstId { get; set; }
    [Column("VAR_INST_ID_")] public string? VarInstId { get; set; }
    [Column("NAME_")] public string Name { get; set; }
    [Column("VAR_TYPE_")] public string? VarType { get; set; }
    [Column("REV_")] public int? Rev { get; set; }
    [Column("TIME_")] public DateTime Time { get; set; }
    [Column("BYTEARRAY_ID_")] public string? BytearrayId { get; set; }
    [Column("DOUBLE_")] public double? Double { get; set; }
    [Column("LONG_")] public decimal? Long { get; set; }
    [Column("TEXT_")] public string? Text { get; set; }
    [Column("TEXT2_")] public string? Text2 { get; set; }
    [Column("SEQUENCE_COUNTER_")] public decimal? SequenceCounter { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("OPERATION_ID_")] public string? OperationId { get; set; }
    [Column("REMOVAL_TIME_")] public DateTime? RemovalTime { get; set; }
    [Column("INITIAL_")] public bool? Initial { get; set; }
}
[Table("ACT_HI_EXT_TASK_LOG", Schema="dbo")] public sealed class SqlActHiExtTaskLog : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("TIMESTAMP_")] public DateTime Timestamp { get; set; }
    [Column("EXT_TASK_ID_")] public string ExtTaskId { get; set; }
    [Column("RETRIES_")] public int? Retries { get; set; }
    [Column("TOPIC_NAME_")] public string? TopicName { get; set; }
    [Column("WORKER_ID_")] public string? WorkerId { get; set; }
    [Column("PRIORITY_")] public decimal Priority { get; set; }
    [Column("ERROR_MSG_")] public string? ErrorMsg { get; set; }
    [Column("ERROR_DETAILS_ID_")] public string? ErrorDetailsId { get; set; }
    [Column("ACT_ID_")] public string? ActId { get; set; }
    [Column("ACT_INST_ID_")] public string? ActInstId { get; set; }
    [Column("EXECUTION_ID_")] public string? ExecutionId { get; set; }
    [Column("ROOT_PROC_INST_ID_")] public string? RootProcInstId { get; set; }
    [Column("PROC_INST_ID_")] public string? ProcInstId { get; set; }
    [Column("PROC_DEF_ID_")] public string? ProcDefId { get; set; }
    [Column("PROC_DEF_KEY_")] public string? ProcDefKey { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("STATE_")] public int? State { get; set; }
    [Column("REMOVAL_TIME_")] public DateTime? RemovalTime { get; set; }
}
[Table("ACT_HI_IDENTITYLINK", Schema="dbo")] public sealed class SqlActHiIdentitylink : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("TIMESTAMP_")] public DateTime Timestamp { get; set; }
    [Column("TYPE_")] public string? Type { get; set; }
    [Column("USER_ID_")] public string? UserId { get; set; }
    [Column("GROUP_ID_")] public string? GroupId { get; set; }
    [Column("TASK_ID_")] public string? TaskId { get; set; }
    [Column("ROOT_PROC_INST_ID_")] public string? RootProcInstId { get; set; }
    [Column("PROC_DEF_ID_")] public string? ProcDefId { get; set; }
    [Column("OPERATION_TYPE_")] public string? OperationType { get; set; }
    [Column("ASSIGNER_ID_")] public string? AssignerId { get; set; }
    [Column("PROC_DEF_KEY_")] public string? ProcDefKey { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("REMOVAL_TIME_")] public DateTime? RemovalTime { get; set; }
}
[Table("ACT_HI_INCIDENT", Schema="dbo")] public sealed class SqlActHiIncident : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("PROC_DEF_KEY_")] public string? ProcDefKey { get; set; }
    [Column("PROC_DEF_ID_")] public string? ProcDefId { get; set; }
    [Column("ROOT_PROC_INST_ID_")] public string? RootProcInstId { get; set; }
    [Column("PROC_INST_ID_")] public string? ProcInstId { get; set; }
    [Column("EXECUTION_ID_")] public string? ExecutionId { get; set; }
    [Column("CREATE_TIME_")] public DateTime CreateTime { get; set; }
    [Column("END_TIME_")] public DateTime? EndTime { get; set; }
    [Column("INCIDENT_MSG_")] public string? IncidentMsg { get; set; }
    [Column("INCIDENT_TYPE_")] public string IncidentType { get; set; }
    [Column("ACTIVITY_ID_")] public string? ActivityId { get; set; }
    [Column("FAILED_ACTIVITY_ID_")] public string? FailedActivityId { get; set; }
    [Column("CAUSE_INCIDENT_ID_")] public string? CauseIncidentId { get; set; }
    [Column("ROOT_CAUSE_INCIDENT_ID_")] public string? RootCauseIncidentId { get; set; }
    [Column("CONFIGURATION_")] public string? Configuration { get; set; }
    [Column("HISTORY_CONFIGURATION_")] public string? HistoryConfiguration { get; set; }
    [Column("INCIDENT_STATE_")] public int? IncidentState { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("JOB_DEF_ID_")] public string? JobDefId { get; set; }
    [Column("ANNOTATION_")] public string? Annotation { get; set; }
    [Column("REMOVAL_TIME_")] public DateTime? RemovalTime { get; set; }
}
[Table("ACT_HI_JOB_LOG", Schema="dbo")] public sealed class SqlActHiJobLog : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("TIMESTAMP_")] public DateTime Timestamp { get; set; }
    [Column("JOB_ID_")] public string JobId { get; set; }
    [Column("JOB_DUEDATE_")] public DateTime? JobDuedate { get; set; }
    [Column("JOB_RETRIES_")] public int? JobRetries { get; set; }
    [Column("JOB_PRIORITY_")] public decimal JobPriority { get; set; }
    [Column("JOB_EXCEPTION_MSG_")] public string? JobExceptionMsg { get; set; }
    [Column("JOB_EXCEPTION_STACK_ID_")] public string? JobExceptionStackId { get; set; }
    [Column("JOB_STATE_")] public int? JobState { get; set; }
    [Column("JOB_DEF_ID_")] public string? JobDefId { get; set; }
    [Column("JOB_DEF_TYPE_")] public string? JobDefType { get; set; }
    [Column("JOB_DEF_CONFIGURATION_")] public string? JobDefConfiguration { get; set; }
    [Column("ACT_ID_")] public string? ActId { get; set; }
    [Column("FAILED_ACT_ID_")] public string? FailedActId { get; set; }
    [Column("EXECUTION_ID_")] public string? ExecutionId { get; set; }
    [Column("ROOT_PROC_INST_ID_")] public string? RootProcInstId { get; set; }
    [Column("PROCESS_INSTANCE_ID_")] public string? ProcessInstanceId { get; set; }
    [Column("PROCESS_DEF_ID_")] public string? ProcessDefId { get; set; }
    [Column("PROCESS_DEF_KEY_")] public string? ProcessDefKey { get; set; }
    [Column("DEPLOYMENT_ID_")] public string? DeploymentId { get; set; }
    [Column("SEQUENCE_COUNTER_")] public decimal? SequenceCounter { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("HOSTNAME_")] public string? Hostname { get; set; }
    [Column("REMOVAL_TIME_")] public DateTime? RemovalTime { get; set; }
    [Column("BATCH_ID_")] public string? BatchId { get; set; }
}
[Table("ACT_HI_OP_LOG", Schema="dbo")] public sealed class SqlActHiOpLog : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("DEPLOYMENT_ID_")] public string? DeploymentId { get; set; }
    [Column("PROC_DEF_ID_")] public string? ProcDefId { get; set; }
    [Column("PROC_DEF_KEY_")] public string? ProcDefKey { get; set; }
    [Column("ROOT_PROC_INST_ID_")] public string? RootProcInstId { get; set; }
    [Column("PROC_INST_ID_")] public string? ProcInstId { get; set; }
    [Column("EXECUTION_ID_")] public string? ExecutionId { get; set; }
    [Column("CASE_DEF_ID_")] public string? CaseDefId { get; set; }
    [Column("CASE_INST_ID_")] public string? CaseInstId { get; set; }
    [Column("CASE_EXECUTION_ID_")] public string? CaseExecutionId { get; set; }
    [Column("TASK_ID_")] public string? TaskId { get; set; }
    [Column("JOB_ID_")] public string? JobId { get; set; }
    [Column("JOB_DEF_ID_")] public string? JobDefId { get; set; }
    [Column("BATCH_ID_")] public string? BatchId { get; set; }
    [Column("USER_ID_")] public string? UserId { get; set; }
    [Column("TIMESTAMP_")] public DateTime Timestamp { get; set; }
    [Column("OPERATION_TYPE_")] public string? OperationType { get; set; }
    [Column("OPERATION_ID_")] public string? OperationId { get; set; }
    [Column("ENTITY_TYPE_")] public string? EntityType { get; set; }
    [Column("PROPERTY_")] public string? Property { get; set; }
    [Column("ORG_VALUE_")] public string? OrgValue { get; set; }
    [Column("NEW_VALUE_")] public string? NewValue { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("REMOVAL_TIME_")] public DateTime? RemovalTime { get; set; }
    [Column("CATEGORY_")] public string? Category { get; set; }
    [Column("EXTERNAL_TASK_ID_")] public string? ExternalTaskId { get; set; }
    [Column("ANNOTATION_")] public string? Annotation { get; set; }
}
[Table("ACT_HI_PROCINST", Schema="dbo")] public sealed class SqlActHiProcinst : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("PROC_INST_ID_")] public string ProcInstId { get; set; }
    [Column("BUSINESS_KEY_")] public string? BusinessKey { get; set; }
    [Column("PROC_DEF_KEY_")] public string? ProcDefKey { get; set; }
    [Column("PROC_DEF_ID_")] public string ProcDefId { get; set; }
    [Column("START_TIME_")] public DateTime StartTime { get; set; }
    [Column("END_TIME_")] public DateTime? EndTime { get; set; }
    [Column("REMOVAL_TIME_")] public DateTime? RemovalTime { get; set; }
    [Column("DURATION_")] public decimal? Duration { get; set; }
    [Column("START_USER_ID_")] public string? StartUserId { get; set; }
    [Column("START_ACT_ID_")] public string? StartActId { get; set; }
    [Column("END_ACT_ID_")] public string? EndActId { get; set; }
    [Column("SUPER_PROCESS_INSTANCE_ID_")] public string? SuperProcessInstanceId { get; set; }
    [Column("ROOT_PROC_INST_ID_")] public string? RootProcInstId { get; set; }
    [Column("SUPER_CASE_INSTANCE_ID_")] public string? SuperCaseInstanceId { get; set; }
    [Column("CASE_INST_ID_")] public string? CaseInstId { get; set; }
    [Column("DELETE_REASON_")] public string? DeleteReason { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("STATE_")] public string? State { get; set; }
    [Column("RESTARTED_PROC_INST_ID_")] public string? RestartedProcInstId { get; set; }
}
[Table("ACT_HI_TASKINST", Schema="dbo")] public sealed class SqlActHiTaskinst : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("TASK_DEF_KEY_")] public string? TaskDefKey { get; set; }
    [Column("PROC_DEF_KEY_")] public string? ProcDefKey { get; set; }
    [Column("PROC_DEF_ID_")] public string? ProcDefId { get; set; }
    [Column("ROOT_PROC_INST_ID_")] public string? RootProcInstId { get; set; }
    [Column("PROC_INST_ID_")] public string? ProcInstId { get; set; }
    [Column("EXECUTION_ID_")] public string? ExecutionId { get; set; }
    [Column("CASE_DEF_KEY_")] public string? CaseDefKey { get; set; }
    [Column("CASE_DEF_ID_")] public string? CaseDefId { get; set; }
    [Column("CASE_INST_ID_")] public string? CaseInstId { get; set; }
    [Column("CASE_EXECUTION_ID_")] public string? CaseExecutionId { get; set; }
    [Column("ACT_INST_ID_")] public string? ActInstId { get; set; }
    [Column("NAME_")] public string? Name { get; set; }
    [Column("PARENT_TASK_ID_")] public string? ParentTaskId { get; set; }
    [Column("DESCRIPTION_")] public string? Description { get; set; }
    [Column("OWNER_")] public string? Owner { get; set; }
    [Column("ASSIGNEE_")] public string? Assignee { get; set; }
    [Column("START_TIME_")] public DateTime StartTime { get; set; }
    [Column("END_TIME_")] public DateTime? EndTime { get; set; }
    [Column("DURATION_")] public decimal? Duration { get; set; }
    [Column("DELETE_REASON_")] public string? DeleteReason { get; set; }
    [Column("PRIORITY_")] public int? Priority { get; set; }
    [Column("DUE_DATE_")] public DateTime? DueDate { get; set; }
    [Column("FOLLOW_UP_DATE_")] public DateTime? FollowUpDate { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("REMOVAL_TIME_")] public DateTime? RemovalTime { get; set; }
    [Column("TASK_STATE_")] public string? TaskState { get; set; }
}
[Table("ACT_HI_VARINST", Schema="dbo")] public sealed class SqlActHiVarinst : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("PROC_DEF_KEY_")] public string? ProcDefKey { get; set; }
    [Column("PROC_DEF_ID_")] public string? ProcDefId { get; set; }
    [Column("ROOT_PROC_INST_ID_")] public string? RootProcInstId { get; set; }
    [Column("PROC_INST_ID_")] public string? ProcInstId { get; set; }
    [Column("EXECUTION_ID_")] public string? ExecutionId { get; set; }
    [Column("CASE_DEF_KEY_")] public string? CaseDefKey { get; set; }
    [Column("CASE_DEF_ID_")] public string? CaseDefId { get; set; }
    [Column("CASE_INST_ID_")] public string? CaseInstId { get; set; }
    [Column("CASE_EXECUTION_ID_")] public string? CaseExecutionId { get; set; }
    [Column("ACT_INST_ID_")] public string? ActInstId { get; set; }
    [Column("TASK_ID_")] public string? TaskId { get; set; }
    [Column("NAME_")] public string Name { get; set; }
    [Column("VAR_TYPE_")] public string? VarType { get; set; }
    [Column("CREATE_TIME_")] public DateTime? CreateTime { get; set; }
    [Column("REV_")] public int? Rev { get; set; }
    [Column("BYTEARRAY_ID_")] public string? BytearrayId { get; set; }
    [Column("DOUBLE_")] public double? Double { get; set; }
    [Column("LONG_")] public decimal? Long { get; set; }
    [Column("TEXT_")] public string? Text { get; set; }
    [Column("TEXT2_")] public string? Text2 { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("STATE_")] public string? State { get; set; }
    [Column("REMOVAL_TIME_")] public DateTime? RemovalTime { get; set; }
}
[Table("ACT_ID_GROUP", Schema="dbo")] public sealed class SqlActIdGroup : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int? Rev { get; set; }
    [Column("NAME_")] public string? Name { get; set; }
    [Column("TYPE_")] public string? Type { get; set; }
}
[Table("ACT_ID_INFO", Schema="dbo")] public sealed class SqlActIdInfo : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int? Rev { get; set; }
    [Column("USER_ID_")] public string? UserId { get; set; }
    [Column("TYPE_")] public string? Type { get; set; }
    [Column("KEY_")] public string? Key { get; set; }
    [Column("VALUE_")] public string? Value { get; set; }
    [Column("PASSWORD_")] public string? Password { get; set; }
    [Column("PARENT_ID_")] public string? ParentId { get; set; }
}
[Table("ACT_ID_MEMBERSHIP", Schema="dbo")] public sealed class SqlActIdMembership : SqlServerEntity {
    [Column("USER_ID_")] [Key] public string UserId { get; set; }
    [Column("GROUP_ID_")] public string GroupId { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("HASH_")] public string? Hash { get; set; }
    [Column("REMOVAL_TIME_")] public DateTime? RemovalTime { get; set; }
}
[Table("ACT_ID_REMEMBER_ME", Schema="dbo")] public sealed class SqlActIdRememberMe : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int Rev { get; set; }
    [Column("SELECTOR_")] public string Selector { get; set; }
    [Column("VALIDATOR_")] public string Validator { get; set; }
    [Column("USER_ID_")] public string UserId { get; set; }
    [Column("LAST_USED_")] public DateTime LastUsed { get; set; }
}
[Table("ACT_ID_TENANT", Schema="dbo")] public sealed class SqlActIdTenant : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int? Rev { get; set; }
    [Column("NAME_")] public string? Name { get; set; }
    [Column("PARENT_ID_")] public string? ParentId { get; set; }
}
[Table("ACT_ID_TENANT_MEMBER", Schema="dbo")] public sealed class SqlActIdTenantMember : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("TENANT_ID_")] public string TenantId { get; set; }
    [Column("USER_ID_")] public string? UserId { get; set; }
    [Column("GROUP_ID_")] public string? GroupId { get; set; }
    [Column("REMOVAL_TIME_")] public DateTime? RemovalTime { get; set; }
}
[Table("ACT_ID_USER", Schema="dbo")] public sealed class SqlActIdUser : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int? Rev { get; set; }
    [Column("FIRST_")] public string? First { get; set; }
    [Column("LAST_")] public string? Last { get; set; }
    [Column("EMAIL_")] public string? Email { get; set; }
    [Column("PWD_")] public string? Pwd { get; set; }
    [Column("SALT_")] public string? Salt { get; set; }
    [Column("LOCK_EXP_TIME_")] public DateTime? LockExpTime { get; set; }
    [Column("ATTEMPTS_")] public int? Attempts { get; set; }
    [Column("PICTURE_ID_")] public string? PictureId { get; set; }
    [Column("MOBILE_")] public string? Mobile { get; set; }
    [Column("CREATE_TIME_")] public DateTime CreateTime { get; set; }
    [Column("UPDATE_TIME_")] public DateTime? UpdateTime { get; set; }
    [Column("BLOCK_")] public bool Block { get; set; }
    [Column("FULLNAME_")] public string Fullname { get; set; }
    [Column("HASH_")] public string? Hash { get; set; }
}
[Table("ACT_RE_ASSOCIATION", Schema="dbo")] public sealed class SqlActReAssociation : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("NAME_")] public string Name { get; set; }
    [Column("REV_")] public int Rev { get; set; }
    [Column("TYPE_")] public string Type { get; set; }
    [Column("TABLE_ID_")] public string TableId { get; set; }
    [Column("INCOMING_NAME_")] public string? IncomingName { get; set; }
    [Column("OUTCOMING_NAME_")] public string? OutcomingName { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("ACT_RE_CAMFORMDEF", Schema="dbo")] public sealed class SqlActReCamformdef : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int? Rev { get; set; }
    [Column("KEY_")] public string Key { get; set; }
    [Column("VERSION_")] public int Version { get; set; }
    [Column("DEPLOYMENT_ID_")] public string? DeploymentId { get; set; }
    [Column("RESOURCE_NAME_")] public string? ResourceName { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("ACT_RE_CASE_DEF", Schema="dbo")] public sealed class SqlActReCaseDef : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int? Rev { get; set; }
    [Column("CATEGORY_")] public string? Category { get; set; }
    [Column("NAME_")] public string? Name { get; set; }
    [Column("KEY_")] public string Key { get; set; }
    [Column("VERSION_")] public int Version { get; set; }
    [Column("DEPLOYMENT_ID_")] public string? DeploymentId { get; set; }
    [Column("RESOURCE_NAME_")] public string? ResourceName { get; set; }
    [Column("DGRM_RESOURCE_NAME_")] public string? DgrmResourceName { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("HISTORY_TTL_")] public int? HistoryTtl { get; set; }
}
[Table("ACT_RE_COLUMN", Schema="dbo")] public sealed class SqlActReColumn : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("OLD_ID_")] public string? OldId { get; set; }
    [Column("REV_")] public int Rev { get; set; }
    [Column("INDEX_")] public int? Index { get; set; }
    [Column("NAME_")] public string Name { get; set; }
    [Column("NULLABLE_")] public bool Nullable { get; set; }
    [Column("TYPE_")] public string Type { get; set; }
    [Column("KEY_")] public string Key { get; set; }
    [Column("NORMALIZE_")] public bool? Normalize { get; set; }
    [Column("DETAILS_")] public string? Details { get; set; }
    [Column("TABLE_ID_")] public string TableId { get; set; }
    [Column("DEFAULT_")] public bool Default { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("KIND_")] public int Kind { get; set; }
    [Column("ESCAPE_HTML_")] public bool? EscapeHtml { get; set; }
}
[Table("ACT_RE_CONSTRAINT", Schema="dbo")] public sealed class SqlActReConstraint : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("NAME_")] public string Name { get; set; }
    [Column("REV_")] public int Rev { get; set; }
    [Column("TYPE_")] public string Type { get; set; }
    [Column("TABLE_ID_")] public string TableId { get; set; }
    [Column("COLUMN_ID_")] public string ColumnId { get; set; }
    [Column("PARENT_TABLE_ID_")] public string? ParentTableId { get; set; }
    [Column("PARENT_COLUMN_ID_")] public string? ParentColumnId { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("UPDATE_RULE_")] public string? UpdateRule { get; set; }
    [Column("DELETE_RULE_")] public string? DeleteRule { get; set; }
    [Column("SQL_SCRIPT_")] public string? SqlScript { get; set; }
    [Column("ERROR_MESSAGE_")] public string? ErrorMessage { get; set; }
}
[Table("ACT_RE_DASHBOARD", Schema="dbo")] public sealed class SqlActReDashboard : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int Rev { get; set; }
    [Column("NAME_")] public string Name { get; set; }
    [Column("CATEGORY_")] public string? Category { get; set; }
    [Column("DEFINITION_")] public string? Definition { get; set; }
    [Column("DEFAULT_")] public bool Default { get; set; }
    [Column("CREATE_TIME_")] public DateTime CreateTime { get; set; }
    [Column("UPDATE_TIME_")] public DateTime? UpdateTime { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("ACT_RE_DATA_FORM", Schema="dbo")] public sealed class SqlActReDataForm : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int Rev { get; set; }
    [Column("NAME_")] public string Name { get; set; }
    [Column("CATEGORY_")] public string? Category { get; set; }
    [Column("TABLE_ID_")] public string TableId { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("CUSTOM_HINTS_")] public string? CustomHints { get; set; }
    [Column("SYNC_")] public bool Sync { get; set; }
}
[Table("ACT_RE_DATA_FORM_COLUMN", Schema="dbo")] public sealed class SqlActReDataFormColumn : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int Rev { get; set; }
    [Column("NAME_")] public string? Name { get; set; }
    [Column("TYPE_")] public int Type { get; set; }
    [Column("ORDER_")] public double Order { get; set; }
    [Column("VISIBLE_")] public bool? Visible { get; set; }
    [Column("SUMMABLE_")] public bool? Summable { get; set; }
    [Column("FILTERABLE_")] public bool? Filterable { get; set; }
    [Column("SUGGESTABLE_")] public bool? Suggestable { get; set; }
    [Column("CATEGORY_")] public string? Category { get; set; }
    [Column("SHOW_TYPE_")] public int? ShowType { get; set; }
    [Column("ACTION_TYPE_")] public int? ActionType { get; set; }
    [Column("DATA_FORM_ID_")] public string DataFormId { get; set; }
    [Column("REF_TABLE_ID_")] public string? RefTableId { get; set; }
    [Column("REF_COLUMN_ID_")] public string? RefColumnId { get; set; }
    [Column("REF_FORM_ID_")] public string? RefFormId { get; set; }
    [Column("REF_FORM_RELATIONS_")] public string? RefFormRelations { get; set; }
    [Column("REF_DATA_FORM_ID_")] public string? RefDataFormId { get; set; }
    [Column("REF_ASSOCIATION_TABLE_ID_")] public string? RefAssociationTableId { get; set; }
    [Column("REF_ASSOCIATION_ID_")] public string? RefAssociationId { get; set; }
    [Column("CHAIN_ASSOCIATIONS_")] public string? ChainAssociations { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("DATA_FORMAT_")] public string? DataFormat { get; set; }
    [Column("SQL_SCRIPT_")] public string? SqlScript { get; set; }
    [Column("SQL_RESULT_TYPE_")] public string? SqlResultType { get; set; }
    [Column("SUGGEST_DETAILS_")] public string? SuggestDetails { get; set; }
    [Column("OPERATION_DETAILS_")] public string? OperationDetails { get; set; }
    [Column("OUT_FULL_DATA_")] public bool? OutFullData { get; set; }
    [Column("READONLY_RELATIONS_")] public string? ReadonlyRelations { get; set; }
    [Column("SORTABLE_")] public bool? Sortable { get; set; }
    [Column("ICON_")] public string? Icon { get; set; }
    [Column("EXPORTABLE_")] public bool? Exportable { get; set; }
    [Column("TAGS_")] public string? Tags { get; set; }
}
[Table("ACT_RE_DATA_FORM_FILTER", Schema="dbo")] public sealed class SqlActReDataFormFilter : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("VALUE_")] public string? Value { get; set; }
    [Column("DATA_FORM_ID_")] public string DataFormId { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("ACT_RE_DATA_FORM_HEADER", Schema="dbo")] public sealed class SqlActReDataFormHeader : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int Rev { get; set; }
    [Column("NAME_")] public string? Name { get; set; }
    [Column("TYPE_")] public int Type { get; set; }
    [Column("ORDER_")] public double Order { get; set; }
    [Column("ACTION_TYPE_")] public int? ActionType { get; set; }
    [Column("DATA_FORM_ID_")] public string DataFormId { get; set; }
    [Column("ASSIGNEE_VAR_")] public string? AssigneeVar { get; set; }
    [Column("REF_FORM_ID_")] public string? RefFormId { get; set; }
    [Column("PROCESS_KEY_")] public string? ProcessKey { get; set; }
    [Column("PROCESS_KEY_NAME_")] public string? ProcessKeyName { get; set; }
    [Column("PROCESS_TENANT_ID_")] public string? ProcessTenantId { get; set; }
    [Column("PROCESS_BINDING_TYPE_")] public int? ProcessBindingType { get; set; }
    [Column("PROCESS_TAG_")] public string? ProcessTag { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("INLINE_EDIT_")] public bool? InlineEdit { get; set; }
    [Column("PROCESS_NEED_ROW_")] public bool? ProcessNeedRow { get; set; }
    [Column("EXPORT_TYPE_")] public int? ExportType { get; set; }
    [Column("OUT_FULL_DATA_")] public bool? OutFullData { get; set; }
    [Column("READONLY_RELATIONS_")] public string? ReadonlyRelations { get; set; }
    [Column("REF_FORM_RELATIONS_")] public string? RefFormRelations { get; set; }
    [Column("EXPORT_DETAIL_")] public string? ExportDetail { get; set; }
    [Column("EXPORT_LIMIT_")] public int? ExportLimit { get; set; }
}
[Table("ACT_RE_DATA_FORM_SORT", Schema="dbo")] public sealed class SqlActReDataFormSort : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("VALUE_")] public string? Value { get; set; }
    [Column("DATA_FORM_ID_")] public string DataFormId { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("ACT_RE_DATA_REPORT", Schema="dbo")] public sealed class SqlActReDataReport : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int Rev { get; set; }
    [Column("NAME_")] public string Name { get; set; }
    [Column("CATEGORY_")] public string? Category { get; set; }
    [Column("FORM_ID_")] public string FormId { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("CUSTOM_HINTS_")] public string? CustomHints { get; set; }
}
[Table("ACT_RE_DATA_REPORT_COLUMN", Schema="dbo")] public sealed class SqlActReDataReportColumn : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int Rev { get; set; }
    [Column("NAME_")] public string? Name { get; set; }
    [Column("TYPE_")] public int Type { get; set; }
    [Column("ORDER_")] public double Order { get; set; }
    [Column("DATA_REPORT_ID_")] public string DataReportId { get; set; }
    [Column("REF_DATA_FORM_ID_")] public string? RefDataFormId { get; set; }
    [Column("REF_DATA_COLUMN_ID_")] public string? RefDataColumnId { get; set; }
    [Column("AGGREGATION_")] public string? Aggregation { get; set; }
    [Column("DATA_FORMAT_")] public string? DataFormat { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("REF_DATA_FILTER_COLUMN_ID_")] public string? RefDataFilterColumnId { get; set; }
    [Column("SUMMABLE_")] public bool? Summable { get; set; }
}
[Table("ACT_RE_DATA_REPORT_FILTER", Schema="dbo")] public sealed class SqlActReDataReportFilter : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("VALUE_")] public string? Value { get; set; }
    [Column("DATA_REPORT_ID_")] public string DataReportId { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("ACT_RE_DATA_REPORT_HEADER", Schema="dbo")] public sealed class SqlActReDataReportHeader : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int Rev { get; set; }
    [Column("NAME_")] public string? Name { get; set; }
    [Column("TYPE_")] public int Type { get; set; }
    [Column("ORDER_")] public double Order { get; set; }
    [Column("EXPORT_TYPE_")] public int? ExportType { get; set; }
    [Column("DATA_REPORT_ID_")] public string DataReportId { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("EXPORT_DETAIL_")] public string? ExportDetail { get; set; }
    [Column("EXPORT_LIMIT_")] public int? ExportLimit { get; set; }
}
[Table("ACT_RE_DATA_REPORT_SORT", Schema="dbo")] public sealed class SqlActReDataReportSort : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("VALUE_")] public string? Value { get; set; }
    [Column("DATA_REPORT_ID_")] public string DataReportId { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("ACT_RE_DECISION_DEF", Schema="dbo")] public sealed class SqlActReDecisionDef : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int? Rev { get; set; }
    [Column("CATEGORY_")] public string? Category { get; set; }
    [Column("NAME_")] public string? Name { get; set; }
    [Column("KEY_")] public string Key { get; set; }
    [Column("VERSION_")] public int Version { get; set; }
    [Column("DEPLOYMENT_ID_")] public string? DeploymentId { get; set; }
    [Column("RESOURCE_NAME_")] public string? ResourceName { get; set; }
    [Column("DGRM_RESOURCE_NAME_")] public string? DgrmResourceName { get; set; }
    [Column("DEC_REQ_ID_")] public string? DecReqId { get; set; }
    [Column("DEC_REQ_KEY_")] public string? DecReqKey { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("HISTORY_TTL_")] public int? HistoryTtl { get; set; }
    [Column("VERSION_TAG_")] public string? VersionTag { get; set; }
}
[Table("ACT_RE_DECISION_REQ_DEF", Schema="dbo")] public sealed class SqlActReDecisionReqDef : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int? Rev { get; set; }
    [Column("CATEGORY_")] public string? Category { get; set; }
    [Column("NAME_")] public string? Name { get; set; }
    [Column("KEY_")] public string Key { get; set; }
    [Column("VERSION_")] public int Version { get; set; }
    [Column("DEPLOYMENT_ID_")] public string? DeploymentId { get; set; }
    [Column("RESOURCE_NAME_")] public string? ResourceName { get; set; }
    [Column("DGRM_RESOURCE_NAME_")] public string? DgrmResourceName { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("ACT_RE_DEPLOYMENT", Schema="dbo")] public sealed class SqlActReDeployment : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("NAME_")] public string? Name { get; set; }
    [Column("DEPLOY_TIME_")] public DateTime? DeployTime { get; set; }
    [Column("SOURCE_")] public string? Source { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("ACT_RE_FORM", Schema="dbo")] public sealed class SqlActReForm : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int Rev { get; set; }
    [Column("NAME_")] public string Name { get; set; }
    [Column("CATEGORY_")] public string? Category { get; set; }
    [Column("DEFINITION_")] public string? Definition { get; set; }
    [Column("CREATE_TIME_")] public DateTime CreateTime { get; set; }
    [Column("UPDATE_TIME_")] public DateTime? UpdateTime { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("ACT_RE_LINK", Schema="dbo")] public sealed class SqlActReLink : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int Rev { get; set; }
    [Column("NAME_")] public string Name { get; set; }
    [Column("DESCRIPTION_")] public string? Description { get; set; }
    [Column("URL_")] public string Url { get; set; }
    [Column("ORDER_")] public double Order { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("ACT_RE_MENU", Schema="dbo")] public sealed class SqlActReMenu : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int Rev { get; set; }
    [Column("NAME_")] public string Name { get; set; }
    [Column("TYPE_")] public int Type { get; set; }
    [Column("LOGO_")] public string? Logo { get; set; }
    [Column("DETAIL_")] public string? Detail { get; set; }
    [Column("ORDER_")] public double Order { get; set; }
    [Column("PARENT_ID_")] public string? ParentId { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("ACT_RE_MODEL", Schema="dbo")] public sealed class SqlActReModel : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int Rev { get; set; }
    [Column("NAME_")] public string Name { get; set; }
    [Column("CATEGORY_")] public string? Category { get; set; }
    [Column("DEFINITION_")] public string? Definition { get; set; }
    [Column("TYPE_")] public int Type { get; set; }
    [Column("CREATE_TIME_")] public DateTime CreateTime { get; set; }
    [Column("UPDATE_TIME_")] public DateTime? UpdateTime { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("ACT_RE_NOTIFICATION", Schema="dbo")] public sealed class SqlActReNotification : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int Rev { get; set; }
    [Column("SENDER_")] public string? Sender { get; set; }
    [Column("GROUP_ID_")] public string? GroupId { get; set; }
    [Column("USER_ID_")] public string? UserId { get; set; }
    [Column("MESSAGE_")] public string Message { get; set; }
    [Column("TYPE_")] public string? Type { get; set; }
    [Column("TARGET_")] public string? Target { get; set; }
    [Column("ENABLE_")] public bool? Enable { get; set; }
    [Column("START_TIME_")] public DateTime? StartTime { get; set; }
    [Column("END_TIME_")] public DateTime? EndTime { get; set; }
    [Column("CHANGE_TIME_")] public DateTime ChangeTime { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("DETAILS_")] public string? Details { get; set; }
}
[Table("ACT_RE_NOTIFICATION_STATUS", Schema="dbo")] public sealed class SqlActReNotificationStatus : SqlServerEntity {
    [Column("NOTIFICATION_ID_")] [Key] public string NotificationId { get; set; }
    [Column("USER_ID_")] public string UserId { get; set; }
    [Column("READ_")] public bool? Read { get; set; }
    [Column("DELETED_")] public bool? Deleted { get; set; }
}
[Table("ACT_RE_PROCDEF", Schema="dbo")] public sealed class SqlActReProcdef : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int? Rev { get; set; }
    [Column("CATEGORY_")] public string? Category { get; set; }
    [Column("NAME_")] public string? Name { get; set; }
    [Column("KEY_")] public string Key { get; set; }
    [Column("VERSION_")] public int Version { get; set; }
    [Column("DEPLOYMENT_ID_")] public string? DeploymentId { get; set; }
    [Column("RESOURCE_NAME_")] public string? ResourceName { get; set; }
    [Column("DGRM_RESOURCE_NAME_")] public string? DgrmResourceName { get; set; }
    [Column("HAS_START_FORM_KEY_")] public byte? HasStartFormKey { get; set; }
    [Column("SUSPENSION_STATE_")] public byte? SuspensionState { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("VERSION_TAG_")] public string? VersionTag { get; set; }
    [Column("HISTORY_TTL_")] public int? HistoryTtl { get; set; }
    [Column("STARTABLE_")] public bool Startable { get; set; }
}
[Table("ACT_RE_QUERY", Schema="dbo")] public sealed class SqlActReQuery : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int Rev { get; set; }
    [Column("NAME_")] public string Name { get; set; }
    [Column("CATEGORY_")] public string? Category { get; set; }
    [Column("TYPE_")] public int Type { get; set; }
    [Column("DEFINITION_")] public string? Definition { get; set; }
    [Column("CREATE_TIME_")] public DateTime CreateTime { get; set; }
    [Column("UPDATE_TIME_")] public DateTime? UpdateTime { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("EXPORT_TYPE_")] public int? ExportType { get; set; }
    [Column("EXPORT_DETAIL_")] public string? ExportDetail { get; set; }
    [Column("EXPORT_LIMIT_")] public int? ExportLimit { get; set; }
}
[Table("ACT_RE_REPORT", Schema="dbo")] public sealed class SqlActReReport : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int Rev { get; set; }
    [Column("NAME_")] public string Name { get; set; }
    [Column("FOLDER_")] public string? Folder { get; set; }
    [Column("DEFINITION_")] public string? Definition { get; set; }
    [Column("CREATE_TIME_")] public DateTime CreateTime { get; set; }
    [Column("UPDATE_TIME_")] public DateTime? UpdateTime { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("ACT_RE_TABLE", Schema="dbo")] public sealed class SqlActReTable : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int Rev { get; set; }
    [Column("NAME_")] public string Name { get; set; }
    [Column("CATEGORY_")] public string? Category { get; set; }
    [Column("SYNC_")] public bool Sync { get; set; }
    [Column("DEFAULT_")] public bool Default { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("ACT_RE_TOPIC", Schema="dbo")] public sealed class SqlActReTopic : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int Rev { get; set; }
    [Column("NAME_")] public string Name { get; set; }
    [Column("TYPE_")] public int Type { get; set; }
    [Column("PACKAGES_CLASS_")] public string? PackagesClass { get; set; }
    [Column("DEFAULT_")] public bool Default { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("ACT_RU_AUTHORIZATION", Schema="dbo")] public sealed class SqlActRuAuthorization : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int? Rev { get; set; }
    [Column("TYPE_")] public int Type { get; set; }
    [Column("GROUP_ID_")] public string? GroupId { get; set; }
    [Column("USER_ID_")] public string? UserId { get; set; }
    [Column("RESOURCE_TYPE_")] public int ResourceType { get; set; }
    [Column("RESOURCE_ID_")] public string? ResourceId { get; set; }
    [Column("PERMS_")] public int? Perms { get; set; }
    [Column("REMOVAL_TIME_")] public DateTime? RemovalTime { get; set; }
    [Column("ROOT_PROC_INST_ID_")] public string? RootProcInstId { get; set; }
    [Column("HASH_")] public string? Hash { get; set; }
}
[Table("ACT_RU_BATCH", Schema="dbo")] public sealed class SqlActRuBatch : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int Rev { get; set; }
    [Column("TYPE_")] public string? Type { get; set; }
    [Column("TOTAL_JOBS_")] public int? TotalJobs { get; set; }
    [Column("JOBS_CREATED_")] public int? JobsCreated { get; set; }
    [Column("JOBS_PER_SEED_")] public int? JobsPerSeed { get; set; }
    [Column("INVOCATIONS_PER_JOB_")] public int? InvocationsPerJob { get; set; }
    [Column("SEED_JOB_DEF_ID_")] public string? SeedJobDefId { get; set; }
    [Column("BATCH_JOB_DEF_ID_")] public string? BatchJobDefId { get; set; }
    [Column("MONITOR_JOB_DEF_ID_")] public string? MonitorJobDefId { get; set; }
    [Column("SUSPENSION_STATE_")] public byte? SuspensionState { get; set; }
    [Column("CONFIGURATION_")] public string? Configuration { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("CREATE_USER_ID_")] public string? CreateUserId { get; set; }
    [Column("START_TIME_")] public DateTime? StartTime { get; set; }
}
[Table("ACT_RU_CASE_EXECUTION", Schema="dbo")] public sealed class SqlActRuCaseExecution : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int? Rev { get; set; }
    [Column("CASE_INST_ID_")] public string? CaseInstId { get; set; }
    [Column("SUPER_CASE_EXEC_")] public string? SuperCaseExec { get; set; }
    [Column("SUPER_EXEC_")] public string? SuperExec { get; set; }
    [Column("BUSINESS_KEY_")] public string? BusinessKey { get; set; }
    [Column("PARENT_ID_")] public string? ParentId { get; set; }
    [Column("CASE_DEF_ID_")] public string? CaseDefId { get; set; }
    [Column("ACT_ID_")] public string? ActId { get; set; }
    [Column("PREV_STATE_")] public int? PrevState { get; set; }
    [Column("CURRENT_STATE_")] public int? CurrentState { get; set; }
    [Column("REQUIRED_")] public byte? Required { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("ACT_RU_CASE_SENTRY_PART", Schema="dbo")] public sealed class SqlActRuCaseSentryPart : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int? Rev { get; set; }
    [Column("CASE_INST_ID_")] public string? CaseInstId { get; set; }
    [Column("CASE_EXEC_ID_")] public string? CaseExecId { get; set; }
    [Column("SENTRY_ID_")] public string? SentryId { get; set; }
    [Column("TYPE_")] public string? Type { get; set; }
    [Column("SOURCE_CASE_EXEC_ID_")] public string? SourceCaseExecId { get; set; }
    [Column("STANDARD_EVENT_")] public string? StandardEvent { get; set; }
    [Column("SOURCE_")] public string? Source { get; set; }
    [Column("VARIABLE_EVENT_")] public string? VariableEvent { get; set; }
    [Column("VARIABLE_NAME_")] public string? VariableName { get; set; }
    [Column("SATISFIED_")] public byte? Satisfied { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("ACT_RU_CHANGE_QUEUE", Schema="dbo")] public sealed class SqlActRuChangeQueue : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("TIME_")] public DateTime Time { get; set; }
    [Column("TYPE_")] public string Type { get; set; }
    [Column("ACTION_")] public string Action { get; set; }
    [Column("TARGET_ID_")] public string TargetId { get; set; }
}
[Table("ACT_RU_EVENT_SUBSCR", Schema="dbo")] public sealed class SqlActRuEventSubscr : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int? Rev { get; set; }
    [Column("EVENT_TYPE_")] public string EventType { get; set; }
    [Column("EVENT_NAME_")] public string? EventName { get; set; }
    [Column("EXECUTION_ID_")] public string? ExecutionId { get; set; }
    [Column("PROC_INST_ID_")] public string? ProcInstId { get; set; }
    [Column("ACTIVITY_ID_")] public string? ActivityId { get; set; }
    [Column("CONFIGURATION_")] public string? Configuration { get; set; }
    [Column("CREATED_")] public DateTime Created { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("ACT_RU_EXECUTION", Schema="dbo")] public sealed class SqlActRuExecution : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int? Rev { get; set; }
    [Column("ROOT_PROC_INST_ID_")] public string? RootProcInstId { get; set; }
    [Column("PROC_INST_ID_")] public string? ProcInstId { get; set; }
    [Column("BUSINESS_KEY_")] public string? BusinessKey { get; set; }
    [Column("PARENT_ID_")] public string? ParentId { get; set; }
    [Column("PROC_DEF_ID_")] public string? ProcDefId { get; set; }
    [Column("SUPER_EXEC_")] public string? SuperExec { get; set; }
    [Column("SUPER_CASE_EXEC_")] public string? SuperCaseExec { get; set; }
    [Column("CASE_INST_ID_")] public string? CaseInstId { get; set; }
    [Column("ACT_ID_")] public string? ActId { get; set; }
    [Column("ACT_INST_ID_")] public string? ActInstId { get; set; }
    [Column("IS_ACTIVE_")] public byte? IsActive { get; set; }
    [Column("IS_CONCURRENT_")] public byte? IsConcurrent { get; set; }
    [Column("IS_SCOPE_")] public byte? IsScope { get; set; }
    [Column("IS_EVENT_SCOPE_")] public byte? IsEventScope { get; set; }
    [Column("SUSPENSION_STATE_")] public byte? SuspensionState { get; set; }
    [Column("CACHED_ENT_STATE_")] public int? CachedEntState { get; set; }
    [Column("SEQUENCE_COUNTER_")] public decimal? SequenceCounter { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("PROC_DEF_KEY_")] public string? ProcDefKey { get; set; }
}
[Table("ACT_RU_EXT_TASK", Schema="dbo")] public sealed class SqlActRuExtTask : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int Rev { get; set; }
    [Column("WORKER_ID_")] public string? WorkerId { get; set; }
    [Column("TOPIC_NAME_")] public string? TopicName { get; set; }
    [Column("RETRIES_")] public int? Retries { get; set; }
    [Column("ERROR_MSG_")] public string? ErrorMsg { get; set; }
    [Column("ERROR_DETAILS_ID_")] public string? ErrorDetailsId { get; set; }
    [Column("LOCK_EXP_TIME_")] public DateTime? LockExpTime { get; set; }
    [Column("SUSPENSION_STATE_")] public byte? SuspensionState { get; set; }
    [Column("EXECUTION_ID_")] public string? ExecutionId { get; set; }
    [Column("PROC_INST_ID_")] public string? ProcInstId { get; set; }
    [Column("PROC_DEF_ID_")] public string? ProcDefId { get; set; }
    [Column("PROC_DEF_KEY_")] public string? ProcDefKey { get; set; }
    [Column("ACT_ID_")] public string? ActId { get; set; }
    [Column("ACT_INST_ID_")] public string? ActInstId { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("PRIORITY_")] public decimal Priority { get; set; }
    [Column("LAST_FAILURE_LOG_ID_")] public string? LastFailureLogId { get; set; }
    [Column("CREATE_TIME_")] public DateTime? CreateTime { get; set; }
}
[Table("ACT_RU_FILTER", Schema="dbo")] public sealed class SqlActRuFilter : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int Rev { get; set; }
    [Column("RESOURCE_TYPE_")] public string ResourceType { get; set; }
    [Column("NAME_")] public string Name { get; set; }
    [Column("OWNER_")] public string? Owner { get; set; }
    [Column("QUERY_")] public string Query { get; set; }
    [Column("PROPERTIES_")] public string? Properties { get; set; }
}
[Table("ACT_RU_IDENTITYLINK", Schema="dbo")] public sealed class SqlActRuIdentitylink : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int? Rev { get; set; }
    [Column("GROUP_ID_")] public string? GroupId { get; set; }
    [Column("TYPE_")] public string? Type { get; set; }
    [Column("USER_ID_")] public string? UserId { get; set; }
    [Column("TASK_ID_")] public string? TaskId { get; set; }
    [Column("PROC_DEF_ID_")] public string? ProcDefId { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("ACT_RU_INCIDENT", Schema="dbo")] public sealed class SqlActRuIncident : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int Rev { get; set; }
    [Column("INCIDENT_TIMESTAMP_")] public DateTime IncidentTimestamp { get; set; }
    [Column("INCIDENT_MSG_")] public string? IncidentMsg { get; set; }
    [Column("INCIDENT_TYPE_")] public string IncidentType { get; set; }
    [Column("EXECUTION_ID_")] public string? ExecutionId { get; set; }
    [Column("ACTIVITY_ID_")] public string? ActivityId { get; set; }
    [Column("FAILED_ACTIVITY_ID_")] public string? FailedActivityId { get; set; }
    [Column("PROC_INST_ID_")] public string? ProcInstId { get; set; }
    [Column("PROC_DEF_ID_")] public string? ProcDefId { get; set; }
    [Column("CAUSE_INCIDENT_ID_")] public string? CauseIncidentId { get; set; }
    [Column("ROOT_CAUSE_INCIDENT_ID_")] public string? RootCauseIncidentId { get; set; }
    [Column("CONFIGURATION_")] public string? Configuration { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("JOB_DEF_ID_")] public string? JobDefId { get; set; }
    [Column("ANNOTATION_")] public string? Annotation { get; set; }
}
[Table("ACT_RU_JOB", Schema="dbo")] public sealed class SqlActRuJob : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int? Rev { get; set; }
    [Column("TYPE_")] public string Type { get; set; }
    [Column("LOCK_EXP_TIME_")] public DateTime? LockExpTime { get; set; }
    [Column("LOCK_OWNER_")] public string? LockOwner { get; set; }
    [Column("EXCLUSIVE_")] public bool? Exclusive { get; set; }
    [Column("EXECUTION_ID_")] public string? ExecutionId { get; set; }
    [Column("PROCESS_INSTANCE_ID_")] public string? ProcessInstanceId { get; set; }
    [Column("PROCESS_DEF_ID_")] public string? ProcessDefId { get; set; }
    [Column("PROCESS_DEF_KEY_")] public string? ProcessDefKey { get; set; }
    [Column("RETRIES_")] public int? Retries { get; set; }
    [Column("EXCEPTION_STACK_ID_")] public string? ExceptionStackId { get; set; }
    [Column("EXCEPTION_MSG_")] public string? ExceptionMsg { get; set; }
    [Column("FAILED_ACT_ID_")] public string? FailedActId { get; set; }
    [Column("DUEDATE_")] public DateTime? Duedate { get; set; }
    [Column("REPEAT_")] public string? Repeat { get; set; }
    [Column("REPEAT_OFFSET_")] public decimal? RepeatOffset { get; set; }
    [Column("HANDLER_TYPE_")] public string? HandlerType { get; set; }
    [Column("HANDLER_CFG_")] public string? HandlerCfg { get; set; }
    [Column("DEPLOYMENT_ID_")] public string? DeploymentId { get; set; }
    [Column("SUSPENSION_STATE_")] public byte SuspensionState { get; set; }
    [Column("PRIORITY_")] public decimal Priority { get; set; }
    [Column("JOB_DEF_ID_")] public string? JobDefId { get; set; }
    [Column("SEQUENCE_COUNTER_")] public decimal? SequenceCounter { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("CREATE_TIME_")] public DateTime? CreateTime { get; set; }
    [Column("LAST_FAILURE_LOG_ID_")] public string? LastFailureLogId { get; set; }
    [Column("ROOT_PROC_INST_ID_")] public string? RootProcInstId { get; set; }
    [Column("USERNAME_")] public string? Username { get; set; }
    [Column("BATCH_ID_")] public string? BatchId { get; set; }
}
[Table("ACT_RU_JOBDEF", Schema="dbo")] public sealed class SqlActRuJobdef : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int? Rev { get; set; }
    [Column("PROC_DEF_ID_")] public string? ProcDefId { get; set; }
    [Column("PROC_DEF_KEY_")] public string? ProcDefKey { get; set; }
    [Column("ACT_ID_")] public string? ActId { get; set; }
    [Column("JOB_TYPE_")] public string JobType { get; set; }
    [Column("JOB_CONFIGURATION_")] public string? JobConfiguration { get; set; }
    [Column("SUSPENSION_STATE_")] public byte? SuspensionState { get; set; }
    [Column("JOB_PRIORITY_")] public decimal? JobPriority { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("DEPLOYMENT_ID_")] public string? DeploymentId { get; set; }
}
[Table("ACT_RU_METER_LOG", Schema="dbo")] public sealed class SqlActRuMeterLog : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("NAME_")] public string Name { get; set; }
    [Column("REPORTER_")] public string? Reporter { get; set; }
    [Column("VALUE_")] public decimal? Value { get; set; }
    [Column("TIMESTAMP_")] public DateTime? Timestamp { get; set; }
    [Column("MILLISECONDS_")] public decimal? Milliseconds { get; set; }
}
[Table("ACT_RU_TASK", Schema="dbo")] public sealed class SqlActRuTask : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int? Rev { get; set; }
    [Column("EXECUTION_ID_")] public string? ExecutionId { get; set; }
    [Column("PROC_INST_ID_")] public string? ProcInstId { get; set; }
    [Column("PROC_DEF_ID_")] public string? ProcDefId { get; set; }
    [Column("CASE_EXECUTION_ID_")] public string? CaseExecutionId { get; set; }
    [Column("CASE_INST_ID_")] public string? CaseInstId { get; set; }
    [Column("CASE_DEF_ID_")] public string? CaseDefId { get; set; }
    [Column("NAME_")] public string? Name { get; set; }
    [Column("PARENT_TASK_ID_")] public string? ParentTaskId { get; set; }
    [Column("DESCRIPTION_")] public string? Description { get; set; }
    [Column("TASK_DEF_KEY_")] public string? TaskDefKey { get; set; }
    [Column("OWNER_")] public string? Owner { get; set; }
    [Column("ASSIGNEE_")] public string? Assignee { get; set; }
    [Column("DELEGATION_")] public string? Delegation { get; set; }
    [Column("PRIORITY_")] public int? Priority { get; set; }
    [Column("CREATE_TIME_")] public DateTime? CreateTime { get; set; }
    [Column("DUE_DATE_")] public DateTime? DueDate { get; set; }
    [Column("FOLLOW_UP_DATE_")] public DateTime? FollowUpDate { get; set; }
    [Column("SUSPENSION_STATE_")] public int? SuspensionState { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("LAST_UPDATED_")] public DateTime? LastUpdated { get; set; }
    [Column("TASK_STATE_")] public string? TaskState { get; set; }
}
[Table("ACT_RU_TASK_METER_LOG", Schema="dbo")] public sealed class SqlActRuTaskMeterLog : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("ASSIGNEE_HASH_")] public decimal? AssigneeHash { get; set; }
    [Column("TIMESTAMP_")] public DateTime? Timestamp { get; set; }
}
[Table("ACT_RU_VARIABLE", Schema="dbo")] public sealed class SqlActRuVariable : SqlServerEntity {
    [Column("ID_")] [Key] public string new Id { get; set; }
    [Column("REV_")] public int? Rev { get; set; }
    [Column("TYPE_")] public string Type { get; set; }
    [Column("NAME_")] public string Name { get; set; }
    [Column("EXECUTION_ID_")] public string? ExecutionId { get; set; }
    [Column("PROC_INST_ID_")] public string? ProcInstId { get; set; }
    [Column("PROC_DEF_ID_")] public string? ProcDefId { get; set; }
    [Column("CASE_EXECUTION_ID_")] public string? CaseExecutionId { get; set; }
    [Column("CASE_INST_ID_")] public string? CaseInstId { get; set; }
    [Column("TASK_ID_")] public string? TaskId { get; set; }
    [Column("BATCH_ID_")] public string? BatchId { get; set; }
    [Column("BYTEARRAY_ID_")] public string? BytearrayId { get; set; }
    [Column("DOUBLE_")] public double? Double { get; set; }
    [Column("LONG_")] public decimal? Long { get; set; }
    [Column("TEXT_")] public string? Text { get; set; }
    [Column("TEXT2_")] public string? Text2 { get; set; }
    [Column("VAR_SCOPE_")] public string VarScope { get; set; }
    [Column("SEQUENCE_COUNTER_")] public decimal? SequenceCounter { get; set; }
    [Column("IS_CONCURRENT_LOCAL_")] public byte? IsConcurrentLocal { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("ExternalIntegrationConnections", Schema="dbo")] public sealed class SqlExternalintegrationconnections : SqlServerEntity {
    [Column("Id")] [Key] public long new Id { get; set; }
    [Column("ShopId")] public int Shopid { get; set; }
    [Column("TenantId")] public string Tenantid { get; set; }
    [Column("Provider")] public byte Provider { get; set; }
    [Column("DisplayName")] public string Displayname { get; set; }
    [Column("AccountIdentifier")] public string Accountidentifier { get; set; }
    [Column("CredentialType")] public byte Credentialtype { get; set; }
    [Column("CredentialsJson")] public string Credentialsjson { get; set; }
    [Column("ExpiresAtUtc")] public DateTime? Expiresatutc { get; set; }
    [Column("IsEnabled")] public bool Isenabled { get; set; }
    [Column("ConnectedAtUtc")] public DateTime Connectedatutc { get; set; }
    [Column("LastSyncAtUtc")] public DateTime? Lastsyncatutc { get; set; }
    [Column("LastError")] public string? Lasterror { get; set; }
}
[Table("ExternalProductMappings", Schema="dbo")] public sealed class SqlExternalproductmappings : SqlServerEntity {
    [Column("Id")] [Key] public long new Id { get; set; }
    [Column("ConnectionId")] public long Connectionid { get; set; }
    [Column("ShopId")] public int Shopid { get; set; }
    [Column("HyperProductId")] public int Hyperproductid { get; set; }
    [Column("ExternalProductId")] public string Externalproductid { get; set; }
    [Column("ExternalSku")] public string? Externalsku { get; set; }
    [Column("ExternalVariantId")] public string? Externalvariantid { get; set; }
    [Column("LastExternalPrice")] public decimal? Lastexternalprice { get; set; }
    [Column("LastExternalInventory")] public decimal? Lastexternalinventory { get; set; }
    [Column("LastSyncAtUtc")] public DateTime? Lastsyncatutc { get; set; }
    [Column("IsActive")] public bool Isactive { get; set; }
}
[Table("IntegrationSyncRuns", Schema="dbo")] public sealed class SqlIntegrationsyncruns : SqlServerEntity {
    [Column("Id")] [Key] public long new Id { get; set; }
    [Column("ConnectionId")] public long Connectionid { get; set; }
    [Column("StartedAtUtc")] public DateTime Startedatutc { get; set; }
    [Column("FinishedAtUtc")] public DateTime? Finishedatutc { get; set; }
    [Column("Status")] public byte Status { get; set; }
    [Column("ItemsRead")] public int Itemsread { get; set; }
    [Column("ItemsWritten")] public int Itemswritten { get; set; }
    [Column("ItemsFailed")] public int Itemsfailed { get; set; }
    [Column("Error")] public string? Error { get; set; }
}
[Table("TBL_Account", Schema="dbo")] public sealed class SqlTblAccount : SqlServerEntity {
    [Column("ACCOUNTID_")] [Key] public int Accountid { get; set; }
    [Column("SHOPID_")] public int? Shopid { get; set; }
    [Column("TYPE_")] public bool? Type { get; set; }
    [Column("DETAILTYPE_")] public byte? Detailtype { get; set; }
    [Column("NATURE_")] public bool? Nature { get; set; }
    [Column("PARENTID_")] public int? Parentid { get; set; }
    [Column("CODE_")] public string? Code { get; set; }
    [Column("NAME_")] public string Name { get; set; }
    [Column("LEVEL_")] public byte Level { get; set; }
    [Column("REFERENCEID_")] public int? Referenceid { get; set; }
    [Column("ISPOSTABLE_")] public bool Ispostable { get; set; }
    [Column("GLOBALACCOUNTKEY_")] public short? Globalaccountkey { get; set; }
    [Column("DESCRIPTION_")] public string? Description { get; set; }
    [Column("FISCALPERIODID_")] public int? Fiscalperiodid { get; set; }
    [Column("REPORTTYPE_")] public byte? Reporttype { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_AccountingArticle", Schema="dbo")] public sealed class SqlTblAccountingarticle : SqlServerEntity {
    [Column("ID_")] [Key] public long new Id { get; set; }
    [Column("DOCUMENTID_")] public long Documentid { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
    [Column("DETAILACCOUNTID_")] public long? Detailaccountid { get; set; }
    [Column("DESCRIPTION_")] public string? Description { get; set; }
    [Column("ACCOUNTID_")] public int Accountid { get; set; }
    [Column("AMOUNT_")] public decimal Amount { get; set; }
    [Column("CURRENCY_")] public string Currency { get; set; }
    [Column("CHECKID_")] public int? Checkid { get; set; }
    [Column("EXCHANGERATE_")] public decimal Exchangerate { get; set; }
    [Column("TYPE_")] public bool Type { get; set; }
    [Column("BASEAMOUNT_")] public decimal Baseamount { get; set; }
}
[Table("TBL_AccountingDocument", Schema="dbo")] public sealed class SqlTblAccountingdocument : SqlServerEntity {
    [Column("ID_")] [Key] public long new Id { get; set; }
    [Column("DESCRIPTION_")] public string? Description { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
    [Column("FISCALPERIODID_")] public int Fiscalperiodid { get; set; }
    [Column("TOTALBASEAMOUNT_")] public decimal Totalbaseamount { get; set; }
    [Column("REFERENCENUMBER_")] public int? Referencenumber { get; set; }
    [Column("STATUS_")] public byte Status { get; set; }
    [Column("NUMBER_")] public int? Number { get; set; }
    [Column("DATE_")] public DateOnly Date { get; set; }
    [Column("PRIMARYTYPE_")] public byte Primarytype { get; set; }
    [Column("REFERENCETYPE_")] public byte Referencetype { get; set; }
    [Column("REFERENCEID_")] public long? Referenceid { get; set; }
    [Column("DATETIME_")] public DateTime Datetime { get; set; }
    [Column("MONTH_")] public int Month { get; set; }
    [Column("PROJECTID_")] public int? Projectid { get; set; }
    [Column("SECONDARYTYPE_")] public byte Secondarytype { get; set; }
}
[Table("TBL_AutoProcessLog", Schema="dbo")] public sealed class SqlTblAutoprocesslog : SqlServerEntity {
    [Column("LOGID_")] [Key] public long Logid { get; set; }
    [Column("PROCESSNAME_")] public string? Processname { get; set; }
    [Column("PROCESSKEY_")] public string? Processkey { get; set; }
    [Column("PROCESSVERSION_")] public short? Processversion { get; set; }
    [Column("TRIGGERCONFIG_")] public string? Triggerconfig { get; set; }
    [Column("ENDTIME_")] public DateTime? Endtime { get; set; }
    [Column("STARTTIME_")] public DateTime? Starttime { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_BankAccount", Schema="dbo")] public sealed class SqlTblBankaccount : SqlServerEntity {
    [Column("ID_")] [Key] public int new Id { get; set; }
    [Column("NAME_")] public string Name { get; set; }
    [Column("NUMBER_")] public string Number { get; set; }
    [Column("IBAN_")] public string? Iban { get; set; }
    [Column("BANKBRANCHNAME_")] public string? Bankbranchname { get; set; }
    [Column("OWNERNAME_")] public string? Ownername { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
    [Column("CARDNUMBER_")] public string? Cardnumber { get; set; }
    [Column("ISENABLED_")] public bool Isenabled { get; set; }
    [Column("ISDEFAULT_")] public bool Isdefault { get; set; }
    [Column("DESCRIPTION_")] public string? Description { get; set; }
    [Column("CURRENCY_")] public string Currency { get; set; }
    [Column("BANK_")] public string Bank { get; set; }
    [Column("DETAILACCOUNTID_")] public long Detailaccountid { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_CashFund", Schema="dbo")] public sealed class SqlTblCashfund : SqlServerEntity {
    [Column("ID_")] [Key] public int new Id { get; set; }
    [Column("NAME_")] public string Name { get; set; }
    [Column("DESCRIPTION_")] public string? Description { get; set; }
    [Column("ISENABLED_")] public bool Isenabled { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
    [Column("ISDEFAULT_")] public bool Isdefault { get; set; }
    [Column("CURRENCY_")] public string Currency { get; set; }
    [Column("DETAILACCOUNTID_")] public long Detailaccountid { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_Check", Schema="dbo")] public sealed class SqlTblCheck : SqlServerEntity {
    [Column("CHECKID_")] [Key] public int Checkid { get; set; }
    [Column("SERIALNUMBER_")] public string Serialnumber { get; set; }
    [Column("DUEDATE_")] public DateOnly Duedate { get; set; }
    [Column("AMOUNT_")] public decimal Amount { get; set; }
    [Column("STATUS_")] public byte Status { get; set; }
    [Column("STATUSDATETIME_")] public DateTime Statusdatetime { get; set; }
    [Column("SAYADNUMBER_")] public string? Sayadnumber { get; set; }
    [Column("DESCRIPTION_")] public string? Description { get; set; }
    [Column("PAYEE_")] public string? Payee { get; set; }
    [Column("PAYEENATIONALID_")] public string? Payeenationalid { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
    [Column("BANKACCOUNTID_")] public int? Bankaccountid { get; set; }
    [Column("ISSUEDATE_")] public DateTime Issuedate { get; set; }
    [Column("BANKBRANCH_")] public string? Bankbranch { get; set; }
    [Column("BANK_")] public string? Bank { get; set; }
    [Column("PERSONID_")] public int Personid { get; set; }
    [Column("ISRECEIPT_")] public bool Isreceipt { get; set; }
    [Column("CURRENCY_")] public string Currency { get; set; }
    [Column("EXCHANGERATE_")] public decimal Exchangerate { get; set; }
    [Column("PROJECTID_")] public int? Projectid { get; set; }
    [Column("FISCALPERIODID_")] public int Fiscalperiodid { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_CreditTransaction", Schema="dbo")] public sealed class SqlTblCredittransaction : SqlServerEntity {
    [Column("ID_")] [Key] public int new Id { get; set; }
    [Column("USERID_")] public string Userid { get; set; }
    [Column("AMOUNT_")] public decimal Amount { get; set; }
    [Column("TYPE_")] public byte Type { get; set; }
    [Column("REFERENCEID_")] public int Referenceid { get; set; }
    [Column("DESCRIPTION_")] public string? Description { get; set; }
    [Column("DATETIME_")] public DateTime Datetime { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_DetailAccount", Schema="dbo")] public sealed class SqlTblDetailaccount : SqlServerEntity {
    [Column("DETAILACCOUNTID_")] [Key] public long Detailaccountid { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
    [Column("ENTITYTYPE_")] public string Entitytype { get; set; }
    [Column("NAME_")] public string Name { get; set; }
    [Column("REFERENCEID_")] public int? Referenceid { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_EntityFile", Schema="dbo")] public sealed class SqlTblEntityfile : SqlServerEntity {
    [Column("ID_")] [Key] public int new Id { get; set; }
    [Column("SHOPID_")] public int? Shopid { get; set; }
    [Column("ENTITYTYPE_")] public byte Entitytype { get; set; }
    [Column("ENTITYID_")] public int Entityid { get; set; }
    [Column("ORIGINALNAME_")] public string Originalname { get; set; }
    [Column("UNIQUENAME_")] public string Uniquename { get; set; }
    [Column("SIZE_")] public long Size { get; set; }
    [Column("RELATIVEURL_")] public string Relativeurl { get; set; }
    [Column("TYPE_")] public string Type { get; set; }
    [Column("CREATEDAT_")] public DateTime Createdat { get; set; }
    [Column("DESCRIPTION_")] public string? Description { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_EntityNote", Schema="dbo")] public sealed class SqlTblEntitynote : SqlServerEntity {
    [Column("ID_")] [Key] public long new Id { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
    [Column("ENTITYID_")] public long Entityid { get; set; }
    [Column("ENTITYTYPE_")] public byte Entitytype { get; set; }
    [Column("NOTE_")] public string Note { get; set; }
    [Column("ISPUBLIC_")] public bool Ispublic { get; set; }
    [Column("CREATEDBY_")] public string Createdby { get; set; }
    [Column("CREATEDTIME_")] public DateTime Createdtime { get; set; }
    [Column("UPDATEDTIME_")] public DateTime? Updatedtime { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_GeneralConfig", Schema="dbo")] public sealed class SqlTblGeneralconfig : SqlServerEntity {
    [Column("CONFIGID_")] [Key] public int Configid { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
    [Column("BASECURRENCY_")] public string Basecurrency { get; set; }
    [Column("TIMEZONE_")] public string Timezone { get; set; }
    [Column("CALENDARTYPE_")] public string Calendartype { get; set; }
    [Column("DEFAULTVATRATE_")] public decimal? Defaultvatrate { get; set; }
    [Column("ISMULTICURRENCYENABLED_")] public bool Ismulticurrencyenabled { get; set; }
    [Column("OTHERCURRENCIES_")] public string? Othercurrencies { get; set; }
    [Column("ALLOWEDITOPENINGBALANCE_")] public bool Alloweditopeningbalance { get; set; }
    [Column("AUTOAPPROVESYSTEMGENERATEDDOCUMENTS_")] public bool Autoapprovesystemgenerateddocuments { get; set; }
    [Column("ISWAREHOUSEENABLED_")] public bool Iswarehouseenabled { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("ALLOWNEGATIVEPHYSICALSTOCK_")] public bool? Allownegativephysicalstock { get; set; }
    [Column("TRACKINGMETHOD_")] public string? Trackingmethod { get; set; }
    [Column("VALUATIONMETHOD_")] public string? Valuationmethod { get; set; }
    [Column("STORAGEEXPIRATIONDATE_")] public DateOnly? Storageexpirationdate { get; set; }
    [Column("STORAGESIZE_")] public int Storagesize { get; set; }
}
[Table("TBL_GlobalConfig", Schema="dbo")] public sealed class SqlTblGlobalconfig : SqlServerEntity {
    [Column("KEY_")] public string Key { get; set; }
    [Column("VALUE_")] public string Value { get; set; }
    [Column("TENANT_ID_")] [Key] public string? TenantId { get; set; }
}
[Table("TBL_GlobalProduct", Schema="dbo")] public sealed class SqlTblGlobalproduct : SqlServerEntity {
    [Column("GLOBALPRODUCTID_")] [Key] public int Globalproductid { get; set; }
    [Column("NAME_")] public string Name { get; set; }
    [Column("BARCODE_")] public string? Barcode { get; set; }
    [Column("DESCRIPTION_")] public string? Description { get; set; }
    [Column("ISPUBLISHED_")] public bool Ispublished { get; set; }
    [Column("BRANDID_")] public short? Brandid { get; set; }
    [Column("UNITCODE_")] public short Unitcode { get; set; }
    [Column("ISENABLED_")] public bool Isenabled { get; set; }
    [Column("ISAPPROVED_")] public bool Isapproved { get; set; }
    [Column("REVIEWSTATUSID_")] public byte Reviewstatusid { get; set; }
    [Column("CREATETIME_")] public DateTime Createtime { get; set; }
    [Column("CREATORUSER_")] public string? Creatoruser { get; set; }
    [Column("UPDATETIME_")] public DateTime Updatetime { get; set; }
    [Column("UPDATERUSER_")] public string? Updateruser { get; set; }
    [Column("SALETAXRATE_")] public decimal Saletaxrate { get; set; }
    [Column("PURCHASETAXRATE_")] public decimal Purchasetaxrate { get; set; }
    [Column("ISSERVICE_")] public bool Isservice { get; set; }
    [Column("GROUPID_")] public short? Groupid { get; set; }
    [Column("IMAGERELATIVEURL_")] public string? Imagerelativeurl { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_GlobalProductGroup", Schema="dbo")] public sealed class SqlTblGlobalproductgroup : SqlServerEntity {
    [Column("GROUPID_")] [Key] public short Groupid { get; set; }
    [Column("PARENTID_")] public short? Parentid { get; set; }
    [Column("NAME_")] public string Name { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_GlobalProductMedia", Schema="dbo")] public sealed class SqlTblGlobalproductmedia : SqlServerEntity {
    [Column("MEDIAID_")] [Key] public int Mediaid { get; set; }
    [Column("PRODUCTID_")] public int Productid { get; set; }
    [Column("FILERELATIVEURL_")] public string? Filerelativeurl { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_Groups", Schema="dbo")] public sealed class SqlTblGroups : SqlServerEntity {
    [Column("SHOPID_")] [Key] public int Shopid { get; set; }
    [Column("GROUPID_")] public int Groupid { get; set; }
    [Column("ENTITYTYPE_")] public byte Entitytype { get; set; }
    [Column("PARENTID_")] public int? Parentid { get; set; }
    [Column("CODE_")] public int Code { get; set; }
    [Column("NAME_")] public string Name { get; set; }
    [Column("DESCRIPTION_")] public string? Description { get; set; }
    [Column("FULLPATH_")] public string? Fullpath { get; set; }
    [Column("NAMEFULLPATH_")] public string? Namefullpath { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_InventoryFifoConsumption", Schema="dbo")] public sealed class SqlTblInventoryfifoconsumption : SqlServerEntity {
    [Column("ID_")] [Key] public int new Id { get; set; }
    [Column("STOCKCARDITEMID_")] public int Stockcarditemid { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
    [Column("CONSUMEDQUANTITY_")] public decimal Consumedquantity { get; set; }
    [Column("UNITCOST_")] public decimal Unitcost { get; set; }
    [Column("CONSUMPTIONDATETIME_")] public DateTime Consumptiondatetime { get; set; }
    [Column("AMOUNT_")] public decimal? Amount { get; set; }
    [Column("PRODUCTID_")] public int Productid { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_InventoryFifoLayer", Schema="dbo")] public sealed class SqlTblInventoryfifolayer : SqlServerEntity {
    [Column("ID_")] [Key] public int new Id { get; set; }
    [Column("STOCKCARDITEMID_")] public int Stockcarditemid { get; set; }
    [Column("PRODUCTID_")] public int Productid { get; set; }
    [Column("WAREHOUSEID_")] public int Warehouseid { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
    [Column("INITIALQUANTITY_")] public decimal Initialquantity { get; set; }
    [Column("REMAININGQUANTITY_")] public decimal Remainingquantity { get; set; }
    [Column("UNITCOST_")] public decimal Unitcost { get; set; }
    [Column("RECEIPTDATETIME_")] public DateTime Receiptdatetime { get; set; }
    [Column("AMOUNT_")] public decimal? Amount { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_IranCities", Schema="dbo")] public sealed class SqlTblIrancities : SqlServerEntity {
    [Column("CITYID_")] [Key] public short Cityid { get; set; }
    [Column("STATEID_")] public byte Stateid { get; set; }
    [Column("CITYNAME_")] public string Cityname { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_IranStates", Schema="dbo")] public sealed class SqlTblIranstates : SqlServerEntity {
    [Column("STATEID_")] [Key] public byte Stateid { get; set; }
    [Column("STATENAME_")] public string Statename { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_MessageHistory", Schema="dbo")] public sealed class SqlTblMessagehistory : SqlServerEntity {
    [Column("MESSAGEID_")] [Key] public int Messageid { get; set; }
    [Column("MOBILENUMBER_")] public string? Mobilenumber { get; set; }
    [Column("SENDTIME_")] public DateTime? Sendtime { get; set; }
    [Column("MESSAGETEXT_")] public string? Messagetext { get; set; }
    [Column("ERRORCODE_")] public string? Errorcode { get; set; }
    [Column("SUBJECT_")] public string? Subject { get; set; }
    [Column("ERRORMESSAGE_")] public string? Errormessage { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("DELIVERYSTATUS_")] public string? Deliverystatus { get; set; }
}
[Table("TBL_NotificationConfig", Schema="dbo")] public sealed class SqlTblNotificationconfig : SqlServerEntity {
    [Column("CONFIGID_")] [Key] public int Configid { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
    [Column("NOTIFICATIONMETHOD_")] public string Notificationmethod { get; set; }
    [Column("NOTIFYSALEINVOICEDUEDATE_")] public bool Notifysaleinvoiceduedate { get; set; }
    [Column("SALEINVOICEDUENOTIFICATIONTHRESHOLD_")] public byte? Saleinvoiceduenotificationthreshold { get; set; }
    [Column("NOTIFYPURCHASEINVOICEDUEDATE_")] public bool Notifypurchaseinvoiceduedate { get; set; }
    [Column("PURCHASEINVOICEDUENOTIFICATIONTHRESHOLD_")] public byte? Purchaseinvoiceduenotificationthreshold { get; set; }
    [Column("NOTIFYRECEIVEDCHEQUEDUEDATE_")] public bool Notifyreceivedchequeduedate { get; set; }
    [Column("RECEIVEDCHEQUEDUENOTIFICATIONTHRESHOLD_")] public byte? Receivedchequeduenotificationthreshold { get; set; }
    [Column("NOTIFYPAIDCHEQUEDUEDATE_")] public bool Notifypaidchequeduedate { get; set; }
    [Column("PAIDCHEQUEDUENOTIFICATIONTHRESHOLD_")] public byte? Paidchequeduenotificationthreshold { get; set; }
    [Column("NOTIFYINSTALLMENTDUEDATE_")] public bool Notifyinstallmentduedate { get; set; }
    [Column("INSTALLMENTDUENOTIFICATIONTHRESHOLD_")] public byte? Installmentduenotificationthreshold { get; set; }
    [Column("NOTIFYREORDERPOINT_")] public bool Notifyreorderpoint { get; set; }
    [Column("NOTIFYTAXINVOICESUBMISSIONDUEDATE_")] public bool Notifytaxinvoicesubmissionduedate { get; set; }
    [Column("TAXINVOICESUBMISSIONDUENOTIFICATIONTHRESHOLD_")] public byte? Taxinvoicesubmissionduenotificationthreshold { get; set; }
    [Column("NOTIFYDOCUMENTLIMITREACHED_")] public bool Notifydocumentlimitreached { get; set; }
    [Column("DOCUMENTLIMITNOTIFICATIONTHRESHOLD_")] public byte? Documentlimitnotificationthreshold { get; set; }
    [Column("NOTIFYONLINEINVOICELIMITREACHED_")] public bool Notifyonlineinvoicelimitreached { get; set; }
    [Column("ONLINEINVOICENOTIFICATIONTHRESHOLD_")] public byte? Onlineinvoicenotificationthreshold { get; set; }
    [Column("NOTIFYTAXSUBMISSIONLIMITREACHED_")] public bool Notifytaxsubmissionlimitreached { get; set; }
    [Column("TAXSUBMISSIONNOTIFICATIONTHRESHOLD_")] public byte? Taxsubmissionnotificationthreshold { get; set; }
    [Column("NOTIFYSTORAGELIMITREACHED_")] public bool Notifystoragelimitreached { get; set; }
    [Column("STORAGENOTIFICATIONTHRESHOLD_")] public int? Storagenotificationthreshold { get; set; }
    [Column("NOTIFYWALLETLOWBALANCE_")] public bool Notifywalletlowbalance { get; set; }
    [Column("WALLETLOWBALANCETHRESHOLD_")] public decimal? Walletlowbalancethreshold { get; set; }
    [Column("SENDDAILYSALESREPORT_")] public bool Senddailysalesreport { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_Person", Schema="dbo")] public sealed class SqlTblPerson : SqlServerEntity {
    [Column("ID_")] [Key] public int new Id { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
    [Column("DETAILACCOUNTID_")] public long Detailaccountid { get; set; }
    [Column("NAME_")] public string? Name { get; set; }
    [Column("LASTNAME_")] public string? Lastname { get; set; }
    [Column("COMPANYNAME_")] public string? Companyname { get; set; }
    [Column("NICKNAME_")] public string Nickname { get; set; }
    [Column("TYPE_")] public byte Type { get; set; }
    [Column("ISENABLED_")] public bool Isenabled { get; set; }
    [Column("IDENTIFIERNUMBER_")] public string? Identifiernumber { get; set; }
    [Column("ECONOMICCODE_")] public string? Economiccode { get; set; }
    [Column("BRANCHCODE_")] public string? Branchcode { get; set; }
    [Column("CREDITLIMIT_")] public decimal? Creditlimit { get; set; }
    [Column("PROFILEIMAGEURL_")] public string? Profileimageurl { get; set; }
    [Column("CONTACTINFO_")] public string? Contactinfo { get; set; }
    [Column("SPECIALDATES_")] public string? Specialdates { get; set; }
    [Column("BANKACCOUNTSINFO_")] public string? Bankaccountsinfo { get; set; }
    [Column("ADDRESSESINFO_")] public string? Addressesinfo { get; set; }
    [Column("ROLES_")] public string? Roles { get; set; }
    [Column("MOBILENUMBER_")] public string? Mobilenumber { get; set; }
    [Column("PASSPORTNUMBER_")] public string? Passportnumber { get; set; }
    [Column("CONTRACTNUMBER_")] public string? Contractnumber { get; set; }
    [Column("SUBSCRIPTIONNUMBER_")] public string? Subscriptionnumber { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_PettyCash", Schema="dbo")] public sealed class SqlTblPettycash : SqlServerEntity {
    [Column("PETTYCASHID_")] [Key] public int Pettycashid { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
    [Column("DETAILACCOUNTID_")] public long? Detailaccountid { get; set; }
    [Column("PERSONID_")] public int Personid { get; set; }
    [Column("ISDEFAULT_")] public bool Isdefault { get; set; }
    [Column("ISENABLED_")] public bool Isenabled { get; set; }
    [Column("DESCRIPTION_")] public string? Description { get; set; }
    [Column("CODE_")] public int Code { get; set; }
    [Column("NAME_")] public string Name { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("CURRENCY_")] public string Currency { get; set; }
}
[Table("TBL_PosDevice", Schema="dbo")] public sealed class SqlTblPosdevice : SqlServerEntity {
    [Column("ID_")] [Key] public int new Id { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
    [Column("ISENABLED_")] public bool Isenabled { get; set; }
    [Column("BANKACCOUNTID_")] public int Bankaccountid { get; set; }
    [Column("DESCRIPTION_")] public string? Description { get; set; }
    [Column("NAME_")] public string Name { get; set; }
    [Column("IPADDRESS_")] public string Ipaddress { get; set; }
    [Column("PORTNUMBER_")] public int? Portnumber { get; set; }
    [Column("TERMINALNUMBER_")] public string? Terminalnumber { get; set; }
    [Column("MERCHANTNUMBER_")] public string? Merchantnumber { get; set; }
    [Column("SERIALNUMBER_")] public string? Serialnumber { get; set; }
    [Column("ISDEFAULT_")] public bool Isdefault { get; set; }
    [Column("PSP_")] public string Psp { get; set; }
    [Column("DETAILACCOUNTID_")] public long Detailaccountid { get; set; }
    [Column("CURRENCY_")] public string Currency { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_PrintConfig", Schema="dbo")] public sealed class SqlTblPrintconfig : SqlServerEntity {
    [Column("CONFIGID_")] [Key] public int Configid { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
    [Column("PRINTLOGO_")] public bool Printlogo { get; set; }
    [Column("INVOICETITLE_")] public string Invoicetitle { get; set; }
    [Column("PRINTSELLERINFO_")] public bool Printsellerinfo { get; set; }
    [Column("PRINTCUSTOMERINFO_")] public bool Printcustomerinfo { get; set; }
    [Column("PRINTSIGNATUREAREA_")] public bool Printsignaturearea { get; set; }
    [Column("SIGNATURETITLE1_")] public string? Signaturetitle1 { get; set; }
    [Column("SIGNATURETITLE2_")] public string? Signaturetitle2 { get; set; }
    [Column("SIGNATURETITLE3_")] public string? Signaturetitle3 { get; set; }
    [Column("SIGNATURETITLE4_")] public string? Signaturetitle4 { get; set; }
    [Column("SIGNATURETITLE5_")] public string? Signaturetitle5 { get; set; }
    [Column("PRINTDUEDATE_")] public bool Printduedate { get; set; }
    [Column("TAXPRINTMETHOD_")] public string Taxprintmethod { get; set; }
    [Column("DISCOUNTPRINTMETHOD_")] public string Discountprintmethod { get; set; }
    [Column("PRINTPAYMENTINFO_")] public bool Printpaymentinfo { get; set; }
    [Column("PRINTACCOUNTBALANCE_")] public bool Printaccountbalance { get; set; }
    [Column("PRINTCURRENTDATETIME_")] public bool Printcurrentdatetime { get; set; }
    [Column("INVOICEFOOTERTEXT_")] public string? Invoicefootertext { get; set; }
    [Column("PRINTCOPYCOUNT_")] public byte Printcopycount { get; set; }
    [Column("PRINTINVOICEITEMCOUNT_")] public bool Printinvoiceitemcount { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_Product", Schema="dbo")] public sealed class SqlTblProduct : SqlServerEntity {
    [Column("ID_")] [Key] public int new Id { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
    [Column("GLOBALID_")] public int? Globalid { get; set; }
    [Column("TAXCODE_")] public string? Taxcode { get; set; }
    [Column("NAME_")] public string Name { get; set; }
    [Column("ISENABLED_")] public bool Isenabled { get; set; }
    [Column("BASEUNIT_")] public short Baseunit { get; set; }
    [Column("SECONDARYUNIT_")] public short? Secondaryunit { get; set; }
    [Column("UNITCONVERSIONFACTOR_")] public decimal? Unitconversionfactor { get; set; }
    [Column("PURCHASEPRICE_")] public decimal Purchaseprice { get; set; }
    [Column("PURCHASETAXRATE_")] public decimal Purchasetaxrate { get; set; }
    [Column("SALEPRICE_")] public decimal Saleprice { get; set; }
    [Column("SALETAXRATE_")] public decimal Saletaxrate { get; set; }
    [Column("PERCENTDISCOUNT_")] public decimal Percentdiscount { get; set; }
    [Column("FIXEDDISCOUNT_")] public decimal Fixeddiscount { get; set; }
    [Column("ISCONSUMABLE_")] public bool? Isconsumable { get; set; }
    [Column("ISPURCHASABLE_")] public bool Ispurchasable { get; set; }
    [Column("ISSTOCKABLE_")] public bool Isstockable { get; set; }
    [Column("ISSELLABLE_")] public bool Issellable { get; set; }
    [Column("ISONLINESELLABLE_")] public bool Isonlinesellable { get; set; }
    [Column("ISSERVICE_")] public bool Isservice { get; set; }
    [Column("ISSERIALIZED_")] public bool? Isserialized { get; set; }
    [Column("DETAILACCOUNTID_")] public long Detailaccountid { get; set; }
    [Column("IMAGERELATIVEURL_")] public string? Imagerelativeurl { get; set; }
    [Column("MAXSALESQUANTITY_")] public decimal? Maxsalesquantity { get; set; }
    [Column("DESCRIPTION_")] public string? Description { get; set; }
    [Column("ACCOUNTINGSTOCK_")] public decimal Accountingstock { get; set; }
    [Column("MINIMUMSTOCK_")] public decimal? Minimumstock { get; set; }
    [Column("GROUPID_")] public int? Groupid { get; set; }
    [Column("REORDERPOINT_")] public decimal? Reorderpoint { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_ProductBarcode", Schema="dbo")] public sealed class SqlTblProductbarcode : SqlServerEntity {
    [Column("BARCODEID_")] [Key] public int Barcodeid { get; set; }
    [Column("PRODUCTID_")] public int Productid { get; set; }
    [Column("BARCODE_")] public string Barcode { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_ProductBrand", Schema="dbo")] public sealed class SqlTblProductbrand : SqlServerEntity {
    [Column("BRANDID_")] [Key] public short Brandid { get; set; }
    [Column("BRANDNAME_")] public string Brandname { get; set; }
    [Column("BRANDDESCRIPTION_")] public string? Branddescription { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("LOGORELATIVEURL_")] public string? Logorelativeurl { get; set; }
}
[Table("TBL_ProductReviewStatus", Schema="dbo")] public sealed class SqlTblProductreviewstatus : SqlServerEntity {
    [Column("PRODUCTREVIEWSTATUSID_")] [Key] public byte Productreviewstatusid { get; set; }
    [Column("PRODUCTREVIEWSTATUSNAME_")] public string Productreviewstatusname { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_ProductUnit", Schema="dbo")] public sealed class SqlTblProductunit : SqlServerEntity {
    [Column("NAME_")] public string Name { get; set; }
    [Column("DECIMALPRECISION_")] public byte Decimalprecision { get; set; }
    [Column("TENANT_ID_")] [Key] public string? TenantId { get; set; }
    [Column("UNITCODE_")] public short Unitcode { get; set; }
    [Column("USAGETYPE_")] public string Usagetype { get; set; }
    [Column("COUNTASSINGLEITEM_")] public bool Countassingleitem { get; set; }
    [Column("CONVERSIONS_")] public string? Conversions { get; set; }
}
[Table("TBL_Project", Schema="dbo")] public sealed class SqlTblProject : SqlServerEntity {
    [Column("PROJECTID_")] [Key] public int Projectid { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
    [Column("NAME_")] public string Name { get; set; }
    [Column("ISDEFAULT_")] public bool Isdefault { get; set; }
    [Column("ISENABLED_")] public bool Isenabled { get; set; }
    [Column("DESCRIPTION_")] public string? Description { get; set; }
    [Column("CODE_")] public byte Code { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_PurchaseOrder", Schema="dbo")] public sealed class SqlTblPurchaseorder : SqlServerEntity {
    [Column("PURCHASEORDERID_")] [Key] public long Purchaseorderid { get; set; }
    [Column("TOTALAMOUNTBEFOREDISCOUNT_")] public decimal Totalamountbeforediscount { get; set; }
    [Column("TOTALVATAMOUNT_")] public decimal Totalvatamount { get; set; }
    [Column("TOTALINVOICEAMOUNT_")] public decimal Totalinvoiceamount { get; set; }
    [Column("TOTALDISCOUNTAMOUNT_")] public decimal Totaldiscountamount { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
    [Column("SHIPPINGDESCRIPTION_")] public string? Shippingdescription { get; set; }
    [Column("SHIPPINGAMOUNT_")] public decimal? Shippingamount { get; set; }
    [Column("ENDORSEMENTID_")] public string? Endorsementid { get; set; }
    [Column("INSURANCEID_")] public string? Insuranceid { get; set; }
    [Column("SALESNOTICEDATE_")] public DateOnly? Salesnoticedate { get; set; }
    [Column("SALESNOTICENUMBER_")] public string? Salesnoticenumber { get; set; }
    [Column("SHIPPEDPRODUCTS_")] public string? Shippedproducts { get; set; }
    [Column("DRIVERIDENTIFICATIONNUMBER_")] public string? Driveridentificationnumber { get; set; }
    [Column("FLEETNUMBER_")] public string? Fleetnumber { get; set; }
    [Column("WAYBILLTYPE_")] public byte? Waybilltype { get; set; }
    [Column("RECEIVERIDENTIFICATIONNUMBER_")] public string? Receiveridentificationnumber { get; set; }
    [Column("SENDERIDENTIFICATIONNUMBER_")] public string? Senderidentificationnumber { get; set; }
    [Column("DESTINATIONCITY_")] public string? Destinationcity { get; set; }
    [Column("DESTINATIONCOUNTRY_")] public string? Destinationcountry { get; set; }
    [Column("ORIGINCITY_")] public string? Origincity { get; set; }
    [Column("ORIGINCOUNTRY_")] public string? Origincountry { get; set; }
    [Column("REFERENCEWAYBILLNUMBER_")] public string? Referencewaybillnumber { get; set; }
    [Column("WAYBILLNUMBER_")] public string? Waybillnumber { get; set; }
    [Column("AGENCYECONOMICCODE_")] public string? Agencyeconomiccode { get; set; }
    [Column("TOTALCURRENCYAMOUNT_")] public decimal? Totalcurrencyamount { get; set; }
    [Column("TOTALRIALAMOUNT_")] public decimal? Totalrialamount { get; set; }
    [Column("TOTALNETWEIGHT_")] public decimal? Totalnetweight { get; set; }
    [Column("SUBSCRIBERNUMBER_")] public string? Subscribernumber { get; set; }
    [Column("CUSTOMSDECLARATIONDATE_")] public DateOnly? Customsdeclarationdate { get; set; }
    [Column("CUSTOMSDECLARATIONNUMBER_")] public string? Customsdeclarationnumber { get; set; }
    [Column("FLIGHTTYPE_")] public byte? Flighttype { get; set; }
    [Column("TOTALARTICLE17TAXAMOUNT_")] public decimal? Totalarticle17taxamount { get; set; }
    [Column("TOTALVATPAIDAMOUNT_")] public decimal? Totalvatpaidamount { get; set; }
    [Column("TOTALCREDITAMOUNT_")] public decimal? Totalcreditamount { get; set; }
    [Column("TOTALCASHPAIDAMOUNT_")] public decimal? Totalcashpaidamount { get; set; }
    [Column("SETTLEMENTTYPEID_")] public byte? Settlementtypeid { get; set; }
    [Column("TAXUNIQUEID_")] public string? Taxuniqueid { get; set; }
    [Column("ADJUSTMENTSAMOUNT_")] public decimal Adjustmentsamount { get; set; }
    [Column("TOTALOTHERTAXESANDCHARGESAMOUNT_")] public decimal? Totalothertaxesandchargesamount { get; set; }
    [Column("SELLERCUSTOMSOFFICECODE_")] public string? Sellercustomsofficecode { get; set; }
    [Column("ADJUSTMENTS_")] public string? Adjustments { get; set; }
    [Column("DUEDATE_")] public DateOnly? Duedate { get; set; }
    [Column("CUSTOMSPERMITNUMBER_")] public string? Customspermitnumber { get; set; }
    [Column("DESCRIPTION_")] public string? Description { get; set; }
    [Column("INVOICEFORMAT_")] public byte Invoiceformat { get; set; }
    [Column("TAXINVOICEINTERNALSERIAL_")] public string? Taxinvoiceinternalserial { get; set; }
    [Column("TOTALAMOUNT_")] public decimal Totalamount { get; set; }
    [Column("PAIDAMOUNT_")] public decimal Paidamount { get; set; }
    [Column("INVOICETYPE_")] public byte? Invoicetype { get; set; }
    [Column("TOTALAMOUNTAFTERDISCOUNT_")] public decimal Totalamountafterdiscount { get; set; }
    [Column("PAYMENTSTATUS_")] public byte Paymentstatus { get; set; }
    [Column("INVENTORYSTATUS_")] public byte? Inventorystatus { get; set; }
    [Column("DELIVERYSTATUS_")] public byte? Deliverystatus { get; set; }
    [Column("SUPPLIERID_")] public int Supplierid { get; set; }
    [Column("STATUS_")] public byte Status { get; set; }
    [Column("ISSUEDATETIME_")] public DateTime Issuedatetime { get; set; }
    [Column("INVOICENUMBER_")] public int Invoicenumber { get; set; }
    [Column("EXCHANGERATE_")] public decimal Exchangerate { get; set; }
    [Column("CURRENCY_")] public string Currency { get; set; }
    [Column("PROJECTID_")] public int? Projectid { get; set; }
    [Column("WAREHOUSEID_")] public int? Warehouseid { get; set; }
    [Column("FISCALPERIODID_")] public int Fiscalperiodid { get; set; }
    [Column("REMAINEDAMOUNT_")] public decimal? Remainedamount { get; set; }
    [Column("ISSTOCKCARDAUTOCREATED_")] public bool Isstockcardautocreated { get; set; }
}
[Table("TBL_PurchaseOrderItem", Schema="dbo")] public sealed class SqlTblPurchaseorderitem : SqlServerEntity {
    [Column("PURCHASEORDERITEMID_")] [Key] public long Purchaseorderitemid { get; set; }
    [Column("PURCHASEORDERID_")] public long Purchaseorderid { get; set; }
    [Column("PRODUCTID_")] public int Productid { get; set; }
    [Column("QUANTITY_")] public decimal? Quantity { get; set; }
    [Column("UNIT_")] public short? Unit { get; set; }
    [Column("PRICE_")] public decimal? Price { get; set; }
    [Column("LINEPERCENTDISCOUNT_")] public decimal? Linepercentdiscount { get; set; }
    [Column("LINETOTALAMOUNT_")] public decimal Linetotalamount { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("VATRATE_")] public decimal Vatrate { get; set; }
    [Column("UNITCONVERSIONFACTOR_")] public decimal? Unitconversionfactor { get; set; }
    [Column("LINEVATPAIDAMOUNT_")] public decimal? Linevatpaidamount { get; set; }
    [Column("LINECASHPAIDAMOUNT_")] public decimal? Linecashpaidamount { get; set; }
    [Column("OTHERLEGALDUESAMOUNT_")] public decimal? Otherlegalduesamount { get; set; }
    [Column("OTHERLEGALDUESRATE_")] public decimal? Otherlegalduesrate { get; set; }
    [Column("OTHERLEGALDUESSUBJECT_")] public string? Otherlegalduessubject { get; set; }
    [Column("OTHERTAXESANDCHARGESAMOUNT_")] public decimal? Othertaxesandchargesamount { get; set; }
    [Column("OTHERTAXESANDCHARGESRATE_")] public decimal? Othertaxesandchargesrate { get; set; }
    [Column("OTHERTAXESANDCHARGESSUBJECT_")] public string? Othertaxesandchargessubject { get; set; }
    [Column("VATBASEAMOUNT_")] public decimal? Vatbaseamount { get; set; }
    [Column("VATCALCULATIONBASE_")] public decimal? Vatcalculationbase { get; set; }
    [Column("CURRENCYBUYRATE_")] public decimal? Currencybuyrate { get; set; }
    [Column("PURITY_")] public decimal? Purity { get; set; }
    [Column("TOTALMAKINGCOMMISSIONPROFIT_")] public decimal? Totalmakingcommissionprofit { get; set; }
    [Column("COMMISSION_")] public decimal? Commission { get; set; }
    [Column("SELLERPROFIT_")] public decimal? Sellerprofit { get; set; }
    [Column("MAKINGCHARGE_")] public decimal? Makingcharge { get; set; }
    [Column("LINECURRENCYVALUE_")] public decimal? Linecurrencyvalue { get; set; }
    [Column("LINERIALVALUE_")] public decimal? Linerialvalue { get; set; }
    [Column("NETWEIGHT_")] public decimal? Netweight { get; set; }
    [Column("LINEAMOUNTAFTERDISCOUNT_")] public decimal Lineamountafterdiscount { get; set; }
    [Column("COMMISSIONCONTRACTNUMBER_")] public string? Commissioncontractnumber { get; set; }
    [Column("LINEDISCOUNTAMOUNT_")] public decimal Linediscountamount { get; set; }
    [Column("LINEAMOUNTBEFOREDISCOUNT_")] public decimal Lineamountbeforediscount { get; set; }
    [Column("LINECURRENCYAMOUNT_")] public decimal? Linecurrencyamount { get; set; }
    [Column("VATAMOUNT_")] public decimal Vatamount { get; set; }
    [Column("LINEEXCHANGERATE_")] public decimal? Lineexchangerate { get; set; }
    [Column("LINECURRENCY_")] public string? Linecurrency { get; set; }
    [Column("PROJECTID_")] public int? Projectid { get; set; }
    [Column("LINEWAREHOUSEID_")] public int? Linewarehouseid { get; set; }
    [Column("FISCALPERIODID_")] public int Fiscalperiodid { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
}
[Table("TBL_PurchaseOrderReturn", Schema="dbo")] public sealed class SqlTblPurchaseorderreturn : SqlServerEntity {
    [Column("PURCHASEORDERRETURNID_")] [Key] public long Purchaseorderreturnid { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
    [Column("FISCALPERIODID_")] public int Fiscalperiodid { get; set; }
    [Column("WAREHOUSEID_")] public int? Warehouseid { get; set; }
    [Column("PROJECTID_")] public int? Projectid { get; set; }
    [Column("REFERENCEPURCHASEORDERID_")] public long Referencepurchaseorderid { get; set; }
    [Column("INVOICENUMBER_")] public int Invoicenumber { get; set; }
    [Column("ISSUEDATETIME_")] public DateTime Issuedatetime { get; set; }
    [Column("CREATIONDATETIME_")] public DateTime Creationdatetime { get; set; }
    [Column("STATUS_")] public byte Status { get; set; }
    [Column("DELIVERYSTATUS_")] public byte? Deliverystatus { get; set; }
    [Column("INVENTORYSTATUS_")] public byte? Inventorystatus { get; set; }
    [Column("PAYMENTSTATUS_")] public byte Paymentstatus { get; set; }
    [Column("TOTALRETURNAMOUNTBEFOREDISCOUNT_")] public decimal Totalreturnamountbeforediscount { get; set; }
    [Column("TOTALDISCOUNTRETURNAMOUNT_")] public decimal Totaldiscountreturnamount { get; set; }
    [Column("TOTALRETURNAMOUNTAFTERDISCOUNT_")] public decimal Totalreturnamountafterdiscount { get; set; }
    [Column("TOTALVATRETURNAMOUNT_")] public decimal Totalvatreturnamount { get; set; }
    [Column("TOTALINVOICERETURNAMOUNT_")] public decimal Totalinvoicereturnamount { get; set; }
    [Column("PAIDRETURNAMOUNT_")] public decimal Paidreturnamount { get; set; }
    [Column("TOTALRETURNAMOUNT_")] public decimal Totalreturnamount { get; set; }
    [Column("DESCRIPTION_")] public string? Description { get; set; }
    [Column("ADJUSTMENTS_")] public string? Adjustments { get; set; }
    [Column("ADJUSTMENTSAMOUNT_")] public decimal Adjustmentsamount { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_PurchaseOrderReturnItem", Schema="dbo")] public sealed class SqlTblPurchaseorderreturnitem : SqlServerEntity {
    [Column("PURCHASEORDERRETURNITEMID_")] [Key] public long Purchaseorderreturnitemid { get; set; }
    [Column("REFERENCEPURCHASEORDERITEMID_")] public long Referencepurchaseorderitemid { get; set; }
    [Column("PURCHASEORDERRETURNID_")] public long Purchaseorderreturnid { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
    [Column("FISCALPERIODID_")] public int Fiscalperiodid { get; set; }
    [Column("LINEWAREHOUSEID_")] public int? Linewarehouseid { get; set; }
    [Column("PROJECTID_")] public int? Projectid { get; set; }
    [Column("PRODUCTID_")] public int Productid { get; set; }
    [Column("RETURNQUANTITY_")] public decimal Returnquantity { get; set; }
    [Column("UNIT_")] public short Unit { get; set; }
    [Column("UNITCONVERSIONFACTOR_")] public decimal? Unitconversionfactor { get; set; }
    [Column("PRICE_")] public decimal Price { get; set; }
    [Column("LINERETURNAMOUNTBEFOREDISCOUNT_")] public decimal Linereturnamountbeforediscount { get; set; }
    [Column("LINEPERCENTDISCOUNT_")] public decimal? Linepercentdiscount { get; set; }
    [Column("LINEDISCOUNTRETURNAMOUNT_")] public decimal Linediscountreturnamount { get; set; }
    [Column("LINERETURNAMOUNTAFTERDISCOUNT_")] public decimal Linereturnamountafterdiscount { get; set; }
    [Column("VATRATE_")] public decimal Vatrate { get; set; }
    [Column("VATRETURNAMOUNT_")] public decimal Vatreturnamount { get; set; }
    [Column("LINETOTALRETURNAMOUNT_")] public decimal Linetotalreturnamount { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_ReleaseNote", Schema="dbo")] public sealed class SqlTblReleasenote : SqlServerEntity {
    [Column("RELEASEID_")] [Key] public byte Releaseid { get; set; }
    [Column("RELEASEDATE_")] public DateOnly Releasedate { get; set; }
    [Column("RELEASEVERSION_")] public string Releaseversion { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_ReleaseNoteItem", Schema="dbo")] public sealed class SqlTblReleasenoteitem : SqlServerEntity {
    [Column("RELEASEITEMID_")] [Key] public short Releaseitemid { get; set; }
    [Column("RELEASEID_")] public byte Releaseid { get; set; }
    [Column("RELEASEITEMORDER_")] public byte Releaseitemorder { get; set; }
    [Column("RELEASEITEMTITLE_")] public string Releaseitemtitle { get; set; }
    [Column("RELEASEITEMDESCRIPTION_")] public string? Releaseitemdescription { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_SaleOrder", Schema="dbo")] public sealed class SqlTblSaleorder : SqlServerEntity {
    [Column("SALEORDERID_")] [Key] public long Saleorderid { get; set; }
    [Column("TOTALAMOUNTBEFOREDISCOUNT_")] public decimal Totalamountbeforediscount { get; set; }
    [Column("TOTALVATAMOUNT_")] public decimal Totalvatamount { get; set; }
    [Column("TOTALDISCOUNTAMOUNT_")] public decimal Totaldiscountamount { get; set; }
    [Column("TOTALINVOICEAMOUNT_")] public decimal Totalinvoiceamount { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
    [Column("SHIPPINGDESCRIPTION_")] public string? Shippingdescription { get; set; }
    [Column("SHIPPINGAMOUNT_")] public decimal? Shippingamount { get; set; }
    [Column("COURIERID_")] public int? Courierid { get; set; }
    [Column("TAXPAYERPORTALINQUIRYRESPONSE_")] public string? Taxpayerportalinquiryresponse { get; set; }
    [Column("ENDORSEMENTID_")] public string? Endorsementid { get; set; }
    [Column("INSURANCEID_")] public string? Insuranceid { get; set; }
    [Column("SALESNOTICEDATE_")] public DateOnly? Salesnoticedate { get; set; }
    [Column("SALESNOTICENUMBER_")] public string? Salesnoticenumber { get; set; }
    [Column("SHIPPEDPRODUCTS_")] public string? Shippedproducts { get; set; }
    [Column("DRIVERIDENTIFICATIONNUMBER_")] public string? Driveridentificationnumber { get; set; }
    [Column("FLEETNUMBER_")] public string? Fleetnumber { get; set; }
    [Column("WAYBILLTYPE_")] public byte? Waybilltype { get; set; }
    [Column("RECEIVERIDENTIFICATIONNUMBER_")] public string? Receiveridentificationnumber { get; set; }
    [Column("SENDERIDENTIFICATIONNUMBER_")] public string? Senderidentificationnumber { get; set; }
    [Column("DESTINATIONCITY_")] public string? Destinationcity { get; set; }
    [Column("DESTINATIONCOUNTRY_")] public string? Destinationcountry { get; set; }
    [Column("ORIGINCITY_")] public string? Origincity { get; set; }
    [Column("ORIGINCOUNTRY_")] public string? Origincountry { get; set; }
    [Column("REFERENCEWAYBILLNUMBER_")] public string? Referencewaybillnumber { get; set; }
    [Column("WAYBILLNUMBER_")] public string? Waybillnumber { get; set; }
    [Column("AGENCYECONOMICCODE_")] public string? Agencyeconomiccode { get; set; }
    [Column("TOTALCURRENCYAMOUNT_")] public decimal? Totalcurrencyamount { get; set; }
    [Column("TOTALRIALAMOUNT_")] public decimal? Totalrialamount { get; set; }
    [Column("TOTALNETWEIGHT_")] public decimal? Totalnetweight { get; set; }
    [Column("SUBSCRIBERNUMBER_")] public string? Subscribernumber { get; set; }
    [Column("CUSTOMSDECLARATIONDATE_")] public DateOnly? Customsdeclarationdate { get; set; }
    [Column("CUSTOMSDECLARATIONNUMBER_")] public string? Customsdeclarationnumber { get; set; }
    [Column("CUSTOMERPASSPORTNUMBER_")] public string? Customerpassportnumber { get; set; }
    [Column("FLIGHTTYPE_")] public byte? Flighttype { get; set; }
    [Column("ADJUSTMENTSAMOUNT_")] public decimal Adjustmentsamount { get; set; }
    [Column("ADJUSTMENTS_")] public string? Adjustments { get; set; }
    [Column("DUEDATE_")] public DateOnly? Duedate { get; set; }
    [Column("DESCRIPTION_")] public string? Description { get; set; }
    [Column("TAXPAYERPORTALSTATUS_")] public byte Taxpayerportalstatus { get; set; }
    [Column("TOTALARTICLE17TAXAMOUNT_")] public decimal? Totalarticle17taxamount { get; set; }
    [Column("TOTALVATPAIDAMOUNT_")] public decimal? Totalvatpaidamount { get; set; }
    [Column("TOTALCREDITAMOUNT_")] public decimal? Totalcreditamount { get; set; }
    [Column("TOTALCASHPAIDAMOUNT_")] public decimal? Totalcashpaidamount { get; set; }
    [Column("SETTLEMENTTYPEID_")] public byte? Settlementtypeid { get; set; }
    [Column("TOTALAMOUNT_")] public decimal Totalamount { get; set; }
    [Column("PAIDAMOUNT_")] public decimal Paidamount { get; set; }
    [Column("TOTALAMOUNTAFTERDISCOUNT_")] public decimal Totalamountafterdiscount { get; set; }
    [Column("TOTALOTHERTAXESANDCHARGESAMOUNT_")] public decimal? Totalothertaxesandchargesamount { get; set; }
    [Column("PAYMENTSTATUS_")] public byte Paymentstatus { get; set; }
    [Column("INVENTORYSTATUS_")] public byte? Inventorystatus { get; set; }
    [Column("DELIVERYSTATUS_")] public byte? Deliverystatus { get; set; }
    [Column("STATUS_")] public byte Status { get; set; }
    [Column("CONTRACTNUMBER_")] public string? Contractnumber { get; set; }
    [Column("SELLERCUSTOMSOFFICECODE_")] public string? Sellercustomsofficecode { get; set; }
    [Column("CUSTOMSPERMITNUMBER_")] public string? Customspermitnumber { get; set; }
    [Column("CUSTOMERBRANCHCODE_")] public string? Customerbranchcode { get; set; }
    [Column("CUSTOMERPOSTALCODE_")] public string? Customerpostalcode { get; set; }
    [Column("SELLERBRANCHCODE_")] public string? Sellerbranchcode { get; set; }
    [Column("CUSTOMERECONOMICCODE_")] public string? Customereconomiccode { get; set; }
    [Column("CUSTOMERIDENTIFICATIONNUMBER_")] public string? Customeridentificationnumber { get; set; }
    [Column("CUSTOMERPERSONTYPE_")] public byte? Customerpersontype { get; set; }
    [Column("SELLERECONOMICCODE_")] public string? Sellereconomiccode { get; set; }
    [Column("CUSTOMERID_")] public int Customerid { get; set; }
    [Column("SALESPERSONID_")] public int? Salespersonid { get; set; }
    [Column("INVOICESUBJECT_")] public byte Invoicesubject { get; set; }
    [Column("INVOICEFORMAT_")] public byte Invoiceformat { get; set; }
    [Column("REFERENCETAXUNIQUEID_")] public string? Referencetaxuniqueid { get; set; }
    [Column("TAXINVOICEINTERNALSERIAL_")] public string? Taxinvoiceinternalserial { get; set; }
    [Column("INVOICETYPE_")] public byte? Invoicetype { get; set; }
    [Column("CREATIONDATETIME_")] public DateTime Creationdatetime { get; set; }
    [Column("ISSUEDATETIME_")] public DateTime Issuedatetime { get; set; }
    [Column("TAXUNIQUEID_")] public string? Taxuniqueid { get; set; }
    [Column("INVOICENUMBER_")] public int Invoicenumber { get; set; }
    [Column("EXCHANGERATE_")] public decimal Exchangerate { get; set; }
    [Column("CURRENCY_")] public string Currency { get; set; }
    [Column("FISCALPERIODID_")] public int Fiscalperiodid { get; set; }
    [Column("PROJECTID_")] public int? Projectid { get; set; }
    [Column("WAREHOUSEID_")] public int? Warehouseid { get; set; }
    [Column("REFERENCESALEORDERID_")] public long? Referencesaleorderid { get; set; }
    [Column("REMAINEDAMOUNT_")] public decimal? Remainedamount { get; set; }
    [Column("ISSTOCKCARDAUTOCREATED_")] public bool Isstockcardautocreated { get; set; }
}
[Table("TBL_SaleOrderConfig", Schema="dbo")] public sealed class SqlTblSaleorderconfig : SqlServerEntity {
    [Column("CONFIGID_")] [Key] public int Configid { get; set; }
    [Column("UPDATESALEPRICEONINVOICESAVE_")] public bool Updatesalepriceoninvoicesave { get; set; }
    [Column("UPDATEPURCHASEPRICEONINVOICESAVE_")] public bool Updatepurchasepriceoninvoicesave { get; set; }
    [Column("NOTIFYUSERAFTERPRICEUPDATE_")] public bool Notifyuserafterpriceupdate { get; set; }
    [Column("ALLOWLOWSTOCKSALE_")] public bool Allowlowstocksale { get; set; }
    [Column("SHOWZERONEGATIVESTOCKITEMS_")] public bool Showzeronegativestockitems { get; set; }
    [Column("ALLOWDUPLICATEITEMSININVOICE_")] public bool Allowduplicateitemsininvoice { get; set; }
    [Column("CHECKCUSTOMERCREDITONSALE_")] public bool Checkcustomercreditonsale { get; set; }
    [Column("WARNSALEBELOWPURCHASEPRICE_")] public bool Warnsalebelowpurchaseprice { get; set; }
    [Column("SHOWPROFITININVOICE_")] public bool Showprofitininvoice { get; set; }
    [Column("DEFAULTCUSTOMER_")] public int? Defaultcustomer { get; set; }
    [Column("SALESPERSONREQUIRED_")] public bool Salespersonrequired { get; set; }
    [Column("ENABLEAUTOWAREHOUSEISSUE_")] public bool Enableautowarehouseissue { get; set; }
    [Column("INVOICEPRINTMETHOD_")] public string Invoiceprintmethod { get; set; }
    [Column("ENABLESCALE_")] public bool Enablescale { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("PAYMENTPAGEOPENINGMODE_")] public byte Paymentpageopeningmode { get; set; }
}
[Table("TBL_SaleOrderItem", Schema="dbo")] public sealed class SqlTblSaleorderitem : SqlServerEntity {
    [Column("SALEORDERITEMID_")] [Key] public long Saleorderitemid { get; set; }
    [Column("SALEORDERID_")] public long Saleorderid { get; set; }
    [Column("PRODUCTID_")] public int Productid { get; set; }
    [Column("QUANTITY_")] public decimal? Quantity { get; set; }
    [Column("UNIT_")] public short? Unit { get; set; }
    [Column("PRICE_")] public decimal? Price { get; set; }
    [Column("LINEPERCENTDISCOUNT_")] public decimal? Linepercentdiscount { get; set; }
    [Column("LINETOTALAMOUNT_")] public decimal Linetotalamount { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("COST_")] public decimal? Cost { get; set; }
    [Column("VATRATE_")] public decimal Vatrate { get; set; }
    [Column("UNITCONVERSIONFACTOR_")] public decimal? Unitconversionfactor { get; set; }
    [Column("VATAMOUNT_")] public decimal Vatamount { get; set; }
    [Column("PRODUCTDESCRIPTION_")] public string? Productdescription { get; set; }
    [Column("PRODUCTTAXCODE_")] public string? Producttaxcode { get; set; }
    [Column("PROJECTID_")] public int? Projectid { get; set; }
    [Column("LINEWAREHOUSEID_")] public int? Linewarehouseid { get; set; }
    [Column("FISCALPERIODID_")] public int Fiscalperiodid { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
    [Column("VATBASEAMOUNT_")] public decimal? Vatbaseamount { get; set; }
    [Column("VATCALCULATIONBASE_")] public decimal? Vatcalculationbase { get; set; }
    [Column("CURRENCYBUYRATE_")] public decimal? Currencybuyrate { get; set; }
    [Column("PURITY_")] public decimal? Purity { get; set; }
    [Column("TOTALMAKINGCOMMISSIONPROFIT_")] public decimal? Totalmakingcommissionprofit { get; set; }
    [Column("COMMISSION_")] public decimal? Commission { get; set; }
    [Column("SELLERPROFIT_")] public decimal? Sellerprofit { get; set; }
    [Column("MAKINGCHARGE_")] public decimal? Makingcharge { get; set; }
    [Column("LINECURRENCYVALUE_")] public decimal? Linecurrencyvalue { get; set; }
    [Column("COMMISSIONCONTRACTNUMBER_")] public string? Commissioncontractnumber { get; set; }
    [Column("LINERIALVALUE_")] public decimal? Linerialvalue { get; set; }
    [Column("NETWEIGHT_")] public decimal? Netweight { get; set; }
    [Column("LINEVATPAIDAMOUNT_")] public decimal? Linevatpaidamount { get; set; }
    [Column("LINECASHPAIDAMOUNT_")] public decimal? Linecashpaidamount { get; set; }
    [Column("OTHERLEGALDUESAMOUNT_")] public decimal? Otherlegalduesamount { get; set; }
    [Column("OTHERLEGALDUESRATE_")] public decimal? Otherlegalduesrate { get; set; }
    [Column("OTHERLEGALDUESSUBJECT_")] public string? Otherlegalduessubject { get; set; }
    [Column("OTHERTAXESANDCHARGESAMOUNT_")] public decimal? Othertaxesandchargesamount { get; set; }
    [Column("OTHERTAXESANDCHARGESRATE_")] public decimal? Othertaxesandchargesrate { get; set; }
    [Column("OTHERTAXESANDCHARGESSUBJECT_")] public string? Othertaxesandchargessubject { get; set; }
    [Column("LINEAMOUNTAFTERDISCOUNT_")] public decimal Lineamountafterdiscount { get; set; }
    [Column("LINEDISCOUNTAMOUNT_")] public decimal Linediscountamount { get; set; }
    [Column("LINEAMOUNTBEFOREDISCOUNT_")] public decimal Lineamountbeforediscount { get; set; }
    [Column("LINECURRENCYAMOUNT_")] public decimal? Linecurrencyamount { get; set; }
    [Column("LINEEXCHANGERATE_")] public decimal? Lineexchangerate { get; set; }
    [Column("LINECURRENCY_")] public string? Linecurrency { get; set; }
}
[Table("TBL_SaleOrderReturn", Schema="dbo")] public sealed class SqlTblSaleorderreturn : SqlServerEntity {
    [Column("SALEORDERRETURNID_")] [Key] public long Saleorderreturnid { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
    [Column("FISCALPERIODID_")] public int Fiscalperiodid { get; set; }
    [Column("WAREHOUSEID_")] public int? Warehouseid { get; set; }
    [Column("PROJECTID_")] public int? Projectid { get; set; }
    [Column("REFERENCESALEORDERID_")] public long Referencesaleorderid { get; set; }
    [Column("INVOICENUMBER_")] public int Invoicenumber { get; set; }
    [Column("ISSUEDATETIME_")] public DateTime Issuedatetime { get; set; }
    [Column("CREATIONDATETIME_")] public DateTime Creationdatetime { get; set; }
    [Column("STATUS_")] public byte Status { get; set; }
    [Column("DELIVERYSTATUS_")] public byte? Deliverystatus { get; set; }
    [Column("INVENTORYSTATUS_")] public byte? Inventorystatus { get; set; }
    [Column("PAYMENTSTATUS_")] public byte Paymentstatus { get; set; }
    [Column("TOTALRETURNAMOUNTBEFOREDISCOUNT_")] public decimal Totalreturnamountbeforediscount { get; set; }
    [Column("TOTALDISCOUNTRETURNAMOUNT_")] public decimal Totaldiscountreturnamount { get; set; }
    [Column("TOTALRETURNAMOUNTAFTERDISCOUNT_")] public decimal Totalreturnamountafterdiscount { get; set; }
    [Column("TOTALVATRETURNAMOUNT_")] public decimal Totalvatreturnamount { get; set; }
    [Column("TOTALOTHERTAXESANDCHARGESRETURNAMOUNT_")] public decimal? Totalothertaxesandchargesreturnamount { get; set; }
    [Column("TOTALINVOICERETURNAMOUNT_")] public decimal Totalinvoicereturnamount { get; set; }
    [Column("PAIDRETURNAMOUNT_")] public decimal Paidreturnamount { get; set; }
    [Column("TOTALRETURNAMOUNT_")] public decimal Totalreturnamount { get; set; }
    [Column("DESCRIPTION_")] public string? Description { get; set; }
    [Column("ADJUSTMENTS_")] public string? Adjustments { get; set; }
    [Column("ADJUSTMENTSAMOUNT_")] public decimal Adjustmentsamount { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_SaleOrderReturnItem", Schema="dbo")] public sealed class SqlTblSaleorderreturnitem : SqlServerEntity {
    [Column("SALEORDERRETURNITEMID_")] [Key] public long Saleorderreturnitemid { get; set; }
    [Column("REFERENCESALEORDERITEMID_")] public long Referencesaleorderitemid { get; set; }
    [Column("SALEORDERRETURNID_")] public long Saleorderreturnid { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
    [Column("FISCALPERIODID_")] public int Fiscalperiodid { get; set; }
    [Column("LINEWAREHOUSEID_")] public int? Linewarehouseid { get; set; }
    [Column("PROJECTID_")] public int? Projectid { get; set; }
    [Column("PRODUCTID_")] public int Productid { get; set; }
    [Column("RETURNQUANTITY_")] public decimal Returnquantity { get; set; }
    [Column("UNIT_")] public short Unit { get; set; }
    [Column("UNITCONVERSIONFACTOR_")] public decimal? Unitconversionfactor { get; set; }
    [Column("PRICE_")] public decimal Price { get; set; }
    [Column("LINERETURNAMOUNTBEFOREDISCOUNT_")] public decimal Linereturnamountbeforediscount { get; set; }
    [Column("LINEPERCENTDISCOUNT_")] public decimal? Linepercentdiscount { get; set; }
    [Column("LINEDISCOUNTRETURNAMOUNT_")] public decimal Linediscountreturnamount { get; set; }
    [Column("LINERETURNAMOUNTAFTERDISCOUNT_")] public decimal Linereturnamountafterdiscount { get; set; }
    [Column("VATRATE_")] public decimal Vatrate { get; set; }
    [Column("VATRETURNAMOUNT_")] public decimal Vatreturnamount { get; set; }
    [Column("LINETOTALRETURNAMOUNT_")] public decimal Linetotalreturnamount { get; set; }
    [Column("OTHERTAXESANDCHARGESRETURNAMOUNT_")] public decimal? Othertaxesandchargesreturnamount { get; set; }
    [Column("OTHERLEGALDUESRETURNAMOUNT_")] public decimal? Otherlegalduesreturnamount { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_ServiceDiscount", Schema="dbo")] public sealed class SqlTblServicediscount : SqlServerEntity {
    [Column("ID_")] [Key] public short new Id { get; set; }
    [Column("NAME_")] public string Name { get; set; }
    [Column("CODE_")] public string Code { get; set; }
    [Column("STARTDATE_")] public DateOnly? Startdate { get; set; }
    [Column("ENDDATE_")] public DateOnly? Enddate { get; set; }
    [Column("ISPUBLIC_")] public bool Ispublic { get; set; }
    [Column("PERCENTAMOUNT_")] public decimal Percentamount { get; set; }
    [Column("FIXEDAMOUNT_")] public decimal Fixedamount { get; set; }
    [Column("DESCRIPTION_")] public string? Description { get; set; }
    [Column("ISENABLED_")] public bool Isenabled { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_ServicePayment", Schema="dbo")] public sealed class SqlTblServicepayment : SqlServerEntity {
    [Column("PAYMENTID_")] [Key] public int Paymentid { get; set; }
    [Column("INVOICENUMBER_")] public string? Invoicenumber { get; set; }
    [Column("INVOICEDATE_")] public string? Invoicedate { get; set; }
    [Column("TRANSACTIONREFERENCEID_")] public string? Transactionreferenceid { get; set; }
    [Column("AMOUNT_")] public decimal? Amount { get; set; }
    [Column("TRANSACTIONDATE_")] public DateTime? Transactiondate { get; set; }
    [Column("REFERENCENUMBER_")] public long? Referencenumber { get; set; }
    [Column("MASKEDCARDNUMBER_")] public string? Maskedcardnumber { get; set; }
    [Column("SHAPARAKREFNUMBER_")] public long? Shaparakrefnumber { get; set; }
    [Column("ISSUCCESS_")] public bool? Issuccess { get; set; }
    [Column("MESSAGE_")] public string? Message { get; set; }
    [Column("TRACENUMBER_")] public long? Tracenumber { get; set; }
    [Column("REGISTERTIME_")] public DateTime? Registertime { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_ServicePlan", Schema="dbo")] public sealed class SqlTblServiceplan : SqlServerEntity {
    [Column("PLANID_")] [Key] public byte Planid { get; set; }
    [Column("PLANNAME_")] public string Planname { get; set; }
    [Column("PLANDESCRIPTION_")] public string? Plandescription { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_ServiceRequest", Schema="dbo")] public sealed class SqlTblServicerequest : SqlServerEntity {
    [Column("ID_")] [Key] public int new Id { get; set; }
    [Column("SHOPID_")] public int? Shopid { get; set; }
    [Column("SUBSCRIPTIONID_")] public byte? Subscriptionid { get; set; }
    [Column("STATUS_")] public byte Status { get; set; }
    [Column("REQUESTTIME_")] public DateTime Requesttime { get; set; }
    [Column("PAYMENTID_")] public int? Paymentid { get; set; }
    [Column("PRICE_")] public decimal Price { get; set; }
    [Column("DISCOUNTID_")] public short? Discountid { get; set; }
    [Column("DISCOUNTAMOUNT_")] public decimal Discountamount { get; set; }
    [Column("PAYABLEAMOUNT_")] public decimal Payableamount { get; set; }
    [Column("SUBSCRIPTIONSTARTDATE_")] public DateOnly? Subscriptionstartdate { get; set; }
    [Column("SUBSCRIPTIONENDDATE_")] public DateOnly? Subscriptionenddate { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("USERID_")] public string Userid { get; set; }
    [Column("TAXAMOUNT_")] public decimal Taxamount { get; set; }
}
[Table("TBL_ServiceSubscription", Schema="dbo")] public sealed class SqlTblServicesubscription : SqlServerEntity {
    [Column("SUBSCRIPTIONID_")] [Key] public byte Subscriptionid { get; set; }
    [Column("SUBSCRIPTIONNAME_")] public string Subscriptionname { get; set; }
    [Column("PLANID_")] public byte Planid { get; set; }
    [Column("SUBSCRIPTIONPRICE_")] public decimal Subscriptionprice { get; set; }
    [Column("SUBSCRIPTIONDESCRIPTION_")] public string? Subscriptiondescription { get; set; }
    [Column("SUBSCRIPTIONENABLE_")] public bool Subscriptionenable { get; set; }
    [Column("SUBSCRIPTIONDEFAULT_")] public bool Subscriptiondefault { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("PERCENTDISCOUNT_")] public decimal Percentdiscount { get; set; }
    [Column("FIXEDDISCOUNT_")] public decimal Fixeddiscount { get; set; }
    [Column("SUBSCRIPTIONTAXRATE_")] public decimal Subscriptiontaxrate { get; set; }
    [Column("SUBSCRIPTIONDAYS_")] public short Subscriptiondays { get; set; }
    [Column("SUBSCRIPTIONPAYABLEAMOUNT_")] public decimal? Subscriptionpayableamount { get; set; }
    [Column("SUBSCRIPTIONTAXAMOUNT_")] public decimal? Subscriptiontaxamount { get; set; }
}
[Table("TBL_Shareholder", Schema="dbo")] public sealed class SqlTblShareholder : SqlServerEntity {
    [Column("SHAREHOLDERID_")] [Key] public int Shareholderid { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
    [Column("FISCALPERIODID_")] public int Fiscalperiodid { get; set; }
    [Column("PERSONID_")] public int Personid { get; set; }
    [Column("SHAREPERCENT_")] public decimal Sharepercent { get; set; }
    [Column("DESCRIPTION_")] public string? Description { get; set; }
    [Column("INITIALCAPITALSHARE_")] public decimal? Initialcapitalshare { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_Shop", Schema="dbo")] public sealed class SqlTblShop : SqlServerEntity {
    [Column("SHOPID_")] [Key] public int Shopid { get; set; }
    [Column("OWNERID_")] public string Ownerid { get; set; }
    [Column("NAME_")] public string Name { get; set; }
    [Column("DESCRIPTION_")] public string? Description { get; set; }
    [Column("ADDRESSLINE_")] public string? Addressline { get; set; }
    [Column("POSTALCODE_")] public string? Postalcode { get; set; }
    [Column("EMAIL_")] public string? Email { get; set; }
    [Column("PHONE_")] public string? Phone { get; set; }
    [Column("ECONOMICCODE_")] public string? Economiccode { get; set; }
    [Column("IDENTIFIERNUMBER_")] public string? Identifiernumber { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("ACTIVITYTYPEID_")] public byte? Activitytypeid { get; set; }
    [Column("LOGORELATIVEURL_")] public string? Logorelativeurl { get; set; }
    [Column("MOBILE_")] public string? Mobile { get; set; }
    [Column("BRANCHCODE_")] public string? Branchcode { get; set; }
    [Column("COUNTRY_")] public string? Country { get; set; }
    [Column("WEBSITE_")] public string? Website { get; set; }
    [Column("TYPE_")] public byte Type { get; set; }
    [Column("PROVINCE_")] public string? Province { get; set; }
    [Column("CITY_")] public string? City { get; set; }
    [Column("STATUS_")] public byte Status { get; set; }
}
[Table("TBL_ShopActivityType", Schema="dbo")] public sealed class SqlTblShopactivitytype : SqlServerEntity {
    [Column("ACTIVITYTYPEID_")] [Key] public byte Activitytypeid { get; set; }
    [Column("ACTIVITYTYPENAME_")] public string Activitytypename { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_ShopFiscalPeriod", Schema="dbo")] public sealed class SqlTblShopfiscalperiod : SqlServerEntity {
    [Column("FISCALPERIODID_")] [Key] public int Fiscalperiodid { get; set; }
    [Column("FISCALPERIODNAME_")] public string Fiscalperiodname { get; set; }
    [Column("FISCALPERIODDESCRIPTION_")] public string? Fiscalperioddescription { get; set; }
    [Column("STARTDATE_")] public DateOnly Startdate { get; set; }
    [Column("ENDDATE_")] public DateOnly Enddate { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
    [Column("FISCALPERIODSTATUSID_")] public byte Fiscalperiodstatusid { get; set; }
    [Column("FIRSTPERIOD_")] public bool Firstperiod { get; set; }
    [Column("LASTDOCUMENTNUMBER_")] public int Lastdocumentnumber { get; set; }
    [Column("LASTREFERENCENUMBER_")] public int Lastreferencenumber { get; set; }
    [Column("LASTDOCUMENTDATETIME_")] public DateTime? Lastdocumentdatetime { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_ShopNotification", Schema="dbo")] public sealed class SqlTblShopnotification : SqlServerEntity {
    [Column("NOTIFICATIONID_")] [Key] public int Notificationid { get; set; }
    [Column("NOTIFICATIONTYPEID_")] public byte Notificationtypeid { get; set; }
    [Column("NOTIFICATIONTITLE_")] public string? Notificationtitle { get; set; }
    [Column("NOTIFICATIONTEXT_")] public string Notificationtext { get; set; }
    [Column("NOTIFICATIONCLOSEALLOWED_")] public bool Notificationcloseallowed { get; set; }
    [Column("NOTIFICATIONSTARTTIME_")] public DateTime Notificationstarttime { get; set; }
    [Column("NOTIFICATIONENDTIME_")] public DateTime Notificationendtime { get; set; }
    [Column("NOTIFICATIONALLSHOPS_")] public bool Notificationallshops { get; set; }
    [Column("NOTIFICATIONALLMEMBERROLES_")] public bool Notificationallmemberroles { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("ISBANNER_")] public bool Isbanner { get; set; }
}
[Table("TBL_ShopNotificationInMemberRole", Schema="dbo")] public sealed class SqlTblShopnotificationinmemberrole : SqlServerEntity {
    [Column("NOTIFICATIONINMEMBERROLEID_")] [Key] public int Notificationinmemberroleid { get; set; }
    [Column("NOTIFICATIONID_")] public int Notificationid { get; set; }
    [Column("MEMBERROLEID_")] public byte Memberroleid { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_ShopNotificationInShop", Schema="dbo")] public sealed class SqlTblShopnotificationinshop : SqlServerEntity {
    [Column("NOTIFICATIONINSHOPID_")] [Key] public int Notificationinshopid { get; set; }
    [Column("NOTIFICATIONID_")] public int Notificationid { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_ShopNotificationType", Schema="dbo")] public sealed class SqlTblShopnotificationtype : SqlServerEntity {
    [Column("NOTIFICATIONTYPEID_")] [Key] public byte Notificationtypeid { get; set; }
    [Column("NOTIFICATIONTYPENAME_")] public string Notificationtypename { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_StockCard", Schema="dbo")] public sealed class SqlTblStockcard : SqlServerEntity {
    [Column("ID_")] [Key] public int new Id { get; set; }
    [Column("WAREHOUSEID_")] public int Warehouseid { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
    [Column("FISCALPERIODID_")] public int Fiscalperiodid { get; set; }
    [Column("PROJECTID_")] public int? Projectid { get; set; }
    [Column("DATETIME_")] public DateTime Datetime { get; set; }
    [Column("NUMBER_")] public int Number { get; set; }
    [Column("DESCRIPTION_")] public string? Description { get; set; }
    [Column("PERSONID_")] public int? Personid { get; set; }
    [Column("DIRECTION_")] public bool Direction { get; set; }
    [Column("REFERENCETYPE_")] public byte Referencetype { get; set; }
    [Column("REFERENCEID_")] public long? Referenceid { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_StockCardItem", Schema="dbo")] public sealed class SqlTblStockcarditem : SqlServerEntity {
    [Column("ID_")] [Key] public int new Id { get; set; }
    [Column("STOCKCARDID_")] public int Stockcardid { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
    [Column("PRODUCTID_")] public int Productid { get; set; }
    [Column("QUANTITY_")] public decimal Quantity { get; set; }
    [Column("SHOWINSECONDUNIT_")] public bool Showinsecondunit { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_StockTaking", Schema="dbo")] public sealed class SqlTblStocktaking : SqlServerEntity {
    [Column("ID_")] [Key] public int new Id { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
    [Column("WAREHOUSEID_")] public int Warehouseid { get; set; }
    [Column("FISCALPERIODID_")] public int Fiscalperiodid { get; set; }
    [Column("PROJECTID_")] public int? Projectid { get; set; }
    [Column("DESCRIPTION_")] public string? Description { get; set; }
    [Column("STATUS_")] public byte Status { get; set; }
    [Column("DATETIME_")] public DateTime Datetime { get; set; }
    [Column("NUMBER_")] public int? Number { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_StockTakingItem", Schema="dbo")] public sealed class SqlTblStocktakingitem : SqlServerEntity {
    [Column("ID_")] [Key] public int new Id { get; set; }
    [Column("STOCKTAKINGID_")] public int Stocktakingid { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
    [Column("PRODUCTID_")] public int Productid { get; set; }
    [Column("INITIALQUANTITY_")] public decimal Initialquantity { get; set; }
    [Column("FINALQUANTITY_")] public decimal? Finalquantity { get; set; }
    [Column("SHOWINSECONDUNIT_")] public bool Showinsecondunit { get; set; }
    [Column("UNITCOST_")] public decimal? Unitcost { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_Tag", Schema="dbo")] public sealed class SqlTblTag : SqlServerEntity {
    [Column("ID_")] [Key] public int new Id { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
    [Column("VALUE_")] public string Value { get; set; }
    [Column("ENTITYTYPE_")] public byte Entitytype { get; set; }
    [Column("DESCRIPTION_")] public string? Description { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_TaxpayerPortalConfig", Schema="dbo")] public sealed class SqlTblTaxpayerportalconfig : SqlServerEntity {
    [Column("CONFIGID_")] [Key] public int Configid { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
    [Column("ISTAXPAYERPORTALENABLED_")] public bool Istaxpayerportalenabled { get; set; }
    [Column("TAXMEMORYUNIQUEID_")] public string? Taxmemoryuniqueid { get; set; }
    [Column("PRIVATEKEY_")] public string? Privatekey { get; set; }
    [Column("DIGITALSIGNATURECERTIFICATE_")] public string? Digitalsignaturecertificate { get; set; }
    [Column("INVOICEFORMAT_")] public byte Invoiceformat { get; set; }
    [Column("INVOICETYPE_")] public byte? Invoicetype { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_Transfer", Schema="dbo")] public sealed class SqlTblTransfer : SqlServerEntity {
    [Column("TRANSFERID_")] [Key] public int Transferid { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
    [Column("FISCALPERIODID_")] public int Fiscalperiodid { get; set; }
    [Column("PROJECTID_")] public int? Projectid { get; set; }
    [Column("DESCRIPTION_")] public string? Description { get; set; }
    [Column("TOTALAMOUNT_")] public decimal Totalamount { get; set; }
    [Column("DATETIME_")] public DateTime Datetime { get; set; }
    [Column("CURRENCY_")] public string Currency { get; set; }
    [Column("EXCHANGERATE_")] public decimal Exchangerate { get; set; }
    [Column("TRANSFERTYPE_")] public byte Transfertype { get; set; }
    [Column("REFERENCETYPE_")] public byte? Referencetype { get; set; }
    [Column("REFERENCEID_")] public long? Referenceid { get; set; }
    [Column("NUMBER_")] public int Number { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_TransferItem", Schema="dbo")] public sealed class SqlTblTransferitem : SqlServerEntity {
    [Column("TRANSFERITEMID_")] [Key] public int Transferitemid { get; set; }
    [Column("TRANSFERID_")] public int Transferid { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
    [Column("TRANSFERTYPE_")] public byte Transfertype { get; set; }
    [Column("ISDESTINATION_")] public bool Isdestination { get; set; }
    [Column("AMOUNT_")] public decimal Amount { get; set; }
    [Column("COMMISSION_")] public decimal Commission { get; set; }
    [Column("DESCRIPTION_")] public string? Description { get; set; }
    [Column("ACCOUNTID_")] public int Accountid { get; set; }
    [Column("ENTITYID_")] public int? Entityid { get; set; }
    [Column("CHECKID_")] public int? Checkid { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_UploadFile", Schema="dbo")] public sealed class SqlTblUploadfile : SqlServerEntity {
    [Column("ID_")] [Key] public int new Id { get; set; }
    [Column("ORIGINALNAME_")] public string Originalname { get; set; }
    [Column("NAME_")] public string Name { get; set; }
    [Column("EXTENSION_")] public string Extension { get; set; }
    [Column("SIZE_")] public long Size { get; set; }
    [Column("UID_")] public string Uid { get; set; }
    [Column("RELATIVEURL_")] public string Relativeurl { get; set; }
    [Column("FILETYPE_")] public string Filetype { get; set; }
    [Column("CREATETIME_")] public DateTime Createtime { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_UserActionLog", Schema="dbo")] public sealed class SqlTblUseractionlog : SqlServerEntity {
    [Column("ID_")] [Key] public long new Id { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
    [Column("USERID_")] public string Userid { get; set; }
    [Column("ACTIONTYPE_")] public byte Actiontype { get; set; }
    [Column("ENTITYTYPE_")] public byte? Entitytype { get; set; }
    [Column("ENTITYID_")] public long? Entityid { get; set; }
    [Column("DETAILS_")] public string? Details { get; set; }
    [Column("DATETIME_")] public DateTime Datetime { get; set; }
    [Column("PARENTID_")] public long? Parentid { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
}
[Table("TBL_Warehouse", Schema="dbo")] public sealed class SqlTblWarehouse : SqlServerEntity {
    [Column("ID_")] [Key] public int new Id { get; set; }
    [Column("NAME_")] public string Name { get; set; }
    [Column("SHOPID_")] public int Shopid { get; set; }
    [Column("ISDEFAULT_")] public bool Isdefault { get; set; }
    [Column("TENANT_ID_")] public string? TenantId { get; set; }
    [Column("ISENABLED_")] public bool Isenabled { get; set; }
    [Column("DETAILACCOUNTID_")] public long Detailaccountid { get; set; }
    [Column("DESCRIPTION_")] public string? Description { get; set; }
    [Column("PERSONID_")] public int Personid { get; set; }
    [Column("PHONE_")] public string? Phone { get; set; }
    [Column("ADDRESS_")] public string? Address { get; set; }
}
[Table("View_AccountBalance", Schema="dbo")] public sealed class SqlViewAccountbalance : SqlServerEntity {
    [Column("documentDate")] public DateOnly Documentdate { get; set; }
    [Column("accountId")] [Key] public int Accountid { get; set; }
    [Column("documentTypeId")] public byte Documenttypeid { get; set; }
    [Column("totalDebit")] public decimal? Totaldebit { get; set; }
    [Column("totalCredit")] public decimal? Totalcredit { get; set; }
    [Column("cnt")] public long? Cnt { get; set; }
}
[Table("View_DetailAccountBalance", Schema="dbo")] public sealed class SqlViewDetailaccountbalance : SqlServerEntity {
    [Column("accountId")] [Key] public int Accountid { get; set; }
    [Column("detailAccountId")] public long? Detailaccountid { get; set; }
    [Column("totalDebit")] public decimal? Totaldebit { get; set; }
    [Column("totalCredit")] public decimal? Totalcredit { get; set; }
    [Column("cnt")] public long? Cnt { get; set; }
}
[Table("vw_IntegrationDashboard", Schema="dbo")] public sealed class SqlVwIntegrationdashboard : SqlServerEntity {
    [Column("ShopId")] [Key] public int Shopid { get; set; }
    [Column("TenantId")] public string Tenantid { get; set; }
    [Column("Provider")] public byte Provider { get; set; }
    [Column("DisplayName")] public string Displayname { get; set; }
    [Column("IsEnabled")] public bool Isenabled { get; set; }
    [Column("LastSyncAtUtc")] public DateTime? Lastsyncatutc { get; set; }
    [Column("MappingCount")] public int? Mappingcount { get; set; }
    [Column("LastRunAtUtc")] public DateTime? Lastrunatutc { get; set; }
    [Column("FailedRuns")] public int? Failedruns { get; set; }
}

