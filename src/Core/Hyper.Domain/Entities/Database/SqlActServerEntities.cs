namespace Hyper.Domain.Entities.Database;

[DisplayName("موتور: داده باینری")]
[DbMap("ACT_GE_BYTEARRAY")]
public sealed class SqlActGeBytearray : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int? Rev { get; set; }
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string? Name { get; set; }
    [DisplayName("استقرار شناسه")]
    [DbMap("DEPLOYMENT_ID_")]
    public string? DeploymentId { get; set; }
    [DisplayName("داده باینری")]
    [DbMap("BYTES_")]
    public byte[]? Bytes { get; set; }
    [DisplayName("تولیدشده")]
    [DbMap("GENERATED_")]
    public byte? Generated { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("نوع")]
    [DbMap("TYPE_")]
    public int? Type { get; set; }
    [DisplayName("ایجاد زمان")]
    [DbMap("CREATE_TIME_")]
    public DateTime? CreateTime { get; set; }
    [DisplayName("ریشه فرایند نمونه شناسه")]
    [DbMap("ROOT_PROC_INST_ID_")]
    public string? RootProcInstId { get; set; }
    [DisplayName("حذف زمان")]
    [DbMap("REMOVAL_TIME_")]
    public DateTime? RemovalTime { get; set; }
}
[DisplayName("موتور: ویژگی")]
[DbMap("ACT_GE_PROPERTY")]
public sealed class SqlActGeProperty : SqlServerEntity
{
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string Name { get; set; } = null!;
    [DisplayName("مقدار")]
    [DbMap("VALUE_")]
    public string? Value { get; set; }
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int? Rev { get; set; }
}
[DisplayName("موتور: سوابق")]
[DbMap("ACT_GE_SCHEMA_LOG")]
public sealed class SqlActGeSchemaLog : SqlServerEntity<string>
{
    [DisplayName("مهر زمانی")]
    [DbMap("TIMESTAMP_")]
    public DateTime? Timestamp { get; set; }
    [DisplayName("نسخه")]
    [DbMap("VERSION_")]
    public string? Version { get; set; }
}
[DisplayName("سوابق موتور: نمونه فعالیت")]
[DbMap("ACT_HI_ACTINST")]
public sealed class SqlActHiActinst : SqlServerEntity<string>
{
    [DisplayName("والد فعالیت نمونه شناسه")]
    [DbMap("PARENT_ACT_INST_ID_")]
    public string? ParentActInstId { get; set; }
    [DisplayName("فرایند تعریف کلید")]
    [DbMap("PROC_DEF_KEY_")]
    public string? ProcDefKey { get; set; }
    [DisplayName("فرایند تعریف شناسه")]
    [DbMap("PROC_DEF_ID_")]
    public string ProcDefId { get; set; } = null!;
    [DisplayName("ریشه فرایند نمونه شناسه")]
    [DbMap("ROOT_PROC_INST_ID_")]
    public string? RootProcInstId { get; set; }
    [DisplayName("فرایند نمونه شناسه")]
    [DbMap("PROC_INST_ID_")]
    public string ProcInstId { get; set; } = null!;
    [DisplayName("اجرا شناسه")]
    [DbMap("EXECUTION_ID_")]
    public string ExecutionId { get; set; } = null!;
    [DisplayName("فعالیت شناسه")]
    [DbMap("ACT_ID_")]
    public string ActId { get; set; } = null!;
    [DisplayName("وظیفه شناسه")]
    [DbMap("TASK_ID_")]
    public string? TaskId { get; set; }
    [DisplayName("فراخوانی فرایند نمونه شناسه")]
    [DbMap("CALL_PROC_INST_ID_")]
    public string? CallProcInstId { get; set; }
    [DisplayName("فراخوانی پرونده نمونه شناسه")]
    [DbMap("CALL_CASE_INST_ID_")]
    public string? CallCaseInstId { get; set; }
    [DisplayName("فعالیت نام")]
    [DbMap("ACT_NAME_")]
    public string? ActName { get; set; }
    [DisplayName("فعالیت نوع")]
    [DbMap("ACT_TYPE_")]
    public string ActType { get; set; } = null!;
    [DisplayName("مسئول")]
    [DbMap("ASSIGNEE_")]
    public string? Assignee { get; set; }
    [DisplayName("شروع زمان")]
    [DbMap("START_TIME_")]
    public DateTime StartTime { get; set; }
    [DisplayName("پایان زمان")]
    [DbMap("END_TIME_")]
    public DateTime? EndTime { get; set; }
    [DisplayName("مدت")]
    [DbMap("DURATION_")]
    public decimal? Duration { get; set; }
    [DisplayName("فعالیت نمونه وضعیت")]
    [DbMap("ACT_INST_STATE_")]
    public byte? ActInstState { get; set; }
    [DisplayName("توالی شمارنده")]
    [DbMap("SEQUENCE_COUNTER_")]
    public decimal? SequenceCounter { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("حذف زمان")]
    [DbMap("REMOVAL_TIME_")]
    public DateTime? RemovalTime { get; set; }
}
[DisplayName("سوابق موتور: پیوست")]
[DbMap("ACT_HI_ATTACHMENT")]
public sealed class SqlActHiAttachment : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int? Rev { get; set; }
    [DisplayName("کاربر شناسه")]
    [DbMap("USER_ID_")]
    public string? UserId { get; set; }
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string? Name { get; set; }
    [DisplayName("توضیحات")]
    [DbMap("DESCRIPTION_")]
    public string? Description { get; set; }
    [DisplayName("نوع")]
    [DbMap("TYPE_")]
    public string? Type { get; set; }
    [DisplayName("وظیفه شناسه")]
    [DbMap("TASK_ID_")]
    public string? TaskId { get; set; }
    [DisplayName("ریشه فرایند نمونه شناسه")]
    [DbMap("ROOT_PROC_INST_ID_")]
    public string? RootProcInstId { get; set; }
    [DisplayName("فرایند نمونه شناسه")]
    [DbMap("PROC_INST_ID_")]
    public string? ProcInstId { get; set; }
    [DisplayName("نشانی وب")]
    [DbMap("URL_")]
    public string? Url { get; set; }
    [DisplayName("محتوا شناسه")]
    [DbMap("CONTENT_ID_")]
    public string? ContentId { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("ایجاد زمان")]
    [DbMap("CREATE_TIME_")]
    public DateTime? CreateTime { get; set; }
    [DisplayName("حذف زمان")]
    [DbMap("REMOVAL_TIME_")]
    public DateTime? RemovalTime { get; set; }
}
[DisplayName("سوابق موتور: دسته")]
[DbMap("ACT_HI_BATCH")]
public sealed class SqlActHiBatch : SqlServerEntity<string>
{
    [DisplayName("نوع")]
    [DbMap("TYPE_")]
    public string? Type { get; set; }
    [DisplayName("مجموع کارهای پس‌زمینه")]
    [DbMap("TOTAL_JOBS_")]
    public int? TotalJobs { get; set; }
    [DisplayName("کارهای پس‌زمینه به‌ازای بذر")]
    [DbMap("JOBS_PER_SEED_")]
    public int? JobsPerSeed { get; set; }
    [DisplayName("فراخوانی‌ها به‌ازای کار پس‌زمینه")]
    [DbMap("INVOCATIONS_PER_JOB_")]
    public int? InvocationsPerJob { get; set; }
    [DisplayName("بذر کار پس‌زمینه تعریف شناسه")]
    [DbMap("SEED_JOB_DEF_ID_")]
    public string? SeedJobDefId { get; set; }
    [DisplayName("پایش کار پس‌زمینه تعریف شناسه")]
    [DbMap("MONITOR_JOB_DEF_ID_")]
    public string? MonitorJobDefId { get; set; }
    [DisplayName("دسته کار پس‌زمینه تعریف شناسه")]
    [DbMap("BATCH_JOB_DEF_ID_")]
    public string? BatchJobDefId { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("ایجاد کاربر شناسه")]
    [DbMap("CREATE_USER_ID_")]
    public string? CreateUserId { get; set; }
    [DisplayName("شروع زمان")]
    [DbMap("START_TIME_")]
    public DateTime StartTime { get; set; }
    [DisplayName("پایان زمان")]
    [DbMap("END_TIME_")]
    public DateTime? EndTime { get; set; }
    [DisplayName("حذف زمان")]
    [DbMap("REMOVAL_TIME_")]
    public DateTime? RemovalTime { get; set; }
}
[DisplayName("سوابق موتور: نمونه فعالیت پرونده")]
[DbMap("ACT_HI_CASEACTINST")]
public sealed class SqlActHiCaseactinst : SqlServerEntity<string>
{
    [DisplayName("والد فعالیت نمونه شناسه")]
    [DbMap("PARENT_ACT_INST_ID_")]
    public string? ParentActInstId { get; set; }
    [DisplayName("پرونده تعریف شناسه")]
    [DbMap("CASE_DEF_ID_")]
    public string CaseDefId { get; set; } = null!;
    [DisplayName("پرونده نمونه شناسه")]
    [DbMap("CASE_INST_ID_")]
    public string CaseInstId { get; set; } = null!;
    [DisplayName("پرونده فعالیت شناسه")]
    [DbMap("CASE_ACT_ID_")]
    public string CaseActId { get; set; } = null!;
    [DisplayName("وظیفه شناسه")]
    [DbMap("TASK_ID_")]
    public string? TaskId { get; set; }
    [DisplayName("فراخوانی فرایند نمونه شناسه")]
    [DbMap("CALL_PROC_INST_ID_")]
    public string? CallProcInstId { get; set; }
    [DisplayName("فراخوانی پرونده نمونه شناسه")]
    [DbMap("CALL_CASE_INST_ID_")]
    public string? CallCaseInstId { get; set; }
    [DisplayName("پرونده فعالیت نام")]
    [DbMap("CASE_ACT_NAME_")]
    public string? CaseActName { get; set; }
    [DisplayName("پرونده فعالیت نوع")]
    [DbMap("CASE_ACT_TYPE_")]
    public string? CaseActType { get; set; }
    [DisplayName("ایجاد زمان")]
    [DbMap("CREATE_TIME_")]
    public DateTime CreateTime { get; set; }
    [DisplayName("پایان زمان")]
    [DbMap("END_TIME_")]
    public DateTime? EndTime { get; set; }
    [DisplayName("مدت")]
    [DbMap("DURATION_")]
    public decimal? Duration { get; set; }
    [DisplayName("وضعیت")]
    [DbMap("STATE_")]
    public byte? State { get; set; }
    [DisplayName("الزامی")]
    [DbMap("REQUIRED_")]
    public byte? Required { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("سوابق موتور: نمونه پرونده")]
[DbMap("ACT_HI_CASEINST")]
public sealed class SqlActHiCaseinst : SqlServerEntity<string>
{
    [DisplayName("پرونده نمونه شناسه")]
    [DbMap("CASE_INST_ID_")]
    public string CaseInstId { get; set; } = null!;
    [DisplayName("کسب‌وکار کلید")]
    [DbMap("BUSINESS_KEY_")]
    public string? BusinessKey { get; set; }
    [DisplayName("پرونده تعریف شناسه")]
    [DbMap("CASE_DEF_ID_")]
    public string CaseDefId { get; set; } = null!;
    [DisplayName("ایجاد زمان")]
    [DbMap("CREATE_TIME_")]
    public DateTime CreateTime { get; set; }
    [DisplayName("بستن زمان")]
    [DbMap("CLOSE_TIME_")]
    public DateTime? CloseTime { get; set; }
    [DisplayName("مدت")]
    [DbMap("DURATION_")]
    public decimal? Duration { get; set; }
    [DisplayName("وضعیت")]
    [DbMap("STATE_")]
    public byte? State { get; set; }
    [DisplayName("ایجاد کاربر شناسه")]
    [DbMap("CREATE_USER_ID_")]
    public string? CreateUserId { get; set; }
    [DisplayName("بالادست پرونده نمونه شناسه")]
    [DbMap("SUPER_CASE_INSTANCE_ID_")]
    public string? SuperCaseInstanceId { get; set; }
    [DisplayName("بالادست فرایند نمونه شناسه")]
    [DbMap("SUPER_PROCESS_INSTANCE_ID_")]
    public string? SuperProcessInstanceId { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("سوابق موتور: نظر")]
[DbMap("ACT_HI_COMMENT")]
public sealed class SqlActHiComment : SqlServerEntity<string>
{
    [DisplayName("نوع")]
    [DbMap("TYPE_")]
    public string? Type { get; set; }
    [DisplayName("زمان")]
    [DbMap("TIME_")]
    public DateTime Time { get; set; }
    [DisplayName("کاربر شناسه")]
    [DbMap("USER_ID_")]
    public string? UserId { get; set; }
    [DisplayName("وظیفه شناسه")]
    [DbMap("TASK_ID_")]
    public string? TaskId { get; set; }
    [DisplayName("ریشه فرایند نمونه شناسه")]
    [DbMap("ROOT_PROC_INST_ID_")]
    public string? RootProcInstId { get; set; }
    [DisplayName("فرایند نمونه شناسه")]
    [DbMap("PROC_INST_ID_")]
    public string? ProcInstId { get; set; }
    [DisplayName("عملیات")]
    [DbMap("ACTION_")]
    public string? Action { get; set; }
    [DisplayName("پیام")]
    [DbMap("MESSAGE_")]
    public string? Message { get; set; }
    [DisplayName("کامل پیام")]
    [DbMap("FULL_MSG_")]
    public byte[]? FullMsg { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("حذف زمان")]
    [DbMap("REMOVAL_TIME_")]
    public DateTime? RemovalTime { get; set; }
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int Rev { get; set; }
}
[DisplayName("سوابق موتور: ورودی")]
[DbMap("ACT_HI_DEC_IN")]
public sealed class SqlActHiDecIn : SqlServerEntity<string>
{
    [DisplayName("تصمیم نمونه شناسه")]
    [DbMap("DEC_INST_ID_")]
    public string DecInstId { get; set; } = null!;
    [DisplayName("عبارت شناسه")]
    [DbMap("CLAUSE_ID_")]
    public string? ClauseId { get; set; }
    [DisplayName("عبارت نام")]
    [DbMap("CLAUSE_NAME_")]
    public string? ClauseName { get; set; }
    [DisplayName("متغیر نوع")]
    [DbMap("VAR_TYPE_")]
    public string? VarType { get; set; }
    [DisplayName("داده باینری شناسه")]
    [DbMap("BYTEARRAY_ID_")]
    public string? BytearrayId { get; set; }
    [DisplayName("اعشاری")]
    [DbMap("DOUBLE_")]
    public double? Double { get; set; }
    [DisplayName("عدد صحیح بلند")]
    [DbMap("LONG_")]
    public decimal? Long { get; set; }
    [DisplayName("متن")]
    [DbMap("TEXT_")]
    public string? Text { get; set; }
    [DisplayName("متن دوم")]
    [DbMap("TEXT2_")]
    public string? Text2 { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("ایجاد زمان")]
    [DbMap("CREATE_TIME_")]
    public DateTime? CreateTime { get; set; }
    [DisplayName("ریشه فرایند نمونه شناسه")]
    [DbMap("ROOT_PROC_INST_ID_")]
    public string? RootProcInstId { get; set; }
    [DisplayName("حذف زمان")]
    [DbMap("REMOVAL_TIME_")]
    public DateTime? RemovalTime { get; set; }
}
[DisplayName("سوابق موتور: خروجی")]
[DbMap("ACT_HI_DEC_OUT")]
public sealed class SqlActHiDecOut : SqlServerEntity<string>
{
    [DisplayName("تصمیم نمونه شناسه")]
    [DbMap("DEC_INST_ID_")]
    public string DecInstId { get; set; } = null!;
    [DisplayName("عبارت شناسه")]
    [DbMap("CLAUSE_ID_")]
    public string? ClauseId { get; set; }
    [DisplayName("عبارت نام")]
    [DbMap("CLAUSE_NAME_")]
    public string? ClauseName { get; set; }
    [DisplayName("قاعده شناسه")]
    [DbMap("RULE_ID_")]
    public string? RuleId { get; set; }
    [DisplayName("قاعده سفارش")]
    [DbMap("RULE_ORDER_")]
    public int? RuleOrder { get; set; }
    [DisplayName("متغیر نام")]
    [DbMap("VAR_NAME_")]
    public string? VarName { get; set; }
    [DisplayName("متغیر نوع")]
    [DbMap("VAR_TYPE_")]
    public string? VarType { get; set; }
    [DisplayName("داده باینری شناسه")]
    [DbMap("BYTEARRAY_ID_")]
    public string? BytearrayId { get; set; }
    [DisplayName("اعشاری")]
    [DbMap("DOUBLE_")]
    public double? Double { get; set; }
    [DisplayName("عدد صحیح بلند")]
    [DbMap("LONG_")]
    public decimal? Long { get; set; }
    [DisplayName("متن")]
    [DbMap("TEXT_")]
    public string? Text { get; set; }
    [DisplayName("متن دوم")]
    [DbMap("TEXT2_")]
    public string? Text2 { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("ایجاد زمان")]
    [DbMap("CREATE_TIME_")]
    public DateTime? CreateTime { get; set; }
    [DisplayName("ریشه فرایند نمونه شناسه")]
    [DbMap("ROOT_PROC_INST_ID_")]
    public string? RootProcInstId { get; set; }
    [DisplayName("حذف زمان")]
    [DbMap("REMOVAL_TIME_")]
    public DateTime? RemovalTime { get; set; }
}
[DisplayName("سوابق موتور: نمونه تصمیم")]
[DbMap("ACT_HI_DECINST")]
public sealed class SqlActHiDecinst : SqlServerEntity<string>
{
    [DisplayName("تصمیم تعریف شناسه")]
    [DbMap("DEC_DEF_ID_")]
    public string DecDefId { get; set; } = null!;
    [DisplayName("تصمیم تعریف کلید")]
    [DbMap("DEC_DEF_KEY_")]
    public string DecDefKey { get; set; } = null!;
    [DisplayName("تصمیم تعریف نام")]
    [DbMap("DEC_DEF_NAME_")]
    public string? DecDefName { get; set; }
    [DisplayName("فرایند تعریف کلید")]
    [DbMap("PROC_DEF_KEY_")]
    public string? ProcDefKey { get; set; }
    [DisplayName("فرایند تعریف شناسه")]
    [DbMap("PROC_DEF_ID_")]
    public string? ProcDefId { get; set; }
    [DisplayName("فرایند نمونه شناسه")]
    [DbMap("PROC_INST_ID_")]
    public string? ProcInstId { get; set; }
    [DisplayName("پرونده تعریف کلید")]
    [DbMap("CASE_DEF_KEY_")]
    public string? CaseDefKey { get; set; }
    [DisplayName("پرونده تعریف شناسه")]
    [DbMap("CASE_DEF_ID_")]
    public string? CaseDefId { get; set; }
    [DisplayName("پرونده نمونه شناسه")]
    [DbMap("CASE_INST_ID_")]
    public string? CaseInstId { get; set; }
    [DisplayName("فعالیت نمونه شناسه")]
    [DbMap("ACT_INST_ID_")]
    public string? ActInstId { get; set; }
    [DisplayName("فعالیت شناسه")]
    [DbMap("ACT_ID_")]
    public string? ActId { get; set; }
    [DisplayName("ارزیابی زمان")]
    [DbMap("EVAL_TIME_")]
    public DateTime EvalTime { get; set; }
    [DisplayName("حذف زمان")]
    [DbMap("REMOVAL_TIME_")]
    public DateTime? RemovalTime { get; set; }
    [DisplayName("جمع‌آوری مقدار")]
    [DbMap("COLLECT_VALUE_")]
    public double? CollectValue { get; set; }
    [DisplayName("کاربر شناسه")]
    [DbMap("USER_ID_")]
    public string? UserId { get; set; }
    [DisplayName("ریشه تصمیم نمونه شناسه")]
    [DbMap("ROOT_DEC_INST_ID_")]
    public string? RootDecInstId { get; set; }
    [DisplayName("ریشه فرایند نمونه شناسه")]
    [DbMap("ROOT_PROC_INST_ID_")]
    public string? RootProcInstId { get; set; }
    [DisplayName("تصمیم درخواست شناسه")]
    [DbMap("DEC_REQ_ID_")]
    public string? DecReqId { get; set; }
    [DisplayName("تصمیم درخواست کلید")]
    [DbMap("DEC_REQ_KEY_")]
    public string? DecReqKey { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("سوابق موتور: تفصیلی")]
[DbMap("ACT_HI_DETAIL")]
public sealed class SqlActHiDetail : SqlServerEntity<string>
{
    [DisplayName("نوع")]
    [DbMap("TYPE_")]
    public string Type { get; set; } = null!;
    [DisplayName("فرایند تعریف کلید")]
    [DbMap("PROC_DEF_KEY_")]
    public string? ProcDefKey { get; set; }
    [DisplayName("فرایند تعریف شناسه")]
    [DbMap("PROC_DEF_ID_")]
    public string? ProcDefId { get; set; }
    [DisplayName("ریشه فرایند نمونه شناسه")]
    [DbMap("ROOT_PROC_INST_ID_")]
    public string? RootProcInstId { get; set; }
    [DisplayName("فرایند نمونه شناسه")]
    [DbMap("PROC_INST_ID_")]
    public string? ProcInstId { get; set; }
    [DisplayName("اجرا شناسه")]
    [DbMap("EXECUTION_ID_")]
    public string? ExecutionId { get; set; }
    [DisplayName("پرونده تعریف کلید")]
    [DbMap("CASE_DEF_KEY_")]
    public string? CaseDefKey { get; set; }
    [DisplayName("پرونده تعریف شناسه")]
    [DbMap("CASE_DEF_ID_")]
    public string? CaseDefId { get; set; }
    [DisplayName("پرونده نمونه شناسه")]
    [DbMap("CASE_INST_ID_")]
    public string? CaseInstId { get; set; }
    [DisplayName("پرونده اجرا شناسه")]
    [DbMap("CASE_EXECUTION_ID_")]
    public string? CaseExecutionId { get; set; }
    [DisplayName("وظیفه شناسه")]
    [DbMap("TASK_ID_")]
    public string? TaskId { get; set; }
    [DisplayName("فعالیت نمونه شناسه")]
    [DbMap("ACT_INST_ID_")]
    public string? ActInstId { get; set; }
    [DisplayName("متغیر نمونه شناسه")]
    [DbMap("VAR_INST_ID_")]
    public string? VarInstId { get; set; }
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string Name { get; set; } = null!;
    [DisplayName("متغیر نوع")]
    [DbMap("VAR_TYPE_")]
    public string? VarType { get; set; }
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int? Rev { get; set; }
    [DisplayName("زمان")]
    [DbMap("TIME_")]
    public DateTime Time { get; set; }
    [DisplayName("داده باینری شناسه")]
    [DbMap("BYTEARRAY_ID_")]
    public string? BytearrayId { get; set; }
    [DisplayName("اعشاری")]
    [DbMap("DOUBLE_")]
    public double? Double { get; set; }
    [DisplayName("عدد صحیح بلند")]
    [DbMap("LONG_")]
    public decimal? Long { get; set; }
    [DisplayName("متن")]
    [DbMap("TEXT_")]
    public string? Text { get; set; }
    [DisplayName("متن دوم")]
    [DbMap("TEXT2_")]
    public string? Text2 { get; set; }
    [DisplayName("توالی شمارنده")]
    [DbMap("SEQUENCE_COUNTER_")]
    public decimal? SequenceCounter { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("عملیات شناسه")]
    [DbMap("OPERATION_ID_")]
    public string? OperationId { get; set; }
    [DisplayName("حذف زمان")]
    [DbMap("REMOVAL_TIME_")]
    public DateTime? RemovalTime { get; set; }
    [DisplayName("اولیه")]
    [DbMap("INITIAL_")]
    public bool? Initial { get; set; }
}
[DisplayName("سوابق موتور: سوابق")]
[DbMap("ACT_HI_EXT_TASK_LOG")]
public sealed class SqlActHiExtTaskLog : SqlServerEntity<string>
{
    [DisplayName("مهر زمانی")]
    [DbMap("TIMESTAMP_")]
    public DateTime Timestamp { get; set; }
    [DisplayName("خارجی وظیفه شناسه")]
    [DbMap("EXT_TASK_ID_")]
    public string ExtTaskId { get; set; } = null!;
    [DisplayName("تلاش‌های مجدد")]
    [DbMap("RETRIES_")]
    public int? Retries { get; set; }
    [DisplayName("موضوع نام")]
    [DbMap("TOPIC_NAME_")]
    public string? TopicName { get; set; }
    [DisplayName("پردازشگر شناسه")]
    [DbMap("WORKER_ID_")]
    public string? WorkerId { get; set; }
    [DisplayName("اولویت")]
    [DbMap("PRIORITY_")]
    public decimal Priority { get; set; }
    [DisplayName("خطا پیام")]
    [DbMap("ERROR_MSG_")]
    public string? ErrorMsg { get; set; }
    [DisplayName("خطا جزئیات شناسه")]
    [DbMap("ERROR_DETAILS_ID_")]
    public string? ErrorDetailsId { get; set; }
    [DisplayName("فعالیت شناسه")]
    [DbMap("ACT_ID_")]
    public string? ActId { get; set; }
    [DisplayName("فعالیت نمونه شناسه")]
    [DbMap("ACT_INST_ID_")]
    public string? ActInstId { get; set; }
    [DisplayName("اجرا شناسه")]
    [DbMap("EXECUTION_ID_")]
    public string? ExecutionId { get; set; }
    [DisplayName("ریشه فرایند نمونه شناسه")]
    [DbMap("ROOT_PROC_INST_ID_")]
    public string? RootProcInstId { get; set; }
    [DisplayName("فرایند نمونه شناسه")]
    [DbMap("PROC_INST_ID_")]
    public string? ProcInstId { get; set; }
    [DisplayName("فرایند تعریف شناسه")]
    [DbMap("PROC_DEF_ID_")]
    public string? ProcDefId { get; set; }
    [DisplayName("فرایند تعریف کلید")]
    [DbMap("PROC_DEF_KEY_")]
    public string? ProcDefKey { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("وضعیت")]
    [DbMap("STATE_")]
    public int? State { get; set; }
    [DisplayName("حذف زمان")]
    [DbMap("REMOVAL_TIME_")]
    public DateTime? RemovalTime { get; set; }
}
[DisplayName("سوابق موتور: پیوند هویت")]
[DbMap("ACT_HI_IDENTITYLINK")]
public sealed class SqlActHiIdentitylink : SqlServerEntity<string>
{
    [DisplayName("مهر زمانی")]
    [DbMap("TIMESTAMP_")]
    public DateTime Timestamp { get; set; }
    [DisplayName("نوع")]
    [DbMap("TYPE_")]
    public string? Type { get; set; }
    [DisplayName("کاربر شناسه")]
    [DbMap("USER_ID_")]
    public string? UserId { get; set; }
    [DisplayName("گروه شناسه")]
    [DbMap("GROUP_ID_")]
    public string? GroupId { get; set; }
    [DisplayName("وظیفه شناسه")]
    [DbMap("TASK_ID_")]
    public string? TaskId { get; set; }
    [DisplayName("ریشه فرایند نمونه شناسه")]
    [DbMap("ROOT_PROC_INST_ID_")]
    public string? RootProcInstId { get; set; }
    [DisplayName("فرایند تعریف شناسه")]
    [DbMap("PROC_DEF_ID_")]
    public string? ProcDefId { get; set; }
    [DisplayName("عملیات نوع")]
    [DbMap("OPERATION_TYPE_")]
    public string? OperationType { get; set; }
    [DisplayName("واگذارکننده شناسه")]
    [DbMap("ASSIGNER_ID_")]
    public string? AssignerId { get; set; }
    [DisplayName("فرایند تعریف کلید")]
    [DbMap("PROC_DEF_KEY_")]
    public string? ProcDefKey { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("حذف زمان")]
    [DbMap("REMOVAL_TIME_")]
    public DateTime? RemovalTime { get; set; }
}
[DisplayName("سوابق موتور: رخداد خطا")]
[DbMap("ACT_HI_INCIDENT")]
public sealed class SqlActHiIncident : SqlServerEntity<string>
{
    [DisplayName("فرایند تعریف کلید")]
    [DbMap("PROC_DEF_KEY_")]
    public string? ProcDefKey { get; set; }
    [DisplayName("فرایند تعریف شناسه")]
    [DbMap("PROC_DEF_ID_")]
    public string? ProcDefId { get; set; }
    [DisplayName("ریشه فرایند نمونه شناسه")]
    [DbMap("ROOT_PROC_INST_ID_")]
    public string? RootProcInstId { get; set; }
    [DisplayName("فرایند نمونه شناسه")]
    [DbMap("PROC_INST_ID_")]
    public string? ProcInstId { get; set; }
    [DisplayName("اجرا شناسه")]
    [DbMap("EXECUTION_ID_")]
    public string? ExecutionId { get; set; }
    [DisplayName("ایجاد زمان")]
    [DbMap("CREATE_TIME_")]
    public DateTime CreateTime { get; set; }
    [DisplayName("پایان زمان")]
    [DbMap("END_TIME_")]
    public DateTime? EndTime { get; set; }
    [DisplayName("رخداد خطا پیام")]
    [DbMap("INCIDENT_MSG_")]
    public string? IncidentMsg { get; set; }
    [DisplayName("رخداد خطا نوع")]
    [DbMap("INCIDENT_TYPE_")]
    public string IncidentType { get; set; } = null!;
    [DisplayName("فعالیت شناسه")]
    [DbMap("ACTIVITY_ID_")]
    public string? ActivityId { get; set; }
    [DisplayName("ناموفق فعالیت شناسه")]
    [DbMap("FAILED_ACTIVITY_ID_")]
    public string? FailedActivityId { get; set; }
    [DisplayName("علت رخداد خطا شناسه")]
    [DbMap("CAUSE_INCIDENT_ID_")]
    public string? CauseIncidentId { get; set; }
    [DisplayName("ریشه علت رخداد خطا شناسه")]
    [DbMap("ROOT_CAUSE_INCIDENT_ID_")]
    public string? RootCauseIncidentId { get; set; }
    [DisplayName("پیکربندی")]
    [DbMap("CONFIGURATION_")]
    public string? Configuration { get; set; }
    [DisplayName("تاریخچه پیکربندی")]
    [DbMap("HISTORY_CONFIGURATION_")]
    public string? HistoryConfiguration { get; set; }
    [DisplayName("رخداد خطا وضعیت")]
    [DbMap("INCIDENT_STATE_")]
    public int? IncidentState { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("کار پس‌زمینه تعریف شناسه")]
    [DbMap("JOB_DEF_ID_")]
    public string? JobDefId { get; set; }
    [DisplayName("یادداشت")]
    [DbMap("ANNOTATION_")]
    public string? Annotation { get; set; }
    [DisplayName("حذف زمان")]
    [DbMap("REMOVAL_TIME_")]
    public DateTime? RemovalTime { get; set; }
}
[DisplayName("سوابق موتور: سوابق")]
[DbMap("ACT_HI_JOB_LOG")]
public sealed class SqlActHiJobLog : SqlServerEntity<string>
{
    [DisplayName("مهر زمانی")]
    [DbMap("TIMESTAMP_")]
    public DateTime Timestamp { get; set; }
    [DisplayName("کار پس‌زمینه شناسه")]
    [DbMap("JOB_ID_")]
    public string JobId { get; set; } = null!;
    [DisplayName("کار پس‌زمینه تاریخ سررسید")]
    [DbMap("JOB_DUEDATE_")]
    public DateTime? JobDuedate { get; set; }
    [DisplayName("کار پس‌زمینه تلاش‌های مجدد")]
    [DbMap("JOB_RETRIES_")]
    public int? JobRetries { get; set; }
    [DisplayName("کار پس‌زمینه اولویت")]
    [DbMap("JOB_PRIORITY_")]
    public decimal JobPriority { get; set; }
    [DisplayName("کار پس‌زمینه استثنا پیام")]
    [DbMap("JOB_EXCEPTION_MSG_")]
    public string? JobExceptionMsg { get; set; }
    [DisplayName("کار پس‌زمینه استثنا پشته شناسه")]
    [DbMap("JOB_EXCEPTION_STACK_ID_")]
    public string? JobExceptionStackId { get; set; }
    [DisplayName("کار پس‌زمینه وضعیت")]
    [DbMap("JOB_STATE_")]
    public int? JobState { get; set; }
    [DisplayName("کار پس‌زمینه تعریف شناسه")]
    [DbMap("JOB_DEF_ID_")]
    public string? JobDefId { get; set; }
    [DisplayName("کار پس‌زمینه تعریف نوع")]
    [DbMap("JOB_DEF_TYPE_")]
    public string? JobDefType { get; set; }
    [DisplayName("کار پس‌زمینه تعریف پیکربندی")]
    [DbMap("JOB_DEF_CONFIGURATION_")]
    public string? JobDefConfiguration { get; set; }
    [DisplayName("فعالیت شناسه")]
    [DbMap("ACT_ID_")]
    public string? ActId { get; set; }
    [DisplayName("ناموفق فعالیت شناسه")]
    [DbMap("FAILED_ACT_ID_")]
    public string? FailedActId { get; set; }
    [DisplayName("اجرا شناسه")]
    [DbMap("EXECUTION_ID_")]
    public string? ExecutionId { get; set; }
    [DisplayName("ریشه فرایند نمونه شناسه")]
    [DbMap("ROOT_PROC_INST_ID_")]
    public string? RootProcInstId { get; set; }
    [DisplayName("فرایند نمونه شناسه")]
    [DbMap("PROCESS_INSTANCE_ID_")]
    public string? ProcessInstanceId { get; set; }
    [DisplayName("فرایند تعریف شناسه")]
    [DbMap("PROCESS_DEF_ID_")]
    public string? ProcessDefId { get; set; }
    [DisplayName("فرایند تعریف کلید")]
    [DbMap("PROCESS_DEF_KEY_")]
    public string? ProcessDefKey { get; set; }
    [DisplayName("استقرار شناسه")]
    [DbMap("DEPLOYMENT_ID_")]
    public string? DeploymentId { get; set; }
    [DisplayName("توالی شمارنده")]
    [DbMap("SEQUENCE_COUNTER_")]
    public decimal? SequenceCounter { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("نام میزبان")]
    [DbMap("HOSTNAME_")]
    public string? Hostname { get; set; }
    [DisplayName("حذف زمان")]
    [DbMap("REMOVAL_TIME_")]
    public DateTime? RemovalTime { get; set; }
    [DisplayName("دسته شناسه")]
    [DbMap("BATCH_ID_")]
    public string? BatchId { get; set; }
}
[DisplayName("سوابق موتور: سوابق")]
[DbMap("ACT_HI_OP_LOG")]
public sealed class SqlActHiOpLog : SqlServerEntity<string>
{
    [DisplayName("استقرار شناسه")]
    [DbMap("DEPLOYMENT_ID_")]
    public string? DeploymentId { get; set; }
    [DisplayName("فرایند تعریف شناسه")]
    [DbMap("PROC_DEF_ID_")]
    public string? ProcDefId { get; set; }
    [DisplayName("فرایند تعریف کلید")]
    [DbMap("PROC_DEF_KEY_")]
    public string? ProcDefKey { get; set; }
    [DisplayName("ریشه فرایند نمونه شناسه")]
    [DbMap("ROOT_PROC_INST_ID_")]
    public string? RootProcInstId { get; set; }
    [DisplayName("فرایند نمونه شناسه")]
    [DbMap("PROC_INST_ID_")]
    public string? ProcInstId { get; set; }
    [DisplayName("اجرا شناسه")]
    [DbMap("EXECUTION_ID_")]
    public string? ExecutionId { get; set; }
    [DisplayName("پرونده تعریف شناسه")]
    [DbMap("CASE_DEF_ID_")]
    public string? CaseDefId { get; set; }
    [DisplayName("پرونده نمونه شناسه")]
    [DbMap("CASE_INST_ID_")]
    public string? CaseInstId { get; set; }
    [DisplayName("پرونده اجرا شناسه")]
    [DbMap("CASE_EXECUTION_ID_")]
    public string? CaseExecutionId { get; set; }
    [DisplayName("وظیفه شناسه")]
    [DbMap("TASK_ID_")]
    public string? TaskId { get; set; }
    [DisplayName("کار پس‌زمینه شناسه")]
    [DbMap("JOB_ID_")]
    public string? JobId { get; set; }
    [DisplayName("کار پس‌زمینه تعریف شناسه")]
    [DbMap("JOB_DEF_ID_")]
    public string? JobDefId { get; set; }
    [DisplayName("دسته شناسه")]
    [DbMap("BATCH_ID_")]
    public string? BatchId { get; set; }
    [DisplayName("کاربر شناسه")]
    [DbMap("USER_ID_")]
    public string? UserId { get; set; }
    [DisplayName("مهر زمانی")]
    [DbMap("TIMESTAMP_")]
    public DateTime Timestamp { get; set; }
    [DisplayName("عملیات نوع")]
    [DbMap("OPERATION_TYPE_")]
    public string? OperationType { get; set; }
    [DisplayName("عملیات شناسه")]
    [DbMap("OPERATION_ID_")]
    public string? OperationId { get; set; }
    [DisplayName("موجودیت نوع")]
    [DbMap("ENTITY_TYPE_")]
    public string? EntityType { get; set; }
    [DisplayName("ویژگی")]
    [DbMap("PROPERTY_")]
    public string? Property { get; set; }
    [DisplayName("سازمان مقدار")]
    [DbMap("ORG_VALUE_")]
    public string? OrgValue { get; set; }
    [DisplayName("جدید مقدار")]
    [DbMap("NEW_VALUE_")]
    public string? NewValue { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("حذف زمان")]
    [DbMap("REMOVAL_TIME_")]
    public DateTime? RemovalTime { get; set; }
    [DisplayName("دسته‌بندی")]
    [DbMap("CATEGORY_")]
    public string? Category { get; set; }
    [DisplayName("خارجی وظیفه شناسه")]
    [DbMap("EXTERNAL_TASK_ID_")]
    public string? ExternalTaskId { get; set; }
    [DisplayName("یادداشت")]
    [DbMap("ANNOTATION_")]
    public string? Annotation { get; set; }
}
[DisplayName("سوابق موتور: نمونه فرایند")]
[DbMap("ACT_HI_PROCINST")]
public sealed class SqlActHiProcinst : SqlServerEntity<string>
{
    [DisplayName("فرایند نمونه شناسه")]
    [DbMap("PROC_INST_ID_")]
    public string ProcInstId { get; set; } = null!;
    [DisplayName("کسب‌وکار کلید")]
    [DbMap("BUSINESS_KEY_")]
    public string? BusinessKey { get; set; }
    [DisplayName("فرایند تعریف کلید")]
    [DbMap("PROC_DEF_KEY_")]
    public string? ProcDefKey { get; set; }
    [DisplayName("فرایند تعریف شناسه")]
    [DbMap("PROC_DEF_ID_")]
    public string ProcDefId { get; set; } = null!;
    [DisplayName("شروع زمان")]
    [DbMap("START_TIME_")]
    public DateTime StartTime { get; set; }
    [DisplayName("پایان زمان")]
    [DbMap("END_TIME_")]
    public DateTime? EndTime { get; set; }
    [DisplayName("حذف زمان")]
    [DbMap("REMOVAL_TIME_")]
    public DateTime? RemovalTime { get; set; }
    [DisplayName("مدت")]
    [DbMap("DURATION_")]
    public decimal? Duration { get; set; }
    [DisplayName("شروع کاربر شناسه")]
    [DbMap("START_USER_ID_")]
    public string? StartUserId { get; set; }
    [DisplayName("شروع فعالیت شناسه")]
    [DbMap("START_ACT_ID_")]
    public string? StartActId { get; set; }
    [DisplayName("پایان فعالیت شناسه")]
    [DbMap("END_ACT_ID_")]
    public string? EndActId { get; set; }
    [DisplayName("بالادست فرایند نمونه شناسه")]
    [DbMap("SUPER_PROCESS_INSTANCE_ID_")]
    public string? SuperProcessInstanceId { get; set; }
    [DisplayName("ریشه فرایند نمونه شناسه")]
    [DbMap("ROOT_PROC_INST_ID_")]
    public string? RootProcInstId { get; set; }
    [DisplayName("بالادست پرونده نمونه شناسه")]
    [DbMap("SUPER_CASE_INSTANCE_ID_")]
    public string? SuperCaseInstanceId { get; set; }
    [DisplayName("پرونده نمونه شناسه")]
    [DbMap("CASE_INST_ID_")]
    public string? CaseInstId { get; set; }
    [DisplayName("حذف دلیل")]
    [DbMap("DELETE_REASON_")]
    public string? DeleteReason { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("وضعیت")]
    [DbMap("STATE_")]
    public string? State { get; set; }
    [DisplayName("راه‌اندازی مجدد فرایند نمونه شناسه")]
    [DbMap("RESTARTED_PROC_INST_ID_")]
    public string? RestartedProcInstId { get; set; }
}
[DisplayName("سوابق موتور: نمونه وظیفه")]
[DbMap("ACT_HI_TASKINST")]
public sealed class SqlActHiTaskinst : SqlServerEntity<string>
{
    [DisplayName("وظیفه تعریف کلید")]
    [DbMap("TASK_DEF_KEY_")]
    public string? TaskDefKey { get; set; }
    [DisplayName("فرایند تعریف کلید")]
    [DbMap("PROC_DEF_KEY_")]
    public string? ProcDefKey { get; set; }
    [DisplayName("فرایند تعریف شناسه")]
    [DbMap("PROC_DEF_ID_")]
    public string? ProcDefId { get; set; }
    [DisplayName("ریشه فرایند نمونه شناسه")]
    [DbMap("ROOT_PROC_INST_ID_")]
    public string? RootProcInstId { get; set; }
    [DisplayName("فرایند نمونه شناسه")]
    [DbMap("PROC_INST_ID_")]
    public string? ProcInstId { get; set; }
    [DisplayName("اجرا شناسه")]
    [DbMap("EXECUTION_ID_")]
    public string? ExecutionId { get; set; }
    [DisplayName("پرونده تعریف کلید")]
    [DbMap("CASE_DEF_KEY_")]
    public string? CaseDefKey { get; set; }
    [DisplayName("پرونده تعریف شناسه")]
    [DbMap("CASE_DEF_ID_")]
    public string? CaseDefId { get; set; }
    [DisplayName("پرونده نمونه شناسه")]
    [DbMap("CASE_INST_ID_")]
    public string? CaseInstId { get; set; }
    [DisplayName("پرونده اجرا شناسه")]
    [DbMap("CASE_EXECUTION_ID_")]
    public string? CaseExecutionId { get; set; }
    [DisplayName("فعالیت نمونه شناسه")]
    [DbMap("ACT_INST_ID_")]
    public string? ActInstId { get; set; }
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string? Name { get; set; }
    [DisplayName("والد وظیفه شناسه")]
    [DbMap("PARENT_TASK_ID_")]
    public string? ParentTaskId { get; set; }
    [DisplayName("توضیحات")]
    [DbMap("DESCRIPTION_")]
    public string? Description { get; set; }
    [DisplayName("مالک")]
    [DbMap("OWNER_")]
    public string? Owner { get; set; }
    [DisplayName("مسئول")]
    [DbMap("ASSIGNEE_")]
    public string? Assignee { get; set; }
    [DisplayName("شروع زمان")]
    [DbMap("START_TIME_")]
    public DateTime StartTime { get; set; }
    [DisplayName("پایان زمان")]
    [DbMap("END_TIME_")]
    public DateTime? EndTime { get; set; }
    [DisplayName("مدت")]
    [DbMap("DURATION_")]
    public decimal? Duration { get; set; }
    [DisplayName("حذف دلیل")]
    [DbMap("DELETE_REASON_")]
    public string? DeleteReason { get; set; }
    [DisplayName("اولویت")]
    [DbMap("PRIORITY_")]
    public int? Priority { get; set; }
    [DisplayName("سررسید تاریخ")]
    [DbMap("DUE_DATE_")]
    public DateTime? DueDate { get; set; }
    [DisplayName("پیگیری بعدی تاریخ")]
    [DbMap("FOLLOW_UP_DATE_")]
    public DateTime? FollowUpDate { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("حذف زمان")]
    [DbMap("REMOVAL_TIME_")]
    public DateTime? RemovalTime { get; set; }
    [DisplayName("وظیفه وضعیت")]
    [DbMap("TASK_STATE_")]
    public string? TaskState { get; set; }
}
[DisplayName("سوابق موتور: نمونه متغیر")]
[DbMap("ACT_HI_VARINST")]
public sealed class SqlActHiVarinst : SqlServerEntity<string>
{
    [DisplayName("فرایند تعریف کلید")]
    [DbMap("PROC_DEF_KEY_")]
    public string? ProcDefKey { get; set; }
    [DisplayName("فرایند تعریف شناسه")]
    [DbMap("PROC_DEF_ID_")]
    public string? ProcDefId { get; set; }
    [DisplayName("ریشه فرایند نمونه شناسه")]
    [DbMap("ROOT_PROC_INST_ID_")]
    public string? RootProcInstId { get; set; }
    [DisplayName("فرایند نمونه شناسه")]
    [DbMap("PROC_INST_ID_")]
    public string? ProcInstId { get; set; }
    [DisplayName("اجرا شناسه")]
    [DbMap("EXECUTION_ID_")]
    public string? ExecutionId { get; set; }
    [DisplayName("پرونده تعریف کلید")]
    [DbMap("CASE_DEF_KEY_")]
    public string? CaseDefKey { get; set; }
    [DisplayName("پرونده تعریف شناسه")]
    [DbMap("CASE_DEF_ID_")]
    public string? CaseDefId { get; set; }
    [DisplayName("پرونده نمونه شناسه")]
    [DbMap("CASE_INST_ID_")]
    public string? CaseInstId { get; set; }
    [DisplayName("پرونده اجرا شناسه")]
    [DbMap("CASE_EXECUTION_ID_")]
    public string? CaseExecutionId { get; set; }
    [DisplayName("فعالیت نمونه شناسه")]
    [DbMap("ACT_INST_ID_")]
    public string? ActInstId { get; set; }
    [DisplayName("وظیفه شناسه")]
    [DbMap("TASK_ID_")]
    public string? TaskId { get; set; }
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string Name { get; set; } = null!;
    [DisplayName("متغیر نوع")]
    [DbMap("VAR_TYPE_")]
    public string? VarType { get; set; }
    [DisplayName("ایجاد زمان")]
    [DbMap("CREATE_TIME_")]
    public DateTime? CreateTime { get; set; }
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int? Rev { get; set; }
    [DisplayName("داده باینری شناسه")]
    [DbMap("BYTEARRAY_ID_")]
    public string? BytearrayId { get; set; }
    [DisplayName("اعشاری")]
    [DbMap("DOUBLE_")]
    public double? Double { get; set; }
    [DisplayName("عدد صحیح بلند")]
    [DbMap("LONG_")]
    public decimal? Long { get; set; }
    [DisplayName("متن")]
    [DbMap("TEXT_")]
    public string? Text { get; set; }
    [DisplayName("متن دوم")]
    [DbMap("TEXT2_")]
    public string? Text2 { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("وضعیت")]
    [DbMap("STATE_")]
    public string? State { get; set; }
    [DisplayName("حذف زمان")]
    [DbMap("REMOVAL_TIME_")]
    public DateTime? RemovalTime { get; set; }
}
[DisplayName("موتور: گروه")]
[DbMap("ACT_ID_GROUP")]
public sealed class SqlActIdGroup : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int? Rev { get; set; }
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string? Name { get; set; }
    [DisplayName("نوع")]
    [DbMap("TYPE_")]
    public string? Type { get; set; }
}
[DisplayName("موتور: اطلاعات")]
[DbMap("ACT_ID_INFO")]
public sealed class SqlActIdInfo : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int? Rev { get; set; }
    [DisplayName("کاربر شناسه")]
    [DbMap("USER_ID_")]
    public string? UserId { get; set; }
    [DisplayName("نوع")]
    [DbMap("TYPE_")]
    public string? Type { get; set; }
    [DisplayName("کلید")]
    [DbMap("KEY_")]
    public string? Key { get; set; }
    [DisplayName("مقدار")]
    [DbMap("VALUE_")]
    public string? Value { get; set; }
    [DisplayName("رمز عبور")]
    [DbMap("PASSWORD_")]
    public byte[]? Password { get; set; }
    [DisplayName("والد شناسه")]
    [DbMap("PARENT_ID_")]
    public string? ParentId { get; set; }
}
[DisplayName("موتور: عضویت")]
[DbMap("ACT_ID_MEMBERSHIP")]
public sealed class SqlActIdMembership : SqlServerEntity
{
    [DisplayName("کاربر شناسه")]
    [DbMap("USER_ID_")]
    public string UserId { get; set; } = null!;
    [DisplayName("گروه شناسه")]
    [DbMap("GROUP_ID_")]
    public string GroupId { get; set; } = null!;
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("هش")]
    [DbMap("HASH_")]
    public string? Hash { get; set; }
    [DisplayName("حذف زمان")]
    [DbMap("REMOVAL_TIME_")]
    public DateTime? RemovalTime { get; set; }
}
[DisplayName("موتور: من")]
[DbMap("ACT_ID_REMEMBER_ME")]
public sealed class SqlActIdRememberMe : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int Rev { get; set; }
    [DisplayName("انتخابگر")]
    [DbMap("SELECTOR_")]
    public string Selector { get; set; } = null!;
    [DisplayName("اعتبارسنج")]
    [DbMap("VALIDATOR_")]
    public string Validator { get; set; } = null!;
    [DisplayName("کاربر شناسه")]
    [DbMap("USER_ID_")]
    public string UserId { get; set; } = null!;
    [DisplayName("آخرین مصرف‌شده")]
    [DbMap("LAST_USED_")]
    public DateTime LastUsed { get; set; }
}
[DisplayName("موتور: مستاجر")]
[DbMap("ACT_ID_TENANT")]
public sealed class SqlActIdTenant : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int? Rev { get; set; }
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string? Name { get; set; }
    [DisplayName("والد شناسه")]
    [DbMap("PARENT_ID_")]
    public string? ParentId { get; set; }
}
[DisplayName("موتور: عضو")]
[DbMap("ACT_ID_TENANT_MEMBER")]
public sealed class SqlActIdTenantMember : SqlServerEntity<string>
{
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string TenantId { get; set; } = null!;
    [DisplayName("کاربر شناسه")]
    [DbMap("USER_ID_")]
    public string? UserId { get; set; }
    [DisplayName("گروه شناسه")]
    [DbMap("GROUP_ID_")]
    public string? GroupId { get; set; }
    [DisplayName("حذف زمان")]
    [DbMap("REMOVAL_TIME_")]
    public DateTime? RemovalTime { get; set; }
}
[DisplayName("موتور: کاربر")]
[DbMap("ACT_ID_USER")]
public sealed class SqlActIdUser : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int? Rev { get; set; }
    [DisplayName("اول")]
    [DbMap("FIRST_")]
    public string? First { get; set; }
    [DisplayName("آخرین")]
    [DbMap("LAST_")]
    public string? Last { get; set; }
    [DisplayName("رایانامه")]
    [DbMap("EMAIL_")]
    public string? Email { get; set; }
    [DisplayName("رمز عبور")]
    [DbMap("PWD_")]
    public string? Pwd { get; set; }
    [DisplayName("نمک رمزنگاری")]
    [DbMap("SALT_")]
    public string? Salt { get; set; }
    [DisplayName("قفل انقضا زمان")]
    [DbMap("LOCK_EXP_TIME_")]
    public DateTime? LockExpTime { get; set; }
    [DisplayName("تلاش‌ها")]
    [DbMap("ATTEMPTS_")]
    public int? Attempts { get; set; }
    [DisplayName("تصویر شناسه")]
    [DbMap("PICTURE_ID_")]
    public string? PictureId { get; set; }
    [DisplayName("تلفن همراه")]
    [DbMap("MOBILE_")]
    public string? Mobile { get; set; }
    [DisplayName("ایجاد زمان")]
    [DbMap("CREATE_TIME_")]
    public DateTime CreateTime { get; set; }
    [DisplayName("به‌روزرسانی زمان")]
    [DbMap("UPDATE_TIME_")]
    public DateTime? UpdateTime { get; set; }
    [DisplayName("بلوک")]
    [DbMap("BLOCK_")]
    public bool Block { get; set; }
    [DisplayName("نام کامل")]
    [DbMap("FULLNAME_")]
    public string Fullname { get; set; } = null!;
    [DisplayName("هش")]
    [DbMap("HASH_")]
    public string? Hash { get; set; }
}
[DisplayName("موتور: ارتباط")]
[DbMap("ACT_RE_ASSOCIATION")]
public sealed class SqlActReAssociation : SqlServerEntity
{
    [DisplayName("شناسه")]
    [DbMap("ID_")]
    public string Id { get; set; } = null!;
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string Name { get; set; } = null!;
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int Rev { get; set; }
    [DisplayName("نوع")]
    [DbMap("TYPE_")]
    public string Type { get; set; } = null!;
    [DisplayName("جدول شناسه")]
    [DbMap("TABLE_ID_")]
    public string TableId { get; set; } = null!;
    [DisplayName("ورودی نام")]
    [DbMap("INCOMING_NAME_")]
    public string? IncomingName { get; set; }
    [DisplayName("خروجی نام")]
    [DbMap("OUTCOMING_NAME_")]
    public string? OutcomingName { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("موتور: تعریف فرم موتور")]
[DbMap("ACT_RE_CAMFORMDEF")]
public sealed class SqlActReCamformdef : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int? Rev { get; set; }
    [DisplayName("کلید")]
    [DbMap("KEY_")]
    public string Key { get; set; } = null!;
    [DisplayName("نسخه")]
    [DbMap("VERSION_")]
    public int Version { get; set; }
    [DisplayName("استقرار شناسه")]
    [DbMap("DEPLOYMENT_ID_")]
    public string? DeploymentId { get; set; }
    [DisplayName("منبع نام")]
    [DbMap("RESOURCE_NAME_")]
    public string? ResourceName { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("موتور: تعریف")]
[DbMap("ACT_RE_CASE_DEF")]
public sealed class SqlActReCaseDef : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int? Rev { get; set; }
    [DisplayName("دسته‌بندی")]
    [DbMap("CATEGORY_")]
    public string? Category { get; set; }
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string? Name { get; set; }
    [DisplayName("کلید")]
    [DbMap("KEY_")]
    public string Key { get; set; } = null!;
    [DisplayName("نسخه")]
    [DbMap("VERSION_")]
    public int Version { get; set; }
    [DisplayName("استقرار شناسه")]
    [DbMap("DEPLOYMENT_ID_")]
    public string? DeploymentId { get; set; }
    [DisplayName("منبع نام")]
    [DbMap("RESOURCE_NAME_")]
    public string? ResourceName { get; set; }
    [DisplayName("نمودار منبع نام")]
    [DbMap("DGRM_RESOURCE_NAME_")]
    public string? DgrmResourceName { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("تاریخچه مدت نگهداری")]
    [DbMap("HISTORY_TTL_")]
    public int? HistoryTtl { get; set; }
}
[DisplayName("موتور: ستون")]
[DbMap("ACT_RE_COLUMN")]
public sealed class SqlActReColumn : SqlServerEntity
{
    [DisplayName("شناسه")]
    [DbMap("ID_")]
    public string Id { get; set; } = null!;
    [DisplayName("قبلی شناسه")]
    [DbMap("OLD_ID_")]
    public string? OldId { get; set; }
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int Rev { get; set; }
    [DisplayName("ترتیب")]
    [DbMap("INDEX_")]
    public int? Index { get; set; }
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string Name { get; set; } = null!;
    [DisplayName("تهی‌پذیر")]
    [DbMap("NULLABLE_")]
    public bool Nullable { get; set; }
    [DisplayName("نوع")]
    [DbMap("TYPE_")]
    public string Type { get; set; } = null!;
    [DisplayName("کلید")]
    [DbMap("KEY_")]
    public string Key { get; set; } = null!;
    [DisplayName("نرمال‌سازی")]
    [DbMap("NORMALIZE_")]
    public bool? Normalize { get; set; }
    [DisplayName("جزئیات")]
    [DbMap("DETAILS_")]
    public string? Details { get; set; }
    [DisplayName("جدول شناسه")]
    [DbMap("TABLE_ID_")]
    public string TableId { get; set; } = null!;
    [DisplayName("پیش‌فرض")]
    [DbMap("DEFAULT_")]
    public bool Default { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("نوع")]
    [DbMap("KIND_")]
    public int Kind { get; set; }
    [DisplayName("گریز اچ‌تی‌ام‌ال")]
    [DbMap("ESCAPE_HTML_")]
    public bool? EscapeHtml { get; set; }
}
[DisplayName("موتور: محدودیت")]
[DbMap("ACT_RE_CONSTRAINT")]
public sealed class SqlActReConstraint : SqlServerEntity<string>
{
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string Name { get; set; } = null!;
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int Rev { get; set; }
    [DisplayName("نوع")]
    [DbMap("TYPE_")]
    public string Type { get; set; } = null!;
    [DisplayName("جدول شناسه")]
    [DbMap("TABLE_ID_")]
    public string TableId { get; set; } = null!;
    [DisplayName("ستون شناسه")]
    [DbMap("COLUMN_ID_")]
    public string ColumnId { get; set; } = null!;
    [DisplayName("والد جدول شناسه")]
    [DbMap("PARENT_TABLE_ID_")]
    public string? ParentTableId { get; set; }
    [DisplayName("والد ستون شناسه")]
    [DbMap("PARENT_COLUMN_ID_")]
    public string? ParentColumnId { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("به‌روزرسانی قاعده")]
    [DbMap("UPDATE_RULE_")]
    public string? UpdateRule { get; set; }
    [DisplayName("حذف قاعده")]
    [DbMap("DELETE_RULE_")]
    public string? DeleteRule { get; set; }
    [DisplayName("اس‌کیوال اسکریپت")]
    [DbMap("SQL_SCRIPT_")]
    public string? SqlScript { get; set; }
    [DisplayName("خطا پیام")]
    [DbMap("ERROR_MESSAGE_")]
    public string? ErrorMessage { get; set; }
}
[DisplayName("موتور: داشبورد")]
[DbMap("ACT_RE_DASHBOARD")]
public sealed class SqlActReDashboard : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int Rev { get; set; }
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string Name { get; set; } = null!;
    [DisplayName("دسته‌بندی")]
    [DbMap("CATEGORY_")]
    public string? Category { get; set; }
    [DisplayName("تعریف")]
    [DbMap("DEFINITION_")]
    public string? Definition { get; set; }
    [DisplayName("پیش‌فرض")]
    [DbMap("DEFAULT_")]
    public bool Default { get; set; }
    [DisplayName("ایجاد زمان")]
    [DbMap("CREATE_TIME_")]
    public DateTime CreateTime { get; set; }
    [DisplayName("به‌روزرسانی زمان")]
    [DbMap("UPDATE_TIME_")]
    public DateTime? UpdateTime { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("موتور: فرم")]
