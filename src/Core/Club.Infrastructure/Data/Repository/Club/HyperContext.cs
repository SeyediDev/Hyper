using Neo.Domain.Entities.Common;
using Neo.Domain.Repository;
using Neo.Infrastructure.Data.Repository.Ef;
using Hyper.Domain.Entities.Common;
using Hyper.Domain.Entities.CallCenter;
using Hyper.Domain.Entities.Events;
using Hyper.Domain.Entities.Products;
using Microsoft.EntityFrameworkCore;
using User = Hyper.Domain.Entities.Common.User;
using Hyper.Domain.Entities.Events.Data;
using Hyper.Domain.Entities.Metrics.Data;
using Hyper.Domain.Entities.Promotions.Surveys.Data;
using Hyper.Domain.Entities.Promotions.Plans;
using Hyper.Domain.Entities.Promotions.Plans.Data;
using Hyper.Domain.Entities.Tenants.Data;
using ChannelEventChannel = Hyper.Domain.Entities.Channels.EventChannel;
using ChannelEventChannelValidEvent = Hyper.Domain.Entities.Channels.EventChannelValidEvent;
using ChannelEventChannelValidIp = Hyper.Domain.Entities.Channels.EventChannelValidIp;
using EventLogData = Hyper.Domain.Entities.Events.Data.EventLog;
using Hyper.Domain.Entities.Lotteries;

namespace Hyper.Infrastructure.Data.Repository.Hyper;

public abstract partial class HyperContext<TContext>(DbContextOptions<TContext> options)
    : EfDbContext<TContext>(options), IUnitOfWork
    where TContext : DbContext
{
    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<DocumentType> DocumentTypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<CultureTerm> CultureTerms { get; set; }

    public virtual DbSet<Language> Language { get; set; }

    //public virtual DbSet<FaqType> Type { get; set; }

    public virtual DbSet<Faq> Faq { get; set; }
    public virtual DbSet<Help> Help { get; set; }

    // Survey entities
    public virtual DbSet<Survey> Surveys { get; set; }
    public virtual DbSet<SurveyItem> SurveyItems { get; set; }
    public virtual DbSet<SurveyParticipation> SurveyParticipations { get; set; }

    // Feedback entities
    public virtual DbSet<CustomerFeedback> CustomerFeedbacks { get; set; }
    public virtual DbSet<FeedbackComment> FeedbackComments { get; set; }
    public virtual DbSet<FeedbackAttachment> FeedbackAttachments { get; set; }
    public virtual DbSet<FeedbackLike> FeedbackLikes { get; set; }

    // Forum entities
    public virtual DbSet<ForumTopic> ForumTopics { get; set; }
    public virtual DbSet<ForumPost> ForumPosts { get; set; }
    public virtual DbSet<ForumTopicLike> ForumTopicLikes { get; set; }
    public virtual DbSet<ForumPostLike> ForumPostLikes { get; set; }

    // Plan entities
    public virtual DbSet<Plan> Plans { get; set; }
    public virtual DbSet<CustomerPlan> CustomerPlans { get; set; }

    // ScoringRule entities - Removed: Migrated to Promotion entities
    // public virtual DbSet<ScoringRule> ScoringRules { get; set; }
    // public virtual DbSet<ScoringRuleAction> ScoringRuleActions { get; set; }
    // public virtual DbSet<ScoringRuleTriggerCondition> ScoringRuleTriggerConditions { get; set; }

    // CallCenter entities
    public virtual DbSet<InteractionType> InteractionTypes { get; set; }
    public virtual DbSet<InteractionOutcome> InteractionOutcomes { get; set; }
    public virtual DbSet<CustomerInteraction> CustomerInteractions { get; set; }
    public virtual DbSet<InteractionFollowUp> InteractionFollowUps { get; set; }
    public virtual DbSet<InteractionAttachment> InteractionAttachments { get; set; }

    // Promotion / Lottery entities
    public virtual DbSet<Lottery> Lotteries { get; set; }
    public virtual DbSet<LotteryReward> LotteryRewards { get; set; }
    public virtual DbSet<LotteryParticipant> LotteryParticipants { get; set; }

    // Customer/Product metrics
    public virtual DbSet<CustomerProductMetrics> CustomerProductMetrics { get; set; }

    // Product entities
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    // Event entities
    public virtual DbSet<EventType> EventTypes { get; set; }
    public virtual DbSet<EventLogData> EventLogs { get; set; }
    public virtual DbSet<EventLogAttribute> EventLogParameters { get; set; }
    public virtual DbSet<ChannelEventChannel> EventChannels { get; set; }
    public virtual DbSet<ChannelEventChannelValidEvent> EventChannelValidEvents { get; set; }
    public virtual DbSet<ChannelEventChannelValidIp> EventChannelValidIps { get; set; }

    // Outbox entities
    public virtual DbSet<OutboxMessage> OutboxMessages { get; set; }

    // Tenant Attribute Aggregation entities
    public virtual DbSet<TenantAttributeDailyAggregation> TenantAttributeDailyAggregations { get; set; }
    public virtual DbSet<TenantAttributeMonthlyAggregationPersian> TenantAttributeMonthlyAggregationPersians { get; set; }
    public virtual DbSet<TenantAttributeMonthlyAggregationGregorian> TenantAttributeMonthlyAggregationGregorians { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply all entity configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HyperContext<>).Assembly);
    }
}
