using Hyper.Domain.Entities.Database;
using Neo.Bpms.Domain.Features.MetaDefinitions.Reports;
using Neo.Bpms.Domain.Models.Cmmn.UI.Reports;
using Neo.Bpms.Domain.Models.Cmmn.Fields;
namespace Hyper.AdminPanel.Domain.UiDefinitions.Database;

public sealed class SqlActGeBytearrayUiDefinitions : CRUDDefinition<SqlActGeBytearray>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActGeBytearray.Id), nameof(SqlActGeBytearray.Rev), nameof(SqlActGeBytearray.Name), nameof(SqlActGeBytearray.DeploymentId), nameof(SqlActGeBytearray.Generated), nameof(SqlActGeBytearray.TenantId), nameof(SqlActGeBytearray.Type), nameof(SqlActGeBytearray.CreateTime), nameof(SqlActGeBytearray.RootProcInstId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActGeBytearray.Id), nameof(SqlActGeBytearray.Rev), nameof(SqlActGeBytearray.Name), nameof(SqlActGeBytearray.DeploymentId), nameof(SqlActGeBytearray.Generated), nameof(SqlActGeBytearray.TenantId), nameof(SqlActGeBytearray.Type), nameof(SqlActGeBytearray.CreateTime), nameof(SqlActGeBytearray.RootProcInstId), nameof(SqlActGeBytearray.RemovalTime));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: داده باینری";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActGePropertyUiDefinitions : CRUDDefinition<SqlActGeProperty>
{
    public override List<string>? Roles => [HyperRoles.Admin];
        protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActGeProperty.Name), nameof(SqlActGeProperty.Value), nameof(SqlActGeProperty.Rev));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActGeProperty.Name), nameof(SqlActGeProperty.Value), nameof(SqlActGeProperty.Rev));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: ویژگی";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActGeSchemaLogUiDefinitions : CRUDDefinition<SqlActGeSchemaLog>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActGeSchemaLog.Id), nameof(SqlActGeSchemaLog.Timestamp), nameof(SqlActGeSchemaLog.Version));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActGeSchemaLog.Id), nameof(SqlActGeSchemaLog.Timestamp), nameof(SqlActGeSchemaLog.Version));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: سوابق";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActHiActinstUiDefinitions : CRUDDefinition<SqlActHiActinst>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActHiActinst.Id), nameof(SqlActHiActinst.ParentActInstId), nameof(SqlActHiActinst.ProcDefKey), nameof(SqlActHiActinst.ProcDefId), nameof(SqlActHiActinst.RootProcInstId), nameof(SqlActHiActinst.ProcInstId), nameof(SqlActHiActinst.ExecutionId), nameof(SqlActHiActinst.ActId), nameof(SqlActHiActinst.TaskId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActHiActinst.Id), nameof(SqlActHiActinst.ParentActInstId), nameof(SqlActHiActinst.ProcDefKey), nameof(SqlActHiActinst.ProcDefId), nameof(SqlActHiActinst.RootProcInstId), nameof(SqlActHiActinst.ProcInstId), nameof(SqlActHiActinst.ExecutionId), nameof(SqlActHiActinst.ActId), nameof(SqlActHiActinst.TaskId), nameof(SqlActHiActinst.CallProcInstId), nameof(SqlActHiActinst.CallCaseInstId), nameof(SqlActHiActinst.ActName), nameof(SqlActHiActinst.ActType), nameof(SqlActHiActinst.Assignee), nameof(SqlActHiActinst.StartTime), nameof(SqlActHiActinst.EndTime), nameof(SqlActHiActinst.Duration), nameof(SqlActHiActinst.ActInstState), nameof(SqlActHiActinst.SequenceCounter), nameof(SqlActHiActinst.TenantId), nameof(SqlActHiActinst.RemovalTime));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد سوابق موتور: نمونه فعالیت";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActHiAttachmentUiDefinitions : CRUDDefinition<SqlActHiAttachment>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActHiAttachment.Id), nameof(SqlActHiAttachment.Rev), nameof(SqlActHiAttachment.UserId), nameof(SqlActHiAttachment.Name), nameof(SqlActHiAttachment.Description), nameof(SqlActHiAttachment.Type), nameof(SqlActHiAttachment.TaskId), nameof(SqlActHiAttachment.RootProcInstId), nameof(SqlActHiAttachment.ProcInstId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActHiAttachment.Id), nameof(SqlActHiAttachment.Rev), nameof(SqlActHiAttachment.UserId), nameof(SqlActHiAttachment.Name), nameof(SqlActHiAttachment.Description), nameof(SqlActHiAttachment.Type), nameof(SqlActHiAttachment.TaskId), nameof(SqlActHiAttachment.RootProcInstId), nameof(SqlActHiAttachment.ProcInstId), nameof(SqlActHiAttachment.Url), nameof(SqlActHiAttachment.ContentId), nameof(SqlActHiAttachment.TenantId), nameof(SqlActHiAttachment.CreateTime), nameof(SqlActHiAttachment.RemovalTime));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد سوابق موتور: پیوست";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActHiBatchUiDefinitions : CRUDDefinition<SqlActHiBatch>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActHiBatch.Id), nameof(SqlActHiBatch.Type), nameof(SqlActHiBatch.TotalJobs), nameof(SqlActHiBatch.InvocationsPerJob), nameof(SqlActHiBatch.MonitorJobDefId), nameof(SqlActHiBatch.BatchJobDefId), nameof(SqlActHiBatch.TenantId), nameof(SqlActHiBatch.CreateUserId), nameof(SqlActHiBatch.StartTime));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActHiBatch.Id), nameof(SqlActHiBatch.Type), nameof(SqlActHiBatch.TotalJobs), nameof(SqlActHiBatch.InvocationsPerJob), nameof(SqlActHiBatch.MonitorJobDefId), nameof(SqlActHiBatch.BatchJobDefId), nameof(SqlActHiBatch.TenantId), nameof(SqlActHiBatch.CreateUserId), nameof(SqlActHiBatch.StartTime), nameof(SqlActHiBatch.EndTime), nameof(SqlActHiBatch.RemovalTime));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد سوابق موتور: دسته";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActHiCaseactinstUiDefinitions : CRUDDefinition<SqlActHiCaseactinst>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActHiCaseactinst.Id), nameof(SqlActHiCaseactinst.ParentActInstId), nameof(SqlActHiCaseactinst.CaseDefId), nameof(SqlActHiCaseactinst.CaseInstId), nameof(SqlActHiCaseactinst.CaseActId), nameof(SqlActHiCaseactinst.TaskId), nameof(SqlActHiCaseactinst.CallProcInstId), nameof(SqlActHiCaseactinst.CallCaseInstId), nameof(SqlActHiCaseactinst.CaseActName));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActHiCaseactinst.Id), nameof(SqlActHiCaseactinst.ParentActInstId), nameof(SqlActHiCaseactinst.CaseDefId), nameof(SqlActHiCaseactinst.CaseInstId), nameof(SqlActHiCaseactinst.CaseActId), nameof(SqlActHiCaseactinst.TaskId), nameof(SqlActHiCaseactinst.CallProcInstId), nameof(SqlActHiCaseactinst.CallCaseInstId), nameof(SqlActHiCaseactinst.CaseActName), nameof(SqlActHiCaseactinst.CaseActType), nameof(SqlActHiCaseactinst.CreateTime), nameof(SqlActHiCaseactinst.EndTime), nameof(SqlActHiCaseactinst.Duration), nameof(SqlActHiCaseactinst.State), nameof(SqlActHiCaseactinst.Required), nameof(SqlActHiCaseactinst.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد سوابق موتور: نمونه فعالیت پرونده";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActHiCaseinstUiDefinitions : CRUDDefinition<SqlActHiCaseinst>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActHiCaseinst.Id), nameof(SqlActHiCaseinst.CaseInstId), nameof(SqlActHiCaseinst.BusinessKey), nameof(SqlActHiCaseinst.CaseDefId), nameof(SqlActHiCaseinst.CreateTime), nameof(SqlActHiCaseinst.CloseTime), nameof(SqlActHiCaseinst.Duration), nameof(SqlActHiCaseinst.State), nameof(SqlActHiCaseinst.CreateUserId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActHiCaseinst.Id), nameof(SqlActHiCaseinst.CaseInstId), nameof(SqlActHiCaseinst.BusinessKey), nameof(SqlActHiCaseinst.CaseDefId), nameof(SqlActHiCaseinst.CreateTime), nameof(SqlActHiCaseinst.CloseTime), nameof(SqlActHiCaseinst.Duration), nameof(SqlActHiCaseinst.State), nameof(SqlActHiCaseinst.CreateUserId), nameof(SqlActHiCaseinst.SuperCaseInstanceId), nameof(SqlActHiCaseinst.SuperProcessInstanceId), nameof(SqlActHiCaseinst.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد سوابق موتور: نمونه پرونده";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActHiCommentUiDefinitions : CRUDDefinition<SqlActHiComment>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActHiComment.Id), nameof(SqlActHiComment.Type), nameof(SqlActHiComment.Time), nameof(SqlActHiComment.UserId), nameof(SqlActHiComment.TaskId), nameof(SqlActHiComment.RootProcInstId), nameof(SqlActHiComment.ProcInstId), nameof(SqlActHiComment.Action), nameof(SqlActHiComment.Message));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActHiComment.Id), nameof(SqlActHiComment.Type), nameof(SqlActHiComment.Time), nameof(SqlActHiComment.UserId), nameof(SqlActHiComment.TaskId), nameof(SqlActHiComment.RootProcInstId), nameof(SqlActHiComment.ProcInstId), nameof(SqlActHiComment.Action), nameof(SqlActHiComment.Message), nameof(SqlActHiComment.FullMsg), nameof(SqlActHiComment.TenantId), nameof(SqlActHiComment.RemovalTime), nameof(SqlActHiComment.Rev));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد سوابق موتور: نظر";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActHiDecInUiDefinitions : CRUDDefinition<SqlActHiDecIn>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActHiDecIn.Id), nameof(SqlActHiDecIn.DecInstId), nameof(SqlActHiDecIn.ClauseId), nameof(SqlActHiDecIn.ClauseName), nameof(SqlActHiDecIn.VarType), nameof(SqlActHiDecIn.BytearrayId), nameof(SqlActHiDecIn.Double), nameof(SqlActHiDecIn.Long), nameof(SqlActHiDecIn.Text));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActHiDecIn.Id), nameof(SqlActHiDecIn.DecInstId), nameof(SqlActHiDecIn.ClauseId), nameof(SqlActHiDecIn.ClauseName), nameof(SqlActHiDecIn.VarType), nameof(SqlActHiDecIn.BytearrayId), nameof(SqlActHiDecIn.Double), nameof(SqlActHiDecIn.Long), nameof(SqlActHiDecIn.Text), nameof(SqlActHiDecIn.TenantId), nameof(SqlActHiDecIn.CreateTime), nameof(SqlActHiDecIn.RootProcInstId), nameof(SqlActHiDecIn.RemovalTime));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد سوابق موتور: ورودی";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActHiDecOutUiDefinitions : CRUDDefinition<SqlActHiDecOut>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActHiDecOut.Id), nameof(SqlActHiDecOut.DecInstId), nameof(SqlActHiDecOut.ClauseId), nameof(SqlActHiDecOut.ClauseName), nameof(SqlActHiDecOut.RuleId), nameof(SqlActHiDecOut.RuleOrder), nameof(SqlActHiDecOut.VarName), nameof(SqlActHiDecOut.VarType), nameof(SqlActHiDecOut.BytearrayId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActHiDecOut.Id), nameof(SqlActHiDecOut.DecInstId), nameof(SqlActHiDecOut.ClauseId), nameof(SqlActHiDecOut.ClauseName), nameof(SqlActHiDecOut.RuleId), nameof(SqlActHiDecOut.RuleOrder), nameof(SqlActHiDecOut.VarName), nameof(SqlActHiDecOut.VarType), nameof(SqlActHiDecOut.BytearrayId), nameof(SqlActHiDecOut.Double), nameof(SqlActHiDecOut.Long), nameof(SqlActHiDecOut.Text), nameof(SqlActHiDecOut.TenantId), nameof(SqlActHiDecOut.CreateTime), nameof(SqlActHiDecOut.RootProcInstId), nameof(SqlActHiDecOut.RemovalTime));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد سوابق موتور: خروجی";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActHiDecinstUiDefinitions : CRUDDefinition<SqlActHiDecinst>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActHiDecinst.Id), nameof(SqlActHiDecinst.DecDefId), nameof(SqlActHiDecinst.DecDefKey), nameof(SqlActHiDecinst.DecDefName), nameof(SqlActHiDecinst.ProcDefKey), nameof(SqlActHiDecinst.ProcDefId), nameof(SqlActHiDecinst.ProcInstId), nameof(SqlActHiDecinst.CaseDefKey), nameof(SqlActHiDecinst.CaseDefId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActHiDecinst.Id), nameof(SqlActHiDecinst.DecDefId), nameof(SqlActHiDecinst.DecDefKey), nameof(SqlActHiDecinst.DecDefName), nameof(SqlActHiDecinst.ProcDefKey), nameof(SqlActHiDecinst.ProcDefId), nameof(SqlActHiDecinst.ProcInstId), nameof(SqlActHiDecinst.CaseDefKey), nameof(SqlActHiDecinst.CaseDefId), nameof(SqlActHiDecinst.CaseInstId), nameof(SqlActHiDecinst.ActInstId), nameof(SqlActHiDecinst.ActId), nameof(SqlActHiDecinst.EvalTime), nameof(SqlActHiDecinst.RemovalTime), nameof(SqlActHiDecinst.CollectValue), nameof(SqlActHiDecinst.UserId), nameof(SqlActHiDecinst.RootDecInstId), nameof(SqlActHiDecinst.RootProcInstId), nameof(SqlActHiDecinst.DecReqId), nameof(SqlActHiDecinst.DecReqKey), nameof(SqlActHiDecinst.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد سوابق موتور: نمونه تصمیم";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActHiDetailUiDefinitions : CRUDDefinition<SqlActHiDetail>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActHiDetail.Id), nameof(SqlActHiDetail.Type), nameof(SqlActHiDetail.ProcDefKey), nameof(SqlActHiDetail.ProcDefId), nameof(SqlActHiDetail.RootProcInstId), nameof(SqlActHiDetail.ProcInstId), nameof(SqlActHiDetail.ExecutionId), nameof(SqlActHiDetail.CaseDefKey), nameof(SqlActHiDetail.CaseDefId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActHiDetail.Id), nameof(SqlActHiDetail.Type), nameof(SqlActHiDetail.ProcDefKey), nameof(SqlActHiDetail.ProcDefId), nameof(SqlActHiDetail.RootProcInstId), nameof(SqlActHiDetail.ProcInstId), nameof(SqlActHiDetail.ExecutionId), nameof(SqlActHiDetail.CaseDefKey), nameof(SqlActHiDetail.CaseDefId), nameof(SqlActHiDetail.CaseInstId), nameof(SqlActHiDetail.CaseExecutionId), nameof(SqlActHiDetail.TaskId), nameof(SqlActHiDetail.ActInstId), nameof(SqlActHiDetail.VarInstId), nameof(SqlActHiDetail.Name), nameof(SqlActHiDetail.VarType), nameof(SqlActHiDetail.Rev), nameof(SqlActHiDetail.Time), nameof(SqlActHiDetail.BytearrayId), nameof(SqlActHiDetail.Double), nameof(SqlActHiDetail.Long), nameof(SqlActHiDetail.Text), nameof(SqlActHiDetail.SequenceCounter), nameof(SqlActHiDetail.TenantId), nameof(SqlActHiDetail.OperationId), nameof(SqlActHiDetail.RemovalTime), nameof(SqlActHiDetail.Initial));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد سوابق موتور: تفصیلی";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActHiExtTaskLogUiDefinitions : CRUDDefinition<SqlActHiExtTaskLog>
{
    public override List<string>? Roles => [HyperRoles.Admin];
        protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActHiExtTaskLog.Id), nameof(SqlActHiExtTaskLog.Timestamp), nameof(SqlActHiExtTaskLog.ExtTaskId), nameof(SqlActHiExtTaskLog.Retries), nameof(SqlActHiExtTaskLog.TopicName), nameof(SqlActHiExtTaskLog.WorkerId), nameof(SqlActHiExtTaskLog.Priority), nameof(SqlActHiExtTaskLog.ErrorMsg), nameof(SqlActHiExtTaskLog.ErrorDetailsId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActHiExtTaskLog.Id), nameof(SqlActHiExtTaskLog.Timestamp), nameof(SqlActHiExtTaskLog.ExtTaskId), nameof(SqlActHiExtTaskLog.Retries), nameof(SqlActHiExtTaskLog.TopicName), nameof(SqlActHiExtTaskLog.WorkerId), nameof(SqlActHiExtTaskLog.Priority), nameof(SqlActHiExtTaskLog.ErrorMsg), nameof(SqlActHiExtTaskLog.ErrorDetailsId), nameof(SqlActHiExtTaskLog.ActId), nameof(SqlActHiExtTaskLog.ActInstId), nameof(SqlActHiExtTaskLog.ExecutionId), nameof(SqlActHiExtTaskLog.RootProcInstId), nameof(SqlActHiExtTaskLog.ProcInstId), nameof(SqlActHiExtTaskLog.ProcDefId), nameof(SqlActHiExtTaskLog.ProcDefKey), nameof(SqlActHiExtTaskLog.TenantId), nameof(SqlActHiExtTaskLog.State), nameof(SqlActHiExtTaskLog.RemovalTime));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد سوابق موتور: سوابق";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActHiIdentitylinkUiDefinitions : CRUDDefinition<SqlActHiIdentitylink>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActHiIdentitylink.Id), nameof(SqlActHiIdentitylink.Timestamp), nameof(SqlActHiIdentitylink.Type), nameof(SqlActHiIdentitylink.UserId), nameof(SqlActHiIdentitylink.GroupId), nameof(SqlActHiIdentitylink.TaskId), nameof(SqlActHiIdentitylink.RootProcInstId), nameof(SqlActHiIdentitylink.ProcDefId), nameof(SqlActHiIdentitylink.OperationType));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActHiIdentitylink.Id), nameof(SqlActHiIdentitylink.Timestamp), nameof(SqlActHiIdentitylink.Type), nameof(SqlActHiIdentitylink.UserId), nameof(SqlActHiIdentitylink.GroupId), nameof(SqlActHiIdentitylink.TaskId), nameof(SqlActHiIdentitylink.RootProcInstId), nameof(SqlActHiIdentitylink.ProcDefId), nameof(SqlActHiIdentitylink.OperationType), nameof(SqlActHiIdentitylink.AssignerId), nameof(SqlActHiIdentitylink.ProcDefKey), nameof(SqlActHiIdentitylink.TenantId), nameof(SqlActHiIdentitylink.RemovalTime));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد سوابق موتور: پیوند هویت";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActHiIncidentUiDefinitions : CRUDDefinition<SqlActHiIncident>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActHiIncident.Id), nameof(SqlActHiIncident.ProcDefKey), nameof(SqlActHiIncident.ProcDefId), nameof(SqlActHiIncident.RootProcInstId), nameof(SqlActHiIncident.ProcInstId), nameof(SqlActHiIncident.ExecutionId), nameof(SqlActHiIncident.CreateTime), nameof(SqlActHiIncident.EndTime), nameof(SqlActHiIncident.IncidentMsg));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActHiIncident.Id), nameof(SqlActHiIncident.ProcDefKey), nameof(SqlActHiIncident.ProcDefId), nameof(SqlActHiIncident.RootProcInstId), nameof(SqlActHiIncident.ProcInstId), nameof(SqlActHiIncident.ExecutionId), nameof(SqlActHiIncident.CreateTime), nameof(SqlActHiIncident.EndTime), nameof(SqlActHiIncident.IncidentMsg), nameof(SqlActHiIncident.IncidentType), nameof(SqlActHiIncident.ActivityId), nameof(SqlActHiIncident.FailedActivityId), nameof(SqlActHiIncident.CauseIncidentId), nameof(SqlActHiIncident.RootCauseIncidentId), nameof(SqlActHiIncident.Configuration), nameof(SqlActHiIncident.HistoryConfiguration), nameof(SqlActHiIncident.IncidentState), nameof(SqlActHiIncident.TenantId), nameof(SqlActHiIncident.JobDefId), nameof(SqlActHiIncident.Annotation), nameof(SqlActHiIncident.RemovalTime));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد سوابق موتور: رخداد خطا";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActHiJobLogUiDefinitions : CRUDDefinition<SqlActHiJobLog>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActHiJobLog.Id), nameof(SqlActHiJobLog.Timestamp), nameof(SqlActHiJobLog.JobId), nameof(SqlActHiJobLog.JobDuedate), nameof(SqlActHiJobLog.JobRetries), nameof(SqlActHiJobLog.JobPriority), nameof(SqlActHiJobLog.JobExceptionMsg), nameof(SqlActHiJobLog.JobExceptionStackId), nameof(SqlActHiJobLog.JobState));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActHiJobLog.Id), nameof(SqlActHiJobLog.Timestamp), nameof(SqlActHiJobLog.JobId), nameof(SqlActHiJobLog.JobDuedate), nameof(SqlActHiJobLog.JobRetries), nameof(SqlActHiJobLog.JobPriority), nameof(SqlActHiJobLog.JobExceptionMsg), nameof(SqlActHiJobLog.JobExceptionStackId), nameof(SqlActHiJobLog.JobState), nameof(SqlActHiJobLog.JobDefId), nameof(SqlActHiJobLog.JobDefType), nameof(SqlActHiJobLog.JobDefConfiguration), nameof(SqlActHiJobLog.ActId), nameof(SqlActHiJobLog.FailedActId), nameof(SqlActHiJobLog.ExecutionId), nameof(SqlActHiJobLog.RootProcInstId), nameof(SqlActHiJobLog.ProcessInstanceId), nameof(SqlActHiJobLog.ProcessDefId), nameof(SqlActHiJobLog.ProcessDefKey), nameof(SqlActHiJobLog.DeploymentId), nameof(SqlActHiJobLog.SequenceCounter), nameof(SqlActHiJobLog.TenantId), nameof(SqlActHiJobLog.Hostname), nameof(SqlActHiJobLog.RemovalTime), nameof(SqlActHiJobLog.BatchId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد سوابق موتور: سوابق";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActHiOpLogUiDefinitions : CRUDDefinition<SqlActHiOpLog>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActHiOpLog.Id), nameof(SqlActHiOpLog.DeploymentId), nameof(SqlActHiOpLog.ProcDefId), nameof(SqlActHiOpLog.ProcDefKey), nameof(SqlActHiOpLog.RootProcInstId), nameof(SqlActHiOpLog.ProcInstId), nameof(SqlActHiOpLog.ExecutionId), nameof(SqlActHiOpLog.CaseDefId), nameof(SqlActHiOpLog.CaseInstId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActHiOpLog.Id), nameof(SqlActHiOpLog.DeploymentId), nameof(SqlActHiOpLog.ProcDefId), nameof(SqlActHiOpLog.ProcDefKey), nameof(SqlActHiOpLog.RootProcInstId), nameof(SqlActHiOpLog.ProcInstId), nameof(SqlActHiOpLog.ExecutionId), nameof(SqlActHiOpLog.CaseDefId), nameof(SqlActHiOpLog.CaseInstId), nameof(SqlActHiOpLog.CaseExecutionId), nameof(SqlActHiOpLog.TaskId), nameof(SqlActHiOpLog.JobId), nameof(SqlActHiOpLog.JobDefId), nameof(SqlActHiOpLog.BatchId), nameof(SqlActHiOpLog.UserId), nameof(SqlActHiOpLog.Timestamp), nameof(SqlActHiOpLog.OperationType), nameof(SqlActHiOpLog.OperationId), nameof(SqlActHiOpLog.EntityType), nameof(SqlActHiOpLog.Property), nameof(SqlActHiOpLog.OrgValue), nameof(SqlActHiOpLog.NewValue), nameof(SqlActHiOpLog.TenantId), nameof(SqlActHiOpLog.RemovalTime), nameof(SqlActHiOpLog.Category), nameof(SqlActHiOpLog.ExternalTaskId), nameof(SqlActHiOpLog.Annotation));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد سوابق موتور: سوابق";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActHiProcinstUiDefinitions : CRUDDefinition<SqlActHiProcinst>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActHiProcinst.Id), nameof(SqlActHiProcinst.ProcInstId), nameof(SqlActHiProcinst.BusinessKey), nameof(SqlActHiProcinst.ProcDefKey), nameof(SqlActHiProcinst.ProcDefId), nameof(SqlActHiProcinst.StartTime), nameof(SqlActHiProcinst.EndTime), nameof(SqlActHiProcinst.RemovalTime), nameof(SqlActHiProcinst.Duration));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActHiProcinst.Id), nameof(SqlActHiProcinst.ProcInstId), nameof(SqlActHiProcinst.BusinessKey), nameof(SqlActHiProcinst.ProcDefKey), nameof(SqlActHiProcinst.ProcDefId), nameof(SqlActHiProcinst.StartTime), nameof(SqlActHiProcinst.EndTime), nameof(SqlActHiProcinst.RemovalTime), nameof(SqlActHiProcinst.Duration), nameof(SqlActHiProcinst.StartUserId), nameof(SqlActHiProcinst.StartActId), nameof(SqlActHiProcinst.EndActId), nameof(SqlActHiProcinst.SuperProcessInstanceId), nameof(SqlActHiProcinst.RootProcInstId), nameof(SqlActHiProcinst.SuperCaseInstanceId), nameof(SqlActHiProcinst.CaseInstId), nameof(SqlActHiProcinst.DeleteReason), nameof(SqlActHiProcinst.TenantId), nameof(SqlActHiProcinst.State), nameof(SqlActHiProcinst.RestartedProcInstId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد سوابق موتور: نمونه فرایند";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActHiTaskinstUiDefinitions : CRUDDefinition<SqlActHiTaskinst>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActHiTaskinst.Id), nameof(SqlActHiTaskinst.TaskDefKey), nameof(SqlActHiTaskinst.ProcDefKey), nameof(SqlActHiTaskinst.ProcDefId), nameof(SqlActHiTaskinst.RootProcInstId), nameof(SqlActHiTaskinst.ProcInstId), nameof(SqlActHiTaskinst.ExecutionId), nameof(SqlActHiTaskinst.CaseDefKey), nameof(SqlActHiTaskinst.CaseDefId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActHiTaskinst.Id), nameof(SqlActHiTaskinst.TaskDefKey), nameof(SqlActHiTaskinst.ProcDefKey), nameof(SqlActHiTaskinst.ProcDefId), nameof(SqlActHiTaskinst.RootProcInstId), nameof(SqlActHiTaskinst.ProcInstId), nameof(SqlActHiTaskinst.ExecutionId), nameof(SqlActHiTaskinst.CaseDefKey), nameof(SqlActHiTaskinst.CaseDefId), nameof(SqlActHiTaskinst.CaseInstId), nameof(SqlActHiTaskinst.CaseExecutionId), nameof(SqlActHiTaskinst.ActInstId), nameof(SqlActHiTaskinst.Name), nameof(SqlActHiTaskinst.ParentTaskId), nameof(SqlActHiTaskinst.Description), nameof(SqlActHiTaskinst.Owner), nameof(SqlActHiTaskinst.Assignee), nameof(SqlActHiTaskinst.StartTime), nameof(SqlActHiTaskinst.EndTime), nameof(SqlActHiTaskinst.Duration), nameof(SqlActHiTaskinst.DeleteReason), nameof(SqlActHiTaskinst.Priority), nameof(SqlActHiTaskinst.DueDate), nameof(SqlActHiTaskinst.FollowUpDate), nameof(SqlActHiTaskinst.TenantId), nameof(SqlActHiTaskinst.RemovalTime), nameof(SqlActHiTaskinst.TaskState));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد سوابق موتور: نمونه وظیفه";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActHiVarinstUiDefinitions : CRUDDefinition<SqlActHiVarinst>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActHiVarinst.Id), nameof(SqlActHiVarinst.ProcDefKey), nameof(SqlActHiVarinst.ProcDefId), nameof(SqlActHiVarinst.RootProcInstId), nameof(SqlActHiVarinst.ProcInstId), nameof(SqlActHiVarinst.ExecutionId), nameof(SqlActHiVarinst.CaseDefKey), nameof(SqlActHiVarinst.CaseDefId), nameof(SqlActHiVarinst.CaseInstId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActHiVarinst.Id), nameof(SqlActHiVarinst.ProcDefKey), nameof(SqlActHiVarinst.ProcDefId), nameof(SqlActHiVarinst.RootProcInstId), nameof(SqlActHiVarinst.ProcInstId), nameof(SqlActHiVarinst.ExecutionId), nameof(SqlActHiVarinst.CaseDefKey), nameof(SqlActHiVarinst.CaseDefId), nameof(SqlActHiVarinst.CaseInstId), nameof(SqlActHiVarinst.CaseExecutionId), nameof(SqlActHiVarinst.ActInstId), nameof(SqlActHiVarinst.TaskId), nameof(SqlActHiVarinst.Name), nameof(SqlActHiVarinst.VarType), nameof(SqlActHiVarinst.CreateTime), nameof(SqlActHiVarinst.Rev), nameof(SqlActHiVarinst.BytearrayId), nameof(SqlActHiVarinst.Double), nameof(SqlActHiVarinst.Long), nameof(SqlActHiVarinst.Text), nameof(SqlActHiVarinst.TenantId), nameof(SqlActHiVarinst.State), nameof(SqlActHiVarinst.RemovalTime));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد سوابق موتور: نمونه متغیر";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActIdGroupUiDefinitions : CRUDDefinition<SqlActIdGroup>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActIdGroup.Id), nameof(SqlActIdGroup.Rev), nameof(SqlActIdGroup.Name), nameof(SqlActIdGroup.Type));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActIdGroup.Id), nameof(SqlActIdGroup.Rev), nameof(SqlActIdGroup.Name), nameof(SqlActIdGroup.Type));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: گروه";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActIdInfoUiDefinitions : CRUDDefinition<SqlActIdInfo>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActIdInfo.Id), nameof(SqlActIdInfo.Rev), nameof(SqlActIdInfo.UserId), nameof(SqlActIdInfo.Type), nameof(SqlActIdInfo.Key), nameof(SqlActIdInfo.Value), nameof(SqlActIdInfo.ParentId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActIdInfo.Id), nameof(SqlActIdInfo.Rev), nameof(SqlActIdInfo.UserId), nameof(SqlActIdInfo.Type), nameof(SqlActIdInfo.Key), nameof(SqlActIdInfo.Value), nameof(SqlActIdInfo.ParentId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: اطلاعات";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActIdMembershipUiDefinitions : CRUDDefinition<SqlActIdMembership>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActIdMembership.UserId), nameof(SqlActIdMembership.GroupId), nameof(SqlActIdMembership.TenantId), nameof(SqlActIdMembership.Hash), nameof(SqlActIdMembership.RemovalTime));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActIdMembership.UserId), nameof(SqlActIdMembership.GroupId), nameof(SqlActIdMembership.TenantId), nameof(SqlActIdMembership.Hash), nameof(SqlActIdMembership.RemovalTime));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: عضویت";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActIdRememberMeUiDefinitions : CRUDDefinition<SqlActIdRememberMe>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActIdRememberMe.Id), nameof(SqlActIdRememberMe.Rev), nameof(SqlActIdRememberMe.Selector), nameof(SqlActIdRememberMe.Validator), nameof(SqlActIdRememberMe.UserId), nameof(SqlActIdRememberMe.LastUsed));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActIdRememberMe.Id), nameof(SqlActIdRememberMe.Rev), nameof(SqlActIdRememberMe.Selector), nameof(SqlActIdRememberMe.Validator), nameof(SqlActIdRememberMe.UserId), nameof(SqlActIdRememberMe.LastUsed));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: من";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActIdTenantUiDefinitions : CRUDDefinition<SqlActIdTenant>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActIdTenant.Id), nameof(SqlActIdTenant.Rev), nameof(SqlActIdTenant.Name), nameof(SqlActIdTenant.ParentId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActIdTenant.Id), nameof(SqlActIdTenant.Rev), nameof(SqlActIdTenant.Name), nameof(SqlActIdTenant.ParentId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: مستاجر";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActIdTenantMemberUiDefinitions : CRUDDefinition<SqlActIdTenantMember>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActIdTenantMember.Id), nameof(SqlActIdTenantMember.TenantId), nameof(SqlActIdTenantMember.UserId), nameof(SqlActIdTenantMember.GroupId), nameof(SqlActIdTenantMember.RemovalTime));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActIdTenantMember.Id), nameof(SqlActIdTenantMember.TenantId), nameof(SqlActIdTenantMember.UserId), nameof(SqlActIdTenantMember.GroupId), nameof(SqlActIdTenantMember.RemovalTime));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: عضو";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActIdUserUiDefinitions : CRUDDefinition<SqlActIdUser>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActIdUser.Id), nameof(SqlActIdUser.Rev), nameof(SqlActIdUser.First), nameof(SqlActIdUser.Last), nameof(SqlActIdUser.Email), nameof(SqlActIdUser.LockExpTime), nameof(SqlActIdUser.Attempts), nameof(SqlActIdUser.PictureId), nameof(SqlActIdUser.Mobile));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActIdUser.Id), nameof(SqlActIdUser.Rev), nameof(SqlActIdUser.First), nameof(SqlActIdUser.Last), nameof(SqlActIdUser.Email), nameof(SqlActIdUser.LockExpTime), nameof(SqlActIdUser.Attempts), nameof(SqlActIdUser.PictureId), nameof(SqlActIdUser.Mobile), nameof(SqlActIdUser.CreateTime), nameof(SqlActIdUser.UpdateTime), nameof(SqlActIdUser.Block), nameof(SqlActIdUser.Fullname), nameof(SqlActIdUser.Hash));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: کاربر";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActReAssociationUiDefinitions : CRUDDefinition<SqlActReAssociation>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActReAssociation.Id), nameof(SqlActReAssociation.Name), nameof(SqlActReAssociation.Rev), nameof(SqlActReAssociation.Type), nameof(SqlActReAssociation.TableId), nameof(SqlActReAssociation.IncomingName), nameof(SqlActReAssociation.OutcomingName), nameof(SqlActReAssociation.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActReAssociation.Id), nameof(SqlActReAssociation.Name), nameof(SqlActReAssociation.Rev), nameof(SqlActReAssociation.Type), nameof(SqlActReAssociation.TableId), nameof(SqlActReAssociation.IncomingName), nameof(SqlActReAssociation.OutcomingName), nameof(SqlActReAssociation.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: ارتباط";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActReCamformdefUiDefinitions : CRUDDefinition<SqlActReCamformdef>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActReCamformdef.Id), nameof(SqlActReCamformdef.Rev), nameof(SqlActReCamformdef.Key), nameof(SqlActReCamformdef.Version), nameof(SqlActReCamformdef.DeploymentId), nameof(SqlActReCamformdef.ResourceName), nameof(SqlActReCamformdef.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActReCamformdef.Id), nameof(SqlActReCamformdef.Rev), nameof(SqlActReCamformdef.Key), nameof(SqlActReCamformdef.Version), nameof(SqlActReCamformdef.DeploymentId), nameof(SqlActReCamformdef.ResourceName), nameof(SqlActReCamformdef.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: تعریف فرم موتور";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActReCaseDefUiDefinitions : CRUDDefinition<SqlActReCaseDef>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActReCaseDef.Id), nameof(SqlActReCaseDef.Rev), nameof(SqlActReCaseDef.Category), nameof(SqlActReCaseDef.Name), nameof(SqlActReCaseDef.Key), nameof(SqlActReCaseDef.Version), nameof(SqlActReCaseDef.DeploymentId), nameof(SqlActReCaseDef.ResourceName), nameof(SqlActReCaseDef.DgrmResourceName));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActReCaseDef.Id), nameof(SqlActReCaseDef.Rev), nameof(SqlActReCaseDef.Category), nameof(SqlActReCaseDef.Name), nameof(SqlActReCaseDef.Key), nameof(SqlActReCaseDef.Version), nameof(SqlActReCaseDef.DeploymentId), nameof(SqlActReCaseDef.ResourceName), nameof(SqlActReCaseDef.DgrmResourceName), nameof(SqlActReCaseDef.TenantId), nameof(SqlActReCaseDef.HistoryTtl));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: تعریف";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActReColumnUiDefinitions : CRUDDefinition<SqlActReColumn>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActReColumn.Id), nameof(SqlActReColumn.OldId), nameof(SqlActReColumn.Rev), nameof(SqlActReColumn.Index), nameof(SqlActReColumn.Name), nameof(SqlActReColumn.Nullable), nameof(SqlActReColumn.Type), nameof(SqlActReColumn.Key), nameof(SqlActReColumn.Normalize));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActReColumn.Id), nameof(SqlActReColumn.OldId), nameof(SqlActReColumn.Rev), nameof(SqlActReColumn.Index), nameof(SqlActReColumn.Name), nameof(SqlActReColumn.Nullable), nameof(SqlActReColumn.Type), nameof(SqlActReColumn.Key), nameof(SqlActReColumn.Normalize), nameof(SqlActReColumn.Details), nameof(SqlActReColumn.TableId), nameof(SqlActReColumn.Default), nameof(SqlActReColumn.TenantId), nameof(SqlActReColumn.Kind), nameof(SqlActReColumn.EscapeHtml));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: ستون";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActReConstraintUiDefinitions : CRUDDefinition<SqlActReConstraint>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActReConstraint.Id), nameof(SqlActReConstraint.Name), nameof(SqlActReConstraint.Rev), nameof(SqlActReConstraint.Type), nameof(SqlActReConstraint.TableId), nameof(SqlActReConstraint.ColumnId), nameof(SqlActReConstraint.ParentTableId), nameof(SqlActReConstraint.ParentColumnId), nameof(SqlActReConstraint.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActReConstraint.Id), nameof(SqlActReConstraint.Name), nameof(SqlActReConstraint.Rev), nameof(SqlActReConstraint.Type), nameof(SqlActReConstraint.TableId), nameof(SqlActReConstraint.ColumnId), nameof(SqlActReConstraint.ParentTableId), nameof(SqlActReConstraint.ParentColumnId), nameof(SqlActReConstraint.TenantId), nameof(SqlActReConstraint.UpdateRule), nameof(SqlActReConstraint.DeleteRule), nameof(SqlActReConstraint.SqlScript), nameof(SqlActReConstraint.ErrorMessage));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: محدودیت";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActReDashboardUiDefinitions : CRUDDefinition<SqlActReDashboard>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActReDashboard.Id), nameof(SqlActReDashboard.Rev), nameof(SqlActReDashboard.Name), nameof(SqlActReDashboard.Category), nameof(SqlActReDashboard.Definition), nameof(SqlActReDashboard.Default), nameof(SqlActReDashboard.CreateTime), nameof(SqlActReDashboard.UpdateTime), nameof(SqlActReDashboard.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActReDashboard.Id), nameof(SqlActReDashboard.Rev), nameof(SqlActReDashboard.Name), nameof(SqlActReDashboard.Category), nameof(SqlActReDashboard.Definition), nameof(SqlActReDashboard.Default), nameof(SqlActReDashboard.CreateTime), nameof(SqlActReDashboard.UpdateTime), nameof(SqlActReDashboard.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: داشبورد";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActReDataFormUiDefinitions : CRUDDefinition<SqlActReDataForm>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActReDataForm.Id), nameof(SqlActReDataForm.Rev), nameof(SqlActReDataForm.Name), nameof(SqlActReDataForm.Category), nameof(SqlActReDataForm.TableId), nameof(SqlActReDataForm.TenantId), nameof(SqlActReDataForm.CustomHints), nameof(SqlActReDataForm.Sync));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActReDataForm.Id), nameof(SqlActReDataForm.Rev), nameof(SqlActReDataForm.Name), nameof(SqlActReDataForm.Category), nameof(SqlActReDataForm.TableId), nameof(SqlActReDataForm.TenantId), nameof(SqlActReDataForm.CustomHints), nameof(SqlActReDataForm.Sync));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: فرم";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActReDataFormColumnUiDefinitions : CRUDDefinition<SqlActReDataFormColumn>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActReDataFormColumn.Id), nameof(SqlActReDataFormColumn.Rev), nameof(SqlActReDataFormColumn.Name), nameof(SqlActReDataFormColumn.Type), nameof(SqlActReDataFormColumn.Order), nameof(SqlActReDataFormColumn.Visible), nameof(SqlActReDataFormColumn.Summable), nameof(SqlActReDataFormColumn.Filterable), nameof(SqlActReDataFormColumn.Suggestable));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActReDataFormColumn.Id), nameof(SqlActReDataFormColumn.Rev), nameof(SqlActReDataFormColumn.Name), nameof(SqlActReDataFormColumn.Type), nameof(SqlActReDataFormColumn.Order), nameof(SqlActReDataFormColumn.Visible), nameof(SqlActReDataFormColumn.Summable), nameof(SqlActReDataFormColumn.Filterable), nameof(SqlActReDataFormColumn.Suggestable), nameof(SqlActReDataFormColumn.Category), nameof(SqlActReDataFormColumn.ShowType), nameof(SqlActReDataFormColumn.ActionType), nameof(SqlActReDataFormColumn.DataFormId), nameof(SqlActReDataFormColumn.RefTableId), nameof(SqlActReDataFormColumn.RefColumnId), nameof(SqlActReDataFormColumn.RefFormId), nameof(SqlActReDataFormColumn.RefFormRelations), nameof(SqlActReDataFormColumn.RefDataFormId), nameof(SqlActReDataFormColumn.RefAssociationTableId), nameof(SqlActReDataFormColumn.RefAssociationId), nameof(SqlActReDataFormColumn.ChainAssociations), nameof(SqlActReDataFormColumn.TenantId), nameof(SqlActReDataFormColumn.DataFormat), nameof(SqlActReDataFormColumn.SqlScript), nameof(SqlActReDataFormColumn.SqlResultType), nameof(SqlActReDataFormColumn.SuggestDetails), nameof(SqlActReDataFormColumn.OperationDetails), nameof(SqlActReDataFormColumn.OutFullData), nameof(SqlActReDataFormColumn.ReadonlyRelations), nameof(SqlActReDataFormColumn.Sortable), nameof(SqlActReDataFormColumn.Icon), nameof(SqlActReDataFormColumn.Exportable), nameof(SqlActReDataFormColumn.Tags));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: ستون";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActReDataFormFilterUiDefinitions : CRUDDefinition<SqlActReDataFormFilter>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActReDataFormFilter.Id), nameof(SqlActReDataFormFilter.Value), nameof(SqlActReDataFormFilter.DataFormId), nameof(SqlActReDataFormFilter.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActReDataFormFilter.Id), nameof(SqlActReDataFormFilter.Value), nameof(SqlActReDataFormFilter.DataFormId), nameof(SqlActReDataFormFilter.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: فیلتر";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActReDataFormHeaderUiDefinitions : CRUDDefinition<SqlActReDataFormHeader>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActReDataFormHeader.Id), nameof(SqlActReDataFormHeader.Rev), nameof(SqlActReDataFormHeader.Name), nameof(SqlActReDataFormHeader.Type), nameof(SqlActReDataFormHeader.Order), nameof(SqlActReDataFormHeader.ActionType), nameof(SqlActReDataFormHeader.DataFormId), nameof(SqlActReDataFormHeader.AssigneeVar), nameof(SqlActReDataFormHeader.RefFormId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActReDataFormHeader.Id), nameof(SqlActReDataFormHeader.Rev), nameof(SqlActReDataFormHeader.Name), nameof(SqlActReDataFormHeader.Type), nameof(SqlActReDataFormHeader.Order), nameof(SqlActReDataFormHeader.ActionType), nameof(SqlActReDataFormHeader.DataFormId), nameof(SqlActReDataFormHeader.AssigneeVar), nameof(SqlActReDataFormHeader.RefFormId), nameof(SqlActReDataFormHeader.ProcessKey), nameof(SqlActReDataFormHeader.ProcessKeyName), nameof(SqlActReDataFormHeader.ProcessTenantId), nameof(SqlActReDataFormHeader.ProcessBindingType), nameof(SqlActReDataFormHeader.ProcessTag), nameof(SqlActReDataFormHeader.TenantId), nameof(SqlActReDataFormHeader.InlineEdit), nameof(SqlActReDataFormHeader.ProcessNeedRow), nameof(SqlActReDataFormHeader.ExportType), nameof(SqlActReDataFormHeader.OutFullData), nameof(SqlActReDataFormHeader.ReadonlyRelations), nameof(SqlActReDataFormHeader.RefFormRelations), nameof(SqlActReDataFormHeader.ExportDetail), nameof(SqlActReDataFormHeader.ExportLimit));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: سرصفحه";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActReDataFormSortUiDefinitions : CRUDDefinition<SqlActReDataFormSort>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActReDataFormSort.Id), nameof(SqlActReDataFormSort.Value), nameof(SqlActReDataFormSort.DataFormId), nameof(SqlActReDataFormSort.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActReDataFormSort.Id), nameof(SqlActReDataFormSort.Value), nameof(SqlActReDataFormSort.DataFormId), nameof(SqlActReDataFormSort.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: مرتب‌سازی";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActReDataReportUiDefinitions : CRUDDefinition<SqlActReDataReport>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActReDataReport.Id), nameof(SqlActReDataReport.Rev), nameof(SqlActReDataReport.Name), nameof(SqlActReDataReport.Category), nameof(SqlActReDataReport.FormId), nameof(SqlActReDataReport.TenantId), nameof(SqlActReDataReport.CustomHints));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActReDataReport.Id), nameof(SqlActReDataReport.Rev), nameof(SqlActReDataReport.Name), nameof(SqlActReDataReport.Category), nameof(SqlActReDataReport.FormId), nameof(SqlActReDataReport.TenantId), nameof(SqlActReDataReport.CustomHints));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: گزارش";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActReDataReportColumnUiDefinitions : CRUDDefinition<SqlActReDataReportColumn>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActReDataReportColumn.Id), nameof(SqlActReDataReportColumn.Rev), nameof(SqlActReDataReportColumn.Name), nameof(SqlActReDataReportColumn.Type), nameof(SqlActReDataReportColumn.Order), nameof(SqlActReDataReportColumn.DataReportId), nameof(SqlActReDataReportColumn.RefDataFormId), nameof(SqlActReDataReportColumn.RefDataColumnId), nameof(SqlActReDataReportColumn.Aggregation));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActReDataReportColumn.Id), nameof(SqlActReDataReportColumn.Rev), nameof(SqlActReDataReportColumn.Name), nameof(SqlActReDataReportColumn.Type), nameof(SqlActReDataReportColumn.Order), nameof(SqlActReDataReportColumn.DataReportId), nameof(SqlActReDataReportColumn.RefDataFormId), nameof(SqlActReDataReportColumn.RefDataColumnId), nameof(SqlActReDataReportColumn.Aggregation), nameof(SqlActReDataReportColumn.DataFormat), nameof(SqlActReDataReportColumn.TenantId), nameof(SqlActReDataReportColumn.RefDataFilterColumnId), nameof(SqlActReDataReportColumn.Summable));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: ستون";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActReDataReportFilterUiDefinitions : CRUDDefinition<SqlActReDataReportFilter>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActReDataReportFilter.Id), nameof(SqlActReDataReportFilter.Value), nameof(SqlActReDataReportFilter.DataReportId), nameof(SqlActReDataReportFilter.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActReDataReportFilter.Id), nameof(SqlActReDataReportFilter.Value), nameof(SqlActReDataReportFilter.DataReportId), nameof(SqlActReDataReportFilter.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: فیلتر";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActReDataReportHeaderUiDefinitions : CRUDDefinition<SqlActReDataReportHeader>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActReDataReportHeader.Id), nameof(SqlActReDataReportHeader.Rev), nameof(SqlActReDataReportHeader.Name), nameof(SqlActReDataReportHeader.Type), nameof(SqlActReDataReportHeader.Order), nameof(SqlActReDataReportHeader.ExportType), nameof(SqlActReDataReportHeader.DataReportId), nameof(SqlActReDataReportHeader.TenantId), nameof(SqlActReDataReportHeader.ExportDetail));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActReDataReportHeader.Id), nameof(SqlActReDataReportHeader.Rev), nameof(SqlActReDataReportHeader.Name), nameof(SqlActReDataReportHeader.Type), nameof(SqlActReDataReportHeader.Order), nameof(SqlActReDataReportHeader.ExportType), nameof(SqlActReDataReportHeader.DataReportId), nameof(SqlActReDataReportHeader.TenantId), nameof(SqlActReDataReportHeader.ExportDetail), nameof(SqlActReDataReportHeader.ExportLimit));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: سرصفحه";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActReDataReportSortUiDefinitions : CRUDDefinition<SqlActReDataReportSort>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActReDataReportSort.Id), nameof(SqlActReDataReportSort.Value), nameof(SqlActReDataReportSort.DataReportId), nameof(SqlActReDataReportSort.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActReDataReportSort.Id), nameof(SqlActReDataReportSort.Value), nameof(SqlActReDataReportSort.DataReportId), nameof(SqlActReDataReportSort.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: مرتب‌سازی";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActReDecisionDefUiDefinitions : CRUDDefinition<SqlActReDecisionDef>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActReDecisionDef.Id), nameof(SqlActReDecisionDef.Rev), nameof(SqlActReDecisionDef.Category), nameof(SqlActReDecisionDef.Name), nameof(SqlActReDecisionDef.Key), nameof(SqlActReDecisionDef.Version), nameof(SqlActReDecisionDef.DeploymentId), nameof(SqlActReDecisionDef.ResourceName), nameof(SqlActReDecisionDef.DgrmResourceName));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActReDecisionDef.Id), nameof(SqlActReDecisionDef.Rev), nameof(SqlActReDecisionDef.Category), nameof(SqlActReDecisionDef.Name), nameof(SqlActReDecisionDef.Key), nameof(SqlActReDecisionDef.Version), nameof(SqlActReDecisionDef.DeploymentId), nameof(SqlActReDecisionDef.ResourceName), nameof(SqlActReDecisionDef.DgrmResourceName), nameof(SqlActReDecisionDef.DecReqId), nameof(SqlActReDecisionDef.DecReqKey), nameof(SqlActReDecisionDef.TenantId), nameof(SqlActReDecisionDef.HistoryTtl), nameof(SqlActReDecisionDef.VersionTag));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: تعریف";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActReDecisionReqDefUiDefinitions : CRUDDefinition<SqlActReDecisionReqDef>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActReDecisionReqDef.Id), nameof(SqlActReDecisionReqDef.Rev), nameof(SqlActReDecisionReqDef.Category), nameof(SqlActReDecisionReqDef.Name), nameof(SqlActReDecisionReqDef.Key), nameof(SqlActReDecisionReqDef.Version), nameof(SqlActReDecisionReqDef.DeploymentId), nameof(SqlActReDecisionReqDef.ResourceName), nameof(SqlActReDecisionReqDef.DgrmResourceName));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActReDecisionReqDef.Id), nameof(SqlActReDecisionReqDef.Rev), nameof(SqlActReDecisionReqDef.Category), nameof(SqlActReDecisionReqDef.Name), nameof(SqlActReDecisionReqDef.Key), nameof(SqlActReDecisionReqDef.Version), nameof(SqlActReDecisionReqDef.DeploymentId), nameof(SqlActReDecisionReqDef.ResourceName), nameof(SqlActReDecisionReqDef.DgrmResourceName), nameof(SqlActReDecisionReqDef.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: تعریف";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActReDeploymentUiDefinitions : CRUDDefinition<SqlActReDeployment>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActReDeployment.Id), nameof(SqlActReDeployment.Name), nameof(SqlActReDeployment.DeployTime), nameof(SqlActReDeployment.Source), nameof(SqlActReDeployment.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActReDeployment.Id), nameof(SqlActReDeployment.Name), nameof(SqlActReDeployment.DeployTime), nameof(SqlActReDeployment.Source), nameof(SqlActReDeployment.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: استقرار";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActReFormUiDefinitions : CRUDDefinition<SqlActReForm>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActReForm.Id), nameof(SqlActReForm.Rev), nameof(SqlActReForm.Name), nameof(SqlActReForm.Category), nameof(SqlActReForm.Definition), nameof(SqlActReForm.CreateTime), nameof(SqlActReForm.UpdateTime), nameof(SqlActReForm.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActReForm.Id), nameof(SqlActReForm.Rev), nameof(SqlActReForm.Name), nameof(SqlActReForm.Category), nameof(SqlActReForm.Definition), nameof(SqlActReForm.CreateTime), nameof(SqlActReForm.UpdateTime), nameof(SqlActReForm.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: فرم";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActReLinkUiDefinitions : CRUDDefinition<SqlActReLink>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActReLink.Id), nameof(SqlActReLink.Rev), nameof(SqlActReLink.Name), nameof(SqlActReLink.Description), nameof(SqlActReLink.Url), nameof(SqlActReLink.Order), nameof(SqlActReLink.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActReLink.Id), nameof(SqlActReLink.Rev), nameof(SqlActReLink.Name), nameof(SqlActReLink.Description), nameof(SqlActReLink.Url), nameof(SqlActReLink.Order), nameof(SqlActReLink.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: پیوند";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActReMenuUiDefinitions : CRUDDefinition<SqlActReMenu>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActReMenu.Id), nameof(SqlActReMenu.Rev), nameof(SqlActReMenu.Name), nameof(SqlActReMenu.Type), nameof(SqlActReMenu.Logo), nameof(SqlActReMenu.Detail), nameof(SqlActReMenu.Order), nameof(SqlActReMenu.ParentId), nameof(SqlActReMenu.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActReMenu.Id), nameof(SqlActReMenu.Rev), nameof(SqlActReMenu.Name), nameof(SqlActReMenu.Type), nameof(SqlActReMenu.Logo), nameof(SqlActReMenu.Detail), nameof(SqlActReMenu.Order), nameof(SqlActReMenu.ParentId), nameof(SqlActReMenu.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: منو";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActReModelUiDefinitions : CRUDDefinition<SqlActReModel>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActReModel.Id), nameof(SqlActReModel.Rev), nameof(SqlActReModel.Name), nameof(SqlActReModel.Category), nameof(SqlActReModel.Definition), nameof(SqlActReModel.Type), nameof(SqlActReModel.CreateTime), nameof(SqlActReModel.UpdateTime), nameof(SqlActReModel.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActReModel.Id), nameof(SqlActReModel.Rev), nameof(SqlActReModel.Name), nameof(SqlActReModel.Category), nameof(SqlActReModel.Definition), nameof(SqlActReModel.Type), nameof(SqlActReModel.CreateTime), nameof(SqlActReModel.UpdateTime), nameof(SqlActReModel.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: مدل";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActReNotificationUiDefinitions : CRUDDefinition<SqlActReNotification>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActReNotification.Id), nameof(SqlActReNotification.Rev), nameof(SqlActReNotification.Sender), nameof(SqlActReNotification.GroupId), nameof(SqlActReNotification.UserId), nameof(SqlActReNotification.Message), nameof(SqlActReNotification.Type), nameof(SqlActReNotification.Target), nameof(SqlActReNotification.Enable));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActReNotification.Id), nameof(SqlActReNotification.Rev), nameof(SqlActReNotification.Sender), nameof(SqlActReNotification.GroupId), nameof(SqlActReNotification.UserId), nameof(SqlActReNotification.Message), nameof(SqlActReNotification.Type), nameof(SqlActReNotification.Target), nameof(SqlActReNotification.Enable), nameof(SqlActReNotification.StartTime), nameof(SqlActReNotification.EndTime), nameof(SqlActReNotification.ChangeTime), nameof(SqlActReNotification.TenantId), nameof(SqlActReNotification.Details));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: اعلان";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActReNotificationStatusUiDefinitions : CRUDDefinition<SqlActReNotificationStatus>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActReNotificationStatus.NotificationId), nameof(SqlActReNotificationStatus.UserId), nameof(SqlActReNotificationStatus.Read), nameof(SqlActReNotificationStatus.Deleted));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActReNotificationStatus.NotificationId), nameof(SqlActReNotificationStatus.UserId), nameof(SqlActReNotificationStatus.Read), nameof(SqlActReNotificationStatus.Deleted));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: وضعیت";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActReProcdefUiDefinitions : CRUDDefinition<SqlActReProcdef>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActReProcdef.Id), nameof(SqlActReProcdef.Rev), nameof(SqlActReProcdef.Category), nameof(SqlActReProcdef.Name), nameof(SqlActReProcdef.Key), nameof(SqlActReProcdef.Version), nameof(SqlActReProcdef.DeploymentId), nameof(SqlActReProcdef.ResourceName), nameof(SqlActReProcdef.DgrmResourceName));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActReProcdef.Id), nameof(SqlActReProcdef.Rev), nameof(SqlActReProcdef.Category), nameof(SqlActReProcdef.Name), nameof(SqlActReProcdef.Key), nameof(SqlActReProcdef.Version), nameof(SqlActReProcdef.DeploymentId), nameof(SqlActReProcdef.ResourceName), nameof(SqlActReProcdef.DgrmResourceName), nameof(SqlActReProcdef.HasStartFormKey), nameof(SqlActReProcdef.SuspensionState), nameof(SqlActReProcdef.TenantId), nameof(SqlActReProcdef.VersionTag), nameof(SqlActReProcdef.HistoryTtl), nameof(SqlActReProcdef.Startable));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: تعریف فرایند";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActReQueryUiDefinitions : CRUDDefinition<SqlActReQuery>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActReQuery.Id), nameof(SqlActReQuery.Rev), nameof(SqlActReQuery.Name), nameof(SqlActReQuery.Category), nameof(SqlActReQuery.Type), nameof(SqlActReQuery.Definition), nameof(SqlActReQuery.CreateTime), nameof(SqlActReQuery.UpdateTime), nameof(SqlActReQuery.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActReQuery.Id), nameof(SqlActReQuery.Rev), nameof(SqlActReQuery.Name), nameof(SqlActReQuery.Category), nameof(SqlActReQuery.Type), nameof(SqlActReQuery.Definition), nameof(SqlActReQuery.CreateTime), nameof(SqlActReQuery.UpdateTime), nameof(SqlActReQuery.TenantId), nameof(SqlActReQuery.ExportType), nameof(SqlActReQuery.ExportDetail), nameof(SqlActReQuery.ExportLimit));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: پرس‌وجو";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActReReportUiDefinitions : CRUDDefinition<SqlActReReport>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActReReport.Id), nameof(SqlActReReport.Rev), nameof(SqlActReReport.Name), nameof(SqlActReReport.Folder), nameof(SqlActReReport.Definition), nameof(SqlActReReport.CreateTime), nameof(SqlActReReport.UpdateTime), nameof(SqlActReReport.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActReReport.Id), nameof(SqlActReReport.Rev), nameof(SqlActReReport.Name), nameof(SqlActReReport.Folder), nameof(SqlActReReport.Definition), nameof(SqlActReReport.CreateTime), nameof(SqlActReReport.UpdateTime), nameof(SqlActReReport.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: گزارش";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActReTableUiDefinitions : CRUDDefinition<SqlActReTable>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActReTable.Id), nameof(SqlActReTable.Rev), nameof(SqlActReTable.Name), nameof(SqlActReTable.Category), nameof(SqlActReTable.Sync), nameof(SqlActReTable.Default), nameof(SqlActReTable.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActReTable.Id), nameof(SqlActReTable.Rev), nameof(SqlActReTable.Name), nameof(SqlActReTable.Category), nameof(SqlActReTable.Sync), nameof(SqlActReTable.Default), nameof(SqlActReTable.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: جدول";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActReTopicUiDefinitions : CRUDDefinition<SqlActReTopic>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActReTopic.Id), nameof(SqlActReTopic.Rev), nameof(SqlActReTopic.Name), nameof(SqlActReTopic.Type), nameof(SqlActReTopic.PackagesClass), nameof(SqlActReTopic.Default), nameof(SqlActReTopic.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActReTopic.Id), nameof(SqlActReTopic.Rev), nameof(SqlActReTopic.Name), nameof(SqlActReTopic.Type), nameof(SqlActReTopic.PackagesClass), nameof(SqlActReTopic.Default), nameof(SqlActReTopic.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: موضوع";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActRuAuthorizationUiDefinitions : CRUDDefinition<SqlActRuAuthorization>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActRuAuthorization.Id), nameof(SqlActRuAuthorization.Rev), nameof(SqlActRuAuthorization.Type), nameof(SqlActRuAuthorization.GroupId), nameof(SqlActRuAuthorization.UserId), nameof(SqlActRuAuthorization.ResourceType), nameof(SqlActRuAuthorization.ResourceId), nameof(SqlActRuAuthorization.Perms), nameof(SqlActRuAuthorization.RemovalTime));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActRuAuthorization.Id), nameof(SqlActRuAuthorization.Rev), nameof(SqlActRuAuthorization.Type), nameof(SqlActRuAuthorization.GroupId), nameof(SqlActRuAuthorization.UserId), nameof(SqlActRuAuthorization.ResourceType), nameof(SqlActRuAuthorization.ResourceId), nameof(SqlActRuAuthorization.Perms), nameof(SqlActRuAuthorization.RemovalTime), nameof(SqlActRuAuthorization.RootProcInstId), nameof(SqlActRuAuthorization.Hash));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: مجوز";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActRuBatchUiDefinitions : CRUDDefinition<SqlActRuBatch>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActRuBatch.Id), nameof(SqlActRuBatch.Rev), nameof(SqlActRuBatch.Type), nameof(SqlActRuBatch.TotalJobs), nameof(SqlActRuBatch.JobsCreated), nameof(SqlActRuBatch.InvocationsPerJob), nameof(SqlActRuBatch.BatchJobDefId), nameof(SqlActRuBatch.MonitorJobDefId), nameof(SqlActRuBatch.SuspensionState));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActRuBatch.Id), nameof(SqlActRuBatch.Rev), nameof(SqlActRuBatch.Type), nameof(SqlActRuBatch.TotalJobs), nameof(SqlActRuBatch.JobsCreated), nameof(SqlActRuBatch.InvocationsPerJob), nameof(SqlActRuBatch.BatchJobDefId), nameof(SqlActRuBatch.MonitorJobDefId), nameof(SqlActRuBatch.SuspensionState), nameof(SqlActRuBatch.Configuration), nameof(SqlActRuBatch.TenantId), nameof(SqlActRuBatch.CreateUserId), nameof(SqlActRuBatch.StartTime));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: دسته";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActRuCaseExecutionUiDefinitions : CRUDDefinition<SqlActRuCaseExecution>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActRuCaseExecution.Id), nameof(SqlActRuCaseExecution.Rev), nameof(SqlActRuCaseExecution.CaseInstId), nameof(SqlActRuCaseExecution.SuperCaseExec), nameof(SqlActRuCaseExecution.SuperExec), nameof(SqlActRuCaseExecution.BusinessKey), nameof(SqlActRuCaseExecution.ParentId), nameof(SqlActRuCaseExecution.CaseDefId), nameof(SqlActRuCaseExecution.ActId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActRuCaseExecution.Id), nameof(SqlActRuCaseExecution.Rev), nameof(SqlActRuCaseExecution.CaseInstId), nameof(SqlActRuCaseExecution.SuperCaseExec), nameof(SqlActRuCaseExecution.SuperExec), nameof(SqlActRuCaseExecution.BusinessKey), nameof(SqlActRuCaseExecution.ParentId), nameof(SqlActRuCaseExecution.CaseDefId), nameof(SqlActRuCaseExecution.ActId), nameof(SqlActRuCaseExecution.PrevState), nameof(SqlActRuCaseExecution.CurrentState), nameof(SqlActRuCaseExecution.Required), nameof(SqlActRuCaseExecution.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: اجرا";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActRuCaseSentryPartUiDefinitions : CRUDDefinition<SqlActRuCaseSentryPart>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActRuCaseSentryPart.Id), nameof(SqlActRuCaseSentryPart.Rev), nameof(SqlActRuCaseSentryPart.CaseInstId), nameof(SqlActRuCaseSentryPart.CaseExecId), nameof(SqlActRuCaseSentryPart.SentryId), nameof(SqlActRuCaseSentryPart.Type), nameof(SqlActRuCaseSentryPart.SourceCaseExecId), nameof(SqlActRuCaseSentryPart.StandardEvent), nameof(SqlActRuCaseSentryPart.Source));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActRuCaseSentryPart.Id), nameof(SqlActRuCaseSentryPart.Rev), nameof(SqlActRuCaseSentryPart.CaseInstId), nameof(SqlActRuCaseSentryPart.CaseExecId), nameof(SqlActRuCaseSentryPart.SentryId), nameof(SqlActRuCaseSentryPart.Type), nameof(SqlActRuCaseSentryPart.SourceCaseExecId), nameof(SqlActRuCaseSentryPart.StandardEvent), nameof(SqlActRuCaseSentryPart.Source), nameof(SqlActRuCaseSentryPart.VariableEvent), nameof(SqlActRuCaseSentryPart.VariableName), nameof(SqlActRuCaseSentryPart.Satisfied), nameof(SqlActRuCaseSentryPart.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: بخش";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActRuChangeQueueUiDefinitions : CRUDDefinition<SqlActRuChangeQueue>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActRuChangeQueue.Id), nameof(SqlActRuChangeQueue.Time), nameof(SqlActRuChangeQueue.Type), nameof(SqlActRuChangeQueue.Action), nameof(SqlActRuChangeQueue.TargetId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActRuChangeQueue.Id), nameof(SqlActRuChangeQueue.Time), nameof(SqlActRuChangeQueue.Type), nameof(SqlActRuChangeQueue.Action), nameof(SqlActRuChangeQueue.TargetId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: صف";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActRuEventSubscrUiDefinitions : CRUDDefinition<SqlActRuEventSubscr>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActRuEventSubscr.Id), nameof(SqlActRuEventSubscr.Rev), nameof(SqlActRuEventSubscr.EventType), nameof(SqlActRuEventSubscr.EventName), nameof(SqlActRuEventSubscr.ExecutionId), nameof(SqlActRuEventSubscr.ProcInstId), nameof(SqlActRuEventSubscr.ActivityId), nameof(SqlActRuEventSubscr.Configuration), nameof(SqlActRuEventSubscr.Created));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActRuEventSubscr.Id), nameof(SqlActRuEventSubscr.Rev), nameof(SqlActRuEventSubscr.EventType), nameof(SqlActRuEventSubscr.EventName), nameof(SqlActRuEventSubscr.ExecutionId), nameof(SqlActRuEventSubscr.ProcInstId), nameof(SqlActRuEventSubscr.ActivityId), nameof(SqlActRuEventSubscr.Configuration), nameof(SqlActRuEventSubscr.Created), nameof(SqlActRuEventSubscr.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: اشتراک";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActRuExecutionUiDefinitions : CRUDDefinition<SqlActRuExecution>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActRuExecution.Id), nameof(SqlActRuExecution.Rev), nameof(SqlActRuExecution.RootProcInstId), nameof(SqlActRuExecution.ProcInstId), nameof(SqlActRuExecution.BusinessKey), nameof(SqlActRuExecution.ParentId), nameof(SqlActRuExecution.ProcDefId), nameof(SqlActRuExecution.SuperExec), nameof(SqlActRuExecution.SuperCaseExec));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActRuExecution.Id), nameof(SqlActRuExecution.Rev), nameof(SqlActRuExecution.RootProcInstId), nameof(SqlActRuExecution.ProcInstId), nameof(SqlActRuExecution.BusinessKey), nameof(SqlActRuExecution.ParentId), nameof(SqlActRuExecution.ProcDefId), nameof(SqlActRuExecution.SuperExec), nameof(SqlActRuExecution.SuperCaseExec), nameof(SqlActRuExecution.CaseInstId), nameof(SqlActRuExecution.ActId), nameof(SqlActRuExecution.ActInstId), nameof(SqlActRuExecution.IsActive), nameof(SqlActRuExecution.IsConcurrent), nameof(SqlActRuExecution.IsScope), nameof(SqlActRuExecution.IsEventScope), nameof(SqlActRuExecution.SuspensionState), nameof(SqlActRuExecution.CachedEntState), nameof(SqlActRuExecution.SequenceCounter), nameof(SqlActRuExecution.TenantId), nameof(SqlActRuExecution.ProcDefKey));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: اجرا";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActRuExtTaskUiDefinitions : CRUDDefinition<SqlActRuExtTask>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActRuExtTask.Id), nameof(SqlActRuExtTask.Rev), nameof(SqlActRuExtTask.WorkerId), nameof(SqlActRuExtTask.TopicName), nameof(SqlActRuExtTask.Retries), nameof(SqlActRuExtTask.ErrorMsg), nameof(SqlActRuExtTask.ErrorDetailsId), nameof(SqlActRuExtTask.LockExpTime), nameof(SqlActRuExtTask.SuspensionState));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActRuExtTask.Id), nameof(SqlActRuExtTask.Rev), nameof(SqlActRuExtTask.WorkerId), nameof(SqlActRuExtTask.TopicName), nameof(SqlActRuExtTask.Retries), nameof(SqlActRuExtTask.ErrorMsg), nameof(SqlActRuExtTask.ErrorDetailsId), nameof(SqlActRuExtTask.LockExpTime), nameof(SqlActRuExtTask.SuspensionState), nameof(SqlActRuExtTask.ExecutionId), nameof(SqlActRuExtTask.ProcInstId), nameof(SqlActRuExtTask.ProcDefId), nameof(SqlActRuExtTask.ProcDefKey), nameof(SqlActRuExtTask.ActId), nameof(SqlActRuExtTask.ActInstId), nameof(SqlActRuExtTask.TenantId), nameof(SqlActRuExtTask.Priority), nameof(SqlActRuExtTask.LastFailureLogId), nameof(SqlActRuExtTask.CreateTime));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: وظیفه";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActRuFilterUiDefinitions : CRUDDefinition<SqlActRuFilter>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActRuFilter.Id), nameof(SqlActRuFilter.Rev), nameof(SqlActRuFilter.ResourceType), nameof(SqlActRuFilter.Name), nameof(SqlActRuFilter.Owner), nameof(SqlActRuFilter.Query), nameof(SqlActRuFilter.Properties));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActRuFilter.Id), nameof(SqlActRuFilter.Rev), nameof(SqlActRuFilter.ResourceType), nameof(SqlActRuFilter.Name), nameof(SqlActRuFilter.Owner), nameof(SqlActRuFilter.Query), nameof(SqlActRuFilter.Properties));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: فیلتر";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActRuIdentitylinkUiDefinitions : CRUDDefinition<SqlActRuIdentitylink>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActRuIdentitylink.Id), nameof(SqlActRuIdentitylink.Rev), nameof(SqlActRuIdentitylink.GroupId), nameof(SqlActRuIdentitylink.Type), nameof(SqlActRuIdentitylink.UserId), nameof(SqlActRuIdentitylink.TaskId), nameof(SqlActRuIdentitylink.ProcDefId), nameof(SqlActRuIdentitylink.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActRuIdentitylink.Id), nameof(SqlActRuIdentitylink.Rev), nameof(SqlActRuIdentitylink.GroupId), nameof(SqlActRuIdentitylink.Type), nameof(SqlActRuIdentitylink.UserId), nameof(SqlActRuIdentitylink.TaskId), nameof(SqlActRuIdentitylink.ProcDefId), nameof(SqlActRuIdentitylink.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: پیوند هویت";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActRuIncidentUiDefinitions : CRUDDefinition<SqlActRuIncident>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActRuIncident.Id), nameof(SqlActRuIncident.Rev), nameof(SqlActRuIncident.IncidentTimestamp), nameof(SqlActRuIncident.IncidentMsg), nameof(SqlActRuIncident.IncidentType), nameof(SqlActRuIncident.ExecutionId), nameof(SqlActRuIncident.ActivityId), nameof(SqlActRuIncident.FailedActivityId), nameof(SqlActRuIncident.ProcInstId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActRuIncident.Id), nameof(SqlActRuIncident.Rev), nameof(SqlActRuIncident.IncidentTimestamp), nameof(SqlActRuIncident.IncidentMsg), nameof(SqlActRuIncident.IncidentType), nameof(SqlActRuIncident.ExecutionId), nameof(SqlActRuIncident.ActivityId), nameof(SqlActRuIncident.FailedActivityId), nameof(SqlActRuIncident.ProcInstId), nameof(SqlActRuIncident.ProcDefId), nameof(SqlActRuIncident.CauseIncidentId), nameof(SqlActRuIncident.RootCauseIncidentId), nameof(SqlActRuIncident.Configuration), nameof(SqlActRuIncident.TenantId), nameof(SqlActRuIncident.JobDefId), nameof(SqlActRuIncident.Annotation));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: رخداد خطا";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActRuJobUiDefinitions : CRUDDefinition<SqlActRuJob>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActRuJob.Id), nameof(SqlActRuJob.Rev), nameof(SqlActRuJob.Type), nameof(SqlActRuJob.LockExpTime), nameof(SqlActRuJob.LockOwner), nameof(SqlActRuJob.Exclusive), nameof(SqlActRuJob.ExecutionId), nameof(SqlActRuJob.ProcessInstanceId), nameof(SqlActRuJob.ProcessDefId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActRuJob.Id), nameof(SqlActRuJob.Rev), nameof(SqlActRuJob.Type), nameof(SqlActRuJob.LockExpTime), nameof(SqlActRuJob.LockOwner), nameof(SqlActRuJob.Exclusive), nameof(SqlActRuJob.ExecutionId), nameof(SqlActRuJob.ProcessInstanceId), nameof(SqlActRuJob.ProcessDefId), nameof(SqlActRuJob.ProcessDefKey), nameof(SqlActRuJob.Retries), nameof(SqlActRuJob.ExceptionStackId), nameof(SqlActRuJob.ExceptionMsg), nameof(SqlActRuJob.FailedActId), nameof(SqlActRuJob.Duedate), nameof(SqlActRuJob.Repeat), nameof(SqlActRuJob.RepeatOffset), nameof(SqlActRuJob.HandlerType), nameof(SqlActRuJob.HandlerCfg), nameof(SqlActRuJob.DeploymentId), nameof(SqlActRuJob.SuspensionState), nameof(SqlActRuJob.Priority), nameof(SqlActRuJob.JobDefId), nameof(SqlActRuJob.SequenceCounter), nameof(SqlActRuJob.TenantId), nameof(SqlActRuJob.CreateTime), nameof(SqlActRuJob.LastFailureLogId), nameof(SqlActRuJob.RootProcInstId), nameof(SqlActRuJob.Username), nameof(SqlActRuJob.BatchId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: کار پس‌زمینه";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActRuJobdefUiDefinitions : CRUDDefinition<SqlActRuJobdef>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActRuJobdef.Id), nameof(SqlActRuJobdef.Rev), nameof(SqlActRuJobdef.ProcDefId), nameof(SqlActRuJobdef.ProcDefKey), nameof(SqlActRuJobdef.ActId), nameof(SqlActRuJobdef.JobType), nameof(SqlActRuJobdef.JobConfiguration), nameof(SqlActRuJobdef.SuspensionState), nameof(SqlActRuJobdef.JobPriority));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActRuJobdef.Id), nameof(SqlActRuJobdef.Rev), nameof(SqlActRuJobdef.ProcDefId), nameof(SqlActRuJobdef.ProcDefKey), nameof(SqlActRuJobdef.ActId), nameof(SqlActRuJobdef.JobType), nameof(SqlActRuJobdef.JobConfiguration), nameof(SqlActRuJobdef.SuspensionState), nameof(SqlActRuJobdef.JobPriority), nameof(SqlActRuJobdef.TenantId), nameof(SqlActRuJobdef.DeploymentId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: تعریف کار پس‌زمینه";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActRuMeterLogUiDefinitions : CRUDDefinition<SqlActRuMeterLog>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActRuMeterLog.Id), nameof(SqlActRuMeterLog.Name), nameof(SqlActRuMeterLog.Reporter), nameof(SqlActRuMeterLog.Value), nameof(SqlActRuMeterLog.Timestamp), nameof(SqlActRuMeterLog.Milliseconds));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActRuMeterLog.Id), nameof(SqlActRuMeterLog.Name), nameof(SqlActRuMeterLog.Reporter), nameof(SqlActRuMeterLog.Value), nameof(SqlActRuMeterLog.Timestamp), nameof(SqlActRuMeterLog.Milliseconds));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: سوابق";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActRuTaskUiDefinitions : CRUDDefinition<SqlActRuTask>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActRuTask.Id), nameof(SqlActRuTask.Rev), nameof(SqlActRuTask.ExecutionId), nameof(SqlActRuTask.ProcInstId), nameof(SqlActRuTask.ProcDefId), nameof(SqlActRuTask.CaseExecutionId), nameof(SqlActRuTask.CaseInstId), nameof(SqlActRuTask.CaseDefId), nameof(SqlActRuTask.Name));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActRuTask.Id), nameof(SqlActRuTask.Rev), nameof(SqlActRuTask.ExecutionId), nameof(SqlActRuTask.ProcInstId), nameof(SqlActRuTask.ProcDefId), nameof(SqlActRuTask.CaseExecutionId), nameof(SqlActRuTask.CaseInstId), nameof(SqlActRuTask.CaseDefId), nameof(SqlActRuTask.Name), nameof(SqlActRuTask.ParentTaskId), nameof(SqlActRuTask.Description), nameof(SqlActRuTask.TaskDefKey), nameof(SqlActRuTask.Owner), nameof(SqlActRuTask.Assignee), nameof(SqlActRuTask.Delegation), nameof(SqlActRuTask.Priority), nameof(SqlActRuTask.CreateTime), nameof(SqlActRuTask.DueDate), nameof(SqlActRuTask.FollowUpDate), nameof(SqlActRuTask.SuspensionState), nameof(SqlActRuTask.TenantId), nameof(SqlActRuTask.LastUpdated), nameof(SqlActRuTask.TaskState));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: وظیفه";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActRuTaskMeterLogUiDefinitions : CRUDDefinition<SqlActRuTaskMeterLog>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActRuTaskMeterLog.Id), nameof(SqlActRuTaskMeterLog.AssigneeHash), nameof(SqlActRuTaskMeterLog.Timestamp));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActRuTaskMeterLog.Id), nameof(SqlActRuTaskMeterLog.AssigneeHash), nameof(SqlActRuTaskMeterLog.Timestamp));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: سوابق";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlActRuVariableUiDefinitions : CRUDDefinition<SqlActRuVariable>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlActRuVariable.Id), nameof(SqlActRuVariable.Rev), nameof(SqlActRuVariable.Type), nameof(SqlActRuVariable.Name), nameof(SqlActRuVariable.ExecutionId), nameof(SqlActRuVariable.ProcInstId), nameof(SqlActRuVariable.ProcDefId), nameof(SqlActRuVariable.CaseExecutionId), nameof(SqlActRuVariable.CaseInstId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlActRuVariable.Id), nameof(SqlActRuVariable.Rev), nameof(SqlActRuVariable.Type), nameof(SqlActRuVariable.Name), nameof(SqlActRuVariable.ExecutionId), nameof(SqlActRuVariable.ProcInstId), nameof(SqlActRuVariable.ProcDefId), nameof(SqlActRuVariable.CaseExecutionId), nameof(SqlActRuVariable.CaseInstId), nameof(SqlActRuVariable.TaskId), nameof(SqlActRuVariable.BatchId), nameof(SqlActRuVariable.BytearrayId), nameof(SqlActRuVariable.Double), nameof(SqlActRuVariable.Long), nameof(SqlActRuVariable.Text), nameof(SqlActRuVariable.VarScope), nameof(SqlActRuVariable.SequenceCounter), nameof(SqlActRuVariable.IsConcurrentLocal), nameof(SqlActRuVariable.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موتور: متغیر";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlExternalintegrationconnectionsUiDefinitions : CRUDDefinition<SqlExternalintegrationconnections>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlExternalintegrationconnections.Id), nameof(SqlExternalintegrationconnections.Shopid), nameof(SqlExternalintegrationconnections.Tenantid), nameof(SqlExternalintegrationconnections.Provider), nameof(SqlExternalintegrationconnections.Displayname), nameof(SqlExternalintegrationconnections.Accountidentifier), nameof(SqlExternalintegrationconnections.Credentialtype), nameof(SqlExternalintegrationconnections.Expiresatutc), nameof(SqlExternalintegrationconnections.Isenabled));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlExternalintegrationconnections.Shopid), nameof(SqlExternalintegrationconnections.Tenantid), nameof(SqlExternalintegrationconnections.Provider), nameof(SqlExternalintegrationconnections.Displayname), nameof(SqlExternalintegrationconnections.Accountidentifier), nameof(SqlExternalintegrationconnections.Credentialtype), nameof(SqlExternalintegrationconnections.Expiresatutc), nameof(SqlExternalintegrationconnections.Isenabled), nameof(SqlExternalintegrationconnections.Connectedatutc), nameof(SqlExternalintegrationconnections.Lastsyncatutc), nameof(SqlExternalintegrationconnections.Lasterror));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد خارجی یکسان‌سازی اتصال‌ها";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByProviderConfig : ChartConfigDefinition
        {
            public ByProviderConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "خارجی یکسان‌سازی اتصال‌ها به تفکیک پلتفرم";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlExternalintegrationconnections.Provider), "پلتفرم"); Count(null, "تعداد"); }
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "خارجی یکسان‌سازی اتصال‌ها به تفکیک مغازه شناسه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlExternalintegrationconnections.Shopid), "مغازه شناسه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlExternalordermappingsUiDefinitions : CRUDDefinition<SqlExternalordermappings>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlExternalordermappings.Id), nameof(SqlExternalordermappings.Connectionid), nameof(SqlExternalordermappings.Shopid), nameof(SqlExternalordermappings.Hypersaleorderid), nameof(SqlExternalordermappings.Externalorderid), nameof(SqlExternalordermappings.Externalparcelid), nameof(SqlExternalordermappings.Status), nameof(SqlExternalordermappings.Lastsyncatutc));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlExternalordermappings.Connectionid), nameof(SqlExternalordermappings.Shopid), nameof(SqlExternalordermappings.Hypersaleorderid), nameof(SqlExternalordermappings.Externalorderid), nameof(SqlExternalordermappings.Externalparcelid), nameof(SqlExternalordermappings.Status), nameof(SqlExternalordermappings.Lastsyncatutc));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد خارجی سفارش نگاشت‌ها";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByStatusConfig : ChartConfigDefinition
        {
            public ByStatusConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "خارجی سفارش نگاشت‌ها به تفکیک وضعیت";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlExternalordermappings.Status), "وضعیت"); Count(null, "تعداد"); }
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "خارجی سفارش نگاشت‌ها به تفکیک مغازه شناسه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlExternalordermappings.Shopid), "مغازه شناسه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlExternalproductmappingsUiDefinitions : CRUDDefinition<SqlExternalproductmappings>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlExternalproductmappings.Id), nameof(SqlExternalproductmappings.Connectionid), nameof(SqlExternalproductmappings.Shopid), nameof(SqlExternalproductmappings.Hyperproductid), nameof(SqlExternalproductmappings.Externalproductid), nameof(SqlExternalproductmappings.Externalsku), nameof(SqlExternalproductmappings.Externalvariantid), nameof(SqlExternalproductmappings.Lastexternalprice), nameof(SqlExternalproductmappings.Lastexternalinventory));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlExternalproductmappings.Connectionid), nameof(SqlExternalproductmappings.Shopid), nameof(SqlExternalproductmappings.Hyperproductid), nameof(SqlExternalproductmappings.Externalproductid), nameof(SqlExternalproductmappings.Externalsku), nameof(SqlExternalproductmappings.Externalvariantid), nameof(SqlExternalproductmappings.Lastexternalprice), nameof(SqlExternalproductmappings.Lastexternalinventory), nameof(SqlExternalproductmappings.Lastsyncatutc), nameof(SqlExternalproductmappings.Isactive));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد خارجی کالا نگاشت‌ها";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "خارجی کالا نگاشت‌ها به تفکیک مغازه شناسه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlExternalproductmappings.Shopid), "مغازه شناسه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlIntegrationadminsimulationsUiDefinitions : CRUDDefinition<SqlIntegrationadminsimulations>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlIntegrationadminsimulations.Id), nameof(SqlIntegrationadminsimulations.Adminuserid), nameof(SqlIntegrationadminsimulations.Shopid), nameof(SqlIntegrationadminsimulations.Merchantidentifier), nameof(SqlIntegrationadminsimulations.Tenantid), nameof(SqlIntegrationadminsimulations.Shopname), nameof(SqlIntegrationadminsimulations.Createdatutc), nameof(SqlIntegrationadminsimulations.Expiresatutc), nameof(SqlIntegrationadminsimulations.Endedatutc));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlIntegrationadminsimulations.Id), nameof(SqlIntegrationadminsimulations.Adminuserid), nameof(SqlIntegrationadminsimulations.Shopid), nameof(SqlIntegrationadminsimulations.Merchantidentifier), nameof(SqlIntegrationadminsimulations.Tenantid), nameof(SqlIntegrationadminsimulations.Shopname), nameof(SqlIntegrationadminsimulations.Createdatutc), nameof(SqlIntegrationadminsimulations.Expiresatutc), nameof(SqlIntegrationadminsimulations.Endedatutc));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد یکسان‌سازی ادمین شبیه‌سازی‌ها";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "یکسان‌سازی ادمین شبیه‌سازی‌ها به تفکیک مغازه شناسه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlIntegrationadminsimulations.Shopid), "مغازه شناسه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlIntegrationeventauditsUiDefinitions : CRUDDefinition<SqlIntegrationeventaudits>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlIntegrationeventaudits.Id), nameof(SqlIntegrationeventaudits.Connectionid), nameof(SqlIntegrationeventaudits.Direction), nameof(SqlIntegrationeventaudits.Eventtype), nameof(SqlIntegrationeventaudits.Externaleventid), nameof(SqlIntegrationeventaudits.Payloadhash), nameof(SqlIntegrationeventaudits.Signaturevalid), nameof(SqlIntegrationeventaudits.Correlationid), nameof(SqlIntegrationeventaudits.Receivedatutc));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlIntegrationeventaudits.Connectionid), nameof(SqlIntegrationeventaudits.Direction), nameof(SqlIntegrationeventaudits.Eventtype), nameof(SqlIntegrationeventaudits.Externaleventid), nameof(SqlIntegrationeventaudits.Payloadhash), nameof(SqlIntegrationeventaudits.Signaturevalid), nameof(SqlIntegrationeventaudits.Correlationid), nameof(SqlIntegrationeventaudits.Receivedatutc));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد یکسان‌سازی رویداد ممیزی‌ها";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlIntegrationmerchantaccessUiDefinitions : CRUDDefinition<SqlIntegrationmerchantaccess>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlIntegrationmerchantaccess.Id), nameof(SqlIntegrationmerchantaccess.Issuer), nameof(SqlIntegrationmerchantaccess.Subjectid), nameof(SqlIntegrationmerchantaccess.Shopid), nameof(SqlIntegrationmerchantaccess.Isenabled), nameof(SqlIntegrationmerchantaccess.Createdatutc), nameof(SqlIntegrationmerchantaccess.Createdby), nameof(SqlIntegrationmerchantaccess.Expiresatutc), nameof(SqlIntegrationmerchantaccess.Revokedatutc));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlIntegrationmerchantaccess.Issuer), nameof(SqlIntegrationmerchantaccess.Subjectid), nameof(SqlIntegrationmerchantaccess.Shopid), nameof(SqlIntegrationmerchantaccess.Isenabled), nameof(SqlIntegrationmerchantaccess.Createdatutc), nameof(SqlIntegrationmerchantaccess.Createdby), nameof(SqlIntegrationmerchantaccess.Expiresatutc), nameof(SqlIntegrationmerchantaccess.Revokedatutc));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد یکسان‌سازی مغازه‌دار دسترسی";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "یکسان‌سازی مغازه‌دار دسترسی به تفکیک مغازه شناسه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlIntegrationmerchantaccess.Shopid), "مغازه شناسه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlIntegrationoutboxUiDefinitions : CRUDDefinition<SqlIntegrationoutbox>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlIntegrationoutbox.Id), nameof(SqlIntegrationoutbox.Connectionid), nameof(SqlIntegrationoutbox.Mappingid), nameof(SqlIntegrationoutbox.Sourceversion), nameof(SqlIntegrationoutbox.Operation), nameof(SqlIntegrationoutbox.Status), nameof(SqlIntegrationoutbox.Attempts), nameof(SqlIntegrationoutbox.Createdatutc), nameof(SqlIntegrationoutbox.Nextattemptatutc));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlIntegrationoutbox.Connectionid), nameof(SqlIntegrationoutbox.Mappingid), nameof(SqlIntegrationoutbox.Sourceversion), nameof(SqlIntegrationoutbox.Operation), nameof(SqlIntegrationoutbox.Status), nameof(SqlIntegrationoutbox.Attempts), nameof(SqlIntegrationoutbox.Createdatutc), nameof(SqlIntegrationoutbox.Nextattemptatutc), nameof(SqlIntegrationoutbox.Leaseid), nameof(SqlIntegrationoutbox.Leaseexpiresatutc), nameof(SqlIntegrationoutbox.Completedatutc), nameof(SqlIntegrationoutbox.Lasterror));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد یکسان‌سازی صف خروجی";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByStatusConfig : ChartConfigDefinition
        {
            public ByStatusConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "یکسان‌سازی صف خروجی به تفکیک وضعیت";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlIntegrationoutbox.Status), "وضعیت"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlIntegrationsyncrunsUiDefinitions : CRUDDefinition<SqlIntegrationsyncruns>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlIntegrationsyncruns.Id), nameof(SqlIntegrationsyncruns.Connectionid), nameof(SqlIntegrationsyncruns.Startedatutc), nameof(SqlIntegrationsyncruns.Finishedatutc), nameof(SqlIntegrationsyncruns.Status), nameof(SqlIntegrationsyncruns.Itemsread), nameof(SqlIntegrationsyncruns.Itemswritten), nameof(SqlIntegrationsyncruns.Itemsfailed), nameof(SqlIntegrationsyncruns.Error));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlIntegrationsyncruns.Connectionid), nameof(SqlIntegrationsyncruns.Startedatutc), nameof(SqlIntegrationsyncruns.Finishedatutc), nameof(SqlIntegrationsyncruns.Status), nameof(SqlIntegrationsyncruns.Itemsread), nameof(SqlIntegrationsyncruns.Itemswritten), nameof(SqlIntegrationsyncruns.Itemsfailed), nameof(SqlIntegrationsyncruns.Error));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد یکسان‌سازی یکسان‌سازی اجراها";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByStatusConfig : ChartConfigDefinition
        {
            public ByStatusConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "یکسان‌سازی یکسان‌سازی اجراها به تفکیک وضعیت";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlIntegrationsyncruns.Status), "وضعیت"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlIntegrationtokenrequestsUiDefinitions : CRUDDefinition<SqlIntegrationtokenrequests>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlIntegrationtokenrequests.Id), nameof(SqlIntegrationtokenrequests.Simulationid), nameof(SqlIntegrationtokenrequests.Provider), nameof(SqlIntegrationtokenrequests.Credentialtype), nameof(SqlIntegrationtokenrequests.Status), nameof(SqlIntegrationtokenrequests.Requestedatutc));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlIntegrationtokenrequests.Id), nameof(SqlIntegrationtokenrequests.Simulationid), nameof(SqlIntegrationtokenrequests.Provider), nameof(SqlIntegrationtokenrequests.Credentialtype), nameof(SqlIntegrationtokenrequests.Status), nameof(SqlIntegrationtokenrequests.Requestedatutc));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد یکسان‌سازی توکن درخواست‌ها";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByStatusConfig : ChartConfigDefinition
        {
            public ByStatusConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "یکسان‌سازی توکن درخواست‌ها به تفکیک وضعیت";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlIntegrationtokenrequests.Status), "وضعیت"); Count(null, "تعداد"); }
        }
        public class ByProviderConfig : ChartConfigDefinition
        {
            public ByProviderConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "یکسان‌سازی توکن درخواست‌ها به تفکیک پلتفرم";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlIntegrationtokenrequests.Provider), "پلتفرم"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlIntegrationwebhookinboxUiDefinitions : CRUDDefinition<SqlIntegrationwebhookinbox>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlIntegrationwebhookinbox.Id), nameof(SqlIntegrationwebhookinbox.Connectionid), nameof(SqlIntegrationwebhookinbox.Externaleventid), nameof(SqlIntegrationwebhookinbox.Eventtype), nameof(SqlIntegrationwebhookinbox.Receivedatutc), nameof(SqlIntegrationwebhookinbox.Processedatutc), nameof(SqlIntegrationwebhookinbox.Status), nameof(SqlIntegrationwebhookinbox.Error));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlIntegrationwebhookinbox.Connectionid), nameof(SqlIntegrationwebhookinbox.Externaleventid), nameof(SqlIntegrationwebhookinbox.Eventtype), nameof(SqlIntegrationwebhookinbox.Receivedatutc), nameof(SqlIntegrationwebhookinbox.Processedatutc), nameof(SqlIntegrationwebhookinbox.Status), nameof(SqlIntegrationwebhookinbox.Error));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد یکسان‌سازی وب‌هوک صندوق ورودی";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByStatusConfig : ChartConfigDefinition
        {
            public ByStatusConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "یکسان‌سازی وب‌هوک صندوق ورودی به تفکیک وضعیت";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlIntegrationwebhookinbox.Status), "وضعیت"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlInventoryreservationlogsUiDefinitions : CRUDDefinition<SqlInventoryreservationlogs>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlInventoryreservationlogs.Id), nameof(SqlInventoryreservationlogs.Shopid), nameof(SqlInventoryreservationlogs.Hyperproductid), nameof(SqlInventoryreservationlogs.Reservationkey), nameof(SqlInventoryreservationlogs.Quantity), nameof(SqlInventoryreservationlogs.Status), nameof(SqlInventoryreservationlogs.Source), nameof(SqlInventoryreservationlogs.Createdatutc), nameof(SqlInventoryreservationlogs.Releasedatutc));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlInventoryreservationlogs.Shopid), nameof(SqlInventoryreservationlogs.Hyperproductid), nameof(SqlInventoryreservationlogs.Reservationkey), nameof(SqlInventoryreservationlogs.Quantity), nameof(SqlInventoryreservationlogs.Status), nameof(SqlInventoryreservationlogs.Source), nameof(SqlInventoryreservationlogs.Createdatutc), nameof(SqlInventoryreservationlogs.Releasedatutc));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد موجودی رزرو سوابق";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByStatusConfig : ChartConfigDefinition
        {
            public ByStatusConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "موجودی رزرو سوابق به تفکیک وضعیت";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlInventoryreservationlogs.Status), "وضعیت"); Count(null, "تعداد"); }
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "موجودی رزرو سوابق به تفکیک مغازه شناسه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlInventoryreservationlogs.Shopid), "مغازه شناسه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblAccountUiDefinitions : CRUDDefinition<SqlTblAccount>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblAccount.Accountid), nameof(SqlTblAccount.Shopid), nameof(SqlTblAccount.Type), nameof(SqlTblAccount.Detailtype), nameof(SqlTblAccount.Nature), nameof(SqlTblAccount.Parentid), nameof(SqlTblAccount.Code), nameof(SqlTblAccount.Name), nameof(SqlTblAccount.Level));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblAccount.Shopid), nameof(SqlTblAccount.Type), nameof(SqlTblAccount.Detailtype), nameof(SqlTblAccount.Nature), nameof(SqlTblAccount.Parentid), nameof(SqlTblAccount.Code), nameof(SqlTblAccount.Name), nameof(SqlTblAccount.Level), nameof(SqlTblAccount.Referenceid), nameof(SqlTblAccount.Ispostable), nameof(SqlTblAccount.Globalaccountkey), nameof(SqlTblAccount.Description), nameof(SqlTblAccount.Fiscalperiodid), nameof(SqlTblAccount.Reporttype), nameof(SqlTblAccount.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد حساب";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "حساب به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblAccount.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblAccountingarticleUiDefinitions : CRUDDefinition<SqlTblAccountingarticle>
{
    public override List<string>? Roles => [HyperRoles.Admin];
        protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblAccountingarticle.Id), nameof(SqlTblAccountingarticle.Documentid), nameof(SqlTblAccountingarticle.TenantId), nameof(SqlTblAccountingarticle.Shopid), nameof(SqlTblAccountingarticle.Detailaccountid), nameof(SqlTblAccountingarticle.Description), nameof(SqlTblAccountingarticle.Accountid), nameof(SqlTblAccountingarticle.Amount), nameof(SqlTblAccountingarticle.Currency));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblAccountingarticle.Documentid), nameof(SqlTblAccountingarticle.TenantId), nameof(SqlTblAccountingarticle.Shopid), nameof(SqlTblAccountingarticle.Detailaccountid), nameof(SqlTblAccountingarticle.Description), nameof(SqlTblAccountingarticle.Accountid), nameof(SqlTblAccountingarticle.Amount), nameof(SqlTblAccountingarticle.Currency), nameof(SqlTblAccountingarticle.Checkid), nameof(SqlTblAccountingarticle.Exchangerate), nameof(SqlTblAccountingarticle.Type), nameof(SqlTblAccountingarticle.Baseamount));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد آرتیکل حسابداری";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "آرتیکل حسابداری به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblAccountingarticle.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblAccountingdocumentUiDefinitions : CRUDDefinition<SqlTblAccountingdocument>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblAccountingdocument.Id), nameof(SqlTblAccountingdocument.Description), nameof(SqlTblAccountingdocument.TenantId), nameof(SqlTblAccountingdocument.Shopid), nameof(SqlTblAccountingdocument.Fiscalperiodid), nameof(SqlTblAccountingdocument.Totalbaseamount), nameof(SqlTblAccountingdocument.Referencenumber), nameof(SqlTblAccountingdocument.Status), nameof(SqlTblAccountingdocument.Number));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblAccountingdocument.Description), nameof(SqlTblAccountingdocument.TenantId), nameof(SqlTblAccountingdocument.Shopid), nameof(SqlTblAccountingdocument.Fiscalperiodid), nameof(SqlTblAccountingdocument.Totalbaseamount), nameof(SqlTblAccountingdocument.Referencenumber), nameof(SqlTblAccountingdocument.Status), nameof(SqlTblAccountingdocument.Number), nameof(SqlTblAccountingdocument.Date), nameof(SqlTblAccountingdocument.Primarytype), nameof(SqlTblAccountingdocument.Referencetype), nameof(SqlTblAccountingdocument.Referenceid), nameof(SqlTblAccountingdocument.Datetime), nameof(SqlTblAccountingdocument.Month), nameof(SqlTblAccountingdocument.Projectid), nameof(SqlTblAccountingdocument.Secondarytype));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد سند حسابداری";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByStatusConfig : ChartConfigDefinition
        {
            public ByStatusConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "سند حسابداری به تفکیک وضعیت سند";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblAccountingdocument.Status), "وضعیت سند"); Count(null, "تعداد"); }
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "سند حسابداری به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblAccountingdocument.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblAutoprocesslogUiDefinitions : CRUDDefinition<SqlTblAutoprocesslog>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblAutoprocesslog.Logid), nameof(SqlTblAutoprocesslog.Processname), nameof(SqlTblAutoprocesslog.Processkey), nameof(SqlTblAutoprocesslog.Processversion), nameof(SqlTblAutoprocesslog.Triggerconfig), nameof(SqlTblAutoprocesslog.Endtime), nameof(SqlTblAutoprocesslog.Starttime), nameof(SqlTblAutoprocesslog.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblAutoprocesslog.Processname), nameof(SqlTblAutoprocesslog.Processkey), nameof(SqlTblAutoprocesslog.Processversion), nameof(SqlTblAutoprocesslog.Triggerconfig), nameof(SqlTblAutoprocesslog.Endtime), nameof(SqlTblAutoprocesslog.Starttime), nameof(SqlTblAutoprocesslog.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد لاگ فرایند خودکار";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlTblBankaccountUiDefinitions : CRUDDefinition<SqlTblBankaccount>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblBankaccount.Id), nameof(SqlTblBankaccount.Name), nameof(SqlTblBankaccount.Number), nameof(SqlTblBankaccount.Iban), nameof(SqlTblBankaccount.Bankbranchname), nameof(SqlTblBankaccount.Ownername), nameof(SqlTblBankaccount.Shopid), nameof(SqlTblBankaccount.Cardnumber), nameof(SqlTblBankaccount.Isenabled));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblBankaccount.Name), nameof(SqlTblBankaccount.Number), nameof(SqlTblBankaccount.Iban), nameof(SqlTblBankaccount.Bankbranchname), nameof(SqlTblBankaccount.Ownername), nameof(SqlTblBankaccount.Shopid), nameof(SqlTblBankaccount.Cardnumber), nameof(SqlTblBankaccount.Isenabled), nameof(SqlTblBankaccount.Isdefault), nameof(SqlTblBankaccount.Description), nameof(SqlTblBankaccount.Currency), nameof(SqlTblBankaccount.Bank), nameof(SqlTblBankaccount.Detailaccountid), nameof(SqlTblBankaccount.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد حساب بانکی";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "حساب بانکی به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblBankaccount.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblCashfundUiDefinitions : CRUDDefinition<SqlTblCashfund>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblCashfund.Id), nameof(SqlTblCashfund.Name), nameof(SqlTblCashfund.Description), nameof(SqlTblCashfund.Isenabled), nameof(SqlTblCashfund.Shopid), nameof(SqlTblCashfund.Isdefault), nameof(SqlTblCashfund.Currency), nameof(SqlTblCashfund.Detailaccountid), nameof(SqlTblCashfund.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblCashfund.Name), nameof(SqlTblCashfund.Description), nameof(SqlTblCashfund.Isenabled), nameof(SqlTblCashfund.Shopid), nameof(SqlTblCashfund.Isdefault), nameof(SqlTblCashfund.Currency), nameof(SqlTblCashfund.Detailaccountid), nameof(SqlTblCashfund.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد صندوق نقدی";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "صندوق نقدی به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblCashfund.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblCheckUiDefinitions : CRUDDefinition<SqlTblCheck>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblCheck.Checkid), nameof(SqlTblCheck.Serialnumber), nameof(SqlTblCheck.Duedate), nameof(SqlTblCheck.Amount), nameof(SqlTblCheck.Status), nameof(SqlTblCheck.Statusdatetime), nameof(SqlTblCheck.Sayadnumber), nameof(SqlTblCheck.Description), nameof(SqlTblCheck.Payee));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblCheck.Serialnumber), nameof(SqlTblCheck.Duedate), nameof(SqlTblCheck.Amount), nameof(SqlTblCheck.Status), nameof(SqlTblCheck.Statusdatetime), nameof(SqlTblCheck.Sayadnumber), nameof(SqlTblCheck.Description), nameof(SqlTblCheck.Payee), nameof(SqlTblCheck.Payeenationalid), nameof(SqlTblCheck.Shopid), nameof(SqlTblCheck.Bankaccountid), nameof(SqlTblCheck.Issuedate), nameof(SqlTblCheck.Bankbranch), nameof(SqlTblCheck.Bank), nameof(SqlTblCheck.Personid), nameof(SqlTblCheck.Isreceipt), nameof(SqlTblCheck.Currency), nameof(SqlTblCheck.Exchangerate), nameof(SqlTblCheck.Projectid), nameof(SqlTblCheck.Fiscalperiodid), nameof(SqlTblCheck.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد چک";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByStatusConfig : ChartConfigDefinition
        {
            public ByStatusConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "چک به تفکیک وضعیت چک";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblCheck.Status), "وضعیت چک"); Count(null, "تعداد"); }
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "چک به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblCheck.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblCredittransactionUiDefinitions : CRUDDefinition<SqlTblCredittransaction>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblCredittransaction.Id), nameof(SqlTblCredittransaction.Userid), nameof(SqlTblCredittransaction.Amount), nameof(SqlTblCredittransaction.Type), nameof(SqlTblCredittransaction.Referenceid), nameof(SqlTblCredittransaction.Description), nameof(SqlTblCredittransaction.Datetime), nameof(SqlTblCredittransaction.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblCredittransaction.Userid), nameof(SqlTblCredittransaction.Amount), nameof(SqlTblCredittransaction.Type), nameof(SqlTblCredittransaction.Referenceid), nameof(SqlTblCredittransaction.Description), nameof(SqlTblCredittransaction.Datetime), nameof(SqlTblCredittransaction.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد تراکنش‌های کیف پول";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlTblDetailaccountUiDefinitions : CRUDDefinition<SqlTblDetailaccount>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblDetailaccount.Detailaccountid), nameof(SqlTblDetailaccount.Shopid), nameof(SqlTblDetailaccount.Entitytype), nameof(SqlTblDetailaccount.Name), nameof(SqlTblDetailaccount.Referenceid), nameof(SqlTblDetailaccount.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblDetailaccount.Shopid), nameof(SqlTblDetailaccount.Entitytype), nameof(SqlTblDetailaccount.Name), nameof(SqlTblDetailaccount.Referenceid), nameof(SqlTblDetailaccount.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد حساب تفصیلی";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "حساب تفصیلی به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblDetailaccount.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblEntityfileUiDefinitions : CRUDDefinition<SqlTblEntityfile>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblEntityfile.Id), nameof(SqlTblEntityfile.Shopid), nameof(SqlTblEntityfile.Entitytype), nameof(SqlTblEntityfile.Entityid), nameof(SqlTblEntityfile.Originalname), nameof(SqlTblEntityfile.Uniquename), nameof(SqlTblEntityfile.Size), nameof(SqlTblEntityfile.Relativeurl), nameof(SqlTblEntityfile.Type));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblEntityfile.Shopid), nameof(SqlTblEntityfile.Entitytype), nameof(SqlTblEntityfile.Entityid), nameof(SqlTblEntityfile.Originalname), nameof(SqlTblEntityfile.Uniquename), nameof(SqlTblEntityfile.Size), nameof(SqlTblEntityfile.Relativeurl), nameof(SqlTblEntityfile.Type), nameof(SqlTblEntityfile.Createdat), nameof(SqlTblEntityfile.Description), nameof(SqlTblEntityfile.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد فایل‌ها";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "فایل‌ها به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblEntityfile.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblEntitynoteUiDefinitions : CRUDDefinition<SqlTblEntitynote>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblEntitynote.Id), nameof(SqlTblEntitynote.Shopid), nameof(SqlTblEntitynote.Entityid), nameof(SqlTblEntitynote.Entitytype), nameof(SqlTblEntitynote.Note), nameof(SqlTblEntitynote.Ispublic), nameof(SqlTblEntitynote.Createdby), nameof(SqlTblEntitynote.Createdtime), nameof(SqlTblEntitynote.Updatedtime));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblEntitynote.Shopid), nameof(SqlTblEntitynote.Entityid), nameof(SqlTblEntitynote.Entitytype), nameof(SqlTblEntitynote.Note), nameof(SqlTblEntitynote.Ispublic), nameof(SqlTblEntitynote.Createdby), nameof(SqlTblEntitynote.Createdtime), nameof(SqlTblEntitynote.Updatedtime), nameof(SqlTblEntitynote.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد یادداشت موجودیت";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "یادداشت موجودیت به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblEntitynote.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblGeneralconfigUiDefinitions : CRUDDefinition<SqlTblGeneralconfig>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblGeneralconfig.Configid), nameof(SqlTblGeneralconfig.Shopid), nameof(SqlTblGeneralconfig.Basecurrency), nameof(SqlTblGeneralconfig.Timezone), nameof(SqlTblGeneralconfig.Calendartype), nameof(SqlTblGeneralconfig.Defaultvatrate), nameof(SqlTblGeneralconfig.Ismulticurrencyenabled), nameof(SqlTblGeneralconfig.Othercurrencies), nameof(SqlTblGeneralconfig.Alloweditopeningbalance));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblGeneralconfig.Shopid), nameof(SqlTblGeneralconfig.Basecurrency), nameof(SqlTblGeneralconfig.Timezone), nameof(SqlTblGeneralconfig.Calendartype), nameof(SqlTblGeneralconfig.Defaultvatrate), nameof(SqlTblGeneralconfig.Ismulticurrencyenabled), nameof(SqlTblGeneralconfig.Othercurrencies), nameof(SqlTblGeneralconfig.Alloweditopeningbalance), nameof(SqlTblGeneralconfig.Autoapprovesystemgenerateddocuments), nameof(SqlTblGeneralconfig.Iswarehouseenabled), nameof(SqlTblGeneralconfig.TenantId), nameof(SqlTblGeneralconfig.Allownegativephysicalstock), nameof(SqlTblGeneralconfig.Trackingmethod), nameof(SqlTblGeneralconfig.Valuationmethod), nameof(SqlTblGeneralconfig.Storageexpirationdate), nameof(SqlTblGeneralconfig.Storagesize));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد تنظیمات کلی فروشگاه";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تنظیمات کلی فروشگاه به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblGeneralconfig.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblGlobalconfigUiDefinitions : CRUDDefinition<SqlTblGlobalconfig>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblGlobalconfig.Key), nameof(SqlTblGlobalconfig.Value), nameof(SqlTblGlobalconfig.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblGlobalconfig.Key), nameof(SqlTblGlobalconfig.Value), nameof(SqlTblGlobalconfig.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد عمومی تنظیمات";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlTblGlobalproductUiDefinitions : CRUDDefinition<SqlTblGlobalproduct>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblGlobalproduct.Globalproductid), nameof(SqlTblGlobalproduct.Name), nameof(SqlTblGlobalproduct.Barcode), nameof(SqlTblGlobalproduct.Description), nameof(SqlTblGlobalproduct.Ispublished), nameof(SqlTblGlobalproduct.Brandid), nameof(SqlTblGlobalproduct.Unitcode), nameof(SqlTblGlobalproduct.Isenabled), nameof(SqlTblGlobalproduct.Isapproved));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblGlobalproduct.Name), nameof(SqlTblGlobalproduct.Barcode), nameof(SqlTblGlobalproduct.Description), nameof(SqlTblGlobalproduct.Ispublished), nameof(SqlTblGlobalproduct.Brandid), nameof(SqlTblGlobalproduct.Unitcode), nameof(SqlTblGlobalproduct.Isenabled), nameof(SqlTblGlobalproduct.Isapproved), nameof(SqlTblGlobalproduct.Reviewstatusid), nameof(SqlTblGlobalproduct.Createtime), nameof(SqlTblGlobalproduct.Creatoruser), nameof(SqlTblGlobalproduct.Updatetime), nameof(SqlTblGlobalproduct.Updateruser), nameof(SqlTblGlobalproduct.Saletaxrate), nameof(SqlTblGlobalproduct.Purchasetaxrate), nameof(SqlTblGlobalproduct.Isservice), nameof(SqlTblGlobalproduct.Groupid), nameof(SqlTblGlobalproduct.Imagerelativeurl), nameof(SqlTblGlobalproduct.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد محصول هایپریک";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlTblGlobalproductgroupUiDefinitions : CRUDDefinition<SqlTblGlobalproductgroup>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblGlobalproductgroup.Groupid), nameof(SqlTblGlobalproductgroup.Parentid), nameof(SqlTblGlobalproductgroup.Name), nameof(SqlTblGlobalproductgroup.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblGlobalproductgroup.Parentid), nameof(SqlTblGlobalproductgroup.Name), nameof(SqlTblGlobalproductgroup.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد گروه محصول هایپریک";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlTblGlobalproductmediaUiDefinitions : CRUDDefinition<SqlTblGlobalproductmedia>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblGlobalproductmedia.Mediaid), nameof(SqlTblGlobalproductmedia.Productid), nameof(SqlTblGlobalproductmedia.Filerelativeurl), nameof(SqlTblGlobalproductmedia.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblGlobalproductmedia.Productid), nameof(SqlTblGlobalproductmedia.Filerelativeurl), nameof(SqlTblGlobalproductmedia.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد رسانه محصول";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlTblGroupsUiDefinitions : CRUDDefinition<SqlTblGroups>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblGroups.Shopid), nameof(SqlTblGroups.Groupid), nameof(SqlTblGroups.Entitytype), nameof(SqlTblGroups.Parentid), nameof(SqlTblGroups.Code), nameof(SqlTblGroups.Name), nameof(SqlTblGroups.Description), nameof(SqlTblGroups.Fullpath), nameof(SqlTblGroups.Namefullpath));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblGroups.Shopid), nameof(SqlTblGroups.Entitytype), nameof(SqlTblGroups.Parentid), nameof(SqlTblGroups.Code), nameof(SqlTblGroups.Name), nameof(SqlTblGroups.Description), nameof(SqlTblGroups.Fullpath), nameof(SqlTblGroups.Namefullpath), nameof(SqlTblGroups.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد گروه‌ها";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "گروه‌ها به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblGroups.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblInventoryfifoconsumptionUiDefinitions : CRUDDefinition<SqlTblInventoryfifoconsumption>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblInventoryfifoconsumption.Id), nameof(SqlTblInventoryfifoconsumption.Stockcarditemid), nameof(SqlTblInventoryfifoconsumption.Shopid), nameof(SqlTblInventoryfifoconsumption.Consumedquantity), nameof(SqlTblInventoryfifoconsumption.Unitcost), nameof(SqlTblInventoryfifoconsumption.Consumptiondatetime), nameof(SqlTblInventoryfifoconsumption.Amount), nameof(SqlTblInventoryfifoconsumption.Productid), nameof(SqlTblInventoryfifoconsumption.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblInventoryfifoconsumption.Stockcarditemid), nameof(SqlTblInventoryfifoconsumption.Shopid), nameof(SqlTblInventoryfifoconsumption.Consumedquantity), nameof(SqlTblInventoryfifoconsumption.Unitcost), nameof(SqlTblInventoryfifoconsumption.Consumptiondatetime), nameof(SqlTblInventoryfifoconsumption.Amount), nameof(SqlTblInventoryfifoconsumption.Productid), nameof(SqlTblInventoryfifoconsumption.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد مصرف لایه‌های FIFO";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "مصرف لایه‌های FIFO به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblInventoryfifoconsumption.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblInventoryfifolayerUiDefinitions : CRUDDefinition<SqlTblInventoryfifolayer>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblInventoryfifolayer.Id), nameof(SqlTblInventoryfifolayer.Stockcarditemid), nameof(SqlTblInventoryfifolayer.Productid), nameof(SqlTblInventoryfifolayer.Warehouseid), nameof(SqlTblInventoryfifolayer.Shopid), nameof(SqlTblInventoryfifolayer.Initialquantity), nameof(SqlTblInventoryfifolayer.Remainingquantity), nameof(SqlTblInventoryfifolayer.Unitcost), nameof(SqlTblInventoryfifolayer.Receiptdatetime));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblInventoryfifolayer.Stockcarditemid), nameof(SqlTblInventoryfifolayer.Productid), nameof(SqlTblInventoryfifolayer.Warehouseid), nameof(SqlTblInventoryfifolayer.Shopid), nameof(SqlTblInventoryfifolayer.Initialquantity), nameof(SqlTblInventoryfifolayer.Remainingquantity), nameof(SqlTblInventoryfifolayer.Unitcost), nameof(SqlTblInventoryfifolayer.Receiptdatetime), nameof(SqlTblInventoryfifolayer.Amount), nameof(SqlTblInventoryfifolayer.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد لایه موجودی FIFO";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "لایه موجودی FIFO به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblInventoryfifolayer.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblIrancitiesUiDefinitions : CRUDDefinition<SqlTblIrancities>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblIrancities.Cityid), nameof(SqlTblIrancities.Stateid), nameof(SqlTblIrancities.Cityname), nameof(SqlTblIrancities.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblIrancities.Cityid), nameof(SqlTblIrancities.Stateid), nameof(SqlTblIrancities.Cityname), nameof(SqlTblIrancities.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد شهرهای ایران";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlTblIranstatesUiDefinitions : CRUDDefinition<SqlTblIranstates>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblIranstates.Stateid), nameof(SqlTblIranstates.Statename), nameof(SqlTblIranstates.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblIranstates.Stateid), nameof(SqlTblIranstates.Statename), nameof(SqlTblIranstates.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد استان‌های ایران";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlTblMessagehistoryUiDefinitions : CRUDDefinition<SqlTblMessagehistory>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblMessagehistory.Messageid), nameof(SqlTblMessagehistory.Mobilenumber), nameof(SqlTblMessagehistory.Sendtime), nameof(SqlTblMessagehistory.Messagetext), nameof(SqlTblMessagehistory.Errorcode), nameof(SqlTblMessagehistory.Subject), nameof(SqlTblMessagehistory.Errormessage), nameof(SqlTblMessagehistory.TenantId), nameof(SqlTblMessagehistory.Deliverystatus));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblMessagehistory.Mobilenumber), nameof(SqlTblMessagehistory.Sendtime), nameof(SqlTblMessagehistory.Messagetext), nameof(SqlTblMessagehistory.Errorcode), nameof(SqlTblMessagehistory.Subject), nameof(SqlTblMessagehistory.Errormessage), nameof(SqlTblMessagehistory.TenantId), nameof(SqlTblMessagehistory.Deliverystatus));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد تاریخچه پیامک‌ها";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlTblNotificationconfigUiDefinitions : CRUDDefinition<SqlTblNotificationconfig>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblNotificationconfig.Configid), nameof(SqlTblNotificationconfig.Shopid), nameof(SqlTblNotificationconfig.Notificationmethod), nameof(SqlTblNotificationconfig.Notifysaleinvoiceduedate), nameof(SqlTblNotificationconfig.Saleinvoiceduenotificationthreshold), nameof(SqlTblNotificationconfig.Notifypurchaseinvoiceduedate), nameof(SqlTblNotificationconfig.Purchaseinvoiceduenotificationthreshold), nameof(SqlTblNotificationconfig.Notifyreceivedchequeduedate), nameof(SqlTblNotificationconfig.Receivedchequeduenotificationthreshold));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblNotificationconfig.Shopid), nameof(SqlTblNotificationconfig.Notificationmethod), nameof(SqlTblNotificationconfig.Notifysaleinvoiceduedate), nameof(SqlTblNotificationconfig.Saleinvoiceduenotificationthreshold), nameof(SqlTblNotificationconfig.Notifypurchaseinvoiceduedate), nameof(SqlTblNotificationconfig.Purchaseinvoiceduenotificationthreshold), nameof(SqlTblNotificationconfig.Notifyreceivedchequeduedate), nameof(SqlTblNotificationconfig.Receivedchequeduenotificationthreshold), nameof(SqlTblNotificationconfig.Notifypaidchequeduedate), nameof(SqlTblNotificationconfig.Paidchequeduenotificationthreshold), nameof(SqlTblNotificationconfig.Notifyinstallmentduedate), nameof(SqlTblNotificationconfig.Installmentduenotificationthreshold), nameof(SqlTblNotificationconfig.Notifyreorderpoint), nameof(SqlTblNotificationconfig.Notifytaxinvoicesubmissionduedate), nameof(SqlTblNotificationconfig.Taxinvoicesubmissionduenotificationthreshold), nameof(SqlTblNotificationconfig.Notifydocumentlimitreached), nameof(SqlTblNotificationconfig.Documentlimitnotificationthreshold), nameof(SqlTblNotificationconfig.Notifyonlineinvoicelimitreached), nameof(SqlTblNotificationconfig.Onlineinvoicenotificationthreshold), nameof(SqlTblNotificationconfig.Notifytaxsubmissionlimitreached), nameof(SqlTblNotificationconfig.Taxsubmissionnotificationthreshold), nameof(SqlTblNotificationconfig.Notifystoragelimitreached), nameof(SqlTblNotificationconfig.Storagenotificationthreshold), nameof(SqlTblNotificationconfig.Notifywalletlowbalance), nameof(SqlTblNotificationconfig.Walletlowbalancethreshold), nameof(SqlTblNotificationconfig.Senddailysalesreport), nameof(SqlTblNotificationconfig.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد تنظیمات اعلانات";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تنظیمات اعلانات به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblNotificationconfig.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblPersonUiDefinitions : CRUDDefinition<SqlTblPerson>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblPerson.Id), nameof(SqlTblPerson.Shopid), nameof(SqlTblPerson.Detailaccountid), nameof(SqlTblPerson.Name), nameof(SqlTblPerson.Lastname), nameof(SqlTblPerson.Companyname), nameof(SqlTblPerson.Nickname), nameof(SqlTblPerson.Type), nameof(SqlTblPerson.Isenabled));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblPerson.Shopid), nameof(SqlTblPerson.Detailaccountid), nameof(SqlTblPerson.Name), nameof(SqlTblPerson.Lastname), nameof(SqlTblPerson.Companyname), nameof(SqlTblPerson.Nickname), nameof(SqlTblPerson.Type), nameof(SqlTblPerson.Isenabled), nameof(SqlTblPerson.Identifiernumber), nameof(SqlTblPerson.Economiccode), nameof(SqlTblPerson.Branchcode), nameof(SqlTblPerson.Creditlimit), nameof(SqlTblPerson.Profileimageurl), nameof(SqlTblPerson.Contactinfo), nameof(SqlTblPerson.Specialdates), nameof(SqlTblPerson.Bankaccountsinfo), nameof(SqlTblPerson.Addressesinfo), nameof(SqlTblPerson.Roles), nameof(SqlTblPerson.Mobilenumber), nameof(SqlTblPerson.Passportnumber), nameof(SqlTblPerson.Contractnumber), nameof(SqlTblPerson.Subscriptionnumber), nameof(SqlTblPerson.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد شخص";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "شخص به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblPerson.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblPettycashUiDefinitions : CRUDDefinition<SqlTblPettycash>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblPettycash.Pettycashid), nameof(SqlTblPettycash.Shopid), nameof(SqlTblPettycash.Detailaccountid), nameof(SqlTblPettycash.Personid), nameof(SqlTblPettycash.Isdefault), nameof(SqlTblPettycash.Isenabled), nameof(SqlTblPettycash.Description), nameof(SqlTblPettycash.Code), nameof(SqlTblPettycash.Name));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblPettycash.Shopid), nameof(SqlTblPettycash.Detailaccountid), nameof(SqlTblPettycash.Personid), nameof(SqlTblPettycash.Isdefault), nameof(SqlTblPettycash.Isenabled), nameof(SqlTblPettycash.Description), nameof(SqlTblPettycash.Code), nameof(SqlTblPettycash.Name), nameof(SqlTblPettycash.TenantId), nameof(SqlTblPettycash.Currency));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد تنخواه";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تنخواه به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblPettycash.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblPosdeviceUiDefinitions : CRUDDefinition<SqlTblPosdevice>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblPosdevice.Id), nameof(SqlTblPosdevice.Shopid), nameof(SqlTblPosdevice.Isenabled), nameof(SqlTblPosdevice.Bankaccountid), nameof(SqlTblPosdevice.Description), nameof(SqlTblPosdevice.Name), nameof(SqlTblPosdevice.Ipaddress), nameof(SqlTblPosdevice.Portnumber), nameof(SqlTblPosdevice.Terminalnumber));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblPosdevice.Shopid), nameof(SqlTblPosdevice.Isenabled), nameof(SqlTblPosdevice.Bankaccountid), nameof(SqlTblPosdevice.Description), nameof(SqlTblPosdevice.Name), nameof(SqlTblPosdevice.Ipaddress), nameof(SqlTblPosdevice.Portnumber), nameof(SqlTblPosdevice.Terminalnumber), nameof(SqlTblPosdevice.Merchantnumber), nameof(SqlTblPosdevice.Serialnumber), nameof(SqlTblPosdevice.Isdefault), nameof(SqlTblPosdevice.Psp), nameof(SqlTblPosdevice.Detailaccountid), nameof(SqlTblPosdevice.Currency), nameof(SqlTblPosdevice.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد دستگاه کارتخوان";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "دستگاه کارتخوان به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblPosdevice.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblPrintconfigUiDefinitions : CRUDDefinition<SqlTblPrintconfig>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblPrintconfig.Configid), nameof(SqlTblPrintconfig.Shopid), nameof(SqlTblPrintconfig.Printlogo), nameof(SqlTblPrintconfig.Invoicetitle), nameof(SqlTblPrintconfig.Printsellerinfo), nameof(SqlTblPrintconfig.Printcustomerinfo), nameof(SqlTblPrintconfig.Printsignaturearea), nameof(SqlTblPrintconfig.Signaturetitle1), nameof(SqlTblPrintconfig.Signaturetitle2));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblPrintconfig.Shopid), nameof(SqlTblPrintconfig.Printlogo), nameof(SqlTblPrintconfig.Invoicetitle), nameof(SqlTblPrintconfig.Printsellerinfo), nameof(SqlTblPrintconfig.Printcustomerinfo), nameof(SqlTblPrintconfig.Printsignaturearea), nameof(SqlTblPrintconfig.Signaturetitle1), nameof(SqlTblPrintconfig.Signaturetitle2), nameof(SqlTblPrintconfig.Signaturetitle3), nameof(SqlTblPrintconfig.Signaturetitle4), nameof(SqlTblPrintconfig.Signaturetitle5), nameof(SqlTblPrintconfig.Printduedate), nameof(SqlTblPrintconfig.Taxprintmethod), nameof(SqlTblPrintconfig.Discountprintmethod), nameof(SqlTblPrintconfig.Printpaymentinfo), nameof(SqlTblPrintconfig.Printaccountbalance), nameof(SqlTblPrintconfig.Printcurrentdatetime), nameof(SqlTblPrintconfig.Invoicefootertext), nameof(SqlTblPrintconfig.Printcopycount), nameof(SqlTblPrintconfig.Printinvoiceitemcount), nameof(SqlTblPrintconfig.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد تنظیمات چاپ";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تنظیمات چاپ به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblPrintconfig.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblProductUiDefinitions : CRUDDefinition<SqlTblProduct>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblProduct.Id), nameof(SqlTblProduct.Shopid), nameof(SqlTblProduct.Globalid), nameof(SqlTblProduct.Taxcode), nameof(SqlTblProduct.Name), nameof(SqlTblProduct.Isenabled), nameof(SqlTblProduct.Baseunit), nameof(SqlTblProduct.Secondaryunit), nameof(SqlTblProduct.Unitconversionfactor));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblProduct.Shopid), nameof(SqlTblProduct.Globalid), nameof(SqlTblProduct.Taxcode), nameof(SqlTblProduct.Name), nameof(SqlTblProduct.Isenabled), nameof(SqlTblProduct.Baseunit), nameof(SqlTblProduct.Secondaryunit), nameof(SqlTblProduct.Unitconversionfactor), nameof(SqlTblProduct.Purchaseprice), nameof(SqlTblProduct.Purchasetaxrate), nameof(SqlTblProduct.Saleprice), nameof(SqlTblProduct.Saletaxrate), nameof(SqlTblProduct.Percentdiscount), nameof(SqlTblProduct.Fixeddiscount), nameof(SqlTblProduct.Isconsumable), nameof(SqlTblProduct.Ispurchasable), nameof(SqlTblProduct.Isstockable), nameof(SqlTblProduct.Issellable), nameof(SqlTblProduct.Isonlinesellable), nameof(SqlTblProduct.Isservice), nameof(SqlTblProduct.Isserialized), nameof(SqlTblProduct.Detailaccountid), nameof(SqlTblProduct.Imagerelativeurl), nameof(SqlTblProduct.Maxsalesquantity), nameof(SqlTblProduct.Description), nameof(SqlTblProduct.Accountingstock), nameof(SqlTblProduct.Minimumstock), nameof(SqlTblProduct.Groupid), nameof(SqlTblProduct.Reorderpoint), nameof(SqlTblProduct.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد محصول";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "محصول به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblProduct.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblProductbarcodeUiDefinitions : CRUDDefinition<SqlTblProductbarcode>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblProductbarcode.Barcodeid), nameof(SqlTblProductbarcode.Productid), nameof(SqlTblProductbarcode.Barcode), nameof(SqlTblProductbarcode.Shopid), nameof(SqlTblProductbarcode.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblProductbarcode.Productid), nameof(SqlTblProductbarcode.Barcode), nameof(SqlTblProductbarcode.Shopid), nameof(SqlTblProductbarcode.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد بارکد محصول";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "بارکد محصول به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblProductbarcode.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblProductbrandUiDefinitions : CRUDDefinition<SqlTblProductbrand>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblProductbrand.Brandid), nameof(SqlTblProductbrand.Brandname), nameof(SqlTblProductbrand.Branddescription), nameof(SqlTblProductbrand.TenantId), nameof(SqlTblProductbrand.Logorelativeurl));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblProductbrand.Brandname), nameof(SqlTblProductbrand.Branddescription), nameof(SqlTblProductbrand.TenantId), nameof(SqlTblProductbrand.Logorelativeurl));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد برند محصول";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlTblProductreviewstatusUiDefinitions : CRUDDefinition<SqlTblProductreviewstatus>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblProductreviewstatus.Productreviewstatusid), nameof(SqlTblProductreviewstatus.Productreviewstatusname), nameof(SqlTblProductreviewstatus.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblProductreviewstatus.Productreviewstatusid), nameof(SqlTblProductreviewstatus.Productreviewstatusname), nameof(SqlTblProductreviewstatus.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد وضعیت فراوری محصول";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlTblProductunitUiDefinitions : CRUDDefinition<SqlTblProductunit>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblProductunit.Name), nameof(SqlTblProductunit.Decimalprecision), nameof(SqlTblProductunit.TenantId), nameof(SqlTblProductunit.Unitcode), nameof(SqlTblProductunit.Usagetype), nameof(SqlTblProductunit.Countassingleitem), nameof(SqlTblProductunit.Conversions));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblProductunit.Name), nameof(SqlTblProductunit.Decimalprecision), nameof(SqlTblProductunit.TenantId), nameof(SqlTblProductunit.Unitcode), nameof(SqlTblProductunit.Usagetype), nameof(SqlTblProductunit.Countassingleitem), nameof(SqlTblProductunit.Conversions));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد واحد اندازه‌گیری محصول";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlTblProjectUiDefinitions : CRUDDefinition<SqlTblProject>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblProject.Projectid), nameof(SqlTblProject.Shopid), nameof(SqlTblProject.Name), nameof(SqlTblProject.Isdefault), nameof(SqlTblProject.Isenabled), nameof(SqlTblProject.Description), nameof(SqlTblProject.Code), nameof(SqlTblProject.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblProject.Shopid), nameof(SqlTblProject.Name), nameof(SqlTblProject.Isdefault), nameof(SqlTblProject.Isenabled), nameof(SqlTblProject.Description), nameof(SqlTblProject.Code), nameof(SqlTblProject.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد پروژه";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "پروژه به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblProject.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblPurchaseorderUiDefinitions : CRUDDefinition<SqlTblPurchaseorder>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblPurchaseorder.Purchaseorderid), nameof(SqlTblPurchaseorder.Totalamountbeforediscount), nameof(SqlTblPurchaseorder.Totalvatamount), nameof(SqlTblPurchaseorder.Totalinvoiceamount), nameof(SqlTblPurchaseorder.Totaldiscountamount), nameof(SqlTblPurchaseorder.TenantId), nameof(SqlTblPurchaseorder.Shopid), nameof(SqlTblPurchaseorder.Shippingdescription), nameof(SqlTblPurchaseorder.Shippingamount));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblPurchaseorder.Totalamountbeforediscount), nameof(SqlTblPurchaseorder.Totalvatamount), nameof(SqlTblPurchaseorder.Totalinvoiceamount), nameof(SqlTblPurchaseorder.Totaldiscountamount), nameof(SqlTblPurchaseorder.TenantId), nameof(SqlTblPurchaseorder.Shopid), nameof(SqlTblPurchaseorder.Shippingdescription), nameof(SqlTblPurchaseorder.Shippingamount), nameof(SqlTblPurchaseorder.Endorsementid), nameof(SqlTblPurchaseorder.Insuranceid), nameof(SqlTblPurchaseorder.Salesnoticedate), nameof(SqlTblPurchaseorder.Salesnoticenumber), nameof(SqlTblPurchaseorder.Shippedproducts), nameof(SqlTblPurchaseorder.Driveridentificationnumber), nameof(SqlTblPurchaseorder.Fleetnumber), nameof(SqlTblPurchaseorder.Waybilltype), nameof(SqlTblPurchaseorder.Receiveridentificationnumber), nameof(SqlTblPurchaseorder.Senderidentificationnumber), nameof(SqlTblPurchaseorder.Destinationcity), nameof(SqlTblPurchaseorder.Destinationcountry), nameof(SqlTblPurchaseorder.Origincity), nameof(SqlTblPurchaseorder.Origincountry), nameof(SqlTblPurchaseorder.Referencewaybillnumber), nameof(SqlTblPurchaseorder.Waybillnumber), nameof(SqlTblPurchaseorder.Agencyeconomiccode), nameof(SqlTblPurchaseorder.Totalcurrencyamount), nameof(SqlTblPurchaseorder.Totalrialamount), nameof(SqlTblPurchaseorder.Totalnetweight), nameof(SqlTblPurchaseorder.Subscribernumber), nameof(SqlTblPurchaseorder.Customsdeclarationdate), nameof(SqlTblPurchaseorder.Customsdeclarationnumber), nameof(SqlTblPurchaseorder.Flighttype), nameof(SqlTblPurchaseorder.Totalarticle17taxamount), nameof(SqlTblPurchaseorder.Totalvatpaidamount), nameof(SqlTblPurchaseorder.Totalcreditamount), nameof(SqlTblPurchaseorder.Totalcashpaidamount), nameof(SqlTblPurchaseorder.Settlementtypeid), nameof(SqlTblPurchaseorder.Taxuniqueid), nameof(SqlTblPurchaseorder.Adjustmentsamount), nameof(SqlTblPurchaseorder.Totalothertaxesandchargesamount), nameof(SqlTblPurchaseorder.Sellercustomsofficecode), nameof(SqlTblPurchaseorder.Adjustments), nameof(SqlTblPurchaseorder.Duedate), nameof(SqlTblPurchaseorder.Customspermitnumber), nameof(SqlTblPurchaseorder.Description), nameof(SqlTblPurchaseorder.Invoiceformat), nameof(SqlTblPurchaseorder.Taxinvoiceinternalserial), nameof(SqlTblPurchaseorder.Totalamount), nameof(SqlTblPurchaseorder.Paidamount), nameof(SqlTblPurchaseorder.Invoicetype), nameof(SqlTblPurchaseorder.Totalamountafterdiscount), nameof(SqlTblPurchaseorder.Paymentstatus), nameof(SqlTblPurchaseorder.Inventorystatus), nameof(SqlTblPurchaseorder.Deliverystatus), nameof(SqlTblPurchaseorder.Supplierid), nameof(SqlTblPurchaseorder.Status), nameof(SqlTblPurchaseorder.Issuedatetime), nameof(SqlTblPurchaseorder.Invoicenumber), nameof(SqlTblPurchaseorder.Exchangerate), nameof(SqlTblPurchaseorder.Currency), nameof(SqlTblPurchaseorder.Projectid), nameof(SqlTblPurchaseorder.Warehouseid), nameof(SqlTblPurchaseorder.Fiscalperiodid), nameof(SqlTblPurchaseorder.Remainedamount), nameof(SqlTblPurchaseorder.Isstockcardautocreated));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد فاکتور خرید";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByStatusConfig : ChartConfigDefinition
        {
            public ByStatusConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "فاکتور خرید به تفکیک وضعیت صورتحساب";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblPurchaseorder.Status), "وضعیت صورتحساب"); Count(null, "تعداد"); }
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "فاکتور خرید به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblPurchaseorder.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblPurchaseorderitemUiDefinitions : CRUDDefinition<SqlTblPurchaseorderitem>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblPurchaseorderitem.Purchaseorderitemid), nameof(SqlTblPurchaseorderitem.Purchaseorderid), nameof(SqlTblPurchaseorderitem.Productid), nameof(SqlTblPurchaseorderitem.Quantity), nameof(SqlTblPurchaseorderitem.Unit), nameof(SqlTblPurchaseorderitem.Price), nameof(SqlTblPurchaseorderitem.Linepercentdiscount), nameof(SqlTblPurchaseorderitem.Linetotalamount), nameof(SqlTblPurchaseorderitem.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblPurchaseorderitem.Purchaseorderid), nameof(SqlTblPurchaseorderitem.Productid), nameof(SqlTblPurchaseorderitem.Quantity), nameof(SqlTblPurchaseorderitem.Unit), nameof(SqlTblPurchaseorderitem.Price), nameof(SqlTblPurchaseorderitem.Linepercentdiscount), nameof(SqlTblPurchaseorderitem.Linetotalamount), nameof(SqlTblPurchaseorderitem.TenantId), nameof(SqlTblPurchaseorderitem.Vatrate), nameof(SqlTblPurchaseorderitem.Unitconversionfactor), nameof(SqlTblPurchaseorderitem.Linevatpaidamount), nameof(SqlTblPurchaseorderitem.Linecashpaidamount), nameof(SqlTblPurchaseorderitem.Otherlegalduesamount), nameof(SqlTblPurchaseorderitem.Otherlegalduesrate), nameof(SqlTblPurchaseorderitem.Otherlegalduessubject), nameof(SqlTblPurchaseorderitem.Othertaxesandchargesamount), nameof(SqlTblPurchaseorderitem.Othertaxesandchargesrate), nameof(SqlTblPurchaseorderitem.Othertaxesandchargessubject), nameof(SqlTblPurchaseorderitem.Vatbaseamount), nameof(SqlTblPurchaseorderitem.Vatcalculationbase), nameof(SqlTblPurchaseorderitem.Currencybuyrate), nameof(SqlTblPurchaseorderitem.Purity), nameof(SqlTblPurchaseorderitem.Totalmakingcommissionprofit), nameof(SqlTblPurchaseorderitem.Commission), nameof(SqlTblPurchaseorderitem.Sellerprofit), nameof(SqlTblPurchaseorderitem.Makingcharge), nameof(SqlTblPurchaseorderitem.Linecurrencyvalue), nameof(SqlTblPurchaseorderitem.Linerialvalue), nameof(SqlTblPurchaseorderitem.Netweight), nameof(SqlTblPurchaseorderitem.Lineamountafterdiscount), nameof(SqlTblPurchaseorderitem.Commissioncontractnumber), nameof(SqlTblPurchaseorderitem.Linediscountamount), nameof(SqlTblPurchaseorderitem.Lineamountbeforediscount), nameof(SqlTblPurchaseorderitem.Linecurrencyamount), nameof(SqlTblPurchaseorderitem.Vatamount), nameof(SqlTblPurchaseorderitem.Lineexchangerate), nameof(SqlTblPurchaseorderitem.Linecurrency), nameof(SqlTblPurchaseorderitem.Projectid), nameof(SqlTblPurchaseorderitem.Linewarehouseid), nameof(SqlTblPurchaseorderitem.Fiscalperiodid), nameof(SqlTblPurchaseorderitem.Shopid));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد اقلام فاکتور خرید";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "اقلام فاکتور خرید به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblPurchaseorderitem.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblPurchaseorderreturnUiDefinitions : CRUDDefinition<SqlTblPurchaseorderreturn>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblPurchaseorderreturn.Purchaseorderreturnid), nameof(SqlTblPurchaseorderreturn.Shopid), nameof(SqlTblPurchaseorderreturn.Fiscalperiodid), nameof(SqlTblPurchaseorderreturn.Warehouseid), nameof(SqlTblPurchaseorderreturn.Projectid), nameof(SqlTblPurchaseorderreturn.Referencepurchaseorderid), nameof(SqlTblPurchaseorderreturn.Invoicenumber), nameof(SqlTblPurchaseorderreturn.Issuedatetime), nameof(SqlTblPurchaseorderreturn.Creationdatetime));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblPurchaseorderreturn.Shopid), nameof(SqlTblPurchaseorderreturn.Fiscalperiodid), nameof(SqlTblPurchaseorderreturn.Warehouseid), nameof(SqlTblPurchaseorderreturn.Projectid), nameof(SqlTblPurchaseorderreturn.Referencepurchaseorderid), nameof(SqlTblPurchaseorderreturn.Invoicenumber), nameof(SqlTblPurchaseorderreturn.Issuedatetime), nameof(SqlTblPurchaseorderreturn.Creationdatetime), nameof(SqlTblPurchaseorderreturn.Status), nameof(SqlTblPurchaseorderreturn.Deliverystatus), nameof(SqlTblPurchaseorderreturn.Inventorystatus), nameof(SqlTblPurchaseorderreturn.Paymentstatus), nameof(SqlTblPurchaseorderreturn.Totalreturnamountbeforediscount), nameof(SqlTblPurchaseorderreturn.Totaldiscountreturnamount), nameof(SqlTblPurchaseorderreturn.Totalreturnamountafterdiscount), nameof(SqlTblPurchaseorderreturn.Totalvatreturnamount), nameof(SqlTblPurchaseorderreturn.Totalinvoicereturnamount), nameof(SqlTblPurchaseorderreturn.Paidreturnamount), nameof(SqlTblPurchaseorderreturn.Totalreturnamount), nameof(SqlTblPurchaseorderreturn.Description), nameof(SqlTblPurchaseorderreturn.Adjustments), nameof(SqlTblPurchaseorderreturn.Adjustmentsamount), nameof(SqlTblPurchaseorderreturn.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد فاکتور برگشت از خرید";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByStatusConfig : ChartConfigDefinition
        {
            public ByStatusConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "فاکتور برگشت از خرید به تفکیک وضعیت صورتحساب برگشتی";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblPurchaseorderreturn.Status), "وضعیت صورتحساب برگشتی"); Count(null, "تعداد"); }
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "فاکتور برگشت از خرید به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblPurchaseorderreturn.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblPurchaseorderreturnitemUiDefinitions : CRUDDefinition<SqlTblPurchaseorderreturnitem>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblPurchaseorderreturnitem.Purchaseorderreturnitemid), nameof(SqlTblPurchaseorderreturnitem.Referencepurchaseorderitemid), nameof(SqlTblPurchaseorderreturnitem.Purchaseorderreturnid), nameof(SqlTblPurchaseorderreturnitem.Shopid), nameof(SqlTblPurchaseorderreturnitem.Fiscalperiodid), nameof(SqlTblPurchaseorderreturnitem.Linewarehouseid), nameof(SqlTblPurchaseorderreturnitem.Projectid), nameof(SqlTblPurchaseorderreturnitem.Productid), nameof(SqlTblPurchaseorderreturnitem.Returnquantity));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblPurchaseorderreturnitem.Referencepurchaseorderitemid), nameof(SqlTblPurchaseorderreturnitem.Purchaseorderreturnid), nameof(SqlTblPurchaseorderreturnitem.Shopid), nameof(SqlTblPurchaseorderreturnitem.Fiscalperiodid), nameof(SqlTblPurchaseorderreturnitem.Linewarehouseid), nameof(SqlTblPurchaseorderreturnitem.Projectid), nameof(SqlTblPurchaseorderreturnitem.Productid), nameof(SqlTblPurchaseorderreturnitem.Returnquantity), nameof(SqlTblPurchaseorderreturnitem.Unit), nameof(SqlTblPurchaseorderreturnitem.Unitconversionfactor), nameof(SqlTblPurchaseorderreturnitem.Price), nameof(SqlTblPurchaseorderreturnitem.Linereturnamountbeforediscount), nameof(SqlTblPurchaseorderreturnitem.Linepercentdiscount), nameof(SqlTblPurchaseorderreturnitem.Linediscountreturnamount), nameof(SqlTblPurchaseorderreturnitem.Linereturnamountafterdiscount), nameof(SqlTblPurchaseorderreturnitem.Vatrate), nameof(SqlTblPurchaseorderreturnitem.Vatreturnamount), nameof(SqlTblPurchaseorderreturnitem.Linetotalreturnamount), nameof(SqlTblPurchaseorderreturnitem.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد اقلام فاکتور برگشت از خرید";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "اقلام فاکتور برگشت از خرید به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblPurchaseorderreturnitem.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblReleasenoteUiDefinitions : CRUDDefinition<SqlTblReleasenote>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblReleasenote.Releaseid), nameof(SqlTblReleasenote.Releasedate), nameof(SqlTblReleasenote.Releaseversion), nameof(SqlTblReleasenote.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblReleasenote.Releasedate), nameof(SqlTblReleasenote.Releaseversion), nameof(SqlTblReleasenote.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد تغییرات نسخه";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlTblReleasenoteitemUiDefinitions : CRUDDefinition<SqlTblReleasenoteitem>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblReleasenoteitem.Releaseitemid), nameof(SqlTblReleasenoteitem.Releaseid), nameof(SqlTblReleasenoteitem.Releaseitemorder), nameof(SqlTblReleasenoteitem.Releaseitemtitle), nameof(SqlTblReleasenoteitem.Releaseitemdescription), nameof(SqlTblReleasenoteitem.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblReleasenoteitem.Releaseid), nameof(SqlTblReleasenoteitem.Releaseitemorder), nameof(SqlTblReleasenoteitem.Releaseitemtitle), nameof(SqlTblReleasenoteitem.Releaseitemdescription), nameof(SqlTblReleasenoteitem.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد آیتم تغییرات نسخه";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlTblSaleorderUiDefinitions : CRUDDefinition<SqlTblSaleorder>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblSaleorder.Saleorderid), nameof(SqlTblSaleorder.Totalamountbeforediscount), nameof(SqlTblSaleorder.Totalvatamount), nameof(SqlTblSaleorder.Totaldiscountamount), nameof(SqlTblSaleorder.Totalinvoiceamount), nameof(SqlTblSaleorder.TenantId), nameof(SqlTblSaleorder.Shopid), nameof(SqlTblSaleorder.Shippingdescription), nameof(SqlTblSaleorder.Shippingamount));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblSaleorder.Totalamountbeforediscount), nameof(SqlTblSaleorder.Totalvatamount), nameof(SqlTblSaleorder.Totaldiscountamount), nameof(SqlTblSaleorder.Totalinvoiceamount), nameof(SqlTblSaleorder.TenantId), nameof(SqlTblSaleorder.Shopid), nameof(SqlTblSaleorder.Shippingdescription), nameof(SqlTblSaleorder.Shippingamount), nameof(SqlTblSaleorder.Courierid), nameof(SqlTblSaleorder.Taxpayerportalinquiryresponse), nameof(SqlTblSaleorder.Endorsementid), nameof(SqlTblSaleorder.Insuranceid), nameof(SqlTblSaleorder.Salesnoticedate), nameof(SqlTblSaleorder.Salesnoticenumber), nameof(SqlTblSaleorder.Shippedproducts), nameof(SqlTblSaleorder.Driveridentificationnumber), nameof(SqlTblSaleorder.Fleetnumber), nameof(SqlTblSaleorder.Waybilltype), nameof(SqlTblSaleorder.Receiveridentificationnumber), nameof(SqlTblSaleorder.Senderidentificationnumber), nameof(SqlTblSaleorder.Destinationcity), nameof(SqlTblSaleorder.Destinationcountry), nameof(SqlTblSaleorder.Origincity), nameof(SqlTblSaleorder.Origincountry), nameof(SqlTblSaleorder.Referencewaybillnumber), nameof(SqlTblSaleorder.Waybillnumber), nameof(SqlTblSaleorder.Agencyeconomiccode), nameof(SqlTblSaleorder.Totalcurrencyamount), nameof(SqlTblSaleorder.Totalrialamount), nameof(SqlTblSaleorder.Totalnetweight), nameof(SqlTblSaleorder.Subscribernumber), nameof(SqlTblSaleorder.Customsdeclarationdate), nameof(SqlTblSaleorder.Customsdeclarationnumber), nameof(SqlTblSaleorder.Customerpassportnumber), nameof(SqlTblSaleorder.Flighttype), nameof(SqlTblSaleorder.Adjustmentsamount), nameof(SqlTblSaleorder.Adjustments), nameof(SqlTblSaleorder.Duedate), nameof(SqlTblSaleorder.Description), nameof(SqlTblSaleorder.Taxpayerportalstatus), nameof(SqlTblSaleorder.Totalarticle17taxamount), nameof(SqlTblSaleorder.Totalvatpaidamount), nameof(SqlTblSaleorder.Totalcreditamount), nameof(SqlTblSaleorder.Totalcashpaidamount), nameof(SqlTblSaleorder.Settlementtypeid), nameof(SqlTblSaleorder.Totalamount), nameof(SqlTblSaleorder.Paidamount), nameof(SqlTblSaleorder.Totalamountafterdiscount), nameof(SqlTblSaleorder.Totalothertaxesandchargesamount), nameof(SqlTblSaleorder.Paymentstatus), nameof(SqlTblSaleorder.Inventorystatus), nameof(SqlTblSaleorder.Deliverystatus), nameof(SqlTblSaleorder.Status), nameof(SqlTblSaleorder.Contractnumber), nameof(SqlTblSaleorder.Sellercustomsofficecode), nameof(SqlTblSaleorder.Customspermitnumber), nameof(SqlTblSaleorder.Customerbranchcode), nameof(SqlTblSaleorder.Customerpostalcode), nameof(SqlTblSaleorder.Sellerbranchcode), nameof(SqlTblSaleorder.Customereconomiccode), nameof(SqlTblSaleorder.Customeridentificationnumber), nameof(SqlTblSaleorder.Customerpersontype), nameof(SqlTblSaleorder.Sellereconomiccode), nameof(SqlTblSaleorder.Customerid), nameof(SqlTblSaleorder.Salespersonid), nameof(SqlTblSaleorder.Invoicesubject), nameof(SqlTblSaleorder.Invoiceformat), nameof(SqlTblSaleorder.Referencetaxuniqueid), nameof(SqlTblSaleorder.Taxinvoiceinternalserial), nameof(SqlTblSaleorder.Invoicetype), nameof(SqlTblSaleorder.Creationdatetime), nameof(SqlTblSaleorder.Issuedatetime), nameof(SqlTblSaleorder.Taxuniqueid), nameof(SqlTblSaleorder.Invoicenumber), nameof(SqlTblSaleorder.Exchangerate), nameof(SqlTblSaleorder.Currency), nameof(SqlTblSaleorder.Fiscalperiodid), nameof(SqlTblSaleorder.Projectid), nameof(SqlTblSaleorder.Warehouseid), nameof(SqlTblSaleorder.Referencesaleorderid), nameof(SqlTblSaleorder.Remainedamount), nameof(SqlTblSaleorder.Isstockcardautocreated));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد فاکتور فروش";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByStatusConfig : ChartConfigDefinition
        {
            public ByStatusConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "فاکتور فروش به تفکیک وضعیت صورتحساب";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblSaleorder.Status), "وضعیت صورتحساب"); Count(null, "تعداد"); }
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "فاکتور فروش به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblSaleorder.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblSaleorderconfigUiDefinitions : CRUDDefinition<SqlTblSaleorderconfig>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblSaleorderconfig.Configid), nameof(SqlTblSaleorderconfig.Updatesalepriceoninvoicesave), nameof(SqlTblSaleorderconfig.Updatepurchasepriceoninvoicesave), nameof(SqlTblSaleorderconfig.Notifyuserafterpriceupdate), nameof(SqlTblSaleorderconfig.Allowlowstocksale), nameof(SqlTblSaleorderconfig.Showzeronegativestockitems), nameof(SqlTblSaleorderconfig.Allowduplicateitemsininvoice), nameof(SqlTblSaleorderconfig.Checkcustomercreditonsale), nameof(SqlTblSaleorderconfig.Warnsalebelowpurchaseprice));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblSaleorderconfig.Updatesalepriceoninvoicesave), nameof(SqlTblSaleorderconfig.Updatepurchasepriceoninvoicesave), nameof(SqlTblSaleorderconfig.Notifyuserafterpriceupdate), nameof(SqlTblSaleorderconfig.Allowlowstocksale), nameof(SqlTblSaleorderconfig.Showzeronegativestockitems), nameof(SqlTblSaleorderconfig.Allowduplicateitemsininvoice), nameof(SqlTblSaleorderconfig.Checkcustomercreditonsale), nameof(SqlTblSaleorderconfig.Warnsalebelowpurchaseprice), nameof(SqlTblSaleorderconfig.Showprofitininvoice), nameof(SqlTblSaleorderconfig.Defaultcustomer), nameof(SqlTblSaleorderconfig.Salespersonrequired), nameof(SqlTblSaleorderconfig.Enableautowarehouseissue), nameof(SqlTblSaleorderconfig.Invoiceprintmethod), nameof(SqlTblSaleorderconfig.Enablescale), nameof(SqlTblSaleorderconfig.Shopid), nameof(SqlTblSaleorderconfig.TenantId), nameof(SqlTblSaleorderconfig.Paymentpageopeningmode));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد تنظیمات فروش";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تنظیمات فروش به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblSaleorderconfig.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblSaleorderitemUiDefinitions : CRUDDefinition<SqlTblSaleorderitem>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblSaleorderitem.Saleorderitemid), nameof(SqlTblSaleorderitem.Saleorderid), nameof(SqlTblSaleorderitem.Productid), nameof(SqlTblSaleorderitem.Quantity), nameof(SqlTblSaleorderitem.Unit), nameof(SqlTblSaleorderitem.Price), nameof(SqlTblSaleorderitem.Linepercentdiscount), nameof(SqlTblSaleorderitem.Linetotalamount), nameof(SqlTblSaleorderitem.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblSaleorderitem.Saleorderid), nameof(SqlTblSaleorderitem.Productid), nameof(SqlTblSaleorderitem.Quantity), nameof(SqlTblSaleorderitem.Unit), nameof(SqlTblSaleorderitem.Price), nameof(SqlTblSaleorderitem.Linepercentdiscount), nameof(SqlTblSaleorderitem.Linetotalamount), nameof(SqlTblSaleorderitem.TenantId), nameof(SqlTblSaleorderitem.Cost), nameof(SqlTblSaleorderitem.Vatrate), nameof(SqlTblSaleorderitem.Unitconversionfactor), nameof(SqlTblSaleorderitem.Vatamount), nameof(SqlTblSaleorderitem.Productdescription), nameof(SqlTblSaleorderitem.Producttaxcode), nameof(SqlTblSaleorderitem.Projectid), nameof(SqlTblSaleorderitem.Linewarehouseid), nameof(SqlTblSaleorderitem.Fiscalperiodid), nameof(SqlTblSaleorderitem.Shopid), nameof(SqlTblSaleorderitem.Vatbaseamount), nameof(SqlTblSaleorderitem.Vatcalculationbase), nameof(SqlTblSaleorderitem.Currencybuyrate), nameof(SqlTblSaleorderitem.Purity), nameof(SqlTblSaleorderitem.Totalmakingcommissionprofit), nameof(SqlTblSaleorderitem.Commission), nameof(SqlTblSaleorderitem.Sellerprofit), nameof(SqlTblSaleorderitem.Makingcharge), nameof(SqlTblSaleorderitem.Linecurrencyvalue), nameof(SqlTblSaleorderitem.Commissioncontractnumber), nameof(SqlTblSaleorderitem.Linerialvalue), nameof(SqlTblSaleorderitem.Netweight), nameof(SqlTblSaleorderitem.Linevatpaidamount), nameof(SqlTblSaleorderitem.Linecashpaidamount), nameof(SqlTblSaleorderitem.Otherlegalduesamount), nameof(SqlTblSaleorderitem.Otherlegalduesrate), nameof(SqlTblSaleorderitem.Otherlegalduessubject), nameof(SqlTblSaleorderitem.Othertaxesandchargesamount), nameof(SqlTblSaleorderitem.Othertaxesandchargesrate), nameof(SqlTblSaleorderitem.Othertaxesandchargessubject), nameof(SqlTblSaleorderitem.Lineamountafterdiscount), nameof(SqlTblSaleorderitem.Linediscountamount), nameof(SqlTblSaleorderitem.Lineamountbeforediscount), nameof(SqlTblSaleorderitem.Linecurrencyamount), nameof(SqlTblSaleorderitem.Lineexchangerate), nameof(SqlTblSaleorderitem.Linecurrency));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد اقلام فاکتور فروش";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "اقلام فاکتور فروش به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblSaleorderitem.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblSaleorderreturnUiDefinitions : CRUDDefinition<SqlTblSaleorderreturn>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblSaleorderreturn.Saleorderreturnid), nameof(SqlTblSaleorderreturn.Shopid), nameof(SqlTblSaleorderreturn.Fiscalperiodid), nameof(SqlTblSaleorderreturn.Warehouseid), nameof(SqlTblSaleorderreturn.Projectid), nameof(SqlTblSaleorderreturn.Referencesaleorderid), nameof(SqlTblSaleorderreturn.Invoicenumber), nameof(SqlTblSaleorderreturn.Issuedatetime), nameof(SqlTblSaleorderreturn.Creationdatetime));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblSaleorderreturn.Shopid), nameof(SqlTblSaleorderreturn.Fiscalperiodid), nameof(SqlTblSaleorderreturn.Warehouseid), nameof(SqlTblSaleorderreturn.Projectid), nameof(SqlTblSaleorderreturn.Referencesaleorderid), nameof(SqlTblSaleorderreturn.Invoicenumber), nameof(SqlTblSaleorderreturn.Issuedatetime), nameof(SqlTblSaleorderreturn.Creationdatetime), nameof(SqlTblSaleorderreturn.Status), nameof(SqlTblSaleorderreturn.Deliverystatus), nameof(SqlTblSaleorderreturn.Inventorystatus), nameof(SqlTblSaleorderreturn.Paymentstatus), nameof(SqlTblSaleorderreturn.Totalreturnamountbeforediscount), nameof(SqlTblSaleorderreturn.Totaldiscountreturnamount), nameof(SqlTblSaleorderreturn.Totalreturnamountafterdiscount), nameof(SqlTblSaleorderreturn.Totalvatreturnamount), nameof(SqlTblSaleorderreturn.Totalothertaxesandchargesreturnamount), nameof(SqlTblSaleorderreturn.Totalinvoicereturnamount), nameof(SqlTblSaleorderreturn.Paidreturnamount), nameof(SqlTblSaleorderreturn.Totalreturnamount), nameof(SqlTblSaleorderreturn.Description), nameof(SqlTblSaleorderreturn.Adjustments), nameof(SqlTblSaleorderreturn.Adjustmentsamount), nameof(SqlTblSaleorderreturn.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد فاکتور برگشت از فروش";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByStatusConfig : ChartConfigDefinition
        {
            public ByStatusConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "فاکتور برگشت از فروش به تفکیک وضعیت صورتحساب برگشتی";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblSaleorderreturn.Status), "وضعیت صورتحساب برگشتی"); Count(null, "تعداد"); }
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "فاکتور برگشت از فروش به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblSaleorderreturn.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblSaleorderreturnitemUiDefinitions : CRUDDefinition<SqlTblSaleorderreturnitem>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblSaleorderreturnitem.Saleorderreturnitemid), nameof(SqlTblSaleorderreturnitem.Referencesaleorderitemid), nameof(SqlTblSaleorderreturnitem.Saleorderreturnid), nameof(SqlTblSaleorderreturnitem.Shopid), nameof(SqlTblSaleorderreturnitem.Fiscalperiodid), nameof(SqlTblSaleorderreturnitem.Linewarehouseid), nameof(SqlTblSaleorderreturnitem.Projectid), nameof(SqlTblSaleorderreturnitem.Productid), nameof(SqlTblSaleorderreturnitem.Returnquantity));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblSaleorderreturnitem.Referencesaleorderitemid), nameof(SqlTblSaleorderreturnitem.Saleorderreturnid), nameof(SqlTblSaleorderreturnitem.Shopid), nameof(SqlTblSaleorderreturnitem.Fiscalperiodid), nameof(SqlTblSaleorderreturnitem.Linewarehouseid), nameof(SqlTblSaleorderreturnitem.Projectid), nameof(SqlTblSaleorderreturnitem.Productid), nameof(SqlTblSaleorderreturnitem.Returnquantity), nameof(SqlTblSaleorderreturnitem.Unit), nameof(SqlTblSaleorderreturnitem.Unitconversionfactor), nameof(SqlTblSaleorderreturnitem.Price), nameof(SqlTblSaleorderreturnitem.Linereturnamountbeforediscount), nameof(SqlTblSaleorderreturnitem.Linepercentdiscount), nameof(SqlTblSaleorderreturnitem.Linediscountreturnamount), nameof(SqlTblSaleorderreturnitem.Linereturnamountafterdiscount), nameof(SqlTblSaleorderreturnitem.Vatrate), nameof(SqlTblSaleorderreturnitem.Vatreturnamount), nameof(SqlTblSaleorderreturnitem.Linetotalreturnamount), nameof(SqlTblSaleorderreturnitem.Othertaxesandchargesreturnamount), nameof(SqlTblSaleorderreturnitem.Otherlegalduesreturnamount), nameof(SqlTblSaleorderreturnitem.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد اقلام فاکتور برگشت از فروش";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "اقلام فاکتور برگشت از فروش به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblSaleorderreturnitem.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblServicediscountUiDefinitions : CRUDDefinition<SqlTblServicediscount>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblServicediscount.Id), nameof(SqlTblServicediscount.Name), nameof(SqlTblServicediscount.Code), nameof(SqlTblServicediscount.Startdate), nameof(SqlTblServicediscount.Enddate), nameof(SqlTblServicediscount.Ispublic), nameof(SqlTblServicediscount.Percentamount), nameof(SqlTblServicediscount.Fixedamount), nameof(SqlTblServicediscount.Description));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblServicediscount.Name), nameof(SqlTblServicediscount.Code), nameof(SqlTblServicediscount.Startdate), nameof(SqlTblServicediscount.Enddate), nameof(SqlTblServicediscount.Ispublic), nameof(SqlTblServicediscount.Percentamount), nameof(SqlTblServicediscount.Fixedamount), nameof(SqlTblServicediscount.Description), nameof(SqlTblServicediscount.Isenabled), nameof(SqlTblServicediscount.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد تخفیف اشتراک";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlTblServicepaymentUiDefinitions : CRUDDefinition<SqlTblServicepayment>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblServicepayment.Paymentid), nameof(SqlTblServicepayment.Invoicenumber), nameof(SqlTblServicepayment.Invoicedate), nameof(SqlTblServicepayment.Transactionreferenceid), nameof(SqlTblServicepayment.Amount), nameof(SqlTblServicepayment.Transactiondate), nameof(SqlTblServicepayment.Referencenumber), nameof(SqlTblServicepayment.Maskedcardnumber), nameof(SqlTblServicepayment.Shaparakrefnumber));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblServicepayment.Invoicenumber), nameof(SqlTblServicepayment.Invoicedate), nameof(SqlTblServicepayment.Transactionreferenceid), nameof(SqlTblServicepayment.Amount), nameof(SqlTblServicepayment.Transactiondate), nameof(SqlTblServicepayment.Referencenumber), nameof(SqlTblServicepayment.Maskedcardnumber), nameof(SqlTblServicepayment.Shaparakrefnumber), nameof(SqlTblServicepayment.Issuccess), nameof(SqlTblServicepayment.Message), nameof(SqlTblServicepayment.Tracenumber), nameof(SqlTblServicepayment.Registertime), nameof(SqlTblServicepayment.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد پرداخت سرویس";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlTblServiceplanUiDefinitions : CRUDDefinition<SqlTblServiceplan>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblServiceplan.Planid), nameof(SqlTblServiceplan.Planname), nameof(SqlTblServiceplan.Plandescription), nameof(SqlTblServiceplan.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblServiceplan.Planname), nameof(SqlTblServiceplan.Plandescription), nameof(SqlTblServiceplan.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد نوع دسترسی اشتراک";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlTblServicerequestUiDefinitions : CRUDDefinition<SqlTblServicerequest>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblServicerequest.Id), nameof(SqlTblServicerequest.Shopid), nameof(SqlTblServicerequest.Subscriptionid), nameof(SqlTblServicerequest.Status), nameof(SqlTblServicerequest.Requesttime), nameof(SqlTblServicerequest.Paymentid), nameof(SqlTblServicerequest.Price), nameof(SqlTblServicerequest.Discountid), nameof(SqlTblServicerequest.Discountamount));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblServicerequest.Shopid), nameof(SqlTblServicerequest.Subscriptionid), nameof(SqlTblServicerequest.Status), nameof(SqlTblServicerequest.Requesttime), nameof(SqlTblServicerequest.Paymentid), nameof(SqlTblServicerequest.Price), nameof(SqlTblServicerequest.Discountid), nameof(SqlTblServicerequest.Discountamount), nameof(SqlTblServicerequest.Payableamount), nameof(SqlTblServicerequest.Subscriptionstartdate), nameof(SqlTblServicerequest.Subscriptionenddate), nameof(SqlTblServicerequest.TenantId), nameof(SqlTblServicerequest.Userid), nameof(SqlTblServicerequest.Taxamount));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد درخواست اشتراک";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByStatusConfig : ChartConfigDefinition
        {
            public ByStatusConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "درخواست اشتراک به تفکیک وضعیت اشتراک";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblServicerequest.Status), "وضعیت اشتراک"); Count(null, "تعداد"); }
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "درخواست اشتراک به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblServicerequest.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblServicesubscriptionUiDefinitions : CRUDDefinition<SqlTblServicesubscription>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblServicesubscription.Subscriptionid), nameof(SqlTblServicesubscription.Subscriptionname), nameof(SqlTblServicesubscription.Planid), nameof(SqlTblServicesubscription.Subscriptionprice), nameof(SqlTblServicesubscription.Subscriptiondescription), nameof(SqlTblServicesubscription.Subscriptionenable), nameof(SqlTblServicesubscription.Subscriptiondefault), nameof(SqlTblServicesubscription.TenantId), nameof(SqlTblServicesubscription.Percentdiscount));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblServicesubscription.Subscriptionname), nameof(SqlTblServicesubscription.Planid), nameof(SqlTblServicesubscription.Subscriptionprice), nameof(SqlTblServicesubscription.Subscriptiondescription), nameof(SqlTblServicesubscription.Subscriptionenable), nameof(SqlTblServicesubscription.Subscriptiondefault), nameof(SqlTblServicesubscription.TenantId), nameof(SqlTblServicesubscription.Percentdiscount), nameof(SqlTblServicesubscription.Fixeddiscount), nameof(SqlTblServicesubscription.Subscriptiontaxrate), nameof(SqlTblServicesubscription.Subscriptiondays), nameof(SqlTblServicesubscription.Subscriptionpayableamount), nameof(SqlTblServicesubscription.Subscriptiontaxamount));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد نوع اشتراک";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlTblShareholderUiDefinitions : CRUDDefinition<SqlTblShareholder>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblShareholder.Shareholderid), nameof(SqlTblShareholder.Shopid), nameof(SqlTblShareholder.Fiscalperiodid), nameof(SqlTblShareholder.Personid), nameof(SqlTblShareholder.Sharepercent), nameof(SqlTblShareholder.Description), nameof(SqlTblShareholder.Initialcapitalshare), nameof(SqlTblShareholder.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblShareholder.Shopid), nameof(SqlTblShareholder.Fiscalperiodid), nameof(SqlTblShareholder.Personid), nameof(SqlTblShareholder.Sharepercent), nameof(SqlTblShareholder.Description), nameof(SqlTblShareholder.Initialcapitalshare), nameof(SqlTblShareholder.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد سهامدار";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "سهامدار به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblShareholder.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblShopUiDefinitions : CRUDDefinition<SqlTblShop>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblShop.Shopid), nameof(SqlTblShop.Ownerid), nameof(SqlTblShop.Name), nameof(SqlTblShop.Description), nameof(SqlTblShop.Addressline), nameof(SqlTblShop.Postalcode), nameof(SqlTblShop.Email), nameof(SqlTblShop.Phone), nameof(SqlTblShop.Economiccode));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblShop.Ownerid), nameof(SqlTblShop.Name), nameof(SqlTblShop.Description), nameof(SqlTblShop.Addressline), nameof(SqlTblShop.Postalcode), nameof(SqlTblShop.Email), nameof(SqlTblShop.Phone), nameof(SqlTblShop.Economiccode), nameof(SqlTblShop.Identifiernumber), nameof(SqlTblShop.TenantId), nameof(SqlTblShop.Activitytypeid), nameof(SqlTblShop.Logorelativeurl), nameof(SqlTblShop.Mobile), nameof(SqlTblShop.Branchcode), nameof(SqlTblShop.Country), nameof(SqlTblShop.Website), nameof(SqlTblShop.Type), nameof(SqlTblShop.Province), nameof(SqlTblShop.City), nameof(SqlTblShop.Status));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد مغازه";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByStatusConfig : ChartConfigDefinition
        {
            public ByStatusConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "مغازه به تفکیک وضعیت";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblShop.Status), "وضعیت"); Count(null, "تعداد"); }
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "مغازه به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblShop.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblShopactivitytypeUiDefinitions : CRUDDefinition<SqlTblShopactivitytype>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblShopactivitytype.Activitytypeid), nameof(SqlTblShopactivitytype.Activitytypename), nameof(SqlTblShopactivitytype.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblShopactivitytype.Activitytypeid), nameof(SqlTblShopactivitytype.Activitytypename), nameof(SqlTblShopactivitytype.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد نوع فعالیت فروشگاه";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlTblShopfiscalperiodUiDefinitions : CRUDDefinition<SqlTblShopfiscalperiod>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblShopfiscalperiod.Fiscalperiodid), nameof(SqlTblShopfiscalperiod.Fiscalperiodname), nameof(SqlTblShopfiscalperiod.Fiscalperioddescription), nameof(SqlTblShopfiscalperiod.Startdate), nameof(SqlTblShopfiscalperiod.Enddate), nameof(SqlTblShopfiscalperiod.Shopid), nameof(SqlTblShopfiscalperiod.Fiscalperiodstatusid), nameof(SqlTblShopfiscalperiod.Firstperiod), nameof(SqlTblShopfiscalperiod.Lastdocumentnumber));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblShopfiscalperiod.Fiscalperiodname), nameof(SqlTblShopfiscalperiod.Fiscalperioddescription), nameof(SqlTblShopfiscalperiod.Startdate), nameof(SqlTblShopfiscalperiod.Enddate), nameof(SqlTblShopfiscalperiod.Shopid), nameof(SqlTblShopfiscalperiod.Fiscalperiodstatusid), nameof(SqlTblShopfiscalperiod.Firstperiod), nameof(SqlTblShopfiscalperiod.Lastdocumentnumber), nameof(SqlTblShopfiscalperiod.Lastreferencenumber), nameof(SqlTblShopfiscalperiod.Lastdocumentdatetime), nameof(SqlTblShopfiscalperiod.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد دوره مالی فروشگاه";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "دوره مالی فروشگاه به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblShopfiscalperiod.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblShopnotificationUiDefinitions : CRUDDefinition<SqlTblShopnotification>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblShopnotification.Notificationid), nameof(SqlTblShopnotification.Notificationtypeid), nameof(SqlTblShopnotification.Notificationtitle), nameof(SqlTblShopnotification.Notificationtext), nameof(SqlTblShopnotification.Notificationcloseallowed), nameof(SqlTblShopnotification.Notificationstarttime), nameof(SqlTblShopnotification.Notificationendtime), nameof(SqlTblShopnotification.Notificationallshops), nameof(SqlTblShopnotification.Notificationallmemberroles));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblShopnotification.Notificationtypeid), nameof(SqlTblShopnotification.Notificationtitle), nameof(SqlTblShopnotification.Notificationtext), nameof(SqlTblShopnotification.Notificationcloseallowed), nameof(SqlTblShopnotification.Notificationstarttime), nameof(SqlTblShopnotification.Notificationendtime), nameof(SqlTblShopnotification.Notificationallshops), nameof(SqlTblShopnotification.Notificationallmemberroles), nameof(SqlTblShopnotification.TenantId), nameof(SqlTblShopnotification.Isbanner));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد اعلان فروشگاه";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlTblShopnotificationinmemberroleUiDefinitions : CRUDDefinition<SqlTblShopnotificationinmemberrole>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblShopnotificationinmemberrole.Notificationinmemberroleid), nameof(SqlTblShopnotificationinmemberrole.Notificationid), nameof(SqlTblShopnotificationinmemberrole.Memberroleid), nameof(SqlTblShopnotificationinmemberrole.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblShopnotificationinmemberrole.Notificationid), nameof(SqlTblShopnotificationinmemberrole.Memberroleid), nameof(SqlTblShopnotificationinmemberrole.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد اعلان فروشگاه در نقش عضو فروشگاه";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlTblShopnotificationinshopUiDefinitions : CRUDDefinition<SqlTblShopnotificationinshop>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblShopnotificationinshop.Notificationinshopid), nameof(SqlTblShopnotificationinshop.Notificationid), nameof(SqlTblShopnotificationinshop.Shopid), nameof(SqlTblShopnotificationinshop.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblShopnotificationinshop.Notificationid), nameof(SqlTblShopnotificationinshop.Shopid), nameof(SqlTblShopnotificationinshop.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد اعلان فروشگاه در فروشگاه";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "اعلان فروشگاه در فروشگاه به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblShopnotificationinshop.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblShopnotificationtypeUiDefinitions : CRUDDefinition<SqlTblShopnotificationtype>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblShopnotificationtype.Notificationtypeid), nameof(SqlTblShopnotificationtype.Notificationtypename), nameof(SqlTblShopnotificationtype.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblShopnotificationtype.Notificationtypeid), nameof(SqlTblShopnotificationtype.Notificationtypename), nameof(SqlTblShopnotificationtype.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد نوع اعلان فروشگاه";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlTblStockcardUiDefinitions : CRUDDefinition<SqlTblStockcard>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblStockcard.Id), nameof(SqlTblStockcard.Warehouseid), nameof(SqlTblStockcard.Shopid), nameof(SqlTblStockcard.Fiscalperiodid), nameof(SqlTblStockcard.Projectid), nameof(SqlTblStockcard.Datetime), nameof(SqlTblStockcard.Number), nameof(SqlTblStockcard.Description), nameof(SqlTblStockcard.Personid));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblStockcard.Warehouseid), nameof(SqlTblStockcard.Shopid), nameof(SqlTblStockcard.Fiscalperiodid), nameof(SqlTblStockcard.Projectid), nameof(SqlTblStockcard.Datetime), nameof(SqlTblStockcard.Number), nameof(SqlTblStockcard.Description), nameof(SqlTblStockcard.Personid), nameof(SqlTblStockcard.Direction), nameof(SqlTblStockcard.Referencetype), nameof(SqlTblStockcard.Referenceid), nameof(SqlTblStockcard.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد کاردکس انبار";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "کاردکس انبار به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblStockcard.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblStockcarditemUiDefinitions : CRUDDefinition<SqlTblStockcarditem>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblStockcarditem.Id), nameof(SqlTblStockcarditem.Stockcardid), nameof(SqlTblStockcarditem.Shopid), nameof(SqlTblStockcarditem.Productid), nameof(SqlTblStockcarditem.Quantity), nameof(SqlTblStockcarditem.Showinsecondunit), nameof(SqlTblStockcarditem.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblStockcarditem.Stockcardid), nameof(SqlTblStockcarditem.Shopid), nameof(SqlTblStockcarditem.Productid), nameof(SqlTblStockcarditem.Quantity), nameof(SqlTblStockcarditem.Showinsecondunit), nameof(SqlTblStockcarditem.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد آیتم کاردکس انبار";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "آیتم کاردکس انبار به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblStockcarditem.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblStocktakingUiDefinitions : CRUDDefinition<SqlTblStocktaking>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblStocktaking.Id), nameof(SqlTblStocktaking.Shopid), nameof(SqlTblStocktaking.Warehouseid), nameof(SqlTblStocktaking.Fiscalperiodid), nameof(SqlTblStocktaking.Projectid), nameof(SqlTblStocktaking.Description), nameof(SqlTblStocktaking.Status), nameof(SqlTblStocktaking.Datetime), nameof(SqlTblStocktaking.Number));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblStocktaking.Shopid), nameof(SqlTblStocktaking.Warehouseid), nameof(SqlTblStocktaking.Fiscalperiodid), nameof(SqlTblStocktaking.Projectid), nameof(SqlTblStocktaking.Description), nameof(SqlTblStocktaking.Status), nameof(SqlTblStocktaking.Datetime), nameof(SqlTblStocktaking.Number), nameof(SqlTblStocktaking.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد انبارگردانی";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByStatusConfig : ChartConfigDefinition
        {
            public ByStatusConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "انبارگردانی به تفکیک وضعیت انبارگردانی";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblStocktaking.Status), "وضعیت انبارگردانی"); Count(null, "تعداد"); }
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "انبارگردانی به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblStocktaking.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblStocktakingitemUiDefinitions : CRUDDefinition<SqlTblStocktakingitem>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblStocktakingitem.Id), nameof(SqlTblStocktakingitem.Stocktakingid), nameof(SqlTblStocktakingitem.Shopid), nameof(SqlTblStocktakingitem.Productid), nameof(SqlTblStocktakingitem.Initialquantity), nameof(SqlTblStocktakingitem.Finalquantity), nameof(SqlTblStocktakingitem.Showinsecondunit), nameof(SqlTblStocktakingitem.Unitcost), nameof(SqlTblStocktakingitem.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblStocktakingitem.Stocktakingid), nameof(SqlTblStocktakingitem.Shopid), nameof(SqlTblStocktakingitem.Productid), nameof(SqlTblStocktakingitem.Initialquantity), nameof(SqlTblStocktakingitem.Finalquantity), nameof(SqlTblStocktakingitem.Showinsecondunit), nameof(SqlTblStocktakingitem.Unitcost), nameof(SqlTblStocktakingitem.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد آیتم انبارگردانی";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "آیتم انبارگردانی به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblStocktakingitem.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblTagUiDefinitions : CRUDDefinition<SqlTblTag>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblTag.Id), nameof(SqlTblTag.Shopid), nameof(SqlTblTag.Value), nameof(SqlTblTag.Entitytype), nameof(SqlTblTag.Description), nameof(SqlTblTag.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblTag.Shopid), nameof(SqlTblTag.Value), nameof(SqlTblTag.Entitytype), nameof(SqlTblTag.Description), nameof(SqlTblTag.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد برچسب";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "برچسب به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblTag.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblTaxpayerportalconfigUiDefinitions : CRUDDefinition<SqlTblTaxpayerportalconfig>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblTaxpayerportalconfig.Configid), nameof(SqlTblTaxpayerportalconfig.Shopid), nameof(SqlTblTaxpayerportalconfig.Istaxpayerportalenabled), nameof(SqlTblTaxpayerportalconfig.Taxmemoryuniqueid), nameof(SqlTblTaxpayerportalconfig.Privatekey), nameof(SqlTblTaxpayerportalconfig.Digitalsignaturecertificate), nameof(SqlTblTaxpayerportalconfig.Invoiceformat), nameof(SqlTblTaxpayerportalconfig.Invoicetype), nameof(SqlTblTaxpayerportalconfig.TenantId));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblTaxpayerportalconfig.Shopid), nameof(SqlTblTaxpayerportalconfig.Istaxpayerportalenabled), nameof(SqlTblTaxpayerportalconfig.Taxmemoryuniqueid), nameof(SqlTblTaxpayerportalconfig.Privatekey), nameof(SqlTblTaxpayerportalconfig.Digitalsignaturecertificate), nameof(SqlTblTaxpayerportalconfig.Invoiceformat), nameof(SqlTblTaxpayerportalconfig.Invoicetype), nameof(SqlTblTaxpayerportalconfig.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد تنظیمات سامانه مودیان";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تنظیمات سامانه مودیان به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblTaxpayerportalconfig.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblTransferUiDefinitions : CRUDDefinition<SqlTblTransfer>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblTransfer.Transferid), nameof(SqlTblTransfer.Shopid), nameof(SqlTblTransfer.Fiscalperiodid), nameof(SqlTblTransfer.Projectid), nameof(SqlTblTransfer.Description), nameof(SqlTblTransfer.Totalamount), nameof(SqlTblTransfer.Datetime), nameof(SqlTblTransfer.Currency), nameof(SqlTblTransfer.Exchangerate));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblTransfer.Shopid), nameof(SqlTblTransfer.Fiscalperiodid), nameof(SqlTblTransfer.Projectid), nameof(SqlTblTransfer.Description), nameof(SqlTblTransfer.Totalamount), nameof(SqlTblTransfer.Datetime), nameof(SqlTblTransfer.Currency), nameof(SqlTblTransfer.Exchangerate), nameof(SqlTblTransfer.Transfertype), nameof(SqlTblTransfer.Referencetype), nameof(SqlTblTransfer.Referenceid), nameof(SqlTblTransfer.Number), nameof(SqlTblTransfer.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد انتقال";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "انتقال به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblTransfer.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblTransferitemUiDefinitions : CRUDDefinition<SqlTblTransferitem>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblTransferitem.Transferitemid), nameof(SqlTblTransferitem.Transferid), nameof(SqlTblTransferitem.Shopid), nameof(SqlTblTransferitem.Transfertype), nameof(SqlTblTransferitem.Isdestination), nameof(SqlTblTransferitem.Amount), nameof(SqlTblTransferitem.Commission), nameof(SqlTblTransferitem.Description), nameof(SqlTblTransferitem.Accountid));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblTransferitem.Transferid), nameof(SqlTblTransferitem.Shopid), nameof(SqlTblTransferitem.Transfertype), nameof(SqlTblTransferitem.Isdestination), nameof(SqlTblTransferitem.Amount), nameof(SqlTblTransferitem.Commission), nameof(SqlTblTransferitem.Description), nameof(SqlTblTransferitem.Accountid), nameof(SqlTblTransferitem.Entityid), nameof(SqlTblTransferitem.Checkid), nameof(SqlTblTransferitem.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد آیتم انتقال";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "آیتم انتقال به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblTransferitem.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblUploadfileUiDefinitions : CRUDDefinition<SqlTblUploadfile>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblUploadfile.Id), nameof(SqlTblUploadfile.Originalname), nameof(SqlTblUploadfile.Name), nameof(SqlTblUploadfile.Extension), nameof(SqlTblUploadfile.Size), nameof(SqlTblUploadfile.Uid), nameof(SqlTblUploadfile.Relativeurl), nameof(SqlTblUploadfile.Filetype), nameof(SqlTblUploadfile.Createtime));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblUploadfile.Originalname), nameof(SqlTblUploadfile.Name), nameof(SqlTblUploadfile.Extension), nameof(SqlTblUploadfile.Size), nameof(SqlTblUploadfile.Uid), nameof(SqlTblUploadfile.Relativeurl), nameof(SqlTblUploadfile.Filetype), nameof(SqlTblUploadfile.Createtime), nameof(SqlTblUploadfile.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد بارگذاری فایل‌ها";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlTblUseractionlogUiDefinitions : CRUDDefinition<SqlTblUseractionlog>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblUseractionlog.Id), nameof(SqlTblUseractionlog.Shopid), nameof(SqlTblUseractionlog.Userid), nameof(SqlTblUseractionlog.Actiontype), nameof(SqlTblUseractionlog.Entitytype), nameof(SqlTblUseractionlog.Entityid), nameof(SqlTblUseractionlog.Details), nameof(SqlTblUseractionlog.Datetime), nameof(SqlTblUseractionlog.Parentid));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblUseractionlog.Shopid), nameof(SqlTblUseractionlog.Userid), nameof(SqlTblUseractionlog.Actiontype), nameof(SqlTblUseractionlog.Entitytype), nameof(SqlTblUseractionlog.Entityid), nameof(SqlTblUseractionlog.Details), nameof(SqlTblUseractionlog.Datetime), nameof(SqlTblUseractionlog.Parentid), nameof(SqlTblUseractionlog.TenantId));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد تاریخچه عملیات کاربران";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تاریخچه عملیات کاربران به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblUseractionlog.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlTblWarehouseUiDefinitions : CRUDDefinition<SqlTblWarehouse>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlTblWarehouse.Id), nameof(SqlTblWarehouse.Name), nameof(SqlTblWarehouse.Shopid), nameof(SqlTblWarehouse.Isdefault), nameof(SqlTblWarehouse.TenantId), nameof(SqlTblWarehouse.Isenabled), nameof(SqlTblWarehouse.Detailaccountid), nameof(SqlTblWarehouse.Description), nameof(SqlTblWarehouse.Personid));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlTblWarehouse.Name), nameof(SqlTblWarehouse.Shopid), nameof(SqlTblWarehouse.Isdefault), nameof(SqlTblWarehouse.TenantId), nameof(SqlTblWarehouse.Isenabled), nameof(SqlTblWarehouse.Detailaccountid), nameof(SqlTblWarehouse.Description), nameof(SqlTblWarehouse.Personid), nameof(SqlTblWarehouse.Phone), nameof(SqlTblWarehouse.Address));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد انبار";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "انبار به تفکیک شناسه مغازه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlTblWarehouse.Shopid), "شناسه مغازه"); Count(null, "تعداد"); }
        }
    }
}