[DbMap("ACT_RE_DATA_FORM")]
public sealed class SqlActReDataForm : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int Rev { get; set; }
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string Name { get; set; } = null!;
    [DisplayName("دسته‌بندی")]
    [DbMap("CATEGORY_")]
    public string? Category { get; set; }
    [DisplayName("جدول شناسه")]
    [DbMap("TABLE_ID_")]
    public string TableId { get; set; } = null!;
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("سفارشی راهنما")]
    [DbMap("CUSTOM_HINTS_")]
    public string? CustomHints { get; set; }
    [DisplayName("یکسان‌سازی")]
    [DbMap("SYNC_")]
    public bool Sync { get; set; }
}
[DisplayName("موتور: ستون")]
[DbMap("ACT_RE_DATA_FORM_COLUMN")]
public sealed class SqlActReDataFormColumn : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int Rev { get; set; }
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string? Name { get; set; }
    [DisplayName("نوع")]
    [DbMap("TYPE_")]
    public int Type { get; set; }
    [DisplayName("سفارش")]
    [DbMap("ORDER_")]
    public double Order { get; set; }
    [DisplayName("نمایان")]
    [DbMap("VISIBLE_")]
    public bool? Visible { get; set; }
    [DisplayName("قابل جمع")]
    [DbMap("SUMMABLE_")]
    public bool? Summable { get; set; }
    [DisplayName("قابل فیلتر")]
    [DbMap("FILTERABLE_")]
    public bool? Filterable { get; set; }
    [DisplayName("قابل پیشنهاد")]
    [DbMap("SUGGESTABLE_")]
    public bool? Suggestable { get; set; }
    [DisplayName("دسته‌بندی")]
    [DbMap("CATEGORY_")]
    public string? Category { get; set; }
    [DisplayName("نمایش نوع")]
    [DbMap("SHOW_TYPE_")]
    public int? ShowType { get; set; }
    [DisplayName("عملیات نوع")]
    [DbMap("ACTION_TYPE_")]
    public int? ActionType { get; set; }
    [DisplayName("داده فرم شناسه")]
    [DbMap("DATA_FORM_ID_")]
    public string DataFormId { get; set; } = null!;
    [DisplayName("مرجع جدول شناسه")]
    [DbMap("REF_TABLE_ID_")]
    public string? RefTableId { get; set; }
    [DisplayName("مرجع ستون شناسه")]
    [DbMap("REF_COLUMN_ID_")]
    public string? RefColumnId { get; set; }
    [DisplayName("مرجع فرم شناسه")]
    [DbMap("REF_FORM_ID_")]
    public string? RefFormId { get; set; }
    [DisplayName("مرجع فرم روابط")]
    [DbMap("REF_FORM_RELATIONS_")]
    public string? RefFormRelations { get; set; }
    [DisplayName("مرجع داده فرم شناسه")]
    [DbMap("REF_DATA_FORM_ID_")]
    public string? RefDataFormId { get; set; }
    [DisplayName("مرجع ارتباط جدول شناسه")]
    [DbMap("REF_ASSOCIATION_TABLE_ID_")]
    public string? RefAssociationTableId { get; set; }
    [DisplayName("مرجع ارتباط شناسه")]
    [DbMap("REF_ASSOCIATION_ID_")]
    public string? RefAssociationId { get; set; }
    [DisplayName("زنجیره ارتباط‌ها")]
    [DbMap("CHAIN_ASSOCIATIONS_")]
    public string? ChainAssociations { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("داده قالب")]
    [DbMap("DATA_FORMAT_")]
    public string? DataFormat { get; set; }
    [DisplayName("اس‌کیوال اسکریپت")]
    [DbMap("SQL_SCRIPT_")]
    public string? SqlScript { get; set; }
    [DisplayName("اس‌کیوال نتیجه نوع")]
    [DbMap("SQL_RESULT_TYPE_")]
    public string? SqlResultType { get; set; }
    [DisplayName("پیشنهاد جزئیات")]
    [DbMap("SUGGEST_DETAILS_")]
    public string? SuggestDetails { get; set; }
    [DisplayName("عملیات جزئیات")]
    [DbMap("OPERATION_DETAILS_")]
    public string? OperationDetails { get; set; }
    [DisplayName("خروجی کامل داده")]
    [DbMap("OUT_FULL_DATA_")]
    public bool? OutFullData { get; set; }
    [DisplayName("فقط‌خواندنی روابط")]
    [DbMap("READONLY_RELATIONS_")]
    public string? ReadonlyRelations { get; set; }
    [DisplayName("قابل مرتب‌سازی")]
    [DbMap("SORTABLE_")]
    public bool? Sortable { get; set; }
    [DisplayName("آیکون")]
    [DbMap("ICON_")]
    public string? Icon { get; set; }
    [DisplayName("قابل صدور")]
    [DbMap("EXPORTABLE_")]
    public bool? Exportable { get; set; }
    [DisplayName("برچسب‌ها")]
    [DbMap("TAGS_")]
    public string? Tags { get; set; }
}
[DisplayName("موتور: فیلتر")]
[DbMap("ACT_RE_DATA_FORM_FILTER")]
public sealed class SqlActReDataFormFilter : SqlServerEntity<string>
{
    [DisplayName("مقدار")]
    [DbMap("VALUE_")]
    public string? Value { get; set; }
    [DisplayName("داده فرم شناسه")]
    [DbMap("DATA_FORM_ID_")]
    public string DataFormId { get; set; } = null!;
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("موتور: سرصفحه")]
[DbMap("ACT_RE_DATA_FORM_HEADER")]
public sealed class SqlActReDataFormHeader : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int Rev { get; set; }
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string? Name { get; set; }
    [DisplayName("نوع")]
    [DbMap("TYPE_")]
    public int Type { get; set; }
    [DisplayName("سفارش")]
    [DbMap("ORDER_")]
    public double Order { get; set; }
    [DisplayName("عملیات نوع")]
    [DbMap("ACTION_TYPE_")]
    public int? ActionType { get; set; }
    [DisplayName("داده فرم شناسه")]
    [DbMap("DATA_FORM_ID_")]
    public string DataFormId { get; set; } = null!;
    [DisplayName("مسئول متغیر")]
    [DbMap("ASSIGNEE_VAR_")]
    public string? AssigneeVar { get; set; }
    [DisplayName("مرجع فرم شناسه")]
    [DbMap("REF_FORM_ID_")]
    public string? RefFormId { get; set; }
    [DisplayName("فرایند کلید")]
    [DbMap("PROCESS_KEY_")]
    public string? ProcessKey { get; set; }
    [DisplayName("فرایند کلید نام")]
    [DbMap("PROCESS_KEY_NAME_")]
    public string? ProcessKeyName { get; set; }
    [DisplayName("فرایند مستاجر شناسه")]
    [DbMap("PROCESS_TENANT_ID_")]
    public string? ProcessTenantId { get; set; }
    [DisplayName("فرایند اتصال نوع")]
    [DbMap("PROCESS_BINDING_TYPE_")]
    public int? ProcessBindingType { get; set; }
    [DisplayName("فرایند برچسب")]
    [DbMap("PROCESS_TAG_")]
    public string? ProcessTag { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("درون‌خطی ویرایش")]
    [DbMap("INLINE_EDIT_")]
    public bool? InlineEdit { get; set; }
    [DisplayName("فرایند نیاز سطر")]
    [DbMap("PROCESS_NEED_ROW_")]
    public bool? ProcessNeedRow { get; set; }
    [DisplayName("خروجی نوع")]
    [DbMap("EXPORT_TYPE_")]
    public int? ExportType { get; set; }
    [DisplayName("خروجی کامل داده")]
    [DbMap("OUT_FULL_DATA_")]
    public bool? OutFullData { get; set; }
    [DisplayName("فقط‌خواندنی روابط")]
    [DbMap("READONLY_RELATIONS_")]
    public string? ReadonlyRelations { get; set; }
    [DisplayName("مرجع فرم روابط")]
    [DbMap("REF_FORM_RELATIONS_")]
    public string? RefFormRelations { get; set; }
    [DisplayName("خروجی تفصیلی")]
    [DbMap("EXPORT_DETAIL_")]
    public string? ExportDetail { get; set; }
    [DisplayName("خروجی محدودیت")]
    [DbMap("EXPORT_LIMIT_")]
    public int? ExportLimit { get; set; }
}
[DisplayName("موتور: مرتب‌سازی")]
[DbMap("ACT_RE_DATA_FORM_SORT")]
public sealed class SqlActReDataFormSort : SqlServerEntity<string>
{
    [DisplayName("مقدار")]
    [DbMap("VALUE_")]
    public string? Value { get; set; }
    [DisplayName("داده فرم شناسه")]
    [DbMap("DATA_FORM_ID_")]
    public string DataFormId { get; set; } = null!;
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("موتور: گزارش")]
[DbMap("ACT_RE_DATA_REPORT")]
public sealed class SqlActReDataReport : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int Rev { get; set; }
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string Name { get; set; } = null!;
    [DisplayName("دسته‌بندی")]
    [DbMap("CATEGORY_")]
    public string? Category { get; set; }
    [DisplayName("فرم شناسه")]
    [DbMap("FORM_ID_")]
    public string FormId { get; set; } = null!;
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("سفارشی راهنما")]
    [DbMap("CUSTOM_HINTS_")]
    public string? CustomHints { get; set; }
}
[DisplayName("موتور: ستون")]
[DbMap("ACT_RE_DATA_REPORT_COLUMN")]
public sealed class SqlActReDataReportColumn : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int Rev { get; set; }
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string? Name { get; set; }
    [DisplayName("نوع")]
    [DbMap("TYPE_")]
    public int Type { get; set; }
    [DisplayName("سفارش")]
    [DbMap("ORDER_")]
    public double Order { get; set; }
    [DisplayName("داده گزارش شناسه")]
    [DbMap("DATA_REPORT_ID_")]
    public string DataReportId { get; set; } = null!;
    [DisplayName("مرجع داده فرم شناسه")]
    [DbMap("REF_DATA_FORM_ID_")]
    public string? RefDataFormId { get; set; }
    [DisplayName("مرجع داده ستون شناسه")]
    [DbMap("REF_DATA_COLUMN_ID_")]
    public string? RefDataColumnId { get; set; }
    [DisplayName("تجمیع")]
    [DbMap("AGGREGATION_")]
    public string? Aggregation { get; set; }
    [DisplayName("داده قالب")]
    [DbMap("DATA_FORMAT_")]
    public string? DataFormat { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("مرجع داده فیلتر ستون شناسه")]
    [DbMap("REF_DATA_FILTER_COLUMN_ID_")]
    public string? RefDataFilterColumnId { get; set; }
    [DisplayName("قابل جمع")]
    [DbMap("SUMMABLE_")]
    public bool? Summable { get; set; }
}
[DisplayName("موتور: فیلتر")]
[DbMap("ACT_RE_DATA_REPORT_FILTER")]
public sealed class SqlActReDataReportFilter : SqlServerEntity<string>
{
    [DisplayName("مقدار")]
    [DbMap("VALUE_")]
    public string? Value { get; set; }
    [DisplayName("داده گزارش شناسه")]
    [DbMap("DATA_REPORT_ID_")]
    public string DataReportId { get; set; } = null!;
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("موتور: سرصفحه")]
[DbMap("ACT_RE_DATA_REPORT_HEADER")]
public sealed class SqlActReDataReportHeader : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int Rev { get; set; }
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string? Name { get; set; }
    [DisplayName("نوع")]
    [DbMap("TYPE_")]
    public int Type { get; set; }
    [DisplayName("سفارش")]
    [DbMap("ORDER_")]
    public double Order { get; set; }
    [DisplayName("خروجی نوع")]
    [DbMap("EXPORT_TYPE_")]
    public int? ExportType { get; set; }
    [DisplayName("داده گزارش شناسه")]
    [DbMap("DATA_REPORT_ID_")]
    public string DataReportId { get; set; } = null!;
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("خروجی تفصیلی")]
    [DbMap("EXPORT_DETAIL_")]
    public string? ExportDetail { get; set; }
    [DisplayName("خروجی محدودیت")]
    [DbMap("EXPORT_LIMIT_")]
    public int? ExportLimit { get; set; }
}
[DisplayName("موتور: مرتب‌سازی")]
[DbMap("ACT_RE_DATA_REPORT_SORT")]
public sealed class SqlActReDataReportSort : SqlServerEntity<string>
{
    [DisplayName("مقدار")]
    [DbMap("VALUE_")]
    public string? Value { get; set; }
    [DisplayName("داده گزارش شناسه")]
    [DbMap("DATA_REPORT_ID_")]
    public string DataReportId { get; set; } = null!;
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("موتور: تعریف")]
[DbMap("ACT_RE_DECISION_DEF")]
public sealed class SqlActReDecisionDef : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int? Rev { get; set; }
    [DisplayName("دسته‌بندی")]
    [DbMap("CATEGORY_")]
    public string? Category { get; set; }
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string? Name { get; set; }
    [DisplayName("کلید")]
    [DbMap("KEY_")]
    public string Key { get; set; } = null!;
    [DisplayName("نسخه")]
    [DbMap("VERSION_")]
    public int Version { get; set; }
    [DisplayName("استقرار شناسه")]
    [DbMap("DEPLOYMENT_ID_")]
    public string? DeploymentId { get; set; }
    [DisplayName("منبع نام")]
    [DbMap("RESOURCE_NAME_")]
    public string? ResourceName { get; set; }
    [DisplayName("نمودار منبع نام")]
    [DbMap("DGRM_RESOURCE_NAME_")]
    public string? DgrmResourceName { get; set; }
    [DisplayName("تصمیم درخواست شناسه")]
    [DbMap("DEC_REQ_ID_")]
    public string? DecReqId { get; set; }
    [DisplayName("تصمیم درخواست کلید")]
    [DbMap("DEC_REQ_KEY_")]
    public string? DecReqKey { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("تاریخچه مدت نگهداری")]
    [DbMap("HISTORY_TTL_")]
    public int? HistoryTtl { get; set; }
    [DisplayName("نسخه برچسب")]
    [DbMap("VERSION_TAG_")]
    public string? VersionTag { get; set; }
}
[DisplayName("موتور: تعریف")]
[DbMap("ACT_RE_DECISION_REQ_DEF")]
public sealed class SqlActReDecisionReqDef : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int? Rev { get; set; }
    [DisplayName("دسته‌بندی")]
    [DbMap("CATEGORY_")]
    public string? Category { get; set; }
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string? Name { get; set; }
    [DisplayName("کلید")]
    [DbMap("KEY_")]
    public string Key { get; set; } = null!;
    [DisplayName("نسخه")]
    [DbMap("VERSION_")]
    public int Version { get; set; }
    [DisplayName("استقرار شناسه")]
    [DbMap("DEPLOYMENT_ID_")]
    public string? DeploymentId { get; set; }
    [DisplayName("منبع نام")]
    [DbMap("RESOURCE_NAME_")]
    public string? ResourceName { get; set; }
    [DisplayName("نمودار منبع نام")]
    [DbMap("DGRM_RESOURCE_NAME_")]
    public string? DgrmResourceName { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("موتور: استقرار")]
[DbMap("ACT_RE_DEPLOYMENT")]
public sealed class SqlActReDeployment : SqlServerEntity<string>
{
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string? Name { get; set; }
    [DisplayName("استقرار زمان")]
    [DbMap("DEPLOY_TIME_")]
    public DateTime? DeployTime { get; set; }
    [DisplayName("منبع")]
    [DbMap("SOURCE_")]
    public string? Source { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("موتور: فرم")]
[DbMap("ACT_RE_FORM")]
public sealed class SqlActReForm : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int Rev { get; set; }
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string Name { get; set; } = null!;
    [DisplayName("دسته‌بندی")]
    [DbMap("CATEGORY_")]
    public string? Category { get; set; }
    [DisplayName("تعریف")]
    [DbMap("DEFINITION_")]
    public string? Definition { get; set; }
    [DisplayName("ایجاد زمان")]
    [DbMap("CREATE_TIME_")]
    public DateTime CreateTime { get; set; }
    [DisplayName("به‌روزرسانی زمان")]
    [DbMap("UPDATE_TIME_")]
    public DateTime? UpdateTime { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("موتور: پیوند")]
[DbMap("ACT_RE_LINK")]
public sealed class SqlActReLink : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int Rev { get; set; }
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string Name { get; set; } = null!;
    [DisplayName("توضیحات")]
    [DbMap("DESCRIPTION_")]
    public string? Description { get; set; }
    [DisplayName("نشانی وب")]
    [DbMap("URL_")]
    public string Url { get; set; } = null!;
    [DisplayName("سفارش")]
    [DbMap("ORDER_")]
    public double Order { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("موتور: منو")]
[DbMap("ACT_RE_MENU")]
public sealed class SqlActReMenu : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int Rev { get; set; }
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string Name { get; set; } = null!;
    [DisplayName("نوع")]
    [DbMap("TYPE_")]
    public int Type { get; set; }
    [DisplayName("نشان")]
    [DbMap("LOGO_")]
    public string? Logo { get; set; }
    [DisplayName("تفصیلی")]
    [DbMap("DETAIL_")]
    public string? Detail { get; set; }
    [DisplayName("سفارش")]
    [DbMap("ORDER_")]
    public double Order { get; set; }
    [DisplayName("والد شناسه")]
    [DbMap("PARENT_ID_")]
    public string? ParentId { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("موتور: مدل")]
[DbMap("ACT_RE_MODEL")]
public sealed class SqlActReModel : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int Rev { get; set; }
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string Name { get; set; } = null!;
    [DisplayName("دسته‌بندی")]
    [DbMap("CATEGORY_")]
    public string? Category { get; set; }
    [DisplayName("تعریف")]
    [DbMap("DEFINITION_")]
    public string? Definition { get; set; }
    [DisplayName("نوع")]
    [DbMap("TYPE_")]
    public int Type { get; set; }
    [DisplayName("ایجاد زمان")]
    [DbMap("CREATE_TIME_")]
    public DateTime CreateTime { get; set; }
    [DisplayName("به‌روزرسانی زمان")]
    [DbMap("UPDATE_TIME_")]
    public DateTime? UpdateTime { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("موتور: اعلان")]
[DbMap("ACT_RE_NOTIFICATION")]
public sealed class SqlActReNotification : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int Rev { get; set; }
    [DisplayName("فرستنده")]
    [DbMap("SENDER_")]
    public string? Sender { get; set; }
    [DisplayName("گروه شناسه")]
    [DbMap("GROUP_ID_")]
    public string? GroupId { get; set; }
    [DisplayName("کاربر شناسه")]
    [DbMap("USER_ID_")]
    public string? UserId { get; set; }
    [DisplayName("پیام")]
    [DbMap("MESSAGE_")]
    public string Message { get; set; } = null!;
    [DisplayName("نوع")]
    [DbMap("TYPE_")]
    public string? Type { get; set; }
    [DisplayName("مقصد")]
    [DbMap("TARGET_")]
    public string? Target { get; set; }
    [DisplayName("فعال‌سازی")]
    [DbMap("ENABLE_")]
    public bool? Enable { get; set; }
    [DisplayName("شروع زمان")]
    [DbMap("START_TIME_")]
    public DateTime? StartTime { get; set; }
    [DisplayName("پایان زمان")]
    [DbMap("END_TIME_")]
    public DateTime? EndTime { get; set; }
    [DisplayName("تغییر زمان")]
    [DbMap("CHANGE_TIME_")]
    public DateTime ChangeTime { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("جزئیات")]
    [DbMap("DETAILS_")]
    public string? Details { get; set; }
}
[DisplayName("موتور: وضعیت")]
[DbMap("ACT_RE_NOTIFICATION_STATUS")]
public sealed class SqlActReNotificationStatus : SqlServerEntity
{
    [DisplayName("اعلان شناسه")]
    [DbMap("NOTIFICATION_ID_")]
    public string NotificationId { get; set; } = null!;
    [DisplayName("کاربر شناسه")]
    [DbMap("USER_ID_")]
    public string UserId { get; set; } = null!;
    [DisplayName("خواندن")]
    [DbMap("READ_")]
    public bool? Read { get; set; }
    [DisplayName("حذف‌شده")]
    [DbMap("DELETED_")]
    public bool? Deleted { get; set; }
}
[DisplayName("موتور: تعریف فرایند")]
[DbMap("ACT_RE_PROCDEF")]
public sealed class SqlActReProcdef : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int? Rev { get; set; }
    [DisplayName("دسته‌بندی")]
    [DbMap("CATEGORY_")]
    public string? Category { get; set; }
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string? Name { get; set; }
    [DisplayName("کلید")]
    [DbMap("KEY_")]
    public string Key { get; set; } = null!;
    [DisplayName("نسخه")]
    [DbMap("VERSION_")]
    public int Version { get; set; }
    [DisplayName("استقرار شناسه")]
    [DbMap("DEPLOYMENT_ID_")]
    public string? DeploymentId { get; set; }
    [DisplayName("منبع نام")]
    [DbMap("RESOURCE_NAME_")]
    public string? ResourceName { get; set; }
    [DisplayName("نمودار منبع نام")]
    [DbMap("DGRM_RESOURCE_NAME_")]
    public string? DgrmResourceName { get; set; }
    [DisplayName("دارای شروع فرم کلید")]
    [DbMap("HAS_START_FORM_KEY_")]
    public byte? HasStartFormKey { get; set; }
    [DisplayName("تعلیق وضعیت")]
    [DbMap("SUSPENSION_STATE_")]
    public byte? SuspensionState { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("نسخه برچسب")]
    [DbMap("VERSION_TAG_")]
    public string? VersionTag { get; set; }
    [DisplayName("تاریخچه مدت نگهداری")]
    [DbMap("HISTORY_TTL_")]
    public int? HistoryTtl { get; set; }
    [DisplayName("قابل شروع")]
    [DbMap("STARTABLE_")]
    public bool Startable { get; set; }
}
[DisplayName("موتور: پرس‌وجو")]
[DbMap("ACT_RE_QUERY")]
public sealed class SqlActReQuery : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int Rev { get; set; }
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string Name { get; set; } = null!;
    [DisplayName("دسته‌بندی")]
    [DbMap("CATEGORY_")]
    public string? Category { get; set; }
    [DisplayName("نوع")]
    [DbMap("TYPE_")]
    public int Type { get; set; }
    [DisplayName("تعریف")]
    [DbMap("DEFINITION_")]
    public string? Definition { get; set; }
    [DisplayName("ایجاد زمان")]
    [DbMap("CREATE_TIME_")]
    public DateTime CreateTime { get; set; }
    [DisplayName("به‌روزرسانی زمان")]
    [DbMap("UPDATE_TIME_")]
    public DateTime? UpdateTime { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("خروجی نوع")]
    [DbMap("EXPORT_TYPE_")]
    public int? ExportType { get; set; }
    [DisplayName("خروجی تفصیلی")]
    [DbMap("EXPORT_DETAIL_")]
    public string? ExportDetail { get; set; }
    [DisplayName("خروجی محدودیت")]
    [DbMap("EXPORT_LIMIT_")]
    public int? ExportLimit { get; set; }
}
[DisplayName("موتور: گزارش")]
[DbMap("ACT_RE_REPORT")]
public sealed class SqlActReReport : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int Rev { get; set; }
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string Name { get; set; } = null!;
    [DisplayName("پوشه")]
    [DbMap("FOLDER_")]
    public string? Folder { get; set; }
    [DisplayName("تعریف")]
    [DbMap("DEFINITION_")]
    public string? Definition { get; set; }
    [DisplayName("ایجاد زمان")]
    [DbMap("CREATE_TIME_")]
    public DateTime CreateTime { get; set; }
    [DisplayName("به‌روزرسانی زمان")]
    [DbMap("UPDATE_TIME_")]
    public DateTime? UpdateTime { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("موتور: جدول")]