public sealed class SqlViewAccountbalanceUiDefinitions : CRUDDefinition<SqlViewAccountbalance>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlViewAccountbalance.Documentdate), nameof(SqlViewAccountbalance.Accountid), nameof(SqlViewAccountbalance.Documenttypeid), nameof(SqlViewAccountbalance.Totaldebit), nameof(SqlViewAccountbalance.Totalcredit), nameof(SqlViewAccountbalance.Cnt));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlViewAccountbalance.Documentdate), nameof(SqlViewAccountbalance.Accountid), nameof(SqlViewAccountbalance.Documenttypeid), nameof(SqlViewAccountbalance.Totaldebit), nameof(SqlViewAccountbalance.Totalcredit), nameof(SqlViewAccountbalance.Cnt));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد حساب مانده";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlViewDetailaccountbalanceUiDefinitions : CRUDDefinition<SqlViewDetailaccountbalance>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlViewDetailaccountbalance.Accountid), nameof(SqlViewDetailaccountbalance.Detailaccountid), nameof(SqlViewDetailaccountbalance.Totaldebit), nameof(SqlViewDetailaccountbalance.Totalcredit), nameof(SqlViewDetailaccountbalance.Cnt));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlViewDetailaccountbalance.Accountid), nameof(SqlViewDetailaccountbalance.Detailaccountid), nameof(SqlViewDetailaccountbalance.Totaldebit), nameof(SqlViewDetailaccountbalance.Totalcredit), nameof(SqlViewDetailaccountbalance.Cnt));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد تفصیلی حساب مانده";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
    }
}