[DbMap("ACT_RE_TABLE")]
public sealed class SqlActReTable : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int Rev { get; set; }
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string Name { get; set; } = null!;
    [DisplayName("دسته‌بندی")]
    [DbMap("CATEGORY_")]
    public string? Category { get; set; }
    [DisplayName("یکسان‌سازی")]
    [DbMap("SYNC_")]
    public bool Sync { get; set; }
    [DisplayName("پیش‌فرض")]
    [DbMap("DEFAULT_")]
    public bool Default { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("موتور: موضوع")]
[DbMap("ACT_RE_TOPIC")]
public sealed class SqlActReTopic : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int Rev { get; set; }
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string Name { get; set; } = null!;
    [DisplayName("نوع")]
    [DbMap("TYPE_")]
    public int Type { get; set; }
    [DisplayName("بسته‌ها کلاس")]
    [DbMap("PACKAGES_CLASS_")]
    public string? PackagesClass { get; set; }
    [DisplayName("پیش‌فرض")]
    [DbMap("DEFAULT_")]
    public bool Default { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("موتور: مجوز")]
[DbMap("ACT_RU_AUTHORIZATION")]
public sealed class SqlActRuAuthorization : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int? Rev { get; set; }
    [DisplayName("نوع")]
    [DbMap("TYPE_")]
    public int Type { get; set; }
    [DisplayName("گروه شناسه")]
    [DbMap("GROUP_ID_")]
    public string? GroupId { get; set; }
    [DisplayName("کاربر شناسه")]
    [DbMap("USER_ID_")]
    public string? UserId { get; set; }
    [DisplayName("منبع نوع")]
    [DbMap("RESOURCE_TYPE_")]
    public int ResourceType { get; set; }
    [DisplayName("منبع شناسه")]
    [DbMap("RESOURCE_ID_")]
    public string? ResourceId { get; set; }
    [DisplayName("مجوزها")]
    [DbMap("PERMS_")]
    public int? Perms { get; set; }
    [DisplayName("حذف زمان")]
    [DbMap("REMOVAL_TIME_")]
    public DateTime? RemovalTime { get; set; }
    [DisplayName("ریشه فرایند نمونه شناسه")]
    [DbMap("ROOT_PROC_INST_ID_")]
    public string? RootProcInstId { get; set; }
    [DisplayName("هش")]
    [DbMap("HASH_")]
    public string? Hash { get; set; }
}
[DisplayName("موتور: دسته")]
[DbMap("ACT_RU_BATCH")]
public sealed class SqlActRuBatch : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int Rev { get; set; }
    [DisplayName("نوع")]
    [DbMap("TYPE_")]
    public string? Type { get; set; }
    [DisplayName("مجموع کارهای پس‌زمینه")]
    [DbMap("TOTAL_JOBS_")]
    public int? TotalJobs { get; set; }
    [DisplayName("کارهای پس‌زمینه ایجادشده")]
    [DbMap("JOBS_CREATED_")]
    public int? JobsCreated { get; set; }
    [DisplayName("کارهای پس‌زمینه به‌ازای بذر")]
    [DbMap("JOBS_PER_SEED_")]
    public int? JobsPerSeed { get; set; }
    [DisplayName("فراخوانی‌ها به‌ازای کار پس‌زمینه")]
    [DbMap("INVOCATIONS_PER_JOB_")]
    public int? InvocationsPerJob { get; set; }
    [DisplayName("بذر کار پس‌زمینه تعریف شناسه")]
    [DbMap("SEED_JOB_DEF_ID_")]
    public string? SeedJobDefId { get; set; }
    [DisplayName("دسته کار پس‌زمینه تعریف شناسه")]
    [DbMap("BATCH_JOB_DEF_ID_")]
    public string? BatchJobDefId { get; set; }
    [DisplayName("پایش کار پس‌زمینه تعریف شناسه")]
    [DbMap("MONITOR_JOB_DEF_ID_")]
    public string? MonitorJobDefId { get; set; }
    [DisplayName("تعلیق وضعیت")]
    [DbMap("SUSPENSION_STATE_")]
    public byte? SuspensionState { get; set; }
    [DisplayName("پیکربندی")]
    [DbMap("CONFIGURATION_")]
    public string? Configuration { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("ایجاد کاربر شناسه")]
    [DbMap("CREATE_USER_ID_")]
    public string? CreateUserId { get; set; }
    [DisplayName("شروع زمان")]
    [DbMap("START_TIME_")]
    public DateTime? StartTime { get; set; }
}
[DisplayName("موتور: اجرا")]
[DbMap("ACT_RU_CASE_EXECUTION")]
public sealed class SqlActRuCaseExecution : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int? Rev { get; set; }
    [DisplayName("پرونده نمونه شناسه")]
    [DbMap("CASE_INST_ID_")]
    public string? CaseInstId { get; set; }
    [DisplayName("بالادست پرونده اجرا")]
    [DbMap("SUPER_CASE_EXEC_")]
    public string? SuperCaseExec { get; set; }
    [DisplayName("بالادست اجرا")]
    [DbMap("SUPER_EXEC_")]
    public string? SuperExec { get; set; }
    [DisplayName("کسب‌وکار کلید")]
    [DbMap("BUSINESS_KEY_")]
    public string? BusinessKey { get; set; }
    [DisplayName("والد شناسه")]
    [DbMap("PARENT_ID_")]
    public string? ParentId { get; set; }
    [DisplayName("پرونده تعریف شناسه")]
    [DbMap("CASE_DEF_ID_")]
    public string? CaseDefId { get; set; }
    [DisplayName("فعالیت شناسه")]
    [DbMap("ACT_ID_")]
    public string? ActId { get; set; }
    [DisplayName("قبلی وضعیت")]
    [DbMap("PREV_STATE_")]
    public int? PrevState { get; set; }
    [DisplayName("فعلی وضعیت")]
    [DbMap("CURRENT_STATE_")]
    public int? CurrentState { get; set; }
    [DisplayName("الزامی")]
    [DbMap("REQUIRED_")]
    public byte? Required { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("موتور: بخش")]
[DbMap("ACT_RU_CASE_SENTRY_PART")]
public sealed class SqlActRuCaseSentryPart : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int? Rev { get; set; }
    [DisplayName("پرونده نمونه شناسه")]
    [DbMap("CASE_INST_ID_")]
    public string? CaseInstId { get; set; }
    [DisplayName("پرونده اجرا شناسه")]
    [DbMap("CASE_EXEC_ID_")]
    public string? CaseExecId { get; set; }
    [DisplayName("شرط محافظ شناسه")]
    [DbMap("SENTRY_ID_")]
    public string? SentryId { get; set; }
    [DisplayName("نوع")]
    [DbMap("TYPE_")]
    public string? Type { get; set; }
    [DisplayName("منبع پرونده اجرا شناسه")]
    [DbMap("SOURCE_CASE_EXEC_ID_")]
    public string? SourceCaseExecId { get; set; }
    [DisplayName("استاندارد رویداد")]
    [DbMap("STANDARD_EVENT_")]
    public string? StandardEvent { get; set; }
    [DisplayName("منبع")]
    [DbMap("SOURCE_")]
    public string? Source { get; set; }
    [DisplayName("متغیر رویداد")]
    [DbMap("VARIABLE_EVENT_")]
    public string? VariableEvent { get; set; }
    [DisplayName("متغیر نام")]
    [DbMap("VARIABLE_NAME_")]
    public string? VariableName { get; set; }
    [DisplayName("برآورده‌شده")]
    [DbMap("SATISFIED_")]
    public byte? Satisfied { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("موتور: صف")]