public sealed class SqlVwIntegrationdashboardUiDefinitions : CRUDDefinition<SqlVwIntegrationdashboard>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    
    protected override void Forms() => DefineCRUDForms("RL");
    protected override void IndexFormViewModel() => AddColumns(nameof(SqlVwIntegrationdashboard.Shopid), nameof(SqlVwIntegrationdashboard.Tenantid), nameof(SqlVwIntegrationdashboard.Provider), nameof(SqlVwIntegrationdashboard.Displayname), nameof(SqlVwIntegrationdashboard.Isenabled), nameof(SqlVwIntegrationdashboard.Lastsyncatutc), nameof(SqlVwIntegrationdashboard.Mappingcount), nameof(SqlVwIntegrationdashboard.Lastrunatutc), nameof(SqlVwIntegrationdashboard.Failedruns));
    protected override void CUDFormsViewModel() => AddFields(nameof(SqlVwIntegrationdashboard.Shopid), nameof(SqlVwIntegrationdashboard.Tenantid), nameof(SqlVwIntegrationdashboard.Provider), nameof(SqlVwIntegrationdashboard.Displayname), nameof(SqlVwIntegrationdashboard.Isenabled), nameof(SqlVwIntegrationdashboard.Lastsyncatutc), nameof(SqlVwIntegrationdashboard.Mappingcount), nameof(SqlVwIntegrationdashboard.Lastrunatutc), nameof(SqlVwIntegrationdashboard.Failedruns));
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        public class CountConfig : ChartConfigDefinition
        {
            public CountConfig() : base(ChartType.MetricBox) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "تعداد یکسان‌سازی داشبورد";
            protected override void DefineGroupBy() => Count(null, "تعداد");
        }
        public class ByProviderConfig : ChartConfigDefinition
        {
            public ByProviderConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "یکسان‌سازی داشبورد به تفکیک پلتفرم";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlVwIntegrationdashboard.Provider), "پلتفرم"); Count(null, "تعداد"); }
        }
        public class ByShopidConfig : ChartConfigDefinition
        {
            public ByShopidConfig() : base(ChartType.Bar) { }
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override string Name => "یکسان‌سازی داشبورد به تفکیک مغازه شناسه";
            protected override void DefineGroupBy() { GroupBy(nameof(SqlVwIntegrationdashboard.Shopid), "مغازه شناسه"); Count(null, "تعداد"); }
        }
    }
}