[DbMap("ACT_RU_CHANGE_QUEUE")]
public sealed class SqlActRuChangeQueue : SqlServerEntity<string>
{
    [DisplayName("زمان")]
    [DbMap("TIME_")]
    public DateTime Time { get; set; }
    [DisplayName("نوع")]
    [DbMap("TYPE_")]
    public string Type { get; set; } = null!;
    [DisplayName("عملیات")]
    [DbMap("ACTION_")]
    public string Action { get; set; } = null!;
    [DisplayName("مقصد شناسه")]
    [DbMap("TARGET_ID_")]
    public string TargetId { get; set; } = null!;
}
[DisplayName("موتور: اشتراک")]
[DbMap("ACT_RU_EVENT_SUBSCR")]
public sealed class SqlActRuEventSubscr : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int? Rev { get; set; }
    [DisplayName("رویداد نوع")]
    [DbMap("EVENT_TYPE_")]
    public string EventType { get; set; } = null!;
    [DisplayName("رویداد نام")]
    [DbMap("EVENT_NAME_")]
    public string? EventName { get; set; }
    [DisplayName("اجرا شناسه")]
    [DbMap("EXECUTION_ID_")]
    public string? ExecutionId { get; set; }
    [DisplayName("فرایند نمونه شناسه")]
    [DbMap("PROC_INST_ID_")]
    public string? ProcInstId { get; set; }
    [DisplayName("فعالیت شناسه")]
    [DbMap("ACTIVITY_ID_")]
    public string? ActivityId { get; set; }
    [DisplayName("پیکربندی")]
    [DbMap("CONFIGURATION_")]
    public string? Configuration { get; set; }
    [DisplayName("ایجادشده")]
    [DbMap("CREATED_")]
    public DateTime Created { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("موتور: اجرا")]
[DbMap("ACT_RU_EXECUTION")]
public sealed class SqlActRuExecution : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int? Rev { get; set; }
    [DisplayName("ریشه فرایند نمونه شناسه")]
    [DbMap("ROOT_PROC_INST_ID_")]
    public string? RootProcInstId { get; set; }
    [DisplayName("فرایند نمونه شناسه")]
    [DbMap("PROC_INST_ID_")]
    public string? ProcInstId { get; set; }
    [DisplayName("کسب‌وکار کلید")]
    [DbMap("BUSINESS_KEY_")]
    public string? BusinessKey { get; set; }
    [DisplayName("والد شناسه")]
    [DbMap("PARENT_ID_")]
    public string? ParentId { get; set; }
    [DisplayName("فرایند تعریف شناسه")]
    [DbMap("PROC_DEF_ID_")]
    public string? ProcDefId { get; set; }
    [DisplayName("بالادست اجرا")]
    [DbMap("SUPER_EXEC_")]
    public string? SuperExec { get; set; }
    [DisplayName("بالادست پرونده اجرا")]
    [DbMap("SUPER_CASE_EXEC_")]
    public string? SuperCaseExec { get; set; }
    [DisplayName("پرونده نمونه شناسه")]
    [DbMap("CASE_INST_ID_")]
    public string? CaseInstId { get; set; }
    [DisplayName("فعالیت شناسه")]
    [DbMap("ACT_ID_")]
    public string? ActId { get; set; }
    [DisplayName("فعالیت نمونه شناسه")]
    [DbMap("ACT_INST_ID_")]
    public string? ActInstId { get; set; }
    [DisplayName("آیا فعال")]
    [DbMap("IS_ACTIVE_")]
    public byte? IsActive { get; set; }
    [DisplayName("آیا همزمان")]
    [DbMap("IS_CONCURRENT_")]
    public byte? IsConcurrent { get; set; }
    [DisplayName("آیا دامنه مجوز")]
    [DbMap("IS_SCOPE_")]
    public byte? IsScope { get; set; }
    [DisplayName("آیا رویداد دامنه مجوز")]
    [DbMap("IS_EVENT_SCOPE_")]
    public byte? IsEventScope { get; set; }
    [DisplayName("تعلیق وضعیت")]
    [DbMap("SUSPENSION_STATE_")]
    public byte? SuspensionState { get; set; }
    [DisplayName("کش‌شده موجودیت وضعیت")]
    [DbMap("CACHED_ENT_STATE_")]
    public int? CachedEntState { get; set; }
    [DisplayName("توالی شمارنده")]
    [DbMap("SEQUENCE_COUNTER_")]
    public decimal? SequenceCounter { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("فرایند تعریف کلید")]
    [DbMap("PROC_DEF_KEY_")]
    public string? ProcDefKey { get; set; }
}
[DisplayName("موتور: وظیفه")]
[DbMap("ACT_RU_EXT_TASK")]
public sealed class SqlActRuExtTask : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int Rev { get; set; }
    [DisplayName("پردازشگر شناسه")]
    [DbMap("WORKER_ID_")]
    public string? WorkerId { get; set; }
    [DisplayName("موضوع نام")]
    [DbMap("TOPIC_NAME_")]
    public string? TopicName { get; set; }
    [DisplayName("تلاش‌های مجدد")]
    [DbMap("RETRIES_")]
    public int? Retries { get; set; }
    [DisplayName("خطا پیام")]
    [DbMap("ERROR_MSG_")]
    public string? ErrorMsg { get; set; }
    [DisplayName("خطا جزئیات شناسه")]
    [DbMap("ERROR_DETAILS_ID_")]
    public string? ErrorDetailsId { get; set; }
    [DisplayName("قفل انقضا زمان")]
    [DbMap("LOCK_EXP_TIME_")]
    public DateTime? LockExpTime { get; set; }
    [DisplayName("تعلیق وضعیت")]
    [DbMap("SUSPENSION_STATE_")]
    public byte? SuspensionState { get; set; }
    [DisplayName("اجرا شناسه")]
    [DbMap("EXECUTION_ID_")]
    public string? ExecutionId { get; set; }
    [DisplayName("فرایند نمونه شناسه")]
    [DbMap("PROC_INST_ID_")]
    public string? ProcInstId { get; set; }
    [DisplayName("فرایند تعریف شناسه")]
    [DbMap("PROC_DEF_ID_")]
    public string? ProcDefId { get; set; }
    [DisplayName("فرایند تعریف کلید")]
    [DbMap("PROC_DEF_KEY_")]
    public string? ProcDefKey { get; set; }
    [DisplayName("فعالیت شناسه")]
    [DbMap("ACT_ID_")]
    public string? ActId { get; set; }
    [DisplayName("فعالیت نمونه شناسه")]
    [DbMap("ACT_INST_ID_")]
    public string? ActInstId { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("اولویت")]
    [DbMap("PRIORITY_")]
    public decimal Priority { get; set; }
    [DisplayName("آخرین شکست سوابق شناسه")]
    [DbMap("LAST_FAILURE_LOG_ID_")]
    public string? LastFailureLogId { get; set; }
    [DisplayName("ایجاد زمان")]
    [DbMap("CREATE_TIME_")]
    public DateTime? CreateTime { get; set; }
}
[DisplayName("موتور: فیلتر")]
[DbMap("ACT_RU_FILTER")]
public sealed class SqlActRuFilter : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int Rev { get; set; }
    [DisplayName("منبع نوع")]
    [DbMap("RESOURCE_TYPE_")]
    public string ResourceType { get; set; } = null!;
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string Name { get; set; } = null!;
    [DisplayName("مالک")]
    [DbMap("OWNER_")]
    public string? Owner { get; set; }
    [DisplayName("پرس‌وجو")]
    [DbMap("QUERY_")]
    public string Query { get; set; } = null!;
    [DisplayName("ویژگی‌ها")]
    [DbMap("PROPERTIES_")]
    public string? Properties { get; set; }
}
[DisplayName("موتور: پیوند هویت")]
[DbMap("ACT_RU_IDENTITYLINK")]
public sealed class SqlActRuIdentitylink : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int? Rev { get; set; }
    [DisplayName("گروه شناسه")]
    [DbMap("GROUP_ID_")]
    public string? GroupId { get; set; }
    [DisplayName("نوع")]
    [DbMap("TYPE_")]
    public string? Type { get; set; }
    [DisplayName("کاربر شناسه")]
    [DbMap("USER_ID_")]
    public string? UserId { get; set; }
    [DisplayName("وظیفه شناسه")]
    [DbMap("TASK_ID_")]
    public string? TaskId { get; set; }
    [DisplayName("فرایند تعریف شناسه")]
    [DbMap("PROC_DEF_ID_")]
    public string? ProcDefId { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("موتور: رخداد خطا")]
[DbMap("ACT_RU_INCIDENT")]
public sealed class SqlActRuIncident : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int Rev { get; set; }
    [DisplayName("رخداد خطا مهر زمانی")]
    [DbMap("INCIDENT_TIMESTAMP_")]
    public DateTime IncidentTimestamp { get; set; }
    [DisplayName("رخداد خطا پیام")]
    [DbMap("INCIDENT_MSG_")]
    public string? IncidentMsg { get; set; }
    [DisplayName("رخداد خطا نوع")]
    [DbMap("INCIDENT_TYPE_")]
    public string IncidentType { get; set; } = null!;
    [DisplayName("اجرا شناسه")]
    [DbMap("EXECUTION_ID_")]
    public string? ExecutionId { get; set; }
    [DisplayName("فعالیت شناسه")]
    [DbMap("ACTIVITY_ID_")]
    public string? ActivityId { get; set; }
    [DisplayName("ناموفق فعالیت شناسه")]
    [DbMap("FAILED_ACTIVITY_ID_")]
    public string? FailedActivityId { get; set; }
    [DisplayName("فرایند نمونه شناسه")]
    [DbMap("PROC_INST_ID_")]
    public string? ProcInstId { get; set; }
    [DisplayName("فرایند تعریف شناسه")]
    [DbMap("PROC_DEF_ID_")]
    public string? ProcDefId { get; set; }
    [DisplayName("علت رخداد خطا شناسه")]
    [DbMap("CAUSE_INCIDENT_ID_")]
    public string? CauseIncidentId { get; set; }
    [DisplayName("ریشه علت رخداد خطا شناسه")]
    [DbMap("ROOT_CAUSE_INCIDENT_ID_")]
    public string? RootCauseIncidentId { get; set; }
    [DisplayName("پیکربندی")]
    [DbMap("CONFIGURATION_")]
    public string? Configuration { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("کار پس‌زمینه تعریف شناسه")]
    [DbMap("JOB_DEF_ID_")]
    public string? JobDefId { get; set; }
    [DisplayName("یادداشت")]
    [DbMap("ANNOTATION_")]
    public string? Annotation { get; set; }
}
[DisplayName("موتور: کار پس‌زمینه")]
[DbMap("ACT_RU_JOB")]
public sealed class SqlActRuJob : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int? Rev { get; set; }
    [DisplayName("نوع")]
    [DbMap("TYPE_")]
    public string Type { get; set; } = null!;
    [DisplayName("قفل انقضا زمان")]
    [DbMap("LOCK_EXP_TIME_")]
    public DateTime? LockExpTime { get; set; }
    [DisplayName("قفل مالک")]
    [DbMap("LOCK_OWNER_")]
    public string? LockOwner { get; set; }
    [DisplayName("انحصاری")]
    [DbMap("EXCLUSIVE_")]
    public bool? Exclusive { get; set; }
    [DisplayName("اجرا شناسه")]
    [DbMap("EXECUTION_ID_")]
    public string? ExecutionId { get; set; }
    [DisplayName("فرایند نمونه شناسه")]
    [DbMap("PROCESS_INSTANCE_ID_")]
    public string? ProcessInstanceId { get; set; }
    [DisplayName("فرایند تعریف شناسه")]
    [DbMap("PROCESS_DEF_ID_")]
    public string? ProcessDefId { get; set; }
    [DisplayName("فرایند تعریف کلید")]
    [DbMap("PROCESS_DEF_KEY_")]
    public string? ProcessDefKey { get; set; }
    [DisplayName("تلاش‌های مجدد")]
    [DbMap("RETRIES_")]
    public int? Retries { get; set; }
    [DisplayName("استثنا پشته شناسه")]
    [DbMap("EXCEPTION_STACK_ID_")]
    public string? ExceptionStackId { get; set; }
    [DisplayName("استثنا پیام")]
    [DbMap("EXCEPTION_MSG_")]
    public string? ExceptionMsg { get; set; }
    [DisplayName("ناموفق فعالیت شناسه")]
    [DbMap("FAILED_ACT_ID_")]
    public string? FailedActId { get; set; }
    [DisplayName("تاریخ سررسید")]
    [DbMap("DUEDATE_")]
    public DateTime? Duedate { get; set; }
    [DisplayName("تکرار")]
    [DbMap("REPEAT_")]
    public string? Repeat { get; set; }
    [DisplayName("تکرار آفست")]
    [DbMap("REPEAT_OFFSET_")]
    public decimal? RepeatOffset { get; set; }
    [DisplayName("پردازشگر نوع")]
    [DbMap("HANDLER_TYPE_")]
    public string? HandlerType { get; set; }
    [DisplayName("پردازشگر تنظیمات")]
    [DbMap("HANDLER_CFG_")]
    public string? HandlerCfg { get; set; }
    [DisplayName("استقرار شناسه")]
    [DbMap("DEPLOYMENT_ID_")]
    public string? DeploymentId { get; set; }
    [DisplayName("تعلیق وضعیت")]
    [DbMap("SUSPENSION_STATE_")]
    public byte SuspensionState { get; set; }
    [DisplayName("اولویت")]
    [DbMap("PRIORITY_")]
    public decimal Priority { get; set; }
    [DisplayName("کار پس‌زمینه تعریف شناسه")]
    [DbMap("JOB_DEF_ID_")]
    public string? JobDefId { get; set; }
    [DisplayName("توالی شمارنده")]
    [DbMap("SEQUENCE_COUNTER_")]
    public decimal? SequenceCounter { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("ایجاد زمان")]
    [DbMap("CREATE_TIME_")]
    public DateTime? CreateTime { get; set; }
    [DisplayName("آخرین شکست سوابق شناسه")]
    [DbMap("LAST_FAILURE_LOG_ID_")]
    public string? LastFailureLogId { get; set; }
    [DisplayName("ریشه فرایند نمونه شناسه")]
    [DbMap("ROOT_PROC_INST_ID_")]
    public string? RootProcInstId { get; set; }
    [DisplayName("نام کاربری")]
    [DbMap("USERNAME_")]
    public string? Username { get; set; }
    [DisplayName("دسته شناسه")]
    [DbMap("BATCH_ID_")]
    public string? BatchId { get; set; }
}
[DisplayName("موتور: تعریف کار پس‌زمینه")]
[DbMap("ACT_RU_JOBDEF")]
public sealed class SqlActRuJobdef : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int? Rev { get; set; }
    [DisplayName("فرایند تعریف شناسه")]
    [DbMap("PROC_DEF_ID_")]
    public string? ProcDefId { get; set; }
    [DisplayName("فرایند تعریف کلید")]
    [DbMap("PROC_DEF_KEY_")]
    public string? ProcDefKey { get; set; }
    [DisplayName("فعالیت شناسه")]
    [DbMap("ACT_ID_")]
    public string? ActId { get; set; }
    [DisplayName("کار پس‌زمینه نوع")]
    [DbMap("JOB_TYPE_")]
    public string JobType { get; set; } = null!;
    [DisplayName("کار پس‌زمینه پیکربندی")]
    [DbMap("JOB_CONFIGURATION_")]
    public string? JobConfiguration { get; set; }
    [DisplayName("تعلیق وضعیت")]
    [DbMap("SUSPENSION_STATE_")]
    public byte? SuspensionState { get; set; }
    [DisplayName("کار پس‌زمینه اولویت")]
    [DbMap("JOB_PRIORITY_")]
    public decimal? JobPriority { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("استقرار شناسه")]
    [DbMap("DEPLOYMENT_ID_")]
    public string? DeploymentId { get; set; }
}
[DisplayName("موتور: سوابق")]
[DbMap("ACT_RU_METER_LOG")]
public sealed class SqlActRuMeterLog : SqlServerEntity<string>
{
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string Name { get; set; } = null!;
    [DisplayName("گزارش‌دهنده")]
    [DbMap("REPORTER_")]
    public string? Reporter { get; set; }
    [DisplayName("مقدار")]
    [DbMap("VALUE_")]
    public decimal? Value { get; set; }
    [DisplayName("مهر زمانی")]
    [DbMap("TIMESTAMP_")]
    public DateTime? Timestamp { get; set; }
    [DisplayName("میلی‌ثانیه")]
    [DbMap("MILLISECONDS_")]
    public decimal? Milliseconds { get; set; }
}
[DisplayName("موتور: وظیفه")]
[DbMap("ACT_RU_TASK")]
public sealed class SqlActRuTask : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int? Rev { get; set; }
    [DisplayName("اجرا شناسه")]
    [DbMap("EXECUTION_ID_")]
    public string? ExecutionId { get; set; }
    [DisplayName("فرایند نمونه شناسه")]
    [DbMap("PROC_INST_ID_")]
    public string? ProcInstId { get; set; }
    [DisplayName("فرایند تعریف شناسه")]
    [DbMap("PROC_DEF_ID_")]
    public string? ProcDefId { get; set; }
    [DisplayName("پرونده اجرا شناسه")]
    [DbMap("CASE_EXECUTION_ID_")]
    public string? CaseExecutionId { get; set; }
    [DisplayName("پرونده نمونه شناسه")]
    [DbMap("CASE_INST_ID_")]
    public string? CaseInstId { get; set; }
    [DisplayName("پرونده تعریف شناسه")]
    [DbMap("CASE_DEF_ID_")]
    public string? CaseDefId { get; set; }
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string? Name { get; set; }
    [DisplayName("والد وظیفه شناسه")]
    [DbMap("PARENT_TASK_ID_")]
    public string? ParentTaskId { get; set; }
    [DisplayName("توضیحات")]
    [DbMap("DESCRIPTION_")]
    public string? Description { get; set; }
    [DisplayName("وظیفه تعریف کلید")]
    [DbMap("TASK_DEF_KEY_")]
    public string? TaskDefKey { get; set; }
    [DisplayName("مالک")]
    [DbMap("OWNER_")]
    public string? Owner { get; set; }
    [DisplayName("مسئول")]
    [DbMap("ASSIGNEE_")]
    public string? Assignee { get; set; }
    [DisplayName("واگذاری")]
    [DbMap("DELEGATION_")]
    public string? Delegation { get; set; }
    [DisplayName("اولویت")]
    [DbMap("PRIORITY_")]
    public int? Priority { get; set; }
    [DisplayName("ایجاد زمان")]
    [DbMap("CREATE_TIME_")]
    public DateTime? CreateTime { get; set; }
    [DisplayName("سررسید تاریخ")]
    [DbMap("DUE_DATE_")]
    public DateTime? DueDate { get; set; }
    [DisplayName("پیگیری بعدی تاریخ")]
    [DbMap("FOLLOW_UP_DATE_")]
    public DateTime? FollowUpDate { get; set; }
    [DisplayName("تعلیق وضعیت")]
    [DbMap("SUSPENSION_STATE_")]
    public int? SuspensionState { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("آخرین به‌روزشده")]
    [DbMap("LAST_UPDATED_")]
    public DateTime? LastUpdated { get; set; }
    [DisplayName("وظیفه وضعیت")]
    [DbMap("TASK_STATE_")]
    public string? TaskState { get; set; }
}
[DisplayName("موتور: سوابق")]
[DbMap("ACT_RU_TASK_METER_LOG")]
public sealed class SqlActRuTaskMeterLog : SqlServerEntity<string>
{
    [DisplayName("مسئول هش")]
    [DbMap("ASSIGNEE_HASH_")]
    public decimal? AssigneeHash { get; set; }
    [DisplayName("مهر زمانی")]
    [DbMap("TIMESTAMP_")]
    public DateTime? Timestamp { get; set; }
}
[DisplayName("موتور: متغیر")]
[DbMap("ACT_RU_VARIABLE")]
public sealed class SqlActRuVariable : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int? Rev { get; set; }
    [DisplayName("نوع")]
    [DbMap("TYPE_")]
    public string Type { get; set; } = null!;
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string Name { get; set; } = null!;
    [DisplayName("اجرا شناسه")]
    [DbMap("EXECUTION_ID_")]
    public string? ExecutionId { get; set; }
    [DisplayName("فرایند نمونه شناسه")]
    [DbMap("PROC_INST_ID_")]
    public string? ProcInstId { get; set; }
    [DisplayName("فرایند تعریف شناسه")]
    [DbMap("PROC_DEF_ID_")]
    public string? ProcDefId { get; set; }
    [DisplayName("پرونده اجرا شناسه")]
    [DbMap("CASE_EXECUTION_ID_")]
    public string? CaseExecutionId { get; set; }
    [DisplayName("پرونده نمونه شناسه")]
    [DbMap("CASE_INST_ID_")]
    public string? CaseInstId { get; set; }
    [DisplayName("وظیفه شناسه")]
    [DbMap("TASK_ID_")]
    public string? TaskId { get; set; }
    [DisplayName("دسته شناسه")]
    [DbMap("BATCH_ID_")]
    public string? BatchId { get; set; }
    [DisplayName("داده باینری شناسه")]
    [DbMap("BYTEARRAY_ID_")]
    public string? BytearrayId { get; set; }
    [DisplayName("اعشاری")]
    [DbMap("DOUBLE_")]
    public double? Double { get; set; }
    [DisplayName("عدد صحیح بلند")]
    [DbMap("LONG_")]
    public decimal? Long { get; set; }
    [DisplayName("متن")]
    [DbMap("TEXT_")]
    public string? Text { get; set; }
    [DisplayName("متن دوم")]
    [DbMap("TEXT2_")]
    public string? Text2 { get; set; }
    [DisplayName("متغیر دامنه مجوز")]
    [DbMap("VAR_SCOPE_")]
    public string VarScope { get; set; } = null!;
    [DisplayName("توالی شمارنده")]
    [DbMap("SEQUENCE_COUNTER_")]
    public decimal? SequenceCounter { get; set; }
    [DisplayName("آیا همزمان محلی")]
    [DbMap("IS_CONCURRENT_LOCAL_")]
    public byte? IsConcurrentLocal { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
